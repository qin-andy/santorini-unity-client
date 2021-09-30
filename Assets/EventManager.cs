using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public event EventHandler OnPrimaryClicked;
    public event EventHandler OnSecondaryClicked;
    public event EventHandler OnMouseHoverEnter;
    public event EventHandler OnMouseHoverLeave;

    public void FirePrimaryClickEvent()
    {
        OnPrimaryClicked?.Invoke(this, EventArgs.Empty);
    }

    public void FireSecondaryClickEvent()
    {
        OnSecondaryClicked?.Invoke(this, EventArgs.Empty);
    }

    public void FireMouseHoverEnter()
    {
        OnMouseHoverEnter?.Invoke(this, EventArgs.Empty);
    }

    public void FireMouseHoverLeave()
    {
        OnMouseHoverLeave?.Invoke(this, EventArgs.Empty);
    }
}
