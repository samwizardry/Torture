using System.Numerics;

using Brutal.Collections;

using Torture.Core.Math;

namespace Torture.Core.Graphics;

public static class Geometry
{
    private static readonly Vector3[] BoxFaceNormals = [
        new(0, 0, 1),
        new(0, 0, -1),
        new(1, 0, 0),
        new(-1, 0, 0),
        new(0, 1, 0),
        new(0, -1, 0)];

    private static readonly Vector2[] BoxTexCoords = [
        new(1, 0),
        new(1, 1),
        new(0, 1),
        new(0, 0)];

    private static readonly Vector3[] OctahedronVertices =
    [
        // when looking down the negative z-axis (into the screen)
        new Vector3(0, 1, 0),  // 0 top
        new Vector3(0, 0, -1), // 1 front
        new Vector3(1, 0, 0),  // 2 right
        new Vector3(0, 0, 1),  // 3 back
        new Vector3(-1, 0, 0), // 4 left
        new Vector3(0, -1, 0), // 5 bottom
    ];

    private static readonly ushort[] OctahedronIndices =
    [
        // clang-format off
        0, 1, 2, // top front-right face
        0, 2, 3, // top back-right face
        0, 3, 4, // top back-left face
        0, 4, 1, // top front-left face
        5, 1, 4, // bottom front-left face
        5, 4, 3, // bottom back-left face
        5, 3, 2, // bottom back-right face
        5, 2, 1, // bottom front-right face
        // clang-format on
    ];

    /// <summary>
    /// Cube (aka a Hexahedron) or Box.
    /// </summary>
    public static (NativeList<VertexPositionNormalTexture> Vertices, NativeList<ushort> Indices) ComputeBox(
        Vector3 size, bool rhcoords, bool invertn)
    {
        NativeList<VertexPositionNormalTexture> vertices = [];
        NativeList<ushort> indices = [];

        // A box has six faces, each one pointing in a different direction.
        const int faceCount = 6;
        const int vertexCount = faceCount * 4;
        const int indexCount = faceCount * 6;

        vertices.EnsureCapacity(vertexCount);
        indices.EnsureCapacity(indexCount);

        Vector3 tsize = size * 0.5f;

        for (int i = 0; i < faceCount; ++i)
        {
            Vector3 normal = BoxFaceNormals[i];

            // Get two vectors perpendicular both to the face normal and to each other.
            Vector3 basis = i >= 4 ? Vector3.UnitZ : Vector3.UnitY;
            Vector3 side1 = Vector3.Cross(normal, basis);
            Vector3 side2 = Vector3.Cross(normal, side1);

            // Six indices (two triangles) per face.
            int vbase = vertices.Count;
            IndexPushBack(indices, vbase + 0);
            IndexPushBack(indices, vbase + 1);
            IndexPushBack(indices, vbase + 2);

            IndexPushBack(indices, vbase + 0);
            IndexPushBack(indices, vbase + 2);
            IndexPushBack(indices, vbase + 3);

            // Four vertices per face.
            // (normal - side1 - side2) * tsize // normal // t0
            vertices.Add(new((normal - side1 - side2) * tsize, normal, BoxTexCoords[0]));
            // (normal - side1 + side2) * tsize // normal // t1
            vertices.Add(new((normal - side1 + side2) * tsize, normal, BoxTexCoords[1]));
            // (normal + side1 + side2) * tsize // normal // t2
            vertices.Add(new((normal + side1 + side2) * tsize, normal, BoxTexCoords[2]));
            // (normal + side1 - side2) * tsize // normal // t3
            vertices.Add(new((normal + side1 - side2) * tsize, normal, BoxTexCoords[3]));
        }

        // Build RH above
        if (!rhcoords)
            ReverseWinding(indices.AsSpan(), vertices.AsSpan());

        if (invertn)
            InvertNormals(vertices.AsSpan());

        return (vertices, indices);
    }

    /// <summary>
    /// Sphere.
    /// </summary>
    public static (NativeList<VertexPositionNormalTexture> Vertices, NativeList<ushort> Indices) ComputeSphere(
        float diameter, int tessellation, bool rhcoords, bool invertn)
    {
        if (tessellation < 3)
            throw new ArgumentException("Tessellation parameter must be at least 3.", nameof(tessellation));

        NativeList<VertexPositionNormalTexture> vertices = [];
        NativeList<ushort> indices = [];

        int verticalSegments = tessellation;
        int horizontalSegments = tessellation * 2;
        float radius = diameter * 0.5f;

        int vertexCount = (verticalSegments + 1) * (horizontalSegments + 1);
        int indexCount = verticalSegments * horizontalSegments * 6;

        CheckIndexOverflow((uint)vertexCount);

        vertices.EnsureCapacity(vertexCount);
        indices.EnsureCapacity(indexCount);

        // Create rings of vertices at progressively higher latitudes.
        for (int i = 0; i <= verticalSegments; ++i)
        {
            float v = 1.0f - (i / (float)verticalSegments);
            float latitude = (i * MathF.PI / verticalSegments) - TortureMath.HalfPI;

            (float dy, float dxz) = MathF.SinCos(latitude);

            // Create a single ring of vertices at this latitude.
            for (int j = 0; j <= horizontalSegments; ++j)
            {
                float u = j / (float)horizontalSegments;

                float longitude = j * MathF.Tau / horizontalSegments;
                (float dx, float dz) = MathF.SinCos(longitude);

                dx *= dxz;
                dz *= dxz;

                Vector3 normal = new(dx, dy, dz);
                Vector2 textureCoords = new(u, v);

                vertices.Add(new(normal * radius, normal, textureCoords));
            }
        }

        // Fill the index buffer with triangles joining each pair of latitude rings.
        int stride = horizontalSegments + 1;

        for (int i = 0; i < verticalSegments; ++i)
        {
            for (int j = 0; j < horizontalSegments; ++j)
            {
                int nextI = i + 1;
                int nextJ = j + 1;

                IndexPushBack(indices, i * stride + j);
                IndexPushBack(indices, nextI * stride + j);
                IndexPushBack(indices, i * stride + nextJ);

                IndexPushBack(indices, i * stride + nextJ);
                IndexPushBack(indices, nextI * stride + j);
                IndexPushBack(indices, nextI * stride + nextJ);
            }
        }

        // Build RH above
        if (!rhcoords)
            ReverseWinding(indices.AsSpan(), vertices.AsSpan());

        if (invertn)
            InvertNormals(vertices.AsSpan());

        return (vertices, indices);
    }

