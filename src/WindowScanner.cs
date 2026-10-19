using System.Runtime.InteropServices;

namespace WindowFixer;

internal static class WindowScanner
{
    /// <summary>
    /// Enumerates all visible, titled, non-tool, non-cloaked top-level windows.
    /// One entry per top-level window (not one per process).
    /// </summary>
    internal static List<nint> EnumerateWindows()
    {
        var handles = new List<nint>();

        // Kept alive in a local for the duration of the EnumWindows call.
        NativeMethods.EnumWindowsProc callback = (hWnd, _) =>
        {
            if (!NativeMethods.IsWindowVisible(hWnd))
                return true;

            if (NativeMethods.GetWindowTextLength(hWnd) == 0)
                return true;

            if ((NativeMethods.GetExStyle(hWnd) & NativeMethods.WS_EX_TOOLWINDOW) != 0)
                return true;

            if (NativeMethods.IsCloaked(hWnd))
                return true;

            handles.Add(hWnd);
            return true;
        };

        NativeMethods.EnumWindows(callback, IntPtr.Zero);
        GC.KeepAlive(callback);

        return handles;
    }

    /// <summary>
    /// Union of all screen bounds (the visible virtual screen).
    /// </summary>
    internal static Rectangle GetVirtualBounds()
    {
        Rectangle bounds = Rectangle.Empty;

        foreach (Screen screen in Screen.AllScreens)
        {
            bounds = bounds.IsEmpty
                ? screen.Bounds
                : Rectangle.Union(bounds, screen.Bounds);
        }

        return bounds;
    }

    /// <summary>
    /// WINDOWPLACEMENT.rcNormalPosition is in workspace coordinates, not screen
    /// coordinates. Calibrate the offset against the first enumerated window in
    /// the normal state; if none exists, assume no offset.
    /// </summary>
    internal static Size GetWorkspaceOffset()
    {
        foreach (nint hWnd in EnumerateWindows())
        {
            NativeMethods.WINDOWPLACEMENT placement = GetPlacement(hWnd);
            if (placement.showCmd != NativeMethods.SW_SHOWNORMAL)
                continue;

            if (!NativeMethods.GetWindowRect(hWnd, out NativeMethods.RECT rect))
                continue;

            return new Size(
                rect.Left - placement.rcNormalPosition.Left,
                rect.Top - placement.rcNormalPosition.Top);
        }

        return Size.Empty;
    }

    /// <summary>
    /// Returns the windows that are stranded outside the visible virtual screen.
    /// A partial overlap counts as stranded.
    /// </summary>
    internal static List<WindowInfo> FindStrandedWindows()
    {
        List<nint> handles = EnumerateWindows();
        Rectangle virtualBounds = GetVirtualBounds();
        Size offset = GetWorkspaceOffset();

        var stranded = new List<WindowInfo>();

        foreach (nint hWnd in handles)
        {
            NativeMethods.WINDOWPLACEMENT placement = GetPlacement(hWnd);

            Rectangle normalRect = new(
                placement.rcNormalPosition.Left + offset.Width,
                placement.rcNormalPosition.Top + offset.Height,
                placement.rcNormalPosition.Right - placement.rcNormalPosition.Left,
                placement.rcNormalPosition.Bottom - placement.rcNormalPosition.Top);

            bool isMinimized = placement.showCmd == NativeMethods.SW_SHOWMINIMIZED;
            bool isMaximized = placement.showCmd == NativeMethods.SW_SHOWMAXIMIZED;
            bool isStranded;

            if (isMinimized)
            {
                // The window itself sits at the taskbar; its restore position is what matters.
                isStranded = !virtualBounds.Contains(normalRect);
            }
            else
            {
                if (!NativeMethods.GetWindowRect(hWnd, out NativeMethods.RECT rect))
                    continue;

                var windowRect = Rectangle.FromLTRB(rect.Left, rect.Top, rect.Right, rect.Bottom);

                // A maximized window's GetWindowRect is inflated by the invisible DWM
                // resize border, so even a window correctly maximized on a connected
                // monitor extends a few pixels past the screen bounds. Use the rectangle
                // center to tell which monitor the maximized window actually occupies.
                isStranded = isMaximized
                    ? !virtualBounds.Contains(
                        windowRect.Left + windowRect.Width / 2,
                        windowRect.Top + windowRect.Height / 2)
                    : !virtualBounds.Contains(windowRect);
            }

            if (!isStranded)
                continue;

            stranded.Add(new WindowInfo
            {
                Hwnd = hWnd,
                Title = NativeMethods.GetTitle(hWnd),
                NormalRect = normalRect,
                IsMinimized = isMinimized,
                IsMaximized = isMaximized
            });
        }

        return stranded;
    }

    private static NativeMethods.WINDOWPLACEMENT GetPlacement(nint hWnd)
    {
        var placement = new NativeMethods.WINDOWPLACEMENT
        {
            length = Marshal.SizeOf<NativeMethods.WINDOWPLACEMENT>()
        };

        NativeMethods.GetWindowPlacement(hWnd, ref placement);
        return placement;
    }
}
