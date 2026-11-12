using System.Runtime.CompilerServices;

using FluentAssertions;

using Torture.Core.Graphics;

namespace Torture.UnitTests;

public class Vertices
{
    [Theory]
    // 16 байт
    [InlineData(VertexElementFormat.Float4, 16)]
    [InlineData(VertexElementFormat.Int4, 16)]
    [InlineData(VertexElementFormat.UInt4, 16)]
    // 12 байт
    [InlineData(VertexElementFormat.Float3, 12)]
    [InlineData(VertexElementFormat.Int3, 12)]
    [InlineData(VertexElementFormat.UInt3, 12)]
    // 8 байт
    [InlineData(VertexElementFormat.Float2, 8)]
    [InlineData(VertexElementFormat.Int2, 8)]
    [InlineData(VertexElementFormat.UInt2, 8)]
    [InlineData(VertexElementFormat.Half4, 8)]
    [InlineData(VertexElementFormat.Short4, 8)]
    [InlineData(VertexElementFormat.UShort4, 8)]
    [InlineData(VertexElementFormat.NormShort4, 8)]
    [InlineData(VertexElementFormat.UNormShort4, 8)]
    // 4 байта
    [InlineData(VertexElementFormat.Float1, 4)]
    [InlineData(VertexElementFormat.Int1, 4)]
    [InlineData(VertexElementFormat.UInt1, 4)]
    [InlineData(VertexElementFormat.Half2, 4)]
    [InlineData(VertexElementFormat.Short2, 4)]
    [InlineData(VertexElementFormat.UShort2, 4)]
    [InlineData(VertexElementFormat.NormShort2, 4)]
    [InlineData(VertexElementFormat.UNormShort2, 4)]
    [InlineData(VertexElementFormat.Byte4, 4)]
    [InlineData(VertexElementFormat.UByte4, 4)]
    [InlineData(VertexElementFormat.NormByte4, 4)]
    [InlineData(VertexElementFormat.UNormByte4, 4)]
    // 2 байта
    [InlineData(VertexElementFormat.Byte2, 2)]
    [InlineData(VertexElementFormat.UByte2, 2)]
    [InlineData(VertexElementFormat.NormByte2, 2)]
    [InlineData(VertexElementFormat.UNormByte2, 2)]
    public void ByteWidth_ReturnsExpectedSize(VertexElementFormat format, uint expectedBytes)
    {
        uint actualBytes = format.ByteWidth();
        actualBytes.Should().Be(expectedBytes,
            because: $"формат {format} обязан занимать ровно {expectedBytes} байт в видеопамяти");
    }

    [Fact]
    public void ByteWidth_CoversAllEnumValues()
    {
        var allFormats = Enum.GetValues<VertexElementFormat>();
        allFormats.Should().NotBeEmpty();
        foreach (var format in allFormats)
        {
            // Проверяем, что для любого формата не бросается NotSupportedException
            var act = () => format.ByteWidth();
            act.Should().NotThrow($"для формата {format} обязана быть ветка в switch");
            // Проверяем, что размер попадает в допустимые GPU-размеры
            uint width = format.ByteWidth();
            width.Should().BeOneOf([2u, 4u, 8u, 12u, 16u],
                because: $"размер {width} байт для формата {format} должен быть аппаратно валидным для видеокарты");
        }
    }

    [Fact]
    public void VertexPosition_Layout_IsCorrect()
    {
        var layout = VertexPosition.Layout;
        AssertLayoutIntegrity(layout, expectedStride: 12);
        layout.Stride.Should().Be((uint)Unsafe.SizeOf<VertexPosition>());
        layout.Elements.Length.Should().Be(1);
        layout.Elements[0].Should().Be(new VertexElement(VertexElementSemantic.Position, VertexElementFormat.Float3) { Offset = 0, SemanticIndex = 0 });
    }

