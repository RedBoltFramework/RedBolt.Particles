// Copyright (c) 2026 RedBolt and contributors.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT> or the accompanying LICENSE file.

namespace RedBolt.Particles.ParticleGraphNodes;

using System;
using RedBolt.Core;
using RedBolt.Math;

[Reflected]
public partial class CurlNoise3D : TemplateNode<CurlNoise3DInstance, Vector3f, Vector3f>
{
    /// <summary>Construct CurlNoise3D.</summary>
    public CurlNoise3D() : base([
        new ParticleGraphPin(ParticleGraphPinFlag.Input, "x", ParticleGraphContainerType.Auto),
        new ParticleGraphPin(ParticleGraphPinFlag.Output, "out", ParticleGraphContainerType.Auto)]) { }
    public override int EvaluateInstanceSize() => IntPtr.Size;
    public override ParticleGraphNodeInstance CreateInstance(ParticleGraphLayerInstance layer)
    { var instance = new CurlNoise3DInstance(); instance.Init(this, layer); return instance; }
}

public sealed class CurlNoise3DInstance : CurlNoise3D.InstanceBase
{
    public CurlNoise3DInstance() => Noise = new PerlinNoise(RandomEngine.GetDefaultEngine());

    public override void Init(ParticleGraphNode node, ParticleGraphLayerInstance layer)
    {
        base.Init(node, layer);
        ScrollPosition = 0.0;
        //noise_.SetNoiseType(FastNoiseLite::NoiseType_Perlin);
        //noise_.SetFractalOctaves(1);
        //noise_.SetFrequency(0.99f);
    }

    public override void Update(in UpdateContext context, int numParticles,
        SparseSpan<Vector3f> x, SparseSpan<Vector3f> output)
    {
        ScrollPosition += context.TimeStep;
        for (var i = 0; i < numParticles; ++i) output[i] = Generate(x[i]);
    }

    public Vector3f Generate(in Vector3f position)
    {
        const float offset = 0.01f;
        const float frequency = 2.0f;
        var x = position.X * frequency;
        var y = position.Y * frequency;
        var z = position.Z * frequency + ScrollPosition;
        const double scale = 0.02;
        var center = Noise.GetDouble(x, y, z) * scale;
        var sampleX = Noise.GetDouble(x + offset, y, z) * scale;
        var sampleY = Noise.GetDouble(x, y + offset, z) * scale;
        var sampleZ = Noise.GetDouble(x, y, z + offset) * scale;
        var dx = (sampleX - center) / offset;
        var dy = (sampleY - center) / offset;
        var dz = (sampleZ - center) / offset;
        return new Vector3f((float)(dz - dy), (float)(dx - dz), (float)(dy - dx));
    }

    public readonly PerlinNoise Noise;
    public double ScrollPosition;
    //FastNoiseLite noise_;
}
