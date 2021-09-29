using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject boardGameObject;
    public GameObject board;
    // Start is called before the first frame update
    void Start()
    {
        board = Instantiate(boardGameObject, new Vector3(0, 0, 0), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
