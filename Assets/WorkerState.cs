using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerState : MonoBehaviour
{
    public TileState currentTile;
    public string playerColor;

    public void MoveStateToTile(TileState targetTile)
    {
        Debug.Log("Moving worker to " + targetTile);
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

    public void MoveModelToParent()
    {
        MoveModelToTile(currentTile);
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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
