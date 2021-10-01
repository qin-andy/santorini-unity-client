using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBlockBuilder : MonoBehaviour
{
    public GameObject block;
    public GameObject cap;
    public TileState tileState;
    // Start is called before the first frame update
    void Start()
    {
        tileState = GetComponent<TileState>();
    }

    public void BuildBlock(int level)
    {
        GameObject newBlock = Instantiate(block, Vector3.zero, gameObject.transform.rotation);
        newBlock.transform.localScale = newBlock.transform.localScale * (float)Math.Pow(0.9, level);
        newBlock.transform.parent = gameObject.transform;
        StartCoroutine("SmoothBuildCoroutine", newBlock);
    }

    IEnumerator SmoothBuildCoroutine(GameObject block)
    {
        Vector3 destination = GetHighestPoint(transform.position);

        destination += new Vector3(0, block.transform.localScale.y * 0.5f, 0);
        Vector3 origin = destination + new Vector3(0, 1, 0);
        block.transform.position = origin;

        Renderer blockRenderer = block.GetComponent<Renderer>();
        Color c = blockRenderer.material.color;
        c.a = 0;
        blockRenderer.material.color = c;


        for (float f = 0; f < 1; f += 0.02f)
        {
            block.transform.position = Vector3.Lerp(origin, destination, f);
            Color c2 = blockRenderer.material.color;
            c2.a = f;
            blockRenderer.material.color = c2;
            yield return null;
        }
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
