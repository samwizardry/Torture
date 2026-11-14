using System.Numerics;
using System.Runtime.InteropServices;

using Vortice.Mathematics;

namespace Torture.Core.Graphics;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public record struct VertexPositionNormalColor(Vector3 position, Vector3 normal, Color4 color)
{
    public static readonly VertexLayout Layout;

    public Vector3 Position = position;
    public Vector3 Normal = normal;
    public Color4 Color = color;

    static VertexPositionNormalColor()
    {
        Layout = new VertexLayout([
            new VertexElement(VertexElementSemantic.Position, VertexElementFormat.Float3),
            new VertexElement(VertexElementSemantic.Normal, VertexElementFormat.Float3),
            new VertexElement(VertexElementSemantic.Color, VertexElementFormat.Float4)
        ]);
    }
}
