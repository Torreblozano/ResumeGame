using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Base class representing a state in the menu system. 
/// This class provides common functionality for all menu states and should be inherited by specific menu state implementations.
/// </summary>
public abstract class MenuStateBase : MonoBehaviour
{
    protected PCSystem pcSystem;
    public List<TMP_Text> actions = new List<TMP_Text>();
    public List<string> summary_key = new List<string>();

    public void Initialize(PCSystem system)
    {
        pcSystem = system; // Assign the PCSystem reference
    }

    public void ShowSummary(int index)
    {
        if (index <= summary_key.Count - 1)
            pcSystem.ShowSummary(summary_key[index]);
        else
            pcSystem.ShowSummary(string.Empty);
    }

    public abstract void EnterState();

    public abstract void ExitState();

    #region Inputs Virtual

    public virtual void HandleSelectionUp() { AudioManager.Instance.PlaySFX("HandleButton"); }

    public virtual void HandleSelectionDown() { AudioManager.Instance.PlaySFX("HandleButton"); }

    public virtual void HandleSelectionLeft() { AudioManager.Instance.PlaySFX("HandleButton"); }

    public virtual void HandleSelectionRight() { AudioManager.Instance.PlaySFX("HandleButton"); }

    public virtual void AcceptSelection() { }

    public virtual void CancelSelection() { }

    #endregion

}
