using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the user interface (UI) system for a PC-like interface in the game.
/// It handles state transitions, action selections, and displays summaries.
/// </summary>
public class PCSystem : MonoBehaviour
{
    // Private variables
    [SerializeField] private List<TMP_Text> actionTexts;
    [SerializeField] private Color highLightColor;
    private int currentAction;
    private Dictionary<string, MenuStateBase> statesDict = new Dictionary<string, MenuStateBase>();// Dictionary for storing states
    private string currentState;
    private Stack<string> backwardStack = new Stack<string>(); // Stack for navigation history (going back)

    // Public variables
    public List<MenuStateBase> statesList = new List<MenuStateBase>(); // List of available menu states
    public Transform summaryPanel;
    public TMP_Text summaryText;

    // Delegates
    public delegate void OnClose();// Delegate for close event
    public event OnClose onClose;

    // Properties to access current and last screens
    public MenuStateBase CurrentScreen => !string.IsNullOrEmpty(currentState) && StatesDict.ContainsKey(currentState) ? StatesDict[currentState] : null;
    public MenuStateBase LastScreen => backwardStack.Count > 0 && StatesDict.ContainsKey(LastState) ? StatesDict[LastState] : null;
    public string LastState => backwardStack.Count > 0 ? backwardStack.Peek() : string.Empty;
    public int CurrentAction => currentAction;

    // Property to initialize the states dictionary when accessed
    private Dictionary<string, MenuStateBase> StatesDict
    {
        get
        {
            if (statesDict.Count == 0)
            {
                foreach (MenuStateBase state in statesList)
                {
                    if (!statesDict.ContainsKey(state.name))
                        statesDict.Add(state.name, state);
                }
            }
            return statesDict;
        }
    }

    #region States controll

    /// <summary>
    /// Activates the PCSystem and sets the initial state.
    /// </summary>
    public void OnEnter()
    {
        this.gameObject.SetActive(true);
        currentAction = 0;
        UpdateActionSelection(0); // Reset action selection
        ChangeState("MainMenu"); // Change to the default initial state
        ShowSummary(string.Empty); // Clear summary text;
    }

    /// <summary>
    /// Deactivates the PCSystem and invokes the close event.
    /// </summary>
    public void OnExit()
    {
        onClose?.Invoke();
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// Changes the current state of the PCSystem.
    /// </summary>
    /// <param name="stateName">The name of the new state to transition to.</param>
    public async void ChangeState(string stateName)
    {
        if (currentState == stateName) return; // No cambiar si ya estamos en este estado

        currentAction = 0;
        UpdateActionSelection(0);

        if (!string.IsNullOrEmpty(currentState))
        {
            backwardStack.Push(currentState); // Registrar el estado actual antes de cambiar
            CurrentScreen?.ExitState();
        }

        if (StatesDict.TryGetValue(stateName, out var newState))
        {
            currentState = stateName;
            newState.Initialize(this);
            newState.EnterState();
            await UniTask.WaitForSeconds(0.5f);
            newState.ShowSummary(0);
            AudioManager.Instance.PlaySFX("AcceptButton");
        }
        else
        {
            Debug.LogError($"State {stateName} not found in StatesDict!");
        }
    }

    public void GoBack()
    {
        print(backwardStack.Count);
        if (backwardStack.Count > 0)
        {
            string previousState = backwardStack.Pop();
            ChangeState(previousState);
        }
    }

    #endregion

    #region Display

    /// <summary>
    /// Displays the available actions for the player to choose from.
    /// </summary>
    /// <param name="actions">List of actions to display.</param>
    public void DisplayActions(List<TMP_Text> actions)
    {
        actionTexts.Clear(); // Clear current action texts
        actionTexts = actions; // Update action texts
        UpdateActionSelection(0); // Reset action selection to the first action
    }

    public async void ShowSummary(string summary)
    {
        summaryPanel.gameObject.SetActive(summary == string.Empty || summary == "" ? false : true);
        summaryText.text = await LocalizationsManager.Instance.GetString(summary);
        LayoutRebuilder.ForceRebuildLayoutImmediate(summaryText.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(summaryPanel.GetComponent<RectTransform>());
    }

    // Updates the currently selected action in the UI.
    public void UpdateActionSelection(int selectedAction)
    {
        currentAction = selectedAction;
        for (int i = 0; i < actionTexts.Count; i++)
        {
            actionTexts[i].color = i == selectedAction ? highLightColor : Color.black;
        }
    }

    #endregion

    #region Inputs

    public void HandleSelectionUp() => CurrentScreen?.HandleSelectionUp();

    public void HandleSelectionDown() => CurrentScreen?.HandleSelectionDown();

    public void HandleSelectionLeft() => CurrentScreen?.HandleSelectionLeft();

    public void HandleSelectionRight() => CurrentScreen?.HandleSelectionRight();

    public void AcceptSelection() => CurrentScreen?.AcceptSelection();

    public void CancelSelection() => CurrentScreen?.CancelSelection();

    #endregion
}
