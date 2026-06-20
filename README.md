# LovecsVfxLib

Small VFX helper library for Slay the Spire 2 / Godot mods.

The library currently provides two main things:

* simple one-shot VFX helpers, such as splash, explosion, hex, bite, charm, and bloom effects
* reusable power aura scenes that can be attached to powers and configured from C#

## Quick aura example

```csharp
using Godot;
using LovecsVfxLibCode.Vfx.Auras;

power.WithAura(LovecAuras.Enchanted)
    .WithKey("MyMod:MyPowerAura")
    .Configure(aura =>
    {
        aura.Set(VfxSlots.Tint, Colors.Purple);

        aura.SetPowerAmountRange(
            () => 0m,
            () => power.Owner.CurrentHp);
    });
```

This attaches the `Enchanted` aura scene to the power owner, tints the aura purple, and scales the aura based on the power amount range.

## Built-in aura templates

Use `LovecAuras` instead of writing raw scene paths manually.

```csharp
power.WithAura(LovecAuras.Default);
power.WithAura(LovecAuras.Enchanted);
power.WithAura(LovecAuras.Guard);
power.WithAura(LovecAuras.Focus);
```

For aura scenes from your own mod, pass the full Godot path directly:

```csharp
power.WithAura("res://MyMod/scenes/vfx/auras/my_custom_aura.tscn");
```

## Common aura options

```csharp
power.WithAura(LovecAuras.Default)
    .Configure(aura =>
    {
        aura.Set(VfxSlots.Tint, Colors.Tomato);
        aura.UsePowerIcon();
        aura.Offset(new Vector2(0, -40));
        aura.WithKey("MyMod:UniqueAuraKey");
    });
```

### `Set`

Replaces a named scene slot.

```csharp
aura.Set(VfxSlots.Tint, Colors.DeepSkyBlue);
aura.Set(VfxSlots.Icon, texture);
```

### `UsePowerIcon`

Uses the icon of the attached power.

```csharp
aura.UsePowerIcon();
```

By default this writes to the `icon` slot.

### `SetSpriteSheet`

Replaces a sprite sheet slot.

```csharp
aura.SetSpriteSheet(
    VfxSlots.Sheet,
    "res://MyMod/images/vfx/flame_sheet.png",
    hFrames: 4,
    vFrames: 4,
    loop: true,
    animSpeedMin: 0.8f,
    animSpeedMax: 1.2f);
```

### `SetParticleScale`

Changes the particle scale range on scenes that expose a particle scale marker.

```csharp
aura.SetParticleScale("scale", 0.5f, 1.2f);
```

You can also set a fixed particle scale by passing only one scale value:

```csharp
aura.SetParticleScale("scale", 0.8f);
```

Some scenes expose separate scale slots for different particle groups:

```csharp
aura.SetParticleScale("iconScale", 0.8f, 1.2f);
aura.SetParticleScale("fragmentScale", 0.4f, 0.9f);
```

### `SetPowerAmountRange`

Controls how strongly the aura reacts to the current power amount.

```csharp
aura.SetPowerAmountRange(() => 0m, () => 10m);
```

You can use static values:

```csharp
aura.SetPowerAmountRange(0m, 10m);
```

Or dynamic providers:

```csharp
aura.SetPowerAmountRange(
    () => 0m,
    () => power.Owner.CurrentHp);
```

### `WithKey`

Sets a stable aura key. Use this when you want one specific aura instance to update instead of creating duplicates.

```csharp
aura.WithKey("MyMod:StrengthAura");
```

## Scene slots

Aura scenes use marker nodes to expose configurable slots.

The most common slot names are available in `VfxSlots`:

```csharp
VfxSlots.Icon
VfxSlots.Particle
VfxSlots.Tint
VfxSlots.Sheet
VfxSlots.Fragments
```

Scale slots are currently scene-specific strings:

```csharp
"scale"
"iconScale"
"fragmentScale"
```

All marker replacements are optional. If you do not set a supported slot from C#, the scene keeps its default value.

### `LovecAuras.Default`

General-purpose aura with icon particles and colored fragment particles.

