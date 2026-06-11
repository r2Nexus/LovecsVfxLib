using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace LovecsVfxLib;

public static class LovecVfx
{
    private const string Root = "res://LovecsVfxLib/scenes/vfx/";

    public static Node2D? PurpleSplash(Creature target)
        => Hit(target, "purple_splash");
    
    public static Node2D? Explosion(Creature target)
        => Hit(target, "explosion");

    public static Node2D? Hex(Creature target)
        => Hit(target, "hex/hex");

    public static Node2D? Charm(Creature target)
        => Hit(target, "charm");

    public static Node2D? Bite(Creature target)
        => Hit(target, "bite");

    public static Node2D? Bloom(Creature target)
        => Hit(target, "bloom/bloom");

    private static Node2D? Hit(Creature target, string name)
    {
        string path = $"{Root}{name}.tscn";

        var scene = GD.Load<PackedScene>(path);
        if (scene == null)
        {
            GD.PushError($"[LovecsVfxLib] Failed to load VFX: {path}");
            return null;
        }

        var node = scene.Instantiate<Node2D>();

        var creatureNode = target.GetCreatureNode();
        if (creatureNode != null)
            node.GlobalPosition = creatureNode.VfxSpawnPosition;

        return node;
    }
}