    [Fact]
    public void VertexPositionColor_Layout_IsCorrect()
    {
        var layout = VertexPositionColor.Layout;
        AssertLayoutIntegrity(layout, expectedStride: 28);
        layout.Stride.Should().Be((uint)Unsafe.SizeOf<VertexPositionColor>());
        layout.Elements.Length.Should().Be(2);
        layout.Elements[0].Should().Be(new VertexElement(VertexElementSemantic.Position, VertexElementFormat.Float3) { Offset = 0, SemanticIndex = 0 });
        layout.Elements[1].Should().Be(new VertexElement(VertexElementSemantic.Color, VertexElementFormat.Float4) { Offset = 12, SemanticIndex = 0 });
    }

    [Fact]
    public void VertexPositionTexture_Layout_IsCorrect()
    {
        var layout = VertexPositionTexture.Layout;
        AssertLayoutIntegrity(layout, expectedStride: 20);
        layout.Stride.Should().Be((uint)Unsafe.SizeOf<VertexPositionTexture>());
        layout.Elements.Length.Should().Be(2);
        layout.Elements[0].Should().Be(new VertexElement(VertexElementSemantic.Position, VertexElementFormat.Float3) { Offset = 0, SemanticIndex = 0 });
        layout.Elements[1].Should().Be(new VertexElement(VertexElementSemantic.TexCoord, VertexElementFormat.Float2) { Offset = 12, SemanticIndex = 0 });
    }

    [Fact]
    public void VertexPositionDualTexture_Layout_CalculatesConsecutiveTexCoords()
    {
        var layout = VertexPositionDualTexture.Layout;
        AssertLayoutIntegrity(layout, expectedStride: 28);
        layout.Stride.Should().Be((uint)Unsafe.SizeOf<VertexPositionDualTexture>());
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
        layout.Stride.Should().Be((uint)Unsafe.SizeOf<VertexPositionNormal>());
        layout.Elements.Length.Should().Be(2);
        layout.Elements[0].Should().Be(new VertexElement(VertexElementSemantic.Position, VertexElementFormat.Float3) { Offset = 0, SemanticIndex = 0 });
        layout.Elements[1].Should().Be(new VertexElement(VertexElementSemantic.Normal, VertexElementFormat.Float3) { Offset = 12, SemanticIndex = 0 });
    }

    [Fact]
    public void VertexPositionColorTexture_Layout_IsCorrect()
    {
        var layout = VertexPositionColorTexture.Layout;
        AssertLayoutIntegrity(layout, expectedStride: 36);
        layout.Stride.Should().Be((uint)Unsafe.SizeOf<VertexPositionColorTexture>());
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
        layout.Stride.Should().Be((uint)Unsafe.SizeOf<VertexPositionNormalColor>());
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
        layout.Stride.Should().Be((uint)Unsafe.SizeOf<VertexPositionNormalTexture>());
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
        layout.Stride.Should().Be((uint)Unsafe.SizeOf<VertexPositionNormalColorTexture>());
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
        layout.Stride.Should().Be((uint)Unsafe.SizeOf<VertexPositionNormalTangentColorTexture>());
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
        layout.Stride.Should().Be((uint)Unsafe.SizeOf<VertexPositionNormalTangentColorTextureSkinning>());
        layout.Elements.Length.Should().Be(7);
        layout.Elements[0].Should().Be(new VertexElement(VertexElementSemantic.Position, VertexElementFormat.Float3) { Offset = 0, SemanticIndex = 0 });
        layout.Elements[1].Should().Be(new VertexElement(VertexElementSemantic.Normal, VertexElementFormat.Float3) { Offset = 12, SemanticIndex = 0 });
        layout.Elements[2].Should().Be(new VertexElement(VertexElementSemantic.Tangent, VertexElementFormat.Float4) { Offset = 24, SemanticIndex = 0 });
        layout.Elements[3].Should().Be(new VertexElement(VertexElementSemantic.Color, VertexElementFormat.UNormByte4) { Offset = 40, SemanticIndex = 0 });
        layout.Elements[4].Should().Be(new VertexElement(VertexElementSemantic.TexCoord, VertexElementFormat.Float2) { Offset = 44, SemanticIndex = 0 });
        layout.Elements[5].Should().Be(new VertexElement(VertexElementSemantic.BlendIndices, VertexElementFormat.UByte4) { Offset = 52, SemanticIndex = 0 });
        layout.Elements[6].Should().Be(new VertexElement(VertexElementSemantic.BlendWeight, VertexElementFormat.UNormByte4) { Offset = 56, SemanticIndex = 0 });
    }

