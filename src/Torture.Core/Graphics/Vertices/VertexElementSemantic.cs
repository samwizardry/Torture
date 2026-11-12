namespace Torture.Core.Graphics;

public enum VertexElementSemantic : byte
{
    /// <summary>
    /// SV_Position
    /// </summary>
    Position,
    /// <summary>
    /// COLOR
    /// </summary>
    Color,
    /// <summary>
    /// TEXCOORD
    /// </summary>
    TexCoord,
    /// <summary>
    /// NORMAL
    /// </summary>
    Normal,
    /// <summary>
    /// TANGENT
    /// </summary>
    Tangent,
    /// <summary>
    /// BLENDINDICES
    /// </summary>
    BlendIndices,
    /// <summary>
    /// BLENDWEIGHT
    /// </summary>
    BlendWeight,
    /// <summary>
    /// Should always be the last element. Do not delete.
    /// </summary>
    Count
}
