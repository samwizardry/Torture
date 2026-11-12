namespace Torture.Core.Graphics;

public abstract class Buffer : IDisposable
{
    protected bool _disposed;

    public abstract void UpdateBuffer<T>(ReadOnlySpan<T> data, uint offset = 0)
        where T : unmanaged;

    public abstract void Dispose();
}
