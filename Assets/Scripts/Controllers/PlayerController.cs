using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;

/// <summary>
/// Controls the player's movement and interactions within the game world.
/// Inherits from the base <see cref="Character"/> class.
/// </summary>
public class PlayerController : Character
{
    //Public variables
    [SerializeField] private float interactionDistance = 0.2f; // Distance to detect interactable objects
    [SerializeField] private LayerMask foregroundLayer, interactableLayer, fighterLayer, portalLayer;

    // Private variables
    private Vector2 input;         // Stores player input direction
    private bool isInputActive;    // Indicates if player input is active

    // Delegates
    public delegate void OnEnterFighterView(Collider2D collider);
    public OnEnterFighterView onEnterFighterView; // Triggered when the player enters an enemy's view

    #region Public methods

    // Handles player movement input.
    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.started || context.performed)
        {
            // Prioritize horizontal movement if both axes are pressed
            input = context.ReadValue<Vector2>();
            if (input.x != 0) input.y = 0;
            isInputActive = true;

            if (!isMoving)
            {
                MoveContinuously().Forget();
            }
        }
        else if (context.canceled)
        {
            // Stop movement on input release
            isInputActive = false;
            input = Vector2.zero;
        }
    }

    // Handles interaction input and triggers interaction with objects in front of the player.
    public void OnAction()
    {
        var direction = new Vector3(animator.GetFloat("moveX"), animator.GetFloat("moveY"));
        var interactPos = transform.position + direction;

        var collider = Physics2D.OverlapCircle(interactPos, interactionDistance, interactableLayer);

        if (collider != null)
        {
            collider.GetComponent<IInteractable>()?.Interact();
        }
    }

    public override async UniTask Move(Vector3 targetPos)
    {
        await base.Move(targetPos);

        // Check if the player has entered an enemy's view
        IsInFighterView();

        // Check if the player has entered a portal
        IsInsidePortal();
    }

    #endregion

    #region Private methods

    // Continuously moves the player based on input while input is active.
    private async UniTask MoveContinuously()
    {
        while (isInputActive)
        {
            if (!isMoving && input != Vector2.zero)
            {
                animator.SetFloat("moveX", input.x);
                animator.SetFloat("moveY", input.y);

                Vector3 targetPos = transform.position + new Vector3(input.x, input.y, 0);
                if (CanMoveTo(targetPos))
                {
                    await Move(targetPos);
                }
            }
            await UniTask.Yield();
        }
    }  

    // Determines if the player can move to the specified position.
    private bool CanMoveTo(Vector3 targetPos)
    {
        return !Physics2D.OverlapCircle(targetPos, interactionDistance, foregroundLayer);
    }

    private void IsInFighterView()
    {
        var collider = Physics2D.OverlapCircle(transform.position, interactionDistance, fighterLayer);

        if (collider != null) 
        {
            onEnterFighterView?.Invoke(collider);
        }
    }

    private void IsInsidePortal()
    {
        var collider = Physics2D.OverlapCircle(transform.position, 0, portalLayer);

        if (collider != null)
        {
            if (collider.gameObject.TryGetComponent<Portal2D>(out Portal2D portal))
                portal.TPPlayer();
        }
    }

    #endregion
}
