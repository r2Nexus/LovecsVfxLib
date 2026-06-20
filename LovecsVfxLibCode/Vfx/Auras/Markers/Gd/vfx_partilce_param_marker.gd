extends Node
class_name VfxPartilceParamMarker

# Used for marking a GPUParticles2D node's ParticleProcessMaterial parameter range.

@export var particle_param_marker: bool = true

@export var slot_name: String = "scale"
@export var required: bool = false
@export var target_path: NodePath

# ParticleProcessMaterial.Parameter.Scale = 8
@export var parameter: int = 8