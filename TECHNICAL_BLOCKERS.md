# Technical Blockers - Third Person Camera Mod

This document provides detailed technical analysis of the blockers preventing a functional third-person camera mod for Schedule 1.

## Overview

Three major technical blockers have been identified:

1. **Camera-Input Coupling** - Player movement breaks when main camera is not primary
2. **Camera Controller Conflicts** - Game's camera system fights external positioning
3. **Input System Abstraction** - Cannot intercept or simulate V key input

---

## Blocker #1: Camera-Input Coupling

### Description

Schedule 1's player input system (WASD movement) is tightly coupled to the main camera in an unknown way. When the main camera is disabled or deprioritized, player movement stops working entirely.

### Evidence

**Test 1: Disable Main Camera**
```csharp
_mainCamera.enabled = false;
_avatarCamera.enabled = true;
```
**Result:** Player cannot move (WASD has no effect)

**Test 2: Camera Depth Layering**
```csharp
_mainCamera.depth = -1;  // Render first
_avatarCamera.depth = 10; // Render on top
// Both cameras enabled
```
**Result:** Player still cannot move

**Test 3: Main Camera Active, Avatar Camera Controlling**
```csharp
_mainCamera.enabled = true;
_avatarCamera.enabled = true;
_avatarCamera.transform.position = thirdPersonPosition;
```
**Result:** Player still cannot move

### Analysis

The coupling is NOT through:
- Camera.enabled state (tested)
- Camera.depth priority (tested)
- Camera.main property (both cameras can be enabled)

The coupling MIGHT be through:
- Custom component on main camera that handles input
- Raycast from camera for input detection
- Camera-specific input layer or culling mask
- Game's custom input manager checking camera state

### Potential Solutions

1. **Find the Input Handler**
   ```csharp
   // Log all components on main camera
   foreach (var component in _mainCamera.GetComponents<Component>())
   {
       Log.Info($"Camera component: {component.GetType().Name}");
   }
   ```

2. **Patch Input Methods**
   - Use Harmony to patch player movement methods
   - Bypass camera check if one exists
   - Requires decompiling game code first

3. **Keep Main Camera Active at Player Position**
   - Main camera stays at player (for input)
   - Avatar camera renders on top (for display)
   - May require render texture or camera stacking

---

## Blocker #2: Camera Controller Conflicts

### Description

Schedule 1 has an active camera controller component that continuously repositions the camera. When we try to position the camera externally, the game's controller fights back, causing severe shake/jitter.

### Evidence

**Test 1: Direct Position Assignment**
```csharp
void LateUpdate()
{
    _camera.transform.position = targetPosition;
    _camera.transform.rotation = targetRotation;
}
```
**Result:** Severe camera shake (game resets position every frame)

