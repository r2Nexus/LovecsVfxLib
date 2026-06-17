using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace LovecsVfxLibCode.Vfx.Auras;

public sealed class PowerAuraController : AuraController
{
    public PowerModel Power { get; }

    public override Creature Target => Power.Owner;
    public override decimal Amount => Power.DisplayAmount;

    public PowerAuraController(PowerModel power, AuraConfig? config = null) : base(config)
    {
        Power = power;
    }

    protected override void CompleteConfig(AuraConfig config)
    {
        config.AuraKey ??= AuraKeys.ForPower(Power);

        // Default power aura convention: if the scene has an "icon" marker,
        // use the power icon unless the caller already provided a custom icon.
        if (!config.Slots.ContainsKey(VfxSlots.Icon))
        {
            var icon = AuraPowerUtil.TryGetPowerIcon(Power);
            if (icon != null)
                config.Set(VfxSlots.Icon, icon);
        }
    }

    protected override void OnAttached()
    {
        Power.DisplayAmountChanged += Sync;
        Power.Removed += Remove;
    }

    protected override void OnDetached()
    {
        Power.DisplayAmountChanged -= Sync;
        Power.Removed -= Remove;
    }
}
