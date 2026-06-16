using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.TestSupport;

namespace LovecsVfxLibCode.Vfx.Auras;

public static class AuraCmd
{
    public const string AuraAnchorName = "LovecAuraAnchor";

    public static string DefaultAuraScenePath { get; set; } =
        "res://LovecsVfxLib/scenes/vfx/auras/DefaultLovecAura.tscn";

    public static LovecAura? Apply(
        AuraController controller,
        string? scenePath = null)
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

        controller.Prepare();
        
        string auraKey = controller.Spec.AuraKey
                         ?? AuraKeys.ForController(controller, scenePath);

        if (anchor.GetNodeOrNull<LovecAura>(auraKey) is { } existing)
        {
            existing.Bind(controller);
            return existing;
        }

        LovecAura? aura = InstantiateAura(scenePath);

        if (aura == null)
            return null;

        aura.Name = auraKey;
        anchor.AddChildSafely(aura);
        aura.Bind(controller);

        return aura;
    }

    private static LovecAura? InstantiateAura(string? scenePath)
    {
        string path = scenePath ?? DefaultAuraScenePath;
        string resolvedPath = ResolveScenePath(path);

        PackedScene? scene = GD.Load<PackedScene>(resolvedPath);

        if (scene == null)
        {
            GD.PushWarning(
                $"[AuraCmd] Aura scene failed to load: {path} resolved to {resolvedPath}.");

            return null;
        }

        Node node = scene.Instantiate();

        if (node is LovecAura aura)
            return aura;

        GD.PushWarning(
            $"[AuraCmd] Aura scene root must inherit LovecAura: {resolvedPath}. " +
            $"Actual root type: {node.GetType().FullName}");

        node.QueueFree();
        return null;
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

    private static string ResolveScenePath(string path)
    {
        if (path.StartsWith("res://"))
            return path;

        return SceneHelper.GetScenePath(path);
    }
}