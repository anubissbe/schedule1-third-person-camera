using ScheduleOneMods.Logging;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace ScheduleOneMods.ThirdPersonCamera;

/// <summary>
/// Finds and hijacks Schedule 1's avatar view camera
/// </summary>
public static class AvatarCameraHijack
{
    /// <summary>The hijacked avatar camera (OverlayCamera) used for third-person rendering</summary>
    private static Camera? _avatarCamera;

    /// <summary>The game's main first-person camera (must stay enabled for player input)</summary>
    private static Camera? _mainCamera;

    /// <summary>Reference to the player GameObject (AnubISS with CharacterController)</summary>
    private static GameObject? _player;

    /// <summary>Whether third-person mode is currently active</summary>
    private static bool _isThirdPersonEnabled;

    /// <summary>Whether the mod is actively searching for the avatar camera (F8 debug mode)</summary>
    private static bool _isSearchingForCamera;

    /// <summary>Time of last F6 toggle press (for cooldown)</summary>
    private static float _lastToggleTime;

    /// <summary>Current horizontal camera rotation angle in degrees</summary>
    private static float _horizontalAngle;

    /// <summary>Current vertical camera rotation angle in degrees (clamped -10 to 60)</summary>
    private static float _verticalAngle = 10f;

    /// <summary>Cooldown between F6 toggle presses in seconds</summary>
    private static readonly float ToggleCooldown = 0.5f;

    /// <summary>Distance of camera from player pivot point in Unity units</summary>
    private static readonly float CameraDistance = 3.5f;

    /// <summary>Mouse sensitivity multiplier for camera rotation</summary>
    private static readonly float MouseSensitivity = 3f;

    /// <summary>
    /// Called every frame by Mod.OnUpdate(). Handles input detection for F6 toggle and F8 debug mode.
    /// </summary>
    public static void Update()
    {
        // F6 to toggle third-person mode (with cooldown to prevent rapid toggling)
        if (Input.GetKeyDown(KeyCode.F6) && Time.time - _lastToggleTime > ToggleCooldown)
        {
            _lastToggleTime = Time.time;
            ToggleThirdPerson();
        }

        // F8 to manually search for avatar camera (debug mode)
        if (Input.GetKeyDown(KeyCode.F8))
        {
            Log.Info("Manual camera search triggered - Press V now!");
            _isSearchingForCamera = true;
        }

        // If searching, continuously scan for newly enabled cameras
        if (_isSearchingForCamera)
        {
            SearchForAvatarCamera();
        }
    }

    /// <summary>
    /// Called every frame after all Update() calls by Mod.OnLateUpdate().
    /// Updates camera position and rotation when third-person is active.
    /// Uses LateUpdate to run after game's camera controller to minimize conflicts.
    /// </summary>
    public static void LateUpdate()
    {
        if (_isThirdPersonEnabled && _avatarCamera != null && _player != null)
        {
            UpdateAvatarCamera();
        }
    }

    /// <summary>
    /// Toggles third-person mode on/off via F6 key.
    /// Handles state management and camera switching.
    /// </summary>
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

    /// <summary>
    /// Activates third-person mode by finding player/cameras and configuring camera depth.
    /// CRITICAL: Does NOT disable main camera due to camera-input coupling blocker.
    /// Main camera must remain enabled or player movement stops working.
    /// </summary>
    private static void EnableThirdPerson()
    {
        // Find main camera and player
        _mainCamera = Camera.main;
        _player = FindPlayer();

        if (_player == null)
        {
            Log.Error("Player not found!");
            _isThirdPersonEnabled = false;
            return;
        }

        // Try to find the avatar camera (OverlayCamera)
        _avatarCamera = FindAvatarCamera();

        if (_avatarCamera == null)
        {
            Log.Warning("Avatar camera not found yet!");
            Log.Info("Press V to show avatar, then press F6 again");
            Log.Info("Or press F8, then press V to help find the camera");
            _isThirdPersonEnabled = false;
            return;
        }

        // Enable avatar camera and set as primary rendering camera
        _avatarCamera.enabled = true;
        _avatarCamera.gameObject.SetActive(true);
        _avatarCamera.depth = 10; // Higher depth = renders on top

        // Keep main camera enabled but deprioritize rendering
        if (_mainCamera != null)
        {
            _mainCamera.depth = -1; // Lower depth = renders first (avatar camera overlays)
            // CRITICAL: DO NOT disable main camera - breaks player input (Blocker #1)
        }

        // Initialize camera angle behind player
        _horizontalAngle = _player.transform.eulerAngles.y + 180f;

        Log.Info($"Third person enabled with avatar camera: {_avatarCamera.name}");
    }

