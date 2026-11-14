using System.Numerics;
using System.Runtime.InteropServices;

using Vortice.Mathematics.PackedVector;

namespace Torture.Core.Graphics;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public record struct VertexPositionNormalTangentColorTextureSkinning(
    Vector3 position, Vector3 normal, Vector4 tangent, UByte4Normalized color, Vector2 texCoord, UByte4 blendIndices, UByte4Normalized blendWeight)
{
    public static readonly VertexLayout Layout;

    public Vector3 Position = position;
    public Vector3 Normal = normal;
    public Vector4 Tangent = tangent;
    public UByte4Normalized Color = color;
    public Vector2 TexCoord = texCoord;
    public UByte4 BlendIndices = blendIndices;
    public UByte4Normalized BlendWeight = blendWeight;

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
