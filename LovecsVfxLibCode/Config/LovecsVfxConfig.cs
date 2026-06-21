using BaseLib.Config;

namespace LovecsVfxLib.LovecsVfxLibCode.Config;

[ConfigHoverTipsByDefault]
internal sealed class LovecsVfxConfig : SimpleModConfig
{
    [ConfigSection("VanillaPowerAuras")]
    public static bool EnableVanillaPowerAuras { get; set; } = true;

    [ConfigSection("IndividualVanillaPowerAuras")]
    [ConfigVisibleIf(nameof(EnableVanillaPowerAuras))]
    public static bool EnableStrengthAura { get; set; } = true;

    [ConfigVisibleIf(nameof(EnableVanillaPowerAuras))]
    public static bool EnableDexterityAura { get; set; } = true;

    [ConfigVisibleIf(nameof(EnableVanillaPowerAuras))]
    public static bool EnableDoomAura { get; set; } = true;

    [ConfigVisibleIf(nameof(EnableVanillaPowerAuras))]
    public static bool EnableFrailAura { get; set; } = true;

    [ConfigVisibleIf(nameof(EnableVanillaPowerAuras))]
    public static bool EnableVulnerableAura { get; set; } = true;

    [ConfigVisibleIf(nameof(EnableVanillaPowerAuras))]
    public static bool EnablePoisonAura { get; set; } = true;

    [ConfigVisibleIf(nameof(EnableVanillaPowerAuras))]
    public static bool EnableFocusAura { get; set; } = true;
}