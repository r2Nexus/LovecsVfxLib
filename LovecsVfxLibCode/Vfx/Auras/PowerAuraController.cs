using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace LovecsVfxLibCode.Vfx.Auras;

public sealed class PowerAuraController : AuraController
{
    private readonly AuraSpec _baseSpec;
    private AuraSpec? _completedSpec;
    private bool _subscribed;

    public PowerModel Power { get; }

    public PowerAuraController(PowerModel power, AuraSpec? spec = null)
    {
        Power = power ?? throw new ArgumentNullException(nameof(power));
        _baseSpec = spec ?? AuraSpec.Default;
    }

    public override Creature Target => Power.Owner;

    public override decimal Amount => Power.DisplayAmount;

    public override AuraSpec Spec => _completedSpec ??= CompleteSpec(_baseSpec);

    private AuraSpec CompleteSpec(AuraSpec spec)
    {
        Texture2D? icon = spec.Icon;

        if (icon == null)
        {
            try
            {
                icon = Power.Icon;
            }
            catch (Exception ex)
            {
                GD.PushWarning($"[Aura] Failed to get power icon for {Power.GetType().Name}: {ex.Message}");
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