    /// <summary>
    /// Geodesic sphere.
    /// </summary>
    public static (NativeList<VertexPositionNormalTexture> Vertices, NativeList<ushort> Indices) ComputeGeoSphere(
        float diameter, int tessellation, bool rhcoords, bool invertn = false)
    {
        NativeList<VertexPositionNormalTexture> vertices = [];
        NativeList<ushort> indices = [.. OctahedronIndices];

        // Makes an undirected edge. Rather than overloading comparison operators to give us the (a,b)==(b,a) property,
        // we'll just ensure that the larger of the two goes first. This'll simplify things greatly.
        static (ushort, ushort) MakeUndirectedEdge(ushort a, ushort b) => (System.Math.Max(a, b), System.Math.Min(a, b));

        float radius = diameter * 0.5f;

        // Start with an octahedron; copy the data into the vertex/index collection.
        List<Vector3> vertexPositions = [.. OctahedronVertices];

        // We know these values by looking at the above index list for the octahedron. Despite the subdivisions that are
        // about to go on, these values aren't ever going to change because the vertices don't move around in the array.
        // We'll need these values later on to fix the singularities that show up at the poles.
        const ushort northPoleIndex = 0;
        const ushort southPoleIndex = 5;

        for (int iSubdivision = 0; iSubdivision < tessellation; ++iSubdivision)
        {
            if (indices.Count % 3 != 0) // sanity
                throw new InvalidOperationException("Indices count must be a multiple of 3.");

            // We use this to keep track of which edges have already been subdivided.
            Dictionary<(ushort, ushort), ushort> subdividedEdges = [];

            // The new index collection after subdivision.
            NativeList<ushort> newIndices = [];
            newIndices.EnsureCapacity(indices.Count * 4);

            int triangleCount = indices.Count / 3;
            for (int iTriangle = 0; iTriangle < triangleCount; ++iTriangle)
            {
                // For each edge on this triangle, create a new vertex in the middle of that edge.
                // The winding order of the triangles we output are the same as the winding order of the inputs.

                // Indices of the vertices making up this triangle
                ushort iv0 = indices[iTriangle * 3 + 0];
                ushort iv1 = indices[iTriangle * 3 + 1];
                ushort iv2 = indices[iTriangle * 3 + 2];

                // Function that, when given the index of two vertices, creates a new vertex at the midpoint of those vertices.
                void DivideEdge(ushort i0, ushort i1, out ushort outIndex)
                {
                    (ushort, ushort) edge = MakeUndirectedEdge(i0, i1);

                    // Check to see if we've already generated this vertex
                    if (!subdividedEdges.TryGetValue(edge, out outIndex))
                    {
                        // Haven't generated this vertex before: so add it now
                        Vector3 outVertex = (vertexPositions[i0] + vertexPositions[i1]) * 0.5f;

                        CheckIndexOverflow((uint)vertexPositions.Count);

                        outIndex = (ushort)vertexPositions.Count;
                        vertexPositions.Add(outVertex);

                        // Now add it to the map.
                        subdividedEdges.Add(edge, outIndex);
                    }
                }

                // Add/get new vertices and their indices
                DivideEdge(iv0, iv1, out ushort iv01);
                DivideEdge(iv1, iv2, out ushort iv12);
                DivideEdge(iv0, iv2, out ushort iv20);

                // Add the new indices. We have four new triangles from our original one:
                //        v0
                //        o
                //       /a\
                //  v20 o---o v01
                //     /b\c/d\
                // v2 o---o---o v1
                //       v12

                // a
                IndexPushBack(newIndices, iv0);
                IndexPushBack(newIndices, iv01);
                IndexPushBack(newIndices, iv20);

                // b
                IndexPushBack(newIndices, iv20);
                IndexPushBack(newIndices, iv12);
                IndexPushBack(newIndices, iv2);

                // c
                IndexPushBack(newIndices, iv20);
                IndexPushBack(newIndices, iv01);
                IndexPushBack(newIndices, iv12);

                // d
                IndexPushBack(newIndices, iv01);
                IndexPushBack(newIndices, iv1);
                IndexPushBack(newIndices, iv12);
            }

            indices.Dispose();
            indices = newIndices;
        }

        // Now that we've completed subdivision, fill in the final vertex collection
        vertices.EnsureCapacity(vertexPositions.Count);

        for (int i = 0; i < vertexPositions.Count; ++i)
        {
            Vector3 vertexPosition = vertexPositions[i];

            Vector3 normal = Vector3.Normalize(vertexPosition);
            Vector3 pos = normal * radius;

            // calculate texture coordinates for this vertex
            float longitude = MathF.Atan2(normal.X, -normal.Z);
            float latitude = MathF.Acos(normal.Y);

            float u = (longitude / MathF.Tau) + 0.5f;
            float v = latitude / MathF.PI;

            Vector2 texCoord = new(1.0f - u, v);
            vertices.Add(new(pos, normal, texCoord));
        }

        // There are a couple of fixes to do. One is a texture coordinate wraparound fixup. At some point, there will be
        // a set of triangles somewhere in the mesh with texture coordinates such that the wraparound across 0.0/1.0
        // occurs across that triangle. Eg. when the left hand side of the triangle has a U coordinate of 0.98 and the
        // right hand side has a U coordinate of 0.0. The intent is that such a triangle should render with a U of 0.98 to
        // 1.0, not 0.98 to 0.0. If we don't do this fixup, there will be a visible seam across one side of the sphere.
        //
        // Luckily this is relatively easy to fix. There is a straight edge which runs down the prime meridian of the
        // completed sphere. If you imagine the vertices along that edge, they circumscribe a semicircular arc starting at
        // y=1 and ending at y=-1, and sweeping across the range of z=0 to z=1. x stays zero. It's along this edge that we
        // need to duplicate our vertices - and provide the correct texture coordinates.
        int preFixupVertexCount = vertices.Count;
        for (int i = 0; i < preFixupVertexCount; ++i)
        {
            // This vertex is on the prime meridian if position.x and texcoord.u are both zero (allowing for small epsilon).
            var vertex = vertices[i];

            bool isOnPrimeMeridian =
                MathF.Abs(vertex.Position.X) <= TortureMath.Epsilon &&
                MathF.Abs(vertex.TexCoord.X) <= TortureMath.Epsilon;

            if (isOnPrimeMeridian)
            {
                CheckIndexOverflow((uint)vertices.Count);
                ushort newIndex = (ushort)vertices.Count.Value;

                // copy this vertex, correct the texture coordinate, and add the vertex
                var v = vertex;
                v.TexCoord.X = 1.0f;
                vertices.Add(v);

                // Now find all the triangles which contain this vertex and update them if necessary
                for (int j = 0; j < indices.Count; j += 3)
                {
                    int targetIdx;
                    int other1, other2;

                    if (indices[j + 0] == i)
                    {
                        targetIdx = j + 0;
                        other1 = j + 1;
                        other2 = j + 2;
                    }
                    else if (indices[j + 1] == i)
                    {
                        targetIdx = j + 1;
                        other1 = j + 0;
                        other2 = j + 2;
                    }
                    else if (indices[j + 2] == i)
                    {
                        targetIdx = j + 2;
                        other1 = j + 0;
                        other2 = j + 1;
                    }
                    else
                    {
                        continue;
                    }

                    var v0 = vertices[indices[targetIdx]];
                    var v1 = vertices[indices[other1]];
                    var v2 = vertices[indices[other2]];

                    // check the other two vertices to see if we might need to fix this triangle
                    if (MathF.Abs(v0.TexCoord.X - v1.TexCoord.X) > 0.5f ||
                        MathF.Abs(v0.TexCoord.X - v2.TexCoord.X) > 0.5f)
                    {
                        // yep; replace the specified index to point to the new, corrected vertex
                        indices[targetIdx] = newIndex;
                    }
                }
            }
        }

        // And one last fix we need to do: the poles. A common use-case of a sphere mesh is to map a rectangular texture onto
        // it. If that happens, then the poles become singularities which map the entire top and bottom rows of the texture
        // onto a single point. In general there's no real way to do that right. But to match the behavior of non-geodesic
        // spheres, we need to duplicate the pole vertex for every triangle that uses it. This will introduce seams near the
        // poles, but reduce stretching.
        void FixPole(ushort poleIndex)
        {
            var poleVertex = vertices[poleIndex];
            bool overwrittenPoleVertex = false; // overwriting the original pole vertex saves us one vertex

            for (int i = 0; i < indices.Count; i += 3)
            {
                int poleIdxPtr;
                int otherIdx0, otherIdx1;

                if (indices[i + 0] == poleIndex)
                {
                    poleIdxPtr = i + 0;
                    otherIdx0 = i + 1;
                    otherIdx1 = i + 2;
                }
                else if (indices[i + 1] == poleIndex)
                {
                    poleIdxPtr = i + 1;
                    otherIdx0 = i + 2;
                    otherIdx1 = i + 0;
                }
                else if (indices[i + 2] == poleIndex)
                {
                    poleIdxPtr = i + 2;
                    otherIdx0 = i + 0;
                    otherIdx1 = i + 1;
                }
                else
                {
                    continue;
                }

                var otherVertex0 = vertices[indices[otherIdx0]];
                var otherVertex1 = vertices[indices[otherIdx1]];

                // Calculate the texcoords for the new pole vertex, add it to the vertices and update the index
                var newPoleVertex = poleVertex;
                newPoleVertex.TexCoord.X = (otherVertex0.TexCoord.X + otherVertex1.TexCoord.X) * 0.5f;
                newPoleVertex.TexCoord.Y = poleVertex.TexCoord.Y;

                if (!overwrittenPoleVertex)
                {
                    vertices[poleIndex] = newPoleVertex;
                    overwrittenPoleVertex = true;
                }
                else
                {
                    CheckIndexOverflow((uint)vertices.Count);
                    indices[poleIdxPtr] = (ushort)vertices.Count.Value;
                    vertices.Add(newPoleVertex);
                }
            }
        }

        FixPole(northPoleIndex);
        FixPole(southPoleIndex);

        // Build RH above
        if (!rhcoords)
            ReverseWinding(indices.AsSpan(), vertices.AsSpan());

        if (invertn)
            InvertNormals(vertices.AsSpan());

        return (vertices, indices);
    }

