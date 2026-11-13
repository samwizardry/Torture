using System.Numerics;

using Brutal.Collections;

namespace Torture.Core.Graphics;

public static class Geometry
{
    // Helper for flipping winding of geometric primitives for LH vs. RH coords
    private static void ReverseWinding(NativeList<ushort> indices, NativeList<VertexPositionNormalTexture> vertices)
    {
        if (indices.Count % 3 != 0)
        {
            throw new InvalidOperationException();
        }

        for (int i = 0; i < indices.Count; i += 3)
        {
            // Swap
            ushort temp = indices[i];
            indices[i] = indices[i + 2];
            indices[i + 2] = temp;
        }

        for (int i = 0; i < vertices.Count; i += 3)
        {
            vertices[i].TexCoord = new Vector2(1.0f - vertices[i].TexCoord.X, vertices[i].TexCoord.Y);
        }
    }

    // Helper for inverting normals of geometric primitives for 'inside' vs. 'outside' viewing
    private static void InvertNormals(NativeList<VertexPositionNormalTexture> vertices)
    {
        for (int i = 0; i < vertices.Count; i++)
        {
            vertices[i].Normal = Vector3.Negate(vertices[i].Normal);
        }
    }

    private static readonly Vector3[] boxFaceNormals = [
        new(0, 0, 1),
        new(0, 0, -1),
        new(1, 0, 0),
        new(-1, 0, 0),
        new(0, 1, 0),
        new(0, -1, 0)];

    private static readonly Vector2[] boxTexCoords = [
        new(1, 0),
        new(1, 1),
        new(0, 1),
        new(0, 0)];

    /// <summary>
    /// Cube (aka a Hexahedron) or Box.
    /// </summary>
    public static (NativeList<ushort> Indices, NativeList<VertexPositionNormalTexture> Vertices) ComputeBox(Vector3 size, bool rhcoords, bool invertn)
    {
        NativeList<VertexPositionNormalTexture> vertices = [];
        NativeList<ushort> indices = [];

        // A box has six faces, each one pointing in a different direction.
        const int faceCount = 6;
        var tsize = size / 2.0f;

        for (int i = 0; i < faceCount; ++i)
        {
            Vector3 normal = boxFaceNormals[i];

            // Get two vectors perpendicular both to the face normal and to each other.
            Vector3 basis = i >= 4 ? Vector3.UnitZ : Vector3.UnitY;
            Vector3 side1 = Vector3.Cross(normal, basis);
            Vector3 side2 = Vector3.Cross(normal, side1);

            // Six indices (two triangles) per face.
            int vbase = indices.Count;
            indices.Add((ushort)(vbase + 0));
            indices.Add((ushort)(vbase + 1));
            indices.Add((ushort)(vbase + 2));
            indices.Add((ushort)(vbase + 0));
            indices.Add((ushort)(vbase + 2));
            indices.Add((ushort)(vbase + 3));

            // Four vertices per face.
            // (normal - side1 - side2) * tsize // normal // t0
            vertices.Add(new(Vector3.Multiply(normal - side1 - side2, tsize), normal, boxTexCoords[0]));
            // (normal - side1 + side2) * tsize // normal // t1
            vertices.Add(new(Vector3.Multiply(normal - side1 + side2, tsize), normal, boxTexCoords[1]));
            // (normal + side1 + side2) * tsize // normal // t2
            vertices.Add(new(Vector3.Multiply(normal + side1 + side2, tsize), normal, boxTexCoords[2]));
            // (normal + side1 - side2) * tsize // normal // t3
            vertices.Add(new(Vector3.Multiply(normal + side1 - side2, tsize), normal, boxTexCoords[3]));
        }

        // Build RH above
        if (!rhcoords)
        {
            ReverseWinding(indices, vertices);
        }

        if (invertn)
        {
            InvertNormals(vertices);
        }

        return (indices, vertices);
    }
}
