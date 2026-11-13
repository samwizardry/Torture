using System.Numerics;
using System.Runtime.InteropServices;

using Vortice.Mathematics.PackedVector;

namespace Torture.Core.Graphics;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public record struct VertexPositionNormalTangentColorTextureSkinning(
    Vector3 Position, Vector3 Normal, Vector4 Tangent, UByte4Normalized Color, Vector2 TexCoord, UByte4 BlendIndices, UByte4Normalized BlendWeight)
{
    public static readonly VertexLayout Layout;

    static VertexPositionNormalTangentColorTextureSkinning()
    {
        Layout = new VertexLayout([
            new VertexElement(VertexElementSemantic.Position, VertexElementFormat.Float3),
            new VertexElement(VertexElementSemantic.Normal, VertexElementFormat.Float3),
            new VertexElement(VertexElementSemantic.Tangent, VertexElementFormat.Float4),
            new VertexElement(VertexElementSemantic.Color, VertexElementFormat.UNormByte4),
            new VertexElement(VertexElementSemantic.TexCoord, VertexElementFormat.Float2),
            new VertexElement(VertexElementSemantic.BlendIndices, VertexElementFormat.UByte4),
            new VertexElement(VertexElementSemantic.BlendWeight, VertexElementFormat.UNormByte4),
        ]);
    }
}
