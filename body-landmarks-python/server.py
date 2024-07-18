import cv2
import mediapipe as mp
import numpy as np
import asyncio
import websockets
import json

# Inicializar MediaPipe Pose
mp_pose = mp.solutions.pose
pose = mp_pose.Pose(static_image_mode=False, model_complexity=2, enable_segmentation=False, min_detection_confidence=0.5)
mp_drawing = mp.solutions.drawing_utils

# Función para dibujar la pose en 3D
def draw_pose_landmarks(frame, landmarks, connections):
    for landmark in landmarks:
        x = int(landmark.x * frame.shape[1])
        y = int(landmark.y * frame.shape[0])
        cv2.circle(frame, (x, y), 5, (0, 255, 0), -1)

    for connection in connections:
        start = landmarks[connection[0]]
        end = landmarks[connection[1]]
        start_point = (int(start.x * frame.shape[1]), int(start.y * frame.shape[0]))
        end_point = (int(end.x * frame.shape[1]), int(end.y * frame.shape[0]))
        cv2.line(frame, start_point, end_point, (0, 255, 0), 2)

# Capturar video desde la cámara
usuario = "pepe"
contrasena = "pepe123"
# ip = "192.168.56.249"
ip = "10.7.126.209"
url = f"http://{usuario}:{contrasena}@{ip}:8080/video"

cap = cv2.VideoCapture(url)
# cap = cv2.VideoCapture(0)   

connected_clients = set()

async def send_pose_data():
    while True:
        if cap.isOpened():
            ret, frame = cap.read()
            if not ret:
                continue

            frame = cv2.resize(frame, (640, 480))
            rgb_frame = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
            result = pose.process(rgb_frame)

            pose_data = {}
            # if result is None:
            #     pose_data = {
            #         'landmarks': [],
            #         'connections': list()
            #     }

            print(result.pose_landmarks)
            if result.pose_landmarks:
                pose_data = {
                    'landmarks': [{'x': lm.x, 'y': lm.y, 'z': lm.z} for lm in result.pose_landmarks.landmark],
                    'connections': list(mp_pose.POSE_CONNECTIONS)
                }
                draw_pose_landmarks(frame, result.pose_landmarks.landmark, mp_pose.POSE_CONNECTIONS)

            cv2.imshow('Pose Detection', frame)

            if connected_clients:
                message = json.dumps(pose_data)
                if result.pose_landmarks:
                    await asyncio.gather(*[client.send(message) for client in connected_clients])

            if cv2.waitKey(1) & 0xFF == ord('q'):
                break

        await asyncio.sleep(0.03)

async def handle_client(websocket, path):
    connected_clients.add(websocket)
    try:
        await websocket.wait_closed()
    finally:
        connected_clients.remove(websocket)

async def main():
    start_server = websockets.serve(handle_client, "0.0.0.0", 6789)
    await asyncio.gather(start_server, send_pose_data())

print("Iniciando servidor")
asyncio.run(main())

cap.release()
cv2.destroyAllWindows()