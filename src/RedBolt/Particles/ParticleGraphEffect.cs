// Copyright (c) 2026 RedBolt and contributors.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT> or the accompanying LICENSE file.

namespace RedBolt.Particles;

using System.Collections.Generic;
using System.IO;

using RedBolt.Core;
using RedBolt.IO;
using RedBolt.Resource;

/// <summary>
/// Particle graph effect definition.
/// </summary>
[Reflected]
public partial class ParticleGraphEffect : Resource
{
    /// <summary>
    /// Construct.
    /// </summary>
    public ParticleGraphEffect()
    {
    }

    /// <summary>
    /// Destruct.
    /// </summary>
    public override void Dispose()
    {
        for (var index = _layers.Count - 1; index >= 0; --index)
            _layers[index].Dispose();
        _layers.Clear();
        base.Dispose();
    }

    /// <summary>
    /// Set number of layers.
    /// </summary>
    public void SetNumLayers(int numLayers)
    {
        while (numLayers < _layers.Count)
        {
            _layers[^1].Dispose();
            _layers.RemoveAt(_layers.Count - 1);
        }
        _layers.Capacity = global::System.Math.Max(_layers.Capacity, numLayers);
        while (numLayers > _layers.Count)
            _layers.Add(new ParticleGraphLayer());
    }

    /// <summary>
    /// Get number of layers.
    /// </summary>
    public int NumLayers => _layers.Count;

    /// <summary>
    /// Get layer by index.
    /// </summary>
    public ParticleGraphLayer GetLayer(int layerIndex) => _layers[layerIndex];

    /// <summary>
    /// Load resource from stream. May be called from a worker thread. Return true if successful.
    /// </summary>
    public override bool BeginLoad(Deserializer source)
    {
        var extension = Path.GetExtension(source.Name);

        ResetToDefaults();

        using var xmlFile = new XMLFile();
        if (!xmlFile.Load(source))
            return false;

        return xmlFile.LoadObject("particleGraphEffect", this);
    }

    /// <summary>
    /// Save resource. Return true if successful.
    /// </summary>
    public override bool Save(Serializer destination)
    {
        using var xmlFile = new XMLFile();
        xmlFile.SaveObject("particleGraphEffect", this);
        xmlFile.Save(destination);
        return true;
    }

    /// <summary>
    /// Serialize from/to archive. Return true if successful.
    /// </summary>
    public override void SerializeInBlock(Archive archive)
    {
        var loading = archive.IsInput;
        var size = loading ? 0 : _layers.Count;
        using var block = archive.OpenArrayBlock("layers", size);
        if (loading)
        {
            for (var index = _layers.Count - 1; index >= 0; --index)
                _layers[index].Dispose();
            _layers.Clear();
            size = block.SizeHint;
            _layers.Capacity = global::System.Math.Max(_layers.Capacity, size);
            for (var index = 0; index < size; ++index)
            {
                using var itemBlock = archive.OpenUnorderedBlock("layer");
                var value = new ParticleGraphLayer();
                _layers.Add(value);
                value.SerializeInBlock(archive);
            }
        }
        else
        {
            foreach (var value in _layers)
            {
                using var itemBlock = archive.OpenUnorderedBlock("layer");
                value.SerializeInBlock(archive);
            }
        }
    }

    /// <summary>
    /// Reset to defaults.
    /// </summary>
    private void ResetToDefaults()
    {
        // Needs to be a no-op when async loading, as this does a GetResource() which is not allowed from worker threads
        if (!WorkQueue.IsMainThread())
            return;

        for (var index = _layers.Count - 1; index >= 0; --index)
            _layers[index].Dispose();
        _layers.Clear();
    }

    /// <summary>
    /// Effect layers.
    /// </summary>
    private readonly List<ParticleGraphLayer> _layers = [];
}
