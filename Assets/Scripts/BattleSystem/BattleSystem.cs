using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public enum BattleState { START, PLAYER_ACTION, PLAYER_MOVE, ENEMY_MOVE, BUSY, END }

public class BattleSystem : MonoBehaviour
{
    [SerializeField] private BattleUnit playerUnit, enemyUnit; // References to the player and enemy units.
    [SerializeField] private BattleHud playerHud, enemyHud; // HUD displays for player and enemy stats.
    [SerializeField] private BattleDialogBox dialogbox; // Manages battle dialog and move selections.

    private BattleState state;    // Tracks the current state of the battle.
    private int currentAction;    // Tracks the player's current selected action.
    private int currentMove;      // Tracks the player's current selected move.

    public delegate void OnBattleOver(bool playerWon); // Handling battle end events
    public OnBattleOver onBattleOver;

    /// <summary>
    /// Initializes and starts the battle with the given enemy fighter.
    /// </summary>
    /// <param name="enemyScriptable">The enemy fighter's scriptable object.</param>
    public async void StartBattle(FighterBase enemyScriptable)
    {
        enemyUnit.fighterBase = enemyScriptable;
        currentAction = 0;
        await SetUp();
    }

    /// <summary>
    /// Sets up the initial state of the battle, including units, HUD, and dialog.
    /// </summary>
    private async UniTask SetUp()
    {
        state = BattleState.START;
        playerUnit.SetUp();
        enemyUnit.SetUp();

        playerHud.SetData(playerUnit.fighter);
        enemyHud.SetData(enemyUnit.fighter);

        dialogbox.EnableDialogText(true);
        dialogbox.EnableActionSelector(false);
        dialogbox.SetMoveNamesAsync(playerUnit.fighter.Movements);

        //Entry dialog
        var dialog = await GetDialog("BATTLE_ENTRY", enemyUnit.fighter.Base.Name);
        await dialogbox.SetDialog(dialog);
        await UniTask.WaitForSeconds(1);

        //Change state
        PlayerAction();
        UpdateDialogAndSelection();
    }

    #region Battle Dialog

    /// <summary>
    /// Retrieves and formats dialog based on the provided key and optional values for replacements.
    /// </summary>
    private async UniTask<string> GetDialog(string key, string value1 = null, string value2 = null)
    {
        string dialog = await LocalizationsManager.Instance.GetDialog(key);

        if (value1 != null)
        {
            var translation1 = await LocalizationsManager.Instance.GetString(value1);

            if (!string.IsNullOrEmpty(translation1))
                value1 = translation1;

            value1 = Utils.CapitalizeFirstLetter(value1);
            dialog = dialog.Replace("{value1}", value1);
        }

        if (value2 != null)
        {
            var translation2 = await LocalizationsManager.Instance.GetString(value2);

            if (!string.IsNullOrEmpty(translation2))
                value2 = translation2;

            value2 = Utils.CapitalizeFirstLetter(value2);
            dialog = dialog.Replace("{value2}", value2);
        }

        return dialog;
    }

    private void UpdateDialogAndSelection()
    {
        dialogbox.UpdateActionSelection(currentAction);
        dialogbox.UpdateMovementsSelection(currentMove, playerUnit.fighter.Movements[currentMove]);
    }

    #endregion

    #region Stats

