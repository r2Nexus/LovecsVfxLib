using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace LovecsVfxLibCode.Vfx.Auras;

public abstract class AuraController : IDisposable
{
    private bool _isAttached;

    public abstract Creature Target { get; }

    public AuraSpec Spec { get; private set; }

    public LovecAura? View { get; private set; }

    public virtual decimal Amount => 1m;

    public virtual float AmountScale { get; } = 0.05f;
    public virtual float MinIntensity { get; } = 0f;
    public virtual float MaxIntensity { get; } = 1f;

    protected AuraController(AuraSpec? spec = null)
    {
        Spec = spec ?? new AuraSpec();
    }

    internal void AttachToView(LovecAura view)
    {
        if (ReferenceEquals(View, view) && _isAttached)
        {
            Sync();
            return;
        }

        DetachFromView();

        View = view;
        Spec = CompleteSpec(Spec);

        _isAttached = true;
        OnAttached();

        Sync();
    }

    internal void DetachFromView()
    {
        if (!_isAttached)
        {
            View = null;
            return;
        }

        _isAttached = false;
        OnDetached();
        View = null;
    }

    protected virtual AuraSpec CompleteSpec(AuraSpec spec)
        => spec;

    protected virtual void OnAttached() {}
    protected virtual void OnDetached() {}

    public virtual float GetIntensity()
    {
        return Mathf.Clamp(
            (float)Amount * AmountScale,
            MinIntensity,
            MaxIntensity);
    }

    public virtual bool ShouldRemove()
    {
        return Target == null || Target.IsDead;
    }

    public virtual void Sync()
    {
        if (ShouldRemove())
        {
            Remove();
            return;
        }

        View?.SyncFromController(this);
    }

    public virtual void Remove()
    {
        LovecAura? view = View;

        DetachFromView();

        if (view != null && GodotObject.IsInstanceValid(view) && !view.IsQueuedForDeletion())
            view.QueueFree();
    }

    public virtual void Dispose()
    {
        Remove();
    }
}