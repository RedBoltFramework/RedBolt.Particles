// Copyright (c) 2026 RedBolt and contributors.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT> or the accompanying LICENSE file.

namespace RedBolt.Particles.ParticleGraphNodes;

using System;
using RedBolt.Core;

[Reflected]
public partial class BurstTimer : TemplateNode<BurstTimerInstance, float, float>
{
    /// <summary>Construct BurstTimer.</summary>
    public BurstTimer() : base([
        new ParticleGraphPin(ParticleGraphPinFlag.Input, "count", ParticleGraphContainerType.Auto),
        new ParticleGraphPin(ParticleGraphPinFlag.Output, "out", ParticleGraphContainerType.Auto)]) { }

    public override int EvaluateInstanceSize() => IntPtr.Size;
    public override ParticleGraphNodeInstance CreateInstance(ParticleGraphLayerInstance layer)
    { var instance = new BurstTimerInstance(); instance.Init(this, layer); return instance; }

    [Serialized("Delay")]
    public float Delay { get; set; }
    [Serialized("Interval")]
    public float Interval { get; set; }
    [Serialized("Cycles")]
    public int Cycles { get; set; }
}

public sealed class BurstTimerInstance : BurstTimer.InstanceBase
{
    public override void Init(ParticleGraphNode node, ParticleGraphLayerInstance layer)
    {
        base.Init(node, layer);
        var burstTimer = (BurstTimer)node;
        TimeToBurst = burstTimer.Delay;
        Counter = unchecked((uint)burstTimer.Cycles);
    }

    public override void Update(in UpdateContext context, int numParticles,
        SparseSpan<float> count, SparseSpan<float> output)
    {
        TimeToBurst -= context.TimeStep;
        if (Counter > 0 && TimeToBurst <= 0.0f)
        {
            // Can't use iterator here as it may use ScalarSpan with infinite iterator.
            for (var i = 0; i < numParticles; ++i) output[i] = count[i];
            TimeToBurst += ((BurstTimer)GetGraphNode()).Interval;
            --Counter;
        }
        else
        {
            for (var i = 0; i < numParticles; ++i) output[i] = 0.0f;
        }
    }

    public float TimeToBurst;
    public uint Counter;
}
