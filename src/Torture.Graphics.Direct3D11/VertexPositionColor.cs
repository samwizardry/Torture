using System.Numerics;
using System.Runtime.InteropServices;

using Vortice.Mathematics;

namespace Torture.Graphics.Direct3D11;

public readonly struct VertexPositionColor
{
    public static readonly uint SizeInBytes = (uint)Marshal.SizeOf<VertexPositionColor>();

    public VertexPositionColor(in Vector3 position, in Color4 color)
    {
        Position = position;
        Color = color;
    }

    public readonly Vector3 Position;
    public readonly Color4 Color;
}