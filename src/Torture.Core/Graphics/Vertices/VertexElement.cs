namespace Torture.Core.Graphics;

public record struct VertexElement : IEquatable<VertexElement>
{
    public readonly VertexElementSemantic SemanticName;
    public uint SemanticIndex = 0;
    public readonly VertexElementFormat Format;
    public uint Offset = 0;

    public VertexElement(VertexElementSemantic semanticName, VertexElementFormat format)
    {
        SemanticName = semanticName;
        Format = format;
    }

    public readonly bool Equals(VertexElement other)
    {
        return SemanticName == other.SemanticName &&
            SemanticIndex == other.SemanticIndex &&
            Format == other.Format &&
            Offset == other.Offset;
    }

    public override readonly int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(SemanticName);
        hash.Add(SemanticIndex);
        hash.Add(Format);
        hash.Add(Offset);
        return hash.ToHashCode();
    }
}
