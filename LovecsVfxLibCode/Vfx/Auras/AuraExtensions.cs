using MegaCrit.Sts2.Core.Models;

namespace LovecsVfxLibCode.Vfx.Auras;

public static class AuraExtensions
{
    public static AuraBuilder WithAura(this PowerModel power)
        => new AuraBuilder(power);

    public static AuraBuilder WithAura(this PowerModel power, string scenePath)
        => new AuraBuilder(power, scenePath);

    public static AuraBuilder WithAuraIcon(this PowerModel power)
        => new AuraBuilder(power, usePowerIcon: true);

    public static AuraBuilder WithAuraIcon(this PowerModel power, string scenePath)
        => new AuraBuilder(power, scenePath, usePowerIcon: true);
}