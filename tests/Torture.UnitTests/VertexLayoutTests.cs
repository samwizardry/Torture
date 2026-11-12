using System.Runtime.CompilerServices;

using FluentAssertions;

using Torture.Core.Graphics;

namespace Torture.UnitTests;

/// <summary>
/// Unit tests verifying vertex layout integrity, continuous offset calculation, and semantic indexing.
/// </summary>
public class VertexLayoutTests
{
    #region Smart Layout Verifier

    /// <summary>
    /// Validates continuity of offsets, monotonicity of semantic indices, and cumulative stride.
    /// </summary>
    private static void AssertLayoutIntegrity(VertexLayout layout, uint expectedStride)
    {
        var elements = layout.Elements;

        elements.Length.Should().BeGreaterThan(0, because: "a vertex layout must contain at least one element");

        uint runningOffset = 0;
        Dictionary<VertexElementSemantic, uint> expectedSemanticCounters = [];

        for (int i = 0; i < elements.Length; i++)
        {
            var element = elements[i];
            uint formatSize = element.Format.ByteWidth();

            // 1. Validate contiguous byte offset
            element.Offset.Should().Be(runningOffset,
                because: $"element [{i}] ({element.SemanticName}) must be located at offset {runningOffset} bytes without gaps");

            // 2. Validate monotonic semantic indexing (0, 1, 2...)
            uint expectedIndex = expectedSemanticCounters.GetValueOrDefault(element.SemanticName, 0u);
            element.SemanticIndex.Should().Be(expectedIndex,
                because: $"semantic {element.SemanticName} at element [{i}] must have index {expectedIndex}");
            expectedSemanticCounters[element.SemanticName] = expectedIndex + 1;

            runningOffset += formatSize;
        }

        // 3. Validate overall stride
        layout.Stride.Should().Be(runningOffset,
            because: "stride must match the exact sum of all element byte widths");
        layout.Stride.Should().Be(expectedStride,
            because: $"stride must match the expected layout size ({expectedStride} bytes)");
    }

    #endregion

    #region Standard Layouts Tests

    [Fact]
    public void VertexPosition_Layout_IsCorrect()
    {
        var layout = VertexPosition.Layout;

        AssertLayoutIntegrity(layout, expectedStride: 12);
        layout.Stride.Should().Be((uint)Unsafe.SizeOf<VertexPosition>(),
            because: "layout stride must match physical struct size");

        layout.Elements.Length.Should().Be(1);
        layout.Elements[0].Should().Be(new VertexElement(VertexElementSemantic.Position, VertexElementFormat.Float3) { Offset = 0, SemanticIndex = 0 });
    }

    [Fact]
    public void VertexPositionColor_Layout_IsCorrect()
    {
        var layout = VertexPositionColor.Layout;

        AssertLayoutIntegrity(layout, expectedStride: 28);
        layout.Stride.Should().Be((uint)Unsafe.SizeOf<VertexPositionColor>(),
            because: "layout stride must match physical struct size");

        layout.Elements.Length.Should().Be(2);
        layout.Elements[0].Should().Be(new VertexElement(VertexElementSemantic.Position, VertexElementFormat.Float3) { Offset = 0, SemanticIndex = 0 });
        layout.Elements[1].Should().Be(new VertexElement(VertexElementSemantic.Color, VertexElementFormat.Float4) { Offset = 12, SemanticIndex = 0 });
    }

    [Fact]
    public void VertexPositionTexture_Layout_IsCorrect()
    {
        var layout = VertexPositionTexture.Layout;

        AssertLayoutIntegrity(layout, expectedStride: 20);
        layout.Stride.Should().Be((uint)Unsafe.SizeOf<VertexPositionTexture>(),
            because: "layout stride must match physical struct size");

        layout.Elements.Length.Should().Be(2);
        layout.Elements[0].Should().Be(new VertexElement(VertexElementSemantic.Position, VertexElementFormat.Float3) { Offset = 0, SemanticIndex = 0 });
        layout.Elements[1].Should().Be(new VertexElement(VertexElementSemantic.TexCoord, VertexElementFormat.Float2) { Offset = 12, SemanticIndex = 0 });
    }

    [Fact]
    public void VertexPositionDualTexture_Layout_CalculatesConsecutiveTexCoords()
    {
        var layout = VertexPositionDualTexture.Layout;

        AssertLayoutIntegrity(layout, expectedStride: 28);
        layout.Stride.Should().Be((uint)Unsafe.SizeOf<VertexPositionDualTexture>(),
            because: "layout stride must match physical struct size");

        layout.Elements.Length.Should().Be(3);
        layout.Elements[0].Should().Be(new VertexElement(VertexElementSemantic.Position, VertexElementFormat.Float3) { Offset = 0, SemanticIndex = 0 });
        layout.Elements[1].Should().Be(new VertexElement(VertexElementSemantic.TexCoord, VertexElementFormat.Float2) { Offset = 12, SemanticIndex = 0 });
        layout.Elements[2].Should().Be(new VertexElement(VertexElementSemantic.TexCoord, VertexElementFormat.Float2) { Offset = 20, SemanticIndex = 1 });
    }

