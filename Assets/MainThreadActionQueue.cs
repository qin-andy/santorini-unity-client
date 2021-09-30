using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainThreadActionQueue : MonoBehaviour
{
    public class CoordAction
    {
        public string type { get; }
        public string extra { get; }
        public Vector2Int[] coords { get; }

        public CoordAction(string type, Vector2Int[] coords)
        {
            this.type = type;
            this.coords = coords;
        }

        public CoordAction(string type, string extra)
        {
            this.type = type;
            this.coords = new Vector2Int[0];
            this.extra = extra;
        }

    }

    public Queue<CoordAction> actionQueue;
    // Start is called before the first frame update
    void Start()
    {
        actionQueue = new Queue<CoordAction>();
    }
}
