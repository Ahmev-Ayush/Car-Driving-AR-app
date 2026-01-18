// 0. USING DIRECTIVES
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ReticleBehaviour : MonoBehaviour
{
    // 1. COMPONENTS & DATA
    public GameObject Child; // The visual circle/disk
    public DrivingSurfaceManager SurfaceManager; // Reference to our surface manager
    public ARPlane CurrentPlane; // The currently detected plane

    void Update()
    {
        // 1. PROJECTING FROM SCREEN CENTER TO REAL WORLD
            // DEFINE screenCenter = Middle point of the camera's view (X:0.5, Y:0.5)
        var screenCenter = Camera.main.ViewportToScreenPoint(new Vector2(0.5f, 0.5f));

            // CREATE an empty list called 'hits' to store what we find
        var hits = new List<ARRaycastHit>();

        // 2. SEARCHING FOR PHYSICAL SURFACES (happening every frame)
            // TELL the RaycastManager: "Shoot a ray from screenCenter and find any Planes"
            // STORE results in 'hits'
        SurfaceManager.RaycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinBounds);

        // 3. FILTERING THE RESULTS
            // RESET CurrentPlane to nothing (null)
        CurrentPlane = null;
        ARRaycastHit? hit = null;

        // IF 'hits' list is NOT empty:
        if (hits.Count > 0)
        {
            // GET 'lockedPlane' from the SurfaceManager
            var lockedPlane = SurfaceManager.LockedPlane;

        //    IF no plane is locked yet:
        //      CHOOSE the very first hit in the list
        //    ELSE:
        //      CHOOSE only the hit that matches our 'lockedPlane' ID
            hit = lockedPlane == null ? hits[0] : hits.Find(x => x.trackableId == lockedPlane.trackableId);
        }

        // 4. APPLYING THE HIT DATA
            // IF we found a valid hit (after filtering):
            //     UPDATE CurrentPlane = Find the specific Plane object using the hit ID
            //     SET this object's Position = The exact 3D coordinates of the hit
        if (hit.HasValue)
        {
            CurrentPlane = SurfaceManager.PlaneManager.GetPlane(hit.Value.trackableId);
            transform.position = hit.Value.pose.position;
        }

        // 5. MANAGING VISUALS
            // Only show the reticle if we are hitting a valid plane
        Child.SetActive(CurrentPlane != null);
        // IF CurrentPlane exists:
        //     SHOW the reticle visual (Child)
        // ELSE:
        //     HIDE the reticle visual (Child)
    }
}