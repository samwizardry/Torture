using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.Direct3D11.Debug;
using Vortice.DXGI;
using Vortice.DXGI.Debug;
using Vortice.Mathematics;

using static Vortice.Direct3D11.D3D11;
using static Vortice.DXGI.DXGI;

namespace Torture.Graphics.Direct3D11;

/// <inheritdoc />
public sealed class D3D11GraphicsDevice : Core.Graphics.GraphicsDevice
{
    private static readonly FeatureLevel[] FeatureLevels = [
        FeatureLevel.Level_11_1,
        FeatureLevel.Level_11_0];

    private const int BufferCount = 2;

    private readonly bool _tearingSupported;
    private readonly SwapChainFlags _swapChainFlags;
    private readonly Format _colorFormat;
    private readonly Format _depthStencilFormat;

    private readonly IDXGIFactory7 _factory;
    private readonly ID3D11Device5 _device;
    private readonly ID3D11DeviceContext4 _imContext;
    private readonly IDXGISwapChain1 _swapChain;
#if DEBUG
    private readonly ID3D11InfoQueue? _infoQueue;
#endif
    private ID3D11Texture2D _rtvBackBuffer = null!;
    private ID3D11RenderTargetView _renderTargetView = null!;
    private ID3D11Texture2D _depthStencilBuffer = null!;
    private ID3D11DepthStencilView _depthStencilView = null!;

