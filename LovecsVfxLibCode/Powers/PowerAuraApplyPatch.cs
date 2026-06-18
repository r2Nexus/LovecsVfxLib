using System;
using System.Collections.Generic;
using System.Reflection;
using Godot;
using HarmonyLib;
using LovecsVfxLibCode.Vfx.Auras;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace LovecsVfxLib.LovecsVfxLibCode.Powers;

[HarmonyPatch]
public static class PowerAuraApplyPatch
{
    public static MethodBase TargetMethod()
    {
        return AccessTools.Method(
            typeof(PowerCmd),
            nameof(PowerCmd.Apply),
            new[]
            {
                typeof(PlayerChoiceContext),
                typeof(PowerModel),
                typeof(Creature),
                typeof(decimal),
                typeof(Creature),
                typeof(CardModel),
                typeof(bool)
            });
    }

    public static void Postfix(Task __result, PowerModel power)
    {
        _ = ApplyAfterOriginal(__result, power);
    }

    private static async Task ApplyAfterOriginal(Task originalTask, PowerModel power)
    {
        await originalTask;
        PowerAuraDefinitions.TryApply(power);
    }
}