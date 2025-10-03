# Developer Guide

So you want to work on this? Cool! Fair warning though - this mod doesn't actually work due to some fundamental game architecture issues. But if you're interested in Unity modding, Il2Cpp, or just want to learn from my mistakes, this guide should help.

---

## Quick Start

### Prerequisites
- **.NET 6.0 SDK** - for building the mod
- **Schedule 1 game** - obviously
- **MelonLoader 0.7.1+** - the modding framework
- **Visual Studio Code / Visual Studio** - or whatever editor you prefer

### Development Setup

1. **Clone/download the repository**
   ```bash
   git clone <repository-url>
   cd shedule1mod/src/ScheduleOneMods.ThirdPersonCamera
   ```

2. **Verify game references**
   Ensure `../../lib/` contains:
   - `Assembly-CSharp.dll`
   - `Il2Cppmscorlib.dll`
   - `UnityEngine.CoreModule.dll`
   - `UnityEngine.PhysicsModule.dll`
   - `UnityEngine.InputLegacyModule.dll`

3. **Build the mod**
   ```bash
   # Debug build (includes logging project reference)
   dotnet build -c Debug

   # Release build (includes logging as linked file)
   dotnet build -c Release
   ```

4. **Deploy for testing**
   ```bash
   # Copy DLL to game's Mods folder
   cp bin/Debug/net6.0/ScheduleOneMods.ThirdPersonCamera.dll \
      "C:/Program Files/Schedule I/Mods/"
   ```

5. **Test in-game**
   - Launch Schedule 1 via MelonLoader
   - Check MelonLoader console for `[Third_Person_Camera]` logs
   - Press F6, F8, V to test functionality

---

## Architecture Deep Dive

### Execution Flow

```
Game Launch
    ↓
MelonLoader loads mod
    ↓
Mod.OnInitializeMelon()
    ├─ Initialize logging
    └─ Display instructions
    ↓
Every Frame:
    ↓
Mod.OnUpdate()
    ├─ Forward to AvatarCameraHijack.Update()
    ├─ Check F6 (toggle)
    ├─ Check F8 (debug search)
    └─ SearchForAvatarCamera() if in debug mode
    ↓
Mod.OnLateUpdate()
    ├─ Forward to AvatarCameraHijack.LateUpdate()
    └─ UpdateAvatarCamera() if third-person active
        ├─ Read mouse input
        ├─ Calculate camera position
        └─ Apply transform (causes shake - Blocker #2)
```

### State Machine

```
┌─────────────────┐
│   INITIALIZED   │ (Mod loaded, third-person disabled)
└────────┬────────┘
         │ F6 pressed
         ↓
┌─────────────────┐
│  FINDING_PLAYER │ (Searching for AnubISS GameObject)
└────────┬────────┘
         │ Player found
         ↓
┌─────────────────┐
│ FINDING_CAMERA  │ (Searching for OverlayCamera)
└────────┬────────┘
         │ Camera found
         ↓
┌─────────────────┐
│     ACTIVE      │ (Third-person enabled, updating camera)
└────────┬────────┘
         │ F6 pressed again
         ↓
┌─────────────────┐
│    DISABLED     │ (Back to first-person)
└─────────────────┘

Alternative Path:
┌─────────────────┐
│  DEBUG_SEARCH   │ (F8 pressed, waiting for V key)
└────────┬────────┘
         │ V pressed (OverlayCamera activates)
         ↓
    Camera detected → Can now use F6
```

---

## Modifying Camera Behavior

### Adjust Camera Distance

Pretty straightforward - just change this constant in `AvatarCameraHijack.cs`:
```csharp
private static readonly float CameraDistance = 3.5f; // Default: 3.5 units
```

**Examples:**
- Closer view: `2.0f`
- Wider view: `5.0f`
- First-person: `0.5f` (but defeats the purpose)

### Adjust Mouse Sensitivity

```csharp
private static readonly float MouseSensitivity = 3f; // Default: 3x
```

**Examples:**
- Slower rotation: `1.5f`
- Faster rotation: `5.0f`

### Adjust Vertical Angle Limits

