using LovecsVfxLibCode.Auras;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace LovecsVfxLibCode.Vfx.Auras;

public class PowerAuraController : AuraController
{
    private readonly float _amountScale;

    public PowerModel Power { get; }

    public override Creature Target => Power.Owner;
    public override decimal Amount => Power.DisplayAmount;
    public override float AmountScale => _amountScale;

    public PowerAuraController(
        PowerModel power,
        AuraSpec? spec = null,
        float amountScale = 0.05f)
        : base(spec)
    {
        Power = power;
        _amountScale = amountScale;
    }

    protected override AuraSpec CompleteSpec(AuraSpec spec)
    {
        if (spec is DefaultAuraSpec defaultSpec)
        {
            return defaultSpec with
            {
                Icon = defaultSpec.Icon ?? Power.Icon,
                AuraKey = defaultSpec.AuraKey ?? AuraKeys.ForPower(Power)
            };
        }

        return spec with
        {
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

public class PowerAuraController<TPower> : PowerAuraController
    where TPower : PowerModel
{
    public new TPower Power => (TPower)base.Power;

    public PowerAuraController(
        TPower power,
        AuraSpec? spec = null,
        float amountScale = 0.05f)
        : base(power, spec, amountScale)
    {
    }
}