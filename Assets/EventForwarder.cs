using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventForwarder : MonoBehaviour
{

    void ForwardPrimaryToParent(System.Object sender, EventArgs e)
    {
        transform.parent.GetComponent<EventManager>().FirePrimaryClickEvent();
    }
    void ForwardSecondaryToParent(System.Object sender, EventArgs e)
    {
        transform.parent.GetComponent<EventManager>().FireSecondaryClickEvent();
    }
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<EventManager>().OnPrimaryClicked += ForwardPrimaryToParent;
        GetComponent<EventManager>().OnSecondaryClicked += ForwardSecondaryToParent;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
