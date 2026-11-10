// Copyright (c) Amer Koleci and contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Numerics;
using System.Runtime.CompilerServices;

using SharpGen.Runtime;

using Torture.Core;
using Torture.Core.Graphics;

using Vortice;
using Vortice.D3DCompiler;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.Direct3D11.Debug;
using Vortice.DXGI;
using Vortice.DXGI.Debug;
using Vortice.Mathematics;

using static Vortice.Direct3D11.D3D11;
using static Vortice.DXGI.DXGI;

namespace Torture.Graphics.Direct3D11;

public sealed class D3D11GraphicsDevice : IGraphicsDevice
{
    /// <summary>
    /// Gets the active graphics backend type.
    /// </summary>
    public GraphicsBackend Backend { get; }

    /// <summary>
    /// Gets the current render target size in pixels.
    /// </summary>
    public SizeI Size { get; }

    /// <summary>
    /// Gets or sets the active viewport.
    /// </summary>
    public Viewport Viewport { get; set; }

    /// <summary>
    /// Gets or sets the active scissor rectangle.
    /// </summary>
    public RectI ScissorRect { get; set; }

    /// <summary>
    /// Gets or sets whether vertical synchronization (VSync) is enabled.
    /// </summary>
    public bool VSync { get; set; }

    public void Dispose()
    {

    }

    // ========================================================================
    // Window & Display State Notifications
    // ========================================================================

    /// <summary>
    /// Resizes the swap chain and size-dependent back buffers when the window pixel size changes.
    /// </summary>
    public void OnResize(int width, int height) { }

    /// <summary>
    /// Notifies the device that the window moved to new desktop coordinates (used to update HDR/ColorSpace).
    /// </summary>
    public void OnWindowMoved(RectI bounds) { }

    /// <summary>
    /// Notifies the device that the window display mode changed (Windowed, BorderlessFullscreen, or Fullscreen).
    /// </summary>
    public void OnWindowModeChanged(WindowMode mode) { }

    /// <summary>
    /// Notifies the device that the window was minimized, allowing driver resource trimming or suspension.
    /// </summary>
    public void OnMinimized() { }

    /// <summary>
    /// Notifies the device that the window was restored from a minimized state.
    /// </summary>
    public void OnRestored() { }

    // ========================================================================
    // Frame Lifecycle & Render Target Control
    // ========================================================================

    /// <summary>
    /// Acquires the next swap chain image and prepares the command context for recording a frame.
    /// </summary>
    public bool BeginFrame() { return true; }

    /// <summary>
    /// Clears the active render targets and depth-stencil buffer.
    /// </summary>
    public void Clear(in Color4 color, ClearFlags flags = ClearFlags.All, float depth = 1.0f, byte stencil = 0) { }

    /// <summary>
    /// Submits recorded commands to the GPU queue and presents the swap chain image.
    /// </summary>
    public bool EndFrame() { return true; }

    /// <summary>
    /// Blocks the calling thread until all submitted GPU work has finished executing.
    /// </summary>
    public void WaitIdle() { }

    // ========================================================================
    // Resource Creation & Binding
    // ========================================================================

    /// <summary>
    /// Binds a vertex buffer to the input assembler stage.
    /// </summary>
    public void SetVertexBuffer(IBuffer buffer, uint stride, uint slot = 0, uint offsetInBytes = 0) { }

    /// <summary>
    /// Binds an index buffer to the input assembler stage.
    /// </summary>
    public void SetIndexBuffer(IBuffer buffer, IndexFormat format = IndexFormat.UInt32, uint offsetInBytes = 0) { }

    // ========================================================================
    // Drawing Commands (Direct, Instanced, Indirect)
    // ========================================================================

    /// <summary>
    /// Draws non-indexed, non-instanced primitives.
    /// </summary>
    public void Draw(uint vertexCount, uint startVertexLocation = 0) { }

    /// <summary>
    /// Draws indexed, non-instanced primitives.
    /// </summary>
    public void DrawIndexed(uint indexCount, uint startIndexLocation = 0, int baseVertexLocation = 0) { }

    /// <summary>
    /// Draws non-indexed, instanced primitives.
    /// </summary>
    public void DrawInstanced(
        uint vertexCountPerInstance,
        uint instanceCount,
        uint startVertexLocation = 0,
        uint startInstanceLocation = 0)
    { }

    /// <summary>
    /// Draws indexed, instanced primitives.
    /// </summary>
    public void DrawIndexedInstanced(
        uint indexCountPerInstance,
        uint instanceCount,
        uint startIndexLocation = 0,
        int baseVertexLocation = 0,
        uint startInstanceLocation = 0)
    { }

    /// <summary>
    /// Draws non-indexed primitives with parameters generated on the GPU in an indirect arguments buffer.
    /// </summary>
    public void DrawIndirect(IBuffer buffer, uint alignedByteOffset = 0) { }

    /// <summary>
    /// Draws indexed primitives with parameters generated on the GPU in an indirect arguments buffer.
    /// </summary>
    public void DrawIndexedIndirect(IBuffer buffer, uint alignedByteOffset = 0) { }

    // ========================================================================
    // Compute Commands
    // ========================================================================

    /// <summary>
    /// Executes a compute shader using the specified thread group counts.
    /// </summary>
    public void Dispatch(uint threadGroupCountX, uint threadGroupCountY = 1, uint threadGroupCountZ = 1) { }

    /// <summary>
    /// Executes a compute shader with thread group counts generated on the GPU in an indirect arguments buffer.
    /// </summary>
    public void DispatchIndirect(IBuffer buffer, uint alignedByteOffset = 0) { }
}