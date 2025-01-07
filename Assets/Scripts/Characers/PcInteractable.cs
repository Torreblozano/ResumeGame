using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PcInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] Dialog dialogs;

    public void Interact()
    {
        AudioManager.Instance.PlaySFX("PC");
        DialogManager.Instance.ShowDialog(dialogs.LocalizationKeys[0], () =>
        {
            ShowOptions();
        });
    }

    private void ShowOptions()
    {
        DialogManager.Instance.ShowOptions(AccessToPC, ClosePC);
    }

    private void AccessToPC()
    {
        DialogManager.Instance.ShowDialog(dialogs.LocalizationKeys[1], () =>
        {
            GameManager.Instance.TurnOnPC();
        });
    }

    private void ClosePC()
    {
        DialogManager.Instance.ShowDialog(dialogs.LocalizationKeys[2], () =>
        {
            GameManager.Instance.OnGame();
        });
    }
}
