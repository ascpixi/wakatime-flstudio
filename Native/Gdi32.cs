using System.Runtime.InteropServices;

namespace Ascpixi.Wakatime.FLStudio.Native;

public static partial class Gdi32
{
    [LibraryImport("gdi32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool RoundRect(
        DCHandle hdc,
        int leftRect,
        int topRect,
        int rightRect,
        int bottomRect,
        int width,
        int height
    );
}