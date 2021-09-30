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
    void ForwardHoverEnter(System.Object sender, EventArgs e)
    {
        transform.parent.GetComponent<EventManager>().FireMouseHoverEnter();
    }
    void ForwardHoverLeave(System.Object sender, EventArgs e)
    {
        transform.parent.GetComponent<EventManager>().FireMouseHoverLeave();
    }
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<EventManager>().OnPrimaryClicked += ForwardPrimaryToParent;
        GetComponent<EventManager>().OnSecondaryClicked += ForwardSecondaryToParent;
        GetComponent<EventManager>().OnMouseHoverEnter += ForwardHoverEnter;
        GetComponent<EventManager>().OnMouseHoverLeave += ForwardHoverLeave;
    }
}
