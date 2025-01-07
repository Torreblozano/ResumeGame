using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class BattleHud : MonoBehaviour
{

    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text lvlText;
    [SerializeField] Scrollbar hpBar;

    Fighter currentFighter;

    public void SetData(Fighter fighter)
    {
        currentFighter = fighter;
        nameText.text = fighter.Base.Name;
        lvlText.text = "Lvl " + fighter.Level;
        UpdateHP();
    }

    public void UpdateHP()
    {
        hpBar.size = Mathf.Clamp01((float)currentFighter.HP / currentFighter.MaxHp);
    }
}
