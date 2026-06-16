using Godot;

namespace LovecsVfxLibCode.Vfx.Auras;

public partial class DefaultLovecAura : LovecAura
{
    private GpuParticles2D? _iconParticles;
    private GpuParticles2D? _colorParticles;

    private float _baseIconAmountRatio;
    private float _baseColorAmountRatio;
    private float _baseIconSpeedScale;
    private float _baseColorSpeedScale;

    protected override void InitializeOnce()
    {
        base.InitializeOnce();

        _iconParticles = GetNodeOrNull<GpuParticles2D>("IconParticles");
        _colorParticles = GetNodeOrNull<GpuParticles2D>("ColorParticles");

        if (_iconParticles == null)
            GD.PushWarning($"{nameof(DefaultLovecAura)} is missing IconParticles.");

        if (_colorParticles == null)
            GD.PushWarning($"{nameof(DefaultLovecAura)} is missing ColorParticles.");

        if (_iconParticles != null)
        {
            _baseIconAmountRatio = (float)_iconParticles.AmountRatio;
            _baseIconSpeedScale = (float)_iconParticles.SpeedScale;
            LocalizeMaterial(_iconParticles);
        }

        if (_colorParticles != null)
        {
            _baseColorAmountRatio = (float)_colorParticles.AmountRatio;
            _baseColorSpeedScale = (float)_colorParticles.SpeedScale;
            LocalizeMaterial(_colorParticles);
        }

        ResetParticles();
    }

    protected override void ApplySpec(AuraSpec spec)
    {
        Modulate = Colors.White;

        if (spec is not DefaultAuraSpec defaultSpec)
        {
            ApplyIconParticles(null);
            ApplyColorParticles(null);
            return;
        }

        ApplyIconParticles(defaultSpec.Icon);
        ApplyColorParticles(defaultSpec.Color);
    }

    protected override void ApplyIntensity(float intensity)
    {
        if (_iconParticles != null)
        {
            _iconParticles.AmountRatio = Mathf.Clamp(
                _baseIconAmountRatio + intensity,
                0f,
                0.7f);

            _iconParticles.SpeedScale = Mathf.Clamp(
                _baseIconSpeedScale * (1f + intensity),
                0.75f,
                1.5f);
        }

        if (_colorParticles != null)
        {
            _colorParticles.AmountRatio = Mathf.Clamp(
                _baseColorAmountRatio + intensity,
                0f,
                0.7f);

            _colorParticles.SpeedScale = Mathf.Clamp(
                _baseColorSpeedScale * (1f + intensity),
                0.75f,
                1.5f);
        }
    }

    private void ApplyIconParticles(Texture2D? icon)
    {
        if (_iconParticles == null)
            return;

        _iconParticles.Modulate = Colors.White;
        _iconParticles.Texture = icon;
        _iconParticles.Emitting = icon != null;

        if (_iconParticles.ProcessMaterial is ParticleProcessMaterial material)
            material.Color = Colors.White;
    }

    private void ApplyColorParticles(Color? color)
    {
        if (_colorParticles == null)
            return;

        _colorParticles.Modulate = Colors.White;
        _colorParticles.Emitting = color.HasValue;

        if (_colorParticles.ProcessMaterial is ParticleProcessMaterial material)
            material.Color = color ?? Colors.White;
    }

    private void ResetParticles()
    {
        ApplyIconParticles(null);
        ApplyColorParticles(null);
    }

    private static void LocalizeMaterial(GpuParticles2D particles)
    {
        if (particles.ProcessMaterial is Material material)
            particles.ProcessMaterial = (Material)material.Duplicate();
    }
}