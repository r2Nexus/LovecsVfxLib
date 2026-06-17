using System.Runtime.CompilerServices;
using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.TestSupport;

namespace LovecsVfxLibCode.Vfx.Auras;

public static partial class AuraCmd
{
    public const string AuraAnchorPrefix = "LovecAuraAnchor";

    public static string DefaultAuraScenePath { get; set; } =
        "res://LovecsVfxLib/scenes/vfx/auras/DefaultLovecAura.tscn";

    public static LovecAura? Apply(
        AuraController controller,
        string? scenePath = null)
    {
        if (TestMode.IsOn)
            return null;

        // Put this FIRST while debugging.
        // If CompleteSpec still does not print, WithAura/AuraCmd.Apply is not being reached.

        Creature target = controller.Target;

        if (target == null || target.IsDead)
            return null;

        Control? container = target.GetVfxContainer();

        if (container == null)
        {
            GD.PushWarning($"[AuraCmd] No VFX container for {target}.");
            return null;
        }

        AuraAnchor anchor = GetOrCreateAnchor(container, target);

        string auraKey = AuraKeys.ForController(controller);
        string nodeName = ToNodeName(auraKey);

        if (anchor.GetNodeOrNull<LovecAura>(nodeName) is { } existing)
        {
            existing.Bind(controller);
            return existing;
        }

        LovecAura? aura = InstantiateAura(scenePath);

        if (aura == null)
            return null;

        aura.Name = nodeName;
        aura.Position = controller.Spec.Offset;

        anchor.AddChildSafely(aura);
        aura.Bind(controller);

        return aura;
    }

    private static LovecAura? InstantiateAura(string? scenePath)
    {
        string path = scenePath ?? DefaultAuraScenePath;

        PackedScene? scene = GD.Load<PackedScene>(path);

        if (scene == null)
        {
            GD.PushWarning($"[AuraCmd] Failed to load aura scene: {path}");
            return null;
        }

        Node node = scene.Instantiate();

        if (node is LovecAura aura)
            return aura;

        GD.PushWarning(
            $"[AuraCmd] Aura scene root must inherit LovecAura: {path}. " +
            $"Actual root type: {node.GetType().FullName}");

        node.QueueFree();
        return null;
    }

    public static void Remove(AuraController controller)
    {
        controller.Remove();
    }

    public static void Sync(AuraController controller)
    {
        controller.Sync();
    }

    private static AuraAnchor GetOrCreateAnchor(Control container, Creature target)
    {
        string anchorName = GetAnchorName(target);

        if (container.GetNodeOrNull<AuraAnchor>(anchorName) is { } existing)
            return existing;

        AuraAnchor anchor = new()
        {
            Name = anchorName,
            Target = target,
            ZIndex = 100
        };

        container.AddChildSafely(anchor);
        anchor.SyncPositionNow();

        return anchor;
    }

    private static string GetAnchorName(Creature target)
    {
        return $"{AuraAnchorPrefix}_{RuntimeHelpers.GetHashCode(target)}";
    }

    private static string ToNodeName(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return "Aura";

        char[] chars = key.ToCharArray();

        for (int i = 0; i < chars.Length; i++)
        {
            char c = chars[i];

            if (!char.IsLetterOrDigit(c) && c != '_')
                chars[i] = '_';
        }

        return new string(chars);
    }

    public partial class AuraAnchor : Node2D
    {
        public Creature? Target { get; set; }

        public override void _Ready()
        {
            SyncPositionNow();
        }

        public override void _Process(double delta)
        {
            if (Target == null || Target.IsDead)
            {
                QueueFree();
                return;
            }

            SyncPositionNow();
        }

        public void SyncPositionNow()
        {
            if (Target == null || TestMode.IsOn)
                return;

            NCreature? creatureNode = Target.GetCreatureNode();

            if (creatureNode == null)
                return;

            GlobalPosition = creatureNode.VfxSpawnPosition;
        }
    }
}