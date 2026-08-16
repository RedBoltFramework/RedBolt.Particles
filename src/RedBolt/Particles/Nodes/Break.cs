// Copyright (c) 2026 RedBolt and contributors.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT> or the accompanying LICENSE file.

namespace RedBolt.Particles.ParticleGraphNodes;

using RedBolt.Core;
using RedBolt.Math;

[Reflected]
public partial class Break : PatternMatchingNode
{
    private static readonly NodePattern[] BreakPatterns =
    [
        Helpers.MakePattern<Vector3f, float, float, float>(Update,
            new ParticleGraphTypedPin<Vector3f>("vec"),
            new ParticleGraphTypedPin<float>(ParticleGraphPinFlag.Output, "x"),
            new ParticleGraphTypedPin<float>(ParticleGraphPinFlag.Output, "y"),
            new ParticleGraphTypedPin<float>(ParticleGraphPinFlag.Output, "z")),
        Helpers.MakePattern<Vector2f, float, float>(Update,
            new ParticleGraphTypedPin<Vector2f>("vec"),
            new ParticleGraphTypedPin<float>(ParticleGraphPinFlag.Output, "x"),
            new ParticleGraphTypedPin<float>(ParticleGraphPinFlag.Output, "y")),
        Helpers.MakePattern<Quaternion, float, float, float, float>(Update,
            new ParticleGraphTypedPin<Quaternion>("q"),
            new ParticleGraphTypedPin<float>(ParticleGraphPinFlag.Output, "x"),
            new ParticleGraphTypedPin<float>(ParticleGraphPinFlag.Output, "y"),
            new ParticleGraphTypedPin<float>(ParticleGraphPinFlag.Output, "z"),
            new ParticleGraphTypedPin<float>(ParticleGraphPinFlag.Output, "w")),
        Helpers.MakePattern<Quaternion, Vector3f, float>(Update,
            new ParticleGraphTypedPin<Quaternion>("q"),
            new ParticleGraphTypedPin<Vector3f>(ParticleGraphPinFlag.Output, "axis"),
            new ParticleGraphTypedPin<float>(ParticleGraphPinFlag.Output, "angle"))
    ];

    /// <summary>Construct Break.</summary>
    public Break() : base(BreakPatterns) { }

    public static void Update(in UpdateContext context, int count, SparseSpan<Vector3f> vec,
        SparseSpan<float> x, SparseSpan<float> y, SparseSpan<float> z)
    { for (var i = 0; i < count; ++i) { x[i] = vec[i].X; y[i] = vec[i].Y; z[i] = vec[i].Z; } }
    public static void Update(in UpdateContext context, int count, SparseSpan<Vector2f> vec,
        SparseSpan<float> x, SparseSpan<float> y)
    { for (var i = 0; i < count; ++i) { x[i] = vec[i].X; y[i] = vec[i].Y; } }
    public static void Update(in UpdateContext context, int count, SparseSpan<Quaternion> q,
        SparseSpan<float> x, SparseSpan<float> y, SparseSpan<float> z, SparseSpan<float> w)
    { for (var i = 0; i < count; ++i) { x[i] = q[i].X; y[i] = q[i].Y; z[i] = q[i].Z; w[i] = q[i].W; } }
    public static void Update(in UpdateContext context, int count, SparseSpan<Quaternion> q,
        SparseSpan<Vector3f> axis, SparseSpan<float> angle)
    { for (var i = 0; i < count; ++i) { var value = q[i]; angle[i] = value.Angle; axis[i] = value.Axis; } }
}
