namespace Ascpixi.Wakatime.FLStudio.Native.Enumerations;

/// <summary>
/// Specifies the styles for a window class.
/// </summary>
[Flags]
public enum WindowClassStyle : uint
{
    /// <summary>
    /// Aligns the window's client area on a byte boundary (in the x direction). 
    /// This style affects the width of the window and its horizontal placement on the display.
    /// </summary>
    ByteAlignClient = 0x1000,

    /// <summary>
    /// Aligns the window on a byte boundary (in the x direction). 
    /// This style affects the width of the window and its horizontal placement on the display.
    /// </summary>
    ByteAlignWindow = 0x2000,

    /// <summary>
    /// Allocates one device context to be shared by all windows in the class. 
    /// Multiple threads of an application can create windows of the same class, 
    /// and the system ensures only one thread finishes its drawing operation at a time.
    /// </summary>
    ClassDC = 0x0040,

    /// <summary>
    /// Sends a double-click message to the window procedure when the user double-clicks 
    /// the mouse while the cursor is within a window belonging to the class.
    /// </summary>
    DblClks = 0x0008,

    /// <summary>
    /// Enables the drop shadow effect on a window. Typically used for small, short-lived windows, 
    /// such as menus, to emphasize their Z-order relationship to other windows. 
    /// Windows with this style must be top-level windows and cannot be child windows.
    /// </summary>
    DropShadow = 0x00020000,

    /// <summary>
    /// Indicates that the window class is an application global class.
    /// </summary>
    GlobalClass = 0x4000,

    /// <summary>
    /// Redraws the entire window if a movement or size adjustment changes the width of the client area.
    /// </summary>
    HRedraw = 0x0002,

    /// <summary>
    /// Disables Close on the window menu.
    /// </summary>
    NoClose = 0x0200,

    /// <summary>
    /// Allocates a unique device context for each window in the class.
    /// </summary>
    OwnDC = 0x0020,

    /// <summary>
    /// Sets the clipping rectangle of the child window to that of the parent window 
    /// so that the child can draw on the parent. A window with this style receives a regular 
    /// device context from the system's cache of device contexts, enhancing application performance.
    /// </summary>
    ParentDC = 0x0080,

    /// <summary>
    /// Saves, as a bitmap, the portion of the screen image obscured by a window of this class. 
    /// This style is useful for small windows, such as menus or dialog boxes, 
    /// that are displayed briefly and then removed. 
    /// </summary>
    SaveBits = 0x0800,

    /// <summary>
    /// Redraws the entire window if a movement or size adjustment changes the height of the client area.
    /// </summary>
    VRedraw = 0x0001
}
