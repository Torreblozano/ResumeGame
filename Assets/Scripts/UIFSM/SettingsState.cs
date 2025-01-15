using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SettingsState : MenuStateBase
{
    // Private variables
    [SerializeField] private GameObject optionsPanel;
    private int currentLanguage;

    // Public variables
    [SerializeField] TMP_Text languagesText;
    public List<string> languages = new List<string>();

    #region Override methods

    public override void EnterState()
    {
        currentLanguage = LocalizationsManager.Instance.currentLanguageIndex;
        languagesText.text = languages[currentLanguage];

        pcSystem.DisplayActions(new List<TMP_Text>(actions));
        this.gameObject.SetActive(true);
        optionsPanel.SetActive(false);
    }

    public override async void AcceptSelection()
    {
        if (!optionsPanel.activeSelf) 
        {
            int currentAction = pcSystem.CurrentAction;

            if (currentAction == 0)
            {
                optionsPanel.SetActive(true);
            }
            else
                pcSystem.GoBack();
        }
        else
        {
            await LocalizationsManager.Instance.ChangeLanguage(currentLanguage);
            optionsPanel.SetActive(false);
        }            
    }

    public override void CancelSelection()
    {
        if (!optionsPanel.activeSelf)
        {
            pcSystem.UpdateActionSelection(actions.Count-1);
            pcSystem.ShowSummary(string.Empty);
        }
        else
        {
            optionsPanel.SetActive(false);
        }
    }

    public override void ExitState()
    {
        this.gameObject.SetActive(false);
    }

    public override void HandleSelectionUp()
    {
        if (optionsPanel.activeSelf)
            return;

        HandleSelection(-1);
        base.HandleSelectionUp();
    }

    public override void HandleSelectionDown()
    {
        if (optionsPanel.activeSelf)
            return;

        HandleSelection(1);
        base.HandleSelectionDown();
    }

    public override void HandleSelectionLeft()
    {
        if (!optionsPanel.activeSelf)
            return;

        HandleOptionSelection(-1);
        base.HandleSelectionUp();
    }

    public override void HandleSelectionRight()
    {
        if (!optionsPanel.activeSelf)
            return;

        HandleOptionSelection(1);
        base.HandleSelectionDown();
    }

    #endregion

    #region Private methods

    private void HandleOptionSelection(int step)
    {
        currentLanguage = (currentLanguage + step + languages.Count) % languages.Count;
        languagesText.text = languages[currentLanguage];
    }

    private void HandleSelection(int step)
    {
        int currentAction = pcSystem.CurrentAction;
        int nextStep = currentAction + step;

        if (nextStep > actions.Count - 1)
        {
            nextStep = actions.Count - 1;
        }

        if (nextStep < 0)
        {
            nextStep = 0;
        }
        pcSystem.UpdateActionSelection(nextStep);
    }

    #endregion

}
