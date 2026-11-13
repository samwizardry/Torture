using FluentAssertions;

using Torture.Graphics.Direct3D11;

using CoreFormat = Torture.Core.Graphics.Format;
using CoreGraphics = Torture.Core.Graphics;
using D3D = Vortice.Direct3D;
using D3D11 = Vortice.Direct3D11;
using DxgiFormat = Vortice.DXGI.Format;

namespace Torture.Direct3D11.UnitTests;

/// <summary>
/// Unit tests verifying conversion extension methods between Core graphics enums and DirectX/DXGI native types.
/// </summary>
public class D3D11ExtensionsTests
{
    #region Core.Graphics.Format -> DXGI.Format Tests

    [Theory]
    [InlineData(CoreFormat.Unknown, DxgiFormat.Unknown)]
    [InlineData(CoreFormat.R8G8B8A8_UNorm, DxgiFormat.R8G8B8A8_UNorm)]
    [InlineData(CoreFormat.R8G8B8A8_UNorm_SRgb, DxgiFormat.R8G8B8A8_UNorm_SRgb)]
    [InlineData(CoreFormat.B8G8R8A8_UNorm, DxgiFormat.B8G8R8A8_UNorm)]
    [InlineData(CoreFormat.R32G32B32A32_Float, DxgiFormat.R32G32B32A32_Float)]
    [InlineData(CoreFormat.R32G32B32_Float, DxgiFormat.R32G32B32_Float)]
    [InlineData(CoreFormat.R32G32_Float, DxgiFormat.R32G32_Float)]
    [InlineData(CoreFormat.R32_Float, DxgiFormat.R32_Float)]
    [InlineData(CoreFormat.D24_UNorm_S8_UInt, DxgiFormat.D24_UNorm_S8_UInt)]
    [InlineData(CoreFormat.D32_Float, DxgiFormat.D32_Float)]
    [InlineData(CoreFormat.D16_UNorm, DxgiFormat.D16_UNorm)]
    [InlineData(CoreFormat.BC1_UNorm, DxgiFormat.BC1_UNorm)]
    [InlineData(CoreFormat.BC3_UNorm, DxgiFormat.BC3_UNorm)]
    [InlineData(CoreFormat.BC7_UNorm, DxgiFormat.BC7_UNorm)]
    public void ToDXGIFormat_CoreFormat_MapsKeyFormatsCorrectly(CoreFormat coreFormat, DxgiFormat expectedDxgiFormat)
    {
        var actual = coreFormat.ToDXGIFormat();

        actual.Should().Be(expectedDxgiFormat,
            because: $"Core Format {coreFormat} must map to DXGI.Format {expectedDxgiFormat}");
    }

    [Fact]
    public void ToDXGIFormat_CoreFormat_NumericValuesMatchOneToOne()
    {
        // Verifies that integer underlying values of Core.Graphics.Format exactly match Vortice.DXGI.Format
        foreach (CoreFormat coreFormat in Enum.GetValues<CoreFormat>())
        {
            var dxgiFormat = coreFormat.ToDXGIFormat();
            ((uint)dxgiFormat).Should().Be((uint)coreFormat,
                because: $"underlying enum integer value of {coreFormat} ({(uint)coreFormat}) must match DXGI format value ({(uint)dxgiFormat})");
        }
    }

    #endregion

    #region IndexFormat Tests

    [Theory]
    [InlineData(CoreGraphics.IndexFormat.UInt32, DxgiFormat.R32_UInt)]
    [InlineData(CoreGraphics.IndexFormat.UInt16, DxgiFormat.R16_UInt)]
    public void ToDXGIFormat_IndexFormat_MapsCorrectly(CoreGraphics.IndexFormat coreFormat, DxgiFormat expectedDXGIFormat)
    {
        var actual = coreFormat.ToDXGIFormat();

        actual.Should().Be(expectedDXGIFormat,
            because: $"Core IndexFormat {coreFormat} must map to DXGI {expectedDXGIFormat}");
    }

    #endregion

    #region VertexElementFormat Tests