    /// <summary>
    /// Cylinder.
    /// </summary>
    public static (NativeList<VertexPositionNormalTexture> Vertices, NativeList<ushort> Indices) ComputeCylinder(
        float height, float diameter, int tessellation, bool rhcoords, bool invertn = false)
    {
        if (tessellation < 3)
            throw new ArgumentException("Tessellation parameter must be at least 3.", nameof(tessellation));

        NativeList<VertexPositionNormalTexture> vertices = [];
        NativeList<ushort> indices = [];

        float radius = diameter * 0.5f;
        float halfHeight = height * 0.5f;

        int vertexCount = (tessellation * 4) + 6;
        int indexCount = tessellation * 12;

        CheckIndexOverflow((uint)vertexCount);

        vertices.EnsureCapacity(vertexCount);
        indices.EnsureCapacity(indexCount);

        // Create a ring of vertices around the outside of the cylinder.
        Vector3 topOffset = new(0, halfHeight, 0);

        for (int i = 0; i <= tessellation; ++i)
        {
            float angle = i * MathF.Tau / tessellation;
            (float sin, float cos) = MathF.SinCos(angle);

            Vector3 normal = new(cos, 0, sin);
            Vector3 sideOffset = normal * radius;
            float u = 1.0f - (i / (float)tessellation);

            vertices.Add(new(sideOffset + topOffset, normal, new Vector2(u, 0)));
            vertices.Add(new(sideOffset - topOffset, normal, new Vector2(u, 1)));
        }

        // Fill index buffer for side of cylinder
        for (int i = 0; i < tessellation; ++i)
        {
            int v0 = i * 2;
            int v1 = (i * 2) + 1;
            int v2 = (i + 1) * 2;
            int v3 = ((i + 1) * 2) + 1;

            IndexPushBack(indices, v0);
            IndexPushBack(indices, v2);
            IndexPushBack(indices, v1);

            IndexPushBack(indices, v1);
            IndexPushBack(indices, v2);
            IndexPushBack(indices, v3);
        }

        // Create cylinder caps
        void CreateCylinderCap(bool isTop)
        {
            // First, create an intermediate vertex at the center of the cap.
            Vector3 normal = isTop ? Vector3.UnitY : -Vector3.UnitY;
            Vector3 yOffset = normal * halfHeight;
            int centerIndex = vertices.Count;

            vertices.Add(new(yOffset, normal, new Vector2(0.5f, 0.5f)));

            // Create a ring of vertices around the outside of the cap.
            for (int i = 0; i <= tessellation; ++i)
            {
                float angle = i * MathF.Tau / tessellation;
                (float sin, float cos) = MathF.SinCos(angle);

                Vector3 pos = (new Vector3(cos, 0, sin) * radius) + yOffset;

                Vector2 texCoord = isTop
                    ? new Vector2(0.5f + (cos * 0.5f), 0.5f - (sin * 0.5f))
                    : new Vector2(0.5f + (cos * 0.5f), 0.5f + (sin * 0.5f));

                vertices.Add(new(pos, normal, texCoord));
            }

            for (int i = 0; i < tessellation; ++i)
            {
                if (isTop)
                {
                    IndexPushBack(indices, centerIndex);
                    IndexPushBack(indices, centerIndex + i + 2);
                    IndexPushBack(indices, centerIndex + i + 1);
                }
                else
                {
                    IndexPushBack(indices, centerIndex);
                    IndexPushBack(indices, centerIndex + i + 1);
                    IndexPushBack(indices, centerIndex + i + 2);
                }
            }
        }

        CreateCylinderCap(isTop: true);
        CreateCylinderCap(isTop: false);

        // Build RH above
        if (!rhcoords)
            ReverseWinding(indices.AsSpan(), vertices.AsSpan());

        if (invertn)
            InvertNormals(vertices.AsSpan());

        return (vertices, indices);
    }

