# Development Log - Third Person Camera Mod

This document chronicles the development process, iterations, and lessons learned while attempting to create a third-person camera mod for Schedule 1.

## Timeline

### Version 1: Initial Implementation
**Date:** October 3, 2025  
**Approach:** Create new camera, position behind player  
**Result:** Compilation error - ambiguous `Object` reference  

**Issue:**
```
error CS0118: 'Object' is ambiguous between 'UnityEngine.Object' and 'object'
```

**Fix:** Use fully qualified `UnityEngine.Object` names

---

### Version 2: Fixed Compilation
**Approach:** Same as v1, with namespace fixes  
**Result:** ❌ Player at origin (0,0,0) - wrong object detected  

**Console Output:**
```
Player found: Character at position (0.00, 0.00, 0.00)
Player is at origin (0,0,0), this might be incorrect
```

**Learning:** The "Character" object at origin is a prefab/template, not the actual player

---

### Version 3: Ignore Origin Objects
**Approach:** Skip objects at (0,0,0), prioritize camera parent  
**Result:** ❌ Still found wrong object, camera at sky view  

**Issue:** Camera positioned at world origin, showing sky instead of player

---

### Version 4: Camera Parent Detection
**Approach:** Use camera's parent as player (common Unity pattern)  
**Result:** ⚠️ Found "CameraContainer" but it's not the player  

**Console Output:**
```
Found player as camera parent: CameraContainer at (-49.85, 44.81, -186.67)
Third person camera enabled successfully
```

**Issue:** Camera following camera container, not actual player body

---

### Version 5: Stability Fixes
**Approach:** Remove interpolation, fix update methods  
**Result:** ❌ Still shaking, camera zooming continuously  

**Changes:**
- Removed dual updates (Update + LateUpdate conflict)
- Added toggle cooldown
- Removed auto-follow rotation
- Increased smooth speed

**Issue:** Camera kept zooming in, screen shaking, unplayable

---

### Version 6: Direct Positioning
**Approach:** Remove all smoothing, use direct position updates  
**Result:** ❌ Still shaking  

**Changes:**
- Removed `Vector3.Lerp`
- Direct position assignment
- Better vertical angle (10° instead of 20°)

**Issue:** Shake persists even with direct positioning - indicates game's camera controller is fighting us

---

### Version 7: Simplified Approach
**Approach:** Work with existing camera instead of creating new one  
**Result:** ❌ Still shaking  

**Realization:** Creating new camera vs. modifying existing doesn't matter - the shake comes from Schedule 1's camera controller

---

### Version 8: Disable Game Scripts
**Approach:** Find and disable all camera control scripts  
**Result:** ❌ Made it worse  

**Changes:**
- Disabled all MonoBehaviour components on camera
- Disabled scripts on camera parent
- Took "nuclear option" of complete control

**Issue:** Disabling all scripts broke more than it fixed

---

### Version 9: Hijack V Key
**Approach:** Patch `Input.GetKey()` to simulate holding V  
**Result:** ❌ Game doesn't use `Input.GetKey()`  

**Discovery:** Schedule 1 uses a different input system

**Console Output:**
```
Patched Input.GetKey
Patched Input.GetKeyDown  
Patched Input.GetKeyUp
```

**Test:** Held V manually - no diagnostic messages appeared

**Conclusion:** Schedule 1 doesn't use Unity's standard Input class for V key

---

### Version 10: Diagnostic Mode
**Approach:** Log all Input method calls to understand V key handling  
**Result:** ✅ Confirmed game doesn't use `Input.GetKey()`  

**Evidence:** No messages when holding V, despite patches being active

---

### Version 11: Avatar Camera Hijack
**Approach:** Find and control Schedule 1's existing avatar camera  
**Result:** ⚠️ **Partial success** - Camera works but player can't move  

**Breakthrough:**
```
Found player via CharacterController: AnubISS (765611980072343461)
Found potential avatar camera: OverlayCamera
Third person enabled with avatar camera: OverlayCamera
```

**Success:**
- ✅ Found actual player object: "AnubISS"
- ✅ Found avatar camera: "OverlayCamera"
- ✅ Camera shows third-person view
- ✅ Character visible from behind

