using System.Runtime.InteropServices;
using Ascpixi.Wakatime.FLStudio.Native.Enumerations;

namespace Ascpixi.Wakatime.FLStudio.Native;

/// <summary>
/// Represents a RECT structure.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct Rect
{
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;

    public int Width => Right - Left;
    public int Height => Bottom - Top;
}

public record struct WindowHandle(nuint Value);
public record struct ModuleHandle(nuint Value);
public record struct ProcessHandle(nuint Value);
public record struct EventHookHandle(nuint Value);

public record struct DCHandle(nuint Value);

public record struct Atom(ushort Value);

[StructLayout(LayoutKind.Sequential)]
public struct MemoryBasicInformation
{
    public nuint BaseAddress;
    public nuint AllocationBase;
    public uint AllocationProtect;
    public ushort PartitionId;
    public nuint RegionSize;
    public uint State;
    public uint Protect;
    public uint Type;
}

[StructLayout(LayoutKind.Sequential)]
public record struct Point(int X, int Y);

[StructLayout(LayoutKind.Sequential)]
public struct WindowMessage
{
    public WindowHandle Hwnd;
    public WindowMessageKind Message;
    public nint WParam;
    public nint LParam;
    public uint Time;
    public Point Point;
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct LastInputInfo()
{
    public readonly uint CbSize = (uint)sizeof(LastInputInfo);
    public uint DwTime;
}

public delegate nint WndProc(WindowHandle hWnd, WindowMessageKind msg, nint wParam, nint lParam);

[StructLayout(LayoutKind.Sequential)]
public struct WndClassEx()
{
    public uint Size = (uint)Marshal.SizeOf<WndClassEx>();
    public WindowClassStyle Style;
    [MarshalAs(UnmanagedType.FunctionPtr)] public WndProc WndProc;
    public int ClsExtra;
    public int WndExtra;
    public ModuleHandle Instance;
    public nint Icon;
    public nint Cursor;
    public nint Background;
    [MarshalAs(UnmanagedType.LPWStr)] public string? MenuName;
    [MarshalAs(UnmanagedType.LPWStr)] public string? ClassName;
    public nint IconSm;
}