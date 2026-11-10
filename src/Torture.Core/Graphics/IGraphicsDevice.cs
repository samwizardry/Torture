using Vortice.Mathematics;

namespace Torture.Core.Graphics;

/// <summary>
/// Represents a low-level graphics device abstraction supporting modern rendering and compute pipelines.
/// </summary>
public interface IGraphicsDevice : IDisposable
{
    /// <summary>
    /// Gets the active graphics backend type.
    /// </summary>
    GraphicsBackend Backend { get; }

    /// <summary>
    /// Gets the current render target size in pixels.
    /// </summary>
    SizeI Size { get; }

    /// <summary>
    /// Gets or sets the active viewport.
    /// </summary>
    Viewport Viewport { get; set; }

    /// <summary>
    /// Gets or sets the active scissor rectangle.
    /// </summary>
    RectI ScissorRect { get; set; }

    /// <summary>
    /// Gets or sets whether vertical synchronization (VSync) is enabled.
    /// </summary>
    bool VSync { get; set; }

    // ========================================================================
    // Window & Display State Notifications
    // ========================================================================

    /// <summary>
    /// Resizes the swap chain and size-dependent back buffers when the window pixel size changes.
    /// </summary>
    void OnResize(int width, int height);

    /// <summary>
    /// Notifies the device that the window moved to new desktop coordinates (used to update HDR/ColorSpace).
    /// </summary>
    void OnWindowMoved(RectI bounds);

    /// <summary>
    /// Notifies the device that the window display mode changed (Windowed, BorderlessFullscreen, or Fullscreen).
    /// </summary>
    void OnWindowModeChanged(WindowMode mode);

    /// <summary>
    /// Notifies the device that the window was minimized, allowing driver resource trimming or suspension.
    /// </summary>
    void OnMinimized();

    /// <summary>
    /// Notifies the device that the window was restored from a minimized state.
    /// </summary>
    void OnRestored();

    // ========================================================================
    // Frame Lifecycle & Render Target Control
    // ========================================================================

    /// <summary>
    /// Acquires the next swap chain image and prepares the command context for recording a frame.
    /// </summary>
    bool BeginFrame();

    /// <summary>
    /// Clears the active render targets and depth-stencil buffer.
    /// </summary>
    void Clear(in Color4 color, ClearFlags flags = ClearFlags.All, float depth = 1.0f, byte stencil = 0);

    /// <summary>
    /// Submits recorded commands to the GPU queue and presents the swap chain image.
    /// </summary>
    bool EndFrame();

    /// <summary>
    /// Blocks the calling thread until all submitted GPU work has finished executing.
    /// </summary>
    void WaitIdle();

    // ========================================================================
    // Resource Creation & Binding
    // ========================================================================

    /// <summary>
    /// Binds a vertex buffer to the input assembler stage.
    /// </summary>
    void SetVertexBuffer(IBuffer buffer, uint stride, uint slot = 0, uint offsetInBytes = 0);

    /// <summary>
    /// Binds an index buffer to the input assembler stage.
    /// </summary>
    void SetIndexBuffer(IBuffer buffer, IndexFormat format = IndexFormat.UInt32, uint offsetInBytes = 0);

    // ========================================================================
    // Drawing Commands (Direct, Instanced, Indirect)
    // ========================================================================

    /// <summary>
    /// Draws non-indexed, non-instanced primitives.
    /// </summary>
    void Draw(uint vertexCount, uint startVertexLocation = 0);

    /// <summary>
    /// Draws indexed, non-instanced primitives.
    /// </summary>
    void DrawIndexed(uint indexCount, uint startIndexLocation = 0, int baseVertexLocation = 0);

    /// <summary>
    /// Draws non-indexed, instanced primitives.
    /// </summary>
    void DrawInstanced(
        uint vertexCountPerInstance,
        uint instanceCount,
        uint startVertexLocation = 0,
        uint startInstanceLocation = 0);

    /// <summary>
    /// Draws indexed, instanced primitives.
    /// </summary>
    void DrawIndexedInstanced(
        uint indexCountPerInstance,
        uint instanceCount,
        uint startIndexLocation = 0,
        int baseVertexLocation = 0,
        uint startInstanceLocation = 0);

    /// <summary>
    /// Draws non-indexed primitives with parameters generated on the GPU in an indirect arguments buffer.
    /// </summary>
    void DrawIndirect(IBuffer buffer, uint alignedByteOffset = 0);

    /// <summary>
    /// Draws indexed primitives with parameters generated on the GPU in an indirect arguments buffer.
    /// </summary>
    void DrawIndexedIndirect(IBuffer buffer, uint alignedByteOffset = 0);

    // ========================================================================
    // Compute Commands
    // ========================================================================

    /// <summary>
    /// Executes a compute shader using the specified thread group counts.
    /// </summary>
    void Dispatch(uint threadGroupCountX, uint threadGroupCountY = 1, uint threadGroupCountZ = 1);

    /// <summary>
    /// Executes a compute shader with thread group counts generated on the GPU in an indirect arguments buffer.
    /// </summary>
    void DispatchIndirect(IBuffer buffer, uint alignedByteOffset = 0);
}