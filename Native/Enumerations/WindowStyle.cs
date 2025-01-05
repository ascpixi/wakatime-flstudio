namespace Ascpixi.Wakatime.FLStudio.Native.Enumerations;

[Flags]
public enum WindowStyle : uint
{
    /// <summary>
    /// The window has a thin-line border.
    /// </summary>
    Border = 0x00800000,

    /// <summary>
    /// The window has a title bar (includes the WS_BORDER style).
    /// </summary>
    Caption = 0x00C00000,

    /// <summary>
    /// The window is a child window. A window with this style cannot have a menu bar.
    /// </summary>
    Child = 0x40000000,

    /// <summary>
    /// Same as the WS_CHILD style.
    /// </summary>
    ChildWindow = 0x40000000,

    /// <summary>
    /// Excludes the area occupied by child windows when drawing occurs within the parent window.
    /// </summary>
    ClipChildren = 0x02000000,

    /// <summary>
    /// Clips child windows relative to each other.
    /// </summary>
    ClipSiblings = 0x04000000,

    /// <summary>
    /// The window is initially disabled. A disabled window cannot receive input from the user.
    /// </summary>
    Disabled = 0x08000000,

    /// <summary>
    /// The window has a border of a style typically used with dialog boxes. It cannot have a title bar.
    /// </summary>
    DlgFrame = 0x00400000,

    /// <summary>
    /// The window is the first control in a group of controls for dialog box navigation.
    /// </summary>
    Group = 0x00020000,

    /// <summary>
    /// The window has a horizontal scroll bar.
    /// </summary>
    HScroll = 0x00100000,

    /// <summary>
    /// The window is initially minimized. Same as the WS_MINIMIZE style.
    /// </summary>
    Iconic = 0x20000000,

    /// <summary>
    /// The window is initially maximized.
    /// </summary>
    Maximize = 0x01000000,

    /// <summary>
    /// The window has a maximize button. Cannot be combined with the WS_EX_CONTEXTHELP style.
    /// </summary>
    MaximizeBox = 0x00010000,

    /// <summary>
    /// The window is initially minimized. Same as the WS_ICONIC style.
    /// </summary>
    Minimize = 0x20000000,

    /// <summary>
    /// The window has a minimize button. Cannot be combined with the WS_EX_CONTEXTHELP style.
    /// </summary>
    MinimizeBox = 0x00020000,

    /// <summary>
    /// The window is an overlapped window. An overlapped window has a title bar and a border.
    /// </summary>
    Overlapped = 0x00000000,

    /// <summary>
    /// The window is an overlapped window with additional styles.
    /// </summary>
    OverlappedWindow = 0x00000000 | 0x00C00000 | 0x00080000 | 0x00040000 | 0x00020000 | 0x00010000,

    /// <summary>
    /// The window is a pop-up window. This style cannot be used with the WS_CHILD style.
    /// </summary>
    Popup = 0x80000000,

    /// <summary>
    /// The window is a pop-up window with additional styles to make the menu visible.
    /// </summary>
    PopupWindow = 0x80000000 | 0x00800000 | 0x00080000,

    /// <summary>
    /// The window has a sizing border. Same as the WS_THICKFRAME style.
    /// </summary>
    SizeBox = 0x00040000,

    /// <summary>
    /// The window has a window menu on its title bar. The WS_CAPTION style must also be specified.
    /// </summary>
    SysMenu = 0x00080000,

    /// <summary>
    /// The window is a control that can receive keyboard focus when the user presses the TAB key.
    /// </summary>
    TabStop = 0x00010000,

    /// <summary>
    /// The window has a sizing border. Same as the WS_SIZEBOX style.
    /// </summary>
    ThickFrame = 0x00040000,

    /// <summary>
    /// The window is an overlapped window. An overlapped window has a title bar and a border.
    /// </summary>
    Tiled = 0x00000000,

    /// <summary>
    /// The window is an overlapped window with additional styles.
    /// </summary>
    TiledWindow = 0x00000000 | 0x00C00000 | 0x00080000 | 0x00040000 | 0x00020000 | 0x00010000,

    /// <summary>
    /// The window is initially visible. This style can be turned on and off by using the ShowWindow or SetWindowPos function.
    /// </summary>
    Visible = 0x10000000,

    /// <summary>
    /// The window has a vertical scroll bar.
    /// </summary>
    VScroll = 0x00200000
}
