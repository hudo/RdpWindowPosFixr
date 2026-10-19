using System.Runtime.InteropServices;

namespace WindowFixer;

internal static class WindowRescuer
{
    /// <summary>
    /// Moves a stranded window to the center of the primary screen's working area,
    /// preserving its size. Minimized windows stay minimized (only their restore
    /// position changes); maximized windows stay maximized.
    /// </summary>
    internal static bool TryMoveToPrimary(WindowInfo window)
    {
        Rectangle workingArea = Screen.PrimaryScreen!.WorkingArea;

        if (window.IsMaximized)
            return MoveMaximized(window, workingArea);

        if (window.IsMinimized)
            return MoveMinimized(window, workingArea);

        return MoveNormal(window, workingArea);
    }

    private static bool MoveNormal(WindowInfo window, Rectangle workingArea)
    {
        if (!NativeMethods.GetWindowRect(window.Hwnd, out NativeMethods.RECT rect))
            return false;

        int width = rect.Right - rect.Left;
        int height = rect.Bottom - rect.Top;
        Rectangle target = CenterIn(workingArea, width, height);

        return NativeMethods.SetWindowPos(
            window.Hwnd,
            IntPtr.Zero,
            target.X,
            target.Y,
            target.Width,
            target.Height,
            NativeMethods.SWP_NOZORDER | NativeMethods.SWP_NOACTIVATE);
    }

    private static bool MoveMaximized(WindowInfo window, Rectangle workingArea)
    {
        Rectangle centered = CenterIn(workingArea, window.NormalRect.Width, window.NormalRect.Height);
        Size offset = WindowScanner.GetWorkspaceOffset();

        var placement = new NativeMethods.WINDOWPLACEMENT
        {
            length = Marshal.SizeOf<NativeMethods.WINDOWPLACEMENT>(),
            showCmd = NativeMethods.SW_SHOWMAXIMIZED,
            rcNormalPosition = ToWorkspace(centered, offset)
        };
        NativeMethods.SetWindowPlacement(window.Hwnd, ref placement);

        if (IsOnPrimaryMonitor(window.Hwnd))
            return true;

        // Fallback: restore, move to the centered normal rect, then maximize again.
        NativeMethods.ShowWindow(window.Hwnd, NativeMethods.SW_RESTORE);
        NativeMethods.SetWindowPos(
            window.Hwnd,
            IntPtr.Zero,
            centered.X,
            centered.Y,
            centered.Width,
            centered.Height,
            NativeMethods.SWP_NOZORDER | NativeMethods.SWP_NOACTIVATE);
        NativeMethods.ShowWindow(window.Hwnd, NativeMethods.SW_SHOWMAXIMIZED);

        return IsOnPrimaryMonitor(window.Hwnd);
    }

    private static bool MoveMinimized(WindowInfo window, Rectangle workingArea)
    {
        Rectangle centered = CenterIn(workingArea, window.NormalRect.Width, window.NormalRect.Height);
        Size offset = WindowScanner.GetWorkspaceOffset();

        var placement = new NativeMethods.WINDOWPLACEMENT
        {
            length = Marshal.SizeOf<NativeMethods.WINDOWPLACEMENT>(),
            showCmd = NativeMethods.SW_SHOWMINIMIZED,
            rcNormalPosition = ToWorkspace(centered, offset)
        };

        if (!NativeMethods.SetWindowPlacement(window.Hwnd, ref placement))
            return false;

        // The window must remain minimized after the change.
        var current = new NativeMethods.WINDOWPLACEMENT
        {
            length = Marshal.SizeOf<NativeMethods.WINDOWPLACEMENT>()
        };

        if (!NativeMethods.GetWindowPlacement(window.Hwnd, ref current))
            return false;

        return current.showCmd == NativeMethods.SW_SHOWMINIMIZED;
    }

    private static bool IsOnPrimaryMonitor(nint hWnd)
    {
        if (!NativeMethods.GetWindowRect(hWnd, out NativeMethods.RECT rect))
            return false;

        // Maximized windows report a rectangle inflated by the invisible resize border,
        // so use the rectangle center to tell which monitor the window occupies.
        return Screen.PrimaryScreen!.Bounds.Contains(
            rect.Left + (rect.Right - rect.Left) / 2,
            rect.Top + (rect.Bottom - rect.Top) / 2);
    }

    private static Rectangle CenterIn(Rectangle area, int width, int height)
    {
        int x = area.X + (area.Width - width) / 2;
        int y = area.Y + (area.Height - height) / 2;
        return new Rectangle(x, y, width, height);
    }

    private static NativeMethods.RECT ToWorkspace(Rectangle screenRect, Size offset) => new()
    {
        Left = screenRect.Left - offset.Width,
        Top = screenRect.Top - offset.Height,
        Right = screenRect.Right - offset.Width,
        Bottom = screenRect.Bottom - offset.Height
    };
}
