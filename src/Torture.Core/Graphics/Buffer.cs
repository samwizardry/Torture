namespace Torture.Core.Graphics;

/// <summary>
/// Represents a hardware buffer residing in GPU memory.
/// </summary>
public abstract class Buffer : IDisposable
{
    protected bool _disposed;

    /// <summary>
    /// Updates the contents of a dynamic or updatable GPU buffer.
    /// </summary>
    public abstract void UpdateBuffer<T>(ReadOnlySpan<T> data, uint offset = 0)
        where T : unmanaged;

    public abstract void Dispose();
}
