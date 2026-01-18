using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.XR.ARFoundation;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class DrivingSurfaceManager : MonoBehaviour
{
    // 1. THE TOOLKIT (Variables)
    public ARPlaneManager PlaneManager;     // Manages detected planes
    public ARRaycastManager RaycastManager; // Manages raycasting against detected planes
    public ReticleBehaviour Reticle;        // Reference to our reticle script
    public ARPlane LockedPlane;             // The plane we've locked onto to drive on

    [Header("Spawn Settings")]
    public GameObject PackagePrefab;        // Prefab for the packages to spawn
    public GameObject CarPrefab;             // Prefab for the car to spawn
    private GameObject _spawnedCar;          // Reference to the spawned car

    // 2. INPUT SETUP
        // WHEN Script Turns On:
        //     ENABLE Enhanced Touch (so we can detect screen taps)
        // WHEN Script Turns Off:
        //     DISABLE Enhanced Touch (cleanup)
    void OnEnable() => EnhancedTouchSupport.Enable();
    void OnDisable() => EnhancedTouchSupport.Disable();

    // Exit Application button
    public void ExitApplication()
    {
        Application.Quit();
    }

    /* Previous version of Update method doesn't handle mouse clicks" */

    // void Update()
    // {
    //     var activeTouches = Touch.activeTouches;

    //     if (activeTouches.Count > 0)
    //     {
    //         var primaryTouch = activeTouches[0];

    //         // Lock the plane on the first tap
    //         if (primaryTouch.began && LockedPlane == null && Reticle.CurrentPlane != null)
    //         {
    //             LockPlane(Reticle.CurrentPlane);
    //         }
    //     }
    // }

    // 3. THE MAIN LOOP (Waiting for "Start Game")
        // FUNCTION Update():
    void Update()  /* Updated version of Update method to include mouse click support */
{
    // A. Listen for Input___
    
    // 1. Get the current state of Touch and Mouse
    var activeTouches = Touch.activeTouches;
    
    // Check if screen was tapped (Touch)
    bool screenTapped = activeTouches.Count > 0 && activeTouches[0].began;
    
    // Check if left mouse button was clicked (PC/Editor)
    // We use ?. to ensure Mouse.current isn't null (prevents errors)
    bool mouseClicked = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;

    // 2. Combine them: If (Tap OR Click) AND we are pointing at a plane
    // IF (User Tapped OR Clicked) AND (LockedPlane is Empty) AND (Reticle Found a Plane):
    if ((screenTapped || mouseClicked) && LockedPlane == null && Reticle.CurrentPlane != null)
    {
        // We are ready to play!
        LockPlane(Reticle.CurrentPlane);
    }
}

    // 4. THE TRANSITION (Setting the Stage)
    private void LockPlane(ARPlane plane)
    {
        // A. Lock onto the selected plane
        LockedPlane = plane;

        // B. Hide all other planes and disable plane detection
        foreach (var p in PlaneManager.trackables)
        {
            if (p != LockedPlane) p.gameObject.SetActive(false);
        }

        // Disable further plane detection
        PlaneManager.enabled = false;

        // Spawn the first package where the reticle is
        SpawnNewPackage(Reticle.transform.position);

        // 1. Spawn the CAR at the Reticle's position
        if (CarPrefab != null && _spawnedCar == null) //IF we haven't spawned a car yet:
        {
            // CREATE (Instantiate) the Car at the Reticle's exact position
            _spawnedCar = Instantiate(CarPrefab, Reticle.transform.position, Quaternion.identity);

            // Link the car to the reticle so it knows where to go
            // This assumes your Car prefab has the CarController script on it
            _spawnedCar.GetComponent<CarController>().Reticle = this.Reticle;

            // FIND the 'CarController' script on the new car
            // TELL the CarController: "This is the Reticle you need to follow"
        }

        // 2. Spawn the FIRST PACKAGE
        SpawnNewPackage(Reticle.transform.position + (transform.forward * 0.2f));
    }

    // This function can be called by other scripts to spawn more packages
    public void SpawnNewPackage(Vector3 spawnPosition)
    {
        if (PackagePrefab != null)
        {
            Instantiate(PackagePrefab, spawnPosition, Quaternion.identity);
        }
    }
}