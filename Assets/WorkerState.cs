using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerState : MonoBehaviour
{
    public TileState currentTile;
    public Vector3 origin;
    public Vector3 dest;
    public string playerColor;

    float[] posBeziers;
    float[] negBeziers;

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
        transform.position = GetHighestPoint(targetTile.transform.position) + new Vector3(0, transform.localScale.y * 0.5f, 0);
    }

    public void SmoothMoveModelToTile(TileState targetTile)
    {
        origin = transform.position;
        dest = GetHighestPoint(targetTile.transform.position) + new Vector3(0, transform.localScale.y * 0.5f, 0);
        StartCoroutine("SmoothMoveCoroutine");
    }

    public void MoveModelToParent()
    {
        SmoothMoveModelToTile(currentTile);
    }

    IEnumerator SmoothMoveCoroutine()
    {
        for (float f = 0; f < 1; f += 0.017f)
        {
            Vector2 horizontal = Vector2.Lerp(new Vector2(origin.x, origin.z), new Vector2(dest.x, dest.z), f);
            float distance = Vector2.Distance(new Vector2(origin.x, origin.z), new Vector2(dest.x, dest.z));
            float vertical = 1.7f * distance * f * (distance - distance * f) + origin.y + (dest.y - origin.y) * f;
            transform.position = new Vector3(horizontal.x, vertical, horizontal.y);
            yield return null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        dest = Vector3.zero;
        origin = Vector3.zero;
        posBeziers = new float[] { 0.0f, 1.5f, 1.5f, 1.0f };
        negBeziers = new float[] { 0.0f, -1.5f, -1.5f, 1.0f };
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
