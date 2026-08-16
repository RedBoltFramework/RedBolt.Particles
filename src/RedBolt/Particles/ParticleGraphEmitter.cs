// Copyright (c) 2026 RedBolt and contributors.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT> or the accompanying LICENSE file.

namespace RedBolt.Particles;

using System.Collections.Generic;

using RedBolt.Core;
using RedBolt.Graphics;
using RedBolt.Resource;
using RedBolt.Scene;

/// <summary>
/// Particle graph emitter component.
/// </summary>
[Reflected(ObjectCategory.Category_Geometry)]
public partial class ParticleGraphEmitter : Component
{
    /// <summary>
    /// Construct.
    /// </summary>
    public ParticleGraphEmitter()
    {
    }

    /// <summary>
    /// Destruct.
    /// </summary>
    public override void Dispose()
    {
        ClearLayers();
        _effect = null;
        base.Dispose();
    }

    /// <summary>
    /// Handle enabled/disabled state change.
    /// </summary>
    public override void OnSetEnabled()
    {
        base.OnSetEnabled();

        var scene = Scene;
        if (scene is not null)
        {
            if (IsEnabledEffective)
                SubscribeToEvent<SceneEvents.ScenePostUpdateEvent>(scene, HandleScenePostUpdate);
            else
                UnsubscribeFromEvent<SceneEvents.ScenePostUpdateEvent>(scene);
        }
    }

    /// <summary>
    /// Set particle effect.
    /// </summary>
    public void SetEffect(ParticleGraphEffect? effect)
    {
        if (ReferenceEquals(effect, _effect))
            return;

        ClearLayers();

        // Unsubscribe from the reload event of previous effect (if any), then subscribe to the new
        if (_effect is not null)
            UnsubscribeFromEvent<ResourceEvents.ReloadFinished>(_effect);

        _effect = effect;

        if (_effect is not null)
            SubscribeToEvent<ResourceEvents.ReloadFinished>(_effect, HandleEffectReloadFinished);

        ApplyEffect();
    }

    /// <summary>
    /// Reset the particle emitter completely. Removes current particles, sets emitting state on, and resets the
    /// emission timer.
    /// </summary>
    public void Reset()
    {
        foreach (var layer in _layers)
            layer.Reset();
    }

    /// <summary>
    /// Apply not continuously updated values such as the material, the number of particles and sorting mode from the
    /// particle effect. Call this if you change the effect programmatically.
    /// </summary>
    public void ApplyEffect()
    {
        if (_effect is null)
            return;

        var numLayers = _effect.NumLayers;
        while (_layers.Count > numLayers)
        {
            _layers[^1].Dispose();
            _layers.RemoveAt(_layers.Count - 1);
        }
        while (_layers.Count < numLayers)
            _layers.Add(new ParticleGraphLayerInstance());

        for (var i = 0; i < numLayers; ++i)
        {
            _layers[i].SetEmitterFromParticleGraphEmitterFriend(this);
            _layers[i].Apply(_effect.GetLayer(i));
        }

        Reset();
    }

    /// <summary>
    /// Set whether should be emitting. If the state was changed, also resets the emission period timer.
    /// </summary>
    /// <remarks>Property.</remarks>
    public bool Emitting
    {
        get => _emitting;
        set
        {
            if (value != _emitting)
            {
                _emitting = value;

                // If stopping emission now, and there are active particles, send finish event once they are gone
                //sendFinishedEvent_ = enable || CheckActiveParticles();
                //periodTimer_ = 0.0f;
            }
        }
    }

    /// <summary>
    /// Remove all current particles.
    /// </summary>
    public void RemoveAllParticles()
    {
        for (var i = 0; i < _layers.Count; ++i)
            _layers[i].RemoveAllParticles();
    }

    /// <summary>
    /// Return particle effect.
    /// </summary>
    public ParticleGraphEffect? Effect => _effect;

