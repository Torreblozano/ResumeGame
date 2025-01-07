using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    // Public variables 
    [SerializeField] PlayerInput player, UI; // Input configurations for Player and UI
    [SerializeField] float selectionCooldown = 0.2f; // Minimum time interval between actions

    // Private variables
    private InputContext currentContext;
    private float lastSelectionTime; // Tracks the last time an input action was performed

    #region Delegates

    // Delegates for input button events
    public delegate void OnUpButton();
    public delegate void OnDownButton();
    public delegate void OnLeftButton();
    public delegate void OnRightButton();
    public delegate void OnAcceptButton();
    public delegate void OnCancelButton();

    // Public event handlers for input buttons
    public OnUpButton onUpButton;
    public OnDownButton onDownButton;
    public OnLeftButton onLeftButton;
    public OnRightButton onRightButton;
    public OnAcceptButton onAcceptButton;
    public OnCancelButton onCancelButton;

    #endregion

    #region Input states

    /// <summary>
    /// Represents different input contexts the game can be in.
    /// </summary>
    public enum InputContext
    {
        PLAYER,  // Player movement and interaction
        UI,      // User interface navigation
        DISABLE  // Input completely disabled
    }

    /// <summary>
    /// Sets the current input context and enables/disables relevant input configurations.
    /// </summary>
    /// <param name="context">The desired input context.</param>
    public void SetContext(InputContext context)
    {
        currentContext = context;

        switch (currentContext)
        {
            case InputContext.PLAYER:
                player.enabled = true;
                UI.enabled = false;
                break;
            case InputContext.UI:
                player.enabled = false;
                UI.enabled = true;
                break;
            case InputContext.DISABLE:
                ClearAllDelegates();
                player.enabled = false;
                UI.enabled = false;
                break;
        }
    }

    // Returns the current input context.
    public InputContext GetCurrentContext() => currentContext;

    #endregion

    #region Player inputs

    // Ensures the action can only be performed if the cooldown period has passed.
    private bool CanExecuteAction()
    {
        if (Time.time - lastSelectionTime < selectionCooldown) return false;

        lastSelectionTime = Time.time;
        return true;
    }

    public void UpButton(InputAction.CallbackContext context)
    {
        if (!context.performed || !CanExecuteAction()) return;
        onUpButton?.Invoke();
    }

    public void DownButton(InputAction.CallbackContext context)
    {
        if (!context.performed || !CanExecuteAction()) return;
        onDownButton?.Invoke();
    }

    public void LeftButton(InputAction.CallbackContext context)
    {
        if (!context.performed || !CanExecuteAction()) return;
        onLeftButton?.Invoke();
    }

    public void RightButton(InputAction.CallbackContext context)
    {
        if (!context.performed || !CanExecuteAction()) return;
        onRightButton?.Invoke();
    }

    public void AcceptButton(InputAction.CallbackContext context)
    {
        if (!context.performed || !CanExecuteAction()) return;
        onAcceptButton?.Invoke();
    }

    public void CancelButton(InputAction.CallbackContext context)
    {
        if (!context.performed || !CanExecuteAction()) return;

        onCancelButton?.Invoke();
    }

    public void ClearAllDelegates()
    {
        onUpButton = null;
        onDownButton = null;
        onLeftButton = null;
        onRightButton = null;
        onAcceptButton = null;
        onCancelButton = null;
    }

    #endregion
}
