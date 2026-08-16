// Copyright (c) 2026 RedBolt and contributors.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT> or the accompanying LICENSE file.

namespace RedBolt.Particles.ParticleGraphNodes;

using System;
using RedBolt.Core;
using RedBolt.Math;

[Reflected]
public partial class Move : TemplateNode<MoveInstance, Vector3f, Vector3f, Vector3f>
{
    /// <summary>Construct Move.</summary>
    public Move() : base([
        new ParticleGraphPin(ParticleGraphPinFlag.Input, "position", ParticleGraphContainerType.Auto),
        new ParticleGraphPin(ParticleGraphPinFlag.Input, "velocity", ParticleGraphContainerType.Auto),
        new ParticleGraphPin(ParticleGraphPinFlag.Output, "newPosition", ParticleGraphContainerType.Auto)]) { }
    /// <summary>Evaluate size required to place new node instance.</summary>
    public override int EvaluateInstanceSize() => IntPtr.Size;
    /// <summary>Create new instance.</summary>
    public override ParticleGraphNodeInstance CreateInstance(ParticleGraphLayerInstance layer)
    { var instance = new MoveInstance(); instance.Init(this, layer); return instance; }
}

public sealed class MoveInstance : Move.InstanceBase
{
    public override void Update(in UpdateContext context, int numParticles, SparseSpan<Vector3f> position,
        SparseSpan<Vector3f> velocity, SparseSpan<Vector3f> newPosition)
    { for (var i = 0; i < numParticles; ++i) newPosition[i] = position[i] + context.TimeStep * velocity[i]; }
}
