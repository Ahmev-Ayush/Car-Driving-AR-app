using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using UnityEngine.SceneManagement;

public class DrivingSurfaceManager : MonoBehaviour
{
    public ARPlaneManager PlaneManager;
    public ARRaycastManager RaycastManager;
    public ReticleBehaviour Reticle;
    public ARPlane LockedPlane;

    [Header("Spawn Settings")]
    public GameObject PackagePrefab;
    public GameObject CarPrefab; // Add this new variable!
    private GameObject _spawnedCar;

    void OnEnable() => EnhancedTouchSupport.Enable();
    void OnDisable() => EnhancedTouchSupport.Disable();

    public void ExitApplication()
    {
        Application.Quit();
    }

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

    void Update()
{
    // 1. Get the current state of Touch and Mouse
    var activeTouches = Touch.activeTouches;
    
    // Check if screen was tapped (Touch)
    bool screenTapped = activeTouches.Count > 0 && activeTouches[0].began;
    
    // Check if left mouse button was clicked (PC/Editor)
    // We use ?. to ensure Mouse.current isn't null (prevents errors)
    bool mouseClicked = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;

    // 2. Combine them: If (Tap OR Click) AND we are pointing at a plane
    if ((screenTapped || mouseClicked) && LockedPlane == null && Reticle.CurrentPlane != null)
    {
        LockPlane(Reticle.CurrentPlane);
    }
}

    private void LockPlane(ARPlane plane)
    {
        LockedPlane = plane;
        foreach (var p in PlaneManager.trackables)
        {
            if (p != LockedPlane) p.gameObject.SetActive(false);
        }
        PlaneManager.enabled = false;

        // Spawn the first package where the reticle is
        // SpawnNewPackage(Reticle.transform.position);

        // 1. Spawn the CAR at the Reticle's position
        if (CarPrefab != null && _spawnedCar == null)
        {
            _spawnedCar = Instantiate(CarPrefab, Reticle.transform.position, Quaternion.identity);

            // Link the car to the reticle so it knows where to go
            // This assumes your Car prefab has the CarController script on it
            _spawnedCar.GetComponent<CarController>().Reticle = this.Reticle;
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