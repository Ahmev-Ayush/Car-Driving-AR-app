using UnityEngine;

public class Package : MonoBehaviour
{
    private DrivingSurfaceManager _manager;

    void Start()
    {
        // Find the manager in the scene so we can tell it to spawn the next one
        _manager = FindFirstObjectByType<DrivingSurfaceManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the car (tagged Player) hit the package
        if (other.CompareTag("Player"))
        {
            // Spawn next package at a random spot on the locked plane
            if (_manager != null && _manager.LockedPlane != null)
            {
                Vector3 randomPos = GetRandomPositionOnPlane(_manager.LockedPlane);
                _manager.SpawnNewPackage(randomPos);
            }

            Destroy(gameObject);
        }
    }

    // Helper to pick a random spot on the detected floor
    Vector3 GetRandomPositionOnPlane(UnityEngine.XR.ARFoundation.ARPlane plane)
    {
        float x = Random.Range(-plane.extents.x, plane.extents.x);
        float z = Random.Range(-plane.extents.y, plane.extents.y);
        return plane.center + (plane.transform.right * x) + (plane.transform.forward * z);
    }
}