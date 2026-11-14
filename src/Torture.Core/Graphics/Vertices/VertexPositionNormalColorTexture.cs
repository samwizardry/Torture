using System.Numerics;
using System.Runtime.InteropServices;

using Vortice.Mathematics;

namespace Torture.Core.Graphics;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public record struct VertexPositionNormalColorTexture(Vector3 position, Vector3 normal, Color4 color, Vector2 texCoord)
{
    public static readonly VertexLayout Layout;

    public Vector3 Position = position;
    public Vector3 Normal = normal;
    public Color4 Color = color;
    public Vector2 TexCoord = texCoord;

    static VertexPositionNormalColorTexture()
    {
        Layout = new VertexLayout([
            new VertexElement(VertexElementSemantic.Position, VertexElementFormat.Float3),
            new VertexElement(VertexElementSemantic.Normal, VertexElementFormat.Float3),
            new VertexElement(VertexElementSemantic.Color, VertexElementFormat.Float4),
            new VertexElement(VertexElementSemantic.TexCoord, VertexElementFormat.Float2)
        ]);
    }
}
