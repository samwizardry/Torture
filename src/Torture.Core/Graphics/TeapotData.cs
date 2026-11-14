using System;
using System.Numerics;

namespace Torture.Core.Graphics;

/// <summary>
/// Bicubic Bézier patch data for the Utah Teapot (Martin Newell, 1975).
/// Evaluates and expands the 32 bicubic patches (16 control points each) in DirectX coordinates (Y-up, centered at origin).
/// </summary>
internal static class TeapotData
{
    public const int PatchCount = 32;
    public const int ControlPointsPerPatch = 16;

    /// <summary>
    /// All 32 bicubic Bézier patches (32 patches * 16 control points = 512 Vector3 control points).
    /// </summary>
    private static readonly Vector3[] Patches = new Vector3[PatchCount * ControlPointsPerPatch];

    // Canonical Newell/GLUT 10 patch prototypes (16 indices each).
    // Patches 0..5 (rim, body, lid, bottom) reflect across both X and Z (4 patches each).
    // Patches 6..9 (handle, spout) reflect across Z only (2 patches each).
    private static readonly ushort[][] PatchPrototypes =
    [
        // Rim
        [102, 103, 104, 105, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15],
        // Body upper
        [12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27],
        // Body lower
        [24, 25, 26, 27, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40],
        // Lid upper
        [96, 96, 96, 96, 97, 98, 99, 100, 101, 101, 101, 101, 0, 1, 2, 3],
        // Lid lower
        [0, 1, 2, 3, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117],
        // Bottom
        [118, 118, 118, 118, 124, 122, 119, 121, 123, 126, 125, 120, 40, 39, 38, 37],
        // Handle upper
        [41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56],
        // Handle lower
        [53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 28, 65, 66, 67],
        // Spout upper
        [68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83],
        // Spout lower
        [80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95]
    ];

