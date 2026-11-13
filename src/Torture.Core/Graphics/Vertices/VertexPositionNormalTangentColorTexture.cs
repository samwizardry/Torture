using System.Numerics;
using System.Runtime.InteropServices;

using Vortice.Mathematics.PackedVector;

namespace Torture.Core.Graphics;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public record struct VertexPositionNormalTangentColorTexture(
    Vector3 Position, Vector3 Normal, Vector4 Tangent, UByte4Normalized Color, Vector2 TexCoord)
{
    public static readonly VertexLayout Layout;

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
