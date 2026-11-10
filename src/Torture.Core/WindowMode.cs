namespace Torture.Core;

/// <summary>
/// Specifies the display mode of the application window.
/// </summary>
public enum WindowMode
{
    /// <summary>
    /// Standard window with title bar and borders.
    /// </summary>
    Windowed,

    /// <summary>
    /// Fullscreen window without borders covering the entire display.
    /// </summary>
    BorderlessFullscreen,

    /// <summary>
    /// Exclusive fullscreen mode.
    /// </summary>
    Fullscreen
}