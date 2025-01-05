using System.Diagnostics;
using System.Text;
using Ascpixi.Wakatime.FLStudio.Native;

namespace Ascpixi.Wakatime.FLStudio;

public static class Windowing
{
    /// <summary>
    /// Gets a window belonging to the given process of the given class. Returns "default"
    /// if no windows match.
    /// </summary>
    public static WindowHandle GetProcessWindowByClass(Process process, string className, out int threadId)
    {
        var classBuffer = new StringBuilder(className.Length + 4);
        
        WindowHandle found = default;
        int foundThread = -1;
        
        foreach (ProcessThread thread in process.Threads) {
            User32.EnumThreadWindows(
                thread.Id,
                (hwnd, _) => {
                    if (User32.GetClassName(hwnd, classBuffer, 32) != className.Length)
                        return true;

                    found = hwnd;
                    foundThread = thread.Id;
                    return false;
                },
                default
            );
        }

        threadId = foundThread;
        return found;
    }

    /// <summary>
    /// Checks if the window with the handle of 'target' is of the given class.
    /// </summary>
    public static bool CheckWindowClass(WindowHandle target, string className)
    {
        var sb = new StringBuilder(className.Length + 1);
        if (User32.GetClassName(target, sb, sb.Capacity) != className.Length)
            return false;

        return sb.ToString() == className;
    } 
}