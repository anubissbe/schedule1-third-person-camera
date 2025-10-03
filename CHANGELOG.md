# Changelog - Third Person Camera Mod

All notable changes and iterations of this mod are documented in this file.

## [Unreleased] - Experimental

### Current Status
⚠️ **Non-Functional** - Camera works but player movement is broken

---

## [v12] - 2025-10-03

### Changed
- Keep main camera enabled with lower depth instead of disabling
- Use camera depth layering to control rendering priority
- Main camera depth = -1, Avatar camera depth = 10

### Fixed
- Attempted to fix player movement by keeping main camera active

### Known Issues
- Player still cannot move in third-person mode
- Camera rotation doesn't work

---

## [v11] - 2025-10-03

### Added
- `AvatarCameraHijack.cs` - New approach to find and control avatar camera
- F8 key for manual camera search
- Comprehensive camera detection logging

### Changed
- Find OverlayCamera (Schedule 1's avatar view camera)
- Hijack existing camera instead of creating new one
- Disable main camera when third-person active

### Breakthrough
- ✅ Successfully found player object: "AnubISS"
- ✅ Successfully found avatar camera: "OverlayCamera"
- ✅ Third-person view renders correctly

### Known Issues
- Player cannot move (WASD doesn't work)
- Camera rotation doesn't work

---

## [v10] - 2025-10-03

### Added
- `VKeyPatchDiagnostic.cs` - Diagnostic version to understand V key handling
- F7 key to toggle diagnostic logging
- Comprehensive Input method logging

### Changed
- Patch GetKey, GetKeyDown, GetKeyUp for diagnostics
- Log every V key check attempt

### Discovery
- ❌ Schedule 1 does NOT use `Input.GetKey()` for V key
- Game likely uses Unity's new Input System or custom input

---

## [v9] - 2025-10-03

### Added
- `VKeyPatch.cs` - Harmony patches to hijack V key input
- `CameraControllerV9.cs` - V key simulation approach

### Approach
- Patch `Input.GetKey(KeyCode.V)` to return true when F6 toggled
- Simulate holding V key to activate avatar view

### Result
- ❌ Patches applied successfully but had no effect
- Game doesn't use standard Input methods

---

## [v8] - 2025-10-03

### Added
- `CameraControllerFinal.cs` - Aggressive script disabling

### Changed
- Disable ALL MonoBehaviour components on camera
- Disable scripts on camera parent and grandparent
- Take complete control of camera

### Result
- ❌ Made shake worse
- Broke more functionality than it fixed

---

## [v7] - 2025-10-03

### Added
- `CameraController_v2.cs` - Simplified approach

### Changed
- Work with existing camera instead of creating new one
- Direct position updates without interpolation
- Simpler code (< 200 lines)

### Result
- ❌ Still shaking
- Confirmed issue is game's camera controller, not our approach

---

## [v6] - 2025-10-03

### Changed
- Removed all smooth interpolation
- Use direct position assignment
- Better vertical angle (10° instead of 20°)

### Result
- ❌ Still shaking
- Indicates fundamental conflict with game's camera system

---

## [v5] - 2025-10-03

### Changed
- Remove dual update methods (Update + LateUpdate conflict)
- Add toggle cooldown (0.5s)
- Remove auto-follow rotation
- Increase smooth speed to 15x

### Fixed
- Attempted to fix rapid toggling
- Attempted to fix camera shake

### Known Issues
- Camera still shaking and zooming
- Unplayable

---

## [v4] - 2025-10-03

### Changed
- Use camera's parent as player object
- Better hierarchy detection

### Result
- ⚠️ Found "CameraContainer" but it's not the actual player
- Camera following wrong object

---

## [v3] - 2025-10-03

### Added
- Ignore objects at origin (0,0,0)
- Prioritize camera parent for player detection

### Fixed
- Skip prefab/template objects at world origin

### Known Issues
- Still finding wrong player object
- Camera positioned at sky

---

## [v2] - 2025-10-03

### Fixed
- Namespace conflict with `Object` type
- Use fully qualified `UnityEngine.Object` names

### Known Issues
- Player detected at origin (0,0,0) - wrong object

---

## [v1] - 2025-10-03

### Added
- Initial implementation
- `CameraController.cs` - Main camera control logic
- `Mod.cs` - MelonLoader entry point
- F6 key to toggle third-person
- Right-click to rotate camera
- Smooth camera following
- Collision detection

### Known Issues
- Compilation error: ambiguous `Object` reference

---

## Version Summary

| Version | Status | Key Feature | Main Issue |
|---------|--------|-------------|------------|
| v1 | ❌ | Initial implementation | Compilation error |
| v2 | ❌ | Fixed compilation | Wrong player object |
| v3 | ❌ | Ignore origin objects | Still wrong object |
| v4 | ❌ | Camera parent detection | Found container, not player |
| v5 | ❌ | Stability fixes | Shaking continues |
| v6 | ❌ | Direct positioning | Still shaking |
| v7 | ❌ | Simplified approach | Confirmed system conflict |
| v8 | ❌ | Disable all scripts | Made it worse |
| v9 | ❌ | Hijack V key | Game doesn't use Input.GetKey() |
| v10 | ✅ | Diagnostic mode | Confirmed input system difference |
| v11 | ⚠️ | Avatar camera hijack | Camera works, movement doesn't |
| v12 | ⚠️ | Camera depth fix | Still no movement |

---

## Statistics

- **Total Versions:** 12
- **Successful Compilations:** 11
- **Functional Versions:** 0
- **Partial Success:** v11, v12 (camera works, movement doesn't)
- **Development Time:** ~4 hours
- **Lines of Code:** ~2000+

---

## Future Versions

### Planned Features (if blockers resolved)
- [ ] Adjustable camera distance
- [ ] Adjustable camera height
- [ ] Multiple camera presets
- [ ] Smooth zoom in/out
- [ ] Collision-aware camera positioning
- [ ] Configuration file for settings
- [ ] Save camera preferences

### Required Before Next Version
- Decompile Schedule 1 to understand:
  - Camera controller implementation
  - Input system architecture
  - Player movement code
- Identify specific components to patch
- Find solution to camera-input coupling

---

**Last Updated:** October 3, 2025  
**Current Version:** v12 (Experimental)  
**Status:** Blocked pending game code analysis
