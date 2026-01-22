---
name: write-madmapper-shader
description: Writes ISF shaders for MadMapper. Use when creating visual effects, generators, or filters for MadMapper projection mapping software.
metadata:
  short-description: Plan and write ISF shaders for MadMapper
---

# MadMapper ISF Shader Planning

This skill produces a detailed implementation plan for user review before any code is written. The plan should be thorough enough for a separate session to execute flawlessly.

## Platform Capabilities

MadMapper ISF supports advanced features not available in Resolume Wire:

- **Multipass rendering** via `PASSES` array
- **Persistent buffers** (`PERSISTENT: true`) for feedback effects
- **`PASSINDEX` variable** for pass-specific logic
- **Audio FFT input** (`audioFFT` type)

## Workflow

### Step 1: Gather Requirements

Ask clarifying questions to understand:

1. **Effect Type**: Generator, Filter, Audio-reactive, or Transition?
2. **Visual Goal**: What should it look like? Reference images/videos?
3. **Inputs**: Image inputs? Audio FFT? How many?
4. **Controls**: What parameters should be adjustable?
5. **Feedback**: Does it need persistent buffers for trails/accumulation?
6. **Multipass**: Blur? Post-processing? Multi-stage effects?
7. **Audio Reactivity**: How should it respond to audio analysis?
8. **Projection Context**: How will this be used in projection mapping?

### Step 2: Create Implementation Plan

After gathering requirements, create a comprehensive plan document. Save it to `/project-specs/isf-mm-[effect-name]-plan.md`.

**CRITICAL: Stop here and present the plan to the user for review. Do NOT proceed to implementation.**

## Plan Document Template

```markdown
# MadMapper ISF Plan: [Effect Name]

## Overview
[One paragraph describing what this shader does in a projection mapping context]

## Effect Category
- [ ] Generator (creates visuals from nothing)
- [ ] Color Effect (modifies colors)
- [ ] Distortion (warps geometry)
- [ ] Blur (blur effects)
- [ ] Stylize (artistic effects)
- [ ] Transition (mixing between sources)
- [ ] Audio-Reactive

## Requirements Summary

### Visual Description
[Detailed description - what does it look like? How does it respond to parameters and audio?]

### Image Inputs
| Input Name | Purpose |
|------------|---------|
| `inputImage` | [primary source] |
| [other] | [purpose] |

### Audio Integration
- [ ] Uses `audioFFT` input
- [ ] No audio input

If using audio:
| Frequency Range | Visual Response |
|-----------------|-----------------|
| Bass (0.0-0.1) | [what happens] |
| Mid (0.1-0.5) | [what happens] |
| Treble (0.5-1.0) | [what happens] |

### Controls (ISF INPUTS)

| Name | Type | Default | Min | Max | Label | Purpose |
|------|------|---------|-----|-----|-------|---------|
| [name] | float/bool/color/point2D/long | [default] | [min] | [max] | [label] | [what it does] |

### Multipass Requirements
- [ ] Single pass sufficient
- [ ] Multipass needed (detail below)

### Feedback/Persistence
- [ ] No feedback needed
- [ ] Persistent buffer required (detail below)

## Technical Approach

### Algorithm
[Step-by-step description of how the effect works]

1. [First operation]
2. [Second operation]
3. [How results combine]

### Key Techniques
- [Technique 1]: [purpose and approach]
- [Technique 2]: [purpose and approach]

### Multipass Strategy (if applicable)

| Pass | Target | Resolution | Purpose |
|------|--------|------------|---------|
| 0 | `bufferA` | full / `$WIDTH/4` | [what happens] |
| 1 | `bufferB` | full | [what happens] |
| 2 | (final) | full | [composite/output] |

### Feedback Strategy (if applicable)
- Buffer name: `feedbackBuffer`
- Decay method: [multiplication / subtraction / other]
- Decay rate: [value]
- Motion: [zoom / translation / none]

## ISF JSON Header

```json
{
    "DESCRIPTION": "[description]",
    "CREDIT": "[author]",
    "ISFVSN": "2.0",
    "CATEGORIES": ["[category]"],
    "INPUTS": [
        // Full input definitions
    ],
    "PASSES": [
        // If multipass/feedback
    ]
}
```

## Shader Structure

### Main Flow (Pseudocode)
```
main():
    if PASSINDEX == 0:
        [first pass operations]
    else if PASSINDEX == 1:
        [second pass operations]
    else:
        [final pass / composite]
