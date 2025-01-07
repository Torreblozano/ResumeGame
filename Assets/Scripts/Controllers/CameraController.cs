using Cysharp.Threading.Tasks;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Public variables
    [SerializeField] private Transform target; // The player or object to follow
    [SerializeField] private float smoothSpeed = 0.125f; // Smoothing factor for camera movement
    [SerializeField] private Vector2 minBounds; // Minimum X and Y limits
    [SerializeField] private Vector2 maxBounds; // Maximum X and Y limits
    
    // Private variables
    private Vector3 offset; // Offset between camera and target

    private void Start()
    {
        if (target == null)
        {
            Debug.LogWarning("Target is not assigned to CameraController. Please assign a target.");
        }
        offset = new Vector3(0, 0, -10);
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = target.position + offset;

        float clampedX = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
        float clampedY = Mathf.Clamp(targetPosition.y, minBounds.y, maxBounds.y);
        Vector3 clampedPosition = new Vector3(clampedX, clampedY, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, clampedPosition, smoothSpeed);
    }

    /// <summary>
    /// Sets new bounds for the camera movement.
    /// </summary>
    /// <param name="newMinBounds">New minimum X and Y limits.</param>
    /// <param name="newMaxBounds">New maximum X and Y limits.</param>
    public void SetBounds(Vector2 newMinBounds, Vector2 newMaxBounds)
    {
        minBounds = newMinBounds;
        maxBounds = newMaxBounds;
    }

    /// <summary>
    /// Sets a new target for the camera to follow.
    /// </summary>
    /// <param name="newTarget">The new target Transform.</param>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        offset = new Vector3(0, 0, transform.position.z - target.position.z); // Recalculate offset
    }

    /// <summary>
    /// Configures the camera settings: sets culling mask to nothing, updates bounds, and checks if camera is in position.
    /// </summary>
    /// <param name="newMinBounds">New minimum bounds for the camera.</param>
    /// <param name="newMaxBounds">New maximum bounds for the camera.</param>
    /// <returns>True if the camera is in position with respect to the target, false otherwise.</returns>
    public async UniTask ConfigureCamera(Vector2 newMinBounds, Vector2 newMaxBounds)
    {
        SetBounds(newMinBounds, newMaxBounds);

        await UniTask.WaitUntil(() =>
        {
            // Calculate target position
            Vector3 targetPosition = target.position + offset;
            float clampedX = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
            float clampedY = Mathf.Clamp(targetPosition.y, minBounds.y, maxBounds.y);
            Vector3 clampedPosition = new Vector3(clampedX, clampedY, transform.position.z);

            // Return true if the camera is close enough to the clamped position
            return Vector3.Distance(transform.position, clampedPosition) < 0.1f;
        });
    }
}
