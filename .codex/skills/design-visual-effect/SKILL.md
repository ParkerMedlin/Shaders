---
name: design-visual-effect
description: Guides the design process for visual effects and generators. Use when the user wants to create a new shader, effect, or generator for any platform (ISF/MadMapper, TouchDesigner, Synesthesia, etc.). Follows a two-phase process - Requirements (what/why) then Design (how).
metadata:
  short-description: Two-phase visual effect design process
---

# Visual Effect Design Process

This skill produces a comprehensive implementation plan through two distinct phases: Requirements (WHAT) and Design (HOW). The user reviews and approves each phase before proceeding. The final output is a plan document detailed enough for a separate session to implement.

## Workflow Overview

```
Phase 1: Requirements → User Review → Approval
Phase 2: Design → User Review → Approval
Output: Complete plan ready for implementation session
```

**CRITICAL: Do not skip phases or combine them. Get explicit user approval at each gate.**

---

## Phase 1: Requirements

**Goal**: Define WHAT the effect does and WHY, without getting into HOW.

### Gather Information

Ask clarifying questions about:

1. **Visual Outcome**: What should it look like? Reference images? Existing effects to emulate?
2. **Intent/Purpose**: What's the use case? Live performance? Installation? Music video?
3. **Platform**: Which platform(s)? (ISF/Wire, MadMapper, Synesthesia, TouchDesigner)
4. **Category**: Generator, Filter, Transition, Audio-reactive, or combination?
5. **Inputs**: Image input(s)? Audio? Feedback?
6. **User Controls**: What should be adjustable at runtime?
7. **Behavior**: Any specific edge cases or modes?

### Create Requirements Document

Save to `/project-specs/[effect-name]-requirements.md`:

```markdown
# [Effect Name] - Requirements

## Description
[What is this effect? Describe the visual result in plain language. What does it look like? What's the vibe? No technical implementation details.]

## Intent
[Why does this effect exist? What's the use case? What mood/feeling/purpose does it serve?]

## Target Platform
[Primary platform, plus any secondary targets]

## Category
- [ ] Generator (creates from nothing)
- [ ] Filter (processes input)
- [ ] Transition (mixes sources)
- [ ] Audio-Reactive
- [ ] Other: [specify]

## Inputs

| Input | Type | Purpose |
|-------|------|---------|
| [name] | image/audio/feedback | [what it provides] |

## User Controls

| Control | Type | Purpose |
|---------|------|---------|
| [name] | slider/toggle/color/menu/xy-pad | [what adjusting this does to the visual - conceptually, not technically] |

## Behavior Notes
- At minimum settings: [what happens]
- At maximum settings: [what happens]
- Without input (if filter): [what happens]
- Without audio (if audio-reactive): [what happens]
- [Other specific behaviors]

## Open Questions
[Anything unclear that needs discussion]

---

**Phase 1 Complete.** Awaiting user review before proceeding to Design phase.
```

**STOP HERE. Present requirements to user and wait for approval.**

---

## Phase 2: Design

**Goal**: Define HOW to achieve the requirements technically.

Only begin after user approves Phase 1. Create design document at `/project-specs/[effect-name]-design.md`:

```markdown
# [Effect Name] - Design

## Target Platform
[Platform from requirements]

## Platform Constraints
[What limitations does this platform impose?]
- [constraint 1]
- [constraint 2]

## Algorithmic Approach

### Core Technique
[What mathematical/visual technique creates this effect?]

### Step-by-Step
1. [First visual operation]
2. [Second visual operation]
3. [How they combine]

## Visual Breakdown

### Layers/Components
| Layer | Purpose | How Generated |
|-------|---------|---------------|
| [name] | [visual purpose] | [technique] |

### Composition
[How layers combine: additive, multiplicative, masking, etc.]

## Control Mapping

| Control (from requirements) | Implementation |
|-----------------------------|----------------|
| [name] | [what parameter/math it affects and how] |

## Audio Mapping (if applicable)

| Audio Input | Visual Response | Implementation |
|-------------|-----------------|----------------|
| [frequency/beat/level] | [what changes] | [how it's achieved] |

## Feedback Strategy (if applicable)
- Method: [persistent buffer / external routing / syn_FinalPass / etc.]
- Decay: [rate and method]
- Motion: [zoom / rotation / translation / none]

## Platform-Specific Configuration

### For [Platform]
[JSON header / scene.json / uniforms configuration]

```json
{
  // Full configuration
}
```

## Shader Structure (Pseudocode)

```
main / renderMain:
    1. [Setup coordinates]
    2. [Sample inputs if any]
    3. [Audio processing if any]
    4. [Core visual generation]
    5. [Layer composition]
    6. [Feedback mixing if any]
    7. [Output]
```

## Helper Functions

| Function | Purpose | Signature |
|----------|---------|-----------|
| [name] | [what it does] | [return type and params] |

## Edge Cases

| Condition | Handling |
|-----------|----------|
| [from requirements] | [technical solution] |

## Performance Considerations
- [Potential bottlenecks]
- [Optimization strategies]
- [Resolution limits if any]

## Implementation Checklist

### Platform Configuration
- [ ] [config item 1]
- [ ] [config item 2]

### Shader Code
- [ ] [code requirement 1]
- [ ] [code requirement 2]

### Testing
- [ ] [test criterion 1]
- [ ] [test criterion 2]

## Reference Code Snippets

### From Platform Reference
[Pre-selected patterns from the platform's skill reference that apply to this specific effect]

---

**Phase 2 Complete. Full implementation plan ready for review.**

Once approved, implementation can begin in a new session using:
1. This design document as the specification
2. The platform skill's reference materials for syntax details
```

**STOP HERE. Present design to user and wait for approval.**

---

## Phase Summary

### Phase 1 Deliverable
- `/project-specs/[effect-name]-requirements.md`
- Describes WHAT and WHY
- No technical implementation details
- User must approve before Phase 2

### Phase 2 Deliverable
- `/project-specs/[effect-name]-design.md`
- Describes HOW
- Complete technical specification
- User must approve before implementation

### Ready for Implementation
After both phases approved:
- Requirements doc defines the goal
- Design doc defines the approach
- Platform skill references provide syntax
- A new session can implement from these documents

---

## Why This Process

1. **Requirements first**: Ensures we're building the right thing before deciding how
2. **Explicit approval gates**: User maintains control and can redirect early
3. **Separate documents**: Clean handoff to implementation session
4. **Platform-agnostic requirements**: Same requirements could target different platforms
5. **Thorough design**: Reduces implementation surprises and rework

---

## Platform Skills Reference

Once design is approved, implementation uses:

- **Resolume Wire**: `write-resolume-shader` skill
- **Synesthesia**: `write-synesthesia-shader` skill
- **MadMapper**: `write-madmapper-shader` skill
- **TouchDesigner**: `write-touchdesigner-shader` skill

Each platform skill has reference materials with syntax details, built-in functions, and common patterns.
