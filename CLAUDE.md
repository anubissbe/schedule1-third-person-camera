# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Third Person Camera Mod for Schedule 1**

A MelonLoader mod attempting to add third-person camera functionality to the Unity game "Schedule 1". This is an **experimental/non-functional** project with documented technical blockers.

**Status:** ⚠️ Research/experimental - core functionality blocked by game architecture limitations

## Build Commands

```bash
# Build debug version (includes logging project reference)
dotnet build -c Debug

# Build release version (includes logging as linked file)
dotnet build -c Release

# Clean build artifacts
dotnet clean
```

**Output:** DLL placed in `bin/{Configuration}/net6.0/ScheduleOneMods.ThirdPersonCamera.dll`

**Deployment:** Copy DLL to `Schedule I/Mods/` directory in game installation

## Technical Stack

- **Framework:** .NET 6.0
- **Modding Framework:** MelonLoader (attributes injected via MSBuild targets)
- **Patching Library:** HarmonyX 2.14.0 (for runtime patching)
- **Game Engine:** Unity (Il2Cpp build)
- **Game References:** Located in `../../lib/` directory
  - `Assembly-CSharp.dll` - Game code
  - `Il2Cppmscorlib.dll` - Il2Cpp runtime
  - `UnityEngine.CoreModule.dll` - Core Unity
  - `UnityEngine.PhysicsModule.dll` - Physics
  - `UnityEngine.InputLegacyModule.dll` - Legacy input system

## Architecture

### Mod Entry Point

**File:** `Mod.cs`

- Inherits from `MelonMod` (MelonLoader base class)
- `OnInitializeMelon()` - Initialization hook
- `OnUpdate()` - Called every frame (forwarded to camera controller)
- `OnLateUpdate()` - Called after Update (forwarded to camera controller)

### Camera System Approaches

The project contains **multiple implementation attempts** (all currently non-functional):

1. **`AvatarCameraHijack.cs`** (CURRENT/BEST) - Hijacks game's built-in avatar camera
2. `CameraController.cs` - Creates independent third-person camera
3. `CameraController_v2.cs` - Modifies main camera directly
4. `CameraControllerFinal.cs` - Attempts to disable game's camera scripts
5. `CameraControllerV9.cs` - V key simulation approach
6. `VKeyPatch.cs` / `VKeyPatchDiagnostic.cs` - HarmonyX input patching attempts

**Active Implementation:** `AvatarCameraHijack.cs`
- F6: Toggle third-person mode
- F8: Debug camera search mode
- Right-click + drag: Rotate camera (when in third-person)
- Finds player via `CharacterController` component (named "AnubISS")
- Searches for avatar camera (found as "OverlayCamera")

### Logging System

**Debug builds:** Use project reference to `../ScheduleOneMods.Logging/`
**Release builds:** Link `Log.cs` directly as compile item

Usage: `Log.Info()`, `Log.Warning()`, `Log.Error()`, `Log.Debug()`

## Critical Technical Blockers

⚠️ **This mod is blocked by three fundamental game architecture issues:**

### 1. Camera-Input Coupling (CRITICAL)
- Player movement (WASD) stops when main camera is disabled or deprioritized
- Input system is tightly coupled to main camera in unknown way
- No workaround found without decompiling game code

### 2. Camera Controller Conflicts (HIGH)
- Game's camera controller actively fights external positioning
- Causes severe shake/jitter when camera is moved programmatically
- Controller component type unknown (Il2Cpp obfuscation)

### 3. Input System Abstraction (MEDIUM)
- Game does NOT use Unity's `Input.GetKey()` for V key (avatar view)
- Likely uses new Unity Input System or custom input handler
- HarmonyX patches on `Input` class have zero effect

**See `TECHNICAL_BLOCKERS.md` for detailed analysis and evidence**

## Key Game Discoveries

**Player Object:**
- GameObject name: "AnubISS"
- Has `CharacterController` component
- World position validation: `position != Vector3.zero`

**Camera System:**
- Main camera: First-person view, depth=0
- OverlayCamera: Avatar view (V key), normally disabled
- Hierarchy: `CameraContainer > Main Camera`

**V Key (Avatar View):**
- Hold V = shows character from front
- Input NOT detectable via `Input.GetKey()` patches
- Camera: "OverlayCamera" activates when V is held

## Development Notes

### When Working on Camera Code:

1. **Player detection** happens via `FindObjectsOfType<CharacterController>()`
2. **Camera search** must check both enabled and disabled cameras (`FindObjectsOfType<Camera>(includeInactive: true)`)
3. **LateUpdate** is used for camera positioning (runs after game's camera update)
4. **DO NOT disable main camera** - breaks player input entirely
5. **Camera depth layering** doesn't solve input coupling issue

### HarmonyX Patching:

- Use `[HarmonyPatch(typeof(TargetClass), nameof(MethodName))]`
- Prefix patches: Return `false` to skip original method
- Il2Cpp games may have obfuscated/unnamed types
- **Input patches don't work** for this game's custom input system

### Testing Workflow:

1. Build mod: `dotnet build -c Debug`
2. Copy DLL to game Mods folder
3. Launch game via MelonLoader
4. Check MelonLoader console for `[Third_Person_Camera]` logs
5. In-game: Press F6 (toggle), F8 (debug), V (avatar view)

## Next Steps for Future Development

1. **Decompile game using Il2CppDumper** to understand:
   - Camera controller component names
   - Input system implementation
   - Player movement code coupling to camera

2. **Identify specific camera controller** via runtime reflection:
   ```csharp
   foreach (var comp in camera.GetComponents<Component>())
       Log.Info($"Component: {comp.GetType().FullName}");
   ```

3. **Find InputAction assets** (new Input System):
   ```csharp
   var actions = Resources.FindObjectsOfTypeAll<InputAction>();
   ```

4. **Consider alternative approaches:**
   - Photo mode instead of live third-person
   - Render texture approach (dual camera rendering)
   - Wait for official mod API from developers

## File Reference

- `Mod.cs` - MelonLoader entry point
- `AvatarCameraHijack.cs` - Current best implementation (still blocked)
- `README.md` - User-facing documentation with full context
- `TECHNICAL_BLOCKERS.md` - Detailed blocker analysis and evidence
- `DEVELOPMENT_LOG.md` - Development history (if present)
- `CHANGELOG.md` - Version history (if present)
- `ScheduleOneMods.ThirdPersonCamera.csproj` - MSBuild project file

## Important Constraints

- **Do not create new files** unless absolutely necessary - this is an experimental/research project
- **Preserve all existing attempts** - each file documents a different failed approach
- **When modifying camera code** - test against all three blockers (see TECHNICAL_BLOCKERS.md)
- **HarmonyX patches require game decompilation** to target correct methods

## Code Files (Inactive Implementations)

These files are **NOT** currently active but preserved for research:
- `CameraController.cs` - Approach 1: Independent camera creation
- `CameraController_v2.cs` - Approach 2: Main camera repositioning
- `CameraControllerFinal.cs` - Approach 3: Camera script disabling
- `CameraControllerV9.cs` - Approach 4: V key simulation
- `VKeyPatch.cs` - Approach 4a: HarmonyX Input.GetKey patching
- `VKeyPatchDiagnostic.cs` - Approach 4b: Diagnostic Input patching
- `Mod_v2.cs` - Alternative mod entry point (unused)

**Only `Mod.cs` and `AvatarCameraHijack.cs` are actively used.** Other files remain for documentation of attempted solutions.
