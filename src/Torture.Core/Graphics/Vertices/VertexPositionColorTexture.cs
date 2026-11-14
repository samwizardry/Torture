using System.Numerics;
using System.Runtime.InteropServices;

using Vortice.Mathematics;

namespace Torture.Core.Graphics;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public record struct VertexPositionColorTexture(Vector3 position, Color4 color, Vector2 texCoord)
{
    public static readonly VertexLayout Layout;

    public Vector3 Position = position;
    public Color4 Color = color;
    public Vector2 TexCoord = texCoord;

    static VertexPositionColorTexture()
    {
        Layout = new VertexLayout([
            new VertexElement(VertexElementSemantic.Position, VertexElementFormat.Float3),
            new VertexElement(VertexElementSemantic.Color, VertexElementFormat.Float4),
            new VertexElement(VertexElementSemantic.TexCoord, VertexElementFormat.Float2)
        ]);
    }
}
