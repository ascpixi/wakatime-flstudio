using Ascpixi.Wakatime.FLStudio.Native;

namespace Ascpixi.Wakatime.FLStudio;

public static class IdleDetection
{
    // The number of milliseconds between the last user input and the current moment
    // in time that have to elapse to consider the user to be idle. For example, if this
    // value is "15000", this means that the user has to not provide any inputs for 15
    // seconds before they're considered idle.
    const uint LastInputThreshold = 15 * 1000;
    
    static Point lastCursorPos;

    /// <summary>
    /// Determines whether the user is currently idle and not interacting with the system.
    /// </summary>
    public static bool IsIdle()
    {
        User32.GetCursorPos(out var cursorPos);
        if (lastCursorPos == cursorPos)
            return true;

        lastCursorPos = cursorPos;
        
        var lastInput = new LastInputInfo();
        User32.GetLastInputInfo(ref lastInput);

        uint delta = Kernel32.GetTickCount() - lastInput.DwTime;
        if (delta > LastInputThreshold)
            return true;
        
        return false;
    }
}