extends Node
class_name VfxSpriteSheetMarker

# Presence marker used by the C# adapter to identify this marker kind.
@export var sprite_sheet_marker: bool = true

@export var slot_name: String = "sheet"
@export var required: bool = false
@export var target_path: NodePath
