using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleDialogBox : MonoBehaviour
{
    [SerializeField] TMP_Text dialogText;
    [SerializeField] Color highLightColor;
    [SerializeField] GameObject actionSelector, movementsSelector, movementsDetails; // UI selectors for actions and movements.
    [SerializeField] public List<TMP_Text> actionTexts, moveTexts, disabledMovements;
    [SerializeField] TMP_Text ppText, typeText;

    private bool isWriting = false; // Flag 

    #region Dialog Management

    /// <summary>
    /// Sets the dialog text with a typing animation.
    /// </summary>
    /// <param name="dialog">The dialog text to display.</param>
    public async UniTask SetDialog(string dialog)
    {
        if (isWriting)
            return;

        isWriting = true;
        await Utils.WritteLine(dialog, dialogText);
        isWriting = false;
    }

    /// <summary>
    /// Asynchronously sets the names of the available movements in the move texts.
    /// </summary>
    /// <param name="movements">List of movements to display.</param>
    public async void SetMoveNamesAsync(List<Movement> movements)
    {
        disabledMovements = new List<TMP_Text>();

        // Recorre cada texto en moveTexts y actualiza su contenido de forma asíncrona
        for (int i = 0; i < moveTexts.Count; i++)
        {
            if (i < movements.Count)
            {
                // Obtén la localización del nombre del movimiento
                string localizedName = await LocalizationsManager.Instance.GetString(movements[i].Base.MovementName);

                // Si no hay traducción, usa el nombre original como fallback
                moveTexts[i].text = !string.IsNullOrEmpty(localizedName) ? localizedName : movements[i].Base.MovementName;
            }
            else
            {
                // Si no hay movimiento correspondiente, muestra un guion
                moveTexts[i].text = "-";
            }
        }
    }

    #endregion

    #region UI Enable/Disable Methods

    // Enables or disables the dialog text UI.
    public void EnableDialogText(bool enable) => dialogText.enabled = enable;

    // Enables or disables the action selector UI.
    public void EnableActionSelector(bool enable) => actionSelector.SetActive(enable);

    // Enables or disables the movements selector UI.
    public void EnableMovementsSelector(bool enable)
    {
        movementsSelector.SetActive(enable);
        movementsDetails.SetActive(enable);
    }

    #endregion

    #region Selection Updates

    /// Updates the action selection highlighting based on the selected index.
    public void UpdateActionSelection(int selectedAction) =>
     actionTexts.ForEach(text => text.color = actionTexts.IndexOf(text) == selectedAction ? highLightColor : Color.black);

    // Disables a specific movement by changing its text color to gray.
    public void DisableMovement(int selectedAction)
    {
        moveTexts[selectedAction].color = Color.gray;

        if (!disabledMovements.Contains(moveTexts[selectedAction]))
            disabledMovements.Add(moveTexts[selectedAction]);
    }

    // Checks if all movements are disabled.
    public bool WithNotMovements()=> moveTexts.Count == disabledMovements.Count;

    /// Updates the movement selection highlighting and displays movement details.
    public void UpdateMovementsSelection(int selectedMove, Movement movement)
    {
        moveTexts.ForEach(text =>
        {
            if (moveTexts.IndexOf(text) == selectedMove)
            {
                text.color = highLightColor; // Resalta el texto seleccionado.
            }
            else
            {
                text.color = disabledMovements.Contains(text) ? Color.gray : Color.black; // Gris si está deshabilitado, negro si no.
            }
        });

        ppText.text = $"PP {movement.PP}/{movement.Base.PP}";
        typeText.text = $"{movement.Base.Type}";
    }

    #endregion
}