    // Canonical 127 control points in GLUT coordinates (Z is up, height 0 to 3.15).
    private static readonly Vector3[] RawControlPoints =
    [
        new(0.2f, 0.0f, 2.7f),
        new(0.2f, -0.112f, 2.7f),
        new(0.112f, -0.2f, 2.7f),
        new(0.0f, -0.2f, 2.7f),
        new(1.3375f, 0.0f, 2.53125f),
        new(1.3375f, -0.749f, 2.53125f),
        new(0.749f, -1.3375f, 2.53125f),
        new(0.0f, -1.3375f, 2.53125f),
        new(1.4375f, 0.0f, 2.53125f),
        new(1.4375f, -0.805f, 2.53125f),
        new(0.805f, -1.4375f, 2.53125f),
        new(0.0f, -1.4375f, 2.53125f),
        new(1.5f, 0.0f, 2.4f),
        new(1.5f, -0.84f, 2.4f),
        new(0.84f, -1.5f, 2.4f),
        new(0.0f, -1.5f, 2.4f),
        new(1.75f, 0.0f, 1.875f),
        new(1.75f, -0.98f, 1.875f),
        new(0.98f, -1.75f, 1.875f),
        new(0.0f, -1.75f, 1.875f),
        new(2.0f, 0.0f, 1.35f),
        new(2.0f, -1.12f, 1.35f),
        new(1.12f, -2.0f, 1.35f),
        new(0.0f, -2.0f, 1.35f),
        new(2.0f, 0.0f, 0.9f),
        new(2.0f, -1.12f, 0.9f),
        new(1.12f, -2.0f, 0.9f),
        new(0.0f, -2.0f, 0.9f),
        new(-2.0f, 0.0f, 0.9f),
        new(2.0f, 0.0f, 0.45f),
        new(2.0f, -1.12f, 0.45f),
        new(1.12f, -2.0f, 0.45f),
        new(0.0f, -2.0f, 0.45f),
        new(1.5f, 0.0f, 0.225f),
        new(1.5f, -0.84f, 0.225f),
        new(0.84f, -1.5f, 0.225f),
        new(0.0f, -1.5f, 0.225f),
        new(1.5f, 0.0f, 0.15f),
        new(1.5f, -0.84f, 0.15f),
        new(0.84f, -1.5f, 0.15f),
        new(0.0f, -1.5f, 0.15f),
        new(-1.6f, 0.0f, 2.025f),
        new(-1.6f, -0.3f, 2.025f),
        new(-1.5f, -0.3f, 2.25f),
        new(-1.5f, 0.0f, 2.25f),
        new(-2.3f, 0.0f, 2.025f),
        new(-2.3f, -0.3f, 2.025f),
        new(-2.5f, -0.3f, 2.25f),
        new(-2.5f, 0.0f, 2.25f),
        new(-2.7f, 0.0f, 1.8f),
        new(-2.7f, -0.3f, 1.8f),
        new(-3.0f, -0.3f, 1.8f),
        new(-3.0f, 0.0f, 1.8f),
        new(-2.7f, 0.0f, 1.575f),
        new(-2.7f, -0.3f, 1.575f),
        new(-3.0f, -0.3f, 1.35f),
        new(-3.0f, 0.0f, 1.35f),
        new(-2.5f, 0.0f, 1.125f),
        new(-2.5f, -0.3f, 1.125f),
        new(-2.65f, -0.3f, 0.9375f),
        new(-2.65f, 0.0f, 0.9375f),
        new(-2.0f, 0.0f, 0.7875f),
        new(-2.0f, -0.3f, 0.7875f),
        new(-1.9f, -0.3f, 0.6f),
        new(-1.9f, 0.0f, 0.6f),
        new(-1.2f, 0.0f, 0.6f),
        new(-1.2f, -0.3f, 0.6f),
        new(-1.3f, -0.3f, 0.6f),
        new(1.7f, 0.0f, 1.425f),
        new(1.7f, -0.66f, 1.425f),
        new(1.7f, -0.66f, 0.6f),
        new(1.7f, 0.0f, 0.6f),
        new(2.6f, 0.0f, 1.2f),
        new(2.6f, -0.66f, 1.2f),
        new(3.1f, -0.66f, 0.825f),
        new(3.1f, 0.0f, 0.825f),
        new(2.3f, 0.0f, 2.1f),
        new(2.3f, -0.25f, 2.1f),
        new(2.4f, -0.25f, 2.025f),
        new(2.4f, 0.0f, 2.025f),
        new(2.7f, 0.0f, 2.4f),
        new(2.7f, -0.25f, 2.4f),
        new(3.3f, -0.25f, 2.4f),
        new(3.3f, 0.0f, 2.4f),
        new(2.8f, 0.0f, 2.475f),
        new(2.8f, -0.25f, 2.475f),
        new(3.525f, -0.25f, 2.49375f),
        new(3.525f, 0.0f, 2.49375f),
        new(2.9f, 0.0f, 2.475f),
        new(2.9f, -0.15f, 2.475f),
        new(3.45f, -0.15f, 2.5125f),
        new(3.45f, 0.0f, 2.5125f),
        new(2.8f, 0.0f, 2.4f),
        new(2.8f, -0.15f, 2.4f),
        new(3.2f, -0.15f, 2.4f),
        new(3.2f, 0.0f, 2.4f),
        new(0.0f, 0.0f, 3.15f),
        new(0.8f, 0.0f, 3.15f),
        new(0.8f, -0.45f, 3.15f),
        new(0.45f, -0.8f, 3.15f),
        new(0.0f, -0.8f, 3.15f),
        new(0.0f, 0.0f, 2.85f),
        new(1.4f, 0.0f, 2.4f),
        new(1.4f, -0.784f, 2.4f),
        new(0.784f, -1.4f, 2.4f),
        new(0.0f, -1.4f, 2.4f),
        new(0.4f, 0.0f, 2.55f),
        new(0.4f, -0.224f, 2.55f),
        new(0.224f, -0.4f, 2.55f),
        new(0.0f, -0.4f, 2.55f),
        new(1.3f, 0.0f, 2.55f),
        new(1.3f, -0.728f, 2.55f),
        new(0.728f, -1.3f, 2.55f),
        new(0.0f, -1.3f, 2.55f),
        new(1.3f, 0.0f, 2.4f),
        new(1.3f, -0.728f, 2.4f),
        new(0.728f, -1.3f, 2.4f),
        new(0.0f, -1.3f, 2.4f),
        new(0.0f, 0.0f, 0.0f),
        new(1.425f, -0.798f, 0.0f),
        new(1.5f, 0.0f, 0.075f),
        new(1.425f, 0.0f, 0.0f),
        new(0.798f, -1.425f, 0.0f),
        new(0.0f, -1.5f, 0.075f),
        new(0.0f, -1.425f, 0.0f),
        new(1.5f, -0.84f, 0.075f),
        new(0.84f, -1.5f, 0.075f)
    ];

