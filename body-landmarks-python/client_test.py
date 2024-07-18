import asyncio
import websockets
import json

async def receive_pose_data():
    uri = "ws://localhost:6789"
    async with websockets.connect(uri) as websocket:
        while True:
            message = await websocket.recv()
            data = json.loads(message)
            print(f"Pose Data: {data}")

# Corrección para el bucle de eventos en versiones recientes de asyncio
asyncio.run(receive_pose_data())
