using System.Collections.Generic;
using Godot;

namespace LovecsVfxLibCode.Vfx.Auras;

public static class VfxMarkerScanner
{
    public static void Scan(Node node, ICollection<IVfxMarker> markers, ICollection<IVfxStateReceiver> stateReceivers)
    {
        if (node is IVfxMarker csharpMarker)
            markers.Add(csharpMarker);
        else if (GdVfxMarkerAdapter.TryCreate(node, out GdVfxMarkerAdapter gdMarker))
            markers.Add(gdMarker);

        if (node is IVfxStateReceiver csharpReceiver)
            stateReceivers.Add(csharpReceiver);
        else if (GdVfxStateReceiverAdapter.TryCreate(node, out GdVfxStateReceiverAdapter gdReceiver))
            stateReceivers.Add(gdReceiver);

        foreach (Node child in node.GetChildren())
            Scan(child, markers, stateReceivers);
    }
}
