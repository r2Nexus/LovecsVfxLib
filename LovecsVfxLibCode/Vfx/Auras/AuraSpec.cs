using Godot;

namespace LovecsVfxLibCode.Vfx.Auras;

public record AuraSpec
{
    public string? ScenePath { get; init; }
    public string? AuraKey { get; init; }
    public Vector2 Offset { get; init; } = Vector2.Zero;

    public static AuraSpec Scene(string scenePath)
        => new() { ScenePath = scenePath };
}