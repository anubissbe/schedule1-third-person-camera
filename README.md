# Schedule 1 - Third Person Camera Mod

**Status:** ⚠️ **Experimental / Non-Functional**

This mod attempts to add a third-person camera view to Schedule 1. Despite extensive research and multiple implementation approaches, a fully functional version has not been achieved due to technical limitations.

## 🎯 Goal

Create a toggleable third-person camera mod that allows players to:
- Press F6 to toggle between first-person and third-person views
- View their character from behind
- Rotate the camera with mouse controls
- Move freely without camera shake or control issues

## 📋 What We Learned

### Game Architecture

**Schedule 1 Technical Details:**
- **Engine:** Unity (Il2Cpp build)
- **Modding Framework:** MelonLoader
- **Player Object:** `AnubISS` (CharacterController component)
- **Camera Structure:** Main camera is child of `CameraContainer`
- **Avatar View:** Built-in system activated by holding V key (shows character from front)
- **Avatar Camera:** `OverlayCamera` (discovered through testing)

### Key Discoveries

1. **Player Detection:**
   - Player object is at a valid world position (not origin)
   - Has CharacterController component
   - Named "AnubISS" in game

2. **Camera System:**
   - Main camera: Used for first-person view and player input
   - OverlayCamera: Used for avatar view (V key)
   - Camera hierarchy: `CameraContainer > Main Camera`

3. **Input System:**
   - Schedule 1 does NOT use Unity's standard `Input.GetKey()` for V key
   - Likely uses new Unity Input System or custom input handling
   - Harmony patches on Input methods have no effect