    public D3D11GraphicsDevice(nint hwnd, SizeI size, Format colorFormat = Format.B8G8R8A8_UNorm, Format depthStencilFormat = Format.D24_UNorm_S8_UInt)
    {
        try
        {
            Size = size;
            
            /*
             * Родной формат для Windows (DWM и Direct2D): оконный менеджер Windows (Desktop Window Manager) внутри себя работает именно в BGRA8
             * Для SwapChain (буфера экрана) всегда берём B8G8R8A8_UNorm.
             */
            _colorFormat = colorFormat;

            /*
             * Буфер глубины и трафарета (Depth-Stencil Buffer) решает две задачи:
             * Depth (Z-буфер): хранит расстояние от камеры до каждого пикселя в диапазоне [0.0 .. 1.0], чтобы близкие объекты перекрывали дальние.
             * Stencil (Трафарет): хранит 8-битное целое число (0..255) на каждый пиксель. Используется для спецэффектов: масок интерфейса, обводки выделенных объектов (Outlines), порталов, декалей.
             *
             * Формат                       Разрядность Depth   Разрядность Stencil
             * Format.D24_UNorm_S8_UInt     24 бита             8 бит (0..255)
             *      Классический универсальный выбор, если тебе нужен Stencil (трафарет) и сцена небольшая/средняя.
             *
             * Format.D32_Float             32 бита             Нет (0 бит)
             *      Современный стандарт для 3D-движков и больших открытых пространств, когда Stencil не нужен. За те же 4 байта на пиксель даёт гораздо более высокую точность глубины и спасает от мерцания текстур вдалеке (Z-fighting).
             *
             * Format.D32_Float_S8X24_UInt  32 бита             8 бит (+24 бита пустых для выравнивания)
             *      Когда нужно и 32-битная точность глубины, и Stencil, но весит в 2 раза больше (8 байт на пиксель).
             *
             * Format.Unknown               Нет                 Нет
             *      Для чистых 2D-игр без проверки глубины (экономит видеопамять).
             */
            _depthStencilFormat = depthStencilFormat;

            _factory = CreateDXGIFactory1<IDXGIFactory7>();

            _tearingSupported = _factory.PresentAllowTearing;

            using (IDXGIAdapter1 adapter = GetHardwareAdapter())
            {
                DeviceCreationFlags deviceCreationFlags = DeviceCreationFlags.BgraSupport;

#if DEBUG
                if (SdkLayersAvailable())
                {
                    deviceCreationFlags |= DeviceCreationFlags.Debug;
                }
#endif
                ID3D11Device? device = null;
                ID3D11DeviceContext? imContext = null;

                if (D3D11CreateDevice(
                    adapter,
                    DriverType.Unknown,
                    deviceCreationFlags,
                    FeatureLevels,
                    out device,
                    out FeatureLevel featureLevel,
                    out imContext).Failure)
                {
                    device?.Dispose();
                    imContext?.Dispose();

                    // If the initialization fails, fall back to the WARP device.
                    // For more information on WARP, see:
                    // http://go.microsoft.com/fwlink/?LinkId=286690
                    D3D11CreateDevice(
                        IntPtr.Zero,
                        DriverType.Warp,
                        deviceCreationFlags,
                        FeatureLevels,
                        out device,
                        out featureLevel,
                        out imContext).CheckError();
                }

                _device = device.QueryInterface<ID3D11Device5>();
                _imContext = imContext.QueryInterface<ID3D11DeviceContext4>();

                device.Dispose();
                imContext.Dispose();

                if (!FeatureLevels.Contains(featureLevel))
                {
                    throw new NotSupportedException("Direct3D Feature Level 11 not supported.");
                }
            }

#if DEBUG
            _infoQueue = _device.QueryInterfaceOrNull<ID3D11InfoQueue>();
            if (_infoQueue != null)
            {
                _infoQueue.SetBreakOnSeverity(MessageSeverity.Corruption, true);
                _infoQueue.SetBreakOnSeverity(MessageSeverity.Error, true);
            }
#endif

            _swapChainFlags = SwapChainFlags.None;
            if (_tearingSupported)
            {
                _swapChainFlags |= SwapChainFlags.AllowTearing;
            }

            SwapChainDescription1 swapChainDescription = new()
            {
                Width = (uint)size.Width,
                Height = (uint)size.Height,
                Format = _colorFormat,
                Stereo = false,
                SampleDescription = SampleDescription.Default,
                BufferUsage = Usage.RenderTargetOutput,
                BufferCount = BufferCount,
                Scaling = Scaling.Stretch,
                SwapEffect = SwapEffect.FlipDiscard,
                AlphaMode = AlphaMode.Ignore,
                Flags = _swapChainFlags
            };

            SwapChainFullscreenDescription fullscreenDescription = new()
            {
                RefreshRate = new Rational(0, 0),
                ScanlineOrdering = ModeScanlineOrder.Unspecified,
                Scaling = ModeScaling.Unspecified,
                Windowed = true
            };

            _swapChain = _factory.CreateSwapChainForHwnd(_device, hwnd, swapChainDescription, fullscreenDescription);
            _factory.MakeWindowAssociation(hwnd, WindowAssociationFlags.IgnoreAltEnter)
                .CheckError();

            // Handle color space settings for HDR
            UpdateColorSpace();

            OnResize(size.Width, size.Height);
        }
        catch
        {
            Dispose();
            throw;
        }
    }

    /// <inheritdoc />
    public override Core.Graphics.GraphicsAPI Backend => Core.Graphics.GraphicsAPI.D3D11;

    /// <inheritdoc />
    public override SizeI Size { get; protected set; }

    /// <inheritdoc />
    public override Viewport Viewport { get; protected set; }

    /// <inheritdoc />
    public override RectI ScissorRect { get; protected set; }

    /// <inheritdoc />
    public override bool VSync { get; set; }

    /// <inheritdoc />
    public override Color4 ClearColor { get; set; } = Colors.CornflowerBlue;

    #region Window & Display State Notifications

