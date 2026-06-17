extends Node
class_name VfxParticlePowerScaling
#Used for marking node as able to scale particle power

@export var target_path: NodePath
@export var min_power_amount: float = 0.0
@export var max_power_amount: float = 10.0
@export var min_speed_multiplier: float = 1.0
@export var max_speed_multiplier: float = 1.0
