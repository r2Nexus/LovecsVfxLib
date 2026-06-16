using Godot;

namespace LovecsVfxLibCode.Vfx.Auras;

public sealed record AuraSpec
{
    public static AuraSpec Default { get; } = new();

    public string? ScenePath { get; init; }

    public string? AuraKey { get; init; }

    public Texture2D? Icon { get; init; }

    public Color? Color { get; init; }

    public float AmountScale { get; init; } = 0.05f;

    public float MinIntensity { get; init; } = 0f;

    public float MaxIntensity { get; init; } = 1f;

    public bool ReactWithNodeScale { get; init; } = false;

    public bool ReactWithParticleAmount { get; init; } = true;

    public bool ReactWithParticleSpeed { get; init; } = false;

    public float MinParticleAmountRatio { get; init; } = 0f;

    public float MaxParticleAmountRatio { get; init; } = 0.7f;

    public float MinParticleSpeed { get; init; } = 0.75f;

    public float MaxParticleSpeed { get; init; } = 1.5f;

    public Vector2 Offset { get; init; } = Vector2.Zero;

    public static AuraSpec FromColor(Color color, float amountScale)
        => Default with
        {
            Color = color,
            AmountScale = amountScale
        };

    public static AuraSpec FromScene(string scenePath, float amountScale)
        => Default with
        {
            ScenePath = scenePath,
            AmountScale = amountScale
        };
}