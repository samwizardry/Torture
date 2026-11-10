namespace Torture.Core.Graphics;

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