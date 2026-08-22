// Copyright (c) 2026 RedBolt and contributors.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT> or the accompanying LICENSE file.

namespace RedBolt.Particles;

using System;
using System.Collections.Generic;

using RedBolt.Container;
using RedBolt.Core;

/// <summary>
/// Particle graph effect definition.
/// </summary>
public class ParticleGraphSystem : RedObject
{
    public ParticleGraphSystem()
        : this(Context.Instance)
    {
    }

    private ParticleGraphSystem(Context context)
    {
        RegisterParticleGraphLibrary(context, this);
    }

    public override void Dispose()
    {
        _factories.Clear();
        base.Dispose();
    }

    public ParticleGraphNode? CreateObject(StringHash type)
    {
        return _factories.TryGetValue(type, out var factory) ? factory() : null;
    }

    public void Register<T>() where T : ParticleGraphNode, new()
    {
        _factories.Add(typeof(T).GetTypeId(), static () => new T());
    }

    /// <summary>
    /// Return the registered particle graph system or register a default system.
    /// </summary>
    public static ParticleGraphSystem GetOrCreate(Context context)
    {
        ArgumentNullException.ThrowIfNull(context);

        lock (SystemLock)
        {
            var system = context.GetSubsystem<ParticleGraphSystem>();
            if (system is not null)
                return system;

            system = new ParticleGraphSystem(context);
            context.RegisterSubsystem(system);
            return system;
        }
    }

    /// <summary>
    /// Register Particle Graph library objects.
    /// </summary>
    public static void RegisterParticleGraphLibrary(Context context, ParticleGraphSystem system)
    {
        context.RegisterAssembly(typeof(ParticleGraphEffect).Assembly);
        ParticleGraphNodes.All.RegisterGraphNodes(system);
    }

    private readonly Dictionary<StringHash, Func<ParticleGraphNode>> _factories = [];
    private static readonly object SystemLock = new();
}
