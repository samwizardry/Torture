using Vortice.Mathematics;

namespace Torture.Core.Graphics;

public readonly record struct NativeWindowInfo(
    nint SdlWindow,
    nint WindowHandle,
    SizeI ClientSize,
    RectI Bounds);
