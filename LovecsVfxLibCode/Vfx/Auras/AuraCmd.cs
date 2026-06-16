using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.TestSupport;
using System.Runtime.CompilerServices;

namespace LovecsVfxLibCode.Auras;

public static class AuraCmd
{
    public const string AuraAnchorName = "LovecAuraAnchor";
    
    public static string? DefaultAuraScenePath { get; set; }
        = "res://LovecsVfxLib/scenes/vfx/auras/DefaultLovecAura.tscn";
    
    public static LovecAura? Apply(AuraController controller)
    {
        ArgumentNullException.ThrowIfNull(controller);

        if (TestMode.IsOn)
            return null;

        Creature target = controller.Target;
        if (target == null || target.IsDead)
        {
            controller.Dispose();
            return null;
        }

        Control? vfxContainer = target.GetVfxContainer();
        NCreature? creatureNode = target.GetCreatureNode();

        if (vfxContainer == null || creatureNode == null)
        {
            GD.PushWarning($"[LovecAura] Could not resolve VFX container/creature node for {target}.");
            controller.Dispose();
            return null;
        }

        Node2D anchor = GetOrCreateAnchor(vfxContainer, target);
        return ApplyToAnchor(anchor, controller);
    }
    
    public static LovecAura ApplyToAnchor(Node anchor, AuraController controller)
    {
        ArgumentNullException.ThrowIfNull(anchor);
        ArgumentNullException.ThrowIfNull(controller);

        string key = AuraKeys.ForController(controller);

        LovecAura? existing = anchor.GetNodeOrNull<LovecAura>(key);
        if (existing != null)
        {
            existing.Bind(controller);
            UpdateAuraPosition(existing, controller);
            return existing;
        }

        LovecAura aura = InstantiateAura(controller.Spec);
        aura.Name = key;

        anchor.AddChildSafely(aura);
        aura.Bind(controller);
        UpdateAuraPosition(aura, controller);

        return aura;
    }

    public static void Remove(Creature target, string auraKey)
    {
        if (TestMode.IsOn || target == null)
            return;

        Control? vfxContainer = target.GetVfxContainer();
        if (vfxContainer == null)
            return;

        Node? anchor = GetAuraAnchor(vfxContainer, target);
        LovecAura? aura = anchor?.GetNodeOrNull<LovecAura>(auraKey);
        aura?.Remove();
    }

    public static void Remove(AuraController controller)
    {
        controller.Remove();
    }

    public static void Sync(AuraController controller)
    {
        controller.Sync();
    }

    public static Node2D GetOrCreateAnchor(Control vfxContainer, Creature target)
    {
        string anchorName = GetAnchorName(target);

        Node2D? existing = vfxContainer.GetNodeOrNull<Node2D>(anchorName);
        if (existing != null)
            return existing;

        Node2D anchor = new()
        {
            Name = anchorName,
            ZIndex = 100
        };

        vfxContainer.AddChildSafely(anchor);
        return anchor;
    }

    public static Node2D? GetAuraAnchor(Control vfxContainer, Creature target)
    {
        return vfxContainer.GetNodeOrNull<Node2D>(GetAnchorName(target));
    }

    public static string GetAnchorName(Creature target)
    {
        // VFX containers are shared, so anchors must be per-creature.
        // RuntimeHelpers hash is enough for a combat-lifetime visual node name.
        return $"{AuraAnchorName}_{RuntimeHelpers.GetHashCode(target)}";
    }

    public static LovecAura InstantiateAura(AuraSpec spec)
    {
        string? scenePath = spec.ScenePath ?? DefaultAuraScenePath;

        if (!string.IsNullOrWhiteSpace(scenePath))
        {
            string resolvedPath = AuraPathAliases.Resolve(scenePath);

            if (ResourceLoader.Exists(resolvedPath))
            {
                Node node = PreloadManager.Cache.GetScene(resolvedPath).Instantiate();

                if (node is LovecAura aura)
                    return aura;

                node.QueueFree();
                throw new InvalidOperationException(
                    $"Aura scene root must inherit LovecAura: {resolvedPath}");
            }

            GD.PushWarning($"[LovecAura] Aura scene not found: {scenePath} resolved to {resolvedPath}. Using programmatic DefaultLovecAura.");
        }

        return new DefaultLovecAura();
    }

    public static void UpdateAuraPosition(LovecAura aura, AuraController controller)
    {
        AuraSpec spec = controller.Spec;

        if (!spec.FollowTarget)
        {
            aura.Position = spec.Offset;
            return;
        }

        Vector2? position = GetAuraPosition(controller.Target, spec);
        if (position.HasValue)
            aura.GlobalPosition = position.Value;
        else
            controller.Remove();
    }

    public static Vector2? GetAuraPosition(Creature target, AuraSpec spec)
    {
        if (TestMode.IsOn || target == null || target.IsDead)
            return null;

        NCreature? creatureNode = target.GetCreatureNode();
        if (creatureNode == null)
            return null;

        Vector2 position = spec.PositionMode switch
        {
            AuraPositionMode.CreatureOrigin => creatureNode.GlobalPosition,
            AuraPositionMode.HitboxBottom => creatureNode.GetBottomOfHitbox(),
            AuraPositionMode.VfxSpawnPosition => creatureNode.VfxSpawnPosition,
            _ => creatureNode.VfxSpawnPosition
        };

        return position + spec.Offset;
    }
}
