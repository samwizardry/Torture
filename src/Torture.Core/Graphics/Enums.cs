namespace Torture.Core.Graphics;

public enum GraphicsAPI : byte
{
    Default,
    D3D11,
    D3D12,
    Vulkan,
    Headless
}

[Flags]
public enum BufferUsage : ushort
{
    None = 0,
    VertexBuffer = 1 << 0,
    IndexBuffer = 1 << 1,
    ConstantBuffer = 1 << 2,
    StructuredBuffer = 1 << 3,
    IndirectArguments = 1 << 4,
    ShaderResource = 1 << 5,
    UnorderedAccess = 1 << 6,
    RawBuffer = 1 << 7
}

public enum CpuAccess : byte
{
    /// <summary>
    /// Only for GPU (best performance).
    /// </summary>
    None = 0,
    /// <summary>
    /// CPU can write to VRAM (Dynamic / Upload).
    /// </summary>
    Write,
    /// <summary>
    /// CPU can read from VRAM (Staging / Readback).
    /// </summary>
    Read
}

public enum Format : uint
{
    Unknown = 0u,
    R32G32B32A32_Typeless = 1u,
    R32G32B32A32_Float = 2u,
    R32G32B32A32_UInt = 3u,
    R32G32B32A32_SInt = 4u,
    R32G32B32_Typeless = 5u,
    R32G32B32_Float = 6u,
    R32G32B32_UInt = 7u,
    R32G32B32_SInt = 8u,
    R16G16B16A16_Typeless = 9u,
    R16G16B16A16_Float = 10u,
    R16G16B16A16_UNorm = 11u,
    R16G16B16A16_UInt = 12u,
    R16G16B16A16_SNorm = 13u,
    R16G16B16A16_SInt = 14u,
    R32G32_Typeless = 15u,
    R32G32_Float = 16u,
    R32G32_UInt = 17u,
    R32G32_SInt = 18u,
    R32G8X24_Typeless = 19u,
    D32_Float_S8X24_UInt = 20u,
    R32_Float_X8X24_Typeless = 21u,
    X32_Typeless_G8X24_UInt = 22u,
    R10G10B10A2_Typeless = 23u,
    R10G10B10A2_UNorm = 24u,
    R10G10B10A2_UInt = 25u,
    R11G11B10_Float = 26u,
    R8G8B8A8_Typeless = 27u,
    R8G8B8A8_UNorm = 28u,
    R8G8B8A8_UNorm_SRgb = 29u,
    R8G8B8A8_UInt = 30u,
    R8G8B8A8_SNorm = 31u,
    R8G8B8A8_SInt = 32u,
    R16G16_Typeless = 33u,
    R16G16_Float = 34u,
    R16G16_UNorm = 35u,
    R16G16_UInt = 36u,
    R16G16_SNorm = 37u,
    R16G16_SInt = 38u,
    R32_Typeless = 39u,
    D32_Float = 40u,
    R32_Float = 41u,
    R32_UInt = 42u,
    R32_SInt = 43u,
    R24G8_Typeless = 44u,
    D24_UNorm_S8_UInt = 45u,
    R24_UNorm_X8_Typeless = 46u,
    X24_Typeless_G8_UInt = 47u,
    R8G8_Typeless = 48u,
    R8G8_UNorm = 49u,
    R8G8_UInt = 50u,
    R8G8_SNorm = 51u,
    R8G8_SInt = 52u,
    R16_Typeless = 53u,
    R16_Float = 54u,
    D16_UNorm = 55u,
    R16_UNorm = 56u,
    R16_UInt = 57u,
    R16_SNorm = 58u,
    R16_SInt = 59u,
    R8_Typeless = 60u,
    R8_UNorm = 61u,
    R8_UInt = 62u,
    R8_SNorm = 63u,
    R8_SInt = 64u,
    A8_UNorm = 65u,
    R1_UNorm = 66u,
    R9G9B9E5_SharedExp = 67u,
    R8G8_B8G8_UNorm = 68u,
    G8R8_G8B8_UNorm = 69u,
    BC1_Typeless = 70u,
    BC1_UNorm = 71u,
    BC1_UNorm_SRgb = 72u,
    BC2_Typeless = 73u,
    BC2_UNorm = 74u,
    BC2_UNorm_SRgb = 75u,
    BC3_Typeless = 76u,
    BC3_UNorm = 77u,
    BC3_UNorm_SRgb = 78u,
    BC4_Typeless = 79u,
    BC4_UNorm = 80u,
    BC4_SNorm = 81u,
    BC5_Typeless = 82u,
    BC5_UNorm = 83u,
    BC5_SNorm = 84u,
    B5G6R5_UNorm = 85u,
    B5G5R5A1_UNorm = 86u,
    B8G8R8A8_UNorm = 87u,
    B8G8R8X8_UNorm = 88u,
    R10G10B10_Xr_Bias_A2_UNorm = 89u,
    B8G8R8A8_Typeless = 90u,
    B8G8R8A8_UNorm_SRgb = 91u,
    B8G8R8X8_Typeless = 92u,
    B8G8R8X8_UNorm_SRgb = 93u,
    BC6H_Typeless = 94u,
    BC6H_Uf16 = 95u,
    BC6H_Sf16 = 96u,
    BC7_Typeless = 97u,
    BC7_UNorm = 98u,
    BC7_UNorm_SRgb = 99u,
    AYUV = 100u,
    Y410 = 101u,
    Y416 = 102u,
    NV12 = 103u,
    P010 = 104u,
    P016 = 105u,
    Opaque420 = 106u,
    YUY2 = 107u,
    Y210 = 108u,
    Y216 = 109u,
    NV11 = 110u,
    AI44 = 111u,
    IA44 = 112u,
    P8 = 113u,
    A8P8 = 114u,
    B4G4R4A4_UNorm = 115u,
    P208 = 130u,
    V208 = 131u,
    V408 = 132u,
    Sampler_Feedback_Min_Mip_Opaque = 189u,
    Sampler_Feedback_Mip_Region_Used_Opaque = 190u,
    A4B4G4R4_UNorm = 191u
}

public enum IndexFormat : byte
{
    UInt32,
    UInt16
}

[Flags]
public enum ClearFlags : byte
{
    None = 0,
    Target = 1,
    Depth = 2,
    Stencil = 4,
    DepthStencil = Depth | Stencil,
    All = Target | Depth | Stencil
}

public enum ShaderType : ushort
{
    None = 0x0000,
    Vertex = 0x0001,
    Hull = 0x0002,
    Domain = 0x0004,
    Geometry = 0x0008,
    Pixel = 0x0010,
    Compute = 0x0020,
    Amplification = 0x0040,
    Mesh = 0x0080,
    AllGraphics = 0x00DF,
    RayGeneration = 0x0100,
    AnyHit = 0x0200,
    ClosestHit = 0x0400,
    Miss = 0x0800,
    Intersection = 0x1000,
    Callable = 0x2000,
    AllRayTracing = 0x3F00,
    All = 0x3FFF,
};

public enum PrimitiveTopology : byte
{
    Undefined = 0,
    PointList = 1,
    LineList = 2,
    LineStrip = 3,
    TriangleList = 4,
    TriangleStrip = 5,
    TriangleFan = 6
}