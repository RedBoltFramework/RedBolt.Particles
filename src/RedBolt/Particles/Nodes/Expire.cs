// Copyright (c) 2026 RedBolt and contributors.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT> or the accompanying LICENSE file.

namespace RedBolt.Particles.ParticleGraphNodes;

using System;
using RedBolt.Core;

[Reflected]
public partial class Expire : TemplateNode<ExpireInstance, float, float>
{
    /// <summary>Construct Expire.</summary>
    public Expire() : base([
        new ParticleGraphPin(ParticleGraphPinFlag.Input, "time", ParticleGraphContainerType.Auto),
        new ParticleGraphPin(ParticleGraphPinFlag.Input, "lifetime", ParticleGraphContainerType.Auto)]) { }
    /// <summary>Evaluate size required to place new node instance.</summary>
    public override int EvaluateInstanceSize() => IntPtr.Size;
    /// <summary>Create new instance.</summary>
    public override ParticleGraphNodeInstance CreateInstance(ParticleGraphLayerInstance layer)
    { var instance = new ExpireInstance(); instance.Init(this, layer); return instance; }
}

public sealed class ExpireInstance : Expire.InstanceBase
{
    public override void Update(in UpdateContext context, int numParticles,
        SparseSpan<float> time, SparseSpan<float> lifetime)
    {
        // Iterate all particles even if all pins are scalar.
        for (var i = 0; i < context.Indices.Length; ++i)
            if (time[i] >= lifetime[i]) context.Layer.MarkForDeletion(i);
    }
}
