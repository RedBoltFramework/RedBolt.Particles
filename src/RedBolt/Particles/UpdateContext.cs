// Copyright (c) 2026 RedBolt and contributors.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT> or the accompanying LICENSE file.

namespace RedBolt.Particles;

using System;

public struct UpdateContext
{
    /// <summary>
    /// Current frame time step.
    /// </summary>
    public float TimeStep;

    /// <summary>
    /// Time since emitter start.
    /// </summary>
    public float Time;

    public Memory<int> Indices;
    public ParticleGraphMemoryBuffer Attributes;
    public ParticleGraphMemoryBuffer TempBuffer;
    public ParticleGraphLayerInstance Layer;

    public readonly SparseSpan<T> GetSpan<T>(ParticleGraphPinRef pin)
    {
        switch (pin.Type)
        {
            case ParticleGraphContainerType.Span:
                return Layer.GetSpan<T>(pin.Index);
            case ParticleGraphContainerType.Scalar:
                return Layer.GetScalar<T>(pin.Index);
            case ParticleGraphContainerType.Sparse:
                return Layer.GetSparse<T>(pin.Index, Indices.Span);
            default:
                throw new InvalidOperationException("Invalid particle graph pin container type.");
        }
    }
}
