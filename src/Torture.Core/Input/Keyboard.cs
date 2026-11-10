using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using SDL3;

namespace Torture.Core.Input;

public static unsafe class Keyboard
{
    private static int s_numkeys;
    private static bool* s_state;
    private static bool* s_lastState;

    internal static void Initialize()
    {
        var state = SDL.GetKeyboardState(out s_numkeys);

        s_state = (bool*)NativeMemory.Alloc((nuint)s_numkeys, sizeof(bool));
        s_lastState = (bool*)NativeMemory.Alloc((nuint)s_numkeys, sizeof(bool));

        state.CopyTo(new Span<bool>(s_state, s_numkeys));
        NativeMemory.Copy(s_state, s_lastState, (nuint)s_numkeys);
    }

    internal static void Update()
    {
        var state = SDL.GetKeyboardState(out _);

        NativeMemory.Copy(s_state, s_lastState, (nuint)s_numkeys);
        state.CopyTo(new Span<bool>(s_state, s_numkeys));
    }

    internal static void Shutdown()
    {
        s_numkeys = 0;

        if (s_state != null)
        {
            NativeMemory.Free(s_state);
            s_state = null;
        }

        if (s_lastState != null)
        {
            NativeMemory.Free(s_lastState);
            s_lastState = null;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsKeyDown(KeyboardKey key)
    {
        int index = (int)key;
        return (uint)index < (uint)s_numkeys && s_state[index];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsKeyUp(KeyboardKey key)
    {
        int index = (int)key;
        return (uint)index < (uint)s_numkeys && !s_state[index];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsKeyPressed(KeyboardKey key)
    {
        int index = (int)key;
        return (uint)index < (uint)s_numkeys && !s_lastState[index] && s_state[index];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsKeyReleased(KeyboardKey key)
    {
        int index = (int)key;
        return (uint)index < (uint)s_numkeys && s_lastState[index] && !s_state[index];
    }
}
