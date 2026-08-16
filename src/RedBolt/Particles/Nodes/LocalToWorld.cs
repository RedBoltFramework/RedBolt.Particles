// Copyright (c) 2026 RedBolt and contributors.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT> or the accompanying LICENSE file.

namespace RedBolt.Particles.ParticleGraphNodes;

using System;
using RedBolt.Core;
using RedBolt.Math;

[Reflected]
public partial class LocalToWorld : TemplateNode<LocalToWorldInstance, Matrix3x4>
{
    /// <summary>Construct LocalToWorld.</summary>
    public LocalToWorld() : base([new ParticleGraphPin(ParticleGraphPinFlag.Output, "out", ParticleGraphContainerType.Scalar)]) { }
    public override int EvaluateInstanceSize() => IntPtr.Size;
    public override ParticleGraphNodeInstance CreateInstance(ParticleGraphLayerInstance layer)
    { var instance = new LocalToWorldInstance(); instance.Init(this, layer); return instance; }
}

public sealed class LocalToWorldInstance : LocalToWorld.InstanceBase
{
    public override void Update(in UpdateContext context, int numParticles, SparseSpan<Matrix3x4> output)
    { var node = GetNode()!; for (var i = 0; i < numParticles; ++i) output[i] = node.GetWorldTransform(); }
}
