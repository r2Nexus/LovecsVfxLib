using Godot;

namespace LovecsVfxLibCode.Vfx.Auras;

public record AuraSpec
{
    public string? AuraKey { get; init; }

    public Texture2D? Icon { get; init; }

    public Color? Color { get; init; }

    public float AmountScale { get; init; } = 0.05f;

    public Vector2 Offset { get; init; } = Vector2.Zero;

    public static AuraSpec Default { get; } = new();

    public static AuraSpec WithColor(Color color, float amountScale = 0.05f)
        => new()
        {
            Color = color,
            AmountScale = amountScale
        };
}