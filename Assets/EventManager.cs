using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public event EventHandler OnPrimaryClicked;
    public event EventHandler OnSecondaryClicked;

    public void FirePrimaryClickEvent()
    {
        OnPrimaryClicked?.Invoke(this, EventArgs.Empty);
    }

    public void FireSecondaryClickEvent()
    {
        OnSecondaryClicked?.Invoke(this, EventArgs.Empty);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
