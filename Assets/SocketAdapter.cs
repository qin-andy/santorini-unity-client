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
        if (type.GetString() == "start success")
        {
            JsonElement players = payload.GetProperty("players");
            string redPlayerId = players.GetProperty("red").GetString();
            Debug.Log("Red player id: " + redPlayerId);
            string playerColor = client.Id == redPlayerId ? "red" : "blue";
            actionQueue.actionQueue.Enqueue(new CoordAction("game start", playerColor));
        }
        else if (type.GetString() == "placement update")
        {
            Vector2Int coord = CoordJsonToVector(payload.GetProperty("coord"));
            Vector2Int[] coords = new Vector2Int[1] { coord };
            Debug.Log("Dispatching action from thread: " + Thread.CurrentThread.Name);
            actionQueue.actionQueue.Enqueue(new CoordAction("place worker", coords));
        }
        else if (type.GetString() == "santorini move")
        {
            JsonElement moves = payload.GetProperty("move");
            JsonElement workerCoordElement = moves.GetProperty("workerCoord");
            JsonElement moveCoordElement = moves.GetProperty("moveCoord");
            JsonElement buildCoordElement = moves.GetProperty("buildCoord");

            Vector2Int workerCoord = CoordJsonToVector(workerCoordElement);
            Vector2Int moveCoord = CoordJsonToVector(moveCoordElement);
            Vector2Int buildCoord = CoordJsonToVector(buildCoordElement);
            actionQueue.actionQueue.Enqueue(new CoordAction("make move", new Vector2Int[]
            {
                workerCoord,
                moveCoord,
                buildCoord
            }));
        }
    }

    public void SendPlacement(Vector2Int coord)
    {
        string coordJson = VectorToCoordJson(coord);
        string payloadString = "{\"coord\": " + coordJson + "}";
        Debug.Log("Sending payload; " + payloadString);
        client.EmitAsync("game action", "santorini place", payloadString);
    }

    public void SendMove(Vector2Int[] coords) // worker, move, and build coords at 0, 1, 2
    {
        string workerCoord = VectorToCoordJson(coords[0]);
        string moveCoord = VectorToCoordJson(coords[1]);
        string buildCoord = VectorToCoordJson(coords[2]);
        string payloadString = "{\"workerCoord\": " + workerCoord + "," +
                                "\"moveCoord\": " + moveCoord + "," +
                                "\"buildCoord\": " + buildCoord + "}";
        client.EmitAsync("game action", "santorini move", payloadString);
    }
    
    public string VectorToCoordJson(Vector2Int coord)
    {
        string payloadString = "{\"x\": " + coord.x + ", \"y\": " + coord.y + "}";
        return payloadString;
    }

    public Vector2Int CoordJsonToVector(JsonElement coord)
    {
        int x = coord.GetProperty("x").GetInt32();
        int y = coord.GetProperty("y").GetInt32();
        return new Vector2Int(x, y);
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