**Test 2: Smooth Interpolation**
```csharp
void LateUpdate()
{
    _camera.transform.position = Vector3.Lerp(
        _camera.transform.position, 
        targetPosition, 
        Time.deltaTime * 15f
    );
}
```
**Result:** Still shakes (interpolation doesn't help)

**Test 3: Disable All MonoBehaviours**
```csharp
foreach (var component in _camera.GetComponents<MonoBehaviour>())
{
    component.enabled = false;
}
```
**Result:** Made shake worse, broke more functionality

### Analysis

The camera controller is:
- Running in Update() or LateUpdate()
- Resetting camera position every frame
- Not a standard Unity component (couldn't identify by name)
- Possibly an Il2Cpp component (harder to detect)

The shake pattern suggests:
- Frame 1: Our code positions camera at A
- Frame 2: Game's controller positions camera at B
- Frame 3: Our code positions camera at A
- Result: Camera oscillates between A and B (shake)

### Potential Solutions

1. **Identify Specific Controller**
   ```csharp
   // Decompile game to find controller class name
   // Then disable specifically:
   var controller = _camera.GetComponent("CameraController");
   if (controller != null) controller.enabled = false;
   ```

2. **Patch Controller Update Methods**
   ```csharp
   // Use Harmony to patch the camera controller's Update method
   [HarmonyPatch(typeof(CameraController), "Update")]
   [HarmonyPrefix]
   static bool PreventCameraUpdate()
   {
       return !_isThirdPersonEnabled; // Skip if third-person active
   }
   ```

3. **Run After Game's Controller**
   - If game uses Update(), we use LateUpdate()
   - If game uses LateUpdate(), we need a later hook
   - May require custom update order

---

## Blocker #3: Input System Abstraction

### Description

Schedule 1 does not use Unity's standard `Input.GetKey()` for the V key (avatar view). This means we cannot intercept or simulate the V key press using standard Harmony patches.

### Evidence

**Test 1: Patch Input.GetKey**
```csharp
[HarmonyPatch(typeof(Input), nameof(Input.GetKey))]
[HarmonyPrefix]
static bool GetKeyPatch(KeyCode key, ref bool __result)
{
    if (key == KeyCode.V)
    {
        Log.Info("V key checked!"); // This never appears
        if (_forceV) __result = true;
    }
    return true;
}
```
**Result:** No log messages when pressing V, patch has no effect

**Test 2: Diagnostic Logging**
- Patched GetKey, GetKeyDown, GetKeyUp
- Held V key manually in game
- Avatar view activated (works normally)
- Zero log messages appeared

**Conclusion:** Game uses different input system

### Analysis

Schedule 1 likely uses:
1. **Unity's New Input System**
   - InputAction assets
   - Not detectable through Input class patches
   
2. **Custom Input Manager**
   - Game's own input handling code
   - Reads input directly from hardware

3. **Input Binding System**
   - Rebindable controls
   - Abstracted from Unity's Input class

### Potential Solutions

1. **Find InputAction Assets**
   ```csharp
   // Search for InputAction or InputActionAsset
   var actions = Resources.FindObjectsOfTypeAll<InputAction>();
   foreach (var action in actions)
   {
       Log.Info($"Input action: {action.name}");
   }
   ```

2. **Decompile and Find Input Handler**
   - Use Il2CppDumper on game assembly
   - Search for "avatar" or "view" related methods
   - Find what triggers avatar camera
   - Patch that method instead

3. **Simulate at Lower Level**
   - Instead of faking V key, directly call avatar view method
   - Requires knowing the method name
   - Use Harmony to invoke it

---

## Combined Impact

These three blockers compound each other:

```
┌─────────────────────────────────────────────────────┐
│  Want: Third-Person Camera with Player Movement     │
└─────────────────────────────────────────────────────┘
                        │
        ┌───────────────┼───────────────┐
        │               │               │
        ▼               ▼               ▼
┌──────────────┐ ┌──────────────┐ ┌──────────────┐
│   Blocker 1  │ │   Blocker 2  │ │   Blocker 3  │
│ Camera-Input │ │   Camera     │ │    Input     │
│   Coupling   │ │  Controller  │ │    System    │
└──────────────┘ └──────────────┘ └──────────────┘
        │               │               │
        └───────────────┴───────────────┘
                        │
                        ▼
        ┌───────────────────────────────┐
        │  Cannot achieve both:         │
        │  1. Third-person view         │
        │  2. Player movement           │
        └───────────────────────────────┘
```

**Blocker 1** prevents movement when using custom camera  
**Blocker 2** causes shake when positioning camera  
**Blocker 3** prevents using game's built-in avatar view

Solving any ONE blocker is insufficient - all three must be addressed.

---

## Severity Assessment

### Blocker #1: Camera-Input Coupling
**Severity:** 🔴 **CRITICAL**  
**Impact:** Complete loss of player control  
**Workaround:** None found  
**Solution Difficulty:** High (requires decompiling game)

### Blocker #2: Camera Controller Conflicts
**Severity:** 🟡 **HIGH**  
**Impact:** Unusable camera (shake/jitter)  
**Workaround:** Possible (if controller can be identified)  
**Solution Difficulty:** Medium (requires finding specific component)

### Blocker #3: Input System Abstraction
**Severity:** 🟡 **MEDIUM**  
**Impact:** Cannot hijack existing avatar view  
**Workaround:** Possible (direct camera control instead)  
**Solution Difficulty:** Medium (requires decompiling game)

---

## Research Needed

To overcome these blockers, the following research is required:

### 1. Decompile Schedule 1
**Tools:** Il2CppDumper, dnSpy, ILSpy  
**Target Files:**
- `Assembly-CSharp.dll`
- `GameAssembly.dll`

**Information Needed:**
- Camera controller class name and methods
- Input system implementation
- Player movement code
- Avatar view activation method

### 2. Component Analysis
**Method:** Runtime inspection  
**Code:**
```csharp
// Log all components on relevant objects
var camera = Camera.main;
foreach (var comp in camera.GetComponents<Component>())
{
    Log.Info($"Camera: {comp.GetType().FullName}");
    
    // Try to get component methods
    var methods = comp.GetType().GetMethods();
    foreach (var method in methods)
    {
        if (method.Name.Contains("Update") || 
            method.Name.Contains("Input") ||
            method.Name.Contains("Move"))
        {
            Log.Info($"  Method: {method.Name}");
        }
    }
}
```

### 3. Input System Discovery
**Method:** Asset inspection  
**Code:**
```csharp
// Find all InputAction assets
var inputActions = Resources.FindObjectsOfTypeAll<InputAction>();
var inputAssets = Resources.FindObjectsOfTypeAll<InputActionAsset>();

Log.Info($"Found {inputActions.Length} InputActions");
Log.Info($"Found {inputAssets.Length} InputActionAssets");
```

---

## Conclusion

All three blockers are solvable in theory, but require:
1. Decompiling the game to understand its architecture
2. Identifying specific components and methods
3. Creating targeted Harmony patches

Without access to the game's source code or decompiled assembly, progress is limited to trial-and-error approaches, which have proven insufficient.

The mod is currently **blocked** pending deeper analysis of Schedule 1's internal systems.

---

**Status:** Blocked  
**Next Steps:** Decompile game, analyze components, create targeted patches  
**Estimated Effort:** 10-20 hours of additional research and development
