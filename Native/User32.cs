using System.Runtime.InteropServices;
using System.Text;
using Ascpixi.Wakatime.FLStudio.Native.Enumerations;

namespace Ascpixi.Wakatime.FLStudio.Native;

public static partial class User32
{
    /// <summary>
    /// Retrieves the dimensions of the bounding rectangle of the specified window. The dimensions are given in screen coordinates that are relative to the upper-left corner of the screen.
    /// </summary>
    /// <param name="hWnd">A handle to the window.</param>
    /// <param name="lpRect">A pointer to a RECT structure that receives the screen coordinates of the upper-left and lower-right corners of the window.</param>
    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetWindowRect(
        WindowHandle hWnd,
        out Rect lpRect
    );

    /// <summary>
    /// Creates an overlapped, pop-up, or child window with an extended window style; otherwise, this function is identical to the CreateWindow function.
    /// </summary>
    /// <param name="dwExStyle">The extended window style of the window being created.</param>
    /// <param name="lpClassName">A null-terminated string or a class atom.</param>
    /// <param name="lpWindowName">The window name.</param>
    /// <param name="dwStyle">The style of the window being created.</param>
    /// <param name="x">The initial horizontal position of the window.</param>
    /// <param name="y">The initial vertical position of the window.</param>
    /// <param name="nWidth">The width, in device units, of the window.</param>
    /// <param name="nHeight">The height, in device units, of the window.</param>
    /// <param name="hWndParent">A handle to the parent or owner window of the window being created.</param>
    /// <param name="hMenu">A handle to a menu, or specifies a child-window identifier, depending on the window style.</param>
    /// <param name="hInstance">A handle to the instance of the module to be associated with the window.</param>
    /// <param name="lpParam">A handle to the instance of the module to be associated with the window.</param>
    /// <returns>If the function succeeds, the return value is a handle to the new window.</returns>
    [LibraryImport("user32.dll", EntryPoint = "CreateWindowExW", StringMarshalling = StringMarshalling.Utf16, SetLastError = true)]
    public static partial WindowHandle CreateWindowEx(
        ExWindowStyle dwExStyle,
        string? lpClassName,
        string? lpWindowName,
        WindowStyle dwStyle,
        int x,
        int y,
        int nWidth,
        int nHeight,
        [Optional] WindowHandle hWndParent,
        [Optional] nint hMenu,
        [Optional] ModuleHandle hInstance,
        [Optional] nuint lpParam
    );

    /// <summary>
    /// Displays a modal dialog box that contains a system icon, a set of buttons, and a brief application-specific message, such as status or error information. The message box returns an integer value that indicates which button the user clicked.
    /// </summary>
    /// <param name="hWnd">A handle to the owner window of the message box to be created. If this parameter is NULL, the message box has no owner window.</param>
    /// <param name="lpText">The message to be displayed. If the string consists of more than one line, you can separate the lines using a carriage return and/or linefeed character between each line.</param>
    /// <param name="lpCaption">The dialog box title. If this parameter is NULL, the default title is Error.</param>
    /// <param name="uType">The contents and behavior of the dialog box. </param>
    [LibraryImport("user32.dll", EntryPoint = "MessageBoxW", StringMarshalling = StringMarshalling.Utf16)]
    public static partial int MessageBox(
        [Optional] WindowHandle hWnd,
        string? lpText,
        string? lpCaption,
        MsgBoxFlags uType
    );

    /// <summary>
    /// Copies the text of the specified window's title bar (if it has one) into a buffer. If the specified window is a control, the text of the control is copied. However, GetWindowText cannot retrieve the text of a control in another application.
    /// </summary>
    /// <param name="hWnd">A handle to the window or control containing the text.</param>
    /// <param name="lpString">The buffer that will receive the text. If the string is as long or longer than the buffer, the string is truncated and terminated with a null character.</param>
    /// <param name="nMaxCount">The maximum number of characters to copy to the buffer, including the null character. If the text exceeds this limit, it is truncated.</param>
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern int GetWindowText(
        WindowHandle hWnd,
        StringBuilder lpString,
        int nMaxCount
    );
    
    public delegate bool EnumThreadDelegate(WindowHandle hWnd, nint lParam);

    /// <summary>
    /// Enumerates all nonchild windows associated with a thread by passing the handle to each window, in turn, to an application-defined callback function.
    /// </summary>
    /// <param name="dwThreadId">The identifier of the thread whose windows are to be enumerated.</param>
    /// <param name="lpfn">A pointer to an application-defined callback function.</param>
    /// <param name="lParam">An application-defined value to be passed to the callback function.</param>
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool EnumThreadWindows(
        int dwThreadId,
        EnumThreadDelegate lpfn, 
        nint lParam
    );

    /// <summary>
    /// Retrieves the name of the class to which the specified window belongs.
    /// </summary>
    /// <param name="hwnd">A handle to the window and, indirectly, the class to which the window belongs.</param>
    /// <param name="lpClassName">The class name string.</param>
    /// <param name="nMaxCount">The length of the lpClassName buffer, in characters. The buffer must be large enough to include the terminating null character; otherwise, the class name string is truncated to nMaxCount-1 characters.</param>
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern int GetClassName(
        WindowHandle hwnd,
        StringBuilder lpClassName,
        int nMaxCount
    );

