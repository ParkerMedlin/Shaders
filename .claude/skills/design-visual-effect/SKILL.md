---
name: design-visual-effect
description: Guides the design process for visual effects and generators. Use when the user wants to create a new shader, effect, or generator for any platform (ISF/MadMapper, TouchDesigner, Synesthesia, etc.). Follows a two-phase process: Requirements (what/why) then Design (how).
user-invocable: true
---

# Visual Effect Design Process

When the user wants to create a new visual effect, follow this two-phase process. Do not skip phases or combine them.

## Phase 1: Requirements

**Goal**: Define WHAT the effect does and WHY, without getting into HOW.

When the user describes an effect they want, create a requirements document at `/project-specs/[effect-name]-requirements.md` using this structure:

```markdown
# [Effect Name] - Requirements

## Description
[What is this effect? Describe the visual result in plain language. What does it look like? What's the vibe?]

## Intent
[Why does this effect exist? What's the use case? What mood/feeling/purpose does it serve?]

## Category
[Generator | Filter | Transition | Mixer | Audio-Reactive | Other]

## Inputs
[What does the effect need to work with?]
- Image input: [yes/no, how many?]
- Audio input: [yes/no, what aspect - fft, amplitude, beat?]
- Feedback: [yes/no]
- Other: [any other inputs]

## Controls
[What parameters should the user be able to adjust? For each control, describe what it does conceptually - not how it's implemented.]

| Control | Type | Purpose |
|---------|------|---------|
| [name] | [slider/toggle/color/menu/xy-pad] | [what does adjusting this do to the visual?] |

## Behavior Notes
[Any specific behaviors worth calling out]
- What happens at extreme control values?
- Any modes or states?
- How should it interact with other effects?

## Open Questions
[Anything unclear that needs discussion before moving to design]
```

After creating the draft, present it to the user and ask for revisions. Do NOT proceed to Phase 2 until the user confirms the requirements are complete.

**Keep it pure**: In this phase, resist the urge to talk about algorithms, math, or implementation. If you find yourself writing "using sine waves" or "by sampling neighboring pixels," you're in the wrong phase.

## Phase 2: Design

**Goal**: Define HOW to achieve the requirements, including technical approach and platform considerations.

Only begin this phase after the user approves the requirements. Create a design document at `/project-specs/[effect-name]-design.md`:

```markdown
# [Effect Name] - Design

## Target Platform
[ISF/MadMapper | ISF/Resolume Wire | TouchDesigner | Synesthesia | GLSL Generic]

## Platform Constraints
[What limitations does this platform impose?]
- [e.g., No multipass, no persistent buffers, single image input only, etc.]

## Algorithmic Approach
[How will we create this effect? Describe the technique(s) at a conceptual level.]

## Visual Breakdown
[Break down the effect into components/layers. How do the pieces fit together?]

1. [Component 1]: [what it does]
2. [Component 2]: [what it does]
3. [How they combine]

## Control Mapping
[How does each control from requirements map to the algorithm?]

| Control | Implementation |
|---------|----------------|
| [name] | [what parameter/behavior it affects and how] |

## Edge Cases
[What happens in edge conditions? How should we handle them?]
- [Control at 0]
- [Control at max]
- [No input image]
- [etc.]

## Performance Considerations
[Anything that might affect real-time performance? How to mitigate?]

## Reference
[Any reference shaders, techniques, or resources to draw from]
```

After creating the design draft, present it to the user for revisions. Once approved, proceed to implementation.

## Implementation

After both phases are approved, write the shader. The implementation approach depends on the target platform specified in the design doc.

For ISF shaders, reference the existing ISF skill materials in `.claude/skills/write-resolume-shader/reference/`.

## Workflow Summary

1. User describes effect idea
2. **You draft requirements** (what/why) → save to `/project-specs/[name]-requirements.md`
3. User revises until satisfied
4. **You draft design** (how) → save to `/project-specs/[name]-design.md`
5. User revises until satisfied
6. **You implement** the shader
