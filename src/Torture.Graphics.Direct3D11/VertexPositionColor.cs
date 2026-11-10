using System.Numerics;

using Vortice.Mathematics;

namespace Torture.Graphics.Direct3D11;

public readonly struct VertexPositionColor
{
    public static unsafe readonly uint SizeInBytes = (uint)sizeof(VertexPositionColor);

    public VertexPositionColor(in Vector3 position, in Color4 color)
    {
        Position = position;
        Color = color;
    }

    public readonly Vector3 Position;
    public readonly Color4 Color;
}