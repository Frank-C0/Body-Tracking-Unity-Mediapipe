using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Text;
using UnityEngine;

public class WebSocketClient : MonoBehaviour
{
    public bool isActivated = false;
    private ClientWebSocket ws;
    private CancellationTokenSource cts;
    // public GameObject pointPrefab;  // Prefab de la esfera para los puntos
    public GameObject[] points;  // Array de puntos seteado desde el inspector

    async void Start()
    {
        ws = new ClientWebSocket();
        cts = new CancellationTokenSource();
        //await ws.ConnectAsync(new Uri("ws://10.7.46.183:6789"), cts.Token);
        await ws.ConnectAsync(new Uri("ws://localhost:6789"), cts.Token);

        await ReceiveMessages();
    }

    async Task ReceiveMessages()
    {
        var buffer = new byte[1024 * 4];
        while (ws.State == WebSocketState.Open)
        {
            var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), cts.Token);
            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            Debug.Log("Mensaje recibido del servidor: " + message);

            // Parse the JSON message
            ServerMessage data = JsonUtility.FromJson<ServerMessage>(message);
            Debug.Log(data.landmarks);
            Debug.Log(data.landmarks.Length);
            if (data != null)
            {
                RenderLandmarks(data.landmarks);
            }
        }
    }

    void RenderLandmarks(Landmark[] landmarks)
    {
        if (!isActivated) { return; }
        // Asegúrate de que el array de puntos tiene la misma longitud que los landmarks
        if (points.Length != landmarks.Length)
        {
            Debug.LogError("La cantidad de puntos en el array no coincide con la cantidad de landmarks recibidos.");
            return;
        }

        // Actualiza las posiciones de los puntos
        for (int i = 0; i < landmarks.Length; i++)
        {
            Vector3 position = new Vector3(landmarks[i].x, -landmarks[i].y, landmarks[i].z);
            points[i].transform.position = position;
        }
    }

    async void OnDestroy()
    {
        cts.Cancel();
        try
        {
            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Cerrando conexión", CancellationToken.None);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error al cerrar la conexión: {ex.Message}");
        }
    }

    [Serializable]
    public class Landmark
    {
        public float x;
        public float y;
        public float z;
    }

    [Serializable]
    public class ServerMessage
    {
        public Landmark[] landmarks;
    }
}