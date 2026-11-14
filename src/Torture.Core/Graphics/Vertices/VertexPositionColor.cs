using System.Numerics;
using System.Runtime.InteropServices;

using Vortice.Mathematics;

namespace Torture.Core.Graphics;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public record struct VertexPositionColor(Vector3 position, Color4 color)
{
    public static readonly VertexLayout Layout;

    public Vector3 Position = position;
    public Color4 Color = color;

    static VertexPositionColor()
    {
        Layout = new VertexLayout([
            new VertexElement(VertexElementSemantic.Position, VertexElementFormat.Float3),
            new VertexElement(VertexElementSemantic.Color, VertexElementFormat.Float4),
        ]);
    }
}
