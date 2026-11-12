using Vortice.Mathematics;

namespace Torture.Core.Graphics;

/// <summary>
/// Represents a low-level graphics device abstraction supporting modern rendering and compute pipelines.
/// </summary>
public abstract class GraphicsDevice : IDisposable
{
    /// <summary>
    /// Gets the active graphics backend type.
    /// </summary>
    public virtual GraphicsAPI Backend { get; }

    /// <summary>
    /// Gets the current render target size in pixels.
    /// </summary>
    public virtual SizeI Size { get; protected set; }

    /// <summary>
    /// Gets or sets the active viewport.
    /// </summary>
    public virtual Viewport Viewport { get; protected set; }

    /// <summary>
    /// Gets or sets the active scissor rectangle.
    /// </summary>
    public virtual RectI ScissorRect { get; protected set; }

    /// <summary>
    /// Gets or sets whether vertical synchronization (VSync) is enabled.
    /// </summary>
    public virtual bool VSync { get; set; }

    public virtual Color4 ClearColor { get; set; }

    #region Window & Display State Notifications

    /// <summary>
    /// Resizes the swap chain and size-dependent back buffers when the window pixel size changes.
    /// </summary>
    public abstract void OnResize(int width, int height);

    /// <summary>
    /// Notifies the device that the window moved to new desktop coordinates (used to update HDR/ColorSpace).
    /// </summary>
    public abstract void OnWindowMoved(RectI bounds);

    /// <summary>
    /// Notifies the device that the window display mode changed (Windowed, BorderlessFullscreen, or Fullscreen).
    /// </summary>
    public abstract void OnWindowModeChanged(WindowMode mode);

    /// <summary>
    /// Notifies the device that the window was minimized, allowing driver resource trimming or suspension.
    /// </summary>
    public abstract void OnMinimized();

    /// <summary>
    /// Notifies the device that the window was restored from a minimized state.
    /// </summary>
    public abstract void OnRestored();

    #endregion

    #region Frame Lifecycle & Render Target Control

    /// <summary>
    /// Acquires the next swap chain image and prepares the command context for recording a frame.
    /// </summary>
    public abstract bool BeginFrame();

    /// <summary>
    /// Clears the active render targets and depth-stencil buffer.
    /// </summary>
    public abstract void Clear(in Color4 color, ClearFlags flags = ClearFlags.All, float depth = 1.0f, byte stencil = 0);

    /// <summary>
    /// Submits recorded commands to the GPU queue and presents the swap chain image.
    /// </summary>
    public abstract bool EndFrame();

    /// <summary>
    /// Blocks the calling thread until all submitted GPU work has finished executing.
    /// </summary>
    public abstract void WaitIdle();

    #endregion

    #region Resource Creation & Binding

    /// <summary>
    /// Creates a GPU buffer and initializes it with the provided span of unmanaged structs.
    /// </summary>
    public abstract Buffer CreateBuffer<T>(ReadOnlySpan<T> data, in BufferDescription description)
        where T : unmanaged;

    /// <summary>
    /// TBD:
    /// </summary>
    public abstract Buffer CreateBuffer(in BufferDescription description);

    /// <summary>
    /// TBD:
    /// </summary>
    public abstract Shader CreateShader(in ShaderDescription description);

    /// <summary>
    /// TBD:
    /// </summary>
    public abstract InputLayout CreateInputLayout(in VertexLayout description, Shader shader);

    /// <summary>
    /// Binds a vertex buffer to the input assembler stage.
    /// <param name="offset">
    /// Pointer to the first element of an array of offsets (one for each vertex buffer where
    /// the ith offset corresponds to the ith vertex buffer). This is an offset, in bytes,
    /// from the start of the vertex buffer to the position in the vertex buffer from which
    /// the input assembly should start reading the vertex data.You would use this
    /// if you wanted to skip over some vertex data at the front of the vertex buffer.
    /// </param>
    /// </summary>
    public abstract void SetVertexBuffer(Buffer buffer, uint stride, uint slot = 0, uint offset = 0);

    /// <summary>
    /// Binds an index buffer to the input assembler stage.
    /// <paramref name="offset">
    /// This is an offset, in bytes, from the start of the index buffer to the position in the index buffer
    /// from which the input assembly should start reading the index data.
    /// </paramref>
    /// </summary>
    public abstract void SetIndexBuffer(Buffer buffer, IndexFormat format = IndexFormat.UInt32, uint offset = 0);

    /// <summary>
    /// TBD:
    /// </summary>
    public abstract void SetVertexShader(Shader shader);

    /// <summary>
    /// TBD:
    /// </summary>
    public abstract void SetPixelShader(Shader shader);

    /// <summary>
    /// TBD:
    /// </summary>
    public abstract void SetInputLayout(InputLayout inputLayout);

    /// <summary>
    /// TBD:
    /// </summary>
    public abstract void SetPrimitiveTopology(PrimitiveTopology primitiveTopology);

    #endregion

    #region Drawing Commands (Direct, Instanced, Indirect)

    /// <summary>
    /// Draws non-indexed, non-instanced primitives.
    /// </summary>
    public abstract void Draw(uint vertexCount, uint startVertexLocation = 0);

    /// <summary>
    /// Draws indexed, non-instanced primitives.
    /// </summary>
    public abstract void DrawIndexed(uint indexCount, uint startIndexLocation = 0, int baseVertexLocation = 0);

    /// <summary>
    /// Draws non-indexed, instanced primitives.
    /// </summary>
    public abstract void DrawInstanced(
        uint vertexCountPerInstance,
        uint instanceCount,
        uint startVertexLocation = 0,
        uint startInstanceLocation = 0);

    /// <summary>
    /// Draws indexed, instanced primitives.
    /// </summary>
    public abstract void DrawIndexedInstanced(
        uint indexCountPerInstance,
        uint instanceCount,
        uint startIndexLocation = 0,
        int baseVertexLocation = 0,
        uint startInstanceLocation = 0);

    /// <summary>
    /// Draws non-indexed primitives with parameters generated on the GPU in an indirect arguments buffer.
    /// </summary>
    public abstract void DrawIndirect(Buffer buffer, uint alignedByteOffset = 0);

    /// <summary>
    /// Draws indexed primitives with parameters generated on the GPU in an indirect arguments buffer.
    /// </summary>
    public abstract void DrawIndexedIndirect(Buffer buffer, uint alignedByteOffset = 0);

    #endregion

    #region Compute Commands

    /// <summary>
    /// Executes a compute shader using the specified thread group counts.
    /// </summary>
    public abstract void Dispatch(uint threadGroupCountX, uint threadGroupCountY = 1, uint threadGroupCountZ = 1);

    /// <summary>
    /// Executes a compute shader with thread group counts generated on the GPU in an indirect arguments buffer.
    /// </summary>
    public abstract void DispatchIndirect(Buffer buffer, uint alignedByteOffset = 0);

    #endregion

    #region Disposing

    protected bool _disposed = false;

    public abstract void Dispose();

    #endregion
}
