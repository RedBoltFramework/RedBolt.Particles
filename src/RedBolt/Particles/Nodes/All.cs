// Copyright (c) 2026 RedBolt and contributors.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT> or the accompanying LICENSE file.

namespace RedBolt.Particles.ParticleGraphNodes;

internal static class All
{
    internal static void RegisterGraphNodes(ParticleGraphSystem system)
    {
        system.Register<Add>();
        system.Register<Bounce>();
        system.Register<BurstTimer>();
        system.Register<Cone>();
        system.Register<Constant>();
        system.Register<Curve>();
        system.Register<Destroy>();
        system.Register<Divide>();
        system.Register<Emit>();
        system.Register<Expire>();
        system.Register<GetAttribute>();
        system.Register<GetUniform>();
        system.Register<Lerp>();
        system.Register<Make>();
        system.Register<Move>();
        system.Register<Multiply>();
        system.Register<Negate>();
        system.Register<Print>();
        system.Register<Random>();
        system.Register<RenderBillboard>();
        system.Register<SetAttribute>();
        system.Register<SetUniform>();
        system.Register<Slerp>();
        system.Register<Subtract>();
        system.Register<TimeStep>();
        system.Register<TimeStepScale>();
        system.Register<EffectTime>();
        system.Register<NormalizedEffectTime>();
        system.Register<RenderMesh>();
        system.Register<Sphere>();
        system.Register<Hemisphere>();
        system.Register<LimitVelocity>();
        system.Register<ApplyForce>();
        system.Register<Normalized>();
        system.Register<Length>();
        system.Register<Break>();
        system.Register<Circle>();
        system.Register<Box>();
        system.Register<Cast>();
        system.Register<Noise3D>();
        system.Register<CurlNoise3D>();
    }
}
