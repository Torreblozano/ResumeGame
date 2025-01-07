using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DialogManager : CustomSingleton<DialogManager>
{
    // Selector UI Elements
    [SerializeField] List<TMP_Text> actionTexts;
    [SerializeField] Color highLightColor;
    [SerializeField] private GameObject optionSelector;

    // Dialog UI Elements
    [SerializeField] GameObject dialogPanel;
    [SerializeField] TMP_Text dialogText;
    [SerializeField] int lettersSpeed = 40;

    // Dialog management
    private List<string> completeDialog = new List<string>();
    private int index;

    // Actions for handling user responses
    private Action onAction; // Action to perform after the dialog closes
    private int currentAction; // Current selected action index
    private Action onYesAction; // Action for "Yes" response
    private Action onNoAction; // Action for "No" response

    //Delegates
    public delegate void OnShowDialog();
    public OnShowDialog onShowDialog;

    public delegate void OnCloseDialog();
    public OnCloseDialog onCloseDialog;

    #region Simple dialog

    /// <summary>
    /// Displays a simple dialog using a specified key to retrieve the dialog text.
    /// </summary>
    /// <param name="dialogKey">Key for retrieving dialog text from localization manager.</param>
    /// <param name="onACtion">Action to perform after the dialog closes.</param>
    public async void ShowDialog(string dialogKey, Action onACtion = null)
    {
        onAction = onACtion;

        // Clear
        dialogText.text = "";
        completeDialog = new List<string>();
        index = 0;

        // Hide selector
        optionSelector.SetActive(false);

        onShowDialog?.Invoke();

        // dialog
        dialogPanel.SetActive(true);
        var dialog = await LocalizationsManager.Instance.GetDialog(dialogKey);
        completeDialog = dialog.Split("<br>").ToList();
        WritteLine();
    }

    // Writes the current line of dialog to the dialog text component.
    private async void WritteLine()
    {
        await Utils.WritteLine(completeDialog[index], dialogText);
        await UniTask.WaitForSeconds(1);
        NextLine();
    }

    // Advances to the next line of dialog or closes the dialog if it's the last line.
    public void NextLine()
    {
        index++;

        if (index <= completeDialog.Count - 1)
        {
            WritteLine(); // Write the next line if available
        }
        else
        {
            CloseDialog(); // Close the dialog if no more lines
        }
    }

    #endregion

    #region Dialog with selection
    /// <summary>
    /// Displays a dialog with selectable options (Yes/No).
    /// </summary>
    /// <param name="onYes">Action to perform when "Yes" is selected.</param>
    /// <param name="onNo">Action to perform when "No" is selected.</param>
    public void ShowOptions(Action onYes, Action onNo)
    {
        onShowDialog?.Invoke();
        dialogPanel.SetActive(true);

        optionSelector.SetActive(true); // Mostrar el selector de opciones
        currentAction = 0;
        UpdateActionSelection(currentAction);

        onYesAction = onYes;
        onNoAction = onNo;
    }

    public void SelectYes()
    {
        AudioManager.Instance.PlaySFX("PCStart");
        optionSelector.SetActive(false);
        dialogPanel.SetActive(false); // Optionally close the dialog after selection
        onYesAction?.Invoke();
        onYesAction = null;
        onNoAction = null;
    }

    public void SelectNo()
    {
        optionSelector.SetActive(false);
        dialogPanel.SetActive(false); // Optionally close the dialog after selection
        onNoAction?.Invoke();
        onYesAction = null;
        onNoAction = null;
    }

    private void CloseDialog()
    {
        dialogPanel.SetActive(false);
        onCloseDialog?.Invoke();
        onAction?.Invoke();
        onAction = null;
    }

    #endregion

    #region Inputs

    public void UpdateActionSelection(int selectedAction) =>
    actionTexts.ForEach(text => text.color = actionTexts.IndexOf(text) == selectedAction ? highLightColor : Color.black);

    private void HandleSelection(int step)
    {
        if (!optionSelector.activeInHierarchy)
            return;

        AudioManager.Instance.PlaySFX("HandleButton");

        currentAction = currentAction + step;

        if (currentAction > actionTexts.Count - 1)
        {
            currentAction = actionTexts.Count - 1;
        }

        if (currentAction < 0)
        {
            currentAction = 0;
        }
        UpdateActionSelection(currentAction);
    }

    public void HandleSelectionUp() => HandleSelection(-1);

    public void HandleSelectionDown() => HandleSelection(1);

    public void AcceptSelection()
    {
        if (!optionSelector.activeInHierarchy)
            return;

        if (currentAction == 0)
            SelectYes();
        else
            SelectNo();
    }

    public void CancelSelection()
    {
        if (!optionSelector.activeInHierarchy)
            return;

        SelectNo();
    }

    #endregion
}
