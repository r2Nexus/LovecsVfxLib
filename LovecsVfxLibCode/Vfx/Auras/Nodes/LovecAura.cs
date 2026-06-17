using Godot;

namespace LovecsVfxLibCode.Vfx.Auras;

public partial class LovecAura : Node2D
{
    private bool _initialized;
    private int _appliedConfigVersion = -1;

    private readonly List<IVfxMarker> _markers = new();
    private readonly List<IVfxStateReceiver> _stateReceivers = new();

    public AuraController? Controller { get; private set; }

    public override void _Ready()
    {
        InitializeOnce();
    }

    public override void _Process(double delta)
    {
        if (Controller == null)
            return;

        AuraCmd.UpdateAuraPosition(this, Controller);
        ApplyState(Controller.GetState(delta));
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
        ApplyConfig(controller.Config);
        ApplyState(controller.GetState());
    }

    protected virtual void ApplyConfig(AuraConfig config)
    {
        if (_appliedConfigVersion == config.Version)
            return;

        Modulate = Colors.White;
        VfxMarkerUtil.ApplySlots(_markers, config.Slots);
        _appliedConfigVersion = config.Version;
    }

    protected virtual void ApplyState(VfxState state)
    {
        VfxMarkerUtil.ApplyState(_stateReceivers, state);
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
        _markers.Clear();
        _stateReceivers.Clear();

        VfxMarkerScanner.Scan(this, _markers, _stateReceivers);
    }
}