    public delegate void WinEventProc(
        EventHookHandle hWinEventHook,
        WindowEvent winEvent,
        WindowHandle hwnd,
        int idObject,
        int idChild,
        uint idEventThread,
        uint dwmsEventTime
    );
    
    /// <summary>
    /// Sets an event hook function for a range of events.
    /// </summary>
    // https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwineventhook?redirectedfrom=MSDN
    [LibraryImport("user32.dll", SetLastError = true)]
    public static partial EventHookHandle SetWinEventHook(
        WindowEvent eventMin,
        WindowEvent eventMax,
        ModuleHandle hmodWinEventProc,
        WinEventProc pfnWinEventProc,
        uint idProcess,
        uint idThread,
        EventHookFlags dwFlags
    );

    /// <summary>
    /// Removes an event hook function created by a previous call to SetWinEventHook.
    /// </summary>
    /// <param name="hWinEventHook">Handle to the event hook returned in the previous call to SetWinEventHook.</param>
    /// <returns>If successful, returns TRUE; otherwise, returns FALSE.</returns>
    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool UnhookWinEvent(EventHookHandle hWinEventHook);

    /// <summary>
    /// Dispatches incoming nonqueued messages, checks the thread message queue for a posted message, and retrieves the message (if any exist).
    /// </summary>
    [LibraryImport("user32.dll", EntryPoint = "PeekMessageW", StringMarshalling = StringMarshalling.Utf16)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool PeekMessage(
        out WindowMessage lpMsg,
        [Optional] WindowHandle hWnd,
        uint wMsgFilterMin,
        uint wMsgFilterMax,
        PeekMessageBehavior wRemoveMsg
    );

    public enum PeekMessageBehavior
    {
        NoRemove = 0x0000,
        Remove = 0x0001,
        NoYield = 0x0002
    }

    [LibraryImport("user32.dll", EntryPoint = "GetMessageW", StringMarshalling = StringMarshalling.Utf16)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetMessage(
        out WindowMessage lpMsg,
        [Optional] WindowHandle hWnd,
        uint wMsgFilterMin,
        uint wMsgFilterMax
    );

    /// <summary>
    /// Translates virtual-key messages into character messages. The character messages are posted to the calling thread's message queue, to be read the next time the thread calls the GetMessage or PeekMessage function.
    /// </summary>
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TranslateMessage(ref WindowMessage lpMsg);

    /// <summary>
    /// Dispatches a message to a window procedure. It is typically used to dispatch a message retrieved by the GetMessage function.
    /// </summary>
    [LibraryImport("user32.dll", EntryPoint = "DispatchMessageW", StringMarshalling = StringMarshalling.Utf16)]
    public static partial nint DispatchMessage(ref WindowMessage lpMsg);

    /// <summary>
    /// Retrieves the identifier of the thread that created the specified window and, optionally, the identifier of the process that created the window.
    /// </summary>
    [LibraryImport("user32.dll")]
    public static partial uint GetWindowThreadProcessId(
        WindowHandle hWnd,
        out uint lpdwProcessId
    );

    /// <summary>
    /// Retrieves the cursor's position, in screen coordinates.
    /// </summary>
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetCursorPos(out Point lpPoint);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetLastInputInfo(ref LastInputInfo plii);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool IsWindow(WindowHandle hWnd);

#pragma warning disable SYSLIB1054 // No, we can't! I don't want to write a whole marshaller for this one structure, no thank you.
    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern Atom RegisterClassEx(ref WndClassEx lpwcx);
#pragma warning restore SYSLIB1054

    [LibraryImport("user32.dll", SetLastError = true)]
    public static partial DCHandle GetDC(WindowHandle hWnd);

    [LibraryImport("user32.dll")]
    public static partial int ReleaseDC(WindowHandle hWnd, DCHandle hDC);

    public enum LayerWndAttr : uint
    {
        Alpha = 2,
        ColorKey = 1
    }
    
    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool SetLayeredWindowAttributes(
        WindowHandle hwnd,
        uint crKey,
        byte bAlpha,
        LayerWndAttr dwFlags
    );

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool DestroyWindow(WindowHandle hWnd);

    [LibraryImport("user32.dll", EntryPoint = "DefWindowProcA", StringMarshalling = StringMarshalling.Utf16, SetLastError = true)]
    public static partial nint DefWindowProc(
        WindowHandle hWnd,
        WindowMessageKind uMsg,
        nint wParam,
        nint lParam
    );
    
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool SetWindowPos(
        WindowHandle hWnd,
        [Optional] WindowHandle hWndInsertAfter,
        int X,
        int Y,
        int cx,
        int cy,
        SwpFlags uFlags
    );

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool ShowWindow(WindowHandle hWnd, ShowWindowCmd nCmdShow);

    [LibraryImport("user32.dll")]
    public static partial WindowHandle GetForegroundWindow();
}