    static TeapotData()
    {
        // Convert canonical GLUT control points to DirectX space (Y-up, centered vertically).
        // DX: X = GLUT.X, Y = GLUT.Z - 1.5f, Z = -GLUT.Y
        Span<Vector3> dxCp = stackalloc Vector3[RawControlPoints.Length];
        for (int i = 0; i < RawControlPoints.Length; ++i)
        {
            Vector3 raw = RawControlPoints[i];
            dxCp[i] = new Vector3(raw.X, raw.Z - 1.5f, -raw.Y);
        }

        int patchOffset = 0;

        for (int i = 0; i < 10; ++i)
        {
            ReadOnlySpan<ushort> patchIndices = PatchPrototypes[i];

            // Build patch p (unmodified)
            Span<Vector3> patchP = Patches.AsSpan(patchOffset, ControlPointsPerPatch);
            for (int j = 0; j < 4; ++j)
            {
                for (int k = 0; k < 4; ++k)
                {
                    patchP[(j * 4) + k] = dxCp[patchIndices[(j * 4) + k]];
                }
            }
            patchOffset += ControlPointsPerPatch;

            // Build patch q (reflect across Z => negate Z, flip columns (3 - k) to preserve outward normal)
            Span<Vector3> patchQ = Patches.AsSpan(patchOffset, ControlPointsPerPatch);
            for (int j = 0; j < 4; ++j)
            {
                for (int k = 0; k < 4; ++k)
                {
                    Vector3 cp = dxCp[patchIndices[(j * 4) + (3 - k)]];
                    patchQ[(j * 4) + k] = new Vector3(cp.X, cp.Y, -cp.Z);
                }
            }
            patchOffset += ControlPointsPerPatch;

            if (i < 6)
            {
                // Build patch r (reflect across X => negate X, flip columns (3 - k) to preserve outward normal)
                Span<Vector3> patchR = Patches.AsSpan(patchOffset, ControlPointsPerPatch);
                for (int j = 0; j < 4; ++j)
                {
                    for (int k = 0; k < 4; ++k)
                    {
                        Vector3 cp = dxCp[patchIndices[(j * 4) + (3 - k)]];
                        patchR[(j * 4) + k] = new Vector3(-cp.X, cp.Y, cp.Z);
                    }
                }
                patchOffset += ControlPointsPerPatch;

                // Build patch s (reflect across X and Z => negate X and Z, no column flip needed)
                Span<Vector3> patchS = Patches.AsSpan(patchOffset, ControlPointsPerPatch);
                for (int j = 0; j < 4; ++j)
                {
                    for (int k = 0; k < 4; ++k)
                    {
                        Vector3 cp = dxCp[patchIndices[(j * 4) + k]];
                        patchS[(j * 4) + k] = new Vector3(-cp.X, cp.Y, -cp.Z);
                    }
                }
                patchOffset += ControlPointsPerPatch;
            }
        }
    }

    /// <summary>
    /// Gets the 16 control points for the specified Bézier patch index (0 to 31).
    /// </summary>
    public static ReadOnlySpan<Vector3> GetPatch(int patchIndex)
    {
        return Patches.AsSpan(patchIndex * ControlPointsPerPatch, ControlPointsPerPatch);
    }
}