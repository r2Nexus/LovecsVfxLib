using System.Reflection;
using Godot;
using MegaCrit.Sts2.Core.Models;

namespace LovecsVfxLibCode.Vfx.Auras;

public static class AuraPowerUtil
{
    public static Texture2D? TryGetPowerIcon(PowerModel power)
    {
        if (power == null)
            return null;

        Type type = power.GetType();

        // Prefer already-loaded texture properties if a concrete implementation exposes one.
        foreach (string propertyName in new[] { "Icon", "PackedIcon", "CustomIcon" })
        {
            object? value = GetPropertyValue(type, power, propertyName);
            if (value is Texture2D texture)
                return texture;

            if (value is string texturePath)
                return LoadTextureOrNull(texturePath);
        }

        // BaseLib custom powers commonly expose icon paths rather than Texture2D instances.
        foreach (string propertyName in new[]
                 {
                     "CustomPackedIconPath",
                     "CustomIconPath",
                     "PackedIconPath",
                     "IconPath"
                 })
        {
            object? value = GetPropertyValue(type, power, propertyName);
            if (value is string texturePath)
                return LoadTextureOrNull(texturePath);
        }

        return null;
    }

    private static object? GetPropertyValue(Type type, object instance, string propertyName)
    {
        PropertyInfo? property = type.GetProperty(
            propertyName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (property == null)
            return null;

        try
        {
            return property.GetValue(instance);
        }
        catch
        {
            return null;
        }
    }

    private static Texture2D? LoadTextureOrNull(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return null;

        try
        {
            return GD.Load<Texture2D>(path);
        }
        catch
        {
            GD.PushWarning($"[AuraPowerUtil] Failed to load power icon: {path}");
            return null;
        }
    }
}
