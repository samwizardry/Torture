using Torture.Core.Graphics;

namespace Torture.Graphics.Direct3D11;

/// <inheritdoc />
public sealed class Buffer : IBuffer
{
    /// <inheritdoc />
    public uint SizeInBytez => throw new NotImplementedException();

    /// <inheritdoc />
    public BufferUsage Usage => throw new NotImplementedException();

    /// <inheritdoc />
    public bool IsDynamic => throw new NotImplementedException();

    /// <inheritdoc />
    public void UpdateBuffer<T>(IBuffer buffer, ReadOnlySpan<T> data, uint offsetInBytes = 0) where T : unmanaged
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
