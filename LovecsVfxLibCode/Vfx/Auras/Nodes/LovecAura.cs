using System.Collections.Generic;
using System.Linq;
using Godot;

namespace LovecsVfxLibCode.Vfx.Auras;

public partial class LovecAura : Node2D
{
    private bool _initialized;

    private Vector2 _baseScale = Vector2.One;

    private GpuParticles2D[] _particles = [];

    private readonly Dictionary<GpuParticles2D, float> _baseAmountRatios = new();
    private readonly Dictionary<GpuParticles2D, float> _baseSpeedScales = new();
    private readonly HashSet<GpuParticles2D> _localizedParticleMaterials = new();

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

        AuraSpec spec = controller.Spec;

        AuraCmd.UpdateAuraPosition(this, controller);

        Visible = true;

        ApplySpec(spec);
        ApplyIntensity(controller.GetIntensity(), spec);
    }

    protected virtual void ApplySpec(AuraSpec spec)
    {
        // Generic custom aura behavior:
        // If a color is provided, tint the whole aura.
        // DefaultLovecAura should override this and NOT call base,
        // because its color should only affect ColorParticles.
        Modulate = spec.Color ?? Colors.White;
    }

    protected virtual void ApplyIntensity(float intensity, AuraSpec spec)
    {
        // Intensity is expected to start at 0.
        // Example:
        // Amount = 6, AmountScale = 0.05 -> intensity = 0.3

        if (spec.ReactWithNodeScale)
            Scale = _baseScale * (1f + intensity);
        else
            Scale = _baseScale;

        foreach (GpuParticles2D particles in _particles)
        {
            if (spec.ReactWithParticleAmount)
            {
                float baseRatio = GetBaseAmountRatio(particles);

                particles.AmountRatio = Mathf.Clamp(
                    baseRatio + intensity,
                    spec.MinParticleAmountRatio,
                    spec.MaxParticleAmountRatio);
            }

            if (spec.ReactWithParticleSpeed)
            {
                float baseSpeed = GetBaseSpeedScale(particles);

                particles.SpeedScale = Mathf.Clamp(
                    baseSpeed * (1f + intensity),
                    spec.MinParticleSpeed,
                    spec.MaxParticleSpeed);
            }
        }
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

        _baseScale = Scale;

        RefreshParticleCache();
    }

    protected void RefreshParticleCache()
    {
        _particles = FindParticles(this).ToArray();

        foreach (GpuParticles2D particles in _particles)
        {
            _baseAmountRatios.TryAdd(particles, (float)particles.AmountRatio);
            _baseSpeedScales.TryAdd(particles, (float)particles.SpeedScale);

            LocalizeParticleMaterial(particles);
        }
    }

    private float GetBaseAmountRatio(GpuParticles2D particles)
    {
        return _baseAmountRatios.TryGetValue(particles, out float value)
            ? value
            : (float)particles.AmountRatio;
    }

    private float GetBaseSpeedScale(GpuParticles2D particles)
    {
        return _baseSpeedScales.TryGetValue(particles, out float value)
            ? value
            : (float)particles.SpeedScale;
    }

    private void LocalizeParticleMaterial(GpuParticles2D particles)
    {
        if (!_localizedParticleMaterials.Add(particles))
            return;

        if (particles.ProcessMaterial is Material material)
            particles.ProcessMaterial = (Material)material.Duplicate();
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