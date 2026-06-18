using System;
using Godot;

namespace LovecsVfxLibCode.Vfx.Auras;

/// <summary>
/// Runtime adapter for tiny GDScript marker nodes.
/// The GDScript files provide reliable editor inspector fields; C# still performs all runtime work.
/// </summary>
public sealed class GdVfxMarkerAdapter : IVfxMarker
{
    private const int TextureTargetAuto = 0;
    private const int TextureTargetTexture = 1;
    private const int TextureTargetParticleTexture = 2;

    private const int ColorTargetModulate = 0;
    private const int ColorTargetSelfModulate = 1;
    private const int ColorTargetParticleProcessMaterialColor = 2;
    private const int ColorTargetLightColor = 3;

    private readonly Node _node;

    private GdVfxMarkerAdapter(Node node)
    {
        _node = node;
    }

    public string SlotName => GetString("slot_name");
    public bool Required => GetBool("required");

    public static bool TryCreate(Node node, out GdVfxMarkerAdapter marker)
    {
        marker = null!;

        if (!HasProperty(node, "slot_name"))
            return false;

        bool isKnownMarker =
            HasProperty(node, "texture_target") ||
            HasProperty(node, "color_target") ||
            HasProperty(node, "sprite_sheet_marker");

        if (!isKnownMarker)
            return false;

        marker = new GdVfxMarkerAdapter(node);
        return true;
    }

    public void Apply(VfxSlotValue value)
    {
        if (HasProperty(_node, "texture_target"))
        {
            ApplyTexture(value);
            return;
        }

        if (HasProperty(_node, "color_target"))
        {
            ApplyColor(value);
            return;
        }

        if (HasProperty(_node, "sprite_sheet_marker"))
        {
            ApplySpriteSheet(value);
            return;
        }

        GD.PushWarning($"[GdVfxMarkerAdapter] Unknown marker '{_node.Name}'.");
    }

    private void ApplyTexture(VfxSlotValue value)
    {
        Texture2D texture;
        try
        {
            texture = value.AsTexture();
        }
        catch (Exception e)
        {
            GD.PushWarning($"[GdVfxMarkerAdapter] Slot '{SlotName}' expected texture: {e.Message}");
            return;
        }

        Node? target = GetTargetOrNull();
        if (target == null)
        {
            GD.PushWarning($"[GdVfxMarkerAdapter] Texture marker '{_node.Name}' has no target.");
            return;
        }

        int textureTarget = GetInt("texture_target");

        switch (target)
        {
            case Sprite2D sprite when textureTarget is TextureTargetAuto or TextureTargetTexture:
                sprite.Texture = texture;
                return;

            case TextureRect rect when textureTarget is TextureTargetAuto or TextureTargetTexture:
                rect.Texture = texture;
                return;

            case GpuParticles2D particles when textureTarget is TextureTargetAuto or TextureTargetParticleTexture:
                particles.Texture = texture;
                return;
        }

        GD.PushWarning($"[GdVfxMarkerAdapter] Slot '{SlotName}' cannot apply texture to {target.GetType().Name}.");
    }

    private void ApplyColor(VfxSlotValue value)
    {
        Color color;
        try
        {
            color = value.AsColor();
        }
        catch (Exception e)
        {
            GD.PushWarning($"[GdVfxMarkerAdapter] Slot '{SlotName}' expected color: {e.Message}");
            return;
        }

        Node? target = GetTargetOrNull();
        if (target == null)
        {
            GD.PushWarning($"[GdVfxMarkerAdapter] Color marker '{_node.Name}' has no target.");
            return;
        }

        int colorTarget = GetInt("color_target");

        switch (target)
        {
            case CanvasItem canvas when colorTarget == ColorTargetModulate:
                canvas.Modulate = color;
                return;

            case CanvasItem canvas when colorTarget == ColorTargetSelfModulate:
                canvas.SelfModulate = color;
                return;

            case GpuParticles2D particles when colorTarget == ColorTargetParticleProcessMaterialColor:
                ApplyParticleProcessMaterialColor(particles, color);
                return;

            case Light2D light when colorTarget == ColorTargetLightColor:
                light.Color = color;
                return;
        }

        GD.PushWarning($"[GdVfxMarkerAdapter] Slot '{SlotName}' cannot apply color target {colorTarget} to {target.GetType().Name}.");
    }

