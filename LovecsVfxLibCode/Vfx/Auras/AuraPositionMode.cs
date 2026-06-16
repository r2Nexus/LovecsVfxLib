namespace LovecsVfxLibCode.Auras;

public enum AuraPositionMode
{
    /// <summary>
    /// Same point used by VfxCmd.PlayOnCreatureCenter.
    /// Best default for persistent status/aura visuals.
    /// </summary>
    VfxSpawnPosition,

    /// <summary>
    /// Creature node origin/global position.
    /// Similar to VfxCmd.PlayOnCreature.
    /// </summary>
    CreatureOrigin,

    /// <summary>
    /// Bottom of the hitbox. Useful for ground rings, smoke, puddles, shadows, etc.
    /// </summary>
    HitboxBottom
}
