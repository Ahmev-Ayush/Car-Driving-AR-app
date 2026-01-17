using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class CarController : MonoBehaviour
{
    public ReticleBehaviour Reticle;
    public float Speed = 0.5f;
    public float RotationSpeed = 10.0f;

    void OnEnable() => EnhancedTouchSupport.Enable();
    void OnDisable() => EnhancedTouchSupport.Disable();

    void Update()
    {
        // 1. Don't move if not touching or if no plane is detected
        if (Touch.activeTouches.Count == 0 || Reticle.CurrentPlane == null)
            return;

        // 2. Get the target position from the reticle
        Vector3 targetPos = Reticle.transform.position;

        // --- THE FIX: SNAP TO GROUND ---
        // Force the car to stay at the exact height (Y) of the reticle/plane
        // This prevents the "floating" effect
        Vector3 currentPos = transform.position;
        targetPos.y = Reticle.transform.position.y; 
        currentPos.y = Reticle.transform.position.y;
        transform.position = currentPos;

        // 3. Move the car forward
        transform.position = Vector3.MoveTowards(transform.position, targetPos, Speed * Time.deltaTime);

        // 4. Rotate to face the direction of travel
        Vector3 direction = (targetPos - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            direction.y = 0; // Ensure the car doesn't tilt up/down
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * RotationSpeed);
        }
    }
}