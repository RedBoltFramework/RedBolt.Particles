// Copyright (c) 2026 RedBolt and contributors.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT> or the accompanying LICENSE file.

namespace RedBolt.Particles.ParticleGraphNodes;

using RedBolt.Core;
using RedBolt.Scene;

public abstract class TemplateNodeInstanceBase : ParticleGraphNodeInstance
{
    /// <summary>
    /// Init instance.
    /// </summary>
    public virtual void Init(ParticleGraphNode node, ParticleGraphLayerInstance layer)
    {
        System.Diagnostics.Debug.Assert(node is not null);
        _node = node;
        _layer = layer;
    }

    /// <summary>
    /// Get graph node instance.
    /// </summary>
    public ParticleGraphNode GetGraphNode() => _node!;

    /// <summary>
    /// Get graph layer instance.
    /// </summary>
    public ParticleGraphLayerInstance GetLayerInstance() => _layer!;

    /// <summary>
    /// Get graph layer.
    /// </summary>
    public ParticleGraphLayer? GetLayer() => _layer!.Layer;

    /// <summary>
    /// Get emitter component.
    /// </summary>
    public ParticleGraphEmitter? GetEmitter() => _layer!.Emitter;

    /// <summary>
    /// Get scene node.
    /// </summary>
    public Node? GetNode()
    {
        var emitter = GetEmitter();
        return emitter?.TryGetNode();
    }

    /// <summary>
    /// Get engine context.
    /// </summary>
    public Context? GetContext()
    {
        var emitter = GetEmitter();
        return emitter?.Context;
    }

    /// <summary>
    /// Get scene.
    /// </summary>
    public Scene? GetScene()
    {
        var emitter = GetEmitter();
        return emitter?.Scene;
    }

    // Static assert to simplify compile type error description.
    //template <typename... args> void operator()(args... a) { static_assert(false, "Instance operator() is not defined or arguments doesn't match node pins"); }

    /// <summary>
    /// Pointer to graph node instance.
    /// </summary>
    protected ParticleGraphNode? _node;

    /// <summary>
    /// Pointer to graph layer instance.
    /// </summary>
    protected ParticleGraphLayerInstance? _layer;
}

/// <summary>
/// Template for a particle graph node class.
/// </summary>
public abstract class TemplateNode<TInstance, T0> : ParticleGraphNode
{
    public const int NumberOfPins = 1;

    protected TemplateNode(ParticleGraphPin[] pins)
    {
        _pins = [pins[0].WithType(VariantTypeUtility.GetVariantType<T0>())];
    }

    public abstract class InstanceBase : TemplateNodeInstanceBase
    {
        /// <summary>
        /// Update particles.
        /// </summary>
        public override void Update(ref UpdateContext context)
        {
            var pin0 = context.GetSpan<T0>(_node!.GetPin(0).MemoryReference);
            Update(in context, context.Indices.Length, pin0);
        }

        public abstract void Update(in UpdateContext context, int numParticles, SparseSpan<T0> pin0);
    }

    /// <summary>
    /// Get number of pins.
    /// </summary>
    public override int NumPins => NumberOfPins;

    /// <summary>
    /// Get pin by index.
    /// </summary>
    public override ParticleGraphPin GetPin(int index) => _pins[index];

    /// <summary>
    /// Pins
    /// </summary>
    protected readonly ParticleGraphPin[] _pins;
}

/// <summary>
/// Template for a particle graph node class.
/// </summary>
public abstract class TemplateNode<TInstance, T0, T1> : ParticleGraphNode
{
    public const int NumberOfPins = 2;

    protected TemplateNode(ParticleGraphPin[] pins)
    {
        _pins =
        [
            pins[0].WithType(VariantTypeUtility.GetVariantType<T0>()),
            pins[1].WithType(VariantTypeUtility.GetVariantType<T1>())
        ];
    }

    public abstract class InstanceBase : TemplateNodeInstanceBase
    {
        /// <summary>
        /// Update particles.
        /// </summary>
        public override void Update(ref UpdateContext context)
        {
            var pin0 = context.GetSpan<T0>(_node!.GetPin(0).MemoryReference);
            var pin1 = context.GetSpan<T1>(_node.GetPin(1).MemoryReference);
            Update(in context, context.Indices.Length, pin0, pin1);
        }

