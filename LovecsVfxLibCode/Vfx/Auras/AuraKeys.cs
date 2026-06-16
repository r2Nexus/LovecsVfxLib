using LovecsVfxLibCode.Vfx.Auras;
using MegaCrit.Sts2.Core.Models;

namespace LovecsVfxLibCode.Vfx.Auras;

public static class AuraKeys
{
    public static string ForPower(PowerModel power)
        => $"Aura_Power_{power.GetType().FullName}";

    public static string ForController(AuraController controller, string? scenePath = null)
    {
        string scenePart = scenePath ?? "default";
        return $"Aura_{controller.GetType().FullName}_{scenePart.GetHashCode()}";
    }
}
