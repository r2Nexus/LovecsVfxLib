using Godot;

namespace LovecsVfxLibCode.Auras;

public sealed record AuraSpec
{
    public static AuraSpec Default { get; } = new();

    public string? ScenePath { get; init; }
    public string? AuraKey { get; init; }

    public Texture2D? Icon { get; init; }
    public Color? Color { get; init; }

    public Vector2 Offset { get; init; } = Vector2.Zero;
    
    public bool FollowTarget { get; init; } = true;

    public AuraPositionMode PositionMode { get; init; } = AuraPositionMode.VfxSpawnPosition;

    public float AmountScale { get; init; } = 0.10f;
    public float MinIntensity { get; init; } = 0.05f;
    public float MaxIntensity { get; init; } = 2.00f;
    public bool ReactWithNodeScale { get; init; } = false;
    public bool ReactWithParticleAmount { get; init; } = true;
    public bool ReactWithParticleSpeed { get; init; } = false;

    public float MinParticleAmountRatio { get; init; } = 0.0f;
    public float MaxParticleAmountRatio { get; init; } = 1.00f;

    public float MinParticleSpeedScale { get; init; } = 0.75f;
    public float MaxParticleSpeedScale { get; init; } = 1.50f;

    public bool RemoveAtZero { get; init; } = true;

    public static AuraSpec FromScene(string scenePath, float amountScale = 0.10f)
        => new()
        {
            ScenePath = scenePath,
            AmountScale = amountScale
        };

    public static AuraSpec FromColor(Color color, float amountScale = 0.10f)
        => new()
        {
            Color = color,
            AmountScale = amountScale
        };
}
