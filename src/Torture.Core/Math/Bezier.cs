using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Torture.Core.Math;

/// <summary>
/// Helper routines for evaluating cubic Bézier curves and bicubic Bézier patches.
/// Matches Microsoft DirectX Tool Kit (DirectXTK) Bezier.h.
/// </summary>
public static class Bezier
{
    /// <summary>
    /// Performs a cubic bezier interpolation between four scalar values.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float CubicBezier(float p0, float p1, float p2, float p3, float t)
    {
        float invT = 1.0f - t;
        return (p0 * invT * invT * invT) +
               (p1 * 3.0f * t * invT * invT) +
               (p2 * 3.0f * t * t * invT) +
               (p3 * t * t * t);
    }

    /// <summary>
    /// Performs a cubic bezier interpolation between four vector values.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 CubicBezier(in Vector3 p0, in Vector3 p1, in Vector3 p2, in Vector3 p3, float t)
    {
        float invT = 1.0f - t;
        float b0 = invT * invT * invT;
        float b1 = 3.0f * t * invT * invT;
        float b2 = 3.0f * t * t * invT;
        float b3 = t * t * t;

        return (p0 * b0) + (p1 * b1) + (p2 * b2) + (p3 * b3);
    }

    /// <summary>
    /// Performs a cubic bezier tangent interpolation between four scalar values.
    /// Note: The constant multiplier of 3 is factored out because tangents are normalized during normal evaluation.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float CubicBezierTangent(float p0, float p1, float p2, float p3, float t)
    {
        float t2 = t * t;
        return (p0 * (-1.0f + (2.0f * t) - t2)) +
               (p1 * (1.0f - (4.0f * t) + (3.0f * t2))) +
               (p2 * ((2.0f * t) - (3.0f * t2))) +
               (p3 * t2);
    }

    /// <summary>
    /// Performs a cubic bezier tangent interpolation between four vector values.
    /// Note: The constant multiplier of 3 is factored out because tangents are normalized during normal evaluation.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 CubicBezierTangent(in Vector3 p0, in Vector3 p1, in Vector3 p2, in Vector3 p3, float t)
    {
        float t2 = t * t;
        float b0 = -1.0f + (2.0f * t) - t2;
        float b1 = 1.0f - (4.0f * t) + (3.0f * t2);
        float b2 = (2.0f * t) - (3.0f * t2);
        float b3 = t2;

        return (p0 * b0) + (p1 * b1) + (p2 * b2) + (p3 * b3);
    }

    /// <summary>
    /// Evaluates a position on a bezier patch.
    /// </summary>
    public static Vector3 BezierPatch(ReadOnlySpan<Vector3> patch, float u, float v)
    {
        if (patch.Length < 16)
        {
            throw new ArgumentException("Bézier patch must have at least 16 control points.", nameof(patch));
        }

        // Evaluate four horizontal bezier curves.
        Vector3 p0 = CubicBezier(patch[0], patch[1], patch[2], patch[3], u);
        Vector3 p1 = CubicBezier(patch[4], patch[5], patch[6], patch[7], u);
        Vector3 p2 = CubicBezier(patch[8], patch[9], patch[10], patch[11], u);
        Vector3 p3 = CubicBezier(patch[12], patch[13], patch[14], patch[15], u);

        // Evaluate the vertical curve connecting the results.
        return CubicBezier(p0, p1, p2, p3, v);
    }

    /// <summary>
    /// Evaluates a tangent on a bezier patch along the U direction.
    /// </summary>
    public static Vector3 BezierPatchTangentU(ReadOnlySpan<Vector3> patch, float u, float v)
    {
        if (patch.Length < 16)
        {
            throw new ArgumentException("Bézier patch must have at least 16 control points.", nameof(patch));
        }

        // Evaluate four horizontal bezier curve tangents.
        Vector3 p0 = CubicBezierTangent(patch[0], patch[1], patch[2], patch[3], u);
        Vector3 p1 = CubicBezierTangent(patch[4], patch[5], patch[6], patch[7], u);
        Vector3 p2 = CubicBezierTangent(patch[8], patch[9], patch[10], patch[11], u);
        Vector3 p3 = CubicBezierTangent(patch[12], patch[13], patch[14], patch[15], u);

        // Evaluate the vertical curve connecting the results.
        return CubicBezier(p0, p1, p2, p3, v);
    }

