using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;

namespace Torture.Graphics.Direct3D11;

public static class Extensions
{
    public static ResourceUsage ToD3D11ResourceUsage(this Core.Graphics.ResourceUsage usage) =>
        usage switch
        {
            Core.Graphics.ResourceUsage.Default => ResourceUsage.Default,
            Core.Graphics.ResourceUsage.Immutable => ResourceUsage.Immutable,
            Core.Graphics.ResourceUsage.Dynamic => ResourceUsage.Dynamic,
            Core.Graphics.ResourceUsage.Staging => ResourceUsage.Staging,
            _ => throw new NotSupportedException()
        };

    public static BindFlags ToD3D11BindFlags(this Core.Graphics.BindFlags flags)
    {
        var result = BindFlags.None;

        if ((flags & Core.Graphics.BindFlags.VertexBuffer) == Core.Graphics.BindFlags.VertexBuffer)
        {
            result |= BindFlags.VertexBuffer;
        }
        if ((flags & Core.Graphics.BindFlags.IndexBuffer) == Core.Graphics.BindFlags.IndexBuffer)
        {
            result |= BindFlags.IndexBuffer;
        }
        if ((flags & Core.Graphics.BindFlags.ConstantBuffer) == Core.Graphics.BindFlags.ConstantBuffer)
        {
            result |= BindFlags.ConstantBuffer;
        }
        if ((flags & Core.Graphics.BindFlags.ShaderResource) == Core.Graphics.BindFlags.ShaderResource)
        {
            result |= BindFlags.ShaderResource;
        }
        if ((flags & Core.Graphics.BindFlags.StreamOutput) == Core.Graphics.BindFlags.StreamOutput)
        {
            result |= BindFlags.StreamOutput;
        }
        if ((flags & Core.Graphics.BindFlags.RenderTarget) == Core.Graphics.BindFlags.RenderTarget)
        {
            result |= BindFlags.RenderTarget;
        }
        if ((flags & Core.Graphics.BindFlags.DepthStencil) == Core.Graphics.BindFlags.DepthStencil)
        {
            result |= BindFlags.DepthStencil;
        }
        if ((flags & Core.Graphics.BindFlags.UnorderedAccess) == Core.Graphics.BindFlags.UnorderedAccess)
        {
            result |= BindFlags.UnorderedAccess;
        }
        if ((flags & Core.Graphics.BindFlags.Decoder) == Core.Graphics.BindFlags.Decoder)
        {
            result |= BindFlags.Decoder;
        }
        if ((flags & Core.Graphics.BindFlags.VideoEncoder) == Core.Graphics.BindFlags.VideoEncoder)
        {
            result |= BindFlags.VideoEncoder;
        }

        return result;
    }

    public static CpuAccessFlags ToD3D11CpuAccessFlags(this Core.Graphics.CpuAccessFlags flags)
    {
        CpuAccessFlags result = CpuAccessFlags.None;

        if ((flags & Core.Graphics.CpuAccessFlags.Write) == Core.Graphics.CpuAccessFlags.Write)
        {
            result |= CpuAccessFlags.Write;
        }
        if ((flags & Core.Graphics.CpuAccessFlags.Read) == Core.Graphics.CpuAccessFlags.Read)
        {
            result |= CpuAccessFlags.Read;
        }

        return result;
    }

