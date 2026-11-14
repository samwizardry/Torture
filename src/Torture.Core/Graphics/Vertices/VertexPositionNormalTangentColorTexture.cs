using System.Numerics;
using System.Runtime.InteropServices;

using Vortice.Mathematics;
using Vortice.Mathematics.PackedVector;

namespace Torture.Core.Graphics;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public record struct VertexPositionNormalTangentColorTexture(
    Vector3 position, Vector3 normal, Vector4 tangent, UByte4Normalized color, Vector2 texCoord)
{
    public static readonly VertexLayout Layout;

    public Vector3 Position = position;
    public Vector3 Normal = normal;
    public Vector4 Tangent = tangent;
    public UByte4Normalized Color = color;
    public Vector2 TexCoord = texCoord;

    static VertexPositionNormalTangentColorTexture()
    {
        Layout = new VertexLayout([
            new VertexElement(VertexElementSemantic.Position, VertexElementFormat.Float3),
            new VertexElement(VertexElementSemantic.Normal, VertexElementFormat.Float3),
            new VertexElement(VertexElementSemantic.Tangent, VertexElementFormat.Float4),
            new VertexElement(VertexElementSemantic.Color, VertexElementFormat.UNormByte4),
            new VertexElement(VertexElementSemantic.TexCoord, VertexElementFormat.Float2)
        ]);
    }
}