| Marker                               | Slot / API                                    | Affects                                                                                          |
| ------------------------------------ | --------------------------------------------- | ------------------------------------------------------------------------------------------------ |
| `IconTextureMarker`                  | `VfxSlots.Icon` / `"icon"`                    | Replaces the texture used by the icon particles. This is the slot used by `aura.UsePowerIcon()`. |
| `IconPowerScaling`                   | `aura.SetPowerAmountRange(...)`               | Scales the icon particle speed based on the current power amount.                                |
| `ScaleMarker` under `IconParticles`  | `aura.SetParticleScale("iconScale", ...)`     | Changes the scale range of the icon particles.                                                   |
| `FragmentTextureMarker`              | `VfxSlots.Sheet` / `"sheet"`                  | Replaces the sprite sheet used by the colored fragment particles.                                |
| `TintMarker`                         | `VfxSlots.Tint` / `"tint"`                    | Changes the color of the fragment particles.                                                     |
| `ColorPowerScaling`                  | `aura.SetPowerAmountRange(...)`               | Scales the fragment particle speed based on the current power amount.                            |
| `ScaleMarker` under `ColorParticles` | `aura.SetParticleScale("fragmentScale", ...)` | Changes the scale range of the fragment particles.                                               |

Example:

```csharp
power.WithAura(LovecAuras.Default)
    .Configure(aura =>
    {
        aura.UsePowerIcon();
        aura.Set(VfxSlots.Tint, Colors.Gold);

        aura.SetParticleScale("iconScale", 0.7f, 1.1f);
        aura.SetParticleScale("fragmentScale", 0.4f, 0.8f);

        aura.SetPowerAmountRange(
            () => 0m,
            () => 10m);
    });
```

### `LovecAuras.Enchanted`

Magical floating-symbol aura. Good for curses, doom, hexes, poison, enchantments, or abstract power effects.

| Marker               | Slot / API                            | Affects                                                             |
| -------------------- | ------------------------------------- | ------------------------------------------------------------------- |
| `TintMarker`         | `VfxSlots.Tint` / `"tint"`            | Changes the color of the floating symbol particles.                 |
| `SpriteSheetMarker`  | `VfxSlots.Sheet` / `"sheet"`          | Replaces the sprite sheet used by the floating symbols.             |
| `PowerScalingMarker` | `aura.SetPowerAmountRange(...)`       | Scales the symbol particle speed based on the current power amount. |
| `ScaleMarker`        | `aura.SetParticleScale("scale", ...)` | Changes the scale range of the floating symbol particles.           |

Example:

```csharp
power.WithAura(LovecAuras.Enchanted)
    .Configure(aura =>
    {
        aura.Set(VfxSlots.Tint, Colors.Purple);

        aura.SetSpriteSheet(
            VfxSlots.Sheet,
            "res://MyMod/images/vfx/magic_symbols.png",
            hFrames: 25,
            vFrames: 1,
            loop: true);

        aura.SetParticleScale("scale", 0.5f, 1.2f);

        aura.SetPowerAmountRange(
            () => 0m,
            () => power.Owner.CurrentHp);
    });
```

`LovecAuras.Enchanted` does not currently expose an `icon` marker, so `aura.UsePowerIcon()` has no useful target unless you add one to the scene.

### `LovecAuras.Guard`

Defensive aura with larger icon particles and smaller animated fragment particles. Good for block, armor, shields, guard effects, or protection powers.

| Marker                                 | Slot / API                                    | Affects                                                                                                 |
| -------------------------------------- | --------------------------------------------- | ------------------------------------------------------------------------------------------------------- |
| `IconTexture`                          | `VfxSlots.Icon` / `"icon"`                    | Replaces the texture used by the larger icon particles. This is the slot used by `aura.UsePowerIcon()`. |
| `PowerScaling` under `GPUParticles2D`  | `aura.SetPowerAmountRange(...)`               | Scales the larger icon particle speed based on the current power amount.                                |
| `ScaleMarker` under `GPUParticles2D`   | `aura.SetParticleScale("iconScale", ...)`     | Changes the scale range of the larger icon particles.                                                   |
| `SpriteSheet`                          | `VfxSlots.Sheet` / `"sheet"`                  | Replaces the sprite sheet used by the smaller animated fragment particles.                              |
| `Tint`                                 | `VfxSlots.Tint` / `"tint"`                    | Changes the color of the smaller animated fragment particles.                                           |
| `PowerScaling` under `GPUParticles2D2` | `aura.SetPowerAmountRange(...)`               | Scales the smaller fragment particle speed based on the current power amount.                           |
| `ScaleMarker` under `GPUParticles2D2`  | `aura.SetParticleScale("fragmentScale", ...)` | Changes the scale range of the smaller fragment particles.                                              |

