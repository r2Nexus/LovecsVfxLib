using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace LovecsVfxLibCode.Vfx.Auras;

public sealed class PowerAuraController : AuraController
{
    public PowerModel Power { get; }

    public override Creature Target => Power.Owner;

    public override decimal Amount => Power.DisplayAmount;

    public PowerAuraController(PowerModel power, AuraSpec? spec = null)
        : base(spec)
    {
        Power = power;
    }

    protected override AuraSpec CompleteSpec(AuraSpec spec)
    {
        Texture2D? icon = spec.Icon;

        if (icon == null)
        {
            try
            {
                icon = Power.Icon;
            }
            catch
            {
                icon = null;
            }
        }

        return spec with
        {
            Icon = icon,
            AuraKey = spec.AuraKey ?? AuraKeys.ForPower(Power)
        };
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