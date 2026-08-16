// Copyright (c) 2026 RedBolt and contributors.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT> or the accompanying LICENSE file.

namespace RedBolt.Particles.ParticleGraphNodes;

using System;
using RedBolt.Core;

[Reflected]
public partial class Destroy : TemplateNode<DestroyInstance, bool>
{
    /// <summary>Construct Destroy.</summary>
    public Destroy() : base([new ParticleGraphPin(ParticleGraphPinFlag.Input, "condition", ParticleGraphContainerType.Auto)]) { }
    /// <summary>Evaluate size required to place new node instance.</summary>
    public override int EvaluateInstanceSize() => IntPtr.Size;
    /// <summary>Create new instance.</summary>
    public override ParticleGraphNodeInstance CreateInstance(ParticleGraphLayerInstance layer)
    { var instance = new DestroyInstance(); instance.Init(this, layer); return instance; }
}

public sealed class DestroyInstance : Destroy.InstanceBase
{
    public override void Update(in UpdateContext context, int numParticles, SparseSpan<bool> condition)
    {
        // Iterate all particles even if all pins are scalar.
        for (var i = 0; i < context.Indices.Length; ++i)
            if (condition[i]) context.Layer.MarkForDeletion(i);
    }
}
