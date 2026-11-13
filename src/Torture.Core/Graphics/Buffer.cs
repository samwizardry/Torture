namespace Torture.Core.Graphics;

public abstract class Buffer : IDisposable
{
    protected bool _disposed;

    public uint Count { get; protected set; }

    public uint Size { get; protected set; }

    public uint Stride { get; protected set; }

    public abstract void UpdateBuffer<T>(ReadOnlySpan<T> data, uint offset = 0)
        where T : unmanaged;

    public abstract void Dispose();
}
