using Godot;

namespace LovecsVfxLibCode.Vfx.Auras;

public sealed record DefaultAuraSpec : AuraSpec
{
    public Texture2D? Icon { get; init; }
    public Color? Color { get; init; }

    public static DefaultAuraSpec Empty { get; } = new();

    public static DefaultAuraSpec WithColor(Color color)
        => new() { Color = color };
}