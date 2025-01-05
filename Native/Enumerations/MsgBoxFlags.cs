namespace Ascpixi.Wakatime.FLStudio.Native.Enumerations;

[Flags]
public enum MsgBoxFlags : uint
{
    /// <summary>
    /// The message box contains three push buttons: Abort, Retry, and Ignore.
    /// </summary>
    AbortRetryIgnore = 0x00000002,

    /// <summary>
    /// The message box contains three push buttons: Cancel, Try Again, and Continue. Preferred over AbortRetryIgnore.
    /// </summary>
    CancelTryContinue = 0x00000006,

    /// <summary>
    /// Adds a Help button to the message box. Triggers a WM_HELP message when clicked.
    /// </summary>
    Help = 0x00004000,

    /// <summary>
    /// The message box contains one push button: OK. This is the default.
    /// </summary>
    Ok = 0x00000000,

    /// <summary>
    /// The message box contains two push buttons: OK and Cancel.
    /// </summary>
    OkCancel = 0x00000001,

    /// <summary>
    /// The message box contains two push buttons: Retry and Cancel.
    /// </summary>
    RetryCancel = 0x00000005,

    /// <summary>
    /// The message box contains two push buttons: Yes and No.
    /// </summary>
    YesNo = 0x00000004,

    /// <summary>
    /// The message box contains three push buttons: Yes, No, and Cancel.
    /// </summary>
    YesNoCancel = 0x00000003,

    /// <summary>
    /// An exclamation-point icon appears in the message box.
    /// </summary>
    IconExclamation = 0x00000030,

    /// <summary>
    /// An exclamation-point icon appears in the message box. Same as IconExclamation.
    /// </summary>
    IconWarning = 0x00000030,

    /// <summary>
    /// An icon consisting of a lowercase letter 'i' in a circle appears in the message box.
    /// </summary>
    IconInformation = 0x00000040,

    /// <summary>
    /// An icon consisting of a lowercase letter 'i' in a circle appears in the message box. Same as IconInformation.
    /// </summary>
    IconAsterisk = 0x00000040,

    /// <summary>
    /// A question-mark icon appears in the message box. Not recommended for new applications.
    /// </summary>
    IconQuestion = 0x00000020,

    /// <summary>
    /// A stop-sign icon appears in the message box.
    /// </summary>
    IconStop = 0x00000010,

    /// <summary>
    /// A stop-sign icon appears in the message box. Same as IconStop.
    /// </summary>
    IconError = 0x00000010,

    /// <summary>
    /// A stop-sign icon appears in the message box. Same as IconStop.
    /// </summary>
    IconHand = 0x00000010,

    /// <summary>
    /// The first button is the default button. This is the default unless another default button is specified.
    /// </summary>
    DefaultButton1 = 0x00000000,

    /// <summary>
    /// The second button is the default button.
    /// </summary>
    DefaultButton2 = 0x00000100,

    /// <summary>
    /// The third button is the default button.
    /// </summary>
    DefaultButton3 = 0x00000200,

    /// <summary>
    /// The fourth button is the default button.
    /// </summary>
    DefaultButton4 = 0x00000300,

    /// <summary>
    /// The user must respond to the message box before continuing work in the current window. Default modality.
    /// </summary>
    AppModal = 0x00000000,

    /// <summary>
    /// The message box has the WS_EX_TOPMOST style and requires immediate attention.
    /// </summary>
    SystemModal = 0x00001000,

    /// <summary>
    /// All top-level windows of the current thread are disabled until the message box is closed.
    /// </summary>
    TaskModal = 0x00002000,

    /// <summary>
    /// The message box is displayed only on the default desktop.
    /// </summary>
    DefaultDesktopOnly = 0x00020000,

    /// <summary>
    /// The text is right-justified.
    /// </summary>
    Right = 0x00080000,

    /// <summary>
    /// Displays text using right-to-left reading order, for Hebrew or Arabic systems.
    /// </summary>
    RtlReading = 0x00100000,

    /// <summary>
    /// The message box becomes the foreground window.
    /// </summary>
    SetForeground = 0x00010000,

    /// <summary>
    /// The message box is created with the WS_EX_TOPMOST window style.
    /// </summary>
    Topmost = 0x00040000,

    /// <summary>
    /// The message box appears on the current active desktop, even if no user is logged in.
    /// </summary>
    ServiceNotification = 0x00200000
}