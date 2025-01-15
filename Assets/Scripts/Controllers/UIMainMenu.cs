using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using DG.Tweening;
using Cysharp.Threading.Tasks; // Asegúrate de que DOTween está correctamente instalado en tu proyecto.

public class UIMainMenu : MonoBehaviour
{
    public GameObject mainPanel, languagePanel, startbuttonPanel;
    public PlayerInput playerInput;

    private void Awake()
    {
        startbuttonPanel.SetActive(true);
        playerInput.enabled = false;
        AudioManager.Instance.PlayBackgroundMusic("PokemonTheme");
    }

    public async void StartButton()
    {
        // Reproduce el sonido de aceptar.
        AudioManager.Instance.PlaySFX("AcceptButton");
        AudioManager.Instance.PauseBackgroundMusic(true);
        // Desactiva el panel de botón de inicio y activa el panel principal.
        startbuttonPanel.SetActive(false);
        mainPanel.SetActive(true);

        await UniTask.WaitForSeconds(3);
        // Realiza el fadeOut de mainPanel y fadeIn de languagePanel.
        FadeOutMainPanelAndShowLanguagePanel();
    }

    private void FadeOutMainPanelAndShowLanguagePanel()
    {
        // Asegúrate de que mainPanel tiene un CanvasGroup asignado.
        CanvasGroup mainCanvasGroup = mainPanel.GetComponent<CanvasGroup>();
        CanvasGroup languageCanvasGroup = languagePanel.GetComponent<CanvasGroup>();

        if (mainCanvasGroup != null && languageCanvasGroup != null)
        {
            // Configura el fadeOut de mainPanel.
            mainCanvasGroup.DOFade(0f, 2f).OnComplete(() =>
            {
                mainPanel.SetActive(false); // Desactiva mainPanel después del fadeOut.

                // Activa languagePanel y realiza el fadeIn.
                languagePanel.SetActive(true);
                languageCanvasGroup.alpha = 0f; // Asegúrate de que empieza invisible.
                languageCanvasGroup.DOFade(1f, 2f).OnComplete(() =>
                {
                    // Habilita playerInput después de que termine el fadeIn.
                    playerInput.enabled = true;
                });
            });
        }
        else
        {
            Debug.LogError("CanvasGroup no encontrado en los paneles.");
        }
    }

    public async void RunGame(int index)
    {
        // Cambia el idioma y carga la escena principal.
        await LocalizationsManager.Instance.ChangeLanguage(index);
        SceneManager.LoadScene(1);
    }
}