Example:

```csharp
power.WithAura(LovecAuras.Guard)
    .Configure(aura =>
    {
        aura.UsePowerIcon();
        aura.Set(VfxSlots.Tint, Colors.LightBlue);

        aura.SetParticleScale("iconScale", 0.9f, 1.4f);
        aura.SetParticleScale("fragmentScale", 0.6f, 1.1f);

        aura.SetPowerAmountRange(
            () => 0m,
            () => 12m);
    });
```

### `LovecAuras.Focus`

Energetic aura with particles spawning around the target that linger around before fizzling out of existence.

| Marker                                 | Slot / API                                    | Affects                                                                                                  |
| -------------------------------------- | --------------------------------------------- | -------------------------------------------------------------------------------------------------------- |
| `Icon`                                 | `VfxSlots.Icon` / `"icon"`                    | Replaces the texture used by the larger focus particles. This is the slot used by `aura.UsePowerIcon()`. |
| `PowerScaling` under `GPUParticles2D`  | `aura.SetPowerAmountRange(...)`               | Scales the larger focus particle speed based on the current power amount.                                |
| `ScaleMarker` under `GPUParticles2D`   | `aura.SetParticleScale("iconScale", ...)`     | Changes the scale range of the larger focus particles.                                                   |
| `SpriteSheet`                          | `VfxSlots.Sheet` / `"sheet"`                  | Replaces the sprite sheet used by the smaller animated particles.                                        |
| `Tint`                                 | `VfxSlots.Tint` / `"tint"`                    | Changes the color of the smaller animated particles.                                                     |
| `PowerScaling` under `GPUParticles2D2` | `aura.SetPowerAmountRange(...)`               | Scales the smaller particle speed based on the current power amount.                                     |
| `ScaleMarker` under `GPUParticles2D2`  | `aura.SetParticleScale("fragmentScale", ...)` | Changes the scale range of the smaller animated particles.                                               |

Example:

```csharp
power.WithAura(LovecAuras.Focus)
    .Configure(aura =>
    {
        aura.UsePowerIcon();
        aura.Set(VfxSlots.Tint, Colors.Cyan);

        aura.SetParticleScale("iconScale", 0.7f, 1.1f);
        aura.SetParticleScale("fragmentScale", 0.4f, 0.8f);

        aura.SetPowerAmountRange(
            () => 0m,
            () => 10m);
    });
```

## Creating custom aura scenes

A custom aura scene should inherit from `LovecAura`.

Use marker nodes to expose configurable parts of the scene:

* `VfxTextureMarker` for texture or particle texture replacement
* `VfxColorMarker` for color/tint replacement
* `VfxSpriteSheetMarker` for sprite sheet replacement
* `VfxParticlePowerScaling` for amount-based particle speed scaling
* particle parameter marker for particle material parameter ranges, such as scale range

Each marker has a `slot_name`. C# code uses that same slot name when calling `aura.Set(...)`, `aura.SetSpriteSheet(...)`, or `aura.SetParticleScale(...)`.

Example:

```csharp
aura.Set("tint", Colors.Purple);
aura.Set("icon", texture);

aura.SetSpriteSheet(
    "sheet",
    "res://MyMod/images/vfx/sheet.png",
    hFrames: 4,
    vFrames: 4);

aura.SetParticleScale("scale", 0.5f, 1.2f);
```

## One-shot VFX

`LovecVfx` contains quick helpers for simple vanilla-compatible VFX.

```csharp
using LovecsVfxLib;

LovecVfx.PurpleSplash(target);
LovecVfx.Explosion(target);
LovecVfx.Hex(target);
LovecVfx.Charm(target);
LovecVfx.Bite(target);
LovecVfx.Bloom(target);
```

## Notes

This library is still experimental. API names and built-in scenes may change while the aura system is being finalized.
