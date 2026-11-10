using Torture.Core.Graphics;

namespace Torture.Graphics.Direct3D11;

internal class Buffer : IBuffer
{
    public uint Size => throw new NotImplementedException();

    public BufferUsage Usage => throw new NotImplementedException();

    public bool IsDynamic => throw new NotImplementedException();

    public IBuffer CreateBuffer<T>(ReadOnlySpan<T> data, BufferUsage usage, bool isDynamic = false) where T : unmanaged
    {
        throw new NotImplementedException();
    }

    public IBuffer CreateBuffer(uint sizeInBytes, BufferUsage usage, bool isDynamic = false)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public void UpdateBuffer<T>(IBuffer buffer, ReadOnlySpan<T> data, uint offsetInBytes = 0) where T : unmanaged
    {
        throw new NotImplementedException();
    }
}
