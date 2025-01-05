using System.Runtime.InteropServices;
using Ascpixi.Wakatime.FLStudio.Native.Enumerations;

namespace Ascpixi.Wakatime.FLStudio.Native;

public static partial class Kernel32
{
    /// <summary>
    /// Retrieves a module handle for the specified module. The module must have been loaded by the calling process.
    /// </summary>
    [LibraryImport("kernel32.dll", StringMarshalling = StringMarshalling.Utf16)]
    public static partial ModuleHandle GetModuleHandle(
        string? lpModuleName
    );
    
    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool CloseHandle(nuint hObject);
    
    [LibraryImport("kernel32.dll", SetLastError = true)]
    public static partial ProcessHandle OpenProcess(
        ProcessAccess dwDesiredAccess,
        [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle,
        uint dwProcessId
    );

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static unsafe partial bool ReadProcessMemory(
        ProcessHandle hProcess,
        nuint lpBaseAddress,
        void* lpBuffer,
        nint nSize,
        [Optional] nint* lpNumberOfBytesRead
    );
    
    [LibraryImport("kernel32.dll")]
    public static partial nuint VirtualQueryEx(
        ProcessHandle hProcess,
        nuint lpAddress,
        out MemoryBasicInformation lpBuffer,
        int dwLength
    );

    /// <summary>
    /// Retrieves the number of milliseconds that have elapsed since the system was started, up to 49.7 days.
    /// </summary>
    [LibraryImport("kernel32.dll")]
    public static partial uint GetTickCount();
}