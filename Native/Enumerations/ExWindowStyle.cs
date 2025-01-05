namespace Ascpixi.Wakatime.FLStudio.Native.Enumerations;

[Flags]
public enum ExWindowStyle : uint
{
    /// <summary>
    /// The window accepts drag-drop files.
    /// </summary>
    AcceptFiles = 0x00000010,

    /// <summary>
    /// Forces a top-level window onto the taskbar when the window is visible.
    /// </summary>
    AppWindow = 0x00040000,

    /// <summary>
    /// The window has a border with a sunken edge.
    /// </summary>
    ClientEdge = 0x00000200,

    /// <summary>
    /// Paints all descendants of a window in bottom-to-top painting order using double-buffering.
    /// </summary>
    Composited = 0x02000000,

    /// <summary>
    /// The title bar of the window includes a question mark for context help.
    /// </summary>
    ContextHelp = 0x00000400,

    /// <summary>
    /// The window itself contains child windows that participate in dialog box navigation.
    /// </summary>
    ControlParent = 0x00010000,

    /// <summary>
    /// The window has a double border, optionally with a title bar.
    /// </summary>
    DialogModalFrame = 0x00000001,

    /// <summary>
    /// The window is a layered window, which can have translucency or transparency effects.
    /// </summary>
    Layered = 0x00080000,

    /// <summary>
    /// If the shell language is right-to-left, the horizontal origin is on the right edge.
    /// </summary>
    LayoutRtl = 0x00400000,

    /// <summary>
    /// The window has generic left-aligned properties.
    /// </summary>
    Left = 0x00000000,

    /// <summary>
    /// If the shell language is right-to-left, the vertical scrollbar is to the left of the client area.
    /// </summary>
    LeftScrollbar = 0x00004000,

    /// <summary>
    /// The window text is displayed using left-to-right reading-order properties.
    /// </summary>
    LtrReading = 0x00000000,

    /// <summary>
    /// The window is a MDI (Multiple Document Interface) child window.
    /// </summary>
    MdiChild = 0x00000040,

    /// <summary>
    /// A top-level window does not become the foreground window when the user clicks it.
    /// </summary>
    NoActivate = 0x08000000,

    /// <summary>
    /// The window does not pass its window layout to its child windows.
    /// </summary>
    NoInheritLayout = 0x00100000,

    /// <summary>
    /// The child window does not send the WM_PARENTNOTIFY message to its parent window.
    /// </summary>
    NoParentNotify = 0x00000004,

    /// <summary>
    /// The window does not render to a redirection surface.
    /// </summary>
    NoRedirectionBitmap = 0x00200000,

    /// <summary>
    /// The window is an overlapped window (combined WS_EX_WINDOWEDGE and WS_EX_CLIENTEDGE).
    /// </summary>
    OverlappedWindow = 0x00000100 | 0x00000200,

    /// <summary>
    /// The window is a palette window, a modeless dialog box presenting commands.
    /// </summary>
    PaletteWindow = 0x00000100 | 0x00000080 | 0x00000008,

    /// <summary>
    /// The window has generic right-aligned properties.
    /// </summary>
    Right = 0x00001000,

    /// <summary>
    /// The vertical scroll bar is to the right of the client area.
    /// </summary>
    RightScrollbar = 0x00000000,

    /// <summary>
    /// The window text is displayed using right-to-left reading-order properties.
    /// </summary>
    RtlReading = 0x00002000,

    /// <summary>
    /// The window has a three-dimensional border style intended for items that do not accept user input.
    /// </summary>
    StaticEdge = 0x00020000,

    /// <summary>
    /// The window is intended to be used as a floating toolbar.
    /// </summary>
    ToolWindow = 0x00000080,

    /// <summary>
    /// The window should be placed above all non-topmost windows and stay above them.
    /// </summary>
    Topmost = 0x00000008,

    /// <summary>
    /// The window should not be painted until siblings beneath it have been painted.
    /// </summary>
    Transparent = 0x00000020,

    /// <summary>
    /// The window has a border with a raised edge.
    /// </summary>
    WindowEdge = 0x00000100
}