    /// <summary>
    /// Cone.
    /// </summary>
    public static (NativeList<VertexPositionNormalTexture> Vertices, NativeList<ushort> Indices) ComputeCone(
        float diameter, float height, int tessellation, bool rhcoords, bool invertn = false)
    {
        if (tessellation < 3)
            throw new ArgumentException("Tessellation parameter must be at least 3.", nameof(tessellation));

        NativeList<VertexPositionNormalTexture> vertices = [];
        NativeList<ushort> indices = [];

        float radius = diameter * 0.5f;
        float halfHeight = height * 0.5f;

        int vertexCount = (tessellation * 3) + 3;
        int indexCount = tessellation * 6;

        CheckIndexOverflow((uint)vertexCount);

        vertices.EnsureCapacity(vertexCount);
        indices.EnsureCapacity(indexCount);

        // Create flat bottom cap (base disk)
        int centerIndex = vertices.Count;
        Vector3 bottomNormal = -Vector3.UnitY;
        Vector3 bottomCenter = new(0, -halfHeight, 0);

        vertices.Add(new(bottomCenter, bottomNormal, new Vector2(0.5f, 0.5f)));

        int baseRimStart = vertices.Count;
        for (int i = 0; i <= tessellation; ++i)
        {
            float angle = i * MathF.Tau / tessellation;
            (float sin, float cos) = MathF.SinCos(angle);

            Vector3 pos = new(cos * radius, -halfHeight, sin * radius);
            Vector2 texCoord = new(0.5f + (cos * 0.5f), 0.5f + (sin * 0.5f));

            vertices.Add(new(pos, bottomNormal, texCoord));
        }

        for (int i = 0; i < tessellation; ++i)
        {
            IndexPushBack(indices, centerIndex);
            IndexPushBack(indices, baseRimStart + i);
            IndexPushBack(indices, baseRimStart + i + 1);
        }

        // Create cone mantle (sloped side surface)
        float normalSlope = radius / height;

        int mantleRimStart = vertices.Count;
        for (int i = 0; i <= tessellation; ++i)
        {
            float angle = i * MathF.Tau / tessellation;
            (float sin, float cos) = MathF.SinCos(angle);

            Vector3 pos = new(cos * radius, -halfHeight, sin * radius);
            Vector3 normal = Vector3.Normalize(new Vector3(cos, normalSlope, sin));
            float u = 1.0f - (i / (float)tessellation);

            vertices.Add(new(pos, normal, new Vector2(u, 1.0f)));
        }

        Vector3 topApex = new(0, halfHeight, 0);
        for (int i = 0; i < tessellation; ++i)
        {
            float midAngle = (i + 0.5f) * MathF.Tau / tessellation;
            (float midSin, float midCos) = MathF.SinCos(midAngle);

            Vector3 apexNormal = Vector3.Normalize(new Vector3(midCos, normalSlope, midSin));
            float uMid = 1.0f - ((i + 0.5f) / tessellation);

            int apexIndex = vertices.Count;
            vertices.Add(new(topApex, apexNormal, new Vector2(uMid, 0.0f)));

            IndexPushBack(indices, apexIndex);
            IndexPushBack(indices, mantleRimStart + i + 1);
            IndexPushBack(indices, mantleRimStart + i);
        }

        // Build RH above
        if (!rhcoords)
            ReverseWinding(indices.AsSpan(), vertices.AsSpan());

        if (invertn)
            InvertNormals(vertices.AsSpan());

        return (vertices, indices);
    }

