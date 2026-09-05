// Copyright (c) 2026 RedBolt and contributors.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT> or the accompanying LICENSE file.

namespace RedBolt.Sample.Particles;

using System;

using RedBolt;
using RedBolt.Bindings.DearImGui;
using RedBolt.Core;
using RedBolt.DearImGui;
using RedBolt.Engine;
using RedBolt.Graphics;
using RedBolt.Input;
using RedBolt.Math;
using RedBolt.Particles;
using RedBolt.Particles.ParticleGraphNodes;
using RedBolt.Resource;
using RedBolt.Scene;

internal static class ParticlesProgram
{
    private static int Main()
    {
        try
        {
            return new ParticlesApplication().Run();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return 1;
        }
    }
}

sealed class ParticlesApplication : Application
{
    private Scene? _scene;
    private Node? _cameraNode;
    private Light? _fireLight;
    private ParticleGraphEffect? _graphEffect;
    private Viewport? _viewport;
    private Texture2D[]? _controlPrompts;
    private SampleInputMap? _sampleInput;
    private float _elapsedTime;
    private static readonly Vector2f PromptIconSize = new(24.0f, 24.0f);

    protected override void Setup(EngineParameters parameters)
    {
        parameters.ApplicationName = "RedBolt.Sample.Particles";
        parameters.Borderless = false;
        parameters.FullScreen = false;
        parameters.ResourcePaths = "CoreData;SampleData";
        parameters.ResourcePrefixPaths = ".;..";
        parameters.WindowMaximize = false;
#if !REDBOLT_PLATFORM_BROWSER
            parameters.WindowHeight = 720;
            parameters.WindowWidth = 1280;
#endif
        parameters.WindowTitle = "RedBolt Particles";
    }

    protected override void Start()
    {
        _sampleInput = new SampleInputMap(movement: true);
        Context.RegisterSubsystem(new DearImGui((ImGuiConfigFlags)Engine.Parameters.DearImGuiFlags));
        if (Context.GetSubsystem<ParticleGraphSystem>() is null)
            Context.RegisterSubsystem(new ParticleGraphSystem());
        SubscribeToEvent<DearImGuiEvents.EndRenderingDearImGuiEvent>(
            (ref DearImGuiEvents.EndRenderingDearImGuiEvent _) => DrawInstructions());

        _controlPrompts =
        [
            Resources.Get<Texture2D>("ThirdParty/KenneyInputPrompts/keyboard_w.png"),
            Resources.Get<Texture2D>("ThirdParty/KenneyInputPrompts/keyboard_a.png"),
            Resources.Get<Texture2D>("ThirdParty/KenneyInputPrompts/keyboard_s.png"),
            Resources.Get<Texture2D>("ThirdParty/KenneyInputPrompts/keyboard_d.png"),
            Resources.Get<Texture2D>("ThirdParty/KenneyInputPrompts/mouse_right.png")
        ];

        CreateScene();
        SetupViewport();

        Input.SetMouseMode(MouseMode.Free);
        Input.SetMouseVisible(true);
    }