    /// <summary>
    /// Evaluates a tangent on a bezier patch along the V direction.
    /// </summary>
    public static Vector3 BezierPatchTangentV(ReadOnlySpan<Vector3> patch, float u, float v)
    {
        if (patch.Length < 16)
        {
            throw new ArgumentException("Bézier patch must have at least 16 control points.", nameof(patch));
        }

        // Evaluate four horizontal bezier curves.
        Vector3 p0 = CubicBezier(patch[0], patch[1], patch[2], patch[3], u);
        Vector3 p1 = CubicBezier(patch[4], patch[5], patch[6], patch[7], u);
        Vector3 p2 = CubicBezier(patch[8], patch[9], patch[10], patch[11], u);
        Vector3 p3 = CubicBezier(patch[12], patch[13], patch[14], patch[15], u);

        // Evaluate the vertical curve tangent connecting the results.
        return CubicBezierTangent(p0, p1, p2, p3, v);
    }

    /// <summary>
    /// Evaluates a normal on a bezier patch.
    /// </summary>
    public static Vector3 BezierPatchNormal(ReadOnlySpan<Vector3> patch, float u, float v)
    {
        Vector3 tangentU = BezierPatchTangentU(patch, u, v);
        Vector3 tangentV = BezierPatchTangentV(patch, u, v);

        Vector3 normal = Vector3.Cross(tangentU, tangentV);
        float lenSq = normal.LengthSquared();

        if (lenSq > TortureMath.Epsilon)
        {
            return Vector3.Normalize(normal);
        }

        // Fallback for singular points (e.g. poles or degenerate points where tangents vanish)
        return Vector3.UnitY;
    }

    /// <summary>
    /// Simultaneously evaluates position and surface normal on a bezier patch at (u, v).
    /// Reuses horizontal curve calculations to maximize performance during tessellation.
    /// </summary>
    public static (Vector3 Position, Vector3 Normal) BezierPatchEvaluate(ReadOnlySpan<Vector3> patch, float u, float v)
    {
        if (patch.Length < 16)
        {
            throw new ArgumentException("Bézier patch must have at least 16 control points.", nameof(patch));
        }

        // Evaluate four horizontal curves for position & TangentV
        Vector3 p0 = CubicBezier(patch[0], patch[1], patch[2], patch[3], u);
        Vector3 p1 = CubicBezier(patch[4], patch[5], patch[6], patch[7], u);
        Vector3 p2 = CubicBezier(patch[8], patch[9], patch[10], patch[11], u);
        Vector3 p3 = CubicBezier(patch[12], patch[13], patch[14], patch[15], u);

        Vector3 position = CubicBezier(p0, p1, p2, p3, v);
        Vector3 tangentV = CubicBezierTangent(p0, p1, p2, p3, v);

        // Evaluate four horizontal tangents for TangentU
        Vector3 tu0 = CubicBezierTangent(patch[0], patch[1], patch[2], patch[3], u);
        Vector3 tu1 = CubicBezierTangent(patch[4], patch[5], patch[6], patch[7], u);
        Vector3 tu2 = CubicBezierTangent(patch[8], patch[9], patch[10], patch[11], u);
        Vector3 tu3 = CubicBezierTangent(patch[12], patch[13], patch[14], patch[15], u);

        Vector3 tangentU = CubicBezier(tu0, tu1, tu2, tu3, v);

        Vector3 normal = Vector3.Cross(tangentU, tangentV);
        float lenSq = normal.LengthSquared();

        normal = lenSq > TortureMath.Epsilon ? Vector3.Normalize(normal) : Vector3.UnitY;

        return (position, normal);
    }
}