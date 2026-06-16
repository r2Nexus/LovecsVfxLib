using Godot;
using MegaCrit.Sts2.Core.Models;

namespace LovecsVfxLibCode.Vfx.Auras;

public static class AuraPowerExtensions
{
    public static LovecAura? WithAura(this PowerModel power)
    {
        return power.WithAura(
            scenePath: null,
            color: null,
            amountScale: 0.05f);
    }

    public static LovecAura? WithAura(
        this PowerModel power,
        Color color,
        float amountScale = 0.05f)
    {
        return power.WithAura(
            scenePath: null,
            color: color,
            amountScale: amountScale);
    }

    public static LovecAura? WithAura(
        this PowerModel power,
        string scenePath,
        float amountScale = 0.05f)
    {
        return power.WithAura(
            scenePath: scenePath,
            color: null,
            amountScale: amountScale);
    }

    public static LovecAura? WithAura(
        this PowerModel power,
        string scenePath,
        Color color,
        float amountScale = 0.05f)
    {
        return power.WithAura(
            scenePath: scenePath,
            color: color,
            amountScale: amountScale);
    }

    public static LovecAura? WithAura(
        this PowerModel power,
        string? scenePath,
        Color? color,
        float amountScale = 0.05f)
    {
        AuraSpec spec = new()
        {
            Color = color,
            AmountScale = amountScale
        };

        return power.WithAura(scenePath, spec);
    }

    public static LovecAura? WithAura(
        this PowerModel power,
        string? scenePath,
        AuraSpec spec)
    {
        AuraSpec completedSpec = spec with
        {
            Icon = spec.Icon ?? power.Icon
        };

        return AuraCmd.Apply(
            new PowerAuraController(power, completedSpec),
            scenePath);
    }
}