    [Theory]
    // 32-bit Float
    [InlineData(CoreGraphics.VertexElementFormat.Float1, DxgiFormat.R32_Float)]
    [InlineData(CoreGraphics.VertexElementFormat.Float2, DxgiFormat.R32G32_Float)]
    [InlineData(CoreGraphics.VertexElementFormat.Float3, DxgiFormat.R32G32B32_Float)]
    [InlineData(CoreGraphics.VertexElementFormat.Float4, DxgiFormat.R32G32B32A32_Float)]
    // 32-bit Signed Integer
    [InlineData(CoreGraphics.VertexElementFormat.Int1, DxgiFormat.R32_SInt)]
    [InlineData(CoreGraphics.VertexElementFormat.Int2, DxgiFormat.R32G32_SInt)]
    [InlineData(CoreGraphics.VertexElementFormat.Int3, DxgiFormat.R32G32B32_SInt)]
    [InlineData(CoreGraphics.VertexElementFormat.Int4, DxgiFormat.R32G32B32A32_SInt)]
    // 32-bit Unsigned Integer
    [InlineData(CoreGraphics.VertexElementFormat.UInt1, DxgiFormat.R32_UInt)]
    [InlineData(CoreGraphics.VertexElementFormat.UInt2, DxgiFormat.R32G32_UInt)]
    [InlineData(CoreGraphics.VertexElementFormat.UInt3, DxgiFormat.R32G32B32_UInt)]
    [InlineData(CoreGraphics.VertexElementFormat.UInt4, DxgiFormat.R32G32B32A32_UInt)]
    // 16-bit Float (Half)
    [InlineData(CoreGraphics.VertexElementFormat.Half2, DxgiFormat.R16G16_Float)]
    [InlineData(CoreGraphics.VertexElementFormat.Half4, DxgiFormat.R16G16B16A16_Float)]
    // 16-bit Signed Integer
    [InlineData(CoreGraphics.VertexElementFormat.Short2, DxgiFormat.R16G16_SInt)]
    [InlineData(CoreGraphics.VertexElementFormat.Short4, DxgiFormat.R16G16B16A16_SInt)]
    // 16-bit Unsigned Integer
    [InlineData(CoreGraphics.VertexElementFormat.UShort2, DxgiFormat.R16G16_UInt)]
    [InlineData(CoreGraphics.VertexElementFormat.UShort4, DxgiFormat.R16G16B16A16_UInt)]
    // 16-bit Normalized Signed/Unsigned
    [InlineData(CoreGraphics.VertexElementFormat.NormShort2, DxgiFormat.R16G16_SNorm)]
    [InlineData(CoreGraphics.VertexElementFormat.NormShort4, DxgiFormat.R16G16B16A16_SNorm)]
    [InlineData(CoreGraphics.VertexElementFormat.UNormShort2, DxgiFormat.R16G16_UNorm)]
    [InlineData(CoreGraphics.VertexElementFormat.UNormShort4, DxgiFormat.R16G16B16A16_UNorm)]
    // 8-bit Integer
    [InlineData(CoreGraphics.VertexElementFormat.Byte2, DxgiFormat.R8G8_SInt)]
    [InlineData(CoreGraphics.VertexElementFormat.Byte4, DxgiFormat.R8G8B8A8_SInt)]
    [InlineData(CoreGraphics.VertexElementFormat.UByte2, DxgiFormat.R8G8_UInt)]
    [InlineData(CoreGraphics.VertexElementFormat.UByte4, DxgiFormat.R8G8B8A8_UInt)]
    // 8-bit Normalized Signed/Unsigned
    [InlineData(CoreGraphics.VertexElementFormat.NormByte2, DxgiFormat.R8G8_SNorm)]
    [InlineData(CoreGraphics.VertexElementFormat.NormByte4, DxgiFormat.R8G8B8A8_SNorm)]
    [InlineData(CoreGraphics.VertexElementFormat.UNormByte2, DxgiFormat.R8G8_UNorm)]
    [InlineData(CoreGraphics.VertexElementFormat.UNormByte4, DxgiFormat.R8G8B8A8_UNorm)]
    public void ToDXGIFormat_VertexElementFormat_MapsAll30Formats(CoreGraphics.VertexElementFormat format, DxgiFormat expectedDXGIFormat)
    {
        var actual = format.ToDXGIFormat();

        actual.Should().Be(expectedDXGIFormat,
            because: $"VertexElementFormat.{format} must map directly to DXGI.Format.{expectedDXGIFormat}");
    }

