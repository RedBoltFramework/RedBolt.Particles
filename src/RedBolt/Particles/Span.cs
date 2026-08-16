// Copyright (c) 2026 RedBolt and contributors.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT> or the accompanying LICENSE file.

namespace RedBolt.Particles;

using System;

public enum ParticleGraphContainerType
{
    Span,
    Sparse,
    Scalar,
    Auto
}

public ref struct SparseSpan<T>
{
    public SparseSpan(Span<T> data, Span<int> indices)
    {
        Data = data;
        Indices = indices;
    }

    public SparseSpan(T[] data, int[] indices)
    {
        Data = data;
        Indices = indices;
    }

    public ref T this[int index] => ref Data[Indices[index]];

    public Span<T> Data;
    public Span<int> Indices;
}
