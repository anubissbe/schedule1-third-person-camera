using ScheduleOneMods.Logging;
using UnityEngine;

namespace ScheduleOneMods.ThirdPersonCamera;

/// <summary>
/// Manages the third-person camera system
/// </summary>
public static class CameraController
{
    private static Camera? _mainCamera;
    private static Camera? _thirdPersonCamera;
    private static GameObject? _player;
    private static bool _isThirdPersonEnabled;
    
    // Camera configuration
    private static readonly CameraConfig Config = new()
    {
        Distance = 4f,          // Slightly closer for better view
        Height = 1.8f,          // Adjusted height
        Sensitivity = 3f,       // Increased sensitivity for better control
        MinVerticalAngle = -30f,
        MaxVerticalAngle = 70f,
        SmoothSpeed = 15f,      // Faster smoothing to reduce lag
        CollisionRadius = 0.3f
    };
    
    // Camera rotation state
    private static float _currentHorizontalAngle;
    private static float _currentVerticalAngle = 10f; // Lower angle for behind-the-back view
    private static bool _isInitialized;
    private static float _lastToggleTime;
    private static readonly float ToggleCooldown = 0.5f; // Half second cooldown

    public static void Update()
    {
        // Toggle third-person mode with F6 (with cooldown to prevent rapid toggling)
        if (Input.GetKeyDown(KeyCode.F6) && Time.time - _lastToggleTime > ToggleCooldown)
        {
            _lastToggleTime = Time.time;
            ToggleThirdPerson();
        }
    }

    public static void LateUpdate()
    {
        // Only update camera in LateUpdate to avoid double updates
        if (_isThirdPersonEnabled && _thirdPersonCamera != null && _player != null)
        {
            HandleCameraRotation();
            UpdateCameraPosition();
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

        // Try to find player again if not found during initialization
        if (_player == null)
        {
            _player = FindPlayer();
        }

        if (_player == null)
        {
            Log.Error("Cannot enable third person: Player not found");
            Log.Error("Please check MelonLoader console for available GameObjects");
            _isThirdPersonEnabled = false;
            return;
        }

        Log.Info($"Player found: {_player.name} at position {_player.transform.position}");

        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
            if (_mainCamera == null)
            {
                Log.Error("Cannot enable third person: Main camera not found");
                _isThirdPersonEnabled = false;
                return;
            }
        }

        // Create third-person camera if it doesn't exist
        if (_thirdPersonCamera == null)
        {
            CreateThirdPersonCamera();
        }

        // Initialize camera rotation based on player's current rotation
        _currentHorizontalAngle = _player.transform.eulerAngles.y;
        
        // Position camera immediately to avoid flickering
        var rotation = Quaternion.Euler(_currentVerticalAngle, _currentHorizontalAngle, 0);
        var direction = rotation * Vector3.back;
        var playerPosition = _player.transform.position;
        var pivotPoint = playerPosition + Vector3.up * Config.Height;
        _thirdPersonCamera!.transform.position = pivotPoint + direction * Config.Distance;
        _thirdPersonCamera.transform.LookAt(pivotPoint);

        // Switch cameras
        _mainCamera.enabled = false;
        _thirdPersonCamera.enabled = true;
        
        Log.Info("Third person camera enabled successfully");
        Log.Info($"Camera position: {_thirdPersonCamera.transform.position}");
    }

    private static void DisableThirdPerson()
    {
        if (_mainCamera != null)
        {
            _mainCamera.enabled = true;
        }
        
        if (_thirdPersonCamera != null)
        {
            _thirdPersonCamera.enabled = false;
        }
        
        Log.Debug("Third person camera disabled");
    }

    private static void Initialize()
    {
        Log.Debug("Initializing camera controller...");
        
        // Find the player
        _player = FindPlayer();
        if (_player == null)
        {
            Log.Warning("Player not found during initialization. Will retry when toggling.");
        }
        else
        {
            Log.Debug($"Player found: {_player.name}");
        }

        // Find the main camera
        _mainCamera = Camera.main;
        if (_mainCamera == null)
        {
            Log.Warning("Main camera not found during initialization");
        }
        else
        {
            Log.Debug($"Main camera found: {_mainCamera.name}");
        }

        _isInitialized = true;
    }

