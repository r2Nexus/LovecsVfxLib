using System.Reflection;
using BaseLib.Config;
using Godot;
using HarmonyLib;
using LovecsVfxLib.LovecsVfxLibCode.Config;
using MegaCrit.Sts2.Core.Modding;

namespace LovecsVfxLib.LovecsVfxLibCode;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "LovecsVfxLib"; //Used for resource filepath
    public const string ResPath = $"res://{ModId}";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } =
        new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        ModConfigRegistry.Register(ModId, new LovecsVfxConfig());
        
        var assembly = Assembly.GetExecutingAssembly();
        Godot.Bridge.ScriptManagerBridge.LookupScriptsInAssembly(assembly);
        
        Harmony harmony = new(ModId);

        harmony.PatchAll();
    }
}