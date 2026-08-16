// Copyright (c) 2026 RedBolt and contributors.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT> or the accompanying LICENSE file.

namespace RedBolt.Particles.ParticleGraphNodes;

using RedBolt.Core;

[Reflected]
public partial class Cast : PatternMatchingNode
{
    private static readonly NodePattern[] CastPatterns =
    [
        Helpers.MakePattern<float, int>(Update, new ParticleGraphTypedPin<float>("x"),
            new ParticleGraphTypedPin<int>(ParticleGraphPinFlag.Output, "out")),
        Helpers.MakePattern<int, float>(Update, new ParticleGraphTypedPin<int>("x"),
            new ParticleGraphTypedPin<float>(ParticleGraphPinFlag.Output, "out"))
    ];

    /// <summary>Construct Cast.</summary>
    public Cast() : base(CastPatterns) { }

    public static void Update(in UpdateContext context, int count, SparseSpan<float> x, SparseSpan<int> output)
    { for (var i = 0; i < count; ++i) output[i] = (int)x[i]; }
    public static void Update(in UpdateContext context, int count, SparseSpan<int> x, SparseSpan<float> output)
    { for (var i = 0; i < count; ++i) output[i] = x[i]; }
}
