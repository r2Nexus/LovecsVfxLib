namespace LovecsVfxLibCode.Vfx.Auras;

public interface IVfxMarker
{
    string SlotName { get; }
    bool Required { get; }
    void Apply(VfxSlotValue value);
}
