using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;

public class BattleUnit : MonoBehaviour
{
    public FighterBase fighterBase;
    public int level;
    public Fighter fighter;
    public bool isPlayerUnit;
    public TMP_Text textAnimationState;

    private Image image;
    private Vector3 originalPosition;
    private Color originalColor;

    private void Awake()
    {
        image = GetComponent<Image>();
        originalPosition = image.transform.localPosition;
        originalColor = image.color;
    }

    public void SetUp()
    {
        if (fighter != null && isPlayerUnit)
            return;

        fighter = new Fighter(fighterBase);
        level = fighterBase.Level;
        image.sprite = fighter.Base.Sprite;

        PlayEnterAnimation();
    }

    #region DOTWeens animation

    public void PlayEnterAnimation()
    {
        if (isPlayerUnit)
        {
            image.transform.localPosition = new Vector3(-2000, originalPosition.y);
        }
        else
        {
            image.transform.localPosition = new Vector3(2000, originalPosition.y);
        }

        image.transform.DOLocalMoveX(originalPosition.x, 1f);
        var sequence = DOTween.Sequence();
        sequence.Join(image.DOFade(1f, 0f));
    }

    public void PlayAttackAnimation()
    {
        var sequence = DOTween.Sequence();

        if (isPlayerUnit)
        {
            sequence.Append(image.transform.DOLocalMoveX(originalPosition.x + 100, 0.25f));
        }
        else
        {
            sequence.Append(image.transform.DOLocalMoveX(originalPosition.x - 100, 0.25f));
        }

        sequence.Append(image.transform.DOLocalMoveX(originalPosition.x, 0.25f));
    }

    public void PlayHitAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOColor(Color.gray, 0.1f));
        sequence.Append(image.DOColor(originalColor, 0.1f));
        AudioManager.Instance.PlaySFX("HitSFX");
    }

    public void PlayFaintAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.transform.DOLocalMoveY(originalPosition.y - 10f, 0.5f));
        sequence.Join(image.DOFade(0f, 0.5f));
        AudioManager.Instance.PlaySFX("GameOverSFX");
    }

    public void PlayDefenseAnimation()
    {
        var sequence = DOTween.Sequence();

        sequence.Append(image.transform.DOLocalMove(new Vector3(originalPosition.x + 10, originalPosition.y + 10), 0.1f));
        sequence.Append(image.transform.DOLocalMove(new Vector3(originalPosition.x, originalPosition.y + 10), 0.1f));
        sequence.Append(image.transform.DOLocalMove(new Vector3(originalPosition.x - 10, originalPosition.y + 10), 0.1f));
        sequence.Append(image.transform.DOLocalMove(new Vector3(originalPosition.x - 10, originalPosition.y - 10), 0.1f));
        sequence.Append(image.transform.DOLocalMove(new Vector3(originalPosition.x, originalPosition.y - 10), 0.1f));
        sequence.Append(image.transform.DOLocalMove(new Vector3(originalPosition.x + 10, originalPosition.y - 10), 0.1f));

        sequence.Append(image.transform.DOLocalMove(originalPosition, 0.1f));
        AudioManager.Instance.PlaySFX("BoingSFX");
    }

    public void PlayHPAnimation()
    {
        var sequence = DOTween.Sequence();

        sequence.Append(image.transform.DOScale(new Vector3(0.8f, 0.8f, 1f), 0.2f));
        sequence.Append(image.transform.DOScale(Vector3.one, 0.2f));
        AudioManager.Instance.PlaySFX("HPSFX");
    }

    public void PlayDecreasetAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.transform.DOLocalMoveY(originalPosition.y - 20f, 0.1f));

        sequence.Append(image.transform.DOLocalMoveY(originalPosition.y, 0.05f).SetEase(Ease.InOutSine));
        sequence.Append(image.transform.DOLocalMoveY(originalPosition.y - 10f, 0.05f).SetEase(Ease.InOutSine));
        sequence.Append(image.transform.DOLocalMoveY(originalPosition.y, 0.05f).SetEase(Ease.InOutSine));
               
        sequence.Append(image.DOColor(Color.red, 0.2f));
        sequence.Append(image.DOColor(originalColor, 0.2f));
      
        sequence.Append(image.transform.DOLocalMove(originalPosition, 0.1f));
        AudioManager.Instance.PlaySFX("Hit2SFX");
    }

    public void PlayBoostDefenseAnimation()
    {
        var sequence = DOTween.Sequence();

        sequence.Append(image.DOColor(Color.green, 0.2f));
        sequence.Append(image.DOColor(originalColor, 0.2f));
        sequence.Append(image.DOColor(Color.green, 0.2f));
        sequence.Append(image.DOColor(originalColor, 0.2f));

        AudioManager.Instance.PlaySFX("BoostDefenseSFX");
    }

    public void PlayConfusionAnimation()
    {
        textAnimationState.text = "";

        var sequence = DOTween.Sequence();

        for (int i = 0; i < 5; i++)
        {
            sequence.AppendCallback(() => textAnimationState.text += "?"); 
            sequence.AppendInterval(0.3f);
        }

        sequence.AppendCallback(() => textAnimationState.text = "");
        AudioManager.Instance.PlaySFX("BoingSFX");
    }

    #endregion

}
