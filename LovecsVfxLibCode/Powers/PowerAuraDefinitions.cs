using Godot;
using LovecsVfxLibCode.Vfx.Auras;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace LovecsVfxLib.LovecsVfxLibCode.Powers;

public static class PowerAuraDefinitions
{
    private const string ModId = "LovecsVfxLib";
    private const string AuraFolder = "res://LovecsVfxLib/scenes/vfx/auras/";

    private sealed record AuraDef(
        string Key,
        string Scene,
        Action<PowerModel, AuraBuilder> Configure);

    private static readonly Dictionary<Type, AuraDef> Auras = new()
    {
        [typeof(DoomPower)] = Aura<DoomPower>(
            key: "DoomAura",
            sceneFile: "enchanted_aura.tscn",
            configure: (doom, aura) =>
            {
                aura.Set("tint", Colors.Purple);
                aura.SetPowerAmountRange(
                    () => 0m,
                    () => doom.Owner.CurrentHp);
            }),
        [typeof(StrengthPower)] = Aura<StrengthPower>(
            key: "StrengthAura",
            sceneFile: "DefaultLovecAura.tscn",
            configure: (strength, aura) =>
            {
                aura.Set("tint", Colors.Tomato);
                aura.UsePowerIcon();
                aura.SetPowerAmountRange(
                    () => 0m,
                    () => 15m);
            }),
        
        [typeof(DexterityPower)] = Aura<DexterityPower>(
            key: "DexterityAura",
            sceneFile: "guard_aura.tscn",
            configure: (dexterity, aura) =>
            {
                aura.UsePowerIcon();
                aura.Set("tint", Colors.ForestGreen);
                aura.SetPowerAmountRange(
                    () => 0m,
                    () => 8m);
            }),
        
        [typeof(FrailPower)] = Aura<FrailPower>(
            key: "FrailAura",
            sceneFile: "guard_aura.tscn",
            configure: (frail, aura) =>
            {
                aura.UsePowerIcon();
                aura.Set("tint", Colors.DeepSkyBlue);
                aura.SetPowerAmountRange(
                    () => 0m,
                    () => 8m);
            }),
        
        [typeof(VulnerablePower)] = Aura<VulnerablePower>(
            key: "VulnerableAura",
            sceneFile: "guard_aura.tscn",
            configure: (vulnerable, aura) =>
            {
                aura.UsePowerIcon();
                aura.Set("tint", Colors.Tomato);
                aura.SetPowerAmountRange(
                    () => 0m,
                    () => 12m);
            }),
        
        [typeof(FocusPower)] = Aura<FocusPower>(
            key: "FocusAura",
            sceneFile: "focus_aura.tscn",
            configure: (vulnerable, aura) =>
            {
                aura.UsePowerIcon();
                aura.Set("tint", Colors.Cyan);
                aura.SetPowerAmountRange(
                    () => 0m,
                    () => 10m);
            })
    };

    public static bool TryApply(PowerModel power)
    {
        if (power.Owner == null || power.Owner.IsDead)
            return false;

        if (!Auras.TryGetValue(power.GetType(), out AuraDef? def))
            return false;

        power.WithAura(def.Scene)
            .WithKey(def.Key)
            .Configure(aura => def.Configure(power, aura));

        return true;
    }

    private static AuraDef Aura<TPower>(
        string key,
        string sceneFile,
        Action<TPower, AuraBuilder> configure)
        where TPower : PowerModel
    {
        return new AuraDef(
            Key: $"{ModId}:{key}",
            Scene: AuraFolder + sceneFile,
            Configure: (power, aura) => configure((TPower)power, aura));
    }
}