using Ascpixi.Wakatime.FLStudio.Native;
using Ascpixi.Wakatime.FLStudio.Native.Enumerations;

namespace Ascpixi.Wakatime.FLStudio;

public static class MessageLoop
{
    /// <summary>
    /// Creates a new message loop thread.
    /// </summary>
    public static void Spawn() => new Thread(Run).Start();
    
    /// <summary>
    /// Runs the message loop in the calling thread. This method will only return
    /// once a WM_QUIT message is received.
    /// </summary>
    public static void Run()
    {
        Log.Info("Message loop started.");
        
        while (User32.GetMessage(out var msg, default, 0, 0)) {
            if (msg.Message == WindowMessageKind.WM_QUIT) {
                Log.Info("WM_QUIT received - terminating message loop.");
                break;
            }

            User32.TranslateMessage(ref msg);
            User32.DispatchMessage(ref msg);
        }
    }
}