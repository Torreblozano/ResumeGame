using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerController;

public enum VIEW { RIGHT, LEFT, UP, DOWN }

/// <summary>
/// NPC Fighter Behavior
/// </summary>
public class FighterController : Character, IInteractable
{
    // Public variables
    [SerializeField] VIEW view;
    [SerializeField] FighterBase fighterBase;
    [SerializeField] private GameObject fov;

    // Private variables
    private bool defeated = false;
    private string exclamationEffectKey = "ExclamationEffect";
    private GameObject exclamation;
    private Dialog dialogs;

    private async void Start()
    {
        SetMainView();
        dialogs = GetComponent<Dialog>();

        GameObject exclamationAddressable = await Utils.InstantiateAddressable(exclamationEffectKey);

        if (exclamationAddressable != null)
        {
            exclamation = Instantiate(exclamationAddressable, transform);
            exclamation.transform.localPosition = new Vector3(0, 1.5f, 0);
            exclamation.gameObject.SetActive(false);
        }
    }

    #region Public methods

    public bool Defeated { set { defeated = value; fov.SetActive(!value); } }

    public FighterBase getfighterBase { get { return fighterBase; } }

    public async UniTask TriggerBattle(PlayerController playerController)
    {
        exclamation?.gameObject.SetActive(true);
        await UniTask.WaitForSeconds(0.5f);
        exclamation?.gameObject.SetActive(false);

        var targetPos = playerController.transform.position - transform.position;
        var targetPosNormalized = targetPos - targetPos.normalized;
        targetPosNormalized = transform.position + new Vector3((float)Math.Round(targetPosNormalized.x), (float)Math.Round(targetPosNormalized.y), 0);

        await Move(targetPosNormalized);

        DialogManager.Instance.ShowDialog(dialogs.LocalizationKeys[0], () =>
        {
            GameManager.Instance.StartBattle(this);
        });
    }

    public void BattleLost()
    {
        fov.gameObject.SetActive(false);
    }

    public void Interact()
    {
        if (!defeated)
        {
            var player = GameObject.FindFirstObjectByType<PlayerController>();
            player.onEnterFighterView?.Invoke(fov.GetComponent<Collider2D>());
        }
        else
        {
            DialogManager.Instance.ShowDialog(dialogs.LocalizationKeys[1], () =>
            {
                GameManager.Instance.OnGame();
            });
        }
    }

    #endregion

    #region Private methods        

    private void SetMainView()
    {
        if (view == VIEW.RIGHT)
        {
            animator.SetFloat("moveX", -1);
            animator.SetFloat("moveY", 0);
        }
        else if (view == VIEW.LEFT)
        {
            animator.SetFloat("moveX", 1);
            animator.SetFloat("moveY", 0);
        }
        else if (view == VIEW.UP)
        {
            animator.SetFloat("moveX", 0);
            animator.SetFloat("moveY", 1);
        }
        else
        {
            animator.SetFloat("moveX", 0);
            animator.SetFloat("moveY", -1);
        }
    }

    #endregion

}