    /// <inheritdoc />
    public override void OnResize(int width, int height)
    {
        Size = new SizeI(width, height);
        Viewport = new Viewport(width, height);
        ScissorRect = new RectI(width, height);

        _imContext.OMSetRenderTargets((ID3D11RenderTargetView)null!);

        // Release the old views, as they hold references to the buffers we
        // will be destroying. Also release the old depth/stencil buffer.
        _renderTargetView?.Dispose();
        _rtvBackBuffer?.Dispose();
        _depthStencilView?.Dispose();
        _depthStencilBuffer?.Dispose();

        // Resize the swap chain and recreate the render target view.
        _swapChain.ResizeBuffers(BufferCount, (uint)width, (uint)height, _colorFormat, _swapChainFlags).CheckError();

        _rtvBackBuffer = _swapChain.GetBuffer<ID3D11Texture2D>(0);
        _renderTargetView = _device.CreateRenderTargetView(_rtvBackBuffer);

        // Create the depth/stencil buffer and view.
        _depthStencilBuffer = _device.CreateTexture2D(_depthStencilFormat, (uint)width, (uint)height,
                arraySize: 1, mipLevels: 1, initialData: null,
                bindFlags: BindFlags.DepthStencil, miscFlags: ResourceOptionFlags.None,
                usage: ResourceUsage.Default, cpuAccessFlags: CpuAccessFlags.None);
        _depthStencilView = _device.CreateDepthStencilView(_depthStencilBuffer, new DepthStencilViewDescription(_depthStencilBuffer, DepthStencilViewDimension.Texture2D));

        // Bind the render target view and depth/stencil view to the pipeline.
        _imContext.OMSetRenderTargets(_renderTargetView, _depthStencilView);

        // Set the viewport transform.
        _imContext.RSSetViewport(0, 0, (float)width, (float)height, 0.0f, 1.0f);
    }

    /// <inheritdoc />
    public override void OnWindowMoved(RectI bounds) { }

    /// <inheritdoc />
    public override void OnWindowModeChanged(Core.WindowMode mode) { }

    /// <inheritdoc />
    public override void OnMinimized() { }

    /// <inheritdoc />
    public override void OnRestored() { }

    #endregion

    #region Frame Lifecycle & Render Target Control

    /// <inheritdoc />
    public override bool BeginFrame()
    {
        _imContext.ClearRenderTargetView(_renderTargetView, ClearColor);
        _imContext.ClearDepthStencilView(_depthStencilView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1.0f, 0);

        _imContext.OMSetRenderTargets(_renderTargetView, _depthStencilView);
        _imContext.RSSetViewport(Viewport);
        _imContext.RSSetScissorRect(ScissorRect);

        return true;
    }

    /// <inheritdoc />
    public override void Clear(in Color4 color, Core.Graphics.ClearFlags flags = Core.Graphics.ClearFlags.All, float depth = 1.0f, byte stencil = 0)
    {
        if ((flags & Core.Graphics.ClearFlags.Target) == Core.Graphics.ClearFlags.Target)
        {
            _imContext.ClearRenderTargetView(_renderTargetView, color);
        }

        DepthStencilClearFlags dsClearFlags = DepthStencilClearFlags.None;
        if ((flags & Core.Graphics.ClearFlags.Depth) == Core.Graphics.ClearFlags.Depth) dsClearFlags |= DepthStencilClearFlags.Depth;
        if ((flags & Core.Graphics.ClearFlags.Stencil) == Core.Graphics.ClearFlags.Stencil) dsClearFlags |= DepthStencilClearFlags.Stencil;

        if (dsClearFlags != DepthStencilClearFlags.None)
        {
            _imContext.ClearDepthStencilView(_depthStencilView, dsClearFlags, depth, stencil);
        }
    }

    /// <inheritdoc />
    public override bool EndFrame()
    {
        PresentFlags presentFlags = PresentFlags.None;
        uint syncInterval = 0u;

        if (VSync)
        {
            syncInterval = 1u;
        }
        else if (_tearingSupported)
        {
            presentFlags |= PresentFlags.AllowTearing;
        }

        var result = _swapChain.Present(syncInterval, presentFlags);
        FlushDebugMessages();
        result.CheckError();
        return true;
    }

    /// <inheritdoc />
    public override void WaitIdle()
    {
        _imContext.Flush();
    }

    #endregion

    #region Resource Creation & Binding

    /// <inheritdoc />
    public override Core.Graphics.Buffer CreateBuffer<T>(ReadOnlySpan<T> data, in Core.Graphics.BufferDescription description)
    {
        return D3D11Buffer.Create(_device, data, description);
    }