    [Fact]
    public void VertexPositionNormal_Layout_IsCorrect()
    {
        var layout = VertexPositionNormal.Layout;

        AssertLayoutIntegrity(layout, expectedStride: 24);
        layout.Stride.Should().Be((uint)Unsafe.SizeOf<VertexPositionNormal>(),
            because: "layout stride must match physical struct size");

        layout.Elements.Length.Should().Be(2);
        layout.Elements[0].Should().Be(new VertexElement(VertexElementSemantic.Position, VertexElementFormat.Float3) { Offset = 0, SemanticIndex = 0 });
        layout.Elements[1].Should().Be(new VertexElement(VertexElementSemantic.Normal, VertexElementFormat.Float3) { Offset = 12, SemanticIndex = 0 });
    }

    [Fact]
    public void VertexPositionColorTexture_Layout_IsCorrect()
    {
        var layout = VertexPositionColorTexture.Layout;

        AssertLayoutIntegrity(layout, expectedStride: 36);
        layout.Stride.Should().Be((uint)Unsafe.SizeOf<VertexPositionColorTexture>(),
            because: "layout stride must match physical struct size");

        layout.Elements.Length.Should().Be(3);
        layout.Elements[0].Should().Be(new VertexElement(VertexElementSemantic.Position, VertexElementFormat.Float3) { Offset = 0, SemanticIndex = 0 });
        layout.Elements[1].Should().Be(new VertexElement(VertexElementSemantic.Color, VertexElementFormat.Float4) { Offset = 12, SemanticIndex = 0 });
        layout.Elements[2].Should().Be(new VertexElement(VertexElementSemantic.TexCoord, VertexElementFormat.Float2) { Offset = 28, SemanticIndex = 0 });
    }

    [Fact]
    public void VertexPositionNormalColor_Layout_IsCorrect()
    {
        var layout = VertexPositionNormalColor.Layout;

        AssertLayoutIntegrity(layout, expectedStride: 40);
        layout.Stride.Should().Be((uint)Unsafe.SizeOf<VertexPositionNormalColor>(),
            because: "layout stride must match physical struct size");

        layout.Elements.Length.Should().Be(3);
        layout.Elements[0].Should().Be(new VertexElement(VertexElementSemantic.Position, VertexElementFormat.Float3) { Offset = 0, SemanticIndex = 0 });
        layout.Elements[1].Should().Be(new VertexElement(VertexElementSemantic.Normal, VertexElementFormat.Float3) { Offset = 12, SemanticIndex = 0 });
        layout.Elements[2].Should().Be(new VertexElement(VertexElementSemantic.Color, VertexElementFormat.Float4) { Offset = 24, SemanticIndex = 0 });
    }

    [Fact]
    public void VertexPositionNormalTexture_Layout_IsCorrect()
    {
        var layout = VertexPositionNormalTexture.Layout;

        AssertLayoutIntegrity(layout, expectedStride: 32);
        layout.Stride.Should().Be((uint)Unsafe.SizeOf<VertexPositionNormalTexture>(),
            because: "layout stride must match physical struct size");

        layout.Elements.Length.Should().Be(3);
        layout.Elements[0].Should().Be(new VertexElement(VertexElementSemantic.Position, VertexElementFormat.Float3) { Offset = 0, SemanticIndex = 0 });
        layout.Elements[1].Should().Be(new VertexElement(VertexElementSemantic.Normal, VertexElementFormat.Float3) { Offset = 12, SemanticIndex = 0 });
        layout.Elements[2].Should().Be(new VertexElement(VertexElementSemantic.TexCoord, VertexElementFormat.Float2) { Offset = 24, SemanticIndex = 0 });
    }

    [Fact]
    public void VertexPositionNormalColorTexture_Layout_IsCorrect()
    {
        var layout = VertexPositionNormalColorTexture.Layout;

        AssertLayoutIntegrity(layout, expectedStride: 48);
        layout.Stride.Should().Be((uint)Unsafe.SizeOf<VertexPositionNormalColorTexture>(),
            because: "layout stride must match physical struct size");

        layout.Elements.Length.Should().Be(4);
        layout.Elements[0].Should().Be(new VertexElement(VertexElementSemantic.Position, VertexElementFormat.Float3) { Offset = 0, SemanticIndex = 0 });
        layout.Elements[1].Should().Be(new VertexElement(VertexElementSemantic.Normal, VertexElementFormat.Float3) { Offset = 12, SemanticIndex = 0 });
        layout.Elements[2].Should().Be(new VertexElement(VertexElementSemantic.Color, VertexElementFormat.Float4) { Offset = 24, SemanticIndex = 0 });
        layout.Elements[3].Should().Be(new VertexElement(VertexElementSemantic.TexCoord, VertexElementFormat.Float2) { Offset = 40, SemanticIndex = 0 });
    }

