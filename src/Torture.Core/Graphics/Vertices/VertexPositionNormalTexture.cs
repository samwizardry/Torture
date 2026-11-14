using System.Numerics;
using System.Runtime.InteropServices;

namespace Torture.Core.Graphics;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public record struct VertexPositionNormalTexture(Vector3 position, Vector3 normal, Vector2 texCoord)
{
    public static readonly VertexLayout Layout;

    public Vector3 Position = position;
    public Vector3 Normal = normal;
    public Vector2 TexCoord = texCoord;

    static VertexPositionNormalTexture()
    {
        Layout = new VertexLayout([
            new VertexElement(VertexElementSemantic.Position, VertexElementFormat.Float3),
            new VertexElement(VertexElementSemantic.Normal, VertexElementFormat.Float3),
            new VertexElement(VertexElementSemantic.TexCoord, VertexElementFormat.Float2)
        ]);
    }
}
