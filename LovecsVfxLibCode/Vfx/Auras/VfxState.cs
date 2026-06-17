namespace LovecsVfxLibCode.Vfx.Auras;

public readonly record struct VfxState(
    bool Active,
    decimal Amount,
    double Delta = 0d);