    /// <summary>
    /// Handles the player's action phase, enabling the action selector and checking for valid moves.
    /// </summary>
    public async void PlayerAction()
    {
        state = BattleState.BUSY;

        // Disable moves with no remaining PP
        playerUnit.fighter.Movements.ForEach(mov =>
        {
            if (mov.PP <= 0)
            {
                dialogbox.DisableMovement(playerUnit.fighter.Movements.IndexOf(mov));
            }
        });

        // Check if the player has any usable moves
        if (dialogbox.WithNotMovements())
        {
            var PPdialog = await GetDialog("NO_MOVEMENTS", playerUnit.fighter.Base.Name);
            await dialogbox.SetDialog(PPdialog);
            await EndBattle(playerUnit);
            return;
        }

        // Display dialog for the enemy's chosen move
        var dialog = await GetDialog("BATTLE_ACTION", playerUnit.fighter.Base.Name);
        await dialogbox.SetDialog(dialog);
        await UniTask.WaitForSeconds(2);

        state = BattleState.PLAYER_ACTION;

        dialogbox.UpdateActionSelection(0);
        dialogbox.EnableActionSelector(true);
        dialogbox.EnableDialogText(true);
        dialogbox.EnableMovementsSelector(false);
    }

    public void PlayerMove()
    {
        state = BattleState.PLAYER_MOVE;
        dialogbox.EnableActionSelector(false);
        dialogbox.EnableDialogText(false);
        dialogbox.EnableMovementsSelector(true);
    }

    /// <summary>
    /// Handles the enemy's move phase and executes a random move.
    /// </summary>
    private async UniTask EnemyMove()
    {
        state = BattleState.ENEMY_MOVE;
        // Choose a random movement for the enemy
        var movement = enemyUnit.fighter.GetRandomMovement();

        // Display dialog for the enemy's chosen move
        var dialog = await GetDialog("BATTLE_PLAYER_MOVE", enemyUnit.fighter.Base.Name, movement.Base.MovementName);
        await dialogbox.SetDialog(dialog);
        await UniTask.WaitForSeconds(1);

        // Execute the move's effects
        await ExecuteMove(movement, enemyUnit, playerUnit, enemyHud, playerHud);
        movement.PP--;

        // Update HUD to reflect new stats
        enemyHud.UpdateHP();
        playerHud.UpdateHP();

        await UniTask.WaitForSeconds(1);

        // Check if either unit's HP has dropped to zero, ending the battle if necessary
        if (playerUnit.fighter.HP <= 0)
        {
            await EndBattle(playerUnit);
        }
        else if (enemyUnit.fighter.HP <= 0)
        {
            await EndBattle(enemyUnit);
        }
        else
        {
            PlayerAction(); // Transition back to the player's action phase
        }
    }

    /// <summary>
    /// Executes the enemy's move, selecting a random movement and applying its effects.
    /// </summary>
    private async UniTask PerformPlayerMove()
    {
        state = BattleState.BUSY;

        // Retrieve the selected move
        var movement = playerUnit.fighter.Movements[currentMove];

        // Display dialog for the enemy's chosen move
        var dialog = await GetDialog("BATTLE_PLAYER_MOVE", playerUnit.fighter.Base.Name, movement.Base.MovementName);
        await dialogbox.SetDialog(dialog);
        await UniTask.WaitForSeconds(1);

        // Execute the move's effects
        await ExecuteMove(movement, playerUnit, enemyUnit, playerHud, enemyHud);
        movement.PP--;

        // Update HUD to reflect new stats
        enemyHud.UpdateHP();
        playerHud.UpdateHP();

        // Check if either unit's HP has dropped to zero, ending the battle if necessary
        await UniTask.WaitForSeconds(1);
        if (enemyUnit.fighter.HP <= 0)
        {
            await EndBattle(enemyUnit);
        }
        else if (playerUnit.fighter.HP <= 0)
        {
            await EndBattle(playerUnit);
        }
        else
        {
            await EnemyMove();
        }
    }

    /// <summary>
    /// Ends the battle, displaying appropriate dialog and animations.
    /// </summary>
    /// <param name="losingUnit">The unit that lost the battle.</param>
    private async UniTask EndBattle(BattleUnit losingUnit)
    {
        // Display the dialog for losing the battle
        var dialog = await GetDialog("BATTLE_LOST", losingUnit.fighter.Base.Name);
        await dialogbox.SetDialog(dialog);

        // Play the faint animation for the losing unit
        losingUnit.PlayFaintAnimation();
        state = BattleState.END;

        // Invoke the battleto GameManager
        onBattleOver?.Invoke(losingUnit == enemyUnit ? true : false);
    }

