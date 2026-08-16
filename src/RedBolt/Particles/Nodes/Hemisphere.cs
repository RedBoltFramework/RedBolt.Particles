// Copyright (c) 2026 RedBolt and contributors.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT> or the accompanying LICENSE file.

namespace RedBolt.Particles.ParticleGraphNodes;

using System;
using RedBolt.Core;
using RedBolt.Math;

[Reflected]
public partial class Hemisphere : TemplateNode<HemisphereInstance, Vector3f, Vector3f>
{
    /// <summary>Construct Hemisphere.</summary>
    public Hemisphere() : base([
        new ParticleGraphPin(ParticleGraphPinFlag.Output, "position", ParticleGraphContainerType.Span),
        new ParticleGraphPin(ParticleGraphPinFlag.Output, "velocity", ParticleGraphContainerType.Span)])
    {
        Rotation = new Quaternion();
    }
    public override int EvaluateInstanceSize() => IntPtr.Size;
    public override ParticleGraphNodeInstance CreateInstance(ParticleGraphLayerInstance layer)
    { var instance = new HemisphereInstance(); instance.Init(this, layer); return instance; }

    [Serialized("Radius")] public float Radius { get; set; }
    [Serialized("Radius Thickness")] public float RadiusThickness { get; set; }
    [Serialized("Translation")] public Vector3f Translation { get; set; }
    private static object? GetRotationDefault() => new Quaternion();

    [Serialized("Rotation", DefaultSource = nameof(GetRotationDefault))] public Quaternion Rotation { get; set; }
    [Serialized("Scale")] public Vector3f Scale { get; set; }
    [Serialized("From")] public int From { get; set; }
}

public sealed class HemisphereInstance : Hemisphere.InstanceBase
{
    public override void Update(in UpdateContext context, int numParticles,
        SparseSpan<Vector3f> position, SparseSpan<Vector3f> velocity)
    {
        var hemisphere = (Hemisphere)GetGraphNode();
        var transform = new Matrix3x4(hemisphere.Translation, hemisphere.Rotation, hemisphere.Scale);
        var directionTransform = transform.Matrix3;
        for (var i = 0; i < numParticles; ++i)
        {
            Generate(out var localPosition, out var localVelocity);
            position[i] = transform * localPosition;
            velocity[i] = directionTransform * localVelocity;
        }
    }

    public void Generate(out Vector3f position, out Vector3f velocity)
    {
        var hemisphere = (Hemisphere)GetGraphNode();
        var direction = new Vector3f(MathDefs.Random(2.0f) - 1.0f,
            MathDefs.Random(2.0f) - 1.0f, MathDefs.Random(2.0f) - 1.0f);
        direction.Normalize();
        direction.Z = MathF.Abs(direction.Z);
        var radius = hemisphere.Radius;
        if (hemisphere.RadiusThickness > 0.0f && (EmitFrom)hemisphere.From != EmitFrom.Surface)
            radius *= 1.0f - MathDefs.Random() * hemisphere.RadiusThickness;
        switch ((EmitFrom)hemisphere.From)
        {
            case EmitFrom.Base:
                velocity = direction;
                position = Vector3f.Zero;
                break;
            case EmitFrom.Surface:
                velocity = direction;
                position = direction * hemisphere.Radius;
                break;
            default:
                velocity = direction;
                position = direction * hemisphere.Radius * MathDefs.Pow(MathDefs.Random(), 1.0f / 3.0f) * 0.5f;
                break;
        }
    }
}
