using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace LovecsVfxLibCode.Vfx.Auras;

public abstract class AuraController : IDisposable
{
    private bool _isAttached;
    private bool _isSyncing;
    private bool _isRemoving;

    public abstract Creature Target { get; }

    public AuraSpec Spec { get; private set; }

    public LovecAura? View { get; private set; }

    public virtual decimal Amount => 1m;

    protected AuraController(AuraSpec? spec = null)
    {
        Spec = spec ?? AuraSpec.Default;
    }
    internal void Prepare()
    {
        Spec = CompleteSpec(Spec);
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
        Prepare();

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

    protected virtual AuraSpec CompleteSpec(AuraSpec spec) => spec;

    protected virtual void OnAttached() {}
    protected virtual void OnDetached() {}

    public virtual float GetIntensity()
    {
        return Mathf.Max(0f, (float)Amount * Spec.AmountScale);
    }

    public virtual bool ShouldRemove()
    {
        return Target == null || Target.IsDead;
    }

    public virtual void Sync()
    {
        if (_isSyncing)
            return;

        try
        {
            _isSyncing = true;

            if (ShouldRemove())
            {
                Remove();
                return;
            }

            View?.SyncFromController(this);
        }
        finally
        {
            _isSyncing = false;
        }
    }

    public virtual void Remove()
    {
        if (_isRemoving)
            return;

        try
        {
            _isRemoving = true;

            LovecAura? view = View;

            DetachFromView();

            if (view != null && GodotObject.IsInstanceValid(view) && !view.IsQueuedForDeletion())
                view.QueueFree();
        }
        finally
        {
            _isRemoving = false;
        }
    }

    public virtual void Dispose()
    {
        Remove();
    }
}