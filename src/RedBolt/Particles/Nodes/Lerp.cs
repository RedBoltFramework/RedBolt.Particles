// Copyright (c) 2026 RedBolt and contributors.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT> or the accompanying LICENSE file.

namespace RedBolt.Particles.ParticleGraphNodes;

using RedBolt.Core;
using RedBolt.Math;

[Reflected]
public partial class Lerp : PatternMatchingNode
{
    private static readonly NodePattern[] LerpPatterns =
    [
        Helpers.MakePattern<float, float, float, float>(Update,
            new ParticleGraphTypedPin<float>("x"), new ParticleGraphTypedPin<float>("y"),
            new ParticleGraphTypedPin<float>("t"), new ParticleGraphTypedPin<float>(ParticleGraphPinFlag.Output, "out")),
        Helpers.MakePattern<Vector2f, Vector2f, float, Vector2f>(Update,
            new ParticleGraphTypedPin<Vector2f>("x"), new ParticleGraphTypedPin<Vector2f>("y"),
            new ParticleGraphTypedPin<float>("t"), new ParticleGraphTypedPin<Vector2f>(ParticleGraphPinFlag.Output, "out")),
        Helpers.MakePattern<Vector3f, Vector3f, float, Vector3f>(Update,
            new ParticleGraphTypedPin<Vector3f>("x"), new ParticleGraphTypedPin<Vector3f>("y"),
            new ParticleGraphTypedPin<float>("t"), new ParticleGraphTypedPin<Vector3f>(ParticleGraphPinFlag.Output, "out")),
        Helpers.MakePattern<Vector4f, Vector4f, float, Vector4f>(Update,
            new ParticleGraphTypedPin<Vector4f>("x"), new ParticleGraphTypedPin<Vector4f>("y"),
            new ParticleGraphTypedPin<float>("t"), new ParticleGraphTypedPin<Vector4f>(ParticleGraphPinFlag.Output, "out")),
        Helpers.MakePattern<Color, Color, float, Color>(Update,
            new ParticleGraphTypedPin<Color>("x"), new ParticleGraphTypedPin<Color>("y"),
            new ParticleGraphTypedPin<float>("t"), new ParticleGraphTypedPin<Color>(ParticleGraphPinFlag.Output, "out"))
    ];

    /// <summary>Construct Lerp.</summary>
    public Lerp() : base(LerpPatterns) { }

    public static void Update(in UpdateContext context, int count, SparseSpan<float> x, SparseSpan<float> y,
        SparseSpan<float> t, SparseSpan<float> output)
    { for (var i = 0; i < count; ++i) output[i] = MathDefs.Lerp(x[i], y[i], t[i]); }
    public static void Update(in UpdateContext context, int count, SparseSpan<Vector2f> x, SparseSpan<Vector2f> y,
        SparseSpan<float> t, SparseSpan<Vector2f> output)
    { for (var i = 0; i < count; ++i) output[i] = x[i].Lerp(y[i], t[i]); }
    public static void Update(in UpdateContext context, int count, SparseSpan<Vector3f> x, SparseSpan<Vector3f> y,
        SparseSpan<float> t, SparseSpan<Vector3f> output)
    { for (var i = 0; i < count; ++i) output[i] = x[i].Lerp(y[i], t[i]); }
    public static void Update(in UpdateContext context, int count, SparseSpan<Vector4f> x, SparseSpan<Vector4f> y,
        SparseSpan<float> t, SparseSpan<Vector4f> output)
    { for (var i = 0; i < count; ++i) output[i] = x[i].Lerp(y[i], t[i]); }
    public static void Update(in UpdateContext context, int count, SparseSpan<Color> x, SparseSpan<Color> y,
        SparseSpan<float> t, SparseSpan<Color> output)
    { for (var i = 0; i < count; ++i) output[i] = x[i].Lerp(y[i], t[i]); }
}
