namespace Torture.Core.Graphics;

public readonly record struct BufferDescription
{
    public readonly uint ByteWidth;
    public readonly ResourceUsage Usage = ResourceUsage.Default;
    public readonly BindFlags BindFlags;
    public readonly CpuAccessFlags CpuAccessFlags = CpuAccessFlags.None;
    public readonly ResourceOptionFlags MiscFlags = ResourceOptionFlags.None;
    public readonly uint StructureByteStride = 0;

    public BufferDescription(BindFlags bindFlags,
        bool isDynamic,
        uint byteWidth = 0,
        uint structureByteStride = 0)
        : this(byteWidth,
              usage: isDynamic ? ResourceUsage.Dynamic : ResourceUsage.Default,
              bindFlags: bindFlags,
              cpuAccessFlags: isDynamic ? CpuAccessFlags.Write : CpuAccessFlags.None,
              miscFlags: ResourceOptionFlags.None,
              structureByteStride: structureByteStride)
    { }

    public BufferDescription(uint byteWidth,
        ResourceUsage usage,
        BindFlags bindFlags,
        CpuAccessFlags cpuAccessFlags = CpuAccessFlags.None,
        ResourceOptionFlags miscFlags = ResourceOptionFlags.None,
        uint structureByteStride = 0)
    {
        ByteWidth = byteWidth;
        Usage = usage;
        BindFlags = bindFlags;
        CpuAccessFlags = cpuAccessFlags;
        MiscFlags = miscFlags;
        StructureByteStride = structureByteStride;
    }
}
