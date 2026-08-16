// Copyright (c) 2026 RedBolt and contributors.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT> or the accompanying LICENSE file.

namespace RedBolt.Particles.ParticleGraphNodes;

using System;
using RedBolt.Core;

[Reflected]
public partial class Emit : TemplateNode<EmitInstance, float>
{
    /// <summary>Construct Emit.</summary>
    public Emit() : base([new ParticleGraphPin(ParticleGraphPinFlag.Input, "count", ParticleGraphContainerType.Auto)]) { }
    /// <summary>Evaluate size required to place new node instance.</summary>
    public override int EvaluateInstanceSize() => IntPtr.Size;
    /// <summary>Create new instance.</summary>
    public override ParticleGraphNodeInstance CreateInstance(ParticleGraphLayerInstance layer)
    { var instance = new EmitInstance(); instance.Init(this, layer); return instance; }
}

public sealed class EmitInstance : Emit.InstanceBase
{
    public override void Update(in UpdateContext context, int numParticles, SparseSpan<float> span)
    {
        // Can't use iterator here as it may use ScalarSpan with infinite iterator.
        var sum = 0.0f;
        for (var i = 0; i < numParticles; ++i) sum += span[i];
        context.Layer.EmitNewParticles(sum);
    }
}
