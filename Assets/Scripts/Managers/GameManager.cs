using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { GAME, BATTLE, PC, DIALOG, NOTHING }

public class GameManager : CustomSingleton<GameManager>
{
    //States
    private GameState currentState;
    private GameState previousState;

    //private variables
    private FighterController currentFighterEnemy;
    private CameraController cameraController;
    private int originalCullingMask; // Stores the camera's original culling mask

    //public variables
    [SerializeField] InputManager inputManager;
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] PCSystem pcSystem;
    [SerializeField] Camera worldCamera;

    public event Action<GameState, GameState> OnGameStateChanged;

    /// <summary>
    /// Gets or sets the current game state, handling transitions between states.
    /// </summary>
    public GameState CurrentState
    {
        get => currentState;
        private set
        {
            if (currentState != value)
            {
                previousState = currentState;
                currentState = value;

                // Trigger the state change event
                OnGameStateChanged?.Invoke(previousState, currentState);
                HandleStateChange(previousState, currentState);
            }
        }
    }

    private new async void Awake()
    {
        CurrentState = GameState.NOTHING;
        int currentLanguage = LocalizationsManager.Instance.currentLanguageIndex;
        await LocalizationsManager.Instance.ChangeLanguage(currentLanguage);
    }

    void Start()
    {
        AudioManager.Instance.PauseBackgroundMusic(false);
        AudioManager.Instance.PlayBackgroundMusic("Home");
        cameraController = worldCamera.GetComponent<CameraController>();
        originalCullingMask = worldCamera.cullingMask;

        // Subscribe to battle system events
        battleSystem.onBattleOver += OnBattleOver;

        // Subscribe to player interaction events
        playerController.onEnterFighterView += async (Collider2D collider) =>
        {
            if (collider.transform.parent.TryGetComponent<FighterController>(out FighterController controller))
            {
                AudioManager.Instance.PauseBackgroundMusic(true);
                AudioManager.Instance.PlaySFX("ExclamationSound");
                AudioManager.Instance.PlaySFX("StartBattle");
                DisableInput();
                await controller.TriggerBattle(playerController);
            }
        };

        // Subscribe to dialog events
        DialogManager.Instance.onShowDialog += () => OnDialogue();
        DialogManager.Instance.onCloseDialog += () => OnGame();
        pcSystem.onClose += TurnOffPC;

        // Start initial dialog
        DialogManager.Instance.ShowDialog("INIT_DIALOG", () =>
        {
            OnGame();
        });
    }


    #region Input subscriptions

    /// <summary>
    /// Handles actions required when transitioning between game states.
    /// </summary>
    private void HandleStateChange(GameState previous, GameState current)
    {
        // Unsubscribe from previous state's events
        UnsubscribeFromState(previous);

        // Subscribe to the new state's events and configure the game
        switch (current)
        {
            case GameState.BATTLE:
                inputManager.SetContext(InputManager.InputContext.UI);
                battleSystem.gameObject.SetActive(true);
                worldCamera.gameObject.SetActive(false);
                SubscribeToBattleEvents();
                break;
            case GameState.GAME:
                inputManager.SetContext(InputManager.InputContext.PLAYER);
                SubscribeToGameEvents();
                break;
            case GameState.PC:
                inputManager.SetContext(InputManager.InputContext.UI);
                SubscribeToPCEvents();
                break;
            case GameState.DIALOG:
                inputManager.SetContext(InputManager.InputContext.UI);
                SubscribeToDialogEvents();
                break;
            case GameState.NOTHING:
                inputManager.SetContext(InputManager.InputContext.DISABLE);
                break;
        }
    }

    // Unsubscribes from the input events of the given state.
    private void UnsubscribeFromState(GameState state)
    {
        switch (state)
        {
            case GameState.GAME:
                inputManager.onAcceptButton -= playerController.OnAction;
                break;
            case GameState.BATTLE:
                inputManager.onUpButton -= battleSystem.HandleSelectionUp;
                inputManager.onDownButton -= battleSystem.HandleSelectionDown;
                inputManager.onRightButton -= battleSystem.HandleSelectionRight;
                inputManager.onLeftButton -= battleSystem.HandleSelectionLeft;
                inputManager.onAcceptButton -= battleSystem.SelectAction;
                inputManager.onCancelButton -= battleSystem.CancelAction;
                break;
            case GameState.PC:
                inputManager.onUpButton -= pcSystem.HandleSelectionUp;
                inputManager.onDownButton -= pcSystem.HandleSelectionDown;
                inputManager.onLeftButton -= pcSystem.HandleSelectionLeft;
                inputManager.onRightButton -= pcSystem.HandleSelectionRight;
                inputManager.onAcceptButton -= pcSystem.AcceptSelection;
                inputManager.onCancelButton -= pcSystem.CancelSelection;
                break;
            case GameState.DIALOG:
                inputManager.onUpButton -= DialogManager.Instance.HandleSelectionUp;
                inputManager.onDownButton -= DialogManager.Instance.HandleSelectionDown;
                inputManager.onAcceptButton -= DialogManager.Instance.AcceptSelection;
                inputManager.onCancelButton -= DialogManager.Instance.CancelSelection;
                break;
        }
    }

    // Subscribes to input events for the Game, Battle,PC or Dialog states.
    private void SubscribeToGameEvents()
    {
        inputManager.onAcceptButton += playerController.OnAction;
    }

    private void SubscribeToBattleEvents()
    {
        inputManager.onUpButton += battleSystem.HandleSelectionUp;
        inputManager.onDownButton += battleSystem.HandleSelectionDown;
        inputManager.onRightButton += battleSystem.HandleSelectionRight;
        inputManager.onLeftButton += battleSystem.HandleSelectionLeft;
        inputManager.onAcceptButton += battleSystem.SelectAction;
        inputManager.onCancelButton += battleSystem.CancelAction;
    }

    private void SubscribeToPCEvents()
    {
        inputManager.onUpButton += pcSystem.HandleSelectionUp;
        inputManager.onDownButton += pcSystem.HandleSelectionDown;
        inputManager.onLeftButton += pcSystem.HandleSelectionLeft;
        inputManager.onRightButton += pcSystem.HandleSelectionRight;
        inputManager.onAcceptButton += pcSystem.AcceptSelection;
        inputManager.onCancelButton += pcSystem.CancelSelection;

    }

    private void SubscribeToDialogEvents()
    {
        inputManager.onUpButton += DialogManager.Instance.HandleSelectionUp;
        inputManager.onDownButton += DialogManager.Instance.HandleSelectionDown;
        inputManager.onAcceptButton += DialogManager.Instance.AcceptSelection;
        inputManager.onCancelButton += DialogManager.Instance.CancelSelection;
    }

    #endregion

    #region States controll
    public void OnGame() => CurrentState = GameState.GAME;

    public void OnDialogue() => CurrentState = GameState.DIALOG;

    public void DisableInput() => CurrentState = GameState.NOTHING;

    #endregion

    #region Game events

    //PC system
    public void TurnOnPC()
    {
        CurrentState = GameState.PC;
        pcSystem.OnEnter();
    }

    public async void TurnOffPC()
    {
        CurrentState = GameState.NOTHING;
        await UniTask.WaitForSeconds(1);
        DialogManager.Instance.ShowDialog("PC_OFF", () =>
        {
            OnGame();
        });
    }

    //Battle
    public void StartBattle(FighterController fighterEnemy)
    {
        AudioManager.Instance.StopSFX();

        if (fighterEnemy != null)
        {
            currentFighterEnemy = fighterEnemy;
            CurrentState = GameState.BATTLE;
            battleSystem.gameObject.SetActive(true);
            worldCamera.gameObject.SetActive(false);
            battleSystem.StartBattle(fighterEnemy.getfighterBase);
        }
    }

    public void OnBattleOver(bool playerWon)
    {
        if (playerWon)
        {
            AudioManager.Instance.PauseBackgroundMusic(false);
            currentFighterEnemy.Defeated = true;
            OnGame();
            battleSystem.gameObject.SetActive(false);
            worldCamera.gameObject.SetActive(true);
        }
        else
        {            
            SceneManager.LoadScene("GameOver");
        }
    }

    //Tp Player (Portals or doors)
    public async void TpPlayer(Vector2 minBounds, Vector2 maxBounds, Vector3 playerPos, string backGroundmusicKey)
    {
        DisableInput();
        AudioManager.Instance.PlaySFX("DoorSFX");
        worldCamera.cullingMask = 0;
        playerController.transform.position = playerPos;
        await cameraController.ConfigureCamera(minBounds, maxBounds);
        worldCamera.cullingMask = originalCullingMask;

        if (!string.IsNullOrEmpty(backGroundmusicKey))
            AudioManager.Instance.PlayBackgroundMusic(backGroundmusicKey);

        await UniTask.WaitForSeconds(1);
        OnGame();
    }

    #endregion
}