```

Or for single-pass:
```
main():
    1. Get UV: isf_FragNormCoord
    2. [Sample inputs]
    3. [Process]
    4. Set gl_FragColor with alpha = 1.0
```

### Helper Functions Needed
| Function | Purpose | Signature |
|----------|---------|-----------|
| [name] | [what it does] | [return and params] |

## Audio Sampling Strategy (if applicable)

```glsl
// Pseudocode for audio usage
float bass = IMG_NORM_PIXEL(audioFFT, vec2(0.05, 0.0)).r;
float mid = IMG_NORM_PIXEL(audioFFT, vec2(0.3, 0.0)).r;
float treble = IMG_NORM_PIXEL(audioFFT, vec2(0.7, 0.0)).r;

// Apply sensitivity control
bass *= sensitivity;
// [how bass affects visuals]
```

## Edge Cases & Fallbacks

| Condition | Handling |
|-----------|----------|
| No inputImage connected | [behavior] |
| No audio signal | [behavior - should still look good] |
| Control at minimum | [behavior] |
| Control at maximum | [behavior] |
| First frame (feedback empty) | [behavior] |

## Performance Notes
- [Computational considerations for projection mapping]
- [Multipass resolution strategy for performance]
- [Any resolution limits]

## Implementation Checklist

### ISF File Structure
- [ ] JSON header with all INPUTS defined
- [ ] CATEGORIES appropriate for MadMapper
- [ ] CREDIT and DESCRIPTION filled
- [ ] PASSES array configured (if multipass)
- [ ] PERSISTENT flags set (if feedback)

### GLSL Code
- [ ] Use `IMG_THIS_PIXEL()` or `IMG_NORM_PIXEL()` for sampling
- [ ] Use `isf_FragNormCoord` for UV
- [ ] Set `gl_FragColor.a = 1.0` explicitly
- [ ] No `texture2D()` - use ISF functions only
- [ ] PASSINDEX logic correct (if multipass)
- [ ] Feedback decay prevents infinite accumulation

### Audio Integration (if applicable)
- [ ] `audioFFT` input declared
- [ ] Sensitivity control provided
- [ ] Effect works without audio (fallback behavior)
- [ ] Temporal smoothing if needed (raw FFT is noisy)

### Testing
- [ ] Test at MadMapper's various resolutions
- [ ] Test with and without audio
- [ ] Test all controls at extremes
- [ ] Verify feedback doesn't blow out over time

## Reference Code Snippets

### Multipass Patterns
[Pre-selected patterns for this effect's multipass needs]

### Feedback Patterns
[Relevant persistent buffer code examples]

### Audio Sampling Patterns
[FFT texture sampling examples]

---

**Ready for review.** Once approved, implementation can begin in a new session using this plan.
```

## Key Platform Features

When planning for MadMapper, leverage these capabilities:

1. **Multipass rendering**: Use `PASSES` array for blur, post-processing, multi-stage effects
2. **Persistent buffers**: `PERSISTENT: true` enables frame-to-frame memory for trails/feedback
3. **Audio FFT**: `audioFFT` input type for spectrum analysis
4. **`PASSINDEX`**: Route logic based on current pass
5. **Resolution expressions**: `$WIDTH/4` for downsampled intermediate buffers

## Key Differences from Wire

| Feature | MadMapper | Resolume Wire |
|---------|-----------|---------------|
| Multipass | Supported | Not supported |
| Persistent buffers | Supported | Not supported |
| PASSINDEX | Works | Always 0 |
| Audio FFT | Built-in input type | External node only |

## Reference Material

MadMapper uses ISF format. See the write-resolume-shader skill references for ISF syntax:
- Input Types & JSON Structure
- ISF & GLSL Functions
- Converting Shadertoy/GLSL to ISF

## After User Approval

Once the user approves the plan:
1. The plan document serves as the complete specification
2. A new session (or the user) can implement by following the plan exactly
3. Reference materials provide ISF syntax details
4. The implementation checklist ensures nothing is missed
