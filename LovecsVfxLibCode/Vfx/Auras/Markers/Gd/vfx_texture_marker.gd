extends Node
class_name VfxTextureMarker

enum TextureTarget {
	AUTO,
	TEXTURE,
	PARTICLE_TEXTURE
}

@export var slot_name: String = "icon"
@export var required: bool = false
@export var target_path: NodePath
@export var texture_target: TextureTarget = TextureTarget.AUTO
