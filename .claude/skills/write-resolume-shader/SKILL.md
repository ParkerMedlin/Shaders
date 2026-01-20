---
name: write-resolume-shader
description: Writes ISF (Interactive Shader Format) shaders for Resolume Wire. Use when creating visual effects, generators, or filters for Resolume, or when the user mentions ISF, GLSL shaders for VJ software, Wire, or visual effects programming.
---

# Resolume Wire ISF Shader Planning

This skill produces a detailed implementation plan for user review before any code is written. The plan should be thorough enough for a separate session to execute flawlessly.

## Critical Wire Limitation

**Resolume Wire does NOT support multipass rendering or persistent buffers.**

- No `PASSES` array - Wire ignores it
- No `PERSISTENT: true` buffers
- No `PASSINDEX` variable (always 0)
- No frame-to-frame state within the shader

**Feedback workaround**: Route shader output back as a second `image` input through Wire's node system. Design shaders expecting feedback as an explicit input parameter.

## Workflow

### Step 1: Gather Requirements

Ask clarifying questions to understand:

1. **Effect Type**: Generator (no input) or Filter (processes inputImage)?
2. **Visual Goal**: What should it look like? Reference images/videos?
3. **Inputs**: How many image inputs? Any feedback loops needed?
4. **Controls**: What parameters should be adjustable?
5. **Audio**: Will it receive audio analysis from external Wire nodes?
6. **Performance**: Real-time VJ performance requirements?
7. **Wire Integration**: How does this fit into the node graph?

### Step 2: Create Implementation Plan

After gathering requirements, create a comprehensive plan document. Save it to `/project-specs/isf-wire-[effect-name]-plan.md`.

**CRITICAL: Stop here and present the plan to the user for review. Do NOT proceed to implementation.**

## Plan Document Template

```markdown
# Resolume Wire ISF Plan: [Effect Name]

## Overview
[One paragraph describing what this shader does in a VJ context]

## Wire Constraints Acknowledgment
- [ ] Single-pass only (no multipass)
- [ ] No persistent buffers
- [ ] Feedback requires external routing in Wire

## Effect Category
- [ ] Generator (creates visuals from nothing)
- [ ] Filter (processes inputImage)
- [ ] Feedback-ready (designed for external feedback routing)

## Requirements Summary

### Visual Description
[Detailed description - what does it look like? How does it respond to parameters?]

### Image Inputs
| Input Name | Purpose | Optional? |
|------------|---------|-----------|
| `inputImage` | [primary source] | [yes/no] |
| `feedbackImage` | [previous frame for feedback] | [yes/no] |
| [other] | [purpose] | [yes/no] |

### Controls (ISF INPUTS)

| Name | Type | Default | Min | Max | Label | Purpose |
|------|------|---------|-----|-----|-------|---------|
| [name] | float/bool/color/point2D/long | [default] | [min] | [max] | [label] | [what it does] |

## Technical Approach

### Algorithm
[Step-by-step description of how the effect works]

1. [First operation]
2. [Second operation]
3. [How results combine]

### Key Techniques
- [Technique 1]: [purpose and approach]
- [Technique 2]: [purpose and approach]

### Single-Pass Strategy
[How to achieve the desired effect in one pass. If it would normally require multipass, explain the workaround]

## ISF JSON Header

```json
{
    "DESCRIPTION": "[description]",
    "CREDIT": "[author]",
    "ISFVSN": "2.0",
    "CATEGORIES": ["[category]"],
    "INPUTS": [
        // Full input definitions
    ]
}
```

## Shader Structure

### Main Flow (Pseudocode)
```
main():
    1. Get UV: isf_FragNormCoord
    2. [Sample inputs if filter]
    3. [Apply transformations]
    4. [Generate/process visuals]
    5. Set gl_FragColor with alpha = 1.0
```

### Helper Functions Needed
| Function | Purpose | Signature |
|----------|---------|-----------|
| [name] | [what it does] | [return and params] |

## Wire Node Graph Setup

[Describe how this shader fits into Wire's node system]

```
[Input Source] --> [This Shader] --> [Output/Next Effect]
                        ^
                        |
                   [Feedback loop if needed - route output back]
```

### Required External Nodes
- [Node type]: [purpose]
- [Node type]: [purpose]

### Feedback Routing (if applicable)
[Specific instructions for setting up feedback in Wire's node graph]

## Edge Cases & Fallbacks

| Condition | Handling |
|-----------|----------|
| No inputImage connected | [behavior] |
| feedbackImage not routed | [behavior - should still work] |
| Control at minimum | [behavior] |
| Control at maximum | [behavior] |

## Performance Notes
- [Computational considerations for real-time VJ use]
- [Resolution limits if any]
- [Frame rate considerations]

## Implementation Checklist

### ISF File
- [ ] JSON header with all INPUTS defined
- [ ] CATEGORIES appropriate for Wire's browser
- [ ] CREDIT and DESCRIPTION filled
- [ ] All inputs have LABEL for UI clarity

### GLSL Code
- [ ] Use `IMG_THIS_PIXEL()` or `IMG_NORM_PIXEL()` for sampling
- [ ] Use `isf_FragNormCoord` for UV (not gl_FragCoord)
- [ ] Set `gl_FragColor.a = 1.0` explicitly
- [ ] No `texture2D()` - use ISF functions only
- [ ] No multipass logic (PASSINDEX always 0)

### Wire Integration
- [ ] Test in Wire node graph
- [ ] Verify feedback routing works (if needed)
- [ ] Confirm controls appear correctly in Wire UI
- [ ] Test at various resolutions

## Reference Code Snippets

### Sampling Patterns for This Effect
[Pre-selected ISF sampling patterns that apply]

### Control Patterns
[Relevant input definition examples]

---

**Ready for review.** Once approved, implementation can begin in a new session using this plan.
```

## Reference Material

- [Input Types & JSON Structure](reference/inputs.md) - Complete INPUT definitions
- [ISF & GLSL Functions](reference/functions.md) - IMG_PIXEL, IMG_NORM_PIXEL, etc.
- [Converting Shadertoy/GLSL to ISF](reference/conversion.md) - Porting guide

## Key Platform Constraints

When planning for Wire, always account for:

1. **Single-pass only**: All computation must happen in one `main()` function
2. **Explicit feedback**: No implicit previous frame - must be routed externally
3. **ISF image functions**: `IMG_NORM_PIXEL()`, `IMG_THIS_PIXEL()`, never `texture2D()`
4. **Alpha channel**: Always set `gl_FragColor.a` explicitly
5. **Normalized coordinates**: `isf_FragNormCoord` for resolution independence
6. **No persistent state**: Each frame starts fresh

## After User Approval

Once the user approves the plan:
1. The plan document serves as the complete specification
2. A new session (or the user) can implement by following the plan exactly
3. Reference materials in `reference/` provide syntax details
4. The Wire node graph setup ensures correct integration