    /// <summary>
    /// Deactivates third-person mode and restores first-person camera configuration.
    /// </summary>
    private static void DisableThirdPerson()
    {
        // Restore main camera depth to default
        if (_mainCamera != null)
        {
            _mainCamera.depth = 0;
        }

        // Disable and deactivate avatar camera
        if (_avatarCamera != null)
        {
            _avatarCamera.enabled = false;
            _avatarCamera.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Searches for Schedule 1's avatar camera (OverlayCamera) among all scene cameras.
    /// Prioritizes cameras with keywords in name, falls back to first non-main camera.
    /// </summary>
    /// <returns>Avatar camera if found, null otherwise</returns>
    private static Camera? FindAvatarCamera()
    {
        var allCameras = UnityEngine.Object.FindObjectsOfType<Camera>(true); // Include inactive

        Log.Info($"Searching through {allCameras.Length} cameras...");

        // Look for cameras that aren't the main camera
        foreach (var cam in allCameras)
        {
            if (cam == _mainCamera) continue;

            Log.Debug($"Found camera: {cam.name} (enabled: {cam.enabled}, active: {cam.gameObject.activeInHierarchy})");

            // Search for avatar-related keywords in camera name
            var name = cam.name.ToLower();
            if (name.Contains("avatar") || name.Contains("third") || name.Contains("view") ||
                name.Contains("character") || name.Contains("secondary") || name.Contains("overlay"))
            {
                Log.Info($"Found potential avatar camera: {cam.name}");
                return cam;
            }
        }

        // Fallback: return first non-main camera if no keyword match
        var otherCamera = allCameras.FirstOrDefault(c => c != _mainCamera);
        if (otherCamera != null)
        {
            Log.Info($"Using first non-main camera: {otherCamera.name}");
            return otherCamera;
        }

        return null;
    }

    /// <summary>
    /// Debug mode (F8): Actively searches for cameras that become enabled.
    /// Used to identify the avatar camera when user presses V key.
    /// </summary>
    private static void SearchForAvatarCamera()
    {
        var allCameras = UnityEngine.Object.FindObjectsOfType<Camera>(true);

        foreach (var cam in allCameras)
        {
            if (cam == _mainCamera) continue;

            // Look for newly activated cameras (V key activates OverlayCamera)
            if (cam.enabled && cam.gameObject.activeInHierarchy)
            {
                Log.Info($"FOUND ACTIVE CAMERA: {cam.name}");
                _avatarCamera = cam;
                _isSearchingForCamera = false;
                Log.Info("Avatar camera identified! Now you can use F6 to toggle third-person");
                return;
            }
        }
    }

    /// <summary>
    /// Finds the player GameObject by searching for CharacterController components.
    /// Schedule 1's player is named "AnubISS" and has a CharacterController.
    /// </summary>
    /// <returns>Player GameObject if found, null otherwise</returns>
    private static GameObject? FindPlayer()
    {
        var controllers = UnityEngine.Object.FindObjectsOfType<CharacterController>();
        foreach (var controller in controllers)
        {
            // Validate player is at a real position (not at world origin)
            if (controller.gameObject.transform.position != Vector3.zero)
            {
                Log.Info($"Found player: {controller.gameObject.name}");
                return controller.gameObject;
            }
        }
        return null;
    }

    /// <summary>
    /// Updates camera position and rotation every frame when third-person is active.
    /// Called from LateUpdate() to run after game's camera controller.
    /// NOTE: Camera shake occurs due to game's camera controller fighting our positioning (Blocker #2).
    /// </summary>
    private static void UpdateAvatarCamera()
    {
        if (_avatarCamera == null || _player == null) return;

        // Handle mouse rotation when right mouse button is held
        if (Input.GetMouseButton(1))
        {
            _horizontalAngle += Input.GetAxis("Mouse X") * MouseSensitivity;
            _verticalAngle -= Input.GetAxis("Mouse Y") * MouseSensitivity;
            _verticalAngle = Mathf.Clamp(_verticalAngle, -10f, 60f); // Limit vertical rotation
        }

        // Calculate camera position using spherical coordinates
        var rotation = Quaternion.Euler(_verticalAngle, _horizontalAngle, 0);
        var offset = rotation * new Vector3(0, 0, -CameraDistance);
        var pivotPoint = _player.transform.position + Vector3.up * 1.5f; // Pivot 1.5 units above player
        var targetPosition = pivotPoint + offset;

        // Apply camera position and look at pivot
        // WARNING: This causes shake due to game's camera controller conflict
        _avatarCamera.transform.position = targetPosition;
        _avatarCamera.transform.LookAt(pivotPoint);
    }
}