    private static GameObject? FindPlayer()
    {
        Log.Debug("Searching for player object...");
        
        // PRIORITY METHOD: Find the main camera's parent, then check for CharacterController
        var mainCam = Camera.main;
        if (mainCam != null)
        {
            // First, try to find CharacterController in the camera's hierarchy
            var cameraParent = mainCam.transform.parent;
            if (cameraParent != null)
            {
                // Check if parent has CharacterController
                var controller = cameraParent.GetComponent<CharacterController>();
                if (controller != null && cameraParent.position != Vector3.zero)
                {
                    Log.Info($"Found player with CharacterController on camera parent: {cameraParent.name} at {cameraParent.position}");
                    return cameraParent.gameObject;
                }
                
                // Check if grandparent has CharacterController
                if (cameraParent.parent != null)
                {
                    controller = cameraParent.parent.GetComponent<CharacterController>();
                    if (controller != null && cameraParent.parent.position != Vector3.zero)
                    {
                        Log.Info($"Found player with CharacterController on camera grandparent: {cameraParent.parent.name} at {cameraParent.parent.position}");
                        return cameraParent.parent.gameObject;
                    }
                }
                
                // If camera parent is just a container (like "CameraContainer"), look for sibling with CharacterController
                if (cameraParent.parent != null)
                {
                    foreach (Transform sibling in cameraParent.parent)
                    {
                        controller = sibling.GetComponent<CharacterController>();
                        if (controller != null && sibling.position != Vector3.zero)
                        {
                            Log.Info($"Found player with CharacterController as sibling: {sibling.name} at {sibling.position}");
                            return sibling.gameObject;
                        }
                    }
                }
            }
        }
        
        // Method 1: Find by tag (but skip if at origin)
        try
        {
            var player = GameObject.FindWithTag("Player");
            if (player != null && player.transform.position != Vector3.zero)
            {
                Log.Info($"Found player by tag 'Player': {player.name} at {player.transform.position}");
                return player;
            }
        }
        catch
        {
            Log.Debug("Tag 'Player' does not exist");
        }

        // Method 2: Look for CharacterController component (but skip if at origin)
        var characterControllers = UnityEngine.Object.FindObjectsOfType<CharacterController>();
        foreach (var controller in characterControllers)
        {
            if (controller.gameObject.transform.position != Vector3.zero)
            {
                Log.Info($"Found player via CharacterController: {controller.gameObject.name} at {controller.gameObject.transform.position}");
                return controller.gameObject;
            }
        }

        // Method 3: Search for common player-related names (skip if at origin)
        string[] possibleNames = { 
            "Player", "PlayerController", "PlayerCharacter", "FPSController", 
            "Character", "LocalPlayer", "FirstPersonController",
            "Player(Clone)", "Character(Clone)", "PlayerObject", "MainPlayer"
        };
        
        foreach (var name in possibleNames)
        {
            var obj = GameObject.Find(name);
            if (obj != null && obj.transform.position != Vector3.zero)
            {
                Log.Info($"Found player by name '{name}' at {obj.transform.position}");
                return obj;
            }
        }

        // Method 4: Find objects near the camera
        if (mainCam != null)
        {
            var nearbyObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
            foreach (var obj in nearbyObjects)
            {
                var name = obj.name.ToLower();
                var distance = Vector3.Distance(obj.transform.position, mainCam.transform.position);
                
                if ((name.Contains("player") || name.Contains("character")) && 
                    distance < 5f && 
                    obj.transform.position != Vector3.zero)
                {
                    Log.Info($"Found player near camera: {obj.name} at {obj.transform.position}");
                    return obj;
                }
            }
        }

        // Last resort: List all GameObjects for debugging
        Log.Warning("Could not find valid player object. Listing camera hierarchy and potential candidates:");
        
        // Show camera hierarchy for debugging
        if (mainCam != null)
        {
            Log.Warning("Camera hierarchy:");
            var current = mainCam.transform;
            var depth = 0;
            while (current != null && depth < 5)
            {
                var indent = new string(' ', depth * 2);
                var hasController = current.GetComponent<CharacterController>() != null ? " [HAS CharacterController]" : "";
                Log.Warning($"{indent}- {current.name} at {current.position}{hasController}");
                current = current.parent;
                depth++;
            }
        }
        
        var allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        var candidates = new System.Collections.Generic.List<string>();
        
        foreach (var obj in allObjects)
        {
            var name = obj.name.ToLower();
            if (name.Contains("player") || name.Contains("character") || 
                name.Contains("controller") || name.Contains("fps"))
            {
                var posStr = obj.transform.position == Vector3.zero ? "(ORIGIN - SKIP)" : obj.transform.position.ToString();
                candidates.Add($"{obj.name} at {posStr}");
            }
        }
        
        if (candidates.Count > 0)
        {
            Log.Warning("Potential player objects found:");
            foreach (var candidate in candidates)
            {
                Log.Warning($"  - {candidate}");
            }
        }
        else
        {
            Log.Error("No potential player objects found at all!");
        }

        return null;
    }

