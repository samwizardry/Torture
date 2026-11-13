using System.Numerics;
using System.Runtime.InteropServices;

namespace Torture.Core.Graphics;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public record struct VertexPositionTexture(Vector3 Position, Vector2 TexCoord)
{
    public static readonly VertexLayout Layout;

    static VertexPositionTexture()
    {
        Layout = new VertexLayout([
            new VertexElement(VertexElementSemantic.Position, VertexElementFormat.Float3),
            new VertexElement(VertexElementSemantic.TexCoord, VertexElementFormat.Float2)
        ]);
    }
}
