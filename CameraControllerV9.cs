using ScheduleOneMods.Logging;
using UnityEngine;
using System.Reflection;

namespace ScheduleOneMods.ThirdPersonCamera;

/// <summary>
/// Hijacks Schedule 1's existing avatar view (V key) system
/// </summary>
public static class CameraControllerV9
{
    private static bool _isThirdPersonEnabled;
    private static float _lastToggleTime;
    private static readonly float ToggleCooldown = 0.5f;
    private static bool _isHoldingV;

    public static void Update()
    {
        // Toggle with F6
        if (Input.GetKeyDown(KeyCode.F6) && Time.time - _lastToggleTime > ToggleCooldown)
        {
            _lastToggleTime = Time.time;
            ToggleThirdPerson();
        }

        // Simulate holding V key when third person is enabled
        if (_isThirdPersonEnabled)
        {
            SimulateVKeyHold();
        }
    }

    private static void ToggleThirdPerson()
    {
        _isThirdPersonEnabled = !_isThirdPersonEnabled;
        Log.Info($"Third person mode: {(_isThirdPersonEnabled ? "ENABLED" : "DISABLED")}");
        
        if (_isThirdPersonEnabled)
        {
            Log.Info("Activating avatar view (simulating V key hold)");
        }
        else
        {
            Log.Info("Deactivating avatar view");
        }
    }

    private static void SimulateVKeyHold()
    {
        // This simulates the V key being held down
        // Schedule 1 checks Input.GetKey(KeyCode.V) to show avatar view
        // We need to make the game think V is being held
        
        // Note: We can't directly fake Input.GetKey, but we can try to:
        // 1. Find the avatar view camera and keep it enabled
        // 2. Or find the script that handles V key and call its methods directly
        
        // For now, let's try to find and manipulate the avatar camera
        if (!_isHoldingV)
        {
            _isHoldingV = true;
            TryActivateAvatarView();
        }
    }

    private static void TryActivateAvatarView()
    {
        // Try to find the avatar view camera
        var allCameras = UnityEngine.Object.FindObjectsOfType<Camera>();
        
        foreach (var cam in allCameras)
        {
            // Look for a camera that might be the avatar view camera
            // It's probably disabled by default and enabled when V is pressed
            if (cam.name.ToLower().Contains("avatar") || 
                cam.name.ToLower().Contains("third") ||
                cam.name.ToLower().Contains("character") ||
                cam.gameObject.name.ToLower().Contains("view"))
            {
                if (!cam.enabled || !cam.gameObject.activeInHierarchy)
                {
                    cam.enabled = true;
                    cam.gameObject.SetActive(true);
                    Log.Info($"Activated camera: {cam.name}");
                }
            }
        }
        
        // Also try to find any disabled cameras that might be the avatar view
        var allGameObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (var obj in allGameObjects)
        {
            if (!obj.activeInHierarchy && 
                (obj.name.ToLower().Contains("avatar") || 
                 obj.name.ToLower().Contains("view") ||
                 obj.name.ToLower().Contains("third")))
            {
                var cam = obj.GetComponent<Camera>();
                if (cam != null)
                {
                    obj.SetActive(true);
                    cam.enabled = true;
                    Log.Info($"Activated hidden camera object: {obj.name}");
                }
            }
        }
    }
}
