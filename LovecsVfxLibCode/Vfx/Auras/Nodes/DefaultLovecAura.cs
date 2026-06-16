using Godot;
using LovecsVfxLibCode.Vfx.Auras;

namespace LovecsVfxLibCode.Auras;

public partial class DefaultLovecAura : LovecAura
{
    private GpuParticles2D? _iconParticles;
    private GpuParticles2D? _colorParticles;

    protected override void InitializeOnce()
    {
        base.InitializeOnce();

        _iconParticles = GetNodeOrNull<GpuParticles2D>("IconParticles");
        _colorParticles = GetNodeOrNull<GpuParticles2D>("ColorParticles");

        if (_iconParticles == null)
            GD.PushWarning($"{nameof(DefaultLovecAura)} is missing IconParticles.");

        if (_colorParticles == null)
            GD.PushWarning($"{nameof(DefaultLovecAura)} is missing ColorParticles.");

        ResetParticles();
    }

    protected override void ApplySpec(AuraSpec spec)
    {
        Modulate = Colors.White;

        ApplyIconParticles(spec.Icon);
        ApplyColorParticles(spec.Color);
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
        if (_iconParticles != null)
        {
            _iconParticles.Modulate = Colors.White;
            _iconParticles.Emitting = false;

            if (_iconParticles.ProcessMaterial is ParticleProcessMaterial material)
                material.Color = Colors.White;
        }

        if (_colorParticles != null)
        {
            _colorParticles.Modulate = Colors.White;
            _colorParticles.Emitting = false;

            if (_colorParticles.ProcessMaterial is ParticleProcessMaterial material)
                material.Color = Colors.White;
        }
    }
}