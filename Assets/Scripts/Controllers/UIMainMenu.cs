using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIMainMenu : MonoBehaviour
{
    public GameObject mainPanel, languagePanel;
    public PlayerInput playerInput;

    private void Awake()
    {
        playerInput.enabled = false;
        AudioManager.Instance.PlayBackgroundMusic("GameOverScene");
    }

    private async void Start()
    {
        await UniTask.WaitForSeconds(2);
        FadeOutAndActivateLanguagePanel();
    }

    private void FadeOutAndActivateLanguagePanel()
    {
        CanvasGroup canvasGroup = mainPanel.GetComponent<CanvasGroup>();

        if (canvasGroup != null)
        {
            canvasGroup.DOFade(0f, 1f)
                .OnComplete(async () => {
                    mainPanel.SetActive(false);
                    await UniTask.WaitForSeconds(2f);
                    languagePanel.SetActive(true);
                    playerInput.enabled = true;
                });
        }
    }

    public void RunGame(int index)
    {
        LocalizationsManager.Instance.ChangeLanguage(index);
        SceneManager.LoadScene(1);
    }
}
