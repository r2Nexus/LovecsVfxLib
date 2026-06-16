using System;
using System.Reflection;
using Godot;
using MegaCrit.Sts2.Core.Models;

namespace LovecsVfxLibCode.Vfx.Auras;

public static class AuraPowerExtensions
{
    public static LovecAura? WithAura(this PowerModel power)
        => AuraCmd.Apply(new PowerAuraController(power, DefaultAuraSpec.Empty));

    public static LovecAura? WithAura(
        this PowerModel power,
        Color color,
        float amountScale = 0.05f)
    {
        return AuraCmd.Apply(
            new PowerAuraController(
                power,
                DefaultAuraSpec.WithColor(color),
                amountScale));
    }

    public static LovecAura? WithAura(
        this PowerModel power,
        string scenePath,
        float amountScale = 0.05f)
    {
        return AuraCmd.Apply(
            new PowerAuraController(
                power,
                AuraSpec.Scene(scenePath),
                amountScale));
    }

    public static LovecAura? WithAura<TController>(
        this PowerModel power)
        where TController : PowerAuraController
    {
        return power.WithAura<TController>(DefaultAuraSpec.Empty);
    }

    public static LovecAura? WithAura<TController>(
        this PowerModel power,
        Color color,
        float amountScale = 0.05f)
        where TController : PowerAuraController
    {
        return power.WithAura<TController>(
            DefaultAuraSpec.WithColor(color),
            amountScale);
    }

    public static LovecAura? WithAura<TController>(
        this PowerModel power,
        string scenePath,
        float amountScale = 0.05f)
        where TController : PowerAuraController
    {
        return power.WithAura<TController>(
            AuraSpec.Scene(scenePath),
            amountScale);
    }

    public static LovecAura? WithAura<TController>(
        this PowerModel power,
        AuraSpec spec,
        float amountScale = 0.05f)
        where TController : PowerAuraController
    {
        TController controller = CreateController<TController>(
            power,
            spec,
            amountScale);

        return AuraCmd.Apply(controller);
    }

    private static TController CreateController<TController>(
        PowerModel power,
        AuraSpec spec,
        float amountScale)
        where TController : PowerAuraController
    {
        Type controllerType = typeof(TController);
        Type powerType = power.GetType();

        object? controller =
            TryCreate(controllerType, [powerType, typeof(AuraSpec), typeof(float)], [power, spec, amountScale])
            ?? TryCreate(controllerType, [typeof(PowerModel), typeof(AuraSpec), typeof(float)], [power, spec, amountScale])
            ?? TryCreate(controllerType, [powerType, typeof(AuraSpec)], [power, spec])
            ?? TryCreate(controllerType, [typeof(PowerModel), typeof(AuraSpec)], [power, spec])
            ?? TryCreate(controllerType, [powerType], [power])
            ?? TryCreate(controllerType, [typeof(PowerModel)], [power]);

        if (controller is TController typed)
            return typed;

        throw new InvalidOperationException(
            $"Could not create aura controller {controllerType.Name}.");
    }

    private static object? TryCreate(
        Type type,
        Type[] parameterTypes,
        object?[] args)
    {
        ConstructorInfo? ctor = type.GetConstructor(parameterTypes);
        return ctor?.Invoke(args);
    }
}