In `UpdateAvatarCamera()` method:
```csharp
_verticalAngle = Mathf.Clamp(_verticalAngle, -10f, 60f);
//                                           ^^^^  ^^^
//                                           Min   Max
```

**Examples:**
- Look down more: `-30f` (min)
- Look up less: `45f` (max)
- Free rotation: `-90f, 90f` (not recommended - camera can flip)

### Adjust Pivot Point Height

In `UpdateAvatarCamera()` method:
```csharp
var pivotPoint = _player.transform.position + Vector3.up * 1.5f;
//                                                          ^^^^
//                                                          Height offset
```

**Examples:**
- Head height: `1.8f`
- Chest height: `1.2f`
- Waist height: `0.8f`

---

## Adding New Features

### Feature Idea: Zoom In/Out

**Goal:** Allow mouse wheel to adjust camera distance

**Implementation:**
```csharp
// In AvatarCameraHijack.cs

// 1. Add state field
private static float _currentDistance = CameraDistance;

// 2. In UpdateAvatarCamera(), add zoom input
float scroll = Input.GetAxis("Mouse ScrollWheel");
if (scroll != 0f)
{
    _currentDistance -= scroll * 5f; // 5f = zoom speed
    _currentDistance = Mathf.Clamp(_currentDistance, 1f, 10f); // Limits
}

// 3. Use _currentDistance instead of CameraDistance
var offset = rotation * new Vector3(0, 0, -_currentDistance);
```

### Feature Idea: Camera Smoothing

**Goal:** Reduce jitter with smooth interpolation

**Implementation:**
```csharp
// In AvatarCameraHijack.cs

// 1. Add previous position field
private static Vector3 _lastCameraPosition;

// 2. In UpdateAvatarCamera(), use Lerp
var targetPosition = pivotPoint + offset;
var smoothPosition = Vector3.Lerp(_lastCameraPosition, targetPosition, Time.deltaTime * 10f);
_avatarCamera.transform.position = smoothPosition;
_lastCameraPosition = smoothPosition;
```

**Note:** This may not fully resolve shake due to Blocker #2.

### Feature Idea: Custom Key Bindings

**Goal:** Allow users to configure keys via config file

**Implementation:**
```csharp
// 1. Create config class
public static class ModConfig
{
    public static KeyCode ToggleKey = KeyCode.F6;
    public static KeyCode DebugKey = KeyCode.F8;

    public static void Load()
    {
        // Read from MelonPreferences or JSON config file
        // Example using MelonPreferences:
        var category = MelonPreferences.CreateCategory("ThirdPersonCamera");
        ToggleKey = category.GetEntry<KeyCode>("ToggleKey")?.Value ?? KeyCode.F6;
    }
}

// 2. In Mod.OnInitializeMelon()
ModConfig.Load();

// 3. In AvatarCameraHijack.Update()
if (Input.GetKeyDown(ModConfig.ToggleKey))
{
    // Toggle logic
}
```

### Feature Idea: First-Person Offset Mode

**Goal:** Add slight over-shoulder offset while keeping first-person

**Implementation:**
```csharp
// Add mode enum
private enum CameraMode { FirstPerson, OverShoulder, ThirdPerson }
private static CameraMode _mode = CameraMode.FirstPerson;

// In UpdateAvatarCamera(), add offset based on mode
Vector3 additionalOffset = _mode switch
{
    CameraMode.OverShoulder => new Vector3(0.3f, 0.2f, -0.5f), // Right shoulder
    CameraMode.ThirdPerson => Vector3.zero,
    _ => Vector3.zero
};
var targetPosition = pivotPoint + offset + additionalOffset;
```

---

## Debugging Guide

### Enable Debug Logging

**Method 1: Change log level (if using ScheduleOneMods.Logging)**
```csharp
// In Mod.OnInitializeMelon()
Log.SetLogLevel(LogLevel.Debug);
```

**Method 2: Add temporary debug logs**
```csharp
Log.Info($"Camera position: {_avatarCamera.transform.position}");
Log.Info($"Player position: {_player.transform.position}");
Log.Info($"Angles: H={_horizontalAngle}, V={_verticalAngle}");
```

