using MegaCrit.Sts2.Core.Models;

namespace LovecsVfxLibCode.Vfx.Auras;

public static class AuraExtensions
{
    /// <summary>
    /// Attach the default marker-based aura scene to this power.
    /// The power icon is automatically provided to the "icon" slot if available.
    /// </summary>
    public static AuraBuilder WithAura(this PowerModel power)
        => new AuraBuilder(power).UsePowerIcon(VfxSlots.Icon);

    /// <summary>
    /// Attach a custom marker-based aura scene to this power.
    /// The power icon is automatically provided to the "icon" slot if available.
    /// </summary>
    public static AuraBuilder WithAura(this PowerModel power, string scenePath)
        => new AuraBuilder(power, scenePath).UsePowerIcon(VfxSlots.Icon);
}
