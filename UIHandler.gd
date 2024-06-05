extends Node

# Public fields
@export
var label_offset: Vector2
@export
var has_control_over_pitch_roll: bool

# Serialized fields
@export
var pitch_text: Label
@export
var roll_text: Label
@export
var slider_pitch_label: Label
@export
var slider_roll_label: Label
@export
var pitch_slider: HSlider
@export
var roll_slider: HSlider
@export
var cylinder_panels: Array = []
@export
var cylinder_labels: Array = []

func _ready():
	# Initialize references to UI components
	cylinder_panels = [$CylinderPanel0, $CylinderPanel1, $CylinderPanel2, $CylinderPanel3]
	cylinder_labels = [$CylinderLabel0, $CylinderLabel1, $CylinderLabel2, $CylinderLabel3]

func _process(delta):
	# Update the pitch and roll slider labels
	set_pitch_slider()
	set_roll_slider()

# This method is responsible for updating the pitch label
func update_pitch(pitch: float):
	pitch_text.text = "Pitch: %.2f" % pitch

# This method is responsible for updating the roll label
func update_roll(roll: float):
	roll_text.text = "Roll: %.2f" % roll

# This method is responsible for setting the label of the pitch slider
func set_pitch_slider():
	if has_control_over_pitch_roll:
		slider_pitch_label.text = "Pitch: %.2f" % pitch_slider.value

# This method is responsible for setting the label of the roll slider
func set_roll_slider():
	if has_control_over_pitch_roll:
		slider_roll_label.text = "Roll: %.2f" % roll_slider.value

# This method is responsible for updating the position of a cylinder of the given index
func set_cylinder_panel_position(index: int, new_position: Vector2):
	cylinder_panels[index].rect_position = new_position + label_offset

# This method is responsible for updating the label of a cylinder of the given index
func set_cylinder_label(index: int, position: float, speed: float):
	cylinder_labels[index].text = "%.2f cm\n%.2f cm/s" % [position, speed]
