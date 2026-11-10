using System.Diagnostics;

using Vortice.Mathematics;

using Torture.Core;
using Torture.Core.Graphics;

#if SUPPORT_DIRECTX
using Torture.Graphics.Direct3D11;

#endif
#if SUPPORT_VULKAN
using Torture.Graphics.Vulkan;
#endif

namespace Torture.Luna;

[SDL3.SDL.GenerateMain]
public sealed partial class Luna : Application<Luna>
{
    private readonly Stopwatch _fpsTimer = new();
    private readonly bool _showFpsInTitle = true;
    private int _frameCount;

    public Luna()
    {
        Title = "Luna";
        Identifier = "Semisoft.Luna";
        PreferredWindowSize = new SizeI(1280, 720);
        WindowMode = WindowMode.Windowed;
        IsResizable = true;
    }

    protected override void Initialize()
    {
        _fpsTimer.Start();
        base.Initialize();
    }

    protected override void LoadContent()
    {
    }

    protected override void Update(GameTime gameTime)
    {
        if (_showFpsInTitle)
        {
            _frameCount++;
            double fpsElapsed = _fpsTimer.Elapsed.TotalSeconds;
            if (fpsElapsed >= 1.0)
            {
                double fps = _frameCount / fpsElapsed;
                Title = $"Luna | FPS: {fps:F1}";
                _frameCount = 0;
                _fpsTimer.Restart();
            }
        }
    }

    protected override void Draw(GameTime gameTime)
    {
    }

    protected override void UnloadContent()
    {
    }

    protected override void Cleanup()
    {
    }

    protected override IGraphicsDevice CreateGraphicsDevice(GraphicsBackend backend, NativeWindowInfo window)
    {
        if (backend == GraphicsBackend.Default)
        {
#if SUPPORT_DIRECTX
            backend = GraphicsBackend.Direct3D11;
#elif SUPPORT_VULKAN
            backend = GraphicsBackend.Vulkan;
#endif
        }

        return backend switch
        {
#if SUPPORT_DIRECTX
            GraphicsBackend.Direct3D11
                //=> new D3D11GraphicsDevice(window.Win32Hwnd, window.Size, window.Bounds),
                => new D3D11GraphicsDevice(),
#endif
#if SUPPORT_VULKAN
            GraphicsBackend.Vulkan
                => new VulkanGraphicsDevice(),
#endif
            _ => throw new PlatformNotSupportedException($"Backend {backend} is not supported on this platform.")
        };
    }
}