        public abstract void Update(in UpdateContext context, int numParticles,
            SparseSpan<T0> pin0, SparseSpan<T1> pin1);
    }

    /// <summary>
    /// Get number of pins.
    /// </summary>
    public override int NumPins => NumberOfPins;

    /// <summary>
    /// Get pin by index.
    /// </summary>
    public override ParticleGraphPin GetPin(int index) => _pins[index];

    /// <summary>
    /// Pins
    /// </summary>
    protected readonly ParticleGraphPin[] _pins;
}

/// <summary>
/// Template for a particle graph node class.
/// </summary>
public abstract class TemplateNode<TInstance, T0, T1, T2> : ParticleGraphNode
{
    public const int NumberOfPins = 3;

    protected TemplateNode(ParticleGraphPin[] pins)
    {
        _pins =
        [
            pins[0].WithType(VariantTypeUtility.GetVariantType<T0>()),
            pins[1].WithType(VariantTypeUtility.GetVariantType<T1>()),
            pins[2].WithType(VariantTypeUtility.GetVariantType<T2>())
        ];
    }

    public abstract class InstanceBase : TemplateNodeInstanceBase
    {
        /// <summary>
        /// Update particles.
        /// </summary>
        public override void Update(ref UpdateContext context)
        {
            var pin0 = context.GetSpan<T0>(_node!.GetPin(0).MemoryReference);
            var pin1 = context.GetSpan<T1>(_node.GetPin(1).MemoryReference);
            var pin2 = context.GetSpan<T2>(_node.GetPin(2).MemoryReference);
            Update(in context, context.Indices.Length, pin0, pin1, pin2);
        }

        public abstract void Update(in UpdateContext context, int numParticles,
            SparseSpan<T0> pin0, SparseSpan<T1> pin1, SparseSpan<T2> pin2);
    }

    /// <summary>
    /// Get number of pins.
    /// </summary>
    public override int NumPins => NumberOfPins;

    /// <summary>
    /// Get pin by index.
    /// </summary>
    public override ParticleGraphPin GetPin(int index) => _pins[index];

    /// <summary>
    /// Pins
    /// </summary>
    protected readonly ParticleGraphPin[] _pins;
}

/// <summary>
/// Template for a particle graph node class.
/// </summary>
public abstract class TemplateNode<TInstance, T0, T1, T2, T3> : ParticleGraphNode
{
    public const int NumberOfPins = 4;

    protected TemplateNode(ParticleGraphPin[] pins)
    {
        _pins =
        [
            pins[0].WithType(VariantTypeUtility.GetVariantType<T0>()),
            pins[1].WithType(VariantTypeUtility.GetVariantType<T1>()),
            pins[2].WithType(VariantTypeUtility.GetVariantType<T2>()),
            pins[3].WithType(VariantTypeUtility.GetVariantType<T3>())
        ];
    }

    public abstract class InstanceBase : TemplateNodeInstanceBase
    {
        /// <summary>
        /// Update particles.
        /// </summary>
        public override void Update(ref UpdateContext context)
        {
            var pin0 = context.GetSpan<T0>(_node!.GetPin(0).MemoryReference);
            var pin1 = context.GetSpan<T1>(_node.GetPin(1).MemoryReference);
            var pin2 = context.GetSpan<T2>(_node.GetPin(2).MemoryReference);
            var pin3 = context.GetSpan<T3>(_node.GetPin(3).MemoryReference);
            Update(in context, context.Indices.Length, pin0, pin1, pin2, pin3);
        }

        public abstract void Update(in UpdateContext context, int numParticles,
            SparseSpan<T0> pin0, SparseSpan<T1> pin1, SparseSpan<T2> pin2, SparseSpan<T3> pin3);
    }

    /// <summary>
    /// Get number of pins.
    /// </summary>
    public override int NumPins => NumberOfPins;

    /// <summary>
    /// Get pin by index.
    /// </summary>
    public override ParticleGraphPin GetPin(int index) => _pins[index];

    /// <summary>
    /// Pins
    /// </summary>
    protected readonly ParticleGraphPin[] _pins;
}

/// <summary>
/// Template for a particle graph node class.
/// </summary>
public abstract class TemplateNode<TInstance, T0, T1, T2, T3, T4> : ParticleGraphNode
{
    public const int NumberOfPins = 5;