    /// <summary>
    /// Set particles effect attribute.
    /// </summary>
    public void SetEffectAttr(ResourceRef value)
    {
        var cache = GetSubsystem<ResourceCache>();
        SetEffect(cache.GetResource<ParticleGraphEffect>(value.Name));
    }

    /// <summary>
    /// Set view mask. Is and'ed with camera's view mask to see if the object should be rendered.
    /// </summary>
    public uint ViewMask
    {
        get => _viewMask;
        set
        {
            if (_viewMask != value)
            {
                _viewMask = value;
                UpdateDrawables();
            }
        }
    }

    /// <summary>
    /// Set light mask. Is and'ed with light's and zone's light mask to see if the object should be lit.
    /// </summary>
    public uint LightMask
    {
        get => _lightMask;
        set
        {
            if (_lightMask != value)
            {
                _lightMask = value;
                UpdateDrawables();
            }
        }
    }

    /// <summary>
    /// Set shadow mask. Is and'ed with light's light mask and zone's shadow mask to see if the object should be
    /// rendered to a shadow map.
    /// </summary>
    public uint ShadowMask
    {
        get => _shadowMask;
        set
        {
            if (_shadowMask != value)
            {
                _shadowMask = value;
                UpdateDrawables();
            }
        }
    }

    /// <summary>
    /// Set zone mask. Is and'ed with zone's zone mask to see if the object should belong to the zone.
    /// </summary>
    public uint ZoneMask
    {
        get => _zoneMask;
        set
        {
            if (_zoneMask != value)
            {
                _zoneMask = value;
                UpdateDrawables();
            }
        }
    }

    /// <summary>
    /// Set particles effect attribute.
    /// </summary>
    public ResourceRef GetEffectAttr() => Resource.GetResourceRef(_effect, typeof(ParticleGraphEffect).GetTypeId());

    /// <summary>
    /// Create a new particle. Return true if there was room.
    /// </summary>
    public bool EmitNewParticle(int layer)
    {
        if (layer < 0 || layer >= _layers.Count)
            return false;

        _layers[layer].EmitNewParticles();
        return true;
    }

    /// <summary>
    /// Manually update emitter.
    /// </summary>
    public void Tick(float timeStep)
    {
        for (var i = 0; i < _layers.Count; ++i)
            _layers[i].Update(timeStep, _emitting);
    }

    /// <summary>
    /// Get layer by index.
    /// </summary>
    public ParticleGraphLayerInstance? GetLayer(int layer)
    {
        if (layer < 0 || layer >= _layers.Count)
            return null;
        return _layers[layer];
    }

