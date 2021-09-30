using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerState : MonoBehaviour
{
    public TileState currentTile;
    public Vector3 origin;
    public Vector3 dest;
    public float progressTowardsDest;
    public string playerColor;

    public void MoveStateToTile(TileState targetTile)
    {
        Debug.Log("Moving worker state to " + targetTile.coord);
        if (currentTile != null)
        {
            currentTile.worker = null;
        }
        targetTile.worker = this;
        currentTile = targetTile;
        transform.parent = targetTile.transform;
    }

    public void MoveModelToTile(TileState targetTile)
    {
        transform.position = GetHighestPoint(targetTile.transform.position);
    }

    public void SmoothMoveModelToTile(TileState targetTile)
    {
        origin = transform.position;
        dest = GetHighestPoint(targetTile.transform.position);
        progressTowardsDest = 0;
    }

    public void MoveModelToParent()
    {
        MoveModelToTile(currentTile);
    }


    // Start is called before the first frame update
    void Start()
    {
        progressTowardsDest = 2;
        dest = Vector3.zero;
        origin = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (progressTowardsDest < 1)
        {
            transform.position = Vector3.Lerp(origin, dest, progressTowardsDest);
            progressTowardsDest += 0.01f;
        } 
        else if (progressTowardsDest >= 1)
        {
            progressTowardsDest = 2;
            origin = transform.position;
        }
    }

    public void SetColor(string color)
    {
        if (color == "red")
        {
            playerColor = "red";
            GetComponent<Renderer>().material.SetColor("_Color", Color.red);
        }
        else if (color == "blue")
        {
            playerColor = "blue";
            GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
        }
    }

    public Vector3 GetHighestPoint(Vector3 target)
    {
        Vector3 above = new Vector3(target.x, 20, target.z);
        RaycastHit hit;
        if (Physics.Raycast(above, Vector3.down, out hit, 20.0f))
        {
            float distToGround = hit.distance;
            float newY = (above.y - distToGround);
            return new Vector3(target.x, newY, target.z);
        }
        // No collision detected
        return new Vector3(target.x, 0, target.z);
    }
}
