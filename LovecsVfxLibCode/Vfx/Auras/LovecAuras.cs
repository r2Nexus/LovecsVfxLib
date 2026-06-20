namespace LovecsVfxLibCode.Vfx.Auras;

public static class LovecAuras
{
    private const string Root = "res://LovecsVfxLib/scenes/vfx/auras/";

    public const string Default = Root + "DefaultLovecAura.tscn";
    public const string Enchanted = Root + "enchanted_aura.tscn";
    public const string Guard = Root + "guard_aura.tscn";
    public const string Focus = Root + "focus_aura.tscn";

    public static string Custom(string fileName)
        => Root + fileName;
}