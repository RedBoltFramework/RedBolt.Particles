// Copyright (c) 2026 RedBolt and contributors.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT> or the accompanying LICENSE file.

namespace RedBolt.Particles.ParticleGraphNodes;

using RedBolt.Core;
using RedBolt.Math;

[Reflected]
public partial class Normalized : PatternMatchingNode
{
    private static readonly NodePattern[] NormalizedPatterns =
    [
        Helpers.MakePattern<Vector3f, Vector3f>(Update, new ParticleGraphTypedPin<Vector3f>("x"),
            new ParticleGraphTypedPin<Vector3f>(ParticleGraphPinFlag.Output, "out")),
        Helpers.MakePattern<Vector2f, Vector2f>(Update, new ParticleGraphTypedPin<Vector2f>("x"),
            new ParticleGraphTypedPin<Vector2f>(ParticleGraphPinFlag.Output, "out")),
        Helpers.MakePattern<Quaternion, Quaternion>(Update, new ParticleGraphTypedPin<Quaternion>("x"),
            new ParticleGraphTypedPin<Quaternion>(ParticleGraphPinFlag.Output, "out"))
    ];

    /// <summary>Construct Normalized.</summary>
    public Normalized() : base(NormalizedPatterns) { }

    public static void Update(in UpdateContext context, int count, SparseSpan<Vector3f> x, SparseSpan<Vector3f> output)
    { for (var i = 0; i < count; ++i) output[i] = x[i].Normalized; }
    public static void Update(in UpdateContext context, int count, SparseSpan<Vector2f> x, SparseSpan<Vector2f> output)
    { for (var i = 0; i < count; ++i) output[i] = x[i].Normalized; }
    public static void Update(in UpdateContext context, int count, SparseSpan<Quaternion> x, SparseSpan<Quaternion> output)
    { for (var i = 0; i < count; ++i) output[i] = x[i].Normalized; }
}
