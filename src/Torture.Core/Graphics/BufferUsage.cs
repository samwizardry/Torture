namespace Torture.Core.Graphics;

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