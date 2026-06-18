using Godot;

namespace LovecsVfxLibCode.Vfx.Auras;

public enum VfxSlotValueKind
{
    None,
    Texture,
    TexturePath,
    Color,
    SpriteSheet
}

public readonly record struct VfxSlotValue
{
    public VfxSlotValueKind Kind { get; }
    public object? Value { get; }

    private VfxSlotValue(VfxSlotValueKind kind, object? value)
    {
        Kind = kind;
        Value = value;
    }

    public static implicit operator VfxSlotValue(Texture2D texture) => new(VfxSlotValueKind.Texture, texture);
    public static implicit operator VfxSlotValue(string texturePath) => new(VfxSlotValueKind.TexturePath, texturePath);
    public static implicit operator VfxSlotValue(Color color) => new(VfxSlotValueKind.Color, color);
    public static implicit operator VfxSlotValue(VfxSpriteSheet sheet) => new(VfxSlotValueKind.SpriteSheet, sheet);

    public static VfxSlotValue FromSpriteSheet(
        string texturePath,
        int hFrames,
        int vFrames,
        bool loop = false,
        float? animSpeedMin = null,
        float? animSpeedMax = null)
    {
        Texture2D texture = LoadTexture(texturePath);

        if (animSpeedMin.HasValue && !animSpeedMax.HasValue)
            animSpeedMax = animSpeedMin;

        if (!animSpeedMin.HasValue && animSpeedMax.HasValue)
            animSpeedMin = animSpeedMax;

        return new VfxSpriteSheet(
            texture,
            hFrames,
            vFrames,
            loop,
            animSpeedMin,
            animSpeedMax);
    }

    public Texture2D AsTexture()
    {
        Texture2D? texture = TryAsTexture();
        if (texture != null)
            return texture;

        throw new InvalidOperationException($"Expected Texture2D or texture path, got {Kind}.");
    }

    public Texture2D? TryAsTexture()
    {
        return Kind switch
        {
            VfxSlotValueKind.Texture => Value as Texture2D,
            VfxSlotValueKind.TexturePath => Value is string path ? LoadTexture(path) : null,
            VfxSlotValueKind.SpriteSheet => Value is VfxSpriteSheet sheet ? sheet.Texture : null,
            _ => null
        };
    }

    public Color AsColor()
    {
        Color? color = TryAsColor();
        if (color.HasValue)
            return color.Value;

        throw new InvalidOperationException($"Expected Color, got {Kind}.");
    }

    public Color? TryAsColor()
        => Kind == VfxSlotValueKind.Color && Value is Color color ? color : null;

    public VfxSpriteSheet AsSpriteSheet()
    {
        if (Kind == VfxSlotValueKind.SpriteSheet && Value is VfxSpriteSheet sheet)
            return sheet;

        throw new InvalidOperationException($"Expected sprite-sheet value, got {Kind}.");
    }

    private static Texture2D LoadTexture(string path)
    {
        Texture2D? texture = GD.Load<Texture2D>(path);
        if (texture == null)
            throw new InvalidOperationException($"Failed to load VFX texture: {path}");

        return texture;
    }
}