    protected TemplateNode(ParticleGraphPin[] pins)
    {
        _pins =
        [
            pins[0].WithType(VariantTypeUtility.GetVariantType<T0>()),
            pins[1].WithType(VariantTypeUtility.GetVariantType<T1>()),
            pins[2].WithType(VariantTypeUtility.GetVariantType<T2>()),
            pins[3].WithType(VariantTypeUtility.GetVariantType<T3>()),
            pins[4].WithType(VariantTypeUtility.GetVariantType<T4>())
        ];
    }

    public abstract class InstanceBase : TemplateNodeInstanceBase
    {
        /// <summary>
        /// Update particles.
        /// </summary>
        public override void Update(ref UpdateContext context)
        {
            var pin0 = context.GetSpan<T0>(_node!.GetPin(0).MemoryReference);
            var pin1 = context.GetSpan<T1>(_node.GetPin(1).MemoryReference);
            var pin2 = context.GetSpan<T2>(_node.GetPin(2).MemoryReference);
            var pin3 = context.GetSpan<T3>(_node.GetPin(3).MemoryReference);
            var pin4 = context.GetSpan<T4>(_node.GetPin(4).MemoryReference);
            Update(in context, context.Indices.Length, pin0, pin1, pin2, pin3, pin4);
        }

        public abstract void Update(in UpdateContext context, int numParticles,
            SparseSpan<T0> pin0, SparseSpan<T1> pin1, SparseSpan<T2> pin2, SparseSpan<T3> pin3,
            SparseSpan<T4> pin4);
    }

    /// <summary>
    /// Get number of pins.
    /// </summary>
    public override int NumPins => NumberOfPins;

    /// <summary>
    /// Get pin by index.
    /// </summary>
    public override ParticleGraphPin GetPin(int index) => _pins[index];

    /// <summary>
    /// Pins
    /// </summary>
    protected readonly ParticleGraphPin[] _pins;
}

/// <summary>
/// Template for a particle graph node class.
/// </summary>
public abstract class TemplateNode<TInstance, T0, T1, T2, T3, T4, T5> : ParticleGraphNode
{
    public const int NumberOfPins = 6;

    protected TemplateNode(ParticleGraphPin[] pins)
    {
        _pins =
        [
            pins[0].WithType(VariantTypeUtility.GetVariantType<T0>()),
            pins[1].WithType(VariantTypeUtility.GetVariantType<T1>()),
            pins[2].WithType(VariantTypeUtility.GetVariantType<T2>()),
            pins[3].WithType(VariantTypeUtility.GetVariantType<T3>()),
            pins[4].WithType(VariantTypeUtility.GetVariantType<T4>()),
            pins[5].WithType(VariantTypeUtility.GetVariantType<T5>())
        ];
    }

    public abstract class InstanceBase : TemplateNodeInstanceBase
    {
        /// <summary>
        /// Update particles.
        /// </summary>
        public override void Update(ref UpdateContext context)
        {
            var pin0 = context.GetSpan<T0>(_node!.GetPin(0).MemoryReference);
            var pin1 = context.GetSpan<T1>(_node.GetPin(1).MemoryReference);
            var pin2 = context.GetSpan<T2>(_node.GetPin(2).MemoryReference);
            var pin3 = context.GetSpan<T3>(_node.GetPin(3).MemoryReference);
            var pin4 = context.GetSpan<T4>(_node.GetPin(4).MemoryReference);
            var pin5 = context.GetSpan<T5>(_node.GetPin(5).MemoryReference);
            Update(in context, context.Indices.Length, pin0, pin1, pin2, pin3, pin4, pin5);
        }

        public abstract void Update(in UpdateContext context, int numParticles,
            SparseSpan<T0> pin0, SparseSpan<T1> pin1, SparseSpan<T2> pin2, SparseSpan<T3> pin3,
            SparseSpan<T4> pin4, SparseSpan<T5> pin5);
    }

    /// <summary>
    /// Get number of pins.
    /// </summary>
    public override int NumPins => NumberOfPins;

    /// <summary>
    /// Get pin by index.
    /// </summary>
    public override ParticleGraphPin GetPin(int index) => _pins[index];

    /// <summary>
    /// Pins
    /// </summary>
    protected readonly ParticleGraphPin[] _pins;
}