    public static ResourceOptionFlags ToD3D11ResourceOptionFlags(this Core.Graphics.ResourceOptionFlags flags)
    {
        ResourceOptionFlags result = ResourceOptionFlags.None;

        if ((flags & Core.Graphics.ResourceOptionFlags.GenerateMips) == Core.Graphics.ResourceOptionFlags.GenerateMips)
        {
            result |= ResourceOptionFlags.GenerateMips;
        }
        if ((flags & Core.Graphics.ResourceOptionFlags.Shared) == Core.Graphics.ResourceOptionFlags.Shared)
        {
            result |= ResourceOptionFlags.Shared;
        }
        if ((flags & Core.Graphics.ResourceOptionFlags.TextureCube) == Core.Graphics.ResourceOptionFlags.TextureCube)
        {
            result |= ResourceOptionFlags.TextureCube;
        }
        if ((flags & Core.Graphics.ResourceOptionFlags.DrawIndirectArguments) == Core.Graphics.ResourceOptionFlags.DrawIndirectArguments)
        {
            result |= ResourceOptionFlags.DrawIndirectArguments;
        }
        if ((flags & Core.Graphics.ResourceOptionFlags.BufferAllowRawViews) == Core.Graphics.ResourceOptionFlags.BufferAllowRawViews)
        {
            result |= ResourceOptionFlags.BufferAllowRawViews;
        }
        if ((flags & Core.Graphics.ResourceOptionFlags.BufferStructured) == Core.Graphics.ResourceOptionFlags.BufferStructured)
        {
            result |= ResourceOptionFlags.BufferStructured;
        }
        if ((flags & Core.Graphics.ResourceOptionFlags.ResourceClamp) == Core.Graphics.ResourceOptionFlags.ResourceClamp)
        {
            result |= ResourceOptionFlags.ResourceClamp;
        }
        if ((flags & Core.Graphics.ResourceOptionFlags.SharedKeyedMutex) == Core.Graphics.ResourceOptionFlags.SharedKeyedMutex)
        {
            result |= ResourceOptionFlags.SharedKeyedMutex;
        }
        if ((flags & Core.Graphics.ResourceOptionFlags.GdiCompatible) == Core.Graphics.ResourceOptionFlags.GdiCompatible)
        {
            result |= ResourceOptionFlags.GdiCompatible;
        }
        if ((flags & Core.Graphics.ResourceOptionFlags.SharedNTHandle) == Core.Graphics.ResourceOptionFlags.SharedNTHandle)
        {
            result |= ResourceOptionFlags.SharedNTHandle;
        }
        if ((flags & Core.Graphics.ResourceOptionFlags.RestrictedContent) == Core.Graphics.ResourceOptionFlags.RestrictedContent)
        {
            result |= ResourceOptionFlags.RestrictedContent;
        }
        if ((flags & Core.Graphics.ResourceOptionFlags.RestrictSharedResource) == Core.Graphics.ResourceOptionFlags.RestrictSharedResource)
        {
            result |= ResourceOptionFlags.RestrictSharedResource;
        }
        if ((flags & Core.Graphics.ResourceOptionFlags.RestrictSharedResourceDriver) == Core.Graphics.ResourceOptionFlags.RestrictSharedResourceDriver)
        {
            result |= ResourceOptionFlags.RestrictSharedResourceDriver;
        }
        if ((flags & Core.Graphics.ResourceOptionFlags.Guarded) == Core.Graphics.ResourceOptionFlags.Guarded)
        {
            result |= ResourceOptionFlags.Guarded;
        }
        if ((flags & Core.Graphics.ResourceOptionFlags.TilePool) == Core.Graphics.ResourceOptionFlags.TilePool)
        {
            result |= ResourceOptionFlags.TilePool;
        }
        if ((flags & Core.Graphics.ResourceOptionFlags.Tiled) == Core.Graphics.ResourceOptionFlags.Tiled)
        {
            result |= ResourceOptionFlags.Tiled;
        }
        if ((flags & Core.Graphics.ResourceOptionFlags.HardwareProtected) == Core.Graphics.ResourceOptionFlags.HardwareProtected)
        {
            result |= ResourceOptionFlags.HardwareProtected;
        }
        if ((flags & Core.Graphics.ResourceOptionFlags.SharedDisplayable) == Core.Graphics.ResourceOptionFlags.SharedDisplayable)
        {
            result |= ResourceOptionFlags.SharedDisplayable;
        }
        if ((flags & Core.Graphics.ResourceOptionFlags.SharedExclusiveWriter) == Core.Graphics.ResourceOptionFlags.SharedExclusiveWriter)
        {
            result |= ResourceOptionFlags.SharedExclusiveWriter;
        }
        if ((flags & Core.Graphics.ResourceOptionFlags.NoShaderAccess) == Core.Graphics.ResourceOptionFlags.NoShaderAccess)
        {
            result |= ResourceOptionFlags.NoShaderAccess;
        }

        return result;
    }

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

    public static PrimitiveTopology ToD3D11PrimitiveTopology(this Core.Graphics.PrimitiveTopology primitiveTopology) =>
        primitiveTopology switch
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