    /// <inheritdoc />
    public override Core.Graphics.Buffer CreateBuffer(in Core.Graphics.BufferDescription description)
    {
        return D3D11Buffer.Create(_device, description);
    }

    /// <inheritdoc />
    public override Core.Graphics.Shader CreateShader(in Core.Graphics.ShaderDescription description)
    {
        return new D3D11Shader(_device, description);
    }

    /// <inheritdoc />
    public override Core.Graphics.InputLayout CreateInputLayout(in Core.Graphics.VertexLayout description, Core.Graphics.Shader shader)
    {
        return new D3D11InputLayout(_device, description, (D3D11Shader)shader);
    }

    /// <inheritdoc />
    public override void SetVertexBuffer(Core.Graphics.Buffer buffer, uint stride, uint slot = 0, uint offset = 0)
    {
        var b = (D3D11Buffer)buffer;
        _imContext.IASetVertexBuffer(slot, b.Native, stride, offset);
    }

    /// <inheritdoc />
    public override void SetIndexBuffer(Core.Graphics.Buffer buffer, Core.Graphics.IndexFormat format = Core.Graphics.IndexFormat.UInt32, uint offset = 0)
    {
        var b = (D3D11Buffer)buffer;
        _imContext.IASetIndexBuffer(b.Native, format.ToDXGIFormat(), offset);
    }

    /// <inheritdoc />
    public override void SetVertexShader(Core.Graphics.Shader shader)
    {
        _imContext.VSSetShader((ID3D11VertexShader)((D3D11Shader)shader).Native);
    }

    /// <inheritdoc />
    public override void SetPixelShader(Core.Graphics.Shader shader)
    {
        _imContext.PSSetShader((ID3D11PixelShader)((D3D11Shader)shader).Native);
    }

    /// <inheritdoc />
    public override void SetInputLayout(Core.Graphics.InputLayout inputLayout)
    {
        _imContext.IASetInputLayout(((D3D11InputLayout)inputLayout).Native);
    }

    /// <inheritdoc />
    public override void SetPrimitiveTopology(Core.Graphics.PrimitiveTopology primitiveTopology)
    {
        _imContext.IASetPrimitiveTopology(primitiveTopology.ToD3D11PrimitiveTopology());
    }

    #endregion

    #region Drawing Commands (Direct, Instanced, Indirect)

    /// <inheritdoc />
    public override void Draw(uint vertexCount, uint startVertexLocation = 0)
    {
        _imContext.Draw(vertexCount, startVertexLocation);
    }

    /// <inheritdoc />
    public override void DrawIndexed(uint indexCount, uint startIndexLocation = 0, int baseVertexLocation = 0)
    {
        _imContext.DrawIndexed(indexCount, startIndexLocation, baseVertexLocation);
    }

    /// <inheritdoc />
    public override void DrawInstanced(
        uint vertexCountPerInstance,
        uint instanceCount,
        uint startVertexLocation = 0,
        uint startInstanceLocation = 0)
    { }

    /// <inheritdoc />
    public override void DrawIndexedInstanced(
        uint indexCountPerInstance,
        uint instanceCount,
        uint startIndexLocation = 0,
        int baseVertexLocation = 0,
        uint startInstanceLocation = 0)
    { }

    /// <inheritdoc />
    public override void DrawIndirect(Core.Graphics.Buffer buffer, uint alignedByteOffset = 0)
    {
    }

    /// <inheritdoc />
    public override void DrawIndexedIndirect(Core.Graphics.Buffer buffer, uint alignedByteOffset = 0)
    {
    }

    #endregion

    #region Compute Commands

    /// <inheritdoc />
    public override void Dispatch(uint threadGroupCountX, uint threadGroupCountY = 1, uint threadGroupCountZ = 1) { }

    /// <inheritdoc />
    public override void DispatchIndirect(Core.Graphics.Buffer buffer, uint alignedByteOffset = 0) { }

    #endregion

    #region Disposing

