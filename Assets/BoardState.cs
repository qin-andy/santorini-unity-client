using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardState : MonoBehaviour
{
    // Passed in through inspector
    public GameObject tile;
    public GameObject worker;
    public GameObject socketAdapaterObject;

    public SocketAdapter adapter;
    public PlayerState player;

    public TileState selectedWorkerTile;
    public TileState selectedMoveTile;
    public TileState selectedBuildTile;

    public List<WorkerState> workers;
    public List<TileState> tiles;

    public bool running;

    public string gamePhase;
    public string selectionPhase;
    public string currentTurn;


    // Start is called before the first frame update
    void Start()
    {
        List<Vector2Int> coords = new List<Vector2Int>();
        for (int i = 0; i < 25; i++)
        {
            coords.Add(new Vector2Int(i % 5, i / 5));
        }
        foreach (Vector2Int coord in coords)
        {
            Vector3 position = new Vector3(coord.x - 2, 0, coord.y - 2);
            GameObject newTile = Instantiate(tile, position, Quaternion.identity);
            newTile.GetComponent<TileState>().coord = coord;
            newTile.transform.parent = gameObject.transform;
            tiles.Add(newTile.GetComponent<TileState>());
        }
        selectionPhase = "Selecting Worker";
        gamePhase = "placement";
        currentTurn = "red";
        adapter = socketAdapaterObject.GetComponent<SocketAdapter>();
    }

    // Update is called once per frame
    void Update()
    {
        PrimaryClick();
        SecondaryClick();
    }

    public List<Vector2Int> GetAdjacentCoords(Vector2Int tilePosition)
    {
        List<Vector2Int> coords = new List<Vector2Int>();
        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                Vector2Int potential = tilePosition + new Vector2Int(x, y);
                if (!(potential.x < 0 || potential.x > 4 || potential.y < 0 || potential.y > 4 ))
                {
                    coords.Add(potential);
                }
            }
        }
        Debug.Log(tilePosition + ": " + coords.ToString());
        return coords;
    }   

    public List<TileState> GetAdjacentTiles(TileState targetTile)
    {
        Vector2Int coord = targetTile.coord;
        List<Vector2Int> coords = GetAdjacentCoords(coord);
        List<TileState> adjTiles = new List<TileState>();
        foreach (Vector2Int adjCoord in coords)
        {
            adjTiles.Add(tiles[adjCoord.y * 5 + adjCoord.x]);
        }
        return adjTiles;
    }

    public void HighlightForMoveAdjacentTiles(TileState targetTile)
    {
        List<TileState> adjTiles = GetAdjacentTiles(targetTile);
        foreach (TileState adjTile in adjTiles)
        {
            int heightDiff = adjTile.elevation - selectedWorkerTile.elevation;
            if (adjTile.GetComponent<TileState>().IsBuildable() && heightDiff <= 1)
            {
                adjTile.GetComponent<TileState>().HighlightForMove();
            }
        }
    }

    public void HighlightForBuildAdjacentTiles(TileState targetTile)
    {
        List<TileState> adjTiles = GetAdjacentTiles(targetTile);
        foreach (TileState adjTile in adjTiles)
        {
            if (adjTile.IsBuildable() && adjTile != selectedMoveTile)
            {
                adjTile.HighlightForBuild();
            } 
        }
        selectedWorkerTile.HighlightForBuild();
    }

    public void UnhighlightAll()
    {
        foreach (TileState boardTile in tiles)
        {
            boardTile.Unhighlight();
        }
    }

    public void CreateWorkerOnTile(TileState targetTile, string color)
    {
        GameObject newWorker = Instantiate(worker, Vector3.zero, Quaternion.identity);

        WorkerState newWorkerState = newWorker.GetComponent<WorkerState>();
        newWorkerState.SetColor(color);
        newWorkerState.MoveModelToTile(targetTile);
        newWorkerState.MoveStateToTile(targetTile);
        Debug.Log(newWorker);
        workers.Add(newWorkerState);
    }

    public void PlaceWorker(TileState targetTile)
    {
        switch (workers.Count)
        {
            case 0:
                Debug.Log("Placing with " + workers.Count);
                CreateWorkerOnTile(targetTile, "red");
                currentTurn = "blue";
                break;
            case 1:
                CreateWorkerOnTile(targetTile, "blue");
                break;
            case 2:
                CreateWorkerOnTile(targetTile, "blue");
                currentTurn = "red";
                break;
            case 3:
                CreateWorkerOnTile(targetTile, "red");
                gamePhase = "build";
                currentTurn = "red";
                break;
        }
    }

    public void SwapTurns()
    {
        currentTurn = currentTurn == "red" ? "blue" : "red";
    }

    public void MakeMove(TileState workerTile, TileState moveTile, TileState buildTile)
    {
        workerTile.worker.MoveStateToTile(moveTile);
        buildTile.BuildBlock();
        UnhighlightAll();
        SwapTurns();
        selectionPhase = "Selecting Worker";
    }

    public TileState GetTileAtCoord(Vector2Int coord)
    {
        return tiles[coord.x + coord.y * 5];
    }

    void PrimaryClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject clickedObject = GetMouseGameObject();
            clickedObject?.GetComponent<EventManager>()?.FirePrimaryClickEvent();
        }
    }

    void SecondaryClick()
    {
        if (Input.GetMouseButtonDown(1))
        {
            GameObject clickedObject = GetMouseGameObject();
            clickedObject?.GetComponent<EventManager>()?.FireSecondaryClickEvent();
        }
    }

    GameObject GetMouseGameObject()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 30.0f))
        {
            return hit.collider.gameObject;
        }
        return null;
    }
}
