extends Node

# Public variables
@export
var c_factor: float
@export
var minimum_position: float
@export
var cylinder_override: bool

# Serialized fields
@export
var platform_size: Vector2
@export
var ui_handler: Node
var cylinders: Array[MeshInstance3D]
@export
var platform: MeshInstance3D
@export
var inverse_platform_mesh_instance: MeshInstance3D
@export
var pitch: float = 0
@export
var roll: float = 0
@export
var cylinder_base_y: float
@export
var camera: Camera3D

# Private variables
var platform_mesh_instance: MeshInstance3D
var platform_pitch: float
var platform_roll: float
var x_center: float
var y_center: float
var normal: Vector3
var cylinder_y_position_history: Array = []
var last_speed: Array[float] = [0,0,0,0]

enum VectorAxis { X, Y, Z }

func _ready():
	# Get the mesh instance of the platform
	platform_mesh_instance = platform

	# Calculate the center of the platform
	x_center = platform_size.x / 2
	y_center = platform_size.y / 2
	
	cylinders.append(get_node("Cylinder +,+"))
	cylinders.append(get_node("Cylinder +,-"))
	cylinders.append(get_node("Cylinder -,+"))
	cylinders.append(get_node("Cylinder -,-"))

	# Initialize position history
	cylinder_y_position_history = [0, 0, 0, 0]

func _process(delta):
	# Handle the platform
	handle_platform()
	
	if ui_handler.get("has_control_over_pitch_roll"):
		set_pitch(ui_handler.get("pitch_slider").value)
		set_roll(ui_handler.get("roll_slider").value)

	# Only handle the cylinders if it is not being overridden
	if not cylinder_override:
		handle_cylinders()

func _physics_process(delta):
	# Update the UI
	update_ui()

func set_pitch(value: float):
	pitch = value

func set_roll(value: float):
	roll = value

func handle_platform():
	# Set the position of the platform
	platform.translate(get_average_position())

	# Set the mesh of the platform
	var vertices = get_vertices()
	platform.mesh = calculate_mesh(vertices)
	inverse_platform_mesh_instance.mesh = platform.mesh

	# Calculate the angles of the platform
	calculate_angles(vertices)

func handle_cylinders():
	# Iterate through each cylinder
	for i in range(4):
		if (cylinders[i] == null):
			continue
		# Store the current position of the cylinder for history
		cylinder_y_position_history[i] = cylinders[i].position.y

		# Set the position of the cylinder with the offset
		cylinders[i].position = Vector3(
			 x_center if (i % 2 == 0) else -x_center, 
			calculate_y_position(i), 
			y_center if (i < 2) else -y_center
		)

func calculate_y_position(index: int) -> float:
	var pitch_rad = deg_to_rad(pitch)
	var roll_rad = deg_to_rad(roll)

	var pitch_sin = sin(pitch_rad) * c_factor
	var pitch_cos = cos(pitch_rad) * c_factor

	var roll_sin = sin(roll_rad) * c_factor
	var roll_cos = cos(roll_rad) * c_factor

	return pitch_cos * cylinder_base_y + (-1 if (index < 2) else 1) * pitch_sin * y_center - roll_cos * cylinder_base_y + (-1 if (index % 2 == 0) else 1) * roll_sin * x_center

func update_ui():
	ui_handler.update_pitch(platform_pitch)
	ui_handler.update_roll(platform_roll)

	# Iterate through each cylinder
	for i in range(cylinders.size()):
		var cylinder = cylinders[i]
		var speed = (cylinder.position.y - cylinder_y_position_history[i]) / 1
		if speed == 0 and cylinder_y_position_history[i] != 0:
			speed = last_speed[i]
			
		ui_handler.set_cylinder_panel_position(i, camera.unproject_position(cylinder.position))
		ui_handler.set_cylinder_label(i, cylinder.position.y + minimum_position, speed)
		last_speed[i] = speed

func get_average_position() -> Vector3:
	# Initialize the average position
	var average_position = Vector3()
	
	if (cylinders == null || average_position == null):
		return Vector3.ZERO
		
	# Iterate through each cylinder
	for cylinder in cylinders:
		if (cylinder == null):
			continue
		average_position += cylinder.position

	# Calculate the average position
	average_position /= cylinders.size()

	# Return the average position
	return average_position

func calculate_mesh(vertices: Array) -> Mesh:
	var mesh = Mesh.new()  # Create a new mesh
	var surface_tool = SurfaceTool.new()  # Create a surface tool

	# Define the primitive type (triangle fan is suitable for 4 points)
	surface_tool.begin(Mesh.PRIMITIVE_TRIANGLES)

	# Add each point as a vertex
	for vertex in vertices:
		surface_tool.add_vertex(vertex)
		
	surface_tool.generate_normals()

	# Finalize the mesh creation
	return surface_tool.commit()

func calculate_angles(vertices: Array):
	for vertex in vertices:
		if (vertex == null):
			return
	if (vertices.size() < 6):
		return
	# Calculate the normals of the platform
	var normal1 = (vertices[1] - vertices[0]).cross(vertices[2] - vertices[0])
	var normal2 = (vertices[3] - vertices[0]).cross(vertices[5] - vertices[0])

	# Combine the normals and normalize
	normal = (normal1 + normal2).normalized()

	# Calculate the platform pitch and roll
	platform_pitch = rad_to_deg(atan2(normal.z, normal.y))
	platform_roll = rad_to_deg(atan2(normal.x, normal.y))

func get_vertices() -> Array:
	for cylinder in cylinders:
		if (cylinder == null):
			return []
	return [cylinders[1].position, cylinders[0].position, cylinders[2].position, cylinders[1].position, cylinders[2].position, cylinders[3].position]