    #endregion

    #region Moves

    /// <summary>
    /// Executes a movement, applying its effects based on the movement type.
    /// </summary>
    private async UniTask ExecuteMove(Movement movement, BattleUnit attackerUnit, BattleUnit targetUnit, BattleHud attackerHud, BattleHud targetHud)
    {
        if (await AttackByConfusion(movement, attackerUnit))
            return; // Skip further processing if the attacker was confused and hurt themselves

        switch (movement.Base.Type)
        {
            case MovementType.NORMAL:
            case MovementType.PSY:

                await AttackMove(movement, attackerUnit, targetUnit);

                if (movement.Base.Type == MovementType.PSY && targetUnit.fighter.fighterState != FighterState.CONFUSE)
                {
                    //Set a delay
                    await UniTask.WaitForSeconds(1);
                    targetUnit.PlayConfusionAnimation();
                    await UniTask.WaitForSeconds(1);

                    // Display dialog for the enemy's chosen move
                    var dialog = await GetDialog("BATTLE_IS_CONFUSE", targetUnit.fighter.Base.Name);
                    await dialogbox.SetDialog(dialog);

                    Confuse(targetUnit);
                }

                break;

            case MovementType.DEF:
                await DefenseMove(movement, attackerUnit, targetUnit);
                break;

            case MovementType.HP:
                await HealthMove(movement, attackerUnit);
                attackerHud.UpdateHP();
                break;
        }
    }

    private async UniTask<bool> AttackByConfusion(Movement movement, BattleUnit attackerUnit)
    {
        // If is confuse, check randomly if the units hurts themselves
        if (attackerUnit.fighter.fighterState == FighterState.CONFUSE)
        {
            attackerUnit.PlayConfusionAnimation();

            var dialog = await GetDialog("BATTLE_IS_CONFUSE", attackerUnit.fighter.Base.Name);
            await dialogbox.SetDialog(dialog);

            await UniTask.WaitForSeconds(1);
            var randomAttack = Random.Range(0, 10);

            if (randomAttack >= 5) // 50% chance to attack themselves
            {
                // Se ataca a sí mismo
                dialog = await GetDialog("BATTLE_CONFUSION_DAMAGE", attackerUnit.fighter.Base.Name);
                await dialogbox.SetDialog(dialog);

                var selfDamageMovementBase = new MovementsBase();
                selfDamageMovementBase.Power = 3;
                var fakeMovement = new Movement(selfDamageMovementBase);
                await AttackMove(fakeMovement, attackerUnit, attackerUnit);
                return true;
            }

            var releaseState = Random.Range(0, 10);

            if (releaseState >= 5) // 50% chance to recover from confusion
            {
                Release(attackerUnit);
                dialog = await GetDialog("BATTLE_CONFUSION_END", attackerUnit.fighter.Base.Name);
                await dialogbox.SetDialog(dialog);
            }

            return false;
        }
        return false;
    }

    /// <summary>
    /// Executes an attack move, applying damage to the target unit.
    /// </summary>
    private async UniTask AttackMove(Movement movement, BattleUnit attackerUnit, BattleUnit targetUnit)
    {
        attackerUnit.PlayAttackAnimation();
        await UniTask.WaitForSeconds(1);
        targetUnit.PlayHitAnimation();
        Attack(movement, targetUnit, attackerUnit);
    }

