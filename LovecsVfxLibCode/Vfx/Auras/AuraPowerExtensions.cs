using System;
using System.Reflection;
using Godot;
using LovecsVfxLibCode.Auras;
using MegaCrit.Sts2.Core.Models;

namespace LovecsVfxLibCode.Vfx.Auras;

public static class AuraPowerExtensions
{
    public static LovecAura? WithAura(this PowerModel power)
    {
        return power.WithAura(AuraSpec.Default);
    }

    public static LovecAura? WithAura(
        this PowerModel power,
        Color color,
        float amountScale = 0.05f)
    {
        return power.WithAura(AuraSpec.FromColor(color, amountScale));
    }

    public static LovecAura? WithAura(
        this PowerModel power,
        string scenePath,
        float amountScale = 0.05f)
    {
        return power.WithAura(AuraSpec.FromScene(scenePath, amountScale));
    }

    public static LovecAura? WithAura(
        this PowerModel power,
        AuraSpec spec)
    {
        return AuraCmd.Apply(new PowerAuraController(power, spec));
    }

    public static LovecAura? WithAura<TController>(
        this PowerModel power)
        where TController : PowerAuraController
    {
        return power.WithAura<TController>(AuraSpec.Default);
    }

    public static LovecAura? WithAura<TController>(
        this PowerModel power,
        Color color,
        float amountScale = 0.05f)
        where TController : PowerAuraController
    {
        return power.WithAura<TController>(
            AuraSpec.FromColor(color, amountScale));
    }

    public static LovecAura? WithAura<TController>(
        this PowerModel power,
        string scenePath,
        float amountScale = 0.05f)
        where TController : PowerAuraController
    {
        return power.WithAura<TController>(
            AuraSpec.FromScene(scenePath, amountScale));
    }

    public static LovecAura? WithAura<TController>(
        this PowerModel power,
        AuraSpec spec)
        where TController : PowerAuraController
    {
        TController controller = CreateController<TController>(power, spec);
        return AuraCmd.Apply(controller);
    }

    private static TController CreateController<TController>(
        PowerModel power,
        AuraSpec spec)
        where TController : PowerAuraController
    {
        Type controllerType = typeof(TController);
        Type powerType = power.GetType();

        object? controller =
            TryCreate(controllerType, new[] { powerType, typeof(AuraSpec) }, new object[] { power, spec })
            ?? TryCreate(controllerType, new[] { typeof(PowerModel), typeof(AuraSpec) }, new object[] { power, spec })
            ?? TryCreate(controllerType, new[] { powerType }, new object[] { power })
            ?? TryCreate(controllerType, new[] { typeof(PowerModel) }, new object[] { power });

        if (controller is TController typed)
            return typed;

        throw new InvalidOperationException(
            $"Could not create aura controller {controllerType.Name}. " +
            $"Expected constructor ({powerType.Name}, AuraSpec), " +
            $"(PowerModel, AuraSpec), ({powerType.Name}), or (PowerModel).");
    }

    private static object? TryCreate(
        Type type,
        Type[] parameterTypes,
        object[] args)
    {
        ConstructorInfo? ctor = type.GetConstructor(parameterTypes);
        return ctor?.Invoke(args);
    }
}