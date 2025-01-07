using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CVState : MenuStateBase
{
    #region Override methods

    public override void EnterState()
    {
        pcSystem.DisplayActions(new List<TMP_Text>(actions));
        this.gameObject.SetActive(true);
    }

    public override void ExitState()
    {
        this.gameObject.SetActive(false);
    }

    public override void CancelSelection()
    {
        pcSystem.UpdateActionSelection(actions.Count - 1);
        pcSystem.ShowSummary(string.Empty);
    }

    public override void AcceptSelection()
    {
        int currentAction = pcSystem.CurrentAction;

        if (currentAction == (actions.Count - 1))
            pcSystem.GoBack();

    }

    public override void HandleSelectionUp()
    {
        HandleSelection(-1);
        base.HandleSelectionUp();
    }

    public override void HandleSelectionDown()
    {
        HandleSelection(1);
        base.HandleSelectionDown();
    }

    #endregion

    #region Private methods

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
        ShowSummary(nextStep);
    }

    #endregion
}