    /// <inheritdoc />
    public override void Dispose()
    {
        if (_disposed)
            return;

        _depthStencilView?.Dispose();
        _depthStencilBuffer?.Dispose();
        _renderTargetView?.Dispose();
        _rtvBackBuffer?.Dispose();

        if (_imContext != null && _imContext.NativePointer != IntPtr.Zero)
        {
            _imContext.ClearState();
            _imContext.Flush();
            _imContext.Dispose();
        }

        _swapChain?.Dispose();

#if DEBUG
        if (_device != null)
        {
            if (_device.QueryInterfaceOrNull<ID3D11Debug>() is ID3D11Debug d3d11Debug)
            {
                d3d11Debug.ReportLiveDeviceObjects(ReportLiveDeviceObjectFlags.Detail | ReportLiveDeviceObjectFlags.IgnoreInternal);
                FlushDebugMessages();
                d3d11Debug.Dispose();
            }
        }

        _infoQueue?.Dispose();
#endif

        _device?.Dispose();
        _factory?.Dispose();

#if DEBUG
        if (DXGIGetDebugInterface1(out IDXGIDebug1? dxgiDebug).Success && dxgiDebug != null)
        {
            if (DXGIGetDebugInterface1(out IDXGIInfoQueue? dxgiInfoQueue).Success && dxgiInfoQueue != null)
            {
                dxgiInfoQueue.ClearStoredMessages(DebugAll);
                dxgiDebug.ReportLiveObjects(DebugAll, ReportLiveObjectFlags.Detail | ReportLiveObjectFlags.IgnoreInternal);

                ulong count = dxgiInfoQueue.GetNumStoredMessages(DebugAll);
                for (ulong i = 0; i < count; i++)
                {
                    InfoQueueMessage msg = dxgiInfoQueue.GetMessage(DebugAll, i);
                    Console.WriteLine($"[DXGI {msg.Severity}] ({msg.Id}): {msg.Description}");
                }
                dxgiInfoQueue.Dispose();
            }
            dxgiDebug.Dispose();
        }
#endif

        _disposed = true;
    }

    #endregion

    #region Private

    private IDXGIAdapter1 GetHardwareAdapter()
    {
        for (uint adapterIndex = 0;
                _factory.EnumAdapterByGpuPreference(adapterIndex, GpuPreference.HighPerformance, out IDXGIAdapter1? adapter).Success;
                adapterIndex++)
        {
            if (adapter == null)
            {
                continue;
            }

            if ((adapter.Description1.Flags & AdapterFlags.Software) != AdapterFlags.None)
            {
                adapter.Dispose();
                continue;
            }

            return adapter;
        }

        for (uint adapterIndex = 0;
            _factory.EnumAdapters1(adapterIndex, out IDXGIAdapter1? adapter).Success;
            adapterIndex++)
        {
            if ((adapter.Description1.Flags & AdapterFlags.Software) != AdapterFlags.None)
            {
                adapter.Dispose();
                continue;
            }

            return adapter;
        }

        throw new InvalidOperationException("Cannot detect Direct3D11 adapter.");
    }

    private void UpdateColorSpace()
    {
        // TBD:
    }

    [System.Diagnostics.Conditional("DEBUG")]
    private void FlushDebugMessages()
    {
#if DEBUG
        if (_infoQueue == null || _infoQueue.NativePointer == nint.Zero)
        {
            return;
        }

        ulong count = _infoQueue.NumStoredMessages;
        for (ulong i = 0; i < count; i++)
        {
            Message msg = _infoQueue.GetMessage(i);
            ConsoleColor prevColor = Console.ForegroundColor;
            Console.ForegroundColor = msg.Severity switch
            {
                MessageSeverity.Corruption or MessageSeverity.Error => ConsoleColor.Red,
                MessageSeverity.Warning => ConsoleColor.Yellow,
                _ => ConsoleColor.Gray
            };
            Console.WriteLine($"[D3D11 {msg.Severity}] ({msg.Id}): {msg.Description}");
            Console.ForegroundColor = prevColor;
        }
        _infoQueue.ClearStoredMessages();
#endif
    }

    #endregion
}