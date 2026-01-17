using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ReticleBehaviour : MonoBehaviour
{
    public GameObject Child; // The visual circle/disk
    public DrivingSurfaceManager SurfaceManager;
    public ARPlane CurrentPlane;

    void Update()
    {
        var screenCenter = Camera.main.ViewportToScreenPoint(new Vector2(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();

        SurfaceManager.RaycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinBounds);

        CurrentPlane = null;
        ARRaycastHit? hit = null;

        if (hits.Count > 0)
        {
            var lockedPlane = SurfaceManager.LockedPlane;
            hit = lockedPlane == null ? hits[0] : hits.Find(x => x.trackableId == lockedPlane.trackableId);
        }

        if (hit.HasValue)
        {
            CurrentPlane = SurfaceManager.PlaneManager.GetPlane(hit.Value.trackableId);
            transform.position = hit.Value.pose.position;
        }

        // Only show the reticle if we are hitting a valid plane
        Child.SetActive(CurrentPlane != null);
    }
}