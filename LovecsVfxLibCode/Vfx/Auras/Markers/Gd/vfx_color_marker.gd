extends Node
class_name VfxColorMarker
#Used for marking node with changable color

enum ColorTarget {
	MODULATE,
	SELF_MODULATE,
	PARTICLE_PROCESS_MATERIAL_COLOR,
	LIGHT_COLOR
}

@export var slot_name: String = "tint"
@export var required: bool = false
@export var target_path: NodePath
@export var color_target: ColorTarget = ColorTarget.MODULATE