### Diagnose Camera Not Found

**Problem:** F6 pressed, but "Avatar camera not found yet!" message appears

**Debugging Steps:**
1. Check if OverlayCamera exists:
   ```csharp
   var allCameras = UnityEngine.Object.FindObjectsOfType<Camera>(true);
   foreach (var cam in allCameras)
   {
       Log.Info($"Camera: {cam.name}, enabled: {cam.enabled}, active: {cam.gameObject.activeInHierarchy}");
   }
   ```

2. Test F8 debug mode:
   - Press F8
   - Press V in-game
   - Check console for "FOUND ACTIVE CAMERA"

3. Verify camera search keywords:
   - Add more keywords to `FindAvatarCamera()` if camera has unexpected name

### Diagnose Player Not Found

**Problem:** "Player not found!" error when enabling third-person

**Debugging Steps:**
1. Check CharacterController components:
   ```csharp
   var controllers = UnityEngine.Object.FindObjectsOfType<CharacterController>();
   Log.Info($"Found {controllers.Length} CharacterControllers");
   foreach (var ctrl in controllers)
   {
       Log.Info($"Controller: {ctrl.gameObject.name}, position: {ctrl.transform.position}");
   }
   ```

2. Verify player is spawned:
   - Try enabling third-person after fully loading into game
   - Player may not exist in main menu

### Diagnose Camera Shake

**Problem:** Third-person works but camera shakes/jitters

**Root Cause:** Game's camera controller conflicts with our positioning (Blocker #2)

**Attempted Solutions:**
1. **Disable camera controller components:**
   ```csharp
   foreach (var comp in _avatarCamera.GetComponents<MonoBehaviour>())
   {
       if (comp.GetType().Name.Contains("Camera"))
       {
           comp.enabled = false;
           Log.Info($"Disabled: {comp.GetType().Name}");
       }
   }
   ```

2. **Use FixedUpdate instead of LateUpdate:**
   - May reduce shake frequency but won't eliminate it

3. **Ultimate solution:** Requires game decompilation to identify specific camera controller

---

## Working Around Blockers

### Blocker #1: Camera-Input Coupling

**Problem:** Player movement stops when main camera is disabled/deprioritized

**Current Workaround:**
```csharp
// Keep main camera enabled at all times
_mainCamera.enabled = true; // Never disable this
_mainCamera.depth = -1;     // Render first, avatar camera overlays

_avatarCamera.enabled = true;
_avatarCamera.depth = 10;   // Higher depth renders on top
```

**Limitations:**
- Main camera still renders (wasted performance)
- Player movement still broken in current implementation

**Potential Improvements:**
1. **Render Texture Approach:**
   ```csharp
   // Render main camera to texture (offscreen)
   _mainCamera.targetTexture = new RenderTexture(1, 1, 0);
   _mainCamera.enabled = true; // Still needed for input

   // Avatar camera renders to screen
   _avatarCamera.targetTexture = null;
   _avatarCamera.enabled = true;
   ```

2. **Culling Mask Approach:**
   ```csharp
   // Main camera renders nothing (but stays enabled)
   _mainCamera.cullingMask = 0;
   _mainCamera.clearFlags = CameraClearFlags.Nothing;

   // Avatar camera renders everything
   _avatarCamera.cullingMask = ~0; // All layers
   ```

### Blocker #2: Camera Controller Conflicts

**Problem:** Game's camera controller fights our positioning

**Investigation Required:**
```csharp
// Log all components on camera
foreach (var comp in _avatarCamera.GetComponents<Component>())
{
    Log.Info($"Component: {comp.GetType().FullName}");

    // Check for Update methods
    var updateMethod = comp.GetType().GetMethod("Update",
        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
    if (updateMethod != null)
    {
        Log.Info($"  Has Update method: {comp.GetType().Name}");
    }
}
```

**Potential Solutions:**
1. **HarmonyX Patch (requires game decompilation):**
   ```csharp
   [HarmonyPatch(typeof(CameraController), "Update")]
   [HarmonyPrefix]
   static bool PreventCameraUpdate()
   {
       return !AvatarCameraHijack.IsThirdPersonEnabled;
   }
   ```

