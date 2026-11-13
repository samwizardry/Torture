namespace Torture.Core.Graphics;

public readonly record struct BufferDescription
{
    public readonly uint ByteSize;
    public readonly uint ByteStride;
    public readonly BufferUsage Usage;
    public readonly CpuAccess CpuAccess;

    public BufferDescription(
        uint byteSize,
        BufferUsage usage,
        CpuAccess cpuAccess = CpuAccess.None,
        uint byteStride = 0)
    {
        ByteSize = byteSize;
        Usage = usage;
        CpuAccess = cpuAccess;
        ByteStride = byteStride;
    }

    public BufferDescription(
        BufferUsage usage,
        bool isDynamic = false,
        uint byteSize = 0,
        uint byteStride = 0)
        : this(byteSize, usage, isDynamic ? CpuAccess.Write : CpuAccess.None, byteStride)
    {
    }

    public static BufferDescription Vertex(uint byteSize, uint byteStride, bool isDynamic = false) =>
        new(byteSize, BufferUsage.VertexBuffer, isDynamic ? CpuAccess.Write : CpuAccess.None, byteStride);

    public static BufferDescription Index(uint byteSize, bool isDynamic = false) =>
        new(byteSize, BufferUsage.IndexBuffer, isDynamic ? CpuAccess.Write : CpuAccess.None);

    public static BufferDescription Constant(uint byteSize) =>
        new(byteSize, BufferUsage.ConstantBuffer, CpuAccess.Write);
}
