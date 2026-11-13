namespace Torture.Core.Graphics;

public abstract class Buffer : IDisposable
{
    protected bool _disposed;

    /// <summary>
    /// Buffer size in bytes.
    /// </summary>
    public uint ByteSize { get; protected set; }

    /// <summary>
    /// Buffer stride in bytes.
    /// </summary>
    public uint ByteStride { get; protected set; }

    /// <summary>
    /// Buffer element count.
    /// </summary>
    public uint Count => ByteSize / ByteStride;

    public abstract void UpdateBuffer<T>(ReadOnlySpan<T> data, uint byteOffset = 0)
        where T : unmanaged;

    public abstract void Dispose();
}
