using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIGameOverController : MonoBehaviour
{
    public PlayerInput playerInput;

    // Start is called before the first frame update
    async void Start()
    {
        playerInput.enabled = false;
        AudioManager.Instance.PlayBackgroundMusic("GameOverScene");
        AudioManager.Instance.PauseBackgroundMusic(false);
        await UniTask.WaitForSeconds(2);
        playerInput.enabled = true;
    }

    public void ChangeScene(int i) => SceneManager.LoadScene(i);
}
