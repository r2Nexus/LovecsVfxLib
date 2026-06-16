using Godot;

namespace LovecsVfxLibCode.Vfx.Auras;

public partial class LovecAura : Node2D
{
    private bool _initialized;

    public AuraController? Controller { get; private set; }

    public override void _Ready()
    {
        InitializeOnce();
    }

    public override void _Process(double delta)
    {
        if (Controller != null)
            AuraCmd.UpdateAuraPosition(this, Controller);
    }

    public virtual void Bind(AuraController controller)
    {
        InitializeOnce();

        if (ReferenceEquals(Controller, controller))
        {
            controller.Sync();
            return;
        }

        Controller?.DetachFromView();
        Controller = controller;
        controller.AttachToView(this);
    }

    public virtual void SyncFromController(AuraController controller)
    {
        InitializeOnce();

        AuraCmd.UpdateAuraPosition(this, controller);

        Visible = true;

        ApplySpec(controller.Spec);
        ApplyIntensity(controller.GetIntensity());
    }

    protected virtual void ApplySpec(AuraSpec spec)
    {
        Modulate = Colors.White;
    }

    protected virtual void ApplyIntensity(float intensity)
    {
        // Intentionally empty.
        // Concrete aura scenes decide what intensity means.
    }

    public virtual void Remove()
    {
        AuraController? controller = Controller;
        Controller = null;

        controller?.DetachFromView();

        if (IsInsideTree() && !IsQueuedForDeletion())
            QueueFree();
    }

    public override void _ExitTree()
    {
        AuraController? controller = Controller;
        Controller = null;

        controller?.DetachFromView();
    }

    protected virtual void InitializeOnce()
    {
        if (_initialized)
            return;

        _initialized = true;
    }
}