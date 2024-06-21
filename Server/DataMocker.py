import asyncio
import websockets
import json

async def send_pitch_roll(uri, pitch, roll):
    async with websockets.connect(uri) as websocket:
        data = {
            "pitch": pitch,
            "roll": roll
        }
        json_data = json.dumps(data)
        await websocket.send(json_data)
        print(f"Sent: {json_data}")

# Replace with your WebSocket server URI
websocket_uri = "ws://localhost:2567"

# Replace with your actual pitch and roll values
pitch_value = 1.23
roll_value = 4.56

# Running the asynchronous function
asyncio.get_event_loop().run_until_complete(send_pitch_roll(websocket_uri, pitch_value, roll_value))