**Blocker:**
- ❌ Player cannot move (WASD doesn't work)
- ❌ Camera rotation doesn't work

**Hypothesis:** Disabling main camera breaks input system

---

### Version 12: Movement Fix (Camera Depth)
**Approach:** Use camera depth instead of disabling main camera  
**Result:** ❌ Player still can't move  

**Changes:**
```csharp
_mainCamera.depth = -1;  // Keep enabled but render first
_avatarCamera.depth = 10; // Render on top
```

**Issue:** Even with main camera enabled, player movement doesn't work when avatar camera is active

**Conclusion:** Input system is coupled to camera in a way we don't understand

---

## Key Findings

### What Works
1. ✅ **Player Detection:** Successfully found "AnubISS" object with CharacterController
2. ✅ **Camera Detection:** Successfully found "OverlayCamera" (avatar view camera)
3. ✅ **Camera Activation:** Can activate and position third-person camera
4. ✅ **Visual Display:** Third-person view renders correctly

### What Doesn't Work
1. ❌ **Player Movement:** Cannot move when third-person is active
2. ❌ **Camera Stability:** Shake/jitter when using custom camera positioning
3. ❌ **Input Hijacking:** Cannot fake V key press (game uses different input system)
4. ❌ **Script Disabling:** Disabling camera scripts breaks more than it fixes

### Root Causes

**Problem 1: Camera-Input Coupling**
- Schedule 1's input system requires main camera to be active and primary
- Disabling or deprioritizing main camera breaks WASD movement
- Unknown coupling mechanism (not obvious from Unity's standard systems)

**Problem 2: Camera Controller Conflicts**
- Game has active camera controller that fights external positioning
- Creates shake/jitter when we try to position camera
- Cannot identify specific controller component to disable

**Problem 3: Input System Abstraction**
- Game doesn't use `Input.GetKey()` for V key
- Likely uses Unity's new Input System or custom implementation
- Cannot intercept or simulate input without knowing the system

## Lessons Learned

### Technical Insights

1. **Il2Cpp Modding is Hard**
   - Limited reflection capabilities
   - Can't easily inspect component types
   - Harmony patching has limitations

2. **Unity Camera Systems are Complex**
   - Multiple cameras can conflict
   - Camera depth doesn't solve all rendering issues
   - Input systems can be tightly coupled to cameras

3. **Game-Specific Knowledge is Crucial**
   - Need to decompile to understand actual implementation
   - Generic Unity approaches may not work
   - Each game has unique architecture

### Modding Strategies

**What Worked:**
- Iterative testing with diagnostic logging
- Finding game objects through multiple methods
- Using Harmony for method patching
- Documenting failures to avoid repeating them

**What Didn't Work:**
- Assuming standard Unity patterns
- Fighting game's systems instead of working with them
- Trying to fix issues without understanding root cause

## Recommendations

### For Continuing This Work

1. **Decompile the Game**
   - Use Il2CppDumper or similar tools
   - Find actual camera controller class names
   - Understand input system implementation
   - Locate player movement code

2. **Focus on Input System**
   - The camera works, movement doesn't
   - Fix input coupling before improving camera
   - May need to patch player movement methods

3. **Community Collaboration**
   - Share findings with other Schedule 1 modders
   - Someone may have solved similar issues
   - Pool knowledge about game's architecture

### Alternative Approaches

1. **Photo Mode Instead**
   - Freeze game, allow camera movement
   - Simpler than live third-person
   - Still useful for players

2. **Wait for Official Support**
   - Game developers might add camera API
   - Official mod support would solve coupling issues

3. **Replay System**
   - Record player actions, replay with free camera
   - More complex but avoids input issues

## Statistics

- **Total Versions:** 12
- **Approaches Tried:** 5 major approaches
- **Development Time:** ~4 hours
- **Lines of Code Written:** ~2000+
- **Success Rate:** Partial (camera works, movement doesn't)

## Conclusion

While we successfully demonstrated that:
- Third-person camera rendering is possible
- Player and camera objects can be found
- Camera positioning works visually

We were unable to overcome the fundamental issue of player input being coupled to the camera system in an unknown way. This mod remains experimental and non-functional until that coupling can be understood and bypassed.

The research and code provided here should serve as a foundation for future attempts, either by the original developer or the community.

---

**Final Status:** Experimental / Non-Functional  
**Best Version:** v11/v12 (AvatarCameraHijack.cs)  
**Blocking Issue:** Player movement doesn't work in third-person mode