    // Additional methods for DefenseMove, HealthMove, Attack, Confuse, and similar moves.
    private async UniTask DefenseMove(Movement movement, BattleUnit attackerUnit, BattleUnit targetUnit)
    {
        attackerUnit.PlayDefenseAnimation();
        await UniTask.WaitForSeconds(1);

        if (movement.Base.Power > 0)
        {
            attackerUnit.fighter.BoostDefense(movement);
            attackerUnit.PlayBoostDefenseAnimation();

            var dialog = await GetDialog("BATTLE_BOOST_DEFENSE", attackerUnit.fighter.Base.Name);
            await dialogbox.SetDialog(dialog);
        }
        else
        {
            targetUnit.fighter.DecreaseDefense(movement);
            targetUnit.PlayDecreasetAnimation();

            var dialog = await GetDialog("BATTLE_DECREASE_DEFENSE", targetUnit.fighter.Base.Name);
            await dialogbox.SetDialog(dialog);
        }
    }

    private async UniTask HealthMove(Movement movement, BattleUnit targetUnit)
    {
        targetUnit.fighter.Health(movement);

        var dialog = await GetDialog("BATTLE_RECOVER_HP", targetUnit.fighter.Base.Name);
        await dialogbox.SetDialog(dialog);
    }

    private void Attack(Movement movement, BattleUnit targetUnit, BattleUnit attackerUnit)
    {
        if (movement.Base.Power > 0)
        {
            targetUnit.fighter.TakeDamage(movement, attackerUnit.fighter);
        }
    }

    private void Confuse(BattleUnit targetUnit) => targetUnit.fighter.fighterState = FighterState.CONFUSE;

    private void Release(BattleUnit targetUnit) => targetUnit.fighter.fighterState = FighterState.OK;

    #endregion

    #region Inputs

    private void HandleSelection(int step, bool isVertical)
    {
        switch (state)
        {
            case BattleState.PLAYER_ACTION:
                if (isVertical)
                {
                    AudioManager.Instance.PlaySFX("HandleButton");
                    currentAction = Mathf.Clamp(currentAction + (step > 0 ? -1 : 1), 0, 1);
                    dialogbox.UpdateActionSelection(currentAction);
                }
                break;

            case BattleState.PLAYER_MOVE:

                AudioManager.Instance.PlaySFX("HandleButton");
                UpdateCurrentMove(step, isVertical);
                break;
        }
    }

    private void UpdateCurrentMove(int step, bool isVertical)
    {
        if (isVertical)
        {
            currentMove += step < 0 ? 2 : -2;
            currentMove = Mathf.Clamp(currentMove, 0, playerUnit.fighter.Movements.Count - 1);
        }
        else
        {
            currentMove += step < 0 ? -1 : 1;
            currentMove = Mathf.Clamp(currentMove, 0, playerUnit.fighter.Movements.Count - 1);
        }
        dialogbox.UpdateMovementsSelection(currentMove, playerUnit.fighter.Movements[currentMove]);
    }

    public void HandleSelectionUp() => HandleSelection(1, true);

    public void HandleSelectionDown() => HandleSelection(-1, true);

    public void HandleSelectionLeft() => HandleSelection(-1, false);

    public void HandleSelectionRight() => HandleSelection(1, false);

    //Discriminate inputs between PLAYER_ACTION and PLAYER_MOVE
    public async void SelectAction()
    {
        if (state == BattleState.PLAYER_ACTION)
        {
            AudioManager.Instance.PlaySFX("AcceptButton");

            if (currentAction == 0)
                PlayerMove();
            else
            {
                var dialog = await GetDialog("BATTLE_RUN_AWAY");
                await dialogbox.SetDialog(dialog);
            }
        }
        else if (state == BattleState.PLAYER_MOVE)
        {
            if (!dialogbox.disabledMovements.Contains(dialogbox.moveTexts[currentMove]))
            {
                dialogbox.EnableMovementsSelector(false);
                dialogbox.EnableActionSelector(false);
                dialogbox.EnableDialogText(true);
                await PerformPlayerMove();
            }
        }
    }

    public void CancelAction()
    {
        dialogbox.EnableMovementsSelector(false);
        dialogbox.EnableDialogText(true);
        PlayerAction();
    }

    #endregion

}
