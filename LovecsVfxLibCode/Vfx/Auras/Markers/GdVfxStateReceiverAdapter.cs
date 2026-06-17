using Godot;

namespace LovecsVfxLibCode.Vfx.Auras;

/// <summary>
/// Runtime adapter for GDScript VfxParticlePowerScaling marker nodes.
/// </summary>
public sealed class GdVfxStateReceiverAdapter : IVfxStateReceiver
{
    private readonly Node _node;

    private GdVfxStateReceiverAdapter(Node node)
    {
        _node = node;
    }

    public static bool TryCreate(Node node, out GdVfxStateReceiverAdapter receiver)
    {
        receiver = null!;

        if (!GdVfxMarkerAdapter.HasProperty(node, "min_power_amount"))
            return false;

        if (!GdVfxMarkerAdapter.HasProperty(node, "max_power_amount"))
            return false;

        receiver = new GdVfxStateReceiverAdapter(node);
        return true;
    }

    public void Apply(VfxState state)
    {
        Node? target = GetTargetOrNull();
        if (target is not GpuParticles2D particles)
        {
            if (target != null)
                GD.PushWarning($"[GdVfxStateReceiverAdapter] Target must be GpuParticles2D. Actual: {target.GetType().Name}.");
            return;
        }

        float ratio = GetPowerRatio(state);
        float minSpeed = GetFloat("min_speed_multiplier");
        float maxSpeed = GetFloat("max_speed_multiplier");

        particles.Emitting = state.Active && ratio > 0f;
        particles.AmountRatio = ratio;
        particles.SpeedScale = Mathf.Lerp(minSpeed, maxSpeed, ratio);
    }

    private Node? GetTargetOrNull()
    {
        NodePath targetPath = GetNodePath("target_path");

        if (!targetPath.IsEmpty)
            return _node.GetNodeOrNull(targetPath);

        return _node.GetParent();
    }

    private float GetPowerRatio(VfxState state)
    {
        float amountValue = (float)state.Amount;

        float min = (float)(state.MinPowerAmount ?? (decimal)GetFloat("min_power_amount", 0f));
        float max = (float)(state.MaxPowerAmount ?? (decimal)GetFloat("max_power_amount", 10f));

        if (Mathf.IsEqualApprox(min, max))
            return amountValue >= max ? 1f : 0f;

        float ratio = Mathf.InverseLerp(min, max, amountValue);
        return Mathf.Clamp(ratio, 0f, 1f);
    }

    private float GetFloat(string propertyName, float fallback = 0f)
    {
        Variant value = _node.Get(propertyName);

        if (value.VariantType == Variant.Type.Nil)
            return fallback;

        return (float)value.AsDouble();
    }

    private NodePath GetNodePath(string propertyName)
    {
        Variant value = _node.Get(propertyName);
        return value.VariantType == Variant.Type.Nil
            ? new NodePath("")
            : value.AsNodePath();
    }
}
