using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace LovecsVfxLibCode.Vfx.Auras;

public abstract class AuraController : IDisposable
{
    private bool _isAttached;
    private bool _isSyncing;
    private bool _isRemoving;

    public abstract Creature Target { get; }
    public AuraConfig Config { get; }
    public LovecAura? View { get; private set; }
    public virtual decimal Amount => 1m;

    protected AuraController(AuraConfig? config = null)
    {
        Config = config ?? new AuraConfig();
    }

    internal void Prepare()
    {
        CompleteConfig(Config);
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

    protected virtual void CompleteConfig(AuraConfig config) {}
    protected virtual void OnAttached() {}
    protected virtual void OnDetached() {}

    public virtual bool IsActive() => Amount > 0m;

    public virtual VfxState GetState(double delta = 0d)
        => new(
            IsActive(),
            Amount,
            delta,
            Config.TryGetMinPowerAmount(),
            Config.TryGetMaxPowerAmount());

    public virtual bool ShouldRemove()
        => Target == null || Target.IsDead;

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
