using Godot;
using LovecsVfxLib.LovecsVfxLibCode.Config;
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
        Func<bool> IsEnabled,
        Action<PowerModel, AuraBuilder> Configure);

    private static readonly Dictionary<Type, AuraDef> Auras = new()
    {
        [typeof(DoomPower)] = Aura<DoomPower>(
            key: "DoomAura",
            isEnabled: () => LovecsVfxConfig.EnableDoomAura,
            sceneFile: LovecAuras.Enchanted,
            configure: (doom, aura) =>
            {
                aura.Set("tint", Colors.Purple);
                aura.SetPowerAmountRange(
                    () => 0m,
                    () => doom.Owner.CurrentHp);
            }),
        
        [typeof(PoisonPower)] = Aura<PoisonPower>(
            key: "PoisonAura",
            isEnabled: () => LovecsVfxConfig.EnablePoisonAura,
            sceneFile: LovecAuras.Enchanted,
            configure: (doom, aura) =>
            {
                aura.Set("tint", Colors.Green);
                aura.SetSpriteSheet("sheet","res://LovecsVfxLib/images/vfx/skull.png", 2, 2, true, 2f,4f);
                aura.SetParticleScale("scale", 0.5f,1f);
                aura.SetPowerAmountRange(
                    () => 0m,
                    () => doom.Owner.CurrentHp);
            }),
        
        [typeof(StrengthPower)] = Aura<StrengthPower>(
            key: "StrengthAura",
            isEnabled: () => LovecsVfxConfig.EnableStrengthAura,
            sceneFile: LovecAuras.Default,
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
            isEnabled: () => LovecsVfxConfig.EnableDexterityAura,
            sceneFile: LovecAuras.Guard,
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
            isEnabled: () => LovecsVfxConfig.EnableFrailAura,
            sceneFile: LovecAuras.Guard,
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
            isEnabled: () => LovecsVfxConfig.EnableVulnerableAura,
            sceneFile: LovecAuras.Guard,
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
            isEnabled: () => LovecsVfxConfig.EnableFocusAura,
            sceneFile: LovecAuras.Focus,
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
        if (!LovecsVfxConfig.EnableVanillaPowerAuras)
            return false;

        if (!Auras.TryGetValue(power.GetType(), out AuraDef? def))
            return false;

        if (!def.IsEnabled())
            return false;

        power.WithAura(def.Scene)
            .WithKey($"{ModId}:{def.Key}")
            .Configure(aura => def.Configure(power, aura));

        return true;
    }

    private static AuraDef Aura<TPower>(
        string key,
        string sceneFile,
        Func<bool> isEnabled,
        Action<TPower, AuraBuilder> configure)
        where TPower : PowerModel
    {
        return new AuraDef(
            key,
            sceneFile,
            isEnabled,
            (power, aura) => configure((TPower)power, aura));
    }
}