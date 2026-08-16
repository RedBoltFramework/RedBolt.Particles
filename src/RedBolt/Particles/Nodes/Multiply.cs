// Copyright (c) 2026 RedBolt and contributors.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT> or the accompanying LICENSE file.

namespace RedBolt.Particles.ParticleGraphNodes;

using RedBolt.Core;
using RedBolt.Math;

[Reflected]
public partial class Multiply : PatternMatchingNode
{
    private static readonly NodePattern[] MultiplyPatterns =
    [
        Helpers.MakePattern<float, float, float>(Update,
            new ParticleGraphTypedPin<float>("x"), new ParticleGraphTypedPin<float>("y"),
            new ParticleGraphTypedPin<float>(ParticleGraphPinFlag.Output, "out")),
        Helpers.MakePattern<Vector3f, float, Vector3f>(Update,
            new ParticleGraphTypedPin<Vector3f>("x"), new ParticleGraphTypedPin<float>("y"),
            new ParticleGraphTypedPin<Vector3f>(ParticleGraphPinFlag.Output, "out")),
        Helpers.MakePattern<float, Vector3f, Vector3f>(Update,
            new ParticleGraphTypedPin<float>("x"), new ParticleGraphTypedPin<Vector3f>("y"),
            new ParticleGraphTypedPin<Vector3f>(ParticleGraphPinFlag.Output, "out")),
        Helpers.MakePattern<Color, Color, Color>(Update,
            new ParticleGraphTypedPin<Color>("x"), new ParticleGraphTypedPin<Color>("y"),
            new ParticleGraphTypedPin<Color>(ParticleGraphPinFlag.Output, "out"))
    ];

    /// <summary>Construct Multiply.</summary>
    public Multiply()
        : base(MultiplyPatterns)
    {
    }

    public static void Update(in UpdateContext context, int numParticles,
        SparseSpan<float> x, SparseSpan<float> y, SparseSpan<float> output)
    { for (var i = 0; i < numParticles; ++i) output[i] = x[i] * y[i]; }

    public static void Update(in UpdateContext context, int numParticles,
        SparseSpan<Vector3f> x, SparseSpan<float> y, SparseSpan<Vector3f> output)
    { for (var i = 0; i < numParticles; ++i) output[i] = x[i] * y[i]; }

    public static void Update(in UpdateContext context, int numParticles,
        SparseSpan<float> x, SparseSpan<Vector3f> y, SparseSpan<Vector3f> output)
    { for (var i = 0; i < numParticles; ++i) output[i] = x[i] * y[i]; }

    public static void Update(in UpdateContext context, int numParticles,
        SparseSpan<Color> x, SparseSpan<Color> y, SparseSpan<Color> output)
    { for (var i = 0; i < numParticles; ++i) output[i] = x[i] * y[i]; }
}
