// Copyright (c) 2026 RedBolt and contributors.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT> or the accompanying LICENSE file.

namespace RedBolt.Particles.ParticleGraphNodes;

using System;
using RedBolt.Core;
using RedBolt.Math;

[Reflected]
public partial class Noise3D : TemplateNode<Noise3DInstance, Vector3f, float>
{
    /// <summary>Construct Noise3D.</summary>
    public Noise3D() : base([
        new ParticleGraphPin(ParticleGraphPinFlag.Input, "x", ParticleGraphContainerType.Auto),
        new ParticleGraphPin(ParticleGraphPinFlag.Output, "out", ParticleGraphContainerType.Auto)]) { }
    public override int EvaluateInstanceSize() => IntPtr.Size;
    public override ParticleGraphNodeInstance CreateInstance(ParticleGraphLayerInstance layer)
    { var instance = new Noise3DInstance(); instance.Init(this, layer); return instance; }
}

public sealed class Noise3DInstance : Noise3D.InstanceBase
{
    public Noise3DInstance() => Noise = new PerlinNoise(RandomEngine.GetDefaultEngine());
    public override void Update(in UpdateContext context, int numParticles,
        SparseSpan<Vector3f> x, SparseSpan<float> output)
    { for (var i = 0; i < numParticles; ++i) output[i] = Generate(x[i]); }
    public float Generate(in Vector3f position) => (float)Noise.GetDouble(position.X, position.Y, position.Z);
    public readonly PerlinNoise Noise;
}
