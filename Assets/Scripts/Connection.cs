using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NativeWebSocket;

[System.Serializable]
public class FloatData
{
  public float pitch;
  public float roll;
}

public class Connection : MonoBehaviour
{
  [SerializeField]
  private Controller controller;
  [SerializeField]
  private UIHandler uiHandler;

  WebSocket websocket;

  // Start is called before the first frame update
  async void Start()
  {
    websocket = new WebSocket("ws://localhost:2567");

    websocket.OnOpen += () =>
    {
      Debug.Log("Connection open!");
      uiHandler.hasControlOverPitchRoll = false;
    };

    websocket.OnError += (e) =>
    {
      Debug.Log("Error! " + e);
    };

    websocket.OnClose += (e) =>
    {
      Debug.Log("Connection closed!");
      uiHandler.hasControlOverPitchRoll = true;
    };

    websocket.OnMessage += (bytes) =>
    {
      string jsonString = System.Text.Encoding.UTF8.GetString(bytes);
      FloatData data = JsonUtility.FromJson<FloatData>(jsonString);
      controller.SetPitch(data.pitch);
      controller.SetRoll(data.roll);
    };

    // waiting for messages
    await websocket.Connect();
  }

  void Update()
  {
    #if !UNITY_WEBGL || UNITY_EDITOR
      websocket.DispatchMessageQueue();
    #endif
  }

  async void SendWebSocketMessage()
  {
    if (websocket.State == WebSocketState.Open)
    {
    }
  }

  private async void OnApplicationQuit()
  {
    await websocket.Close();
  }

}