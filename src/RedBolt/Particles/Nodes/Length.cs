// Copyright (c) 2026 RedBolt and contributors.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT> or the accompanying LICENSE file.

namespace RedBolt.Particles.ParticleGraphNodes;

using RedBolt.Core;
using RedBolt.Math;

[Reflected]
public partial class Length : PatternMatchingNode
{
    private static readonly NodePattern[] LengthPatterns =
    [
        Helpers.MakePattern<Vector3f, float>(Update, new ParticleGraphTypedPin<Vector3f>("x"),
            new ParticleGraphTypedPin<float>(ParticleGraphPinFlag.Output, "out")),
        Helpers.MakePattern<Vector2f, float>(Update, new ParticleGraphTypedPin<Vector2f>("x"),
            new ParticleGraphTypedPin<float>(ParticleGraphPinFlag.Output, "out"))
    ];

    /// <summary>Construct Length.</summary>
    public Length() : base(LengthPatterns) { }

    public static void Update(in UpdateContext context, int count, SparseSpan<Vector3f> x, SparseSpan<float> output)
    { for (var i = 0; i < count; ++i) output[i] = x[i].Length; }
    public static void Update(in UpdateContext context, int count, SparseSpan<Vector2f> x, SparseSpan<float> output)
    { for (var i = 0; i < count; ++i) output[i] = x[i].Length; }
}
