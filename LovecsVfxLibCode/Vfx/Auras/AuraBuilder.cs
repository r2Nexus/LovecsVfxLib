using Godot;
using MegaCrit.Sts2.Core.Models;

namespace LovecsVfxLibCode.Vfx.Auras;

/// <summary>
/// Fluent entry point for marker-based power auras.
/// The aura is applied immediately, and later Set(...) calls sync the already-spawned view.
/// </summary>
public sealed class AuraBuilder
{
    private readonly PowerAuraController _controller;

    public AuraConfig Config => _controller.Config;

    internal AuraBuilder(PowerModel power, string? scenePath = null)
    {
        var config = new AuraConfig { ScenePath = scenePath };

        _controller = new PowerAuraController(power, config);
        AuraCmd.Apply(_controller, scenePath);
    }

    public AuraBuilder Set(string slotName, VfxSlotValue value)
    {
        Config.Set(slotName, value);
        _controller.Sync();
        return this;
    }

    public AuraBuilder SetSpriteSheet(
        string slotName,
        string texturePath,
        int hFrames,
        int vFrames,
        bool loop = false)
    {
        Config.SetSpriteSheet(slotName, texturePath, hFrames, vFrames, loop);
        _controller.Sync();
        return this;
    }

    public AuraBuilder UsePowerIcon(string slotName = VfxSlots.Icon)
    {
        Texture2D? icon = AuraPowerUtil.TryGetPowerIcon(_controller.Power);
        if (icon != null)
            Set(slotName, icon);
        else
            GD.PushWarning($"[AuraBuilder] Could not find icon for power {_controller.Power.GetType().FullName}.");

        return this;
    }

    public AuraBuilder Offset(Vector2 offset)
    {
        Config.Offset = offset;
        _controller.Sync();
        return this;
    }

    public AuraBuilder WithKey(string auraKey)
    {
        Config.AuraKey = auraKey;
        _controller.Sync();
        return this;
    }
    
    public AuraBuilder SetPowerAmountRange(decimal minPowerAmount, decimal maxPowerAmount)
    {
        Config.SetPowerAmountRange(minPowerAmount, maxPowerAmount);
        _controller.Sync();
        return this;
    }

    public AuraBuilder SetPowerAmountRange(Func<decimal> minPowerAmountProvider, Func<decimal> maxPowerAmountProvider)
    {
        Config.SetPowerAmountRange(minPowerAmountProvider, maxPowerAmountProvider);
        _controller.Sync();
        return this;
    }

    public AuraBuilder LethalAt(decimal lethalAmount)
        => SetPowerAmountRange(lethalAmount, lethalAmount);

    public AuraBuilder LethalAt(Func<decimal> lethalAmountProvider)
        => SetPowerAmountRange(lethalAmountProvider, lethalAmountProvider);

    public AuraBuilder AsLethalAura(decimal lethalAmount)
        => LethalAt(lethalAmount);

    public AuraBuilder AsLethalAura(Func<decimal> lethalAmountProvider)
        => LethalAt(lethalAmountProvider);

    public AuraController Controller => _controller;
}
