using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class LightEstimation : MonoBehaviour
{
    public ARCameraManager CameraManager;
    public Light LightComponent;

    void OnEnable() => CameraManager.frameReceived += OnCameraFrameReceived;
    void OnDisable() => CameraManager.frameReceived -= OnCameraFrameReceived;

    void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
    {
        if (eventArgs.lightEstimation.averageBrightness.HasValue)
            LightComponent.intensity = eventArgs.lightEstimation.averageBrightness.Value;
        
        if (eventArgs.lightEstimation.averageColorTemperature.HasValue)
            LightComponent.colorTemperature = eventArgs.lightEstimation.averageColorTemperature.Value;
    }
}