using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace LovecsVfxLibCode.Vfx.Auras;

public abstract class AuraController : IDisposable
{
    private bool _isAttached;
    private bool _isDisposed;

    public abstract Creature Target { get; }

    public LovecAura? View { get; private set; }

    public virtual decimal Amount => 1m;

    public virtual AuraSpec Spec => AuraSpec.Default;

    public bool IsDisposed => _isDisposed;

    internal void AttachToView(LovecAura view)
    {
        if (_isDisposed)
            return;

        if (ReferenceEquals(View, view) && _isAttached)
        {
            Sync();
            return;
        }

        if (View != null)
            DetachFromView();

        View = view;
        _isAttached = true;

        OnAttached();
        Sync();
    }

    internal void DetachFromView()
    {
        if (!_isAttached)
            return;

        _isAttached = false;
        OnDetached();
        View = null;
    }

    protected virtual void OnAttached() {}
    protected virtual void OnDetached() {}

    public virtual float GetIntensity()
    {
        return Mathf.Max(0f, (float)Amount * Spec.AmountScale);
    }

    public virtual bool ShouldRemove()
    {
        Creature target = Target;
        return target == null || target.IsDead;
    }

    public virtual void Sync()
    {
        if (_isDisposed || View == null)
            return;

        if (ShouldRemove())
        {
            Remove();
            return;
        }

        View.SyncFromController(this);
    }

    public virtual void Remove()
    {
        View?.Remove();
        Dispose();
    }

    public virtual void Dispose()
    {
        if (_isDisposed)
            return;

        _isDisposed = true;
        DetachFromView();
        GC.SuppressFinalize(this);
    }
}