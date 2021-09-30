using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainThreadActionQueue : MonoBehaviour
{
    public class CoordAction
    {
        public string type { get; }
        public Vector2Int[] coords { get; }

        public CoordAction(string type, Vector2Int[] coords)
        {
            this.type = type;
            this.coords = coords;
        }

    }

    public Queue<CoordAction> actionQueue;
    // Start is called before the first frame update
    void Start()
    {
        actionQueue = new Queue<CoordAction>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