    [Fact]
    public void CustomHeavyUberVertex_CalculatesOffsetsAndIndicesFlawlessly()
    {
        // Сложная вершина продвинутого шейдера:
        // - Геометрия: Position (12B), Normal (12B), Tangent (16B) = 40B
        // - Цвета: DiffuseColor (4B), SpecularColor (4B) = 8B
        // - Текстурные координаты: UV0 Float2 (8B), UV1 Lightmap Half2 (4B), UV2 DetailMap Half2 (4B) = 16B
        // - Скиннинг на 8 костей: Bones 0..3 (4B), Bones 4..7 (4B), Weights 0..3 (4B), Weights 4..7 (4B) = 16B
        // ИТОГО: 40 + 8 + 16 + 16 = 80 байт
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
        // 1. Проверяем авто-расчёт целостности
        AssertLayoutIntegrity(customLayout, expectedStride: 80);
        // 2. Детальная проверка каждого элемента
        var elems = customLayout.Elements;
        elems.Length.Should().Be(12);
        // Проверка семантических индексов для повторяющихся семантик
        elems[3].SemanticIndex.Should().Be(0);
        elems[4].SemanticIndex.Should().Be(1);
        elems[5].SemanticIndex.Should().Be(0);
        elems[6].SemanticIndex.Should().Be(1);
        elems[7].SemanticIndex.Should().Be(2);
        elems[8].SemanticIndex.Should().Be(0);
        elems[9].SemanticIndex.Should().Be(1);
        elems[10].SemanticIndex.Should().Be(0);
        elems[11].SemanticIndex.Should().Be(1);
        // Проверка смещений критических точек
        elems[5].Offset.Should().Be(48); // Первый TexCoord
        elems[8].Offset.Should().Be(64); // Первый BlendIndices
        elems[11].Offset.Should().Be(76); // Последний элемент
        (elems[11].Offset + elems[11].Format.ByteWidth()).Should().Be(80);
        // 3. Проверка Structural Equality и HashCode
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
        customLayout.Should().Be(identicalLayout);
        customLayout.GetHashCode().Should().Be(identicalLayout.GetHashCode());
    }

    private static void AssertLayoutIntegrity(VertexLayout layout, uint expectedStride)
    {
        var elements = layout.Elements;
        elements.Length.Should().BeGreaterThan(0, "лейаут не может быть пустым");
        uint runningOffset = 0;
        Dictionary<VertexElementSemantic, uint> expectedSemanticCounters = [];
        for (int i = 0; i < elements.Length; i++)
        {
            var element = elements[i];
            uint formatSize = element.Format.ByteWidth();
            // 1. Проверка непрерывности смещения (Offset)
            element.Offset.Should().Be(runningOffset,
                $"элемент [{i}] ({element.SemanticName}) должен находиться по смещению {runningOffset} байт");
            // 2. Проверка монотонности SemanticIndex
            uint expectedIndex = expectedSemanticCounters.GetValueOrDefault(element.SemanticName, 0u);
            element.SemanticIndex.Should().Be(expectedIndex,
                $"семантика {element.SemanticName} на позиции [{i}] должна иметь индекс {expectedIndex}");
            expectedSemanticCounters[element.SemanticName] = expectedIndex + 1;
            runningOffset += formatSize;
        }
        // 3. Проверка итогового Stride
        layout.Stride.Should().Be(runningOffset, "Stride должен точно равняться сумме размеров всех элементов");
        layout.Stride.Should().Be(expectedStride, $"Stride должен соответствовать ожидаемому значению ({expectedStride} байт)");
    }
}
