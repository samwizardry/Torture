namespace Torture.Core.Graphics;

public readonly record struct VertexLayout : IEquatable<VertexLayout>
{
    public uint Stride { get; }

    private readonly VertexElement[] _elements;

    public VertexLayout(VertexElement[] elements)
    {
        if (elements == null || elements.Length == 0)
        {
            throw new ArgumentNullException(nameof(elements));
        }

        _elements = elements;

        Span<uint> semanticCounter = stackalloc uint[(int)VertexElementSemantic.Count];

        Stride = 0;

        for (int i = 0; i < _elements.Length; ++i)
        {
            uint semanticIndex = semanticCounter[(int)_elements[i].SemanticName];
            ++semanticCounter[(int)_elements[i].SemanticName];

            if (i != 0)
            {
                _elements[i].SemanticIndex = semanticIndex;
                _elements[i].Offset = _elements[i - 1].Offset + _elements[i - 1].Format.ByteWidth();
            }
            else
            {
                _elements[i].SemanticIndex = semanticIndex;
                _elements[i].Offset = 0;
            }

            Stride += _elements[i].Format.ByteWidth();
        }
    }

    public ReadOnlySpan<VertexElement> Elements => _elements;

    public bool Equals(VertexLayout other)
    {
        return Stride == other.Stride &&
            _elements.AsSpan().SequenceEqual(other._elements.AsSpan());
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(Stride);
        if (_elements != null)
        {
            foreach (var element in _elements)
            {
                hash.Add(element);
            }
        }
        return hash.ToHashCode();
    }
}
