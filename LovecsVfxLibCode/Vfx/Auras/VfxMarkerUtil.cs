using Godot;

namespace LovecsVfxLibCode.Vfx.Auras;

public static class VfxMarkerUtil
{
    public static void Collect(
        Node root,
        List<IVfxMarker> markers,
        List<IVfxStateReceiver> stateReceivers)
    {
        if (root is IVfxMarker marker)
            markers.Add(marker);

        if (root is IVfxStateReceiver stateReceiver)
            stateReceivers.Add(stateReceiver);

        foreach (Node child in root.GetChildren())
            Collect(child, markers, stateReceivers);
    }

    public static void ApplySlots(
        IEnumerable<IVfxMarker> markers,
        IReadOnlyDictionary<string, VfxSlotValue> slots)
    {
        HashSet<string> applied = new(StringComparer.OrdinalIgnoreCase);

        foreach (IVfxMarker marker in markers)
        {
            if (string.IsNullOrWhiteSpace(marker.SlotName))
                continue;

            if (slots.TryGetValue(marker.SlotName, out VfxSlotValue value))
            {
                marker.Apply(value);
                applied.Add(marker.SlotName);
            }
            else if (marker.Required)
            {
                GD.PushWarning($"[VfxMarkerUtil] Required VFX slot '{marker.SlotName}' was not provided.");
            }
        }

        foreach (string key in slots.Keys)
        {
            if (!applied.Contains(key))
                GD.PushWarning($"[VfxMarkerUtil] VFX slot value '{key}' was provided, but no marker used it.");
        }
    }

    public static void ApplyState(
        IEnumerable<IVfxStateReceiver> stateReceivers,
        VfxState state)
    {
        foreach (IVfxStateReceiver receiver in stateReceivers)
            receiver.Apply(state);
    }
}
