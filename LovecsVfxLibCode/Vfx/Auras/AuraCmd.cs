using Godot;
using LovecsVfxLibCode.Auras;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.TestSupport;

namespace LovecsVfxLibCode.Vfx.Auras;

public static class AuraCmd
{
    public const string AuraAnchorName = "LovecAuraAnchor";

    public static string DefaultAuraScenePath { get; set; } =
        "res://LovecsVfxLib/scenes/vfx/auras/DefaultLovecAura.tscn";

    public static LovecAura? Apply(AuraController controller)
    {
        if (TestMode.IsOn)
            return null;

        Creature target = controller.Target;

        if (target == null || target.IsDead)
            return null;

        Control? container = target.GetVfxContainer();

        if (container == null)
            return null;

        Node anchor = GetOrCreateAnchor(container);

        string auraKey = controller.Spec.AuraKey
            ?? AuraKeys.ForController(controller);

        if (anchor.GetNodeOrNull<LovecAura>(auraKey) is { } existing)
        {
            existing.Bind(controller);
            return existing;
        }

        LovecAura? aura = InstantiateAura(controller.Spec);

        if (aura == null)
            return null;

        aura.Name = auraKey;
        anchor.AddChildSafely(aura);
        aura.Bind(controller);

        return aura;
    }
    
    public static void UpdateAuraPosition(LovecAura aura, AuraController controller)
    {
        Creature target = controller.Target;

        if (target == null || target.IsDead)
        {
            controller.Remove();
            return;
        }

        var creatureNode = target.GetCreatureNode();

        if (creatureNode == null)
            return;

        aura.GlobalPosition = creatureNode.VfxSpawnPosition + controller.Spec.Offset;
    }

    public static void Remove(AuraController controller)
    {
        controller.Remove();
    }

    public static void Sync(AuraController controller)
    {
        controller.Sync();
    }

    private static Node GetOrCreateAnchor(Node container)
    {
        if (container.GetNodeOrNull<Node>(AuraAnchorName) is { } existing)
            return existing;

        var anchor = new Node2D
        {
            Name = AuraAnchorName,
            ZIndex = 0
        };

        container.AddChildSafely(anchor);
        return anchor;
    }

    private static LovecAura? InstantiateAura(AuraSpec spec)
    {
        string path = spec.ScenePath ?? DefaultAuraScenePath;
        string resolvedPath = ResolveScenePath(path);

        if (!ResourceLoader.Exists(resolvedPath))
        {
            GD.PushWarning(
                $"[AuraCmd] Aura scene not found: {path} resolved to {resolvedPath}.");

            return null;
        }

        PackedScene scene = PreloadManager.Cache.GetScene(resolvedPath);
        Node node = scene.Instantiate();

        if (node is LovecAura aura)
            return aura;

        GD.PushWarning(
            $"[AuraCmd] Aura scene root must inherit LovecAura: {resolvedPath}");

        node.QueueFree();
        return null;
    }

    private static string ResolveScenePath(string path)
    {
        if (path.StartsWith("res://"))
            return path;

        return SceneHelper.GetScenePath(path);
    }
}