    /// <summary>
    /// Return whether has active particles.
    /// </summary>
    public bool CheckActiveParticles()
    {
        for (var i = 0; i < _layers.Count; ++i)
        {
            if (_layers[i].NumActiveParticles > 0)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Handle scene being assigned.
    /// </summary>
    protected override void OnSceneSet(Scene? previousScene, Scene? scene)
    {
        if (scene is not null && IsEnabledEffective)
            SubscribeToEvent<SceneEvents.ScenePostUpdateEvent>(scene, HandleScenePostUpdate);
        else if (scene is null)
            UnsubscribeFromEvent<SceneEvents.ScenePostUpdateEvent>();

        for (var i = 0; i < _layers.Count; ++i)
            _layers[i].OnSceneSetFromParticleGraphEmitterFriend(scene);
    }

    /// <summary>
    /// Handle scene post-update event.
    /// </summary>
    private void HandleScenePostUpdate(ref SceneEvents.ScenePostUpdateEvent eventData)
    {
        // Store scene's timestep and use it instead of global timestep, as time scale may be other than 1
        _lastTimeStep = eventData.TimeStep;

        Tick(_lastTimeStep);

        //// If no invisible update, check that the billboardset is in view (framenumber has changed)
        //if ((effect_ && effect_->GetUpdateInvisible()) || viewFrameNumber_ != lastUpdateFrameNumber_)
        //{
        //    lastUpdateFrameNumber_ = viewFrameNumber_;
        //    needUpdate_ = true;
        //    MarkForUpdate();
        //}

        //// Send finished event only once all particles are gone
        //if (node_ && !emitting_ && sendFinishedEvent_ && !CheckActiveParticles())
        //{
        //    sendFinishedEvent_ = false;

        //    // Make a weak pointer to self to check for destruction during event handling
        //    WeakPtr<ParticleGraphEmitter> self(this);

        //    using namespace ParticleEffectFinished;

        //    VariantMap& eventData = GetEventDataMap();
        //    eventData[P_NODE] = node_;
        //    eventData[P_EFFECT] = effect_;

        //    node_->SendEvent(E_PARTICLEEFFECTFINISHED, eventData);

        //    if (self.Expired())
        //        return;

        //    DoAutoRemove(autoRemove_);
        //}
    }

    /// <summary>
    /// Handle live reload of the particle effect.
    /// </summary>
    private void HandleEffectReloadFinished(ref ResourceEvents.ReloadFinished eventData)
    {
        // When particle effect file is live-edited, remove existing particles and reapply the effect parameters
        ClearLayers();
        ApplyEffect();
    }

    /// <summary>
    /// Update all drawable attributes.
    /// </summary>
    private void UpdateDrawables()
    {
        foreach (var layer in _layers)
            layer.UpdateDrawables();
    }

    private void ClearLayers()
    {
        for (var index = _layers.Count - 1; index >= 0; --index)
            _layers[index].Dispose();
        _layers.Clear();
    }

    private static object? GetEnabledDefault() => true;
    private static object? GetEffectDefault() => new ResourceRef(typeof(ParticleGraphEffect).GetTypeId());
    private static object? GetViewMaskDefault() => Drawable.DefaultViewMask;
    private static object? GetLightMaskDefault() => Drawable.DefaultLightMask;
    private static object? GetShadowMaskDefault() => Drawable.DefaultShadowMask;
    private static object? GetZoneMaskDefault() => Drawable.DefaultZoneMask;

    [Serialized("Is Enabled", DefaultSource = nameof(GetEnabledDefault))]
    private bool SerializedEnabled
    {
        get => Enabled;
        set => SetEnabled(value);
    }

    [Serialized("Effect", DefaultSource = nameof(GetEffectDefault))]
    private ResourceRef SerializedEffect
    {
        get => GetEffectAttr();
        set => SetEffectAttr(value);
    }

    [Serialized("View Mask", DefaultSource = nameof(GetViewMaskDefault))]
    private uint SerializedViewMask
    {
        get => ViewMask;
        set => ViewMask = value;
    }

    [Serialized("Light Mask", DefaultSource = nameof(GetLightMaskDefault))]
    private uint SerializedLightMask
    {
        get => LightMask;
        set => LightMask = value;
    }

    [Serialized("Shadow Mask", DefaultSource = nameof(GetShadowMaskDefault))]
    private uint SerializedShadowMask
    {
        get => ShadowMask;
        set => ShadowMask = value;
    }

    [Serialized("Zone Mask", DefaultSource = nameof(GetZoneMaskDefault))]
    private uint SerializedZoneMask
    {
        get => ZoneMask;
        set => ZoneMask = value;
    }

    /// <summary>
    /// Particle effect.
    /// </summary>
    private ParticleGraphEffect? _effect;

    private readonly List<ParticleGraphLayerInstance> _layers = [];

    /// <summary>
    /// View mask.
    /// </summary>
    private uint _viewMask = Drawable.DefaultViewMask;

    /// <summary>
    /// Light mask.
    /// </summary>
    private uint _lightMask = Drawable.DefaultLightMask;

    /// <summary>
    /// Shadow mask.
    /// </summary>
    private uint _shadowMask = Drawable.DefaultShadowMask;

    /// <summary>
    /// Zone mask.
    /// </summary>
    private uint _zoneMask = Drawable.DefaultZoneMask;

    /// <summary>
    /// Last scene timestep.
    /// </summary>
    private float _lastTimeStep;

    /// <summary>
    /// Currently emitting flag.
    /// </summary>
    private bool _emitting = true;
}