    [Fact]
    public void VertexPositionNormalTangentColorTexture_Layout_IsCorrect()
    {
        var layout = VertexPositionNormalTangentColorTexture.Layout;

        AssertLayoutIntegrity(layout, expectedStride: 52);
        layout.Stride.Should().Be((uint)Unsafe.SizeOf<VertexPositionNormalTangentColorTexture>(),
            because: "layout stride must match physical struct size");

        layout.Elements.Length.Should().Be(5);
        layout.Elements[0].Should().Be(new VertexElement(VertexElementSemantic.Position, VertexElementFormat.Float3) { Offset = 0, SemanticIndex = 0 });
        layout.Elements[1].Should().Be(new VertexElement(VertexElementSemantic.Normal, VertexElementFormat.Float3) { Offset = 12, SemanticIndex = 0 });
        layout.Elements[2].Should().Be(new VertexElement(VertexElementSemantic.Tangent, VertexElementFormat.Float4) { Offset = 24, SemanticIndex = 0 });
        layout.Elements[3].Should().Be(new VertexElement(VertexElementSemantic.Color, VertexElementFormat.UNormByte4) { Offset = 40, SemanticIndex = 0 });
        layout.Elements[4].Should().Be(new VertexElement(VertexElementSemantic.TexCoord, VertexElementFormat.Float2) { Offset = 44, SemanticIndex = 0 });
    }

    [Fact]
    public void VertexPositionNormalTangentColorTextureSkinning_Layout_IsCorrect()
    {
        var layout = VertexPositionNormalTangentColorTextureSkinning.Layout;

        AssertLayoutIntegrity(layout, expectedStride: 60);
        layout.Stride.Should().Be((uint)Unsafe.SizeOf<VertexPositionNormalTangentColorTextureSkinning>(),
            because: "layout stride must match physical struct size");

        layout.Elements.Length.Should().Be(7);
        layout.Elements[0].Should().Be(new VertexElement(VertexElementSemantic.Position, VertexElementFormat.Float3) { Offset = 0, SemanticIndex = 0 });
        layout.Elements[1].Should().Be(new VertexElement(VertexElementSemantic.Normal, VertexElementFormat.Float3) { Offset = 12, SemanticIndex = 0 });
        layout.Elements[2].Should().Be(new VertexElement(VertexElementSemantic.Tangent, VertexElementFormat.Float4) { Offset = 24, SemanticIndex = 0 });
        layout.Elements[3].Should().Be(new VertexElement(VertexElementSemantic.Color, VertexElementFormat.UNormByte4) { Offset = 40, SemanticIndex = 0 });
        layout.Elements[4].Should().Be(new VertexElement(VertexElementSemantic.TexCoord, VertexElementFormat.Float2) { Offset = 44, SemanticIndex = 0 });
        layout.Elements[5].Should().Be(new VertexElement(VertexElementSemantic.BlendIndices, VertexElementFormat.UByte4) { Offset = 52, SemanticIndex = 0 });
        layout.Elements[6].Should().Be(new VertexElement(VertexElementSemantic.BlendWeight, VertexElementFormat.UNormByte4) { Offset = 56, SemanticIndex = 0 });
    }

    #endregion

    #region Heavy Custom Layout Stress Test

