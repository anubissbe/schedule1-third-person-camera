# API Reference

Quick reference for the mod's code structure. This is probably more detailed than needed for such a small project, but hey, maybe it'll help someone who wants to fork this and try their own approach.

## Core Components

### Mod Entry Point

#### `Mod` class
**Namespace:** `ScheduleOneMods.ThirdPersonCamera`
**Inherits:** `MelonMod` (MelonLoader base class)

MelonLoader entry point that initializes the mod and forwards Unity lifecycle events to the camera controller.

**Methods:**

##### `OnInitializeMelon()`
```csharp
public override void OnInitializeMelon()
```
Called once when the mod is loaded by MelonLoader. Initializes logging and displays usage instructions to console.

**Lifecycle:** Called once at mod initialization
**Side Effects:** Initializes `Log` system, writes instructions to MelonLoader console

##### `OnUpdate()`
```csharp
public override void OnUpdate()
```
Called every frame by MelonLoader. Forwards to `AvatarCameraHijack.Update()` for input handling.

**Lifecycle:** Called every frame (~60Hz)
**Forwards to:** `AvatarCameraHijack.Update()`

##### `OnLateUpdate()`
```csharp
public override void OnLateUpdate()
```
Called every frame after all `OnUpdate()` calls. Forwards to `AvatarCameraHijack.LateUpdate()` for camera positioning.

**Lifecycle:** Called every frame after Update
**Forwards to:** `AvatarCameraHijack.LateUpdate()`
**Purpose:** Runs after game's camera controller to minimize conflicts

---

### Camera Controller

#### `AvatarCameraHijack` class
**Namespace:** `ScheduleOneMods.ThirdPersonCamera`
**Type:** `static class`

Core camera control logic. Hijacks Schedule 1's built-in avatar camera (OverlayCamera) to provide third-person view.

**State Fields:**

| Field | Type | Description |
|-------|------|-------------|
| `_avatarCamera` | `Camera?` | Hijacked avatar camera (OverlayCamera) used for third-person rendering |
| `_mainCamera` | `Camera?` | Game's main first-person camera (must stay enabled for input) |
| `_player` | `GameObject?` | Player GameObject (AnubISS with CharacterController) |
| `_isThirdPersonEnabled` | `bool` | Whether third-person mode is currently active |
| `_isSearchingForCamera` | `bool` | Whether mod is in F8 debug mode searching for cameras |
| `_lastToggleTime` | `float` | Time of last F6 toggle (for cooldown) |
| `_horizontalAngle` | `float` | Current horizontal camera rotation in degrees |
| `_verticalAngle` | `float` | Current vertical camera rotation in degrees (clamped -10° to 60°) |

**Configuration Constants:**

| Constant | Value | Description |
|----------|-------|-------------|
| `ToggleCooldown` | `0.5f` | Cooldown between F6 presses in seconds |
| `CameraDistance` | `3.5f` | Distance from player pivot in Unity units |
| `MouseSensitivity` | `3f` | Mouse sensitivity multiplier for rotation |

**Public Methods:**

##### `Update()`
```csharp
public static void Update()
```
Handles keyboard input detection every frame.

**Inputs Handled:**
- **F6**: Toggle third-person mode (with cooldown)
- **F8**: Enable debug camera search mode

**Called by:** `Mod.OnUpdate()`
**Frequency:** Every frame

##### `LateUpdate()`
```csharp
public static void LateUpdate()
```
Updates camera position and rotation when third-person is active.

**Behavior:**
- Only runs when `_isThirdPersonEnabled == true`
- Calls `UpdateAvatarCamera()` to position camera
- Runs after game's camera controller (minimizes conflicts)

**Called by:** `Mod.OnLateUpdate()`
**Frequency:** Every frame
**Blockers:** Subject to camera shake from game's controller (Blocker #2)

**Private Methods:**

##### `ToggleThirdPerson()`
```csharp
private static void ToggleThirdPerson()
```
Toggles third-person mode on/off.

**Behavior:**
- Inverts `_isThirdPersonEnabled` flag
- Calls `EnableThirdPerson()` or `DisableThirdPerson()`
- Logs state change to console

