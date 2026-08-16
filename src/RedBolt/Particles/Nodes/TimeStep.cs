// Copyright (c) 2026 RedBolt and contributors.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT> or the accompanying LICENSE file.

namespace RedBolt.Particles.ParticleGraphNodes;

using System;
using RedBolt.Core;

[Reflected]
public partial class TimeStep : TemplateNode<TimeStepInstance, float>
{
    /// <summary>Construct TimeStep.</summary>
    public TimeStep() : base([new ParticleGraphPin(ParticleGraphPinFlag.Output, "out", ParticleGraphContainerType.Scalar)]) { }

    /// <summary>Evaluate size required to place new node instance.</summary>
    public override int EvaluateInstanceSize() => IntPtr.Size;

    /// <summary>Create new instance.</summary>
    public override ParticleGraphNodeInstance CreateInstance(ParticleGraphLayerInstance layer)
    { var instance = new TimeStepInstance(); instance.Init(this, layer); return instance; }
}

public sealed class TimeStepInstance : TimeStep.InstanceBase
{
    public override void Update(in UpdateContext context, int numParticles, SparseSpan<float> pin0)
    { for (var i = 0; i < numParticles; ++i) pin0[i] = context.TimeStep; }
}