    [Fact]
    public void ToDXGIFormat_CoversEveryVertexElementFormat()
    {
        foreach (var format in Enum.GetValues<CoreGraphics.VertexElementFormat>())
        {
            var act = () => format.ToDXGIFormat();
            act.Should().NotThrow(because: $"all defined VertexElementFormat values must have a valid DXGI mapping ({format})");

            var dxgiFormat = format.ToDXGIFormat();
            dxgiFormat.Should().NotBe(DxgiFormat.Unknown,
                because: $"format {format} cannot map to DXGI.Format.Unknown");
        }
    }

    #endregion

    #region VertexElementSemantic Tests

    [Theory]
    [InlineData(CoreGraphics.VertexElementSemantic.Position, "POSITION")]
    [InlineData(CoreGraphics.VertexElementSemantic.Color, "COLOR")]
    [InlineData(CoreGraphics.VertexElementSemantic.TexCoord, "TEXCOORD")]
    [InlineData(CoreGraphics.VertexElementSemantic.Normal, "NORMAL")]
    [InlineData(CoreGraphics.VertexElementSemantic.Tangent, "TANGENT")]
    [InlineData(CoreGraphics.VertexElementSemantic.BlendIndices, "BLENDINDICES")]
    [InlineData(CoreGraphics.VertexElementSemantic.BlendWeight, "BLENDWEIGHT")]
    public void ToD3D11SemanticName_MapsCorrectly(CoreGraphics.VertexElementSemantic semantic, string expectedName)
    {
        var actual = semantic.ToD3D11SemanticName();

        actual.Should().Be(expectedName,
            because: $"semantic {semantic} must match the HLSL standard input semantic '{expectedName}'");
    }

    [Fact]
    public void ToD3D11SemanticName_CountSemantic_ThrowsNotSupportedException()
    {
        var countSemantic = CoreGraphics.VertexElementSemantic.Count;
        var act = () => countSemantic.ToD3D11SemanticName();

        act.Should().Throw<NotSupportedException>(
            because: "the internal sentinel value VertexElementSemantic.Count is not a valid HLSL semantic and must throw");
    }

    #endregion

    #region PrimitiveTopology Tests

    [Theory]
    [InlineData(CoreGraphics.PrimitiveTopology.Undefined, D3D.PrimitiveTopology.Undefined)]
    [InlineData(CoreGraphics.PrimitiveTopology.PointList, D3D.PrimitiveTopology.PointList)]
    [InlineData(CoreGraphics.PrimitiveTopology.LineList, D3D.PrimitiveTopology.LineList)]
    [InlineData(CoreGraphics.PrimitiveTopology.LineStrip, D3D.PrimitiveTopology.LineStrip)]
    [InlineData(CoreGraphics.PrimitiveTopology.TriangleList, D3D.PrimitiveTopology.TriangleList)]
    [InlineData(CoreGraphics.PrimitiveTopology.TriangleStrip, D3D.PrimitiveTopology.TriangleStrip)]
    [InlineData(CoreGraphics.PrimitiveTopology.TriangleFan, D3D.PrimitiveTopology.TriangleFan)]
    public void ToD3D11PrimitiveTopology_MapsCorrectly(CoreGraphics.PrimitiveTopology topology, D3D.PrimitiveTopology expectedD3DTopology)
    {
        var actual = topology.ToD3D11PrimitiveTopology();

        actual.Should().Be(expectedD3DTopology,
            because: $"Core PrimitiveTopology.{topology} must map directly to Direct3D PrimitiveTopology.{expectedD3DTopology}");
    }

    #endregion
}