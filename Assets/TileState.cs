using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileState : MonoBehaviour
{
    public Vector2Int coord;
    public string myName;
    public int elevation;

    public bool highlightedForMove;
    public bool highlightedForBuild;
    public Color previousColor;
    public Color defaultColor;
    
    public TileBlockBuilder builder;
    public WorkerState worker;
    public BoardState board;

    private float blockHeight;
    void SelectTile()
    {
        if (board.gamePhase == "placement")
        {
            board.PlaceWorker(this);
            /*board.adapter.SendPlacement(coord);*/
        }
        else if (board.gamePhase == "build")
        {
            switch (board.selectionPhase)
            {
                case "Selecting Worker":
                    if (worker != null 
                        /*&& worker.GetComponent<WorkerState>().playerColor == board.playerColor
                        && board.playerColor == board.currentTurn*/
                        )
                    {
                        board.UnhighlightAll();
                        board.selectedWorkerTile = this;
                        board.selectionPhase = "Selecting Move Tile";
                        board.HighlightForMoveAdjacentTiles(this);
                    }
                    break;
                case "Selecting Move Tile":
                    if (highlightedForMove)
                    {
                        board.UnhighlightAll();
                        board.selectedMoveTile = this;
                        board.selectionPhase = "Selecting Build Tile";
                        board.HighlightForBuildAdjacentTiles(this);
                        board.selectedWorkerTile.worker.SmoothMoveModelToTile(this);
                    }
                    else
                    {
                        board.selectionPhase = "Selecting Worker";
                        board.UnhighlightAll();
                    }
                    break;
                case "Selecting Build Tile":
                    if (highlightedForBuild)
                    {
                        // Send move to adapter
                        /*                        Vector2Int[] coords = new Vector2Int[] {
                                                    board.selectedWorkerTile.coord,
                                                    board.selectedMoveTile.coord,
                                                    this.coord,
                                                };
                                                board.adapter.SendMove(coords);*/
                        //
                        board.MakeMove(board.selectedWorkerTile, board.selectedMoveTile, this);
                    }
                    else
                    {
                        board.selectionPhase = "Selecting Worker";
                        board.selectedWorkerTile.worker.MoveModelToParent();
                        board.UnhighlightAll();
                    }
                    break;
            }
        }
    }

    public void HighlightForMove()
    {
        GetComponent<Renderer>().material.SetColor("_Color", Color.white);
        highlightedForMove = true;
    }
    public void HighlightForBuild()
    {
        GetComponent<Renderer>().material.SetColor("_Color", Color.gray);
        highlightedForBuild = true;
    }

    public bool IsBuildable()
    {
        return worker == null && elevation < 4;
    }

    public void Unhighlight()
    {
        GetComponent<Renderer>().material.SetColor("_Color", defaultColor);
        highlightedForMove = false;
        highlightedForBuild = false;
    }

    void OnClicked(System.Object sender, EventArgs e)
    {
        SelectTile();
    }

    void OnHoverEnter(System.Object sender, EventArgs e)
    {
        previousColor = GetComponent<Renderer>().material.color;
        GetComponent<Renderer>().material.SetColor("_Color", Color.gray);
    }
    void OnHoverLeave(System.Object sender, EventArgs e)
    {
        GetComponent<Renderer>().material.SetColor("_Color", previousColor);
    }

    public void BuildBlock()
    {
        if (elevation < 3)
        {
            elevation += 1;
            builder.BuildBlock(elevation);

        }
        else if (elevation == 3)
        {
            elevation += 1;
            builder.BuildCap();
        }
    }

    void Start()
    {
        board = transform.parent.GetComponent<BoardState>();
        elevation = 0;
        builder = GetComponent<TileBlockBuilder>();
        GetComponent<EventManager>().OnPrimaryClicked += OnClicked;
        GetComponent<EventManager>().OnMouseHoverEnter += OnHoverEnter;
        GetComponent<EventManager>().OnMouseHoverLeave += OnHoverLeave;

        defaultColor = GetComponent<Renderer>().material.color;
        previousColor = defaultColor;
    }
}