    /// <summary>
    /// Torus (doughnut shape).
    /// </summary>
    public static (NativeList<VertexPositionNormalTexture> Vertices, NativeList<ushort> Indices) ComputeTorus(
        float diameter, float thickness, int tessellation, bool rhcoords, bool invertn = false)
    {
        if (tessellation < 3)
            throw new ArgumentException("Tessellation parameter must be at least 3.", nameof(tessellation));

        NativeList<VertexPositionNormalTexture> vertices = [];
        NativeList<ushort> indices = [];

        int stride = tessellation + 1;
        int vertexCount = stride * stride;
        int indexCount = tessellation * tessellation * 6;

        CheckIndexOverflow((uint)vertexCount);

        vertices.EnsureCapacity(vertexCount);
        indices.EnsureCapacity(indexCount);

        // First we loop around the main ring of the torus.
        for (int i = 0; i <= tessellation; i++)
        {
            float u = (float)i / tessellation;

            float outerAngle = (i * MathF.Tau / tessellation) - TortureMath.HalfPI;

            // Create a transform matrix that will align geometry to slice of the torus.
            Matrix4x4 transform = Matrix4x4.CreateTranslation(diameter * 0.5f, 0, 0) * Matrix4x4.CreateRotationY(outerAngle);

            // Now we loop along the other axis, around the side of the tube.
            for (int j = 0; j <= tessellation; j++)
            {
                float v = 1.0f - ((float)j / tessellation);

                float innerAngle = (j * MathF.Tau / tessellation) + MathF.PI;
                (float dy, float dx) = MathF.SinCos(innerAngle);

                // Create a vertex.
                Vector3 normal = new(dx, dy, 0);
                Vector3 position = normal * (thickness * 0.5f);
                Vector2 textureCoordinate = new(u, v);

                position = Vector3.Transform(position, transform);
                normal = Vector3.Normalize(Vector3.TransformNormal(normal, transform));

                vertices.Add(new(position, normal, textureCoordinate));
            }
        }

        // Fill the index buffer with triangles joining each pair of rings.
        for (int i = 0; i < tessellation; i++)
        {
            for (int j = 0; j < tessellation; j++)
            {
                int nextI = i + 1;
                int nextJ = j + 1;

                IndexPushBack(indices, (i * stride) + j);
                IndexPushBack(indices, (i * stride) + nextJ);
                IndexPushBack(indices, (nextI * stride) + j);

                IndexPushBack(indices, (i * stride) + nextJ);
                IndexPushBack(indices, (nextI * stride) + nextJ);
                IndexPushBack(indices, (nextI * stride) + j);
            }
        }

        // Build RH above
        if (!rhcoords)
            ReverseWinding(indices.AsSpan(), vertices.AsSpan());

        if (invertn)
            InvertNormals(vertices.AsSpan());

        return (vertices, indices);
    }

    /// <summary>
    /// Tetrahedron (triangular pyramid).
    /// </summary>
    public static (NativeList<VertexPositionNormalTexture> Vertices, NativeList<ushort> Indices) ComputeTetrahedron(
        float size, bool rhcoords, bool invertn = false)
    {
        NativeList<VertexPositionNormalTexture> vertices = [];
        NativeList<ushort> indices = [];

        const int faceCount = 4;
        const int vertexCount = faceCount * 3;
        const int indexCount = faceCount * 3;

        CheckIndexOverflow(vertexCount);

        vertices.EnsureCapacity(vertexCount);
        indices.EnsureCapacity(indexCount);

        // A regular tetrahedron centered around origin
        float s = size * (0.5f / MathF.Sqrt(3.0f));

        ReadOnlySpan<Vector3> cornerPositions = stackalloc Vector3[]
        {
            new( s,  s,  s), // 0
            new(-s, -s,  s), // 1
            new(-s,  s, -s), // 2
            new( s, -s, -s), // 3
        };

        ReadOnlySpan<int> faceIndices = stackalloc int[]
        {
            0, 1, 2, // Face 0
            0, 2, 3, // Face 1
            0, 3, 1, // Face 2
            1, 3, 2  // Face 3
        };

        ReadOnlySpan<Vector2> uvs = stackalloc Vector2[]
        {
            new(0.0f, 0.0f),
            new(1.0f, 0.0f),
            new(0.5f, 1.0f)
        };

        for (int i = 0; i < faceCount; ++i)
        {
            Vector3 v0 = cornerPositions[faceIndices[(i * 3) + 0]];
            Vector3 v1 = cornerPositions[faceIndices[(i * 3) + 1]];
            Vector3 v2 = cornerPositions[faceIndices[(i * 3) + 2]];

            // Calculate face normal for faceted shading
            Vector3 normal = Vector3.Normalize(Vector3.Cross(v1 - v0, v2 - v0));

            int vbase = vertices.Count;
            IndexPushBack(indices, vbase + 0);
            IndexPushBack(indices, vbase + 1);
            IndexPushBack(indices, vbase + 2);

            vertices.Add(new(v0, normal, uvs[0]));
            vertices.Add(new(v1, normal, uvs[1]));
            vertices.Add(new(v2, normal, uvs[2]));
        }

        // Build RH above
        if (!rhcoords)
            ReverseWinding(indices.AsSpan(), vertices.AsSpan());

        if (invertn)
            InvertNormals(vertices.AsSpan());

        return (vertices, indices);
    }