    private void CreateScene()
    {
        _scene = new Scene();
        _scene.CreateComponent<Octree>();

        var zone = _scene.CreateChild("Zone").CreateComponent<Zone>();
        zone.SetBoundingBox(new BoundingBox(-100.0f, 100.0f));
        zone.SetAmbientColor(new Color(0.055f, 0.065f, 0.11f));
        zone.SetFogColor(new Color(0.008f, 0.014f, 0.035f));
        zone.SetFogStart(22.0f);
        zone.SetFogEnd(48.0f);

        var floorNode = _scene.CreateChild("Floor");
        floorNode.SetScale(new Vector3f(60.0f, 1.0f, 60.0f));
        var floor = floorNode.CreateComponent<StaticModel>();
        floor.SetModel(Resources.Get<Model>("Models/Plane.mdl"));
        var floorMaterial = Resources.Get<Material>(
            "ThirdParty/KenneyNature/grass.glb.d/Materials/grass_Lit.xml").Clone("MoonlitGrass");
        floorMaterial.SetShaderParameter("MatDiffColor", new Vector4f(0.035f, 0.085f, 0.065f, 1.0f));
        floor.SetMaterial(floorMaterial);
        floor.SetCastShadows(false);

        // A ring of irregular stones and real log meshes gives the effect a recognizable, intimate setting.
        var fireplace = _scene.CreateChild("Fireplace");
        fireplace.SetPosition(new Vector3f(0.0f, 0.0f, 1.2f));
        CreateStoneRing(fireplace);
        CreateLogStack(fireplace);

        _graphEffect = CreateCoalBedEffect();
        var coalBed = fireplace.CreateChild("GraphCoalBed");
        coalBed.SetPosition(new Vector3f(0.0f, 0.46f, 0.0f));
        coalBed.CreateComponent<ParticleGraphEmitter>().SetEffect(_graphEffect);

        CreateEmitter(fireplace, "AnimatedFlames", new Vector3f(0.0f, 0.54f, 0.0f), 1.0f,
            Resources.Get<ParticleEffect>("Particle/FireplaceFire.xml"));
        CreateEmitter(fireplace, "DriftingSmoke", new Vector3f(0.0f, 0.78f, 0.0f), 1.0f,
            Resources.Get<ParticleEffect>("Particle/FireplaceSmoke.xml"));
        CreateEmitter(fireplace, "RisingEmbers", new Vector3f(0.0f, 0.62f, 0.0f), 1.0f,
            Resources.Get<ParticleEffect>("Particle/FireplaceEmbers.xml"));

        _fireLight = CreatePointLight(_scene, "FireLight", new Vector3f(0.0f, 1.65f, 1.2f), 15.0f, 3.4f,
            new Color(1.0f, 0.43f, 0.10f), true);
        CreateMoonlight();
        CreateMoon();
        CreateForestSilhouette();

        _cameraNode = _scene.CreateChild("Camera");
        _cameraNode.CreateComponent<FreeFlyController>().SetInputMap(_sampleInput!.Map);
        var camera = _cameraNode.CreateComponent<Camera>();
        camera.SetFarClip(60.0f);
        _cameraNode.SetPosition(new Vector3f(3.8f, 2.45f, -4.9f));
        _cameraNode.LookAt(new Vector3f(0.0f, 1.25f, 1.25f));
    }

    private void CreateStoneRing(Node fireplace)
    {
        var model = Resources.Get<Model>(
            "ThirdParty/KenneyNature/rock_smallA.glb.d/Models/Mesh rock_smallA.mdl");
        Material[] materials =
        [
            Resources.Get<Material>(
                "ThirdParty/KenneyNature/rock_smallA.glb.d/Materials/grass_Lit.xml"),
            Resources.Get<Material>(
                "ThirdParty/KenneyNature/rock_smallA.glb.d/Materials/dirt_Lit.xml")
        ];
        const int numStones = 13;
        for (var i = 0; i < numStones; ++i)
        {
            var angle = i * 360.0f / numStones;
            var radius = 1.16f + 0.06f * MathDefs.Sin(angle * 3.0f);
            var stone = CreateModel(fireplace, $"HearthStone{i}",
                new Vector3f(MathDefs.Sin(angle) * radius, 0.12f, MathDefs.Cos(angle) * radius),
                0.72f + 0.08f * (i % 3), model, materials);
            stone.SetRotation(new Quaternion(5.0f * (i % 2), angle + 18.0f, 3.0f * (i % 3 - 1)));
        }
    }

    private void CreateLogStack(Node fireplace)
    {
        var model = Resources.Get<Model>("ThirdParty/KenneyNature/log.glb.d/Models/Mesh log.mdl");
        Material[] materials =
        [
            Resources.Get<Material>("ThirdParty/KenneyNature/log.glb.d/Materials/woodBark_Lit.xml"),
            Resources.Get<Material>("ThirdParty/KenneyNature/log.glb.d/Materials/woodInner_Lit.xml")
        ];
        var first = CreateModel(fireplace, "LogLowerA", new Vector3f(0.0f, 0.30f, 0.0f), 1.2f,
            model, materials);
        first.SetRotation(new Quaternion(0.0f, 48.0f, 0.0f));
        var second = CreateModel(fireplace, "LogLowerB", new Vector3f(0.0f, 0.34f, 0.0f), 1.2f,
            model, materials);
        second.SetRotation(new Quaternion(0.0f, 132.0f, 0.0f));
    }

