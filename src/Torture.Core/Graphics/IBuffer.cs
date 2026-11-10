namespace Torture.Core.Graphics;

/// <summary>
/// Represents a hardware buffer residing in GPU memory.
/// </summary>
public interface IBuffer : IDisposable
{
    /// <summary>
    /// The total size, in bytes.
    /// </summary>
    uint SizeInBytez { get; }

    /// <summary>
    /// A bitmask indicating this buffer usage.
    /// </summary>
    BufferUsage Usage { get; }

    bool IsDynamic { get; }

    /// <summary>
    /// Updates the contents of a dynamic or updatable GPU buffer.
    /// </summary>
    void UpdateBuffer<T>(IBuffer buffer, ReadOnlySpan<T> data, uint offsetInBytes = 0) where T : unmanaged;
}
