using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Ascpixi.Wakatime.FLStudio;

public static class Log
{
    static readonly Lock fileLock = new();

    static bool canWriteToFile;
    static readonly string dir = Path.Join(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "wakatime-flstudio"
    );

    public static readonly string FilePath = Path.Join(dir, "latest.log");
    
    static Log()
    {
        if (File.Exists(FilePath)) {
            string pathBase = Path.Combine(dir, $"{File.GetCreationTime(FilePath):yyyy-MM-dd}");
            string newPath = pathBase + ".log";

            if (File.Exists(newPath)) {
                var seq = 1;

                do {
                    newPath = $"{pathBase}-{seq++}.log";
                } while (File.Exists(newPath));
            }
            
            Info($"Rotating old log to {newPath}.");
            
            File.Move(FilePath, newPath);
        }
        else {
            Directory.CreateDirectory(dir);
        }

        canWriteToFile = true;
    }

    static void Write(string severity, string msg)
    {
        var data = $"[{DateTime.Now:T}] ({severity}) {msg}";

        if (canWriteToFile) {
            lock (fileLock) {
                File.AppendAllText(FilePath, data + Environment.NewLine);
            }
        }
        
        Debug.WriteLine(data);
        Console.WriteLine(data);
    }

    public static void Info(string msg) => Write("info", msg);

    public static void Warning(string msg) => Write("warn", msg);

    public static void Error(string msg) => Write("error", msg);

    static string CreateWin32Message(string msg)
        => $"{msg} {Marshal.GetLastPInvokeErrorMessage()} (error {Marshal.GetLastWin32Error()})";
    
    /// <summary>
    /// Reports a warning caused by a Win32 call failing.
    /// </summary>
    public static void Win32Warning(string msg)
        => Warning(CreateWin32Message(msg));
    
    /// <summary>
    /// Reports an error caused by a Win32 call failing.
    /// </summary>
    public static void Win32Error(string msg)
        => Error(CreateWin32Message(msg));
}