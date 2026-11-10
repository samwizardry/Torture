using Torture.Core;
using Torture.Core.Graphics;
using Torture.Core.Input;

using Vortice.Mathematics;

namespace Torture.Luna;

[SDL3.SDL.GenerateMain]
public sealed partial class Luna : Application<Luna>
{
    private readonly bool _showFpsInTitle = true;
    private float _elapsedSeconds;
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
        base.Initialize();
    }

    protected override void LoadContent()
    {
    }

    protected override void Update(GameTime gameTime)
    {
        Task.Delay(16).ConfigureAwait(false).GetAwaiter().GetResult();

        if (Keyboard.IsKeyPressed(KeyboardKey.Escape))
        {
            Exit();
        }

        var deltaPos = Mouse.Delta;
        var pos = Mouse.Position;

        Console.WriteLine($"pos ({pos.X}, {pos.Y})\tdelta({deltaPos.X}, {deltaPos.Y})");

        if (_showFpsInTitle)
        {
            _frameCount++;
            _elapsedSeconds += gameTime.DeltaSeconds;
            if (_elapsedSeconds >= 1.0f)
            {
                double fps = _frameCount / _elapsedSeconds;
                Title = $"Luna | FPS: {fps:F1}";
                _elapsedSeconds -= 1.0f;
                _frameCount = 0;
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
            //backend = GraphicsBackend.Vulkan;
#endif
        }

        return backend switch
        {
#if SUPPORT_DIRECTX
            GraphicsBackend.Direct3D11
                //=> new D3D11GraphicsDevice(window.Win32Hwnd, window.Size, window.Bounds),
                => new Torture.Graphics.Direct3D11.GraphicsDevice(),
#endif
#if SUPPORT_VULKAN
            //GraphicsBackend.Vulkan
            //    => new VulkanGraphicsDevice(),
#endif
            _ => throw new PlatformNotSupportedException($"Backend {backend} is not supported on this platform.")
        };
    }
}