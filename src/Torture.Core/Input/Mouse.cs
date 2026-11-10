using System.Numerics;
using System.Runtime.CompilerServices;

using SDL3;

namespace Torture.Core.Input;

/// <summary>
/// Specifies mouse buttons as bitmasks matching SDL3 mouse button flags.
/// </summary>
[Flags]
public enum MouseButton : uint
{
    None = 0,
    Left = 1u << 0,
    Middle = 1u << 1,
    Right = 1u << 2,
    X1 = 1u << 3,
    X2 = 1u << 4
}

public static class Mouse
{
    private static nint s_window;
    private static MouseButton s_buttons;
    private static MouseButton s_lastButtons;
    private static Vector2 s_position;
    private static Vector2 s_lastPosition;
    private static Vector2 s_delta;
    private static Vector2 s_rawScroll;
    private static Vector2 s_lastRawScroll;
    private static Vector2 s_scrollDelta;

    /// <summary>
    /// Gets the current cursor position in window coordinates.
    /// </summary>
    public static Vector2 Position => s_position;

    /// <summary>
    /// Gets the cursor movement delta since the previous frame.
    /// </summary>
    public static Vector2 Delta => s_delta;

    /// <summary>
    /// Gets the mouse wheel scroll delta (X = horizontal, Y = vertical) for the current frame.
    /// </summary>
    public static Vector2 ScrollDelta => s_scrollDelta;

    /// <summary>
    /// Gets the vertical mouse wheel scroll delta for the current frame.
    /// </summary>
    public static float ScrollY => s_scrollDelta.Y;

    /// <summary>
    /// Gets or sets whether relative mouse mode (cursor locked and hidden for 3D camera look) is enabled.
    /// </summary>
    public static bool IsRelativeMode
    {
        get => s_window != IntPtr.Zero && SDL.GetWindowRelativeMouseMode(s_window);
        set
        {
            if (s_window != IntPtr.Zero)
            {
                SDL.SetWindowRelativeMouseMode(s_window, value);
                // Reset accumulated delta mouse position
                SDL.GetRelativeMouseState(out _, out _);
                s_delta = Vector2.Zero;
            }
        }
    }

    /// <summary>
    /// Gets or sets whether the OS mouse cursor is visible.
    /// </summary>
    public static bool IsCursorVisible
    {
        get => SDL.CursorVisible();
        set
        {
            if (value)
            {
                SDL.ShowCursor();
            }
            else
            {
                SDL.HideCursor();
            }
        }
    }

    internal static void Initialize(nint window)
    {
        s_window = window;
        s_buttons = (MouseButton)SDL.GetMouseState(out float x, out float y);
        s_lastButtons = s_buttons;
        s_position = new Vector2(x, y);
        s_lastPosition = s_position;
        s_delta = Vector2.Zero;
        s_rawScroll = Vector2.Zero;
        s_lastRawScroll = Vector2.Zero;
        s_scrollDelta = Vector2.Zero;
    }

    internal static void Update()
    {
        s_lastButtons = s_buttons;
        s_lastPosition = s_position;
        s_buttons = (MouseButton)SDL.GetMouseState(out float x, out float y);
        s_position = new Vector2(x, y);
        if (IsRelativeMode)
        {
            SDL.GetRelativeMouseState(out float relX, out float relY);
            s_delta = new Vector2(relX, relY);
        }
        else
        {
            s_delta = s_position - s_lastPosition;
        }
        s_scrollDelta = s_rawScroll - s_lastRawScroll;
        s_lastRawScroll = s_rawScroll;
    }

    internal static void Shutdown()
    {
        s_window = IntPtr.Zero;
    }

    internal static void OnScrollWheel(float scrollX, float scrollY)
    {
        s_rawScroll += new Vector2(scrollX, scrollY);
    }

    /// <summary>
    /// Moves the mouse cursor to the specified coordinates within the window.
    /// </summary>
    public static void SetPosition(float x, float y)
    {
        if (s_window != IntPtr.Zero)
        {
            SDL.WarpMouseInWindow(s_window, x, y);
            s_position = new Vector2(x, y);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsButtonDown(MouseButton button) => (s_buttons & button) != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsButtonUp(MouseButton button) => (s_buttons & button) == 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsButtonPressed(MouseButton button) => (s_lastButtons & button) == 0 && (s_buttons & button) != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsButtonReleased(MouseButton button) => (s_lastButtons & button) != 0 && (s_buttons & button) == 0;
}