4. **Critical Issues:**
   - Disabling main camera breaks player movement
   - Creating new camera causes shake/jitter (conflicts with game's camera controller)
   - Using camera depth to layer cameras doesn't prevent input issues
   - Direct camera positioning causes visual shake

## 🔧 Approaches Attempted

### Approach 1: Create New Third-Person Camera
**Method:** Create independent camera, position behind player, disable main camera

**Result:** ❌ Camera shake, player can't move

**Code:** `CameraController.cs`

### Approach 2: Modify Existing Camera
**Method:** Reposition main camera directly without creating new one

**Result:** ❌ Severe camera shake (game's camera controller fights our positioning)

**Code:** `CameraController_v2.cs`

### Approach 3: Disable Game's Camera Scripts
**Method:** Find and disable MonoBehaviour scripts controlling camera

**Result:** ❌ Made shake worse, broke more functionality

**Code:** `CameraControllerFinal.cs`

### Approach 4: Hijack V Key (Input Patching)
**Method:** Use Harmony to patch `Input.GetKey()` to fake holding V

**Result:** ❌ Game doesn't use `Input.GetKey()` for V key

**Code:** `VKeyPatch.cs`, `VKeyPatchDiagnostic.cs`

### Approach 5: Hijack Avatar Camera
**Method:** Find OverlayCamera, keep it enabled, control it directly

**Result:** ⚠️ Camera found and activated, but player can't move

**Code:** `AvatarCameraHijack.cs` (most promising approach)

## 🚧 Technical Blockers

### 1. Camera-Input Coupling
**Problem:** Schedule 1's player input system is tightly coupled to the main camera. Disabling or deprioritizing the main camera breaks player movement.

**Evidence:**
- Player can move in first-person (main camera active)
- Player cannot move when main camera disabled
- Player cannot move when avatar camera is primary

**Potential Solutions:**
- Find and patch the input handler to decouple from camera
- Discover game's actual input system and work with it
- Keep main camera fully active but hide its rendering

### 2. Camera Controller Conflicts
**Problem:** Schedule 1 has active camera controller scripts that continuously reposition the camera, fighting any external positioning.

**Evidence:**
- Direct camera positioning causes jitter/shake
- Camera "snaps back" to game's desired position every frame
- Smooth interpolation doesn't help

**Potential Solutions:**
- Identify and disable specific camera controller component (not just all MonoBehaviours)
- Use Harmony to patch camera update methods
- Work with game's camera system instead of against it

### 3. Input System Mystery
**Problem:** V key detection doesn't use Unity's standard Input class.

**Evidence:**
- Harmony patches on `Input.GetKey()` show no calls for V key
- V key functionality works but isn't detectable through standard methods

**Potential Solutions:**
- Decompile game code to find actual input handling
- Find InputAction assets or new Input System usage
- Reverse engineer V key handler

## 📁 File Structure

```
src/ScheduleOneMods.ThirdPersonCamera/
├── Mod.cs                          # Main mod entry point
├── CameraController.cs             # Approach 1: New camera
├── CameraController_v2.cs          # Approach 2: Modify existing
├── CameraControllerFinal.cs        # Approach 3: Disable scripts
├── CameraControllerV9.cs           # Approach 4: V key simulation
├── VKeyPatch.cs                    # Approach 4: Input patching
├── VKeyPatchDiagnostic.cs          # Approach 4: Diagnostic version
├── AvatarCameraHijack.cs           # Approach 5: Hijack avatar camera (BEST)
└── README.md                       # This file
```

## 🎮 Current Best Attempt

**File:** `AvatarCameraHijack.cs`

**How to Use:**
1. Build the mod: `dotnet build -c Release`
2. Copy DLL to `Schedule I/Mods/`
3. Launch game
4. Press V to activate avatar view
5. Press F6 to lock camera on
6. (Player cannot move - this is the blocker)

**What Works:**
- ✅ Finds player (AnubISS)
- ✅ Finds avatar camera (OverlayCamera)
- ✅ Activates third-person view
- ✅ Shows character from behind

**What Doesn't Work:**
- ❌ Player movement (WASD doesn't work)
- ❌ Camera rotation (right-click doesn't rotate)

## 🔬 Research Data

### Console Output Examples

**Successful Player Detection:**
```
[Third_Person_Camera] Found player via CharacterController: AnubISS at (-99.85, 44.05, -186.67)
```

**Successful Camera Detection:**
```
[Third_Person_Camera] Found potential avatar camera: OverlayCamera
[Third_Person_Camera] FOUND ACTIVE CAMERA: OverlayCamera
```

**Camera Activation:**
```
[Third_Person_Camera] Third person enabled with avatar camera: OverlayCamera
```

### Camera Hierarchy
```
Main Camera (depth: 0)
  └─ CameraContainer
      └─ [Player/Character object with CharacterController]
```

## 💡 Recommendations for Future Work

### Short Term
1. **Decompile Schedule 1** using Il2CppDumper to understand:
   - Actual input system implementation
   - Camera controller component names
   - Player movement code

2. **Test camera depth approach more thoroughly:**
   - Try different depth values
   - Test with both cameras at different render settings
   - Experiment with camera culling masks

3. **Find the specific camera controller:**
   - Log all component types on camera objects
   - Disable components one by one to find the culprit

### Long Term
1. **Wait for official mod support** - Game developers might add proper camera API
2. **Community collaboration** - Share findings with other Schedule 1 modders
3. **Alternative approach** - Create a "photo mode" instead of live third-person

## 🤝 Contributing

If you want to continue this work:

1. **Start with `AvatarCameraHijack.cs`** - It's the closest to working
2. **Focus on the movement issue** - Camera works, movement doesn't
3. **Decompile the game** - Understanding the actual code is crucial
4. **Join Schedule 1 modding community** - Others may have insights

## 📚 References

- [MelonLoader Documentation](https://melonwiki.xyz/)
- [HarmonyX Documentation](https://github.com/BepInEx/HarmonyX/wiki)
- [Unity Camera System](https://docs.unity3d.com/Manual/class-Camera.html)
- [Unity Input System](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/manual/index.html)

## 📝 License

This mod is experimental and provided as-is for educational and research purposes.

## 🙏 Acknowledgments

Research and development by anubissbe with assistance from AI pair programming.

---

**Last Updated:** October 2025  
**Game Version:** Schedule I 0.4.0f8  
**MelonLoader Version:** 0.7.1
