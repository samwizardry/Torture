using System.Numerics;
using System.Runtime.InteropServices;

namespace Torture.Core.Graphics;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public readonly record struct VertexPositionNormal(Vector3 Position, Vector3 Normal)
{
    public static readonly VertexLayout Layout;

    static VertexPositionNormal()
    {
        Layout = new VertexLayout([
            new VertexElement(VertexElementSemantic.Position, VertexElementFormat.Float3),
            new VertexElement(VertexElementSemantic.Normal, VertexElementFormat.Float3)
        ]);
    }
}
