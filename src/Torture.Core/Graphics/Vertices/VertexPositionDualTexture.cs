using System.Numerics;
using System.Runtime.InteropServices;

namespace Torture.Core.Graphics;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public record struct VertexPositionDualTexture(Vector3 position, Vector2 texCoord0, Vector2 texCoord1)
{
    public static readonly VertexLayout Layout;

    public Vector3 Position = position;
    public Vector2 TexCoord0 = texCoord0;
    public Vector2 TexCoord1 = texCoord1;

    static VertexPositionDualTexture()
    {
        Layout = new VertexLayout([
            new VertexElement(VertexElementSemantic.Position, VertexElementFormat.Float3),
            new VertexElement(VertexElementSemantic.TexCoord, VertexElementFormat.Float2),
            new VertexElement(VertexElementSemantic.TexCoord, VertexElementFormat.Float2)
        ]);
    }
}