    /// <summary>
    /// Octahedron.
    /// </summary>
    public static (NativeList<VertexPositionNormalTexture> Vertices, NativeList<ushort> Indices) ComputeOctahedron(
        float size, bool rhcoords, bool invertn = false)
    {
        NativeList<VertexPositionNormalTexture> vertices = [];
        NativeList<ushort> indices = [];

        const int faceCount = 8;
        const int vertexCount = faceCount * 3;
        const int indexCount = faceCount * 3;

        CheckIndexOverflow(vertexCount);

        vertices.EnsureCapacity(vertexCount);
        indices.EnsureCapacity(indexCount);

        float radius = size * 0.5f;

        ReadOnlySpan<Vector2> uvs = stackalloc Vector2[]
        {
            new(0.0f, 0.0f),
            new(1.0f, 0.0f),
            new(0.5f, 1.0f)
        };

        for (int i = 0; i < faceCount; ++i)
        {
            Vector3 v0 = OctahedronVertices[OctahedronIndices[(i * 3) + 0]] * radius;
            Vector3 v1 = OctahedronVertices[OctahedronIndices[(i * 3) + 1]] * radius;
            Vector3 v2 = OctahedronVertices[OctahedronIndices[(i * 3) + 2]] * radius;

            // Calculate face normal for faceted shading
            Vector3 normal = Vector3.Normalize(Vector3.Cross(v1 - v0, v2 - v0));

            int vbase = vertices.Count;
            IndexPushBack(indices, vbase + 0);
            IndexPushBack(indices, vbase + 1);
            IndexPushBack(indices, vbase + 2);

            vertices.Add(new(v0, normal, uvs[0]));
            vertices.Add(new(v1, normal, uvs[1]));
            vertices.Add(new(v2, normal, uvs[2]));
        }

        // Build RH above
        if (!rhcoords)
            ReverseWinding(indices.AsSpan(), vertices.AsSpan());

        if (invertn)
            InvertNormals(vertices.AsSpan());

        return (vertices, indices);
    }

