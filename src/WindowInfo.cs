namespace WindowFixer;

internal sealed class WindowInfo
{
    public nint Hwnd { get; init; }

    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// Screen coordinates of the window's normal/restore position.
    /// </summary>
    public Rectangle NormalRect { get; init; }

    public bool IsMinimized { get; init; }

    public bool IsMaximized { get; init; }

    public string PositionText => $"({NormalRect.X}, {NormalRect.Y})";
}
