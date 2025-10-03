using ScheduleOneMods.Logging;
using UnityEngine;

namespace ScheduleOneMods.ThirdPersonCamera;

/// <summary>
/// Final camera controller that disables the game's camera scripts
/// </summary>
public static class CameraControllerFinal
{
    private static Camera? _mainCamera;
    private static GameObject? _player;
    private static MonoBehaviour[]? _disabledCameraScripts;
    private static Transform? _originalCameraParent;
    private static Vector3 _originalCameraLocalPosition;
    private static Quaternion _originalCameraLocalRotation;
    private static bool _isThirdPersonEnabled;
    private static bool _isInitialized;
    private static float _lastToggleTime;
    private static readonly float ToggleCooldown = 0.5f;
    
    // Camera settings
    private static float _horizontalRotation;
    private static float _verticalRotation = 15f;
    private static readonly float CameraDistance = 3.5f;
    private static readonly float CameraHeight = 1.5f;
    private static readonly float MouseSensitivity = 3f;

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
        
        // CRITICAL: Disable all MonoBehaviour scripts on the camera to stop game's camera control
        DisableGameCameraScripts();
        
        // Initialize rotation to player's facing direction
        _horizontalRotation = _player.transform.eulerAngles.y;
        
        Log.Info($"Third person enabled - Disabled {_disabledCameraScripts?.Length ?? 0} camera scripts");
    }

    private static void DisableThirdPerson()
    {
        // Re-enable game's camera scripts
        EnableGameCameraScripts();
        
        if (_mainCamera != null && _originalCameraParent != null)
        {
            // Restore original camera setup
            _mainCamera.transform.parent = _originalCameraParent;
            _mainCamera.transform.localPosition = _originalCameraLocalPosition;
            _mainCamera.transform.localRotation = _originalCameraLocalRotation;
        }
        
        Log.Info("Third person disabled - Camera scripts re-enabled");
    }

    private static void DisableGameCameraScripts()
    {
        if (_mainCamera == null) return;

        // Find all MonoBehaviour components on the camera and its parents
        var scripts = new System.Collections.Generic.List<MonoBehaviour>();
        
        // Check camera itself
        var cameraScripts = _mainCamera.GetComponents<MonoBehaviour>();
        foreach (var script in cameraScripts)
        {
            if (script != null && script.enabled && script.GetType().Name != "Camera")
            {
                scripts.Add(script);
                script.enabled = false;
                Log.Debug($"Disabled script on camera: {script.GetType().Name}");
            }
        }
        
        // Check camera parent (CameraContainer)
        if (_mainCamera.transform.parent != null)
        {
            var parentScripts = _mainCamera.transform.parent.GetComponents<MonoBehaviour>();
            foreach (var script in parentScripts)
            {
                if (script != null && script.enabled)
                {
                    scripts.Add(script);
                    script.enabled = false;
                    Log.Debug($"Disabled script on camera parent: {script.GetType().Name}");
                }
            }
            
            // Check grandparent if exists
            if (_mainCamera.transform.parent.parent != null)
            {
                var grandparentScripts = _mainCamera.transform.parent.parent.GetComponents<MonoBehaviour>();
                foreach (var script in grandparentScripts)
                {
                    if (script != null && script.enabled)
                    {
                        scripts.Add(script);
                        script.enabled = false;
                        Log.Debug($"Disabled script on camera grandparent: {script.GetType().Name}");
                    }
                }
            }
        }
        
        _disabledCameraScripts = scripts.ToArray();
        Log.Info($"Disabled {_disabledCameraScripts.Length} camera control scripts");
    }

    private static void EnableGameCameraScripts()
    {
        if (_disabledCameraScripts == null) return;

        foreach (var script in _disabledCameraScripts)
        {
            if (script != null)
            {
                script.enabled = true;
                Log.Debug($"Re-enabled script: {script.GetType().Name}");
            }
        }
        
        _disabledCameraScripts = null;
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

        Log.Error("No valid CharacterController found");
        return null;
    }

    private static void UpdateCamera()
    {
        if (_player == null || _mainCamera == null) return;

        // Handle rotation input
        if (Input.GetMouseButton(1))
        {
            _horizontalRotation += Input.GetAxis("Mouse X") * MouseSensitivity;
            _verticalRotation -= Input.GetAxis("Mouse Y") * MouseSensitivity;
            _verticalRotation = Mathf.Clamp(_verticalRotation, -10f, 60f);
        }

        // Calculate camera position
        var rotation = Quaternion.Euler(_verticalRotation, _horizontalRotation, 0);
        var offset = rotation * new Vector3(0, 0, -CameraDistance);
        var pivotPoint = _player.transform.position + Vector3.up * CameraHeight;
        var targetPosition = pivotPoint + offset;

        // Check for obstacles
        RaycastHit hit;
        if (Physics.Raycast(pivotPoint, offset.normalized, out hit, CameraDistance))
        {
            targetPosition = hit.point - offset.normalized * 0.3f;
        }

        // Set camera position and rotation directly
        _mainCamera.transform.position = targetPosition;
        _mainCamera.transform.LookAt(pivotPoint);
    }
}
