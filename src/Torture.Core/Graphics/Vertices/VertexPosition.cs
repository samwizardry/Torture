using System.Numerics;
using System.Runtime.InteropServices;

namespace Torture.Core.Graphics;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public readonly record struct VertexPosition(Vector3 Position)
{
    public static readonly VertexLayout Layout;

    static VertexPosition()
    {
        Layout = new VertexLayout([
            new VertexElement(VertexElementSemantic.Position, VertexElementFormat.Float3)
        ]);
    }
}
