using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace LovecsVfxLibCode.Auras;

public class PowerAuraController : AuraController
{
    private readonly AuraSpec? _explicitSpec;
    private bool _subscribed;

    public PowerModel Power { get; }

    public PowerAuraController(PowerModel power, AuraSpec? spec = null)
    {
        Power = power ?? throw new ArgumentNullException(nameof(power));
        _explicitSpec = spec;
    }

    public override Creature Target => Power.Owner;

    public override decimal Amount => Power.DisplayAmount;

    public override AuraSpec Spec
    {
        get
        {
            AuraSpec spec = _explicitSpec ?? AuraSpec.Default;

            if (spec.Icon == null)
            {
                try
                {
                    spec = spec with { Icon = Power.Icon };
                }
                catch
                {
                    // Missing icons should not kill VFX.
                }
            }

            spec = spec with
            {
                AuraKey = spec.AuraKey ?? AuraKeys.ForPower(Power)
            };

            return spec;
        }
    }

    protected override void OnAttached()
    {
        if (_subscribed)
            return;

        Power.DisplayAmountChanged += Sync;
        Power.Removed += Remove;
        _subscribed = true;
    }

    protected override void OnDetached()
    {
        if (!_subscribed)
            return;

        Power.DisplayAmountChanged -= Sync;
        Power.Removed -= Remove;
        _subscribed = false;
    }
}

public class PowerAuraController<TPower> : PowerAuraController
    where TPower : PowerModel
{
    public new TPower Power => (TPower)base.Power;

    public PowerAuraController(TPower power, AuraSpec? spec = null)
        : base(power, spec)
    {
    }
}
