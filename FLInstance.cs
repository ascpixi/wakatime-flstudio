using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

using Ascpixi.Wakatime.FLStudio.Native;
using Ascpixi.Wakatime.FLStudio.Native.Enumerations;

namespace Ascpixi.Wakatime.FLStudio;

// ReSharper disable NotAccessedField.Local

/// <summary>
/// Represents an FL Studio instance.
/// </summary>
public class FLInstance : IDisposable
{
    readonly Lock threadLock = new();
    
    bool disposed;
    Process process;
    Overlay? overlay;
    ProcessHandle memHandle;
    WindowHandle mainWindow;
    FileSystemWatcher? fileWatcher;
    StringBuilder titleBuffer = new(256);
    CancellationTokenSource cancel = new();
    DateTime lastHeartbeat, lastWrite;
    bool isForeground;
    uint windowThread;

    Thread timerThread;
    
    string? prevProjectName;
    string? cachedProjectPath;

    EventHookHandle titleChangeHook, fgChangeHook;
    User32.WinEventProc? titleChangeProc, fgChangeProc; // These are kept so that the GC doesn't free them

    // When scanning for the project path in FL Studio's memory, we first begin from searching
    // in regions closest to this address.
    nuint closestScanAddress = 0x8000000;
    
    /// <summary>
    /// The class of main FL Studio windows.
    /// </summary>
    public const string MainWindowClass = "TFruityLoopsMainForm";
    
    public FLInstance(WindowHandle hwnd, uint processId, uint threadId)
    {
        this.process = Process.GetProcessById((int)processId);
        if (this.process.ProcessName is not "FL64" and not "FL64 (Scaled)")
            throw new Exception($"The provided window belongs to an unknown process {this.process.ProcessName}.");
        
        this.windowThread = threadId;
        this.memHandle = Kernel32.OpenProcess(
            ProcessAccess.QueryInformation | ProcessAccess.VmRead,
            false,
            processId
        );

        if (this.memHandle == default) {
            Log.Win32Error($"Could not open a memory process handle to PID {process.Id}.");
            throw new Win32Exception();
        }

        this.mainWindow = hwnd;
        if (this.mainWindow == default)
            throw new Exception("Could not determine the main window.");
        
        Log.Info($"FL Studio window found with HWND 0x{this.mainWindow.Value:X}, TID {windowThread}.");
        
        // When the FL Studio process exits, dispose everything.
        process.WaitForExitAsync(cancel.Token).ContinueWith(_ => this.Dispose());
        
        this.timerThread = new Thread(() => {
            while (!disposed) {
                Thread.Sleep(2 * 60 * 1000);

                lock (threadLock) {
                    if (disposed || !isForeground || ProjectName == "")
                        continue;
                
                    if (DateTime.Now - this.lastHeartbeat < TimeSpan.FromMinutes(2))
                        continue;
                
                    SendHeartbeat(false);
                }
            }
        });
    }

    /// <summary>
    /// Starts monitoring the FL Studio instance for heartbeats.
    /// </summary>
    public void Start()
    {
        ObjectDisposedException.ThrowIf(disposed, this);
        
        this.overlay = new Overlay(this.mainWindow);
        UpdateTitle();
        
        // Listen for changes of the main window's title.
        this.titleChangeHook = User32.SetWinEventHook(
            WindowEvent.ObjectNameChange, WindowEvent.ObjectNameChange,
            default,
            this.titleChangeProc = (_, _, hwnd, _, _, _, _) => {
                if (hwnd != this.mainWindow)
                    return;

                new Thread(() => {
                    lock (threadLock) UpdateTitle();
                }).Start();
            },
            (uint)process.Id,
            windowThread,
            EventHookFlags.OutOfContext
        );

        if (this.titleChangeHook == default) {
            Log.Win32Warning("Could not listen for title change events.");
        }
        
        // We send heartbeats as soon as the foreground window changes to FL Studio,
        // and so we need to listen on those events as well.
        this.fgChangeHook = User32.SetWinEventHook(
            WindowEvent.SystemForeground, WindowEvent.SystemForeground,
            default,
            this.fgChangeProc = (_, _, hwnd, _, _, _, _) => {
                if (hwnd != this.mainWindow) {
                    isForeground = false;
                    return;
                }

                isForeground = true;

                // This is called from unmanaged code. We create a new managed
                // thread to actually send the heartbeat.
                new Thread(() => {
                    lock (threadLock) SendHeartbeat(false);
                }).Start();
            },
            0, 0, // listen globally
            EventHookFlags.OutOfContext
        );
        
        this.timerThread.Start();
        
        Log.Info($"Started monitoring heartbeats for PID {this.process.Id}, HWND 0x{this.mainWindow.Value:X}");
    }

