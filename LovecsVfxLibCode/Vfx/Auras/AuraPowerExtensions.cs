using Godot;
using MegaCrit.Sts2.Core.Models;

namespace LovecsVfxLibCode.Auras;

public static class AuraPowerExtensions
{
    public static LovecAura? WithAura(this PowerModel power)
    {
        return AuraCmd.Apply(new PowerAuraController(power));
    }

    public static LovecAura? WithAura(this PowerModel power, Color color, float amountScale = 0.10f)
    {
        AuraSpec spec = AuraSpec.FromColor(color, amountScale);
        return AuraCmd.Apply(new PowerAuraController(power, spec));
    }

    public static LovecAura? WithAura(
        this PowerModel power,
        string scenePath,
        float amountScale = 0.10f)
    {
        AuraSpec spec = AuraSpec.FromScene(scenePath, amountScale);
        return AuraCmd.Apply(new PowerAuraController(power, spec));
    }

    public static LovecAura? WithAura(this PowerModel power, AuraSpec spec)
    {
        return AuraCmd.Apply(new PowerAuraController(power, spec));
    }
    
    public static LovecAura? WithAura<TController>(this PowerModel power)
        where TController : PowerAuraController
    {
        TController controller = CreatePowerController<TController>(power);
        return AuraCmd.Apply(controller);
    }

    private static TController CreatePowerController<TController>(PowerModel power)
        where TController : PowerAuraController
    {
        try
        {
            return (TController)Activator.CreateInstance(typeof(TController), power)!;
        }
        catch (MissingMethodException)
        {
            throw new InvalidOperationException(
                $"{typeof(TController).Name} needs a constructor that accepts {power.GetType().Name} or PowerModel.");
        }
    }
}
