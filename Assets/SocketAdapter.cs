using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using SocketIOClient;
using UnityEngine;

public class SocketAdapter : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject boardObject;

    SocketIO client;
    BoardState board;

    async void ConnectToServer()
    {
        Debug.Log("Socket Adapater starting...");
        client = new SocketIO("http://localhost:3001/");
        client.On("manager response", response =>
        {
            Debug.Log(response.GetValue(0));
        });
        client.On("game update", response =>
        {
            ParseGameUpdateJson(response.GetValue(0));
        });
        await client.ConnectAsync();
        await client.EmitAsync("manager action", "join queue");
    }

    void ParseManagerResponseJson(string jsonString)
    {
        using (JsonDocument document = JsonDocument.Parse(jsonString))
        {
            JsonElement root = document.RootElement;
            JsonElement error = root.GetProperty("error");
            Debug.Log("root: " + root);
        }
    }

    void ParseGameUpdateJson(JsonElement root)
    {
        JsonElement error = root.GetProperty("error");

        Debug.Log("Attempting to parse: " + root);

        if (error.GetBoolean())
        {
            return;
        }
        JsonElement type = root.GetProperty("type");
        JsonElement payload = root.GetProperty("payload");
        if (type.GetString() == "placement update")
        {
            JsonElement coord = payload.GetProperty("coord");
            int x = coord.GetProperty("x").GetInt32();
            int y = coord.GetProperty("y").GetInt32();
            Debug.Log("received placement update at " + x + "," + y);
            board.PlaceWorker(board.tiles[x + y * 5]);
        }
    }

    public async void SendPlacement(Vector2Int coord)
    {
        string payloadString = "{\"coord\": {\"x\": " + coord.x + ", \"y\": " + coord.y + "}}";

        Debug.Log("Sending payload; " + payloadString);
        await client.EmitAsync("game action", "santorini place", payloadString);
    }

    void Start()
    {
        board = boardObject.GetComponent<BoardState>();
        ConnectToServer();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
