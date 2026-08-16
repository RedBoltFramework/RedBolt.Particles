// Copyright (c) 2026 RedBolt and contributors.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT> or the accompanying LICENSE file.

namespace RedBolt.Particles.ParticleGraphNodes;

using System;
using RedBolt.Core;
using RedBolt.Math;

[Reflected]
public partial class Circle : TemplateNode<CircleInstance, Vector3f, Vector3f>
{
    /// <summary>Construct Circle.</summary>
    public Circle() : base([
        new ParticleGraphPin(ParticleGraphPinFlag.Output, "position", ParticleGraphContainerType.Span),
        new ParticleGraphPin(ParticleGraphPinFlag.Output, "velocity", ParticleGraphContainerType.Span)])
    {
        Rotation = new Quaternion();
    }
    public override int EvaluateInstanceSize() => IntPtr.Size;
    public override ParticleGraphNodeInstance CreateInstance(ParticleGraphLayerInstance layer)
    { var instance = new CircleInstance(); instance.Init(this, layer); return instance; }

    [Serialized("Radius")] public float Radius { get; set; }
    [Serialized("Radius Thickness")] public float RadiusThickness { get; set; }
    [Serialized("Translation")] public Vector3f Translation { get; set; }
    private static object? GetRotationDefault() => new Quaternion();

    [Serialized("Rotation", DefaultSource = nameof(GetRotationDefault))] public Quaternion Rotation { get; set; }
    [Serialized("Scale")] public Vector3f Scale { get; set; }
}

public sealed class CircleInstance : Circle.InstanceBase
{
    public override void Update(in UpdateContext context, int numParticles,
        SparseSpan<Vector3f> position, SparseSpan<Vector3f> velocity)
    {
        var circle = (Circle)GetGraphNode();
        var transform = new Matrix3x4(circle.Translation, circle.Rotation, circle.Scale);
        var directionTransform = transform.RotationMatrix;
        for (var i = 0; i < numParticles; ++i)
        {
            Generate(out var localPosition, out var localVelocity);
            position[i] = transform * localPosition;
            velocity[i] = directionTransform * localVelocity;
        }
    }

    public void Generate(out Vector3f position, out Vector3f velocity)
    {
        var circle = (Circle)GetGraphNode();
        var angle = MathDefs.Random(360.0f);
        var cosine = MathDefs.Cos(angle);
        var sine = MathDefs.Sin(angle);
        var direction = new Vector3f(cosine, sine, 0.0f);
        var radius = circle.Radius;
        if (circle.RadiusThickness > 0.0f)
            radius *= 1.0f - MathDefs.Random() * circle.RadiusThickness;
        velocity = direction;
        position = new Vector3f(cosine * radius, sine * radius, 0.0f);
    }
}
