// Copyright (c) 2026 RedBolt and contributors.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT> or the accompanying LICENSE file.

namespace RedBolt.Particles;

using System;

using RedBolt.Graphics;
using RedBolt.Scene;

public abstract class ParticleGraphNodeInstance : IDisposable
{
    public ParticleGraphNodeInstance()
    {
    }

    public virtual void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public abstract void Update(ref UpdateContext context);

    /// <summary>
    /// Handle scene change in instance.
    /// </summary>
    public virtual void OnSceneSet(Scene? scene)
    {
    }

    /// <summary>
    /// Handle drawable attribute change.
    /// </summary>
    public virtual void UpdateDrawableAttributes()
    {
    }

    public virtual void Reset()
    {
    }

    /// <summary>
    /// Copy drawable attributes from emitter.
    /// </summary>
    protected void CopyDrawableAttributes(Drawable? drawable, ParticleGraphEmitter? emitter)
    {
        if (drawable is null || emitter is null)
            return;

        drawable.SetViewMask(emitter.ViewMask);
        drawable.SetLightMask(emitter.LightMask);
        drawable.SetShadowMask(emitter.ShadowMask);
        drawable.SetZoneMask(emitter.ZoneMask);
    }
}
