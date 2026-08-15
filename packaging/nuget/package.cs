#:project ../../../RedBolt/src/RedBolt/Build/Packaging/RedBolt.ExternalModule.Packaging.csproj

using System.Runtime.CompilerServices;
using RedBolt.ExternalModule.Packaging;

return ExternalModulePackager.Run(args, new ExternalModulePackageManifest
{
    Name = "RedBolt.Particles",
    BuildSteps =
    [
        new("Particles/RedBolt.Particles.csproj", Arguments: "-p:BuildProjectReferences=false")
    ],
    ManagedPackages =
    [
        new(
            "RedBolt.Particles",
            "Programmable particle graphs and emitters for RedBolt.",
            [new("build/bin/Release/linux-x64/core/RedBolt.Particles.dll", "lib/net10.0/RedBolt.Particles.dll")],
            [
                new("RedBolt.Core", PackageVersions.RedBolt)
            ])
    ],
    Consumer = new(
        "RedBolt.Particles",
        """
        using System;
        using RedBolt.Particles;
        Console.WriteLine(typeof(ParticleGraphSystem).FullName);
        """)
}, SourcePath());

static string SourcePath([CallerFilePath] string path = "") => path;
