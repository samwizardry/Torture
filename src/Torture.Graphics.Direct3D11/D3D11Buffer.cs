using Vortice.Direct3D11;

namespace Torture.Graphics.Direct3D11;

/// <inheritdoc />
public sealed class D3D11Buffer : Core.Graphics.Buffer
{
    private D3D11Buffer(ID3D11Buffer buffer)
    {
        Native = buffer;
    }

    public static unsafe D3D11Buffer Create<T>(ID3D11Device5 device, ReadOnlySpan<T> data, in Core.Graphics.BufferDescription description)
        where T : unmanaged
    {
        BufferDescription nativeDescription = new()
        {
            ByteWidth = (uint)(sizeof(T) * data.Length),
            Usage = description.Usage.ToD3D11ResourceUsage(),
            BindFlags = description.BindFlags.ToD3D11BindFlags(),
            CPUAccessFlags = description.CpuAccessFlags.ToD3D11CpuAccessFlags(),
            MiscFlags = description.MiscFlags.ToD3D11ResourceOptionFlags(),
            StructureByteStride = description.StructureByteStride
        };

        ID3D11Buffer buffer = default!;

        fixed (T* dataPtr = data)
        {
            SubresourceData initialData = new((nint)dataPtr);
            buffer = device.CreateBuffer(nativeDescription, initialData);
        }

        return new D3D11Buffer(buffer);
    }

    public static D3D11Buffer Create(ID3D11Device5 device, in Core.Graphics.BufferDescription description)
    {
        BufferDescription nativeDescription = new()
        {
            ByteWidth = description.ByteWidth,
            Usage = description.Usage.ToD3D11ResourceUsage(),
            BindFlags = description.BindFlags.ToD3D11BindFlags(),
            CPUAccessFlags = description.CpuAccessFlags.ToD3D11CpuAccessFlags(),
            MiscFlags = description.MiscFlags.ToD3D11ResourceOptionFlags(),
            StructureByteStride = description.StructureByteStride
        };

        ID3D11Buffer buffer = device.CreateBuffer(nativeDescription, initialData: null);
        return new D3D11Buffer(buffer);
    }

    public ID3D11Buffer Native { get; private set; }

    /// <inheritdoc />
    //public override uint Size { get; protected set; }

    /// <inheritdoc />
    //public override BufferUsage Usage => throw new NotImplementedException();

    /// <inheritdoc />
    //public override bool IsDynamic => throw new NotImplementedException();

    /// <inheritdoc />
    public override void UpdateBuffer<T>(ReadOnlySpan<T> data, uint offsetInBytes = 0)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        if (_disposed)
            return;

        Native.Dispose();

        _disposed = true;
    }
}
