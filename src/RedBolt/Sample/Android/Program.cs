namespace RedBolt.Sample.Android;

internal static class SampleCatalog
{
    internal static readonly SampleEntry[] Entries =
    [
        new("particles", "Particles", static () => new global::RedBolt.Sample.Particles.ParticlesApplication()),
    ];
}
