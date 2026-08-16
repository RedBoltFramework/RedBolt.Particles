// Copyright (c) 2026 RedBolt and contributors.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT> or the accompanying LICENSE file.

namespace RedBolt.Particles.ParticleGraphNodes;

using RedBolt.Core;
using RedBolt.Math;

[Reflected]
public partial class Divide : PatternMatchingNode
{
    private static readonly NodePattern[] DividePatterns =
    [
        Helpers.MakePattern<float, float, float>(Update,
            new ParticleGraphTypedPin<float>("x"), new ParticleGraphTypedPin<float>("y"),
            new ParticleGraphTypedPin<float>(ParticleGraphPinFlag.Output, "out")),
        Helpers.MakePattern<Vector2f, float, Vector2f>(Update,
            new ParticleGraphTypedPin<Vector2f>("x"), new ParticleGraphTypedPin<float>("y"),
            new ParticleGraphTypedPin<Vector2f>(ParticleGraphPinFlag.Output, "out")),
        Helpers.MakePattern<Vector3f, float, Vector3f>(Update,
            new ParticleGraphTypedPin<Vector3f>("x"), new ParticleGraphTypedPin<float>("y"),
            new ParticleGraphTypedPin<Vector3f>(ParticleGraphPinFlag.Output, "out")),
        Helpers.MakePattern<Vector4f, float, Vector4f>(Update,
            new ParticleGraphTypedPin<Vector4f>("x"), new ParticleGraphTypedPin<float>("y"),
            new ParticleGraphTypedPin<Vector4f>(ParticleGraphPinFlag.Output, "out"))
    ];

    /// <summary>Construct Divide.</summary>
    public Divide()
        : base(DividePatterns)
    {
    }

    public static void Update(in UpdateContext context, int numParticles,
        SparseSpan<float> x, SparseSpan<float> y, SparseSpan<float> output)
    { for (var i = 0; i < numParticles; ++i) output[i] = x[i] / y[i]; }

    public static void Update(in UpdateContext context, int numParticles,
        SparseSpan<Vector2f> x, SparseSpan<float> y, SparseSpan<Vector2f> output)
    { for (var i = 0; i < numParticles; ++i) output[i] = x[i] / y[i]; }

    public static void Update(in UpdateContext context, int numParticles,
        SparseSpan<Vector3f> x, SparseSpan<float> y, SparseSpan<Vector3f> output)
    { for (var i = 0; i < numParticles; ++i) output[i] = x[i] / y[i]; }

    public static void Update(in UpdateContext context, int numParticles,
        SparseSpan<Vector4f> x, SparseSpan<float> y, SparseSpan<Vector4f> output)
    { for (var i = 0; i < numParticles; ++i) output[i] = x[i] / y[i]; }
}
