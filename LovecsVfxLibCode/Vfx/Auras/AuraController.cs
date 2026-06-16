using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace LovecsVfxLibCode.Auras;

public abstract class AuraController : IDisposable
{
    private bool _isAttached;
    private bool _isDisposed;

    public abstract Creature Target { get; }

    public LovecAura? View { get; private set; }

    public virtual decimal Amount => 1m;

    public virtual AuraSpec Spec => AuraSpec.Default;

    public bool IsAttached => _isAttached;
    public bool IsDisposed => _isDisposed;

    internal void AttachToView(LovecAura view)
    {
        if (_isDisposed)
            throw new ObjectDisposedException(GetType().Name);

        if (View == view && _isAttached)
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

    protected virtual void OnAttached()
    {
    }

    protected virtual void OnDetached()
    {
    }

    public virtual void Sync()
    {
        if (_isDisposed)
            return;

        if (View == null)
            return;

        if (ShouldRemove())
        {
            Remove();
            return;
        }

        View.SyncFromController(this);
    }

    public virtual bool ShouldRemove()
    {
        AuraSpec spec = Spec;

        if (spec.RemoveAtZero && Amount <= 0)
            return true;

        Creature target = Target;
        return target == null || !target.IsAlive;
    }

    public virtual float GetIntensity()
    {
        AuraSpec spec = Spec;

        return Mathf.Clamp(
            (float)Amount * spec.AmountScale,
            spec.MinIntensity,
            spec.MaxIntensity);
    }

    public virtual void Remove()
    {
        View?.Remove();
        Dispose();
    }

    public void Dispose()
    {
        if (_isDisposed)
            return;

        _isDisposed = true;
        DetachFromView();
        GC.SuppressFinalize(this);
    }
}
