// Copyright (c) 2026 RedBolt and contributors.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT> or the accompanying LICENSE file.

namespace RedBolt.Particles;

public enum EmitFrom
{
    Base,
    Volume,
    Surface,
    Edge,
    Vertex,
}

public static class Emitter
{
    private static readonly string?[] EmitFromNames = ["Base", "Volume", "Surface", "Edge", "Vertex", null];

    public static string?[] GetEmitFromNames()
    {
        return EmitFromNames;
    }
}