**Called by:** `Update()` on F6 press

##### `EnableThirdPerson()`
```csharp
private static void EnableThirdPerson()
```
Activates third-person mode by finding player/cameras and configuring rendering.

**Process:**
1. Find main camera via `Camera.main`
2. Find player via `FindPlayer()`
3. Find avatar camera via `FindAvatarCamera()`
4. Enable avatar camera and set depth to 10 (renders on top)
5. Set main camera depth to -1 (renders first, but overlaid)
6. Initialize camera angle behind player

**Critical Notes:**
- Does NOT disable main camera (Blocker #1: breaks player input)
- Uses camera depth layering for rendering order
- Fails gracefully if player or camera not found

**Error Handling:**
- Player not found → disable mode, log error
- Camera not found → disable mode, log instructions

##### `DisableThirdPerson()`
```csharp
private static void DisableThirdPerson()
```
Deactivates third-person mode and restores first-person camera.

**Process:**
1. Restore main camera depth to 0 (default)
2. Disable avatar camera
3. Deactivate avatar camera GameObject

##### `FindAvatarCamera()`
```csharp
private static Camera? FindAvatarCamera()
```
Searches for Schedule 1's avatar camera (OverlayCamera).

**Returns:** `Camera?` - Avatar camera if found, null otherwise

**Search Strategy:**
1. Get all cameras including inactive via `FindObjectsOfType<Camera>(true)`
2. Prioritize cameras with keywords: "avatar", "third", "view", "character", "secondary", "overlay"
3. Fallback to first non-main camera
4. Return null if no candidates found

**Known Result:** Finds "OverlayCamera" in Schedule 1

##### `SearchForAvatarCamera()`
```csharp
private static void SearchForAvatarCamera()
```
Debug mode (F8): Actively searches for cameras that become enabled.

**Purpose:** Helps identify avatar camera when user presses V key

**Behavior:**
- Scans all cameras every frame while `_isSearchingForCamera == true`
- Detects newly enabled cameras
- Captures first enabled non-main camera as `_avatarCamera`
- Exits search mode and logs success

**Usage Pattern:**
1. User presses F8 (starts search)
2. User presses V in-game (activates OverlayCamera)
3. Mod detects OverlayCamera and captures it

##### `FindPlayer()`
```csharp
private static GameObject? FindPlayer()
```
Finds player GameObject by searching for CharacterController components.

**Returns:** `GameObject?` - Player GameObject if found, null otherwise

**Search Strategy:**
1. Get all CharacterController components
2. Find first controller with position != Vector3.zero (valid world position)
3. Return associated GameObject

**Known Result:** Finds "AnubISS" GameObject in Schedule 1

##### `UpdateAvatarCamera()`
```csharp
private static void UpdateAvatarCamera()
```
Updates camera position and rotation every frame when third-person is active.

**Inputs:**
- Right mouse button: Rotate camera
- Mouse X axis: Horizontal rotation
- Mouse Y axis: Vertical rotation (clamped -10° to 60°)

**Camera Positioning:**
1. Calculate rotation from `_horizontalAngle` and `_verticalAngle`
2. Calculate offset using spherical coordinates
3. Position camera at offset from pivot point (player + 1.5 units up)
4. Look at pivot point

**Blockers:**
- Camera shake occurs due to game's camera controller conflict (Blocker #2)
- Player movement disabled when main camera deprioritized (Blocker #1)

---

## Key Bindings

| Key | Function | Cooldown | Effect |
|-----|----------|----------|--------|
| F6 | Toggle third-person | 0.5s | Enable/disable third-person camera mode |
| F8 | Debug camera search | None | Activate camera detection mode (press V after) |
| V | Avatar view (game) | None | Activates OverlayCamera (game feature, not mod) |
| Right Mouse + Drag | Rotate camera | None | Control camera angle when third-person active |

---

## Dependencies

### External Libraries
- **MelonLoader**: Mod framework providing `MelonMod` base class and Unity hooks
- **HarmonyX** 2.14.0: Runtime patching library (imported but not currently used)
- **UnityEngine.CoreModule**: Core Unity types (Camera, GameObject, Transform, etc.)
- **UnityEngine.PhysicsModule**: CharacterController component
- **UnityEngine.InputLegacyModule**: Input class for keyboard/mouse

### Internal References
- **ScheduleOneMods.Logging**: Logging abstraction (`Log.Info()`, `Log.Error()`, etc.)

### Game References
- **Assembly-CSharp.dll**: Schedule 1 game code (Il2Cpp)
- **Il2Cppmscorlib.dll**: Il2Cpp runtime

---

## Known Technical Blockers

### Blocker #1: Camera-Input Coupling (CRITICAL)
**Impact:** Player can't move when main camera is disabled
**Workaround:** Keep main camera enabled, use depth layering
**Status:** Partial workaround (camera shows but movement still broken)

### Blocker #2: Camera Controller Conflicts (HIGH)
**Impact:** Game's camera controller causes shake/jitter
**Affected Method:** `UpdateAvatarCamera()`
**Status:** No workaround found

### Blocker #3: Input System Abstraction (MEDIUM)
**Impact:** Cannot intercept V key (avatar view) via `Input.GetKey()` patches
**Workaround:** Use F8 debug mode to detect camera activation
**Status:** Workaround available

**See:** `TECHNICAL_BLOCKERS.md` for detailed analysis

---

## Build Configuration

### Debug vs Release
**Debug:**
- Uses project reference to `../ScheduleOneMods.Logging/`
- Full logging output

**Release:**
- Links `Log.cs` as compile item
- Optimized build

### MelonLoader Attributes
Injected via MSBuild targets in `.csproj`:

```csharp
[assembly: MelonInfo(typeof(Mod), "Third Person Camera", "0.1.0", "anubissbe")]
[assembly: MelonGame("TVGS", "Schedule I")]
```

---

## Extension Points

### Camera Configuration
Adjust constants to modify camera behavior:
```csharp
private static readonly float CameraDistance = 3.5f;    // Distance from player
private static readonly float MouseSensitivity = 3f;     // Rotation speed
```

### Vertical Angle Limits
Modify clamp range in `UpdateAvatarCamera()`:
```csharp
_verticalAngle = Mathf.Clamp(_verticalAngle, -10f, 60f);
```

### Camera Search Keywords
Add keywords to `FindAvatarCamera()`:
```csharp
if (name.Contains("avatar") || name.Contains("third") || name.Contains("yourcustomkeyword"))
```

### Input Bindings
Modify key codes in `Update()`:
```csharp
if (Input.GetKeyDown(KeyCode.YourKey))
```

---

## Logging

### Log Levels
- `Log.Info()`: General information and state changes
- `Log.Warning()`: Non-critical issues (camera not found)
- `Log.Error()`: Critical failures (player not found)
- `Log.Debug()`: Detailed diagnostic information

### Log Output
All logs appear in MelonLoader console with `[Third_Person_Camera]` prefix.

**Example:**
```
[Third_Person_Camera] Third Person Camera mod initialized (FINAL - Avatar Camera Hijack)!
[Third_Person_Camera] Found player: AnubISS
[Third_Person_Camera] FOUND ACTIVE CAMERA: OverlayCamera
[Third_Person_Camera] Third person mode: ENABLED
```

---

## Performance Characteristics

| Operation | Frequency | Cost | Notes |
|-----------|-----------|------|-------|
| `Update()` | Every frame | Low | Minimal input checking |
| `LateUpdate()` | Every frame | Medium | Position/rotation calculation |
| `FindPlayer()` | On enable | Medium | Searches all CharacterControllers |
| `FindAvatarCamera()` | On enable | High | Searches all cameras including inactive |
| `SearchForAvatarCamera()` | Every frame (F8 mode) | High | Full camera scan each frame |

**Optimization Notes:**
- Player and camera references cached after first find
- Search operations only run on mode enable or F8 debug
- Camera update only runs when mode is active

---

## Version History

**v0.1.0** - Initial implementation
- Avatar camera hijacking approach
- F6 toggle, F8 debug mode
- Mouse rotation support
- Known blockers documented

**See:** `CHANGELOG.md` for detailed version history
