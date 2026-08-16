// Copyright (c) 2026 RedBolt and contributors.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT> or the accompanying LICENSE file.

namespace RedBolt.Particles.ParticleGraphNodes;

using System;
using RedBolt.Core;
using RedBolt.Math;

[Reflected]
public partial class Box : TemplateNode<BoxInstance, Vector3f, Vector3f>
{
    /// <summary>Construct Box.</summary>
    public Box() : base([
        new ParticleGraphPin(ParticleGraphPinFlag.Output, "position", ParticleGraphContainerType.Span),
        new ParticleGraphPin(ParticleGraphPinFlag.Output, "velocity", ParticleGraphContainerType.Span)])
    {
        Rotation = new Quaternion();
    }
    public override int EvaluateInstanceSize() => IntPtr.Size;
    public override ParticleGraphNodeInstance CreateInstance(ParticleGraphLayerInstance layer)
    { var instance = new BoxInstance(); instance.Init(this, layer); return instance; }

    [Serialized("Box Thickness")] public Vector3f BoxThickness { get; set; }
    [Serialized("Translation")] public Vector3f Translation { get; set; }
    private static object? GetRotationDefault() => new Quaternion();

    [Serialized("Rotation", DefaultSource = nameof(GetRotationDefault))] public Quaternion Rotation { get; set; }
    [Serialized("Scale")] public Vector3f Scale { get; set; }
    [Serialized("From")] public int From { get; set; }
}

public sealed class BoxInstance : Box.InstanceBase
{
    public override void Update(in UpdateContext context, int numParticles,
        SparseSpan<Vector3f> position, SparseSpan<Vector3f> velocity)
    {
        var box = (Box)GetGraphNode();
        var transform = new Matrix3x4(box.Translation, box.Rotation, box.Scale);
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
        var box = (Box)GetGraphNode();
        position = default;
        switch ((EmitFrom)box.From)
        {
            case EmitFrom.Edge:
            {
                var x = MathDefs.Random(-1.0f, 1.0f);
                position = MathDefs.Random(12) switch
                {
                    0 => new Vector3f(x, -1.0f, -1.0f), 1 => new Vector3f(x, -1.0f, 1.0f),
                    2 => new Vector3f(x, 1.0f, -1.0f), 3 => new Vector3f(x, 1.0f, 1.0f),
                    4 => new Vector3f(-1.0f, x, -1.0f), 5 => new Vector3f(-1.0f, x, 1.0f),
                    6 => new Vector3f(1.0f, x, -1.0f), 7 => new Vector3f(1.0f, x, 1.0f),
                    8 => new Vector3f(-1.0f, -1.0f, x), 9 => new Vector3f(-1.0f, 1.0f, x),
                    10 => new Vector3f(1.0f, -1.0f, x), _ => new Vector3f(1.0f, 1.0f, x)
                };
                velocity = position.Normalized;
                break;
            }
            case EmitFrom.Surface:
            {
                var x = MathDefs.Random(-1.0f, 1.0f);
                var y = MathDefs.Random(-1.0f, 1.0f);
                position = MathDefs.Random(6) switch
                {
                    0 => new Vector3f(x, y, -1.0f), 1 => new Vector3f(x, y, 1.0f),
                    2 => new Vector3f(x, -1.0f, y), 3 => new Vector3f(x, 1.0f, y),
                    4 => new Vector3f(-1.0f, x, y), _ => new Vector3f(1.0f, x, y)
                };
                velocity = position.Normalized;
                break;
            }
            default:
                position = new Vector3f(MathDefs.Random(-1.0f, 1.0f), MathDefs.Random(-1.0f, 1.0f),
                    MathDefs.Random(-1.0f, 1.0f));
                velocity = position.Normalized;
                break;
        }
    }
}