    [Fact]
    public void CustomHeavyUberVertex_CalculatesOffsetsAndIndicesFlawlessly()
    {
        // Complex custom uber-shader vertex layout:
        // - Geometry: Position (12B), Normal (12B), Tangent (16B) = 40B
        // - Colors: DiffuseColor (4B), SpecularColor (4B) = 8B
        // - Texture coordinates: UV0 Float2 (8B), UV1 Lightmap Half2 (4B), UV2 DetailMap Half2 (4B) = 16B
        // - Skinning with 8 bones: Bones 0..3 (4B), Bones 4..7 (4B), Weights 0..3 (4B), Weights 4..7 (4B) = 16B
        // TOTAL: 40 + 8 + 16 + 16 = 80 bytes
        var customLayout = new VertexLayout([
            new(VertexElementSemantic.Position, VertexElementFormat.Float3),       // [0]  Off: 0,  Idx: 0, Size: 12
            new(VertexElementSemantic.Normal, VertexElementFormat.Float3),         // [1]  Off: 12, Idx: 0, Size: 12
            new(VertexElementSemantic.Tangent, VertexElementFormat.Float4),        // [2]  Off: 24, Idx: 0, Size: 16
            new(VertexElementSemantic.Color, VertexElementFormat.UNormByte4),      // [3]  Off: 40, Idx: 0, Size: 4
            new(VertexElementSemantic.Color, VertexElementFormat.UNormByte4),      // [4]  Off: 44, Idx: 1, Size: 4
            new(VertexElementSemantic.TexCoord, VertexElementFormat.Float2),       // [5]  Off: 48, Idx: 0, Size: 8
            new(VertexElementSemantic.TexCoord, VertexElementFormat.Half2),        // [6]  Off: 56, Idx: 1, Size: 4
            new(VertexElementSemantic.TexCoord, VertexElementFormat.Half2),        // [7]  Off: 60, Idx: 2, Size: 4
            new(VertexElementSemantic.BlendIndices, VertexElementFormat.UByte4),   // [8]  Off: 64, Idx: 0, Size: 4
            new(VertexElementSemantic.BlendIndices, VertexElementFormat.UByte4),   // [9]  Off: 68, Idx: 1, Size: 4
            new(VertexElementSemantic.BlendWeight, VertexElementFormat.UNormByte4),// [10] Off: 72, Idx: 0, Size: 4
            new(VertexElementSemantic.BlendWeight, VertexElementFormat.UNormByte4) // [11] Off: 76, Idx: 1, Size: 4
        ]);

        // 1. Verify continuous layout integrity and cumulative stride
        AssertLayoutIntegrity(customLayout, expectedStride: 80);

        // 2. Validate multi-semantic element indexing
        var elems = customLayout.Elements;
        elems.Length.Should().Be(12, because: "the custom layout defines exactly 12 vertex elements");

        // Color indices (0 and 1)
        elems[3].SemanticIndex.Should().Be(0, because: "first Color element must have index 0");
        elems[4].SemanticIndex.Should().Be(1, because: "second Color element must have index 1");

        // TexCoord indices (0, 1, 2)
        elems[5].SemanticIndex.Should().Be(0, because: "first TexCoord element must have index 0");
        elems[6].SemanticIndex.Should().Be(1, because: "second TexCoord element must have index 1");
        elems[7].SemanticIndex.Should().Be(2, because: "third TexCoord element must have index 2");

        // 8-bone BlendIndices (0 and 1)
        elems[8].SemanticIndex.Should().Be(0, because: "first BlendIndices element must have index 0");
        elems[9].SemanticIndex.Should().Be(1, because: "second BlendIndices element must have index 1");

        // 8-bone BlendWeight (0 and 1)
        elems[10].SemanticIndex.Should().Be(0, because: "first BlendWeight element must have index 0");
        elems[11].SemanticIndex.Should().Be(1, because: "second BlendWeight element must have index 1");

        // 3. Check critical offset checkpoints
        elems[5].Offset.Should().Be(48, because: "first TexCoord must be placed immediately after the second Color element");
        elems[8].Offset.Should().Be(64, because: "first BlendIndices must be placed immediately after the last TexCoord element");
        elems[11].Offset.Should().Be(76, because: "last BlendWeight element must start at byte 76");
        (elems[11].Offset + elems[11].Format.ByteWidth()).Should().Be(80, because: "end of the last element must match layout stride");

        // 4. Validate value equality and hash code consistency
        var identicalLayout = new VertexLayout([
            new(VertexElementSemantic.Position, VertexElementFormat.Float3),
            new(VertexElementSemantic.Normal, VertexElementFormat.Float3),
            new(VertexElementSemantic.Tangent, VertexElementFormat.Float4),
            new(VertexElementSemantic.Color, VertexElementFormat.UNormByte4),
            new(VertexElementSemantic.Color, VertexElementFormat.UNormByte4),
            new(VertexElementSemantic.TexCoord, VertexElementFormat.Float2),
            new(VertexElementSemantic.TexCoord, VertexElementFormat.Half2),
            new(VertexElementSemantic.TexCoord, VertexElementFormat.Half2),
            new(VertexElementSemantic.BlendIndices, VertexElementFormat.UByte4),
            new(VertexElementSemantic.BlendIndices, VertexElementFormat.UByte4),
            new(VertexElementSemantic.BlendWeight, VertexElementFormat.UNormByte4),
            new(VertexElementSemantic.BlendWeight, VertexElementFormat.UNormByte4)
        ]);

        customLayout.Should().Be(identicalLayout, because: "two layouts with identical elements must be structurally equal");
        customLayout.GetHashCode().Should().Be(identicalLayout.GetHashCode(), because: "equal layouts must produce identical hash codes for caching");
    }

    #endregion
}