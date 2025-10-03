using ScheduleOneMods.Logging;
using UnityEngine;

namespace ScheduleOneMods.ThirdPersonCamera;

/// <summary>
/// Simplified camera controller that modifies the existing camera instead of creating a new one
/// </summary>
public static class CameraControllerV2
{
    private static Camera? _mainCamera;
    private static GameObject? _player;
    private static Transform? _originalCameraParent;
    private static Vector3 _originalCameraLocalPosition;
    private static Quaternion _originalCameraLocalRotation;
    private static bool _isThirdPersonEnabled;
    private static bool _isInitialized;
    private static float _lastToggleTime;
    private static readonly float ToggleCooldown = 0.5f;
    
    // Camera settings
    private static readonly Vector3 CameraOffset = new Vector3(0, 1.5f, -3.5f); // Behind and above player
    private static float _horizontalRotation;
    private static float _verticalRotation = 15f;

    public static void Update()
    {
        // Toggle with F6
        if (Input.GetKeyDown(KeyCode.F6) && Time.time - _lastToggleTime > ToggleCooldown)
        {
            _lastToggleTime = Time.time;
            ToggleThirdPerson();
        }
    }

    public static void LateUpdate()
    {
        if (_isThirdPersonEnabled && _mainCamera != null && _player != null)
        {
            UpdateCamera();
        }
    }

    private static void ToggleThirdPerson()
    {
        _isThirdPersonEnabled = !_isThirdPersonEnabled;
        
        if (_isThirdPersonEnabled)
        {
            EnableThirdPerson();
        }
        else
        {
            DisableThirdPerson();
        }
        
        Log.Info($"Third person mode: {(_isThirdPersonEnabled ? "ENABLED" : "DISABLED")}");
    }

    private static void EnableThirdPerson()
    {
        if (!_isInitialized)
        {
            Initialize();
        }

        if (_player == null || _mainCamera == null)
        {
            Log.Error("Cannot enable third person: Player or camera not found");
            _isThirdPersonEnabled = false;
            return;
        }

        // Store original camera setup
        _originalCameraParent = _mainCamera.transform.parent;
        _originalCameraLocalPosition = _mainCamera.transform.localPosition;
        _originalCameraLocalRotation = _mainCamera.transform.localRotation;
        
        // Initialize rotation to player's facing direction
        _horizontalRotation = _player.transform.eulerAngles.y;
        
        Log.Info($"Third person enabled - Player: {_player.name}");
    }

    private static void DisableThirdPerson()
    {
        if (_mainCamera != null && _originalCameraParent != null)
        {
            // Restore original camera setup
            _mainCamera.transform.parent = _originalCameraParent;
            _mainCamera.transform.localPosition = _originalCameraLocalPosition;
            _mainCamera.transform.localRotation = _originalCameraLocalRotation;
        }
    }

    private static void Initialize()
    {
        _mainCamera = Camera.main;
        if (_mainCamera == null)
        {
            Log.Error("Main camera not found");
            return;
        }

        _player = FindPlayer();
        if (_player == null)
        {
            Log.Error("Player not found");
            return;
        }

        Log.Info($"Initialized - Player: {_player.name}, Camera: {_mainCamera.name}");
        _isInitialized = true;
    }

    private static GameObject? FindPlayer()
    {
        var mainCam = Camera.main;
        if (mainCam == null) return null;

        // Find CharacterController (the actual player controller)
        var controllers = UnityEngine.Object.FindObjectsOfType<CharacterController>();
        foreach (var controller in controllers)
        {
            if (controller.gameObject.transform.position != Vector3.zero)
            {
                Log.Info($"Found player: {controller.gameObject.name} at {controller.gameObject.transform.position}");
                return controller.gameObject;
            }
        }

        return null;
    }

    private static void UpdateCamera()
    {
        if (_player == null || _mainCamera == null) return;

        // Handle rotation input
        if (Input.GetMouseButton(1))
        {
            _horizontalRotation += Input.GetAxis("Mouse X") * 3f;
            _verticalRotation -= Input.GetAxis("Mouse Y") * 3f;
            _verticalRotation = Mathf.Clamp(_verticalRotation, -10f, 60f);
        }

        // Calculate camera position
        var rotation = Quaternion.Euler(_verticalRotation, _horizontalRotation, 0);
        var offset = rotation * new Vector3(0, 0, -3.5f);
        var targetPosition = _player.transform.position + Vector3.up * 1.5f + offset;

        // Set camera position and rotation directly (no parenting, no smoothing)
        _mainCamera.transform.position = targetPosition;
        _mainCamera.transform.LookAt(_player.transform.position + Vector3.up * 1.5f);
    }
}
