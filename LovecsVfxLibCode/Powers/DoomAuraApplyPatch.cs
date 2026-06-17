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
public static class DoomAuraApplyPatch
{
    private const string DoomAuraKey = "LovecsVfxLib:DoomAura";
    private const string DoomAuraScene = "res://LovecsVfxLib/scenes/vfx/auras/enchanted_aura.tscn";

    private static readonly HashSet<DoomPower> Applied = new();

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

        if (power is DoomPower doom)
            ApplyDoomAura(doom);
    }

    private static void ApplyDoomAura(DoomPower doom)
    {
        if (doom.Owner == null || doom.Owner.IsDead)
            return;

        if (!Applied.Add(doom))
            return;

        doom.WithAuraNoIcon(DoomAuraScene)
            .WithKey(DoomAuraKey)
            .Configure(aura =>
            {
                aura.Set("tint", Colors.Purple);
                aura.SetPowerAmountRange(
                    () => 0m,
                    () => (decimal)doom.Owner.CurrentHp);
            });
    }
}