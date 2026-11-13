using Vortice.Mathematics;

namespace Torture.Core.Graphics;

public abstract class GraphicsDevice : IDisposable
{
    public virtual GraphicsAPI API { get; }

    public virtual SizeI ClientSize { get; protected set; }

    public virtual Viewport Viewport { get; protected set; }

    public virtual RectI ScissorRect { get; protected set; }

    public virtual bool VSync { get; set; }

    public virtual Color4 ClearColor { get; set; }

    #region Window & Display State Notifications

    public abstract void OnResize(int width, int height);

    public abstract void OnWindowMoved(RectI bounds);

    public abstract void OnWindowModeChanged(WindowMode mode);

    public abstract void OnMinimized();

    public abstract void OnRestored();

    #endregion

    #region Frame Lifecycle & Render Target Control

    public abstract bool BeginFrame();

    public abstract void Clear();

    public abstract bool EndFrame();

    public abstract void WaitIdle();

    #endregion

    #region Resource Creation & Binding

    public abstract Buffer CreateBuffer<T>(ReadOnlySpan<T> data, in BufferDescription desc)
        where T : unmanaged;

    public abstract Buffer CreateBuffer(in BufferDescription desc);

    public abstract Shader CreateShader(in ShaderDescription desc);

    public abstract InputLayout CreateInputLayout(in VertexLayout layout, Shader shader);

    public abstract void SetVertexBuffer(Buffer buffer, uint byteStride, uint slot = 0, uint byteOffset = 0);

    public abstract void SetIndexBuffer(Buffer buffer, IndexFormat format = IndexFormat.UInt32, uint byteOffset = 0);

    public abstract void SetVertexShader(Shader shader);

    public abstract void SetPixelShader(Shader shader);

    public abstract void SetInputLayout(InputLayout inputLayout);

    public abstract void SetPrimitiveTopology(PrimitiveTopology primitiveTopology);

    #endregion

    #region Drawing Commands (Direct, Instanced, Indirect)

    public abstract void Draw(uint vertexCount, uint startVertexLocation = 0);

    public abstract void DrawIndexed(uint indexCount, uint startIndexLocation = 0, int baseVertexLocation = 0);

    public abstract void DrawInstanced(
        uint vertexCountPerInstance,
        uint instanceCount,
        uint startVertexLocation = 0,
        uint startInstanceLocation = 0);

    public abstract void DrawIndexedInstanced(
        uint indexCountPerInstance,
        uint instanceCount,
        uint startIndexLocation = 0,
        int baseVertexLocation = 0,
        uint startInstanceLocation = 0);

    public abstract void DrawIndirect(Buffer buffer, uint alignedByteOffset = 0);

    public abstract void DrawIndexedIndirect(Buffer buffer, uint alignedByteOffset = 0);

    #endregion

    #region Compute Commands

    public abstract void Dispatch(uint threadGroupCountX, uint threadGroupCountY = 1, uint threadGroupCountZ = 1);

    public abstract void DispatchIndirect(Buffer buffer, uint alignedByteOffset = 0);

    #endregion

    #region Disposing

    protected bool _disposed = false;

    public abstract void Dispose();

    #endregion
}
