using System.Numerics;
using System.Runtime.InteropServices;

namespace Torture.Core.Graphics;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public record struct VertexPosition(Vector3 position)
{
    public static readonly VertexLayout Layout;

    public Vector3 Position = position;

    static VertexPosition()
    {
        Layout = new VertexLayout([
            new VertexElement(VertexElementSemantic.Position, VertexElementFormat.Float3)
        ]);
    }
}