    private static void ApplyParticleProcessMaterialColor(GpuParticles2D particles, Color color)
    {
        if (particles.ProcessMaterial is not ParticleProcessMaterial material)
        {
            GD.PushWarning($"[GdVfxMarkerAdapter] Particle target '{particles.Name}' has no ParticleProcessMaterial to tint.");
            return;
        }

        // Avoid mutating a shared sub-resource across scene instances.
        if (!material.ResourceLocalToScene)
        {
            material = (ParticleProcessMaterial)material.Duplicate();
            material.ResourceLocalToScene = true;
            particles.ProcessMaterial = material;
        }

        material.Color = color;
    }

    private void ApplySpriteSheet(VfxSlotValue value)
    {
        VfxSpriteSheet sheet;
        try
        {
            sheet = value.AsSpriteSheet();
        }
        catch (Exception e)
        {
            GD.PushWarning($"[GdVfxMarkerAdapter] Slot '{SlotName}' expected sprite sheet: {e.Message}");
            return;
        }

        Node? target = GetTargetOrNull();
        if (target is not GpuParticles2D particles)
        {
            string actual = target == null ? "null" : target.GetType().Name;
            GD.PushWarning($"[GdVfxMarkerAdapter] Slot '{SlotName}' sprite-sheet target must be GpuParticles2D. Actual: {actual}.");
            return;
        }

        particles.Texture = sheet.Texture;

        CanvasItemMaterial material;
        if (particles.Material is CanvasItemMaterial existing)
        {
            material = existing.ResourceLocalToScene
                ? existing
                : (CanvasItemMaterial)existing.Duplicate();
        }
        else
        {
            material = new CanvasItemMaterial();
        }

        material.ResourceLocalToScene = true;
        material.ParticlesAnimation = true;
        material.ParticlesAnimHFrames = Math.Max(1, sheet.HFrames);
        material.ParticlesAnimVFrames = Math.Max(1, sheet.VFrames);
        material.ParticlesAnimLoop = sheet.Loop;

        particles.Material = material;

        ApplyParticleAnimationSpeed(particles, sheet);
    }

    private Node? GetTargetOrNull()
    {
        NodePath targetPath = GetNodePath("target_path");

        if (!targetPath.IsEmpty)
            return _node.GetNodeOrNull(targetPath);

        return _node.GetParent();
    }

    private string GetString(string propertyName)
        => _node.Get(propertyName).AsString();

    private bool GetBool(string propertyName)
        => _node.Get(propertyName).AsBool();

    private int GetInt(string propertyName)
        => _node.Get(propertyName).AsInt32();

    private NodePath GetNodePath(string propertyName)
    {
        Variant value = _node.Get(propertyName);
        return value.VariantType == Variant.Type.Nil
            ? new NodePath("")
            : value.AsNodePath();
    }

    public static bool HasProperty(Node node, string propertyName)
    {
        foreach (Godot.Collections.Dictionary property in node.GetPropertyList())
        {
            if (!property.ContainsKey("name"))
                continue;

            if (property["name"].AsString() == propertyName)
                return true;
        }

        return false;
    }
    
    private static void ApplyParticleAnimationSpeed(GpuParticles2D particles, VfxSpriteSheet sheet)
    {
        if (!sheet.AnimSpeedMin.HasValue && !sheet.AnimSpeedMax.HasValue)
            return;

        if (particles.ProcessMaterial is not ParticleProcessMaterial material)
        {
            GD.PushWarning($"[GdVfxMarkerAdapter] Particle target '{particles.Name}' has no ParticleProcessMaterial for animation speed.");
            return;
        }

        // Avoid mutating a shared sub-resource across scene instances.
        if (!material.ResourceLocalToScene)
        {
            material = (ParticleProcessMaterial)material.Duplicate();
            material.ResourceLocalToScene = true;
            particles.ProcessMaterial = material;
        }

        float min = sheet.AnimSpeedMin ?? sheet.AnimSpeedMax!.Value;
        float max = sheet.AnimSpeedMax ?? sheet.AnimSpeedMin!.Value;

        material.SetParamMin(ParticleProcessMaterial.Parameter.AnimSpeed, min);
        material.SetParamMax(ParticleProcessMaterial.Parameter.AnimSpeed, max);
    }
}