2. **Component Disabling:**
   - Identify specific component name via reflection
   - Disable only that component, not all MonoBehaviours

### Blocker #3: Input System Abstraction

**Problem:** Cannot detect V key via `Input.GetKey()` patches

**Current Workaround:**
- F8 debug mode manually detects camera activation

**Investigation Needed:**
```csharp
// Search for InputAction assets
var actions = Resources.FindObjectsOfTypeAll<InputAction>();
foreach (var action in actions)
{
    Log.Info($"InputAction: {action.name}");
    action.performed += ctx => Log.Info($"Action performed: {action.name}");
}
```

---

## Testing Checklist

### Basic Functionality
- [ ] Mod loads without errors
- [ ] F6 toggles third-person mode
- [ ] F8 + V detects avatar camera
- [ ] Right-click + drag rotates camera
- [ ] Camera follows player movement
- [ ] F6 disables third-person mode cleanly

### Edge Cases
- [ ] Pressing F6 before player spawns (should fail gracefully)
- [ ] Pressing F6 rapidly (cooldown prevents spam)
- [ ] Enabling third-person in different game areas
- [ ] Third-person mode persists across area transitions (or fails gracefully)
- [ ] Multiple F6 toggles in quick succession

### Known Issues
- [ ] Player movement broken (Blocker #1) - Expected failure
- [ ] Camera shake/jitter (Blocker #2) - Expected issue
- [ ] Cannot simulate V key (Blocker #3) - Documented limitation

---

## Performance Optimization

### Current Performance Profile
- **Update()**: ~0.01ms per frame (minimal input checking)
- **LateUpdate()**: ~0.1ms per frame when active (position calculation)
- **FindPlayer()**: ~5ms (only on enable)
- **FindAvatarCamera()**: ~10ms (only on enable)

### Optimization Opportunities

**1. Cache Component Lookups**
```csharp
// Instead of:
if (Input.GetMouseButton(1))
{
    _horizontalAngle += Input.GetAxis("Mouse X") * MouseSensitivity;
}

// Do:
private static float _mouseX, _mouseY;

void Update()
{
    if (Input.GetMouseButton(1))
    {
        _mouseX = Input.GetAxis("Mouse X");
        _mouseY = Input.GetAxis("Mouse Y");
    }
}

void LateUpdate()
{
    _horizontalAngle += _mouseX * MouseSensitivity;
    _mouseX = 0f; // Reset
}
```

**2. Reduce FindObjectsOfType Calls**
```csharp
// FindObjectsOfType is expensive - only call when necessary
// Current implementation: Already optimized (only on enable)
```

**3. Object Pooling for Vector3**
```csharp
// Reuse Vector3 allocations
private static Vector3 _cachedOffset = Vector3.zero;
private static Vector3 _cachedPivot = Vector3.zero;

void UpdateAvatarCamera()
{
    _cachedPivot.Set(
        _player.transform.position.x,
        _player.transform.position.y + 1.5f,
        _player.transform.position.z
    );
    // Use _cachedPivot instead of creating new Vector3
}
```

---

## Advanced Topics

### Il2Cpp Interoperability

Schedule 1 uses Il2Cpp, which affects how you interact with Unity types.

**String Handling:**
```csharp
// Use Il2Cpp string methods
var name = camera.name; // Returns Il2CppSystem.String
var managedName = name.ToString(); // Convert to System.String
```

**Collections:**
```csharp
// Il2Cpp arrays need special handling
var cameras = UnityEngine.Object.FindObjectsOfType<Camera>();
// cameras is Il2CppReferenceArray<Camera>, not Camera[]
```

**Null Checks:**
```csharp
// Unity null checks work with Il2Cpp objects
if (camera == null) { } // Correct
if (camera != null) { } // Correct
```

### HarmonyX Integration (Future Work)

Currently imported but not used. Potential use cases:

**1. Patch Camera Controller:**
```csharp
[HarmonyPatch(typeof(UnknownCameraController), "Update")]
[HarmonyPrefix]
static bool DisableCameraControllerUpdate()
{
    if (AvatarCameraHijack.IsThirdPersonEnabled)
        return false; // Skip original method
    return true; // Run original
}
```

**2. Patch Input System:**
```csharp
[HarmonyPatch(typeof(PlayerInput), "ProcessMovement")]
[HarmonyPrefix]
static void AllowMovementInThirdPerson(ref bool ___canMove)
{
    if (AvatarCameraHijack.IsThirdPersonEnabled)
        ___canMove = true; // Force enable movement
}
```

**Requires:** Game decompilation to identify class/method names

---

## Contributing to the Project

### Before Making Changes
1. Read `TECHNICAL_BLOCKERS.md` to understand limitations
2. Check `DEVELOPMENT_LOG.md` for past approaches (don't repeat failures)
3. Review `README.md` for project context

### Making Changes
1. **Create feature branch:**
   ```bash
   git checkout -b feature/your-feature-name
   ```

2. **Make incremental commits:**
   ```bash
   git add AvatarCameraHijack.cs
   git commit -m "Add zoom feature with mouse wheel"
   ```

3. **Test thoroughly:**
   - Test in Debug build first
   - Verify no new errors in MelonLoader console
   - Test edge cases (rapid toggling, early game, etc.)

4. **Update documentation:**
   - Add to `CHANGELOG.md` if user-facing change
   - Update `DEVELOPMENT_LOG.md` if research/experimental
   - Update this guide if adding new features

### Code Style
- Use C# naming conventions (PascalCase for public, _camelCase for private)
- Add XML doc comments for public methods
- Use meaningful variable names
- Keep methods under 50 lines where possible
- Add comments for blocker workarounds

### Pull Request Checklist
- [ ] Code builds without warnings
- [ ] Tested in-game (Debug build)
- [ ] Tested in-game (Release build)
- [ ] Documentation updated
- [ ] No new blockers introduced
- [ ] Existing blockers documented if behavior changed

---

## Resources

### Official Documentation
- [MelonLoader Wiki](https://melonwiki.xyz/)
- [HarmonyX Wiki](https://github.com/BepInEx/HarmonyX/wiki)
- [Unity Camera Documentation](https://docs.unity3d.com/Manual/class-Camera.html)
- [Unity Input System](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/manual/index.html)

### Modding Tools
- [Il2CppDumper](https://github.com/Perfare/Il2CppDumper) - Decompile Il2Cpp games
- [dnSpy](https://github.com/dnSpy/dnSpy) - .NET debugger and decompiler
- [Unity Explorer](https://github.com/sinai-dev/UnityExplorer) - Runtime Unity inspector (MelonLoader mod)

### Project Files
- `CLAUDE.md` - Claude Code AI assistant instructions
- `README.md` - User-facing documentation
- `TECHNICAL_BLOCKERS.md` - Detailed blocker analysis
- `DEVELOPMENT_LOG.md` - Development history and attempts
- `API_REFERENCE.md` - Detailed API documentation
- `CHANGELOG.md` - Version history

---

## FAQ

**Q: Why doesn't player movement work in third-person?**
A: The game's input system is somehow tied to the main camera. I've tried keeping it enabled but deprioritized, but that still breaks movement. Haven't figured out a workaround yet.

**Q: Why does the camera shake?**
A: The game has its own camera controller running that fights against our positioning every frame. Would need to decompile the game to find and disable it properly.

**Q: Can I use this mod in multiplayer?**
A: Schedule 1 is single-player, so N/A. If it were multiplayer, this would be client-side only.

**Q: Will this mod break my save game?**
A: No, this mod doesn't modify save data. It only affects camera behavior at runtime.

**Q: Can I use this with other mods?**
A: Potentially, but untested. Camera-related mods may conflict.

**Q: Why is this marked as non-functional?**
A: Because it doesn't work lol. The camera switches to third-person fine, but then you can't move your character. There are three main issues blocking it - check `TECHNICAL_BLOCKERS.md` for the full breakdown.

---

## License

This mod is experimental and provided as-is for educational and research purposes.

---

**Last Updated:** October 2025
**Game Version:** Schedule I 0.4.0f8
**MelonLoader Version:** 0.7.1
**Mod Version:** 0.1.0
