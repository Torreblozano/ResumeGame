using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteractable : MonoBehaviour, IInteractable
{
    private Dialog dialogs;

    private void Awake()=>dialogs = GetComponent<Dialog>();
    
    public void Interact()
    {
        DialogManager.Instance.ShowDialog(dialogs.LocalizationKeys[0], () =>
        {
            GameManager.Instance.OnGame();
        });
    }
}
