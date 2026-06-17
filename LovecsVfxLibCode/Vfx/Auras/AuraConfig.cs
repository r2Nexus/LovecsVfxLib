using Godot;

namespace LovecsVfxLibCode.Vfx.Auras;

/// <summary>
/// Runtime configuration for one aura instance.
/// This replaces the old "spec" idea with: scene + slot values + source defaults.
/// </summary>
public class AuraConfig
{
    public string? AuraKey { get; set; }
    public string? ScenePath { get; set; }
    public Vector2 Offset { get; set; } = Vector2.Zero;

    public Dictionary<string, VfxSlotValue> Slots { get; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Incremented whenever slot/config values change, so aura views can re-apply slots only when needed.
    /// </summary>
    public int Version { get; private set; }

    public AuraConfig Set(string slotName, VfxSlotValue value)
    {
        if (string.IsNullOrWhiteSpace(slotName))
        {
            GD.PushWarning("[AuraConfig] Ignored empty VFX slot name.");
            return this;
        }

        Slots[slotName] = value;
        Version++;
        return this;
    }

    public AuraConfig SetSpriteSheet(
        string slotName,
        string texturePath,
        int hFrames,
        int vFrames,
        bool loop = false)
        => Set(slotName, VfxSlotValue.FromSpriteSheet(texturePath, hFrames, vFrames, loop));
}

/// <summary>
/// Backwards-compatible shim for old call sites. New code should use AuraConfig/AuraBuilder.
/// </summary>
[Obsolete("Use AuraConfig / WithAura(...).Set(...) instead. AuraSpec is kept only as a compatibility shim.")]
public sealed class AuraSpec : AuraConfig
{
    public Texture2D? Icon
    {
        get => Slots.TryGetValue(VfxSlots.Icon, out var value) ? value.TryAsTexture() : null;
        init
        {
            if (value != null)
                Set(VfxSlots.Icon, value);
        }
    }

    public Color? Color
    {
        get => Slots.TryGetValue(VfxSlots.Tint, out var value) ? value.TryAsColor() : null;
        init
        {
            if (value.HasValue)
                Set(VfxSlots.Tint, value.Value);
        }
    }

    public float AmountScale { get; init; } = 0.05f;

    public static AuraSpec Default => new();

    public static AuraSpec WithColor(Color color, float amountScale = 0.05f)
        => new() { Color = color, AmountScale = amountScale };
}
