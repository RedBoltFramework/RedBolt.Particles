// Copyright (c) 2026 RedBolt and contributors.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT> or the accompanying LICENSE file.

namespace RedBolt.Particles.ParticleGraphNodes;

using RedBolt.Core;
using RedBolt.Math;

[Reflected]
public partial class Clamp : PatternMatchingNode
{
    private static readonly NodePattern[] ClampPatterns =
    [
        Helpers.MakePattern<float, float>(Update, new ParticleGraphTypedPin<float>("x"),
            new ParticleGraphTypedPin<float>(ParticleGraphPinFlag.Output, "out")),
        Helpers.MakePattern<Vector2f, Vector2f>(Update, new ParticleGraphTypedPin<Vector2f>("x"),
            new ParticleGraphTypedPin<Vector2f>(ParticleGraphPinFlag.Output, "out")),
        Helpers.MakePattern<Vector3f, Vector3f>(Update, new ParticleGraphTypedPin<Vector3f>("x"),
            new ParticleGraphTypedPin<Vector3f>(ParticleGraphPinFlag.Output, "out")),
        Helpers.MakePattern<Vector4f, Vector4f>(Update, new ParticleGraphTypedPin<Vector4f>("x"),
            new ParticleGraphTypedPin<Vector4f>(ParticleGraphPinFlag.Output, "out"))
    ];

    /// <summary>Construct Clamp.</summary>
    public Clamp() : base(ClampPatterns) { }

    public static void Update(in UpdateContext context, int count, SparseSpan<float> x, SparseSpan<float> output)
    { for (var i = 0; i < count; ++i) output[i] = MathDefs.Clamp(x[i], 0.0f, 1.0f); }
    public static void Update(in UpdateContext context, int count, SparseSpan<Vector2f> x, SparseSpan<Vector2f> output)
    { for (var i = 0; i < count; ++i) output[i] = MathDefs.VectorClamp(x[i], Vector2f.Zero, Vector2f.One); }
    public static void Update(in UpdateContext context, int count, SparseSpan<Vector3f> x, SparseSpan<Vector3f> output)
    { for (var i = 0; i < count; ++i) output[i] = MathDefs.VectorClamp(x[i], Vector3f.Zero, Vector3f.One); }
    public static void Update(in UpdateContext context, int count, SparseSpan<Vector4f> x, SparseSpan<Vector4f> output)
    { for (var i = 0; i < count; ++i) output[i] = MathDefs.VectorClamp(x[i], Vector4f.Zero, Vector4f.One); }
}
