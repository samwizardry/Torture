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
    private static nint _window;
    private static MouseButton _buttons;
    private static MouseButton _lastButtons;
    private static Vector2 _position;
    private static Vector2 _lastPosition;
    private static Vector2 _delta;
    private static Vector2 _rawScroll;
    private static Vector2 _lastRawScroll;
    private static Vector2 _scrollDelta;

    /// <summary>
    /// Gets the current cursor position in window coordinates.
    /// </summary>
    public static Vector2 Position => _position;

    /// <summary>
    /// Gets the cursor movement delta since the previous frame.
    /// </summary>
    public static Vector2 Delta => _delta;

    /// <summary>
    /// Gets the mouse wheel scroll delta (X = horizontal, Y = vertical) for the current frame.
    /// </summary>
    public static Vector2 ScrollDelta => _scrollDelta;

    /// <summary>
    /// Gets the vertical mouse wheel scroll delta for the current frame.
    /// </summary>
    public static float ScrollY => _scrollDelta.Y;

    /// <summary>
    /// Gets or sets whether relative mouse mode (cursor locked and hidden for 3D camera look) is enabled.
    /// </summary>
    public static bool IsRelativeMode
    {
        get => _window != IntPtr.Zero && SDL.GetWindowRelativeMouseMode(_window);
        set
        {
            if (_window != IntPtr.Zero)
            {
                SDL.SetWindowRelativeMouseMode(_window, value);
                // Reset accumulated delta mouse position
                SDL.GetRelativeMouseState(out _, out _);
                _delta = Vector2.Zero;
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
        _window = window;
        _buttons = (MouseButton)SDL.GetMouseState(out float x, out float y);
        _lastButtons = _buttons;
        _position = new Vector2(x, y);
        _lastPosition = _position;
        _delta = Vector2.Zero;
        _rawScroll = Vector2.Zero;
        _lastRawScroll = Vector2.Zero;
        _scrollDelta = Vector2.Zero;
    }

    internal static void Update()
    {
        _lastButtons = _buttons;
        _lastPosition = _position;
        _buttons = (MouseButton)SDL.GetMouseState(out float x, out float y);
        _position = new Vector2(x, y);
        if (IsRelativeMode)
        {
            SDL.GetRelativeMouseState(out float relX, out float relY);
            _delta = new Vector2(relX, relY);
        }
        else
        {
            _delta = _position - _lastPosition;
        }
        _scrollDelta = _rawScroll - _lastRawScroll;
        _lastRawScroll = _rawScroll;
    }

    internal static void Shutdown()
    {
        _window = IntPtr.Zero;
    }

    internal static void OnScrollWheel(float scrollX, float scrollY)
    {
        _rawScroll += new Vector2(scrollX, scrollY);
    }

    /// <summary>
    /// Moves the mouse cursor to the specified coordinates within the window.
    /// </summary>
    public static void SetPosition(float x, float y)
    {
        if (_window != IntPtr.Zero)
        {
            SDL.WarpMouseInWindow(_window, x, y);
            _position = new Vector2(x, y);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsButtonDown(MouseButton button) => (_buttons & button) != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsButtonUp(MouseButton button) => (_buttons & button) == 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsButtonPressed(MouseButton button) => (_lastButtons & button) == 0 && (_buttons & button) != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsButtonReleased(MouseButton button) => (_lastButtons & button) != 0 && (_buttons & button) == 0;
}
