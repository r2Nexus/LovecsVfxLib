using Godot;
using MegaCrit.Sts2.Core.Models;

namespace LovecsVfxLibCode.Vfx.Auras;

public sealed class AuraBuilder
{
    private readonly PowerAuraController _controller;
    private bool _syncSuspended;

    public AuraConfig Config => _controller.Config;
    public AuraController Controller => _controller;

    internal AuraBuilder(PowerModel power, string? scenePath = null, bool usePowerIcon = true)
    {
        var config = new AuraConfig { ScenePath = scenePath };

        _controller = new PowerAuraController(power, config);
        AuraCmd.Apply(_controller, scenePath);

        if (usePowerIcon)
            UsePowerIcon();
    }

    internal AuraBuilder(PowerAuraController controller, string? scenePath = null)
    {
        _controller = controller;
        AuraCmd.Apply(_controller, scenePath ?? controller.Config.ScenePath);
    }

    public static AuraBuilder CreateNoIcon(PowerModel power, string scenePath)
    {
        var config = new AuraConfig { ScenePath = scenePath };
        var controller = new PowerAuraController(power, config);

        return new AuraBuilder(controller, scenePath);
    }

    public AuraBuilder Set(string slotName, VfxSlotValue value)
    {
        Config.Set(slotName, value);
        SyncIfNeeded();
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
        SyncIfNeeded();
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
        SyncIfNeeded();
        return this;
    }

    public AuraBuilder WithKey(string auraKey)
    {
        Config.AuraKey = auraKey;
        SyncIfNeeded();
        return this;
    }

    public AuraBuilder SetPowerAmountRange(decimal minPowerAmount, decimal maxPowerAmount)
    {
        Config.SetPowerAmountRange(minPowerAmount, maxPowerAmount);
        SyncIfNeeded();
        return this;
    }

    public AuraBuilder SetPowerAmountRange(Func<decimal> minPowerAmountProvider, Func<decimal> maxPowerAmountProvider)
    {
        Config.SetPowerAmountRange(minPowerAmountProvider, maxPowerAmountProvider);
        SyncIfNeeded();
        return this;
    }

    public AuraBuilder Configure(Action<AuraBuilder> configure)
    {
        _syncSuspended = true;

        try
        {
            configure(this);
        }
        finally
        {
            _syncSuspended = false;
            _controller.Sync();
        }

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

    private void SyncIfNeeded()
    {
        if (!_syncSuspended)
            _controller.Sync();
    }
}