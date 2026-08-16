// Copyright (c) 2026 RedBolt and contributors.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT> or the accompanying LICENSE file.

namespace RedBolt.Particles.ParticleGraphNodes;

using System;

using RedBolt.Core;
using RedBolt.Math;
using RedBolt.Resource;

/// <summary>Operation on attribute.</summary>
[Reflected(Factory = false)]
public abstract partial class Attribute : ParticleGraphNode
{
    protected Attribute()
    {
    }

    /// <summary>Set attribute name.</summary>
    public void SetAttributeName(string name) => SetPinName(0, name);
    /// <summary>Get attribute name.</summary>
    public string GetAttributeName() => GetPinName(0);
    /// <summary>Set attribute type.</summary>
    public virtual void SetAttributeType(VariantType valueType) => SetPinValueType(0, valueType);
    /// <summary>Get attribute type.</summary>
    public VariantType GetAttributeType() => GetPinValueType(0);
}

/// <summary>Get particle attribute value.</summary>
[Reflected]
public partial class GetAttribute : Attribute
{
    protected class Instance : ParticleGraphNodeInstance
    {
        public override void Update(ref UpdateContext context) { }
    }

    /// <summary>Construct.</summary>
    public GetAttribute()
    {
        _pins = [new ParticleGraphPin(ParticleGraphPinFlag.MutableName | ParticleGraphPinFlag.MutableType,
            "attr", VariantType.Float, ParticleGraphContainerType.Sparse)];
    }

    public override int NumPins => _pins.Length;
    public override ParticleGraphPin GetPin(int index) => _pins[index];
    public override int EvaluateInstanceSize() => IntPtr.Size;
    public override ParticleGraphNodeInstance CreateInstance(ParticleGraphLayerInstance layer) => new Instance();

    protected override ParticleGraphPin? LoadOutputPin(ParticleGraphReader reader, GraphOutPin pin)
    {
        SetPinName(0, pin.GetName());
        SetPinValueType(0, pin.GetType());
        return _pins[0];
    }

    protected readonly ParticleGraphPin[] _pins;
}

/// <summary>Set particle attribute value.</summary>
[Reflected]
public partial class SetAttribute : Attribute
{
    private class Instance : ParticleGraphNodeInstance
    {
        public Instance(SetAttribute node) => _node = node;

        public override void Update(ref UpdateContext context)
        {
            var pin0 = _node._pins[0];
            var pin1 = _node._pins[1];
            switch (pin0.ValueType)
            {
                case VariantType.Int: Copy<int>(in context, pin0, pin1); break;
                case VariantType.Int64: Copy<long>(in context, pin0, pin1); break;
                case VariantType.Bool: Copy<bool>(in context, pin0, pin1); break;
                case VariantType.Float: Copy<float>(in context, pin0, pin1); break;
                case VariantType.Double: Copy<double>(in context, pin0, pin1); break;
                case VariantType.Vector2: Copy<Vector2f>(in context, pin0, pin1); break;
                case VariantType.Vector3: Copy<Vector3f>(in context, pin0, pin1); break;
                case VariantType.Vector4: Copy<Vector4f>(in context, pin0, pin1); break;
                case VariantType.Quaternion: Copy<Quaternion>(in context, pin0, pin1); break;
                case VariantType.Color: Copy<Color>(in context, pin0, pin1); break;
                case VariantType.String: Copy<string>(in context, pin0, pin1); break;
                case VariantType.Buffer: CopyBuffers(in context, pin0, pin1); break;
                case VariantType.ResourceRef: Copy<ResourceRef>(in context, pin0, pin1); break;
                case VariantType.ResourceRefList: CopyResourceRefLists(in context, pin0, pin1); break;
                case VariantType.Vector2i: Copy<Vector2i>(in context, pin0, pin1); break;
                case VariantType.Vector3i: Copy<Vector3i>(in context, pin0, pin1); break;
                default: throw new InvalidOperationException("Unsupported particle attribute type.");
            }
        }

        private static void Copy<T>(in UpdateContext context, ParticleGraphPin destinationPin,
            ParticleGraphPin sourcePin)
        {
            var source = context.GetSpan<T>(sourcePin.MemoryReference);
            var destination = context.GetSpan<T>(destinationPin.MemoryReference);
            for (var i = 0; i < context.Indices.Length; ++i) destination[i] = source[i];
        }

        private static void CopyBuffers(in UpdateContext context, ParticleGraphPin destinationPin,
            ParticleGraphPin sourcePin)
        {
            var source = context.GetSpan<byte[]>(sourcePin.MemoryReference);
            var destination = context.GetSpan<byte[]>(destinationPin.MemoryReference);
            for (var i = 0; i < context.Indices.Length; ++i)
            {
                if (destination[i].Length != source[i].Length)
                    destination[i] = new byte[source[i].Length];
                source[i].CopyTo(destination[i], 0);
            }
        }

        private static void CopyResourceRefLists(in UpdateContext context, ParticleGraphPin destinationPin,
            ParticleGraphPin sourcePin)
        {
            var source = context.GetSpan<ResourceRefList>(sourcePin.MemoryReference);
            var destination = context.GetSpan<ResourceRefList>(destinationPin.MemoryReference);
            for (var i = 0; i < context.Indices.Length; ++i)
            {
                if (ReferenceEquals(destination[i].Names, source[i].Names))
                    continue;
                destination[i].Type = source[i].Type;
                destination[i].Names.Clear();
                destination[i].Names.AddRange(source[i].Names);
            }
        }

        protected readonly SetAttribute _node;
    }

    /// <summary>Construct.</summary>
    public SetAttribute()
    {
        _pins =
        [
            new ParticleGraphPin(ParticleGraphPinFlag.MutableName | ParticleGraphPinFlag.MutableType,
                "attr", VariantType.Float, ParticleGraphContainerType.Sparse),
            new ParticleGraphPin(ParticleGraphPinFlag.Input | ParticleGraphPinFlag.MutableType,
                "", VariantType.Float)
        ];
    }

    public override int NumPins => _pins.Length;
    public override ParticleGraphPin GetPin(int index) => _pins[index];
    public override int EvaluateInstanceSize() => IntPtr.Size;
    public override ParticleGraphNodeInstance CreateInstance(ParticleGraphLayerInstance layer) => new Instance(this);

    public override void SetAttributeType(VariantType valueType)
    {
        SetPinValueType(0, valueType);
        SetPinValueType(1, valueType);
    }

    protected override ParticleGraphPin? LoadOutputPin(ParticleGraphReader reader, GraphOutPin pin)
    {
        SetPinName(0, pin.GetName());
        SetPinValueType(0, pin.GetType());
        return _pins[0];
    }

    protected readonly ParticleGraphPin[] _pins;
}
