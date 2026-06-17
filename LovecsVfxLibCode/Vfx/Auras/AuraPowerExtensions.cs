using Godot;
using MegaCrit.Sts2.Core.Models;

namespace LovecsVfxLibCode.Vfx.Auras;

public static class AuraPowerExtensions
{
    public static LovecAura? WithAura(this PowerModel power)
    {
        return ApplyAura(
            power,
            scenePath: null,
            spec: new AuraSpec
            {
                AmountScale = 0.05f
            });
    }

    public static LovecAura? WithAura(
        this PowerModel power,
        Color color,
        float amountScale = 0.05f)
    {
        return ApplyAura(
            power,
            scenePath: null,
            spec: new AuraSpec
            {
                Color = color,
                AmountScale = amountScale
            });
    }

    public static LovecAura? WithAura(
        this PowerModel power,
        string scenePath,
        float amountScale = 0.05f)
    {
        return ApplyAura(
            power,
            scenePath,
            spec: new AuraSpec
            {
                AmountScale = amountScale
            });
    }

    public static LovecAura? WithAura(
        this PowerModel power,
        string scenePath,
        Color color,
        float amountScale = 0.05f)
    {
        return ApplyAura(
            power,
            scenePath,
            spec: new AuraSpec
            {
                Color = color,
                AmountScale = amountScale
            });
    }

    public static LovecAura? WithAura(
        this PowerModel power,
        string? scenePath,
        Color? color,
        float amountScale = 0.05f)
    {
        return ApplyAura(
            power,
            scenePath,
            spec: new AuraSpec
            {
                Color = color,
                AmountScale = amountScale
            });
    }

    public static LovecAura? WithAura(
        this PowerModel power,
        string? scenePath,
        AuraSpec spec)
    {
        return ApplyAura(power, scenePath, spec);
    }

    private static LovecAura? ApplyAura(
        PowerModel power,
        string? scenePath,
        AuraSpec spec)
    {
        return AuraCmd.Apply(
            new PowerAuraController(power, spec),
            scenePath);
    }
}