    /// <summary>
    /// Dodecahedron (12 pentagonal faces).
    /// </summary>
    public static (NativeList<VertexPositionNormalTexture> Vertices, NativeList<ushort> Indices) ComputeDodecahedron(
        float size, bool rhcoords, bool invertn = false)
    {
        NativeList<VertexPositionNormalTexture> vertices = [];
        NativeList<ushort> indices = [];

        const int faceCount = 12;
        const int vertexCount = faceCount * 5; // 5 vertices per pentagonal face
        const int indexCount = faceCount * 3 * 3; // 3 triangles per pentagon

        CheckIndexOverflow(vertexCount);

        vertices.EnsureCapacity(vertexCount);
        indices.EnsureCapacity(indexCount);

        float phi = (1.0f + MathF.Sqrt(5.0f)) * 0.5f; // Golden ratio (~1.618034)
        float invPhi = 1.0f / phi;                     // ~0.618034
        float scale = size * (0.5f / MathF.Sqrt(3.0f));

        // 20 unique corner positions of a regular dodecahedron
        ReadOnlySpan<Vector3> corners = stackalloc Vector3[]
        {
            // 8 cube corners (+-1, +-1, +-1)
            new Vector3(-1, -1, -1) * scale, // 0
            new Vector3(-1, -1,  1) * scale, // 1
            new Vector3(-1,  1, -1) * scale, // 2
            new Vector3(-1,  1,  1) * scale, // 3
            new Vector3( 1, -1, -1) * scale, // 4
            new Vector3( 1, -1,  1) * scale, // 5
            new Vector3( 1,  1, -1) * scale, // 6
            new Vector3( 1,  1,  1) * scale, // 7

            // 12 golden ratio rectangle corners
            new Vector3(0, -phi, -invPhi) * scale, // 8
            new Vector3(0, -phi,  invPhi) * scale, // 9
            new Vector3(0,  phi, -invPhi) * scale, // 10
            new Vector3(0,  phi,  invPhi) * scale, // 11

            new Vector3(-invPhi, 0, -phi) * scale, // 12
            new Vector3(-invPhi, 0,  phi) * scale, // 13
            new Vector3( invPhi, 0, -phi) * scale, // 14
            new Vector3( invPhi, 0,  phi) * scale, // 15

            new Vector3(-phi, -invPhi, 0) * scale, // 16
            new Vector3(-phi,  invPhi, 0) * scale, // 17
            new Vector3( phi, -invPhi, 0) * scale, // 18
            new Vector3( phi,  invPhi, 0) * scale  // 19
        };

        // 12 pentagon faces (5 vertex indices per face)
        ReadOnlySpan<int> pentagonFaces = stackalloc int[]
        {
            3, 11, 7, 15, 13,
            7, 19, 6, 14, 15,
            6, 10, 2, 12, 14,
            2, 17, 3, 13, 12,
            13, 15, 5, 9, 1,
            5, 18, 4, 8, 9,
            4, 14, 12, 0, 8,
            0, 16, 1, 9, 8,
            3, 17, 16, 1, 11,
            7, 11, 10, 6, 19,
            4, 18, 19, 6, 14,
            0, 12, 2, 10, 16
        };

        // Planar UV layout for a regular pentagon
        ReadOnlySpan<Vector2> pentagonUVs = stackalloc Vector2[]
        {
            new(0.5f + (0.5f * MathF.Cos(0 * MathF.Tau / 5.0f - TortureMath.HalfPI)), 0.5f + (0.5f * MathF.Sin(0 * MathF.Tau / 5.0f - TortureMath.HalfPI))),
            new(0.5f + (0.5f * MathF.Cos(1 * MathF.Tau / 5.0f - TortureMath.HalfPI)), 0.5f + (0.5f * MathF.Sin(1 * MathF.Tau / 5.0f - TortureMath.HalfPI))),
            new(0.5f + (0.5f * MathF.Cos(2 * MathF.Tau / 5.0f - TortureMath.HalfPI)), 0.5f + (0.5f * MathF.Sin(2 * MathF.Tau / 5.0f - TortureMath.HalfPI))),
            new(0.5f + (0.5f * MathF.Cos(3 * MathF.Tau / 5.0f - TortureMath.HalfPI)), 0.5f + (0.5f * MathF.Sin(3 * MathF.Tau / 5.0f - TortureMath.HalfPI))),
            new(0.5f + (0.5f * MathF.Cos(4 * MathF.Tau / 5.0f - TortureMath.HalfPI)), 0.5f + (0.5f * MathF.Sin(4 * MathF.Tau / 5.0f - TortureMath.HalfPI)))
        };

        for (int i = 0; i < faceCount; ++i)
        {
            int baseFaceIdx = i * 5;
            Vector3 v0 = corners[pentagonFaces[baseFaceIdx + 0]];
            Vector3 v1 = corners[pentagonFaces[baseFaceIdx + 1]];
            Vector3 v2 = corners[pentagonFaces[baseFaceIdx + 2]];

            // Calculate face normal
            Vector3 normal = Vector3.Normalize(Vector3.Cross(v1 - v0, v2 - v0));

            int vbase = vertices.Count;

            // 5 vertices per pentagon face
            for (int j = 0; j < 5; ++j)
            {
                vertices.Add(new(corners[pentagonFaces[baseFaceIdx + j]], normal, pentagonUVs[j]));
            }

            // 3 triangles per pentagon: (0, 1, 2), (0, 2, 3), (0, 3, 4)
            IndexPushBack(indices, vbase + 0);
            IndexPushBack(indices, vbase + 1);
            IndexPushBack(indices, vbase + 2);

            IndexPushBack(indices, vbase + 0);
            IndexPushBack(indices, vbase + 2);
            IndexPushBack(indices, vbase + 3);

            IndexPushBack(indices, vbase + 0);
            IndexPushBack(indices, vbase + 3);
            IndexPushBack(indices, vbase + 4);
        }

        // Build RH above
        if (!rhcoords)
            ReverseWinding(indices.AsSpan(), vertices.AsSpan());

        if (invertn)
            InvertNormals(vertices.AsSpan());

        return (vertices, indices);
    }

    /// <summary>
    /// Icosahedron (20 triangular faces).
    /// </summary>
    public static (NativeList<VertexPositionNormalTexture> Vertices, NativeList<ushort> Indices) ComputeIcosahedron(
        float size, bool rhcoords, bool invertn = false)
    {
        NativeList<VertexPositionNormalTexture> vertices = [];
        NativeList<ushort> indices = [];

        const int faceCount = 20;
        const int vertexCount = faceCount * 3;
        const int indexCount = faceCount * 3;

        CheckIndexOverflow(vertexCount);

        vertices.EnsureCapacity(vertexCount);
        indices.EnsureCapacity(indexCount);

        float phi = (1.0f + MathF.Sqrt(5.0f)) * 0.5f; // Golden ratio (~1.618034)
        float scale = size * (0.5f / MathF.Sqrt(1.0f + (phi * phi)));

        // 12 golden ratio rectangle vertices
        ReadOnlySpan<Vector3> corners = stackalloc Vector3[]
        {
            new Vector3(-1,  phi, 0) * scale, // 0
            new Vector3( 1,  phi, 0) * scale, // 1
            new Vector3(-1, -phi, 0) * scale, // 2
            new Vector3( 1, -phi, 0) * scale, // 3

            new Vector3(0, -1,  phi) * scale, // 4
            new Vector3(0,  1,  phi) * scale, // 5
            new Vector3(0, -1, -phi) * scale, // 6
            new Vector3(0,  1, -phi) * scale, // 7

            new Vector3( phi, 0, -1) * scale, // 8
            new Vector3( phi, 0,  1) * scale, // 9
            new Vector3(-phi, 0, -1) * scale, // 10
            new Vector3(-phi, 0,  1) * scale  // 11
        };

        // 20 triangular faces
        ReadOnlySpan<int> faceIndices = stackalloc int[]
        {
            // 5 faces around vertex 0
            0, 11, 5,
            0, 5, 1,
            0, 1, 7,
            0, 7, 10,
            0, 10, 11,

            // 5 adjacent faces
            1, 5, 9,
            5, 11, 4,
            11, 10, 2,
            10, 7, 6,
            7, 1, 8,

            // 5 faces around vertex 3
            3, 9, 4,
            3, 4, 2,
            3, 2, 6,
            3, 6, 8,
            3, 8, 9,

            // 5 adjacent faces
            4, 9, 5,
            2, 4, 11,
            6, 2, 10,
            8, 6, 7,
            9, 8, 1
        };

        ReadOnlySpan<Vector2> uvs = stackalloc Vector2[]
        {
            new(0.0f, 0.0f),
            new(1.0f, 0.0f),
            new(0.5f, 1.0f)
        };

        for (int i = 0; i < faceCount; ++i)
        {
            Vector3 v0 = corners[faceIndices[(i * 3) + 0]];
            Vector3 v1 = corners[faceIndices[(i * 3) + 1]];
            Vector3 v2 = corners[faceIndices[(i * 3) + 2]];

            // Calculate face normal for faceted shading
            Vector3 normal = Vector3.Normalize(Vector3.Cross(v1 - v0, v2 - v0));

            int vbase = vertices.Count;
            IndexPushBack(indices, vbase + 0);
            IndexPushBack(indices, vbase + 1);
            IndexPushBack(indices, vbase + 2);

            vertices.Add(new(v0, normal, uvs[0]));
            vertices.Add(new(v1, normal, uvs[1]));
            vertices.Add(new(v2, normal, uvs[2]));
        }

        // Build RH above
        if (!rhcoords)
            ReverseWinding(indices.AsSpan(), vertices.AsSpan());

        if (invertn)
            InvertNormals(vertices.AsSpan());

        return (vertices, indices);
    }

