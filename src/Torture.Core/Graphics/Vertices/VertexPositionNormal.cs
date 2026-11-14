using System.Numerics;
using System.Runtime.InteropServices;

namespace Torture.Core.Graphics;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public record struct VertexPositionNormal(Vector3 position, Vector3 normal)
{
    public static readonly VertexLayout Layout;

    public Vector3 Position = position;
    public Vector3 Normal = normal;

    static VertexPositionNormal()
    {
        Layout = new VertexLayout([
            new VertexElement(VertexElementSemantic.Position, VertexElementFormat.Float3),
            new VertexElement(VertexElementSemantic.Normal, VertexElementFormat.Float3)
        ]);
    }
}