    private void CreateMoonlight()
    {
        var node = _scene!.CreateChild("Moonlight");
        node.SetDirection(new Vector3f(0.35f, -0.8f, 0.45f));
        var light = node.CreateComponent<Light>();
        light.SetLightType(LightType.Directional);
        light.SetBrightness(0.52f);
        light.SetColor(new Color(0.28f, 0.40f, 0.72f));
        light.SetCastShadows(true);
        light.SetShadowBias(new BiasParameters(0.0004f, 0.75f));
    }

    private void CreateMoon()
    {
        var material = Resources.Get<Material>("Materials/Constant/UnlitWhite.xml").Clone("MoonGlow");
        material.SetShaderParameter("MatDiffColor", new Vector4f(0.72f, 0.82f, 1.0f, 1.0f));
        CreateModel(_scene!, "Moon", new Vector3f(-4.5f, 7.5f, 11.0f), 0.72f,
            Resources.Get<Model>("Models/Sphere.mdl"), [material]);
    }

    private void CreateForestSilhouette()
    {
        var model = Resources.Get<Model>(
            "ThirdParty/KenneyNature/tree_fat.glb.d/Models/Mesh tree_fat.mdl");
        Material[] materials =
        [
            Resources.Get<Material>(
                "ThirdParty/KenneyNature/tree_fat.glb.d/Materials/leafsGreen_Lit.xml"),
            Resources.Get<Material>(
                "ThirdParty/KenneyNature/tree_fat.glb.d/Materials/woodBark_Lit.xml")
        ];
        Vector3f[] positions =
        [
            new(-11.0f, 0.0f, 10.0f), new(-7.0f, 0.0f, 16.0f), new(8.5f, 0.0f, 14.0f),
            new(13.0f, 0.0f, 8.0f), new(-14.0f, 0.0f, 2.0f), new(15.0f, 0.0f, 0.0f)
        ];
        for (var i = 0; i < positions.Length; ++i)
        {
            var tree = CreateModel(_scene!, $"DistantTree{i}", positions[i], 2.6f + 0.3f * (i % 3),
                model, materials);
            tree.SetRotation(new Quaternion(0.0f, i * 47.0f, 0.0f));
        }
    }

    private static Light CreatePointLight(Scene scene, string name, in Vector3f position, float range,
        float brightness, in Color color, bool castShadows)
    {
        var node = scene.CreateChild(name);
        node.SetPosition(position);
        var light = node.CreateComponent<Light>();
        light.SetLightType(LightType.Point);
        light.SetRange(range);
        light.SetBrightness(brightness);
        light.SetColor(color);
        light.SetCastShadows(castShadows);
        light.SetShadowBias(new BiasParameters(0.0005f, 1.0f));
        return light;
    }

    private static void CreateEmitter(
        Node parent, string name, in Vector3f position, float scale, ParticleEffect effect)
    {
        var node = parent.CreateChild(name);
        node.SetPosition(position);
        node.SetScale(scale);
        node.CreateComponent<ParticleEmitter>().SetEffect(effect);
    }

    private static ParticleGraphEffect CreateCoalBedEffect()
    {
        const int numCoals = 18;
        var effect = new ParticleGraphEffect();
        effect.SetNumLayers(numCoals);
        for (var i = 0; i < numCoals; ++i)
        {
            var layer = effect.GetLayer(i);
            layer.Capacity = 1;

            var emit = new Emit();
            emit.SetPinSource(0, AddConstant(layer.EmitGraph, 1.0f));
            layer.EmitGraph.Add(emit);

            var angle = i * 137.5f;
            var radius = 0.12f + 0.55f * (i % 7) / 6.0f;
            AddSetAttribute(layer.InitGraph, "position", VariantType.Vector3,
                AddConstant(layer.InitGraph, new Vector3f(
                    MathDefs.Sin(angle) * radius, 0.02f * (i % 3), MathDefs.Cos(angle) * radius)));

            var position = new GetAttribute();
            position.SetAttributeName("position");
            position.SetAttributeType(VariantType.Vector3);
            var render = new RenderBillboard
            {
                Material = new ResourceRef(typeof(Material).GetTypeId(), "Materials/Fireplace/Ember.xml"),
                Rows = 1,
                Columns = 1
            };
            render.SetPinSource(0, layer.UpdateGraph.Add(position));
            render.SetPinSource(1, AddConstant(layer.UpdateGraph,
                new Vector2f(0.04f + 0.012f * (i % 4), 0.04f + 0.012f * (i % 4))));
            render.SetPinSource(2, AddConstant(layer.UpdateGraph, 0.0f));
            render.SetPinSource(3, AddConstant(layer.UpdateGraph, i % 3 == 0
                ? new Color(1.0f, 0.88f, 0.32f)
                : new Color(1.0f, 0.24f, 0.015f)));
            render.SetPinSource(4, AddConstant(layer.UpdateGraph, 0.0f));
            render.SetPinSource(5, AddConstant(layer.UpdateGraph, Vector3f.Up));
            layer.UpdateGraph.Add(render);
        }
        return effect;
    }

