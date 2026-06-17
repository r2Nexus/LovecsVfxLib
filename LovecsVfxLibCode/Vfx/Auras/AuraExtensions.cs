using MegaCrit.Sts2.Core.Models;

namespace LovecsVfxLibCode.Vfx.Auras;

public static class AuraExtensions
{
    public static AuraBuilder WithAura(this PowerModel power, string? scenePath = null)
        => new(power, scenePath, usePowerIcon: true);

    public static AuraBuilder WithAuraNoIcon(this PowerModel power, string scenePath)
        => new(power, scenePath, usePowerIcon: false);
}
