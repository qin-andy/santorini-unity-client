using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using SocketIOClient;
using UnityEngine;
using static MainThreadActionQueue;

public class SocketAdapter : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject boardObject;
    public GameObject queueObject;

    SocketIO client;
    BoardState board;
    MainThreadActionQueue actionQueue;

    void ConnectToServer()
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
        AsyncConnect();
    }

    async void AsyncConnect()
    {
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
            /*board.PlaceWorker(board.tiles[x + y * 5]);*/
            Vector2Int[] coords = new Vector2Int[1];
            coords[0] = new Vector2Int(x, y);
            Debug.Log("Dispatching action from thread: " + Thread.CurrentThread.Name);
            actionQueue.actionQueue.Enqueue(new CoordAction("place worker", coords));
        }
    }

    public void SendPlacement(Vector2Int coord)
    {
        string payloadString = "{\"coord\": {\"x\": " + coord.x + ", \"y\": " + coord.y + "}}";

        Debug.Log("Sending payload; " + payloadString);
        client.EmitAsync("game action", "santorini place", payloadString);
    }

    void Start()
    {
        board = boardObject.GetComponent<BoardState>();
        actionQueue = queueObject.GetComponent<MainThreadActionQueue>();
        ConnectToServer();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
