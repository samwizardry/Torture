using FluentAssertions;

using Torture.Core.Graphics;

namespace Torture.UnitTests;

/// <summary>
/// Unit tests verifying byte width calculations for all GPU vertex element formats.
/// </summary>
public class VertexElementFormatTests
{
    [Theory]
    // 16 bytes (4 x 32-bit components)
    [InlineData(VertexElementFormat.Float4, 16u)]
    [InlineData(VertexElementFormat.Int4, 16u)]
    [InlineData(VertexElementFormat.UInt4, 16u)]
    // 12 bytes (3 x 32-bit components)
    [InlineData(VertexElementFormat.Float3, 12u)]
    [InlineData(VertexElementFormat.Int3, 12u)]
    [InlineData(VertexElementFormat.UInt3, 12u)]
    // 8 bytes (2 x 32-bit or 4 x 16-bit components)
    [InlineData(VertexElementFormat.Float2, 8u)]
    [InlineData(VertexElementFormat.Int2, 8u)]
    [InlineData(VertexElementFormat.UInt2, 8u)]
    [InlineData(VertexElementFormat.Half4, 8u)]
    [InlineData(VertexElementFormat.Short4, 8u)]
    [InlineData(VertexElementFormat.UShort4, 8u)]
    [InlineData(VertexElementFormat.NormShort4, 8u)]
    [InlineData(VertexElementFormat.UNormShort4, 8u)]
    // 4 bytes (1 x 32-bit, 2 x 16-bit, or 4 x 8-bit components)
    [InlineData(VertexElementFormat.Float1, 4u)]
    [InlineData(VertexElementFormat.Int1, 4u)]
    [InlineData(VertexElementFormat.UInt1, 4u)]
    [InlineData(VertexElementFormat.Half2, 4u)]
    [InlineData(VertexElementFormat.Short2, 4u)]
    [InlineData(VertexElementFormat.UShort2, 4u)]
    [InlineData(VertexElementFormat.NormShort2, 4u)]
    [InlineData(VertexElementFormat.UNormShort2, 4u)]
    [InlineData(VertexElementFormat.Byte4, 4u)]
    [InlineData(VertexElementFormat.UByte4, 4u)]
    [InlineData(VertexElementFormat.NormByte4, 4u)]
    [InlineData(VertexElementFormat.UNormByte4, 4u)]
    // 2 bytes (2 x 8-bit components)
    [InlineData(VertexElementFormat.Byte2, 2u)]
    [InlineData(VertexElementFormat.UByte2, 2u)]
    [InlineData(VertexElementFormat.NormByte2, 2u)]
    [InlineData(VertexElementFormat.UNormByte2, 2u)]
    public void ByteWidth_ReturnsExpectedSize(VertexElementFormat format, uint expectedBytes)
    {
        uint actualBytes = format.ByteWidth();

        actualBytes.Should().Be(expectedBytes,
            because: $"format {format} must occupy exactly {expectedBytes} bytes in GPU memory");
    }

    [Fact]
    public void ByteWidth_CoversAllEnumValues_WithoutExceptions()
    {
        var allFormats = Enum.GetValues<VertexElementFormat>();

        allFormats.Should().NotBeEmpty(because: "VertexElementFormat enum must contain defined formats");

        foreach (var format in allFormats)
        {
            var act = () => format.ByteWidth();
            act.Should().NotThrow(because: $"format {format} must be handled by the switch expression");

            uint width = format.ByteWidth();
            width.Should().BeOneOf([2u, 4u, 8u, 12u, 16u],
                because: $"byte width ({width}) for format {format} must be a hardware-valid size for the GPU input assembler");
        }
    }
}