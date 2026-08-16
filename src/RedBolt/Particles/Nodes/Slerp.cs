// Copyright (c) 2026 RedBolt and contributors.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT> or the accompanying LICENSE file.

namespace RedBolt.Particles.ParticleGraphNodes;

using System;
using RedBolt.Core;
using RedBolt.Math;

[Reflected]
public partial class Slerp : TemplateNode<SlerpInstance, Quaternion, Quaternion, float, Quaternion>
{
    /// <summary>Construct Slerp.</summary>
    public Slerp() : base([
        new ParticleGraphPin(ParticleGraphPinFlag.Input, "x", ParticleGraphContainerType.Auto),
        new ParticleGraphPin(ParticleGraphPinFlag.Input, "y", ParticleGraphContainerType.Auto),
        new ParticleGraphPin(ParticleGraphPinFlag.Input, "t", ParticleGraphContainerType.Auto),
        new ParticleGraphPin(ParticleGraphPinFlag.Output, "out", ParticleGraphContainerType.Auto)]) { }
    /// <summary>Evaluate size required to place new node instance.</summary>
    public override int EvaluateInstanceSize() => IntPtr.Size;
    /// <summary>Create new instance.</summary>
    public override ParticleGraphNodeInstance CreateInstance(ParticleGraphLayerInstance layer)
    { var instance = new SlerpInstance(); instance.Init(this, layer); return instance; }
}

public sealed class SlerpInstance : Slerp.InstanceBase
{
    public override void Update(in UpdateContext context, int numParticles, SparseSpan<Quaternion> x,
        SparseSpan<Quaternion> y, SparseSpan<float> t, SparseSpan<Quaternion> output)
    { for (var i = 0; i < numParticles; ++i) output[i] = x[i].Slerp(y[i], t[i]); }
}
