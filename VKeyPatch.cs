using HarmonyLib;
using ScheduleOneMods.Logging;
using UnityEngine;

namespace ScheduleOneMods.ThirdPersonCamera;

/// <summary>
/// Patches Schedule 1's V key handler to make it toggleable
/// </summary>
public static class VKeyPatch
{
    private static bool _forceAvatarView;
    private static bool _isThirdPersonEnabled;
    private static float _lastToggleTime;
    private static readonly float ToggleCooldown = 0.5f;

    public static void Initialize(HarmonyLib.Harmony harmony)
    {
        Log.Info("Initializing V key patches...");
        
        // We'll patch any method that checks Input.GetKey(KeyCode.V)
        // This is a generic approach since we don't know the exact class name
        
        try
        {
            // Patch Input.GetKey to return true for V when we want avatar view
            var originalGetKey = AccessTools.Method(typeof(Input), nameof(Input.GetKey), new[] { typeof(KeyCode) });
            var patchGetKey = AccessTools.Method(typeof(VKeyPatch), nameof(GetKeyPatch));
            harmony.Patch(originalGetKey, prefix: new HarmonyMethod(patchGetKey));
            
            Log.Info("Successfully patched Input.GetKey");
        }
        catch (System.Exception ex)
        {
            Log.Error($"Failed to patch Input.GetKey: {ex.Message}");
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
    }

    private static void ToggleThirdPerson()
    {
        _isThirdPersonEnabled = !_isThirdPersonEnabled;
        _forceAvatarView = _isThirdPersonEnabled;
        
        Log.Info($"Third person mode: {(_isThirdPersonEnabled ? "ENABLED" : "DISABLED")}");
        Log.Info($"Avatar view forced: {_forceAvatarView}");
    }

    // Harmony patch: Makes Input.GetKey(KeyCode.V) return true when we want avatar view
    public static bool GetKeyPatch(KeyCode key, ref bool __result)
    {
        if (key == KeyCode.V && _forceAvatarView)
        {
            __result = true;
            return false; // Skip original method
        }
        
        return true; // Run original method
    }
}
