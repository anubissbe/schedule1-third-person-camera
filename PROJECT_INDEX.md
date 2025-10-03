# Project Index

Navigation guide for this project. I went a bit overboard with the documentation (there's more docs than actual code at this point lol), but at least it's well-organized.

**Status:** ⚠️ Doesn't work yet
**Version:** 0.1.0

---

## Quick Navigation

### 🚀 Getting Started
- **User Guide:** [README.md](#readme) - Project overview and usage instructions
- **Developer Setup:** [DEVELOPER_GUIDE.md](#developer-guide) → Quick Start section
- **Build Commands:** [CLAUDE.md](#claude) → Build Commands section

### 📖 Documentation by Purpose

| I want to... | Read this document |
|--------------|-------------------|
| **Understand the project** | [README.md](#readme) |
| **Start developing** | [DEVELOPER_GUIDE.md](#developer-guide) |
| **Configure Claude AI** | [CLAUDE.md](#claude) |
| **Look up API details** | [API_REFERENCE.md](#api-reference) |
| **Understand blockers** | [TECHNICAL_BLOCKERS.md](#technical-blockers) |
| **See development history** | [DEVELOPMENT_LOG.md](#development-log) |
| **Check version changes** | [CHANGELOG.md](#changelog) |

### 🎯 By Role

**End User:**
1. [README.md](#readme) - What this mod does and why it doesn't work
2. [CHANGELOG.md](#changelog) - Version history

**Developer:**
1. [DEVELOPER_GUIDE.md](#developer-guide) - Setup and development
2. [API_REFERENCE.md](#api-reference) - Code reference
3. [TECHNICAL_BLOCKERS.md](#technical-blockers) - Detailed blocker analysis

**AI Assistant (Claude):**
1. [CLAUDE.md](#claude) - Project context and instructions
2. [API_REFERENCE.md](#api-reference) - Code structure
3. [TECHNICAL_BLOCKERS.md](#technical-blockers) - Limitations

**Researcher:**
1. [TECHNICAL_BLOCKERS.md](#technical-blockers) - Problem analysis
2. [DEVELOPMENT_LOG.md](#development-log) - Attempt history
3. [API_REFERENCE.md](#api-reference) → Code Files section

---

## Project Structure

### 📂 Directory Layout

```
ScheduleOneMods.ThirdPersonCamera/
│
├── 📋 Project Files
│   └── ScheduleOneMods.ThirdPersonCamera.csproj
│
├── 🎯 Active Code (what's actually being used)
│   ├── Mod.cs                      - MelonLoader entry point
│   └── AvatarCameraHijack.cs       - The camera hijacking logic
│
├── 🗄️ Failed Attempts (kept for reference)
│   ├── CameraController.cs         - Tried creating new camera
│   ├── CameraController_v2.cs      - Tried modifying main camera
│   ├── CameraControllerFinal.cs    - Tried disabling scripts
│   ├── CameraControllerV9.cs       - Tried simulating V key
│   ├── VKeyPatch.cs                - Tried HarmonyX input patching
│   ├── VKeyPatchDiagnostic.cs      - More input patching attempts
│   └── Mod_v2.cs                   - Alternative entry point
│
└── 📚 Documentation (probably too much tbh)
    ├── README.md                   - Start here
    ├── CLAUDE.md                   - Notes for Claude AI
    ├── DEVELOPER_GUIDE.md          - If you want to work on this
    ├── API_REFERENCE.md            - Code reference
    ├── TECHNICAL_BLOCKERS.md       - Why this doesn't work
    ├── DEVELOPMENT_LOG.md          - What I tried
    ├── CHANGELOG.md                - Version history
    └── PROJECT_INDEX.md            - You are here
```

---

## File Reference

### <a name="readme"></a> 📘 README.md
**Purpose:** User-facing project overview and research documentation
**Audience:** End users, researchers, modders
**Length:** ~240 lines

**Key Sections:**
- ✅ Project goal and current status
- ✅ What was learned about Schedule 1 architecture
- ✅ All 5 approaches attempted with results
- ✅ Three technical blockers explained
- ✅ Recommendations for future work

**Use this when:**
- Learning what this mod tries to do
- Understanding why it doesn't work
- Researching Schedule 1 modding challenges
- Deciding whether to continue development

**Cross-references:**
- → [TECHNICAL_BLOCKERS.md](#technical-blockers) for detailed blocker analysis
- → [DEVELOPMENT_LOG.md](#development-log) for chronological development
- → [AvatarCameraHijack.cs](#avatarcamerahijack) for current implementation

---

### <a name="claude"></a> 📘 CLAUDE.md
**Purpose:** Instructions for Claude Code AI assistant
**Audience:** Claude AI, developers using Claude
**Length:** ~195 lines

**Key Sections:**
- ✅ Build commands and deployment
- ✅ Technical stack overview
- ✅ Architecture with MelonLoader lifecycle
- ✅ Camera system approaches comparison
- ✅ Critical technical blockers summary
- ✅ Game discoveries (player: "AnubISS", camera: "OverlayCamera")
- ✅ Development notes and constraints
- ✅ Testing workflow

**Use this when:**
- Onboarding Claude to the project
- Quick reference for build commands
- Understanding project constraints
- Looking up game-specific discoveries

**Cross-references:**
- → [TECHNICAL_BLOCKERS.md](#technical-blockers) for blocker details
- → [README.md](#readme) for user context
- → [API_REFERENCE.md](#api-reference) for code details

---

### <a name="developer-guide"></a> 📗 DEVELOPER_GUIDE.md
**Purpose:** Comprehensive developer onboarding and tutorials
**Audience:** Developers extending or learning from the mod
**Length:** ~650 lines

**Key Sections:**
- ✅ Quick start setup (prerequisites, build, deploy)
- ✅ Architecture deep dive with execution flow
- ✅ State machine diagrams
- ✅ Modifying camera behavior (distance, sensitivity, angles)
- ✅ Adding new features (zoom, smoothing, custom keys, offset modes)
- ✅ Debugging guide with practical solutions
- ✅ Working around blockers (render textures, culling masks, HarmonyX)
- ✅ Testing checklist
- ✅ Performance optimization
- ✅ Advanced topics (Il2Cpp, HarmonyX integration)
- ✅ Contributing guidelines
- ✅ FAQ

**Use this when:**
- Setting up development environment
- Adding new features
- Debugging issues
- Understanding execution flow
- Learning Il2Cpp modding patterns
- Optimizing performance

**Cross-references:**
- → [API_REFERENCE.md](#api-reference) for method signatures
- → [TECHNICAL_BLOCKERS.md](#technical-blockers) for blocker workarounds
- → [AvatarCameraHijack.cs](#avatarcamerahijack) for code implementation

---

### <a name="api-reference"></a> 📗 API_REFERENCE.md
**Purpose:** Complete API documentation for all classes and methods
**Audience:** Developers, AI assistants
**Length:** ~500 lines

**Key Sections:**
- ✅ `Mod` class lifecycle methods
- ✅ `AvatarCameraHijack` class complete reference
  - State fields with types and descriptions
  - Configuration constants
  - Public methods (`Update()`, `LateUpdate()`)
  - Private methods (all 8 documented)
- ✅ Key bindings table
- ✅ Dependencies (external, internal, game)
- ✅ Technical blockers summary
- ✅ Build configuration (Debug vs Release)
- ✅ Extension points
- ✅ Logging system
- ✅ Performance characteristics

**Use this when:**
- Looking up method signatures
- Understanding state management
- Finding configuration constants
- Checking dependencies
- Identifying extension points
- Analyzing performance

**Cross-references:**
- → [DEVELOPER_GUIDE.md](#developer-guide) for usage examples
- → [AvatarCameraHijack.cs](#avatarcamerahijack) for implementation
- → [TECHNICAL_BLOCKERS.md](#technical-blockers) for blocker impact

---

### <a name="technical-blockers"></a> 📕 TECHNICAL_BLOCKERS.md
**Purpose:** Detailed technical analysis of three critical blockers
**Audience:** Developers, researchers, problem solvers
**Length:** ~365 lines

**Key Sections:**
- ✅ Blocker #1: Camera-Input Coupling (CRITICAL)
  - Evidence from 3 tests
  - Analysis of coupling mechanism
  - 3 potential solutions with code
- ✅ Blocker #2: Camera Controller Conflicts (HIGH)
  - Evidence from 3 tests
  - Shake pattern analysis
  - 3 potential solutions with code
- ✅ Blocker #3: Input System Abstraction (MEDIUM)
  - Evidence from diagnostic tests
  - Analysis of Unity's new Input System
  - 3 potential solutions with code
- ✅ Combined impact diagram
- ✅ Severity assessment
- ✅ Research needed (decompilation, component analysis, input discovery)
- ✅ Conclusion and next steps

**Use this when:**
- Understanding why the mod doesn't work
- Planning blocker resolution research
- Decompiling game code (knows what to look for)
- Implementing workarounds
- Evaluating project feasibility

**Cross-references:**
- → [README.md](#readme) for blocker overview
- → [DEVELOPER_GUIDE.md](#developer-guide) for workaround implementations
- → [API_REFERENCE.md](#api-reference) for affected methods

---

### <a name="development-log"></a> 📕 DEVELOPMENT_LOG.md
**Purpose:** Chronological development history and decision log
**Audience:** Researchers, future developers
**Length:** Varies (project-specific)

**Expected Sections:**
- Development timeline
- Approach iterations
- Key decisions and rationale
- Lessons learned

**Use this when:**
- Understanding development progression
- Learning from past mistakes
- Researching modding patterns

**Cross-references:**
- → [README.md](#readme) for approach summaries
- → [TECHNICAL_BLOCKERS.md](#technical-blockers) for why approaches failed

---

### <a name="changelog"></a> 📙 CHANGELOG.md
**Purpose:** Version history and change tracking
**Audience:** Users, developers
**Length:** ~200 lines

**Key Sections:**
- ✅ Version entries with dates
- ✅ Added/changed/fixed/removed categories
- ✅ Notable discoveries and blockers

**Use this when:**
- Checking version history
- Understanding what changed between versions
- Planning upgrades

**Cross-references:**
- → [README.md](#readme) for current status
- → [API_REFERENCE.md](#api-reference) for version details

---

## Code Reference

### <a name="mod"></a> 🎯 Mod.cs (Active)
**Purpose:** MelonLoader entry point
**Status:** ✅ Active - currently used
**Lines:** ~30
**Namespace:** `ScheduleOneMods.ThirdPersonCamera`

**Class:** `Mod : MelonMod`
**Methods:**
- `OnInitializeMelon()` - Initialize logging, display instructions
- `OnUpdate()` - Forward to `AvatarCameraHijack.Update()`
- `OnLateUpdate()` - Forward to `AvatarCameraHijack.LateUpdate()`

**Dependencies:**
- MelonLoader (MelonMod base class)
- ScheduleOneMods.Logging (Log system)
- AvatarCameraHijack (camera logic)

**Documentation:**
- API details: [API_REFERENCE.md](#api-reference) → Mod Entry Point
- Usage: [DEVELOPER_GUIDE.md](#developer-guide) → Architecture

---

### <a name="avatarcamerahijack"></a> 🎯 AvatarCameraHijack.cs (Active)
**Purpose:** Core camera control logic - hijacks Schedule 1's avatar camera
**Status:** ✅ Active - current best approach
**Lines:** ~285
**Namespace:** `ScheduleOneMods.ThirdPersonCamera`

**Class:** `AvatarCameraHijack` (static)

**State Fields (9):**
- `_avatarCamera`, `_mainCamera`, `_player`
- `_isThirdPersonEnabled`, `_isSearchingForCamera`
- `_lastToggleTime`, `_horizontalAngle`, `_verticalAngle`

**Constants (3):**
- `ToggleCooldown = 0.5f`
- `CameraDistance = 3.5f`
- `MouseSensitivity = 3f`

**Public Methods (2):**
- `Update()` - Handle F6/F8 input
- `LateUpdate()` - Update camera position/rotation

**Private Methods (8):**
- `ToggleThirdPerson()`, `EnableThirdPerson()`, `DisableThirdPerson()`
- `FindAvatarCamera()`, `SearchForAvatarCamera()`, `FindPlayer()`
- `UpdateAvatarCamera()`

**Key Discoveries:**
- Player: "AnubISS" GameObject with CharacterController
- Avatar Camera: "OverlayCamera"
- V key activates avatar view (game feature)

**Known Issues:**
- Player movement broken (Blocker #1)
- Camera shake (Blocker #2)

**Documentation:**
- Complete API: [API_REFERENCE.md](#api-reference) → AvatarCameraHijack
- Inline docs: XML comments in file
- Tutorials: [DEVELOPER_GUIDE.md](#developer-guide) → Adding Features

---

### 🗄️ Archived Code Files (7)

These files are **preserved for research** but not actively used:

#### CameraController.cs
**Approach:** Create independent third-person camera
**Result:** ❌ Camera shake, player can't move
**Line Count:** ~470
**Key Insight:** Creating new camera conflicts with game's system

#### CameraController_v2.cs
**Approach:** Modify main camera directly
**Result:** ❌ Severe shake from game's controller
**Line Count:** ~150
**Key Insight:** Direct repositioning fights game's controller

#### CameraControllerFinal.cs
**Approach:** Disable game's camera scripts
**Result:** ❌ Made shake worse
**Line Count:** ~245
**Key Insight:** Disabling all MonoBehaviours breaks more than it fixes

#### CameraControllerV9.cs
**Approach:** Simulate V key press
**Result:** ❌ Game doesn't use Input.GetKey() for V
**Line Count:** ~110
**Key Insight:** Schedule 1 uses different input system

#### VKeyPatch.cs
**Approach:** HarmonyX patch Input.GetKey()
**Result:** ❌ No effect, V key not detected
**Line Count:** ~70
**Key Insight:** Confirmed game doesn't use Input class

#### VKeyPatchDiagnostic.cs
**Approach:** Diagnostic version of VKeyPatch
**Result:** ❌ Zero log messages, patch never triggered
**Line Count:** ~140
**Key Insight:** Definitively proved Input.GetKey() not used

#### Mod_v2.cs
**Approach:** Alternative mod entry point
**Status:** Unused
**Line Count:** ~20

**Documentation:**
- Approach summaries: [README.md](#readme) → Approaches Attempted
- Why they failed: [TECHNICAL_BLOCKERS.md](#technical-blockers)

---

## Build Configuration

### ScheduleOneMods.ThirdPersonCamera.csproj
**Format:** MSBuild project file
**Framework:** .NET 6.0
**Package:** HarmonyX 2.14.0

**Key Configuration:**
```xml
Debug:   Uses ProjectReference to ../ScheduleOneMods.Logging/
Release: Uses Compile link to ../ScheduleOneMods.Logging/Log.cs
```

**MelonLoader Attributes (injected via MSBuild):**
- MelonInfo: "Third Person Camera", v0.1.0, by "anubissbe"
- MelonGame: "TVGS", "Schedule I"

**Game References (../../lib/):**
- Assembly-CSharp.dll
- Il2Cppmscorlib.dll
- UnityEngine.CoreModule.dll
- UnityEngine.PhysicsModule.dll
- UnityEngine.InputLegacyModule.dll

**Documentation:**
- Build commands: [CLAUDE.md](#claude) → Build Commands
- Build details: [API_REFERENCE.md](#api-reference) → Build Configuration

---

## Technical Context

### Game: Schedule 1
- **Engine:** Unity (Il2Cpp build)
- **Version:** 0.4.0f8
- **Developer:** TVGS
- **Modding:** MelonLoader 0.7.1+

### Key Game Objects
| Object | Type | Purpose | Location |
|--------|------|---------|----------|
| AnubISS | GameObject | Player character | Has CharacterController |
| CameraContainer | GameObject | Camera parent | Hierarchy root |
| Main Camera | Camera | First-person view | depth=0, always enabled |
| OverlayCamera | Camera | Avatar view (V key) | depth varies, normally disabled |

### Three Critical Blockers

**🔴 Blocker #1: Camera-Input Coupling (CRITICAL)**
- Player movement stops when main camera disabled/deprioritized
- No workaround found
- Affects: All camera switching approaches

**🟡 Blocker #2: Camera Controller Conflicts (HIGH)**
- Game's camera controller causes shake/jitter
- Fights external camera positioning
- Affects: `UpdateAvatarCamera()` method

**🟡 Blocker #3: Input System Abstraction (MEDIUM)**
- Cannot intercept V key via Input.GetKey()
- Game uses different input system
- Workaround: F8 debug mode

**Complete Analysis:** [TECHNICAL_BLOCKERS.md](#technical-blockers)

---

## Development Workflow

### Standard Development Cycle
1. **Read** [DEVELOPER_GUIDE.md](#developer-guide) → Quick Start
2. **Make changes** to [AvatarCameraHijack.cs](#avatarcamerahijack)
3. **Build:** `dotnet build -c Debug`
4. **Deploy:** Copy DLL to `Schedule I/Mods/`
5. **Test:** Launch game, check MelonLoader console
6. **Debug:** Use [DEVELOPER_GUIDE.md](#developer-guide) → Debugging Guide

### Testing Checklist
- [ ] Build succeeds without warnings
- [ ] F6 toggles third-person
- [ ] F8 + V detects camera
- [ ] No new errors in console
- [ ] Known blockers still present (expected)

**Full Checklist:** [DEVELOPER_GUIDE.md](#developer-guide) → Testing

---

## Research Roadmap

### To Resolve Blockers
**Required:** Decompile Schedule 1 using Il2CppDumper

**Target Information:**
1. **Camera controller component names**
   - Find specific type to disable (not all MonoBehaviours)
   - Identify Update/LateUpdate methods to patch

2. **Input system implementation**
   - Locate V key handler
   - Find InputAction or custom input manager
   - Understand movement-camera coupling

3. **Player movement code**
   - Discover how input depends on camera
   - Find decoupling points

**Tools:**
- [Il2CppDumper](https://github.com/Perfare/Il2CppDumper)
- [dnSpy](https://github.com/dnSpy/dnSpy)
- [Unity Explorer](https://github.com/sinai-dev/UnityExplorer) (MelonLoader mod)

**Guidance:** [TECHNICAL_BLOCKERS.md](#technical-blockers) → Research Needed

---

## External Resources

### Official Documentation
- [MelonLoader Wiki](https://melonwiki.xyz/)
- [HarmonyX Wiki](https://github.com/BepInEx/HarmonyX/wiki)
- [Unity Camera Docs](https://docs.unity3d.com/Manual/class-Camera.html)
- [Unity Input System](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/manual/)

### Modding Tools
- [Il2CppDumper](https://github.com/Perfare/Il2CppDumper) - Decompile Il2Cpp
- [dnSpy](https://github.com/dnSpy/dnSpy) - .NET decompiler
- [Unity Explorer](https://github.com/sinai-dev/UnityExplorer) - Runtime inspector

**More Resources:** [DEVELOPER_GUIDE.md](#developer-guide) → Resources

---

## Cross-Reference Map

### Documentation Relationships

```
README.md (User Overview)
    ├─→ TECHNICAL_BLOCKERS.md (Detailed blockers)
    ├─→ DEVELOPMENT_LOG.md (Development history)
    └─→ AvatarCameraHijack.cs (Current implementation)

CLAUDE.md (AI Instructions)
    ├─→ TECHNICAL_BLOCKERS.md (Blocker summary)
    ├─→ README.md (User context)
    └─→ API_REFERENCE.md (Code details)

DEVELOPER_GUIDE.md (Developer Onboarding)
    ├─→ API_REFERENCE.md (Method signatures)
    ├─→ TECHNICAL_BLOCKERS.md (Workarounds)
    ├─→ AvatarCameraHijack.cs (Implementation)
    └─→ README.md (Project context)

API_REFERENCE.md (Code Reference)
    ├─→ DEVELOPER_GUIDE.md (Usage examples)
    ├─→ AvatarCameraHijack.cs (Source code)
    └─→ TECHNICAL_BLOCKERS.md (Blocker impact)

TECHNICAL_BLOCKERS.md (Problem Analysis)
    ├─→ README.md (Blocker overview)
    ├─→ DEVELOPER_GUIDE.md (Workaround code)
    └─→ API_REFERENCE.md (Affected methods)

DEVELOPMENT_LOG.md (History)
    ├─→ README.md (Approach summaries)
    └─→ TECHNICAL_BLOCKERS.md (Why approaches failed)

CHANGELOG.md (Version History)
    ├─→ README.md (Current status)
    └─→ API_REFERENCE.md (Version details)
```

### Code → Documentation Map

| Code File | Primary Docs | Related Docs |
|-----------|--------------|--------------|
| Mod.cs | API_REFERENCE.md | DEVELOPER_GUIDE.md, CLAUDE.md |
| AvatarCameraHijack.cs | API_REFERENCE.md | DEVELOPER_GUIDE.md, TECHNICAL_BLOCKERS.md |
| CameraController.cs | README.md | DEVELOPMENT_LOG.md |
| CameraController_v2.cs | README.md | TECHNICAL_BLOCKERS.md |
| CameraControllerFinal.cs | README.md | TECHNICAL_BLOCKERS.md |
| VKeyPatch.cs | TECHNICAL_BLOCKERS.md | README.md |
| *.csproj | CLAUDE.md | API_REFERENCE.md |

---

## Documentation Standards

### File Naming Conventions
- **ALL_CAPS.md** - Documentation files
- **PascalCase.cs** - Code files
- **lowercase.csproj** - Project configuration

### Markdown Structure
- Use `##` for main sections
- Use `###` for subsections
- Use tables for structured data
- Use code blocks with language tags
- Use status emojis: ✅ ❌ ⚠️ 🔴 🟡 🟢

### Code Documentation
- XML doc comments for public members
- Inline comments for blocker workarounds
- Examples in DEVELOPER_GUIDE.md
- Complete signatures in API_REFERENCE.md

---

## Maintenance

### When to Update This Index
- New documentation files added
- Major code restructuring
- New discoveries about game architecture
- Blocker status changes
- Version updates

### Validation Checklist
- [ ] All files listed in structure
- [ ] All cross-references valid
- [ ] Documentation summaries accurate
- [ ] Code file statuses correct (Active/Archived)
- [ ] External links functional

**Last Validated:** October 2025

---

## Quick Reference Cards

### 🔑 Key Bindings
| Key | Function | Effect |
|-----|----------|--------|
| F6 | Toggle | Third-person on/off |
| F8 | Debug | Camera search mode |
| V | Avatar (game) | Show character (game feature) |
| Right-click + Drag | Rotate | Camera rotation |

### 📊 Project Stats
- **Total Files:** 17 (9 code + 8 docs)
- **Active Code:** 2 files (~315 lines)
- **Archived Code:** 7 files (~1,200 lines)
- **Documentation:** 8 files (~2,000 lines)
- **Framework:** .NET 6.0 + MelonLoader
- **Status:** Experimental (3 blockers)

### 🎯 Primary Contacts
- **Author:** anubissbe
- **AI Assistant:** Claude Code
- **Repository:** [Link if available]
- **Game:** Schedule I by TVGS

---

**Navigation:** [Back to Top ↑](#project-index---schedule-1-third-person-camera-mod)

**Last Updated:** October 2025
**Index Version:** 1.0.0
**Project Version:** 0.1.0
