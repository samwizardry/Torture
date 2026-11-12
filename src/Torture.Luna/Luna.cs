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
    }

    VertexPositionColor[] _data;
    ushort[] _indexData;
    Core.Graphics.Buffer _vb;
    Core.Graphics.Buffer _ib;
    Shader _vs;
    Shader _ps;
    InputLayout _inputLayout;

    protected override void Initialize()
    {
        GraphicsDevice.VSync = false;
        GraphicsDevice.ClearColor = new Color4(0.2f, 0.3f, 0.3f, 1.0f);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _data = [
            new(new(0f, 0.5f, 0.0f), new(1.0f, 0.0f, 0.0f, 1.0f)),
            new(new(0.5f, -0.5f, 0.0f), new(0.0f, 1.0f, 0.0f, 1.0f)),
            new(new(-0.5f, -0.5f, 0.0f), new(0.0f, 0.0f, 1.0f, 1.0f))];

        _indexData = [0, 1, 2];

        _vb = GraphicsDevice.CreateBuffer(_data, new BufferDescription(BindFlags.VertexBuffer, false));
        _ib = GraphicsDevice.CreateBuffer(_indexData, new BufferDescription(BindFlags.IndexBuffer, false));

        _vs = GraphicsDevice.CreateShader(new ShaderDescription(
            ShaderType.Vertex,
            File.ReadAllBytes(Path.Combine("Assets", "Triangle.hlsl")),
            "VSMain",
            false,
            "Triangle_VS"));

        _ps = GraphicsDevice.CreateShader(new ShaderDescription(
            ShaderType.Pixel,
            File.ReadAllBytes(Path.Combine("Assets", "Triangle.hlsl")),
            "PSMain",
            false,
            "Triangle_PS"));

        _inputLayout = GraphicsDevice.CreateInputLayout(VertexPositionColor.Layout, _vs);
    }

    protected override void Update(GameTime gameTime)
    {
        if (Keyboard.IsKeyPressed(KeyboardKey.Escape))
        {
            Exit();
        }

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
        GraphicsDevice.SetPrimitiveTopology(PrimitiveTopology.TriangleStrip);
        GraphicsDevice.SetVertexShader(_vs);
        GraphicsDevice.SetPixelShader(_ps);
        GraphicsDevice.SetInputLayout(_inputLayout);
        GraphicsDevice.SetVertexBuffer(_vb, VertexPositionColor.Layout.Stride, 0, 0);
        GraphicsDevice.SetIndexBuffer(_ib, IndexFormat.UInt16, 0);
        GraphicsDevice.DrawIndexed((uint)_indexData.Length, 0, 0);
    }

    protected override void UnloadContent()
    {
    }

    protected override void Cleanup()
    {
        _inputLayout?.Dispose();
        _ps?.Dispose();
        _vs?.Dispose();
        _vb?.Dispose();
        _ib?.Dispose();
    }

    protected override GraphicsDevice CreateGraphicsDevice(GraphicsAPI api, NativeWindowInfo window)
    {
        if (api == GraphicsAPI.Default)
        {
#if SUPPORT_DIRECTX
            api = GraphicsAPI.D3D11;
#elif SUPPORT_VULKAN
            //api = GraphicsAPI.Vulkan;
#endif
        }

        return api switch
        {
#if SUPPORT_DIRECTX
            GraphicsAPI.D3D11
                => new Torture.Graphics.Direct3D11.D3D11GraphicsDevice(window.WindowHandle, window.ClientSize),
#endif
#if SUPPORT_VULKAN
            //GraphicsAPI.Vulkan
            //    => new VulkanGraphicsDevice(),
#endif
            _ => throw new PlatformNotSupportedException($"{api} is not supported on this platform.")
        };
    }
}
