using System.Numerics;

using Brutal.Collections;

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

    (NativeList<ushort> Indices, NativeList<VertexPositionNormalTexture> Vertices) _box;
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
        _box = Geometry.ComputeBox(Vector3.One, false, false);

        _vb = GraphicsDevice.CreateBuffer(_box.Vertices.AsSpan(), BufferDescription.Vertex(0, 0));
        _ib = GraphicsDevice.CreateBuffer(_box.Indices.AsSpan(), BufferDescription.Index(0));

        _box.Vertices.Dispose();
        //_box.Indices.Dispose();

        var triangleCode = File.ReadAllBytes(Path.Combine("Assets", "Triangle.hlsl"));

        _vs = GraphicsDevice.CreateShader(new ShaderDescription(
            ShaderType.Vertex,
            triangleCode,
            "VSMain",
            false,
            "Triangle_VS"));

        _ps = GraphicsDevice.CreateShader(new ShaderDescription(
            ShaderType.Pixel,
            triangleCode,
            "PSMain",
            false,
            "Triangle_PS"));

        _inputLayout = GraphicsDevice.CreateInputLayout(VertexPositionNormalTexture.Layout, _vs);
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
                Console.WriteLine($"Luna | FPS: {fps:F1}");
                _elapsedSeconds -= 1.0f;
                _frameCount = 0;
            }
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.SetPrimitiveTopology(PrimitiveTopology.TriangleList);
        GraphicsDevice.SetVertexShader(_vs);
        GraphicsDevice.SetPixelShader(_ps);
        GraphicsDevice.SetInputLayout(_inputLayout);
        GraphicsDevice.SetVertexBuffer(_vb, _vb.ByteStride, 0, 0);
        GraphicsDevice.SetIndexBuffer(_ib, IndexFormat.UInt16, 0);
        GraphicsDevice.DrawIndexed(_ib.Count, 0, 0);
    }

    protected override void UnloadContent()
    {
        _inputLayout?.Dispose();
        _ps?.Dispose();
        _vs?.Dispose();
        _ib?.Dispose();
        _vb?.Dispose();
    }

    protected override void Cleanup()
    {
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
                => new Graphics.Direct3D11.D3D11GraphicsDevice(window.WindowHandle, window.ClientSize),
#endif
#if SUPPORT_VULKAN
            //GraphicsAPI.Vulkan
            //    => new VulkanGraphicsDevice(),
#endif
            _ => throw new PlatformNotSupportedException($"{api} is not supported on this platform.")
        };
    }
}
