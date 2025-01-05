namespace Ascpixi.Wakatime.FLStudio.Native.Enumerations;

public enum WindowEvent
{
    // Accessibility Interoperability Alliance (AIA)
    AiaStart = 0xA000,
    AiaEnd = 0xAFFF,

    // Minimum and Maximum event values
    Min = 0x00000001,
    Max = 0x7FFFFFFF,

    // Object Events
    ObjectAcceleratorChange = 0x8012,
    ObjectCloaked = 0x8017,
    ObjectContentScrolled = 0x8015,
    ObjectCreate = 0x8000,
    ObjectDefaultActionChange = 0x8011,
    ObjectDescriptionChange = 0x800D,
    ObjectDestroy = 0x8001,
    ObjectDragStart = 0x8021,
    ObjectDragCancel = 0x8022,
    ObjectDragComplete = 0x8023,
    ObjectDragEnter = 0x8024,
    ObjectDragLeave = 0x8025,
    ObjectDragDropped = 0x8026,
    ObjectEnd = 0x80FF,
    ObjectFocus = 0x8005,
    ObjectHelpChange = 0x8010,
    ObjectHide = 0x8003,
    ObjectHostedObjectsInvalidated = 0x8020,
    ObjectImeHide = 0x8028,
    ObjectImeShow = 0x8027,
    ObjectImeChange = 0x8029,
    ObjectInvoked = 0x8013,
    ObjectLiveRegionChanged = 0x8019,
    ObjectLocationChange = 0x800B,
    ObjectNameChange = 0x800C,
    ObjectParentChange = 0x800F,
    ObjectReorder = 0x8004,
    ObjectSelection = 0x8006,
    ObjectSelectionAdd = 0x8007,
    ObjectSelectionRemove = 0x8008,
    ObjectSelectionWithin = 0x8009,
    ObjectShow = 0x8002,
    ObjectStateChange = 0x800A,
    ObjectTextEditConversionTargetChanged = 0x8030,
    ObjectTextSelectionChanged = 0x8014,
    ObjectUncloaked = 0x8018,
    ObjectValueChange = 0x800E,

    // OEM Defined Range
    OemDefinedStart = 0x0101,
    OemDefinedEnd = 0x01FF,

    // System Events
    SystemAlert = 0x0002,
    SystemArrangementPreview = 0x8016,
    SystemCaptureEnd = 0x0009,
    SystemCaptureStart = 0x0008,
    SystemContextHelpEnd = 0x000D,
    SystemContextHelpStart = 0x000C,
    SystemDesktopSwitch = 0x0020,
    SystemDialogEnd = 0x0011,
    SystemDialogStart = 0x0010,
    SystemDragDropEnd = 0x000F,
    SystemDragDropStart = 0x000E,
    SystemEnd = 0x00FF,
    SystemForeground = 0x0003,
    SystemMenuPopupEnd = 0x0007,
    SystemMenuPopupStart = 0x0006,
    SystemMenuEnd = 0x0005,
    SystemMenuStart = 0x0004,
    SystemMinimizeEnd = 0x0017,
    SystemMinimizeStart = 0x0016,
    SystemMoveSizeEnd = 0x000B,
    SystemMoveSizeStart = 0x000A,
    SystemScrollingEnd = 0x0013,
    SystemScrollingStart = 0x0012,
    SystemSound = 0x0001,
    SystemSwitchEnd = 0x0015,
    SystemSwitchStart = 0x0014,

    // UI Automation Ranges
    UiaEventIdStart = 0x4E00,
    UiaEventIdEnd = 0x4EFF,
    UiaPropIdStart = 0x7500,
    UiaPropIdEnd = 0x75FF
}