    private static void CreateThirdPersonCamera()
    {
        Log.Debug("Creating third person camera...");
        
        var cameraObject = new GameObject("ThirdPersonCamera");
        _thirdPersonCamera = cameraObject.AddComponent<Camera>();
        _thirdPersonCamera.gameObject.tag = "MainCamera";
        
        // Make camera persist across scene loads
        UnityEngine.Object.DontDestroyOnLoad(cameraObject);
        
        // Copy settings from main camera
        if (_mainCamera != null)
        {
            _thirdPersonCamera.fieldOfView = _mainCamera.fieldOfView;
            _thirdPersonCamera.nearClipPlane = _mainCamera.nearClipPlane;
            _thirdPersonCamera.farClipPlane = _mainCamera.farClipPlane;
            _thirdPersonCamera.clearFlags = _mainCamera.clearFlags;
            _thirdPersonCamera.backgroundColor = _mainCamera.backgroundColor;
        }
        
        Log.Debug("Third person camera created successfully");
    }

    private static void HandleCameraRotation()
    {
        // Rotate camera when right mouse button is held
        if (Input.GetMouseButton(1))
        {
            _currentHorizontalAngle += Input.GetAxis("Mouse X") * Config.Sensitivity;
            _currentVerticalAngle -= Input.GetAxis("Mouse Y") * Config.Sensitivity;
            
            // Clamp vertical angle
            _currentVerticalAngle = Mathf.Clamp(_currentVerticalAngle, Config.MinVerticalAngle, Config.MaxVerticalAngle);
        }
        // When not holding right-click, keep camera angle fixed (don't auto-follow player rotation)
    }

    private static void UpdateCameraPosition()
    {
        if (_player == null || _thirdPersonCamera == null) return;

        // Verify player is still valid
        if (_player.transform == null)
        {
            Log.Error("Player transform is null, disabling third person");
            _isThirdPersonEnabled = false;
            DisableThirdPerson();
            return;
        }

        // Calculate desired camera position
        var rotation = Quaternion.Euler(_currentVerticalAngle, _currentHorizontalAngle, 0);
        var direction = rotation * Vector3.back;
        
        var playerPosition = _player.transform.position;
        
        // Safety check: if player is at origin or invalid position, something is wrong
        if (playerPosition == Vector3.zero)
        {
            Log.Warning("Player is at origin (0,0,0), this might be incorrect");
        }
        
        var pivotPoint = playerPosition + Vector3.up * Config.Height;
        var desiredPosition = pivotPoint + direction * Config.Distance;

        // Check for collisions
        var finalPosition = HandleCameraCollision(pivotPoint, desiredPosition, direction);

        // Direct positioning (no interpolation to avoid shake)
        _thirdPersonCamera.transform.position = finalPosition;

        // Make camera look at the pivot point
        _thirdPersonCamera.transform.LookAt(pivotPoint);
    }

    private static Vector3 HandleCameraCollision(Vector3 from, Vector3 to, Vector3 direction)
    {
        var distance = Vector3.Distance(from, to);
        
        // Perform a sphere cast to check for obstacles
        if (Physics.SphereCast(from, Config.CollisionRadius, direction, out var hit, distance))
        {
            // Position camera just before the hit point
            return hit.point - direction * Config.CollisionRadius;
        }

        return to;
    }

    private sealed class CameraConfig
    {
        public float Distance { get; init; }
        public float Height { get; init; }
        public float Sensitivity { get; init; }
        public float MinVerticalAngle { get; init; }
        public float MaxVerticalAngle { get; init; }
        public float SmoothSpeed { get; init; }
        public float CollisionRadius { get; init; }
    }
}
