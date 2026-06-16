using MegaCrit.Sts2.Core.Models;

namespace LovecsVfxLibCode.Auras;

public static class AuraKeys
{
    public static string ForPower(PowerModel power)
        => $"Aura_Power_{power.GetType().FullName}";

    public static string ForController(AuraController controller)
    {
        AuraSpec spec = controller.Spec;
        return spec.AuraKey ?? $"Aura_Controller_{controller.GetType().FullName}";
    }
}
