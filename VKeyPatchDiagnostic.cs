using HarmonyLib;
using ScheduleOneMods.Logging;
using UnityEngine;

namespace ScheduleOneMods.ThirdPersonCamera;

/// <summary>
/// Diagnostic version to understand how Schedule 1 checks the V key
/// </summary>
public static class VKeyPatchDiagnostic
{
    private static bool _forceAvatarView;
    private static bool _isThirdPersonEnabled;
    private static float _lastToggleTime;
    private static readonly float ToggleCooldown = 0.5f;
    private static bool _diagnosticMode = true;

    public static void Initialize(HarmonyLib.Harmony harmony)
    {
        Log.Info("Initializing V key diagnostic patches...");
        
        try
        {
            // Patch Input.GetKey
            var getKey = AccessTools.Method(typeof(Input), nameof(Input.GetKey), new[] { typeof(KeyCode) });
            var getKeyPatch = AccessTools.Method(typeof(VKeyPatchDiagnostic), nameof(GetKeyPatch));
            harmony.Patch(getKey, prefix: new HarmonyMethod(getKeyPatch));
            Log.Info("Patched Input.GetKey");
            
            // Patch Input.GetKeyDown
            var getKeyDown = AccessTools.Method(typeof(Input), nameof(Input.GetKeyDown), new[] { typeof(KeyCode) });
            var getKeyDownPatch = AccessTools.Method(typeof(VKeyPatchDiagnostic), nameof(GetKeyDownPatch));
            harmony.Patch(getKeyDown, prefix: new HarmonyMethod(getKeyDownPatch));
            Log.Info("Patched Input.GetKeyDown");
            
            // Patch Input.GetKeyUp
            var getKeyUp = AccessTools.Method(typeof(Input), nameof(Input.GetKeyUp), new[] { typeof(KeyCode) });
            var getKeyUpPatch = AccessTools.Method(typeof(VKeyPatchDiagnostic), nameof(GetKeyUpPatch));
            harmony.Patch(getKeyUp, prefix: new HarmonyMethod(getKeyUpPatch));
            Log.Info("Patched Input.GetKeyUp");
            
            Log.Info("All Input patches applied successfully");
        }
        catch (System.Exception ex)
        {
            Log.Error($"Failed to patch Input methods: {ex.Message}");
        }
    }

    public static void Update()
    {
        // Toggle with F6
        if (Input.GetKeyDown(KeyCode.F6) && Time.time - _lastToggleTime > ToggleCooldown)
        {
            _lastToggleTime = Time.time;
            ToggleThirdPerson();
        }
        
        // Press F7 to toggle diagnostic mode
        if (Input.GetKeyDown(KeyCode.F7))
        {
            _diagnosticMode = !_diagnosticMode;
            Log.Info($"Diagnostic mode: {(_diagnosticMode ? "ON" : "OFF")}");
        }
    }

    private static void ToggleThirdPerson()
    {
        _isThirdPersonEnabled = !_isThirdPersonEnabled;
        _forceAvatarView = _isThirdPersonEnabled;
        
        Log.Info($"Third person mode: {(_isThirdPersonEnabled ? "ENABLED" : "DISABLED")}");
        Log.Info($"Avatar view forced: {_forceAvatarView}");
        
        if (_isThirdPersonEnabled)
        {
            Log.Info("Watch console for V key checks...");
        }
    }

    // Patch for Input.GetKey
    public static bool GetKeyPatch(KeyCode key, ref bool __result)
    {
        if (key == KeyCode.V)
        {
            if (_diagnosticMode)
            {
                Log.Debug($"Game checked Input.GetKey(V) - Original would return: {Input.GetKey(KeyCode.V)}, We return: {_forceAvatarView || Input.GetKey(KeyCode.V)}");
            }
            
            if (_forceAvatarView)
            {
                __result = true;
                return false; // Skip original
            }
        }
        
        return true; // Run original
    }

    // Patch for Input.GetKeyDown
    public static bool GetKeyDownPatch(KeyCode key, ref bool __result)
    {
        if (key == KeyCode.V)
        {
            if (_diagnosticMode)
            {
                Log.Debug($"Game checked Input.GetKeyDown(V) - Original would return: {Input.GetKeyDown(KeyCode.V)}");
            }
        }
        
        return true; // Always run original for GetKeyDown
    }

    // Patch for Input.GetKeyUp
    public static bool GetKeyUpPatch(KeyCode key, ref bool __result)
    {
        if (key == KeyCode.V)
        {
            if (_diagnosticMode)
            {
                Log.Debug($"Game checked Input.GetKeyUp(V) - Original would return: {Input.GetKeyUp(KeyCode.V)}");
            }
            
            // If we're forcing avatar view, never report V as released
            if (_forceAvatarView)
            {
                __result = false;
                return false; // Skip original
            }
        }
        
        return true; // Run original
    }
}
