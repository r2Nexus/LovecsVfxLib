using Godot;
using MegaCrit.Sts2.Core.Helpers;

namespace LovecsVfxLibCode.Auras;

public static class AuraPathAliases
{
    private static readonly Dictionary<string, string> Aliases = new(StringComparer.OrdinalIgnoreCase);

    public static void Add(string alias, string path)
    {
        if (string.IsNullOrWhiteSpace(alias))
            throw new ArgumentException("Alias cannot be empty.", nameof(alias));

        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path cannot be empty.", nameof(path));

        Aliases[alias] = path;
    }

    public static void AddRange(IEnumerable<KeyValuePair<string, string>> aliases)
    {
        foreach (KeyValuePair<string, string> pair in aliases)
            Add(pair.Key, pair.Value);
    }

    public static bool Remove(string alias) => Aliases.Remove(alias);

    public static void Clear() => Aliases.Clear();

    public static string Resolve(string path)
    {
        if (Aliases.TryGetValue(path, out string? aliased))
            path = aliased;

        // Direct Godot path, e.g. res://my_mod/scenes/auras/fire_aura.tscn
        if (ResourceLoader.Exists(path))
            return path;

        // Base-game/VfxCmd style path, e.g. vfx/vfx_attack_blunt
        string scenePath = SceneHelper.GetScenePath(path);
        if (ResourceLoader.Exists(scenePath))
            return scenePath;

        // Return the original-ish value so callers can print useful warnings.
        return path;
    }
}
