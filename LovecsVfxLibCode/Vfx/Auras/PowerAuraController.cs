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
        return spec with
        {
            Icon = spec.Icon ?? Power.Icon,
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