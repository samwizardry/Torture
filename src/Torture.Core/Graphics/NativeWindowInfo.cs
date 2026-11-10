using Vortice.Mathematics;

namespace Torture.Core.Graphics;

public readonly record struct NativeWindowInfo(nint SdlWindow, nint Win32Hwnd, SizeI Size, RectI Bounds);
