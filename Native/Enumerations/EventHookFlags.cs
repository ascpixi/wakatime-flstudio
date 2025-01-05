namespace Ascpixi.Wakatime.FLStudio.Native.Enumerations;

[Flags]
public enum EventHookFlags : uint
{
    OutOfContext = 0,
    SkipOwnThread = 1,
    SkipOwnProcess = 2,
    InContext = 4
}