    /// <summary>
    /// The project name the user is currently working on (including the .flp extension).
    /// This does not contain the full path to the project. Returns an empty string
    /// if the user is working on an untitled, unsaved project.
    /// </summary>
    public string ProjectName { get; private set; } = "";

    /// <summary>
    /// Gets the full path to the currently opened project file. Can only be called if
    /// ProjectName returns a string that's longer than zero characters.
    /// </summary>
    public unsafe string GetProjectPath()
    {
        if (ProjectName.Length == 0)
            throw new Exception("Cannot get the project path when no project is opened.");

        // If we have a cached value, and the project name hasn't changed, we use the
        // previous name.
        if (ProjectName == prevProjectName && cachedProjectPath != null)
            return cachedProjectPath;
        
        Span<char> searchString = stackalloc char[ProjectName.Length + 1];
        searchString[0] = '\\';
        ProjectName.CopyTo(searchString[1..]);
        
        // Project paths are usually stored in 1.25MiB (size 0x140000) R/W regions,
        // always private + commited. The string should be 16-byte aligned.

        const int MemCommit = 0x1000;
        const int MemReadWrite = 0x04;
        const int MemPrivate = 0x20000;
        const int TargetSize = 0x140000;
        
        var regions = new List<MemoryBasicInformation>();
        
        nuint addr = 0;
        while (Kernel32.VirtualQueryEx(memHandle, addr, out var mbi, sizeof(MemoryBasicInformation)) != 0) {
            if (mbi.Type == MemPrivate) {
                regions.Add(mbi);
            }
            
            addr = mbi.BaseAddress + mbi.RegionSize;
        }
        
        // The order in which we retrieve and scan the memory region is important - we
        // first try from the regions that are the most likely to contain the paths.
        var sortedRegions = regions
            .Where(x => x.State == MemCommit)
            .OrderBy(x => x.Protect != MemReadWrite)
            .ThenBy(x => x.RegionSize != TargetSize)
            .ThenBy(x => Math.Abs((nint)x.BaseAddress - (nint)closestScanAddress));

        foreach (var mbi in sortedRegions) {
            var region = new char[mbi.RegionSize / 2];

            fixed (char* regionPtr = region) {
                if (!Kernel32.ReadProcessMemory(memHandle, mbi.BaseAddress, regionPtr, (nint)mbi.RegionSize)) {
                    Log.Warning($"Could not scan memory region 0x{mbi.BaseAddress:X} -> ${mbi.BaseAddress + mbi.RegionSize:X} ({mbi.RegionSize} B)");
                    Log.Warning($"  error: {Marshal.GetLastPInvokeErrorMessage()} (ID {Marshal.GetLastWin32Error()})");
                    continue;
                }
            }

            int idx = region.AsSpan().IndexOf(searchString);
            if (idx == -1)
                continue; // no substring found

            int endIdx = idx + searchString.Length;
            
            // Iterate *backwards* from the index until we find a colon (":"), which indicates
            // disks on Windows. We stop at MAX_PATH (260).
            var found = false;
            for (var i = 0; i < 260; i++) {
                idx--;

                if (idx < 0) {
                    Log.Warning("While iterating backwards to find the project path, we've encountered the end of the memory region.");
                    Log.Warning($"  memory region: 0x{mbi.BaseAddress:X} of size {mbi.RegionSize}");
                    break;
                }
                
                if (region[idx] == ':') {
                    found = true;
                    idx--;
                    break;
                }
            }

            if (!found) {
                Log.Warning($"A portion of the project path was found in region 0x{mbi.BaseAddress:X} (size {mbi.RegionSize}), but no drive letter could be found.");
                continue;
            }
            
            Log.Info($"Found project path @ 0x{mbi.BaseAddress}, sized {mbi.RegionSize}, offset 0x{idx * 2L:X}");

            var path = new string(region[idx..endIdx]);
            if (!File.Exists(path)) {
                Log.Warning($"The found project path {path} does not exist! Skipping!");
            }
            else {
                Log.Info($"Path successfully retrieved: {path}");
                
                // We'll begin scanning from the base address of the region we've found the
                // path when we'll need to invalidate it.
                closestScanAddress = mbi.BaseAddress;
                prevProjectName = ProjectName;
                return cachedProjectPath = path;
            }
        }
        
        throw new Exception($"Could not find project path (scanned {regions.Count} regions).");
    }

