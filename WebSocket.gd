extends Node

# The URL we will connect to
var websocket_url = "ws://localhost:2567"
@export
var controller: Node
@export
var ui_handler: Node

# Our WebSocketClient instance
var _client = WebSocketPeer.new()

func _ready():
	_client.connect_to_url(websocket_url)

func _process(delta):
	_client.poll()
	var state = _client.get_ready_state()
	if state == WebSocketPeer.STATE_OPEN:
		while _client.get_available_packet_count():
			ui_handler.set("has_control_over_pitch_roll", false)
			var data_string = _client.get_packet().get_string_from_utf8()
			var json = JSON.new()
			var prData = json.parse(data_string)
			if prData == OK:
				var data_received = json.data
				controller.set_pitch(float(data_received.pitch))
				controller.set_roll(float(data_received.roll))
			else:
				print("JSON Parse Error: ", json.get_error_message(), " in ", data_string, " at line ", json.get_error_line())
	elif state == WebSocketPeer.STATE_CONNECTING:
		pass
	elif state == WebSocketPeer.STATE_CLOSING:
		# Keep polling to achieve proper close.
		pass
	elif state == WebSocketPeer.STATE_CLOSED:
		ui_handler.set("has_control_over_pitch_roll", true)
		var code = _client.get_close_code()
		var reason = _client.get_close_reason()
		print("WebSocket closed with code: %d, reason %s. Clean: %s" % [code, reason, code != -1])
		set_process(false) # Stop processing.

