using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class CarController : MonoBehaviour
{
    public ReticleBehaviour Reticle;    // Reference to the reticle to follow
    public float Speed = 0.5f;          // Movement speed
    public float RotationSpeed = 2.0f;  // Rotation speed

        // while using SmoothDamp (optional) (for tank like rotation code)___________
    // private Vector3 _velocity = Vector3.zero; // Add this at the top with your variables
    // public float SmoothTime = 0.3f;          // The time it takes to reach the target
        // ____________________________________________

    [Header("Driving Physics")]
    public float MaxSpeed = 1.0f;        // Your original Speed variable
    public float Acceleration = 0.5f;    // How fast it gains speed
    public float BrakingSpeed = 1.0f;    // How fast it brakes
    private float _currentSpeed = 0f;    // The actual speed this frame

    // 1. INPUT SETUP
    // WHEN Script Turns On:
    //         ENABLE Enhanced Touch (so we can detect screen taps)
    // WHEN Script Turns Off:
    //         DISABLE Enhanced Touch (cleanup)
    void OnEnable() => EnhancedTouchSupport.Enable();
    void OnDisable() => EnhancedTouchSupport.Disable();


/*
    // 2. THE MAIN LOOP (Moving the Car / Driving Logic)
    void Update()
    {
        // 1. Don't move if not touching or if no plane is detected
        if (Touch.activeTouches.Count == 0 || Reticle.CurrentPlane == null)
            return;

        // Target Acquisition _______________________
        // 2. Get the target position from the reticle 
        Vector3 targetPos = Reticle.transform.position; // Reticle's position

        // --- THE FIX: SNAP TO GROUND ---

        // GET the Car's current 3D position as 'currentPos'
        Vector3 currentPos = transform.position;

        // Force the car to stay at the exact height (Y) of the reticle/plane
        // This prevents the "floating" effect (no gravity) and ensures it drives on the surface
        targetPos.y = Reticle.transform.position.y;  // UPDATE 'targetPos.y' = Reticle's Y position
        currentPos.y = Reticle.transform.position.y; // UPDATE 'currentPos.y' = Reticle's Y position
        // MOVE the Car instantly to reticle height (prevents floating)
        transform.position = currentPos;

        // Translation (Moving) _______________________

            // Note : Using Lerp instead of MoveTowards for smoother movement

        // 3. Move the car forward
        // transform.position = Vector3.MoveTowards(transform.position, targetPos, Speed * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, targetPos, Speed * Time.deltaTime);
        // transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref _velocity, SmoothTime, Speed);

        // Rotation (Turning) _______________________
        // 4. Rotate to face the direction of travel
        Vector3 direction = (targetPos - transform.position).normalized;
            //  direction = (reticle position - car position).normalized;

        // Gives Tank like turning behavior (rotation on its own axis)
        if (direction != Vector3.zero)
        {
            direction.y = 0; // Ensure the car doesn't tilt up/down
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * RotationSpeed);
        }
    }
*/  
    // realistic car steering and movement
    //  THE MAIN LOOP (Moving the Car / Driving Logic)
// 2. THE MAIN LOOP
    void Update()
    {
        // A. Safety Check: If no plane detected, do nothing
        if (Reticle.CurrentPlane == null)
        {
            StopCarInstantly();
            return;
        }

        // B. Target Acquisition & Height Correction
        Vector3 targetPos = Reticle.transform.position;
        Vector3 currentPos = transform.position;

        // Force the car and target to the same height (Y) to prevent floating/diving
        targetPos.y = Reticle.transform.position.y;
        currentPos.y = Reticle.transform.position.y;
        transform.position = currentPos;

        // C. Calculate Distance and Direction
        float distanceToTarget = Vector3.Distance(transform.position, targetPos);
        Vector3 direction = (targetPos - transform.position).normalized; // unit vector
        direction.y = 0; // Prevent tilting of car

        // D. ACCELERATION LOGIC
        // Only move if the user is touching the screen AND the car isn't already at the target
        if ((Touch.activeTouches.Count > 0 || (Mouse.current != null && Mouse.current.leftButton.isPressed)) && (distanceToTarget > 0.1f ))
        {
            // Gradually increase speed toward MaxSpeed
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, MaxSpeed, Acceleration * Time.deltaTime);
        }
        else
        {
            // Gradually decrease speed to 0 (Smooth Braking)
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, 0, BrakingSpeed * Time.deltaTime);
        }

        // E. STEERING (Rotation)
        // Only steer if the car is actually moving to avoid "shaking" at rest
        if (direction != Vector3.zero && _currentSpeed > 0.01f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            // Higher speed = slightly faster steering response
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * RotationSpeed);
        }

        // F. DRIVING (Translation)
        // Real car feel: Move along the "Forward" axis only
        if (_currentSpeed > 0)
        {
            transform.Translate(Vector3.forward * _currentSpeed * Time.deltaTime);
        }
    }

    private void StopCarInstantly()
    {
        _currentSpeed = 0;
    }
}