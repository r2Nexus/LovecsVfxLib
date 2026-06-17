using Godot;

namespace LovecsVfxLibCode.Vfx.Auras;

public readonly record struct VfxSpriteSheet(
    Texture2D Texture,
    int HFrames,
    int VFrames,
    bool Loop);
