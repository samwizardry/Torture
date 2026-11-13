using Vortice.Direct3D11;

using CoreGraphics = Torture.Core.Graphics;

namespace Torture.Graphics.Direct3D11;

public sealed class D3D11Buffer : CoreGraphics.Buffer
{
    private D3D11Buffer(ID3D11Buffer buffer)
    {
        Native = buffer;
    }

    public ID3D11Buffer Native { get; private set; }

    public static unsafe D3D11Buffer Create<T>(ID3D11Device5 device, ReadOnlySpan<T> data, in CoreGraphics.BufferDescription desc)
        where T : unmanaged
    {
        BufferDescription d3dDesc = ToD3DBufferDescription(desc, true);

        uint vertexByteSize = (uint)sizeof(T);

        uint byteSize = d3dDesc.ByteWidth > 0 ? d3dDesc.ByteWidth : vertexByteSize * (uint)data.Length;
        uint byteStride = d3dDesc.StructureByteStride > 0 ? d3dDesc.StructureByteStride : vertexByteSize;

        bool isStructured = d3dDesc.MiscFlags.HasFlag(ResourceOptionFlags.BufferStructured);
        uint structureByteStride = isStructured ? byteStride : 0;

        d3dDesc.ByteWidth = byteSize;
        d3dDesc.StructureByteStride = structureByteStride;

        ID3D11Buffer buffer = default!;

        fixed (T* dataPtr = data)
        {
            SubresourceData initialData = new((nint)dataPtr);
            buffer = device.CreateBuffer(d3dDesc, initialData);
        }

        return new D3D11Buffer(buffer)
        {
            ByteSize = byteSize,
            ByteStride = byteStride
        };
    }

    public static D3D11Buffer Create(ID3D11Device5 device, in CoreGraphics.BufferDescription desc)
    {
        BufferDescription d3dDesc = ToD3DBufferDescription(desc, false);

        bool isStructured = d3dDesc.MiscFlags.HasFlag(ResourceOptionFlags.BufferStructured);
        uint structureByteStride = isStructured ? desc.ByteStride : 0;

        d3dDesc.StructureByteStride = structureByteStride;

        ID3D11Buffer buffer = device.CreateBuffer(d3dDesc, initialData: null);

        return new D3D11Buffer(buffer)
        {
            ByteSize = d3dDesc.ByteWidth,
            ByteStride = desc.ByteStride
        };
    }

    private static BufferDescription ToD3DBufferDescription(in CoreGraphics.BufferDescription desc, bool hasInitialData)
    {
        ResourceUsage resourceUsage;
        CpuAccessFlags cpuAccessFlags = CpuAccessFlags.None;

        switch (desc.CpuAccess)
        {
            case CoreGraphics.CpuAccess.None:
                // If we have an initial data, we can set Immutable for best performance
                resourceUsage = hasInitialData ? ResourceUsage.Immutable : ResourceUsage.Default;
                break;
            case CoreGraphics.CpuAccess.Write:
                resourceUsage = ResourceUsage.Dynamic;
                cpuAccessFlags = CpuAccessFlags.Write;
                break;
            case CoreGraphics.CpuAccess.Read:
                resourceUsage = ResourceUsage.Staging;
                cpuAccessFlags = CpuAccessFlags.Read;
                break;
            default:
                throw new NotSupportedException();
        }

        // If cpu access is read, then d3d resource usage is staging, for staging buffers BindFlags should be None
        BindFlags bindFlags = BindFlags.None;
        if (desc.CpuAccess != CoreGraphics.CpuAccess.Read)
        {
            if (desc.Usage.HasFlag(CoreGraphics.BufferUsage.VertexBuffer)) bindFlags |= BindFlags.VertexBuffer;
            if (desc.Usage.HasFlag(CoreGraphics.BufferUsage.IndexBuffer)) bindFlags |= BindFlags.IndexBuffer;
            if (desc.Usage.HasFlag(CoreGraphics.BufferUsage.ConstantBuffer)) bindFlags |= BindFlags.ConstantBuffer;
            if (desc.Usage.HasFlag(CoreGraphics.BufferUsage.ShaderResource) ||
                desc.Usage.HasFlag(CoreGraphics.BufferUsage.StructuredBuffer)) bindFlags |= BindFlags.ShaderResource;
            if (desc.Usage.HasFlag(CoreGraphics.BufferUsage.UnorderedAccess)) bindFlags |= BindFlags.UnorderedAccess;
        }

        ResourceOptionFlags miscFlags = ResourceOptionFlags.None;
        if (desc.Usage.HasFlag(CoreGraphics.BufferUsage.IndirectArguments))
        {
            miscFlags |= ResourceOptionFlags.DrawIndirectArguments;
        }

        bool isStructured = desc.Usage.HasFlag(CoreGraphics.BufferUsage.StructuredBuffer);
        if (isStructured)
        {
            miscFlags |= ResourceOptionFlags.BufferStructured;
        }

        if (desc.Usage.HasFlag(CoreGraphics.BufferUsage.RawBuffer))
        {
            miscFlags |= ResourceOptionFlags.BufferAllowRawViews;
        }

        return new BufferDescription
        {
            ByteWidth = desc.ByteSize,
            Usage = resourceUsage,
            BindFlags = bindFlags,
            CPUAccessFlags = cpuAccessFlags,
            MiscFlags = miscFlags,
            StructureByteStride = isStructured ? desc.ByteStride : 0
        };
    }

    public override void UpdateBuffer<T>(ReadOnlySpan<T> data, uint byteOffset = 0)
    {
        throw new NotImplementedException();
    }

    public override void Dispose()
    {
        if (_disposed)
            return;

        Native.Dispose();

        _disposed = true;
    }
}
