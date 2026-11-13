namespace Torture.Core.Graphics;

public readonly record struct VertexLayout : IEquatable<VertexLayout>
{
    private readonly VertexElement[] _elements;

    public VertexLayout(VertexElement[] elements)
    {
        if (elements == null || elements.Length == 0)
        {
            throw new ArgumentNullException(nameof(elements));
        }

        _elements = elements;

        Span<uint> semanticCounter = stackalloc uint[(int)VertexElementSemantic.Count];

        for (int i = 0; i < _elements.Length; ++i)
        {
            uint semanticIndex = semanticCounter[(int)_elements[i].SemanticName];
            ++semanticCounter[(int)_elements[i].SemanticName];

            if (i != 0)
            {
                _elements[i].SemanticIndex = semanticIndex;
                _elements[i].ByteOffset = _elements[i - 1].ByteOffset + _elements[i - 1].Format.ByteSize();
            }
            else
            {
                _elements[i].SemanticIndex = semanticIndex;
                _elements[i].ByteOffset = 0;
            }

            ByteStride += _elements[i].Format.ByteSize();
        }
    }

    /// <summary>
    /// Layout size in bytes.
    /// </summary>
    public uint ByteStride { get; }

    public ReadOnlySpan<VertexElement> Elements => _elements;

    public bool Equals(VertexLayout other)
    {
        return _elements.AsSpan().SequenceEqual(other._elements.AsSpan());
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();

        for (int i = 0; i < _elements.Length; ++i)
        {
            hash.Add(_elements[i]);
        }

        return hash.ToHashCode();
    }
}
