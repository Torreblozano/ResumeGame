using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public delegate void OnAction();
    public event OnAction onAction;

    public delegate void OnCancel();
    public event OnCancel onCancel;

    /// <summary>
    /// Call this to do an action in UI (C Button)
    /// </summary>
    public void OnUIAction()=> onAction.Invoke();


    /// <summary>
    /// Call this to cancel an action in UI (X Button)
    /// </summary>
    public void OnUICancel()=> onCancel.Invoke();
}
