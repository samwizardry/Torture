namespace Torture.Core.Graphics;

public enum GraphicsBackend
{
    Default,
    Direct3D11,
    Vulkan,
    Headless
}

/// <summary>
/// Specifies which buffers to clear during a Clear operation.
/// </summary>
[Flags]
public enum ClearFlags
{
    None = 0,
    Target = 1 << 0,
    Depth = 1 << 1,
    Stencil = 1 << 2,
    DepthStencil = Depth | Stencil,
    All = Target | Depth | Stencil
}

/// <summary>
/// Defines the size of elements in an index buffer.
/// </summary>
public enum IndexFormat
{
    UInt16,
    UInt32
}

/// <summary>
/// Defines how a graphics buffer is bound to the pipeline.
/// </summary>
[Flags]
public enum BufferUsage
{
    None = 0,
    Vertex = 1 << 0,
    Index = 1 << 1,
    Constant = 1 << 2,
    Structured = 1 << 3,
    UnorderedAccess = 1 << 4,
    IndirectArgs = 1 << 5
}