    private static int AddConstant(ParticleGraph graph, float value)
    {
        using var variant = new Variant(value);
        return AddConstant(graph, variant);
    }

    private static int AddConstant(ParticleGraph graph, Vector2f value)
    {
        using var variant = new Variant(value);
        return AddConstant(graph, variant);
    }

    private static int AddConstant(ParticleGraph graph, Vector3f value)
    {
        using var variant = new Variant(value);
        return AddConstant(graph, variant);
    }

    private static int AddConstant(ParticleGraph graph, Color value)
    {
        using var variant = new Variant(value);
        return AddConstant(graph, variant);
    }

    private static int AddConstant(ParticleGraph graph, in Variant value)
    {
        var constant = new Constant();
        constant.SetValue(value);
        return graph.Add(constant);
    }

    private static void AddSetAttribute(ParticleGraph graph, string name, VariantType type, int sourceNode)
    {
        var attribute = new SetAttribute();
        attribute.SetAttributeName(name);
        attribute.SetAttributeType(type);
        attribute.SetPinSource(1, sourceNode);
        graph.Add(attribute);
    }

    private static Node CreateModel(Node parent, string name, in Vector3f position, float scale,
        Model model, Material[] materials)
    {
        var node = parent.CreateChild(name);
        node.SetPosition(position);
        node.SetScale(scale);
        var drawable = node.CreateComponent<StaticModel>();
        drawable.SetModel(model);
        for (var i = 0; i < materials.Length; ++i)
            drawable.SetMaterial(i, materials[i]);
        drawable.SetCastShadows(true);
        return node;
    }

    private void SetupViewport()
    {
        _viewport = new Viewport(_scene!, _cameraNode!.GetComponent<Camera>()!);
        Renderer.SetViewport(0, _viewport);
    }

    protected override void Update(float timeStep)
    {
        if (Input.ExitRequested || _sampleInput!.Pressed("Exit"))
        {
            Exit();
            return;
        }

        _elapsedTime += timeStep;
        if (_fireLight is not null)
        {
            // Layered, low-frequency oscillation feels alive without the harsh strobing of random flicker.
            var flicker = MathDefs.Sin(_elapsedTime * 5.1f) * 0.20f
                + MathDefs.Sin(_elapsedTime * 8.7f + 1.4f) * 0.11f
                + MathDefs.Sin(_elapsedTime * 13.3f + 0.6f) * 0.06f;
            _fireLight.SetBrightness(3.35f + flicker);
        }
    }

    private void DrawInstructions()
    {
        var windowOpen = true;
        ImGui.SetNextWindowPos(new Vector2f(10.0f, 10.0f), ImGuiCond.ImGuiCondAlways);
        if (ImGui.Begin("Night Fireplace"u8, ref windowOpen,
            ImGuiWindowFlags.ImGuiWindowFlagsNoResize | ImGuiWindowFlags.ImGuiWindowFlagsNoSavedSettings))
        {
            ImGui.Image(SampleInputMap.GetMovementPrompt(), PromptIconSize);
            ImGui.SameLine();
            ImGui.TextUnformatted("Move"u8);
            ImGui.Image(SampleInputMap.GetLookPrompt(), PromptIconSize);
            ImGui.SameLine();
            ImGui.TextUnformatted("Look"u8);
        }
        ImGui.End();
    }

    protected override void Stop()
    {
        _sampleInput?.Dispose();
        _sampleInput = null;
        DisposeScene();
        base.Stop();
    }

    public override void Dispose()
    {
        DisposeScene();
        base.Dispose();
    }

    private void DisposeScene()
    {
        if (_scene is not null)
            Renderer.SetNumViewports(0);
        _viewport?.Dispose();
        _viewport = null;
        _fireLight = null;
        _cameraNode = null;
        _scene?.Dispose();
        _scene = null;
        _graphEffect?.Dispose();
        _graphEffect = null;
    }
}
