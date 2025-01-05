using System.Diagnostics;
using Ascpixi.Wakatime.FLStudio;
using Ascpixi.Wakatime.FLStudio.Native;
using Ascpixi.Wakatime.FLStudio.Native.Enumerations;

AppDomain.CurrentDomain.UnhandledException += (_, ev) => {
    if (ev.ExceptionObject is not Exception ex)
        return;

    Log.Error("Fatal error! If you feel this is a bug, you can report it over at https://github.com/ascpixi/wakatime-flstudio/issues.\n\n" + ex);

    User32.MessageBox(
        default,
        (
            "Oops, Wakatime for FL Studio has encountered a fatal error!\n\n"
            + ex + "\n\n"
            + $"You can find a log file over at {Log.FilePath}. If you feel that this is "
            + "a bug, feel free to report this issue over at https://github.com/ascpixi/wakatime-flstudio/issues."
        ),
        "Wakatime for FL Studio fatal error (wakatime-flstudio)",
        MsgBoxFlags.IconError | MsgBoxFlags.Ok | MsgBoxFlags.SystemModal
    );
};

var result = WakaTime.TryInitialize();
if (result.IsFailed) {
    User32.MessageBox(
        default,
        $"Wakatime for FL Studio couldn't start. {result.Errors.First().Message}",
        "Couldn't launch Wakatime for FL Studio",
        MsgBoxFlags.IconError | MsgBoxFlags.Ok
    );

    return 1;
}

var knownInstances = new HashSet<(WindowHandle hwnd, uint tid, uint pid)>();

User32.WinEventProc wndCreateProc =  (_, _, hwnd, _, _, _, _) => {
    if (!Windowing.CheckWindowClass(hwnd, FLInstance.MainWindowClass))
        return;

    uint tid = User32.GetWindowThreadProcessId(hwnd, out uint pid);
        
    // Verify that we don't already have an instance for this PID (for some reason,
    // this can get called multiple times for the same exact combination of HWND/PID/TID)
    if (knownInstances.Contains((hwnd, tid, pid)))
        return;
    
    // FL creates an extremely short-lived window with the main class, then immediately
    // closes it. Wait a bit to see if this window is ephemeral.
    Thread.Sleep(250);
    if (!User32.IsWindow(hwnd))
        return;
        
    Log.Info($"Creating FLInstance for HWND 0x{hwnd.Value:X}, PID {pid}, TID {tid}.");
        
    var fl = new FLInstance(hwnd, pid, tid);
    fl.Start();
    knownInstances.Add((hwnd, tid, pid));
};

User32.SetWinEventHook(
    WindowEvent.ObjectCreate, WindowEvent.ObjectCreate,
    default,
    wndCreateProc,
    0, 0,
    EventHookFlags.OutOfContext
);
    
// Handle FL Studio windows opened before the tracker was started.
foreach (var process in Process.GetProcessesByName("FL64").Concat(Process.GetProcessesByName("FL64 (Scaled)"))) {
    var hwnd = Windowing.GetProcessWindowByClass(process, FLInstance.MainWindowClass, out int tid);
    if (hwnd == default) {
        Log.Warning($"Found an existing FL Studio process w/ PID {process.Id}, but it doesn't have a recognized window.");
        continue;
    }

    Log.Info($"Creating FLInstance for existing FL, HWND 0x{hwnd.Value:X}, PID {process.Id}, TID {tid}.");
        
    var fl = new FLInstance(hwnd, (uint)process.Id, (uint)tid);
    fl.Start();
    knownInstances.Add((hwnd, (uint)tid, (uint)process.Id));
}

MessageLoop.Run();

GC.KeepAlive(wndCreateProc); // This is crucial - without this line, Win32 will try to call into GC'd data!!!
return 0;
