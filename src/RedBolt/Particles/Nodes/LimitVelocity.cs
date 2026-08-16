// Copyright (c) 2026 RedBolt and contributors.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT> or the accompanying LICENSE file.

namespace RedBolt.Particles.ParticleGraphNodes;

using System;
using RedBolt.Core;
using RedBolt.Math;

[Reflected]
public partial class LimitVelocity : TemplateNode<LimitVelocityInstance, Vector3f, float, Vector3f>
{
    /// <summary>Construct LimitVelocity.</summary>
    public LimitVelocity() : base([
        new ParticleGraphPin(ParticleGraphPinFlag.Input, "velocity", ParticleGraphContainerType.Auto),
        new ParticleGraphPin(ParticleGraphPinFlag.Input, "limit", ParticleGraphContainerType.Auto),
        new ParticleGraphPin(ParticleGraphPinFlag.Output, "out", ParticleGraphContainerType.Auto)]) { }
    /// <summary>Evaluate size required to place new node instance.</summary>
    public override int EvaluateInstanceSize() => IntPtr.Size;
    /// <summary>Create new instance.</summary>
    public override ParticleGraphNodeInstance CreateInstance(ParticleGraphLayerInstance layer)
    { var instance = new LimitVelocityInstance(); instance.Init(this, layer); return instance; }

    /// <summary>Set Dampen.</summary>
    [Serialized("Dampen")]
    public float Dampen { get; set; }
}

public sealed class LimitVelocityInstance : LimitVelocity.InstanceBase
{
    public override void Update(in UpdateContext context, int numParticles, SparseSpan<Vector3f> velocity,
        SparseSpan<float> limit, SparseSpan<Vector3f> result)
    {
        var dampen = ((LimitVelocity)GetGraphNode()).Dampen;
        if (dampen <= 1e-6f || context.TimeStep < 1e-6f)
        {
            for (var i = 0; i < numParticles; ++i) result[i] = velocity[i];
        }
        else
        {
            var t = 1.0f - MathF.Pow(1.0f - dampen, 20.0f * context.TimeStep);
            for (var i = 0; i < numParticles; ++i)
            {
                var speed = velocity[i].Length;
                var limitValue = limit[i];
                if (speed > limitValue + 1e-6f)
                    result[i] = velocity[i] * (MathDefs.Lerp(speed, limitValue, t) / speed);
            }
        }
    }
}
