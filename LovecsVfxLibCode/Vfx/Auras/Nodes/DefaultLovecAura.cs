using Godot;
using LovecsVfxLibCode.Vfx.Auras;

public partial class DefaultLovecAura : LovecAura
{
    private bool _defaultInitialized;

    private GpuParticles2D? _iconParticles;
    private GpuParticles2D? _colorParticles;

    private float _baseIconRatio;
    private float _baseColorRatio;

    protected override void InitializeOnce()
    {
        if (_defaultInitialized)
            return;

        _defaultInitialized = true;

        base.InitializeOnce();

        _iconParticles = GetNodeOrNull<GpuParticles2D>("IconParticles");
        _colorParticles = GetNodeOrNull<GpuParticles2D>("ColorParticles");

        if (_iconParticles == null)
            GD.PushWarning($"{nameof(DefaultLovecAura)} is missing IconParticles.");

        if (_colorParticles == null)
            GD.PushWarning($"{nameof(DefaultLovecAura)} is missing ColorParticles.");

        if (_iconParticles != null)
        {
            _baseIconRatio = (float)_iconParticles.AmountRatio;
            LocalizeMaterial(_iconParticles);
        }

        if (_colorParticles != null)
        {
            _baseColorRatio = (float)_colorParticles.AmountRatio;
            LocalizeMaterial(_colorParticles);
        }

        ResetParticles();
    }

    protected override void ApplySpec(AuraSpec spec)
    {
        Modulate = Colors.White;

        if (_iconParticles != null)
        {
            _iconParticles.Texture = spec.Icon;
            _iconParticles.Modulate = Colors.White;
            _iconParticles.Emitting = spec.Icon != null;
        }

        if (_colorParticles != null)
        {
            _colorParticles.Modulate = Colors.White;
            _colorParticles.Emitting = spec.Color.HasValue;

            if (_colorParticles.ProcessMaterial is ParticleProcessMaterial mat)
                mat.Color = spec.Color ?? Colors.White;
        }
    }

    protected override void ApplyIntensity(float intensity)
    {
        if (_iconParticles != null)
        {
            _iconParticles.AmountRatio = Mathf.Clamp(
                _baseIconRatio + intensity,
                0f,
                0.6f);
        }

        if (_colorParticles != null)
        {
            _colorParticles.AmountRatio = Mathf.Clamp(
                _baseColorRatio + intensity,
                0f,
                0.7f);
        }
    }

    private void ResetParticles()
    {
        if (_iconParticles != null)
            _iconParticles.Emitting = false;

        if (_colorParticles != null)
            _colorParticles.Emitting = false;
    }

    private static void LocalizeMaterial(GpuParticles2D particles)
    {
        if (particles.ProcessMaterial is Material material)
            particles.ProcessMaterial = (Material)material.Duplicate();
    }
}