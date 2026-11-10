namespace Torture.Core.Graphics;

/// <summary>
/// Represents a hardware buffer residing in GPU memory.
/// </summary>
public interface IBuffer : IDisposable
{
    uint SizeInBytes { get; }

    BufferUsage Usage { get; }

    bool IsDynamic { get; }

    /// <summary>
    /// Creates a GPU buffer and initializes it with the provided data.
    /// </summary>
    IBuffer CreateBuffer<T>(ReadOnlySpan<T> data, BufferUsage usage, bool isDynamic = false) where T : unmanaged;

    /// <summary>
    /// Creates an uninitialized GPU buffer of the specified byte size.
    /// </summary>
    IBuffer CreateBuffer(uint sizeInBytes, BufferUsage usage, bool isDynamic = false);

    /// <summary>
    /// Updates the contents of a dynamic or updatable GPU buffer.
    /// </summary>
    void UpdateBuffer<T>(IBuffer buffer, ReadOnlySpan<T> data, uint offsetInBytes = 0) where T : unmanaged;
}
