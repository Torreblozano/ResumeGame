using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Networking;

/// <summary>
/// Represents the main menu state of the application, derived from the MenuStateBase class.
/// Handles the display of actions and the user interactions within the main menu.
/// </summary>
public class MainMenuState : MenuStateBase
{
    #region Override methods
    public override void EnterState()
    {
        pcSystem.DisplayActions(new List<TMP_Text>(actions));
        this.gameObject.SetActive(true); ;
    }

    public override void AcceptSelection()
    {
        int currentAction = pcSystem.CurrentAction;

        if (currentAction == 0)
            pcSystem.ChangeState("CV");
        else if (currentAction == 1)
            SendMail();
        else if (currentAction == 2)
            Application.OpenURL("https://www.linkedin.com/in/torreblozano/");
        else if (currentAction == 3)
            Application.OpenURL("https://github.com/Torreblozano");
        else if (currentAction == 4)
            pcSystem.ChangeState("Settings");
        else
            pcSystem.OnExit();
    }

    public override void ExitState()
    {
        this.gameObject.SetActive(false);
    }

    public override void CancelSelection()
    {
        pcSystem.UpdateActionSelection(actions.Count - 1);
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

    private void SendMail()
    {
        // Dirección de correo, asunto y cuerpo del mensaje
        string email = "torreb1990@gmail.com";
        string subject = "";
        string body = "";

        // Construir la URL mailto
        string mailto = $"mailto:{email}?subject={UnityWebRequest.EscapeURL(subject)}&body={UnityWebRequest.EscapeURL(body)}";

        // Abrir la URL
        Application.OpenURL(mailto);
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
        ShowSummary(nextStep);
    }

    #endregion
}
