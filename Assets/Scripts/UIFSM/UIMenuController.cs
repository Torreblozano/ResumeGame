using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class UIMenuController : MonoBehaviour
{
    public List<TMP_Text> options = new List<TMP_Text>();

    [SerializeField] private Color highLightColor;
    private int currentAction = 0;

    // Define UnityEvents
    public UnityEvent option1;
    public UnityEvent option2;

    private void Start()
    {
        currentAction = 0;
        UpdateActionSelection(0);
    }

    public void AcceptSelection(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        AudioManager.Instance.PlaySFX("AcceptButton");
        // Invoke the appropriate UnityEvent based on the current selection
        if (currentAction == 0)
        {
            option1?.Invoke(); // Trigger the restart event
        }
        else
        {
            option2?.Invoke(); // Trigger the main menu event
        }

    }

    public void UpdateActionSelection(int selectedAction)
    {
        currentAction = selectedAction;
        for (int i = 0; i < options.Count; i++)
        {
            options[i].color = i == selectedAction ? highLightColor : Color.white;
        }
    }

    private void HandleSelection(int step)
    {
        int nextStep = currentAction + step;

        if (nextStep > options.Count - 1)
            nextStep = options.Count - 1;

        if (nextStep < 0)
            nextStep = 0;

        UpdateActionSelection(nextStep);
    }

    public void LeftButton(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        HandleSelection(-1);
        AudioManager.Instance.PlaySFX("HandleButton");
    }

    public void RightButton(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        HandleSelection(1);
        AudioManager.Instance.PlaySFX("HandleButton");
    }
}