    /// <summary>
    /// Computes the Utah Teapot (Martin Newell, 1975).
    /// </summary>
    public static (NativeList<VertexPositionNormalTexture> Vertices, NativeList<ushort> Indices) ComputeTeapot(
        float size, int tessellation, bool rhcoords, bool invertn)
    {
        if (tessellation < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(tessellation), "Tessellation parameter must be at least 1.");
        }

        int stride = tessellation + 1;
        int verticesPerPatch = stride * stride;
        int totalVertices = TeapotData.PatchCount * verticesPerPatch;

        CheckIndexOverflow((uint)totalVertices);

        int quadsPerPatch = tessellation * tessellation;
        int totalIndices = TeapotData.PatchCount * quadsPerPatch * 6;

        NativeList<VertexPositionNormalTexture> vertices = [];
        vertices.EnsureCapacity(totalVertices);

        NativeList<ushort> indices = [];
        indices.EnsureCapacity(totalIndices);

        float scale = size * 0.5f;

        for (int patchIndex = 0; patchIndex < TeapotData.PatchCount; ++patchIndex)
        {
            ReadOnlySpan<Vector3> patch = TeapotData.GetPatch(patchIndex);
            ushort patchBaseIndex = (ushort)vertices.Count.Value;

            // Generate vertices for this patch
            for (int vIndex = 0; vIndex <= tessellation; ++vIndex)
            {
                float v = (float)vIndex / tessellation;

                for (int uIndex = 0; uIndex <= tessellation; ++uIndex)
                {
                    float u = (float)uIndex / tessellation;

                    var (position, normal) = Bezier.BezierPatchEvaluate(patch, u, v);

                    position *= scale;
                    Vector2 texCoord = new(u, v);

                    vertices.Add(new(position, normal, texCoord));
                }
            }

            // Generate indices for this patch
            for (int vIndex = 0; vIndex < tessellation; ++vIndex)
            {
                for (int uIndex = 0; uIndex < tessellation; ++uIndex)
                {
                    ushort i0 = (ushort)(patchBaseIndex + (vIndex * stride) + uIndex);
                    ushort i1 = (ushort)(patchBaseIndex + (vIndex * stride) + (uIndex + 1));
                    ushort i2 = (ushort)(patchBaseIndex + ((vIndex + 1) * stride) + uIndex);
                    ushort i3 = (ushort)(patchBaseIndex + ((vIndex + 1) * stride) + (uIndex + 1));

                    IndexPushBack(indices, i0);
                    IndexPushBack(indices, i1);
                    IndexPushBack(indices, i2);

                    IndexPushBack(indices, i2);
                    IndexPushBack(indices, i1);
                    IndexPushBack(indices, i3);
                }
            }
        }

        // Build RH above
        if (!rhcoords)
            ReverseWinding(indices.AsSpan(), vertices.AsSpan());

        if (invertn)
            InvertNormals(vertices.AsSpan());

        return (vertices, indices);
    }

    // Helper for flipping winding of geometric primitives for LH vs. RH coords
    private static void ReverseWinding(Span<ushort> indices, Span<VertexPositionNormalTexture> vertices)
    {
        if (indices.Length % 3 != 0)
        {
            throw new InvalidOperationException("Indices count must be a multiple of 3 to reverse triangle winding.");
        }

        for (int i = 0; i < indices.Length; i += 3)
        {
            (indices[i], indices[i + 2]) = (indices[i + 2], indices[i]);
        }

        foreach (ref var vertex in vertices)
        {
            vertex.TexCoord.X = 1.0f - vertex.TexCoord.X;
        }
    }

    // Helper for inverting normals of geometric primitives for 'inside' vs. 'outside' viewing
    private static void InvertNormals(Span<VertexPositionNormalTexture> vertices)
    {
        foreach (ref var vertex in vertices)
        {
            vertex.Normal = -vertex.Normal;
        }
    }

    // Use >=, not > comparison, because some D3D level 9_x hardware does not support 0xFFFF index values.
    private static void CheckIndexOverflow(uint value)
    {
        if (value >= ushort.MaxValue)
        {
            throw new OverflowException("Index value out of range: cannot tessellate primitive so finely");
        }
    }

    private static void CheckIndexOverflow(int value)
    {
        if ((uint)value >= ushort.MaxValue)
        {
            throw new OverflowException("Index value out of range: cannot tessellate primitive so finely");
        }
    }

    // Collection types used when generating the geometry.
    private static void IndexPushBack(NativeList<ushort> indices, uint value)
    {
        CheckIndexOverflow(value);
        indices.Add((ushort)value);
    }

    private static void IndexPushBack(NativeList<ushort> indices, int value)
    {
        CheckIndexOverflow(value);
        indices.Add((ushort)value);
    }
}
