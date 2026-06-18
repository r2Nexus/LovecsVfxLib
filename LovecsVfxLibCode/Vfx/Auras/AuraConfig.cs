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

    private Func<decimal>? _minPowerAmountProvider;
    private Func<decimal>? _maxPowerAmountProvider;

    public Dictionary<string, VfxSlotValue> Slots { get; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Incremented whenever slot/config values change, so aura views can re-apply slots only when needed.
    /// </summary>
    public int Version { get; set; }

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

    public void SetSpriteSheet(
        string slotName,
        string texturePath,
        int hFrames,
        int vFrames,
        bool loop = false,
        float? animSpeedMin = null,
        float? animSpeedMax = null)
    {
        Set(slotName, VfxSlotValue.FromSpriteSheet(
            texturePath,
            hFrames,
            vFrames,
            loop,
            animSpeedMin,
            animSpeedMax));
    }

    public AuraConfig SetPowerAmountRange(decimal minPowerAmount, decimal maxPowerAmount)
        => SetPowerAmountRange(() => minPowerAmount, () => maxPowerAmount);

    public AuraConfig SetPowerAmountRange(Func<decimal> minPowerAmountProvider, Func<decimal> maxPowerAmountProvider)
    {
        _minPowerAmountProvider = minPowerAmountProvider;
        _maxPowerAmountProvider = maxPowerAmountProvider;
        Version++;
        return this;
    }

    public AuraConfig ClearPowerAmountRange()
    {
        _minPowerAmountProvider = null;
        _maxPowerAmountProvider = null;
        Version++;
        return this;
    }

    public decimal? TryGetMinPowerAmount()
    {
        if (_minPowerAmountProvider == null)
            return null;

        try
        {
            return _minPowerAmountProvider();
        }
        catch (Exception e)
        {
            GD.PushWarning($"[AuraConfig] Min power amount provider failed: {e.Message}");
            return null;
        }
    }

    public decimal? TryGetMaxPowerAmount()
    {
        if (_maxPowerAmountProvider == null)
            return null;

        try
        {
            return _maxPowerAmountProvider();
        }
        catch (Exception e)
        {
            GD.PushWarning($"[AuraConfig] Max power amount provider failed: {e.Message}");
            return null;
        }
    }
}