    void SendHeartbeat(bool isWrite)
    {
        if (ProjectName == "" || IdleDetection.IsIdle())
            return;
        
        WakaTime.EmitHeartbeat(GetProjectPath(), isWrite);
        this.lastHeartbeat = DateTime.Now;

        if (this.overlay != null) {
            this.overlay.Text = WakaTime.GetCategoryTime();
        }
    }
    
    void UpdateTitle()
    {
        this.titleBuffer.Clear();
        User32.GetWindowText(mainWindow, this.titleBuffer, this.titleBuffer.Capacity);

        var title = this.titleBuffer.ToString();
        ProjectName = !title.Contains(" - FL Studio") ? "" : title[..(title.LastIndexOf('-') - 1)];

        if (ProjectName.Length == 0) {
            Log.Info($"PID {this.process.Id}: No (or an unnamed) project loaded.");
            this.overlay!.ShouldShow = false;
        }
        else {
            Log.Info($"PID {this.process.Id}: Project {ProjectName} loaded.");
            this.overlay!.ShouldShow = true;
            
            // We've loaded a different project! Reload the file watcher to the new project.
            if (ProjectName != prevProjectName) {
                CreateFileWatcher();
            }
        }
    }

    void CreateFileWatcher()
    {
        this.fileWatcher?.Dispose();

        string path = GetProjectPath();
        this.fileWatcher = new FileSystemWatcher(Path.GetDirectoryName(path)!) {
            Filter = Path.GetFileName(path),
            NotifyFilter = NotifyFilters.FileName,
            EnableRaisingEvents = true,
            IncludeSubdirectories = false
        };

        // FL Studio first creates a temporary file with a "~" suffix, and then renames
        // it to the actual project name. Thus, we listen to "rename" events rather than
        // "change" ones.
        this.fileWatcher.Renamed += (_, _) => {
            if (DateTime.Now - this.lastWrite < TimeSpan.FromSeconds(1))
                return;

            SendHeartbeat(true);
            this.lastWrite = DateTime.Now;
        };
    }
    
    public void Dispose()
    {
        if (this.disposed)
            return;

        Log.Info($"Disposing FLInstance for PID {this.process.Id}.");
        
        lock (threadLock) {
            this.cancel.Cancel();
            this.process.Dispose();
            this.fileWatcher?.Dispose();
            this.overlay?.Dispose();
        
            if (!Kernel32.CloseHandle(this.memHandle.Value)) {
                Log.Win32Warning($"Could not close memory handle 0x{this.memHandle.Value:X} to FL Studio. Ignoring.");
            }

            // UnhookWinEvent will return FALSE if we are closing a hook on a closed
            // window - that's fine, we don't need to check that.
            if (this.titleChangeHook != default) {
                User32.UnhookWinEvent(this.titleChangeHook);
            }
            
            if (this.fgChangeHook != default) {
                User32.UnhookWinEvent(this.fgChangeHook);
            }
            
            this.disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
