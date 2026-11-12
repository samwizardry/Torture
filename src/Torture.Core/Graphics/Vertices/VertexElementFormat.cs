namespace Torture.Core.Graphics;

public enum VertexElementFormat : byte
{
    /// <summary>
    /// Single 32-bit floating point number.
    /// </summary>
    Float1,
    /// <summary>
    /// Two component 32-bit floating point number.
    /// </summary>
    Float2,
    /// <summary>
    /// Three component 32-bit floating point number.
    /// </summary>
    Float3,
    /// <summary>
    /// Four component 32-bit floating point number.
    /// </summary>
    Float4,
    /// <summary>
    /// Single signed 32-bit integer.
    /// </summary>
    Int1,
    /// <summary>
    /// Two component signed 32-bit integer.
    /// </summary>
    Int2,
    /// <summary>
    /// Three component signed 32-bit integer.
    /// </summary>
    Int3,
    /// <summary>
    /// Four component signed 32-bit integer.
    /// </summary>
    Int4,
    /// <summary>
    /// Four component unsigned 32-bit integer.
    /// </summary>
    UInt1,
    /// <summary>
    /// Four component unsigned 32-bit integer.
    /// </summary>
    UInt2,
    /// <summary>
    /// Four component unsigned 32-bit integer.
    /// </summary>
    UInt3,
    /// <summary>
    /// Four component unsigned 32-bit integer.
    /// </summary>
    UInt4,
    /// <summary>
    /// Two component 16-bit floating point number.
    /// </summary>
    Half2,
    /// <summary>
    /// Four component 16-bit floating point number.
    /// </summary>
    Half4,
    /// <summary>
    /// Two component signed 16-bit integer.
    /// </summary>
    Short2,
    /// <summary>
    /// Four component signed 16-bit integer.
    /// </summary>
    Short4,
    /// <summary>
    /// Two component unsigned 16-bit integer.
    /// </summary>
    UShort2,
    /// <summary>
    /// Four component unsigned 16-bit integer.
    /// </summary>
    UShort4,
    /// <summary>
    /// Two component signed 16-bit normalized integer.
    /// </summary>
    NormShort2,
    /// <summary>
    /// Four component signed 16-bit normalized  integer.
    /// </summary>
    NormShort4,
    /// <summary>
    /// Two component signed 16-bit normalized integer.
    /// </summary>
    UNormShort2,
    /// <summary>
    /// Four component signed 16-bit normalized integer.
    /// </summary>
    UNormShort4,
    /// <summary>
    /// Two component signed 8-bit integer.
    /// </summary>
    Byte2,
    /// <summary>
    /// Four component signed 8-bit integer.
    /// </summary>
    Byte4,
    /// <summary>
    /// Two component unsigned 8-bit integer.
    /// </summary>
    UByte2,
    /// <summary>
    /// Four component unsigned 8-bit integer.
    /// </summary>
    UByte4,
    /// <summary>
    /// Two component signed 8-bit normalized integer.
    /// </summary>
    NormByte2,
    /// <summary>
    /// Four component unsigned 8-bit normalized integer.
    /// </summary>
    NormByte4,
    /// <summary>
    /// Two component unsigned 8-bit normalized integer.
    /// </summary>
    UNormByte2,
    /// <summary>
    /// Four component unsigned 8-bit normalized integer.
    /// </summary>
    UNormByte4
}

public static class VertexElementFormatExtensions
{
    public static uint ByteWidth(this VertexElementFormat format)
    {
        switch (format)
        {
            case VertexElementFormat.Float4:
            case VertexElementFormat.Int4:
            case VertexElementFormat.UInt4:
                return 16;

            case VertexElementFormat.Float3:
            case VertexElementFormat.Int3:
            case VertexElementFormat.UInt3:
                return 12;

            case VertexElementFormat.Float2:
            case VertexElementFormat.Int2:
            case VertexElementFormat.UInt2:
            case VertexElementFormat.Half4:
            case VertexElementFormat.Short4:
            case VertexElementFormat.UShort4:
            case VertexElementFormat.NormShort4:
            case VertexElementFormat.UNormShort4:
                return 8;

            case VertexElementFormat.Float1:
            case VertexElementFormat.Int1:
            case VertexElementFormat.UInt1:
            case VertexElementFormat.Half2:
            case VertexElementFormat.Short2:
            case VertexElementFormat.UShort2:
            case VertexElementFormat.NormShort2:
            case VertexElementFormat.UNormShort2:
            case VertexElementFormat.Byte4:
            case VertexElementFormat.UByte4:
            case VertexElementFormat.NormByte4:
            case VertexElementFormat.UNormByte4:
                return 4;

            case VertexElementFormat.Byte2:
            case VertexElementFormat.UByte2:
            case VertexElementFormat.NormByte2:
            case VertexElementFormat.UNormByte2:
                return 2;

            default: throw new NotSupportedException();
        }
    }
}
