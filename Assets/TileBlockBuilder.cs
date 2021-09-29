using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBlockBuilder : MonoBehaviour
{
    public GameObject block;
    public GameObject cap;
    public TileState tileState;
    public float blockHeight;
    // Start is called before the first frame update
    void Start()
    {
        blockHeight = block.transform.localScale.y;
        tileState = GetComponent<TileState>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BuildBlock()
    {
        Debug.Log(tileState.coord);
        Vector3 destination = GetHighestPoint(transform.position);
        destination += new Vector3(0, block.transform.localScale.y * 0.5f, 0);
        GameObject newBlock = Instantiate(block, destination, gameObject.transform.rotation);
        newBlock.transform.parent = gameObject.transform;
    }

    public void BuildCap()
    {
        Vector3 destination = GetHighestPoint(transform.position);
        GameObject newBlock = Instantiate(cap, destination, gameObject.transform.rotation);
        newBlock.transform.parent = gameObject.transform;
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
