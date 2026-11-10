using Torture.Core;
using Torture.Core.Graphics;

using Vortice.Mathematics;

namespace Torture.Graphics.Vulkan;

public sealed class VulkanGraphicsDevice : IGraphicsDevice
{
    public GraphicsBackend Backend => throw new NotImplementedException();

    public SizeI Size => throw new NotImplementedException();

    public Viewport Viewport { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public RectI ScissorRect { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public bool VSync { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public void BeginFrame()
    {
        throw new NotImplementedException();
    }

    public void Clear(in Color4 color, ClearFlags flags = ClearFlags.All, float depth = 1, byte stencil = 0)
    {
        throw new NotImplementedException();
    }

    public void Dispatch(uint threadGroupCountX, uint threadGroupCountY = 1, uint threadGroupCountZ = 1)
    {
        throw new NotImplementedException();
    }

    public void DispatchIndirect(IBuffer buffer, uint alignedByteOffset = 0)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public void Draw(uint vertexCount, uint startVertexLocation = 0)
    {
        throw new NotImplementedException();
    }

    public void DrawIndexed(uint indexCount, uint startIndexLocation = 0, int baseVertexLocation = 0)
    {
        throw new NotImplementedException();
    }

    public void DrawIndexedIndirect(IBuffer buffer, uint alignedByteOffset = 0)
    {
        throw new NotImplementedException();
    }

    public void DrawIndexedInstanced(uint indexCountPerInstance, uint instanceCount, uint startIndexLocation = 0, int baseVertexLocation = 0, uint startInstanceLocation = 0)
    {
        throw new NotImplementedException();
    }

    public void DrawIndirect(IBuffer buffer, uint alignedByteOffset = 0)
    {
        throw new NotImplementedException();
    }

    public void DrawInstanced(uint vertexCountPerInstance, uint instanceCount, uint startVertexLocation = 0, uint startInstanceLocation = 0)
    {
        throw new NotImplementedException();
    }

    public void EndFrame()
    {
        throw new NotImplementedException();
    }

    public void OnMinimized()
    {
        throw new NotImplementedException();
    }

    public void OnResize(int width, int height)
    {
        throw new NotImplementedException();
    }

    public void OnRestored()
    {
        throw new NotImplementedException();
    }

    public void OnWindowModeChanged(WindowMode mode)
    {
        throw new NotImplementedException();
    }

    public void OnWindowMoved(RectI bounds)
    {
        throw new NotImplementedException();
    }

    public void SetIndexBuffer(IBuffer buffer, IndexFormat format = IndexFormat.UInt32, uint offsetInBytes = 0)
    {
        throw new NotImplementedException();
    }

    public void SetVertexBuffer(IBuffer buffer, uint stride, uint slot = 0, uint offsetInBytes = 0)
    {
        throw new NotImplementedException();
    }

    public void WaitIdle()
    {
        throw new NotImplementedException();
    }

    bool IGraphicsDevice.BeginFrame()
    {
        throw new NotImplementedException();
    }

    bool IGraphicsDevice.EndFrame()
    {
        throw new NotImplementedException();
    }
}
