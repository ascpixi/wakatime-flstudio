using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using Ascpixi.Wakatime.FLStudio.Native;
using Ascpixi.Wakatime.FLStudio.Native.Enumerations;

namespace Ascpixi.Wakatime.FLStudio;

/// <summary>
/// Represents the Wakatime FL Studio overlay.
/// </summary>
public class Overlay : IDisposable
{
    static bool classRegistered;
    static WndProc? wndProc;
    static Dictionary<WindowHandle, Overlay> knownOverlays = [];
    
    readonly WindowHandle parentHandle, handle;
    readonly EventHookHandle resizeEventHook;
    readonly User32.WinEventProc resizeEventProc;

    bool shouldShow = true;
    string text = WakaTime.GetCategoryTime();
    
    const int OverlayWidth = 120;
    const int OverlayHeight = 20;
    
    const string WndClassName = "WakatimeFLStudioOverlay";

    (int X, int Y) PosFromParent(Rect parentRect)
        => (
            parentRect.Width - OverlayWidth - 20,
            parentRect.Height - OverlayHeight
        );
    
    void Reposition()
    {
        if (!User32.GetWindowRect(this.parentHandle, out var parentRect)) {
            Log.Win32Warning($"Could not get the window dimensions of HWND {this.parentHandle.Value:x}.");
            Hide();
            return;
        }

        if (parentRect.Top < -10000 || parentRect.Left < -10000) {
            Hide();
            return;
        }

        var (x, y) = PosFromParent(parentRect);
        
        Show();
        User32.SetWindowPos(this.handle, new(0), x, y, 0, 0, SwpFlags.NoSize);
    }

    void Hide() => User32.ShowWindow(this.handle, ShowWindowCmd.Hide);

    void Show()
    {
        if (!this.shouldShow)
            return;
        
        User32.ShowWindow(this.handle, ShowWindowCmd.Show);
    }

    public bool ShouldShow {
        get => this.shouldShow;
        set {
            this.shouldShow = value;
            
            if (value) {
                if (User32.GetForegroundWindow() == parentHandle) {
                    Show();
                }
            }
            else {
                Hide();
            }
        }
    } 
    
    public string Text {
        get => this.text;
        set {
            this.text = value;
            PaintWindow();
        }
    }
    
    /// <summary>
    /// Creates a new overlay.
    /// </summary>
    public Overlay(WindowHandle parent)
    {
        RegisterClassIfNeeded();
        
        if (!User32.GetWindowRect(parent, out var parentRect)) {
            Log.Win32Error($"Could not get the window dimensions of HWND {parent.Value:x}.");
            throw new Win32Exception();
        }

        var p = PosFromParent(parentRect);
        
        this.parentHandle = parent;
        this.handle = User32.CreateWindowEx(
            ExWindowStyle.Layered | ExWindowStyle.Topmost | ExWindowStyle.NoActivate,
            WndClassName,
            null, // lpWindowName
            WindowStyle.Popup | WindowStyle.Visible,
            p.X, p.Y, OverlayWidth, OverlayHeight
        );

        if (this.handle == default) {
            Log.Win32Error("Could not create overlay window.");
            throw new Win32Exception();
        }

        User32.SetLayeredWindowAttributes(this.handle, 0, 255, User32.LayerWndAttr.Alpha);

        this.resizeEventHook = User32.SetWinEventHook(
            WindowEvent.SystemForeground, WindowEvent.SystemMoveSizeEnd,
            default,
            this.resizeEventProc = (_, ev, hwnd, _, _, _, _) => {
                switch (ev) {
                    case WindowEvent.SystemForeground: {
                        if (hwnd != this.parentHandle) {
                            Hide();
                            return;
                        }

                        Reposition();
                        
                        // We also reposition it 250 ms later, as sometimes, for some reason,
                        // some other (invisible?) window takes priority in the Z-order.
                        new Thread(() => {
                            Thread.Sleep(250);
                            Reposition();
                        }).Start();
                        
                        break;
                    }

                    case WindowEvent.SystemMoveSizeEnd: {
                        Reposition();
                        break;
                    }
                }
            },
            0, 0,
            EventHookFlags.OutOfContext | EventHookFlags.SkipOwnProcess
        );
        
        Reposition();
        if (User32.GetForegroundWindow() != parent) {
            Hide();
        }
        
        Log.Info($"New overlay created with HWND 0x{this.handle.Value:X}.");
        
        knownOverlays.Add(this.handle, this);
    }

    static void RegisterClassIfNeeded()
    {
        if (classRegistered)
            return;

        wndProc = HandleWndProc;
        var wcx = new WndClassEx {
            WndProc = wndProc,
            ClassName = WndClassName
        };

        if (User32.RegisterClassEx(ref wcx).Value == 0) {
            Log.Win32Error("Could not register the Win32 class!");
            throw new Exception("Failed to register the overlay Win32 class.");
        }
        
        classRegistered = true;
    }

    void PaintWindow()
    {
        var hDC = User32.GetDC(this.handle);
        using var g = Graphics.FromHdc((nint)hDC.Value);
        g.SmoothingMode = SmoothingMode.AntiAlias;

        using var innerBrush = new SolidBrush(Color.FromArgb(255, 50, 58, 63));
        g.FillRectangle(innerBrush, 0, 0, OverlayWidth, OverlayHeight);

        using var outerPen = new Pen(Color.FromArgb(255, 29, 38, 43), 1);
        g.DrawRectangle(outerPen, 0, 0, OverlayWidth - 1, OverlayHeight - 1);

        using var font = new Font("Segoe UI", 10, FontStyle.Regular);
        using var textBrush = new SolidBrush(Color.FromArgb(255, 173, 173, 173));

        var textSize = g.MeasureString(this.text, font);
        float y = (OverlayHeight - textSize.Height) / 2;
        g.DrawString(this.text, font, textBrush, 8, y);

        User32.ReleaseDC(this.handle, hDC);
    }
    
    static nint HandleWndProc(WindowHandle hWnd, WindowMessageKind msg, nint wParam, nint lParam)
    {
        switch (msg) {
            case WindowMessageKind.WM_PAINT: {
                knownOverlays[hWnd].PaintWindow();
                break;
            }
            case WindowMessageKind.WM_ERASEBKGND: {
                return 1; // background is handled
            }
            case WindowMessageKind.WM_DESTROY: {
                User32.DestroyWindow(hWnd);
                break;
            }
        }

        return User32.DefWindowProc(hWnd, msg, wParam, lParam);
    }
    
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        
        User32.DestroyWindow(this.handle);

        if (resizeEventHook != default) {
            User32.UnhookWinEvent(resizeEventHook);
        }
        
        knownOverlays.Remove(this.handle);
    }
}