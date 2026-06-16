using Godot;

namespace LovecsVfxLibCode.Auras;

public partial class LovecAura : Node2D
{
    private bool _initialized;
    private GpuParticles2D[] _particles = [];

    public AuraController? Controller { get; private set; }

    public override void _Ready()
    {
        InitializeOnce();
    }

    public override void _Process(double delta)
    {
        if (Controller == null)
            return;

        if (Controller.Spec.FollowTarget)
            AuraCmd.UpdateAuraPosition(this, Controller);
    }

    public virtual void Bind(AuraController controller)
    {
        InitializeOnce();

        if (Controller == controller)
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

        AuraSpec spec = controller.Spec;
        AuraCmd.UpdateAuraPosition(this, controller);
        Visible = true;

        ApplySpec(spec);
        ApplyIntensity(controller.GetIntensity(), spec);
    }

    protected virtual void ApplySpec(AuraSpec spec)
    {
        if (spec.Color is { } color)
            Modulate = color;
    }

    protected virtual void ApplyIntensity(float intensity, AuraSpec spec)
    {
        if (spec.ReactWithNodeScale)
            Scale = Vector2.One * intensity;

        foreach (GpuParticles2D particles in _particles)
        {
            if (spec.ReactWithParticleAmount)
            {
                particles.AmountRatio = Mathf.Clamp(
                    intensity,
                    spec.MinParticleAmountRatio,
                    spec.MaxParticleAmountRatio);
            }

            if (spec.ReactWithParticleSpeed)
            {
                particles.SpeedScale = Mathf.Clamp(
                    intensity,
                    spec.MinParticleSpeedScale,
                    spec.MaxParticleSpeedScale);
            }
        }
    }

    public virtual void Remove()
    {
        Controller?.DetachFromView();
        Controller = null;

        if (IsInsideTree())
            QueueFree();
    }

    public override void _ExitTree()
    {
        Controller?.DetachFromView();
        Controller = null;
    }

    protected void RefreshParticleCache()
    {
        _particles = FindParticles(this).ToArray();

        foreach (GpuParticles2D particles in _particles)
        {
            if (particles.ProcessMaterial is Material material)
                particles.ProcessMaterial = (Material)material.Duplicate();
        }
    }

    protected virtual void InitializeOnce()
    {
        if (_initialized)
            return;

        _initialized = true;
        RefreshParticleCache();
    }

    private static IEnumerable<GpuParticles2D> FindParticles(Node node)
    {
        foreach (Node child in node.GetChildren())
        {
            if (child is GpuParticles2D particles)
                yield return particles;

            foreach (GpuParticles2D nested in FindParticles(child))
                yield return nested;
        }
    }
}
