// Copyright (c) 2026 RedBolt and contributors.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT> or the accompanying LICENSE file.

namespace RedBolt.Particles.ParticleGraphNodes;

using System;
using RedBolt.Core;
using RedBolt.Math;

[Reflected]
public partial class ApplyForce : TemplateNode<ApplyForceInstance, Vector3f, Vector3f, Vector3f>
{
    /// <summary>Construct ApplyForce.</summary>
    public ApplyForce() : base([
        new ParticleGraphPin(ParticleGraphPinFlag.Input, "velocity", ParticleGraphContainerType.Auto),
        new ParticleGraphPin(ParticleGraphPinFlag.Input, "force", ParticleGraphContainerType.Auto),
        new ParticleGraphPin(ParticleGraphPinFlag.Output, "out", ParticleGraphContainerType.Auto)]) { }
    /// <summary>Evaluate size required to place new node instance.</summary>
    public override int EvaluateInstanceSize() => IntPtr.Size;
    /// <summary>Create new instance.</summary>
    public override ParticleGraphNodeInstance CreateInstance(ParticleGraphLayerInstance layer)
    { var instance = new ApplyForceInstance(); instance.Init(this, layer); return instance; }
}

public sealed class ApplyForceInstance : ApplyForce.InstanceBase
{
    public override void Update(in UpdateContext context, int numParticles, SparseSpan<Vector3f> velocity,
        SparseSpan<Vector3f> force, SparseSpan<Vector3f> result)
    { for (var i = 0; i < numParticles; ++i) result[i] = velocity[i] + force[i] * context.TimeStep; }
}
