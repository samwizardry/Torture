using Vortice.Direct3D;
using Vortice.DXGI;

namespace Torture.Graphics.Direct3D11;

public static class Extensions
{
    public static Format ToDXGIFormat(this Core.Graphics.Format format) =>
        (Format)format;

    public static Format ToDXGIFormat(this Core.Graphics.IndexFormat format) =>
        format switch
        {
            Core.Graphics.IndexFormat.UInt32 => Format.R32_UInt,
            Core.Graphics.IndexFormat.UInt16 => Format.R16_UInt,
            _ => throw new NotSupportedException()
        };

    public static Format ToDXGIFormat(this Core.Graphics.VertexElementFormat format) =>
        format switch
        {
            Core.Graphics.VertexElementFormat.Float1 => Format.R32_Float,
            Core.Graphics.VertexElementFormat.Float2 => Format.R32G32_Float,
            Core.Graphics.VertexElementFormat.Float3 => Format.R32G32B32_Float,
            Core.Graphics.VertexElementFormat.Float4 => Format.R32G32B32A32_Float,
            Core.Graphics.VertexElementFormat.Int1 => Format.R32_SInt,
            Core.Graphics.VertexElementFormat.Int2 => Format.R32G32_SInt,
            Core.Graphics.VertexElementFormat.Int3 => Format.R32G32B32_SInt,
            Core.Graphics.VertexElementFormat.Int4 => Format.R32G32B32A32_SInt,
            Core.Graphics.VertexElementFormat.UInt1 => Format.R32_UInt,
            Core.Graphics.VertexElementFormat.UInt2 => Format.R32G32_UInt,
            Core.Graphics.VertexElementFormat.UInt3 => Format.R32G32B32_UInt,
            Core.Graphics.VertexElementFormat.UInt4 => Format.R32G32B32A32_UInt,
            Core.Graphics.VertexElementFormat.Half2 => Format.R16G16_Float,
            Core.Graphics.VertexElementFormat.Half4 => Format.R16G16B16A16_Float,
            Core.Graphics.VertexElementFormat.Short2 => Format.R16G16_SInt,
            Core.Graphics.VertexElementFormat.Short4 => Format.R16G16B16A16_SInt,
            Core.Graphics.VertexElementFormat.UShort2 => Format.R16G16_UInt,
            Core.Graphics.VertexElementFormat.UShort4 => Format.R16G16B16A16_UInt,
            Core.Graphics.VertexElementFormat.NormShort2 => Format.R16G16_SNorm,
            Core.Graphics.VertexElementFormat.NormShort4 => Format.R16G16B16A16_SNorm,
            Core.Graphics.VertexElementFormat.UNormShort2 => Format.R16G16_UNorm,
            Core.Graphics.VertexElementFormat.UNormShort4 => Format.R16G16B16A16_UNorm,
            Core.Graphics.VertexElementFormat.Byte2 => Format.R8G8_SInt,
            Core.Graphics.VertexElementFormat.Byte4 => Format.R8G8B8A8_SInt,
            Core.Graphics.VertexElementFormat.UByte2 => Format.R8G8_UInt,
            Core.Graphics.VertexElementFormat.UByte4 => Format.R8G8B8A8_UInt,
            Core.Graphics.VertexElementFormat.NormByte2 => Format.R8G8_SNorm,
            Core.Graphics.VertexElementFormat.NormByte4 => Format.R8G8B8A8_SNorm,
            Core.Graphics.VertexElementFormat.UNormByte2 => Format.R8G8_UNorm,
            Core.Graphics.VertexElementFormat.UNormByte4 => Format.R8G8B8A8_UNorm,
            _ => throw new NotSupportedException()
        };

    public static string ToD3D11SemanticName(this Core.Graphics.VertexElementSemantic semantic) =>
        semantic switch
        {
            Core.Graphics.VertexElementSemantic.Position => "POSITION",
            Core.Graphics.VertexElementSemantic.Color => "COLOR",
            Core.Graphics.VertexElementSemantic.TexCoord => "TEXCOORD",
            Core.Graphics.VertexElementSemantic.Normal => "NORMAL",
            Core.Graphics.VertexElementSemantic.Tangent => "TANGENT",
            Core.Graphics.VertexElementSemantic.BlendIndices => "BLENDINDICES",
            Core.Graphics.VertexElementSemantic.BlendWeight => "BLENDWEIGHT",
            _ => throw new NotSupportedException()
        };

    public static PrimitiveTopology ToD3D11PrimitiveTopology(this Core.Graphics.PrimitiveTopology topology) =>
        topology switch
        {
            Core.Graphics.PrimitiveTopology.Undefined => PrimitiveTopology.Undefined,
            Core.Graphics.PrimitiveTopology.PointList => PrimitiveTopology.PointList,
            Core.Graphics.PrimitiveTopology.LineList => PrimitiveTopology.LineList,
            Core.Graphics.PrimitiveTopology.LineStrip => PrimitiveTopology.LineStrip,
            Core.Graphics.PrimitiveTopology.TriangleList => PrimitiveTopology.TriangleList,
            Core.Graphics.PrimitiveTopology.TriangleStrip => PrimitiveTopology.TriangleStrip,
            Core.Graphics.PrimitiveTopology.TriangleFan => PrimitiveTopology.TriangleFan,
            _ => throw new NotSupportedException()
        };
}
