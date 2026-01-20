---
name: write-touchdesigner-shader
description: Writes GLSL shaders for TouchDesigner's GLSL TOP. Use when creating visual effects, generators, or filters for TouchDesigner, or when the user mentions TouchDesigner, TD, GLSL TOP, or pixel/compute shaders for TouchDesigner.
---

# TouchDesigner Shader Planning

This skill produces a detailed implementation plan for user review before any code is written. The plan should be thorough enough for a separate session to execute flawlessly.

## Workflow

### Step 1: Gather Requirements

Ask clarifying questions to understand:

1. **Effect Type**: Generator (no input), Filter (processes input), or Multi-buffer effect?
2. **Visual Goal**: What should it look like? Reference images/videos? Existing effects to emulate?
3. **Inputs Needed**: How many texture inputs? What are they (video, images, feedback)?
4. **Controls**: What parameters should be adjustable? (time, colors, intensity, etc.)
5. **Audio Reactivity**: Should it respond to audio? Which aspects?
6. **Performance**: Real-time priority? Resolution expectations?
7. **Integration**: How does this fit into the larger TD network?

### Step 2: Create Implementation Plan

After gathering requirements, create a comprehensive plan document. Save it to `/project-specs/td-[effect-name]-plan.md`.

**CRITICAL: Stop here and present the plan to the user for review. Do NOT proceed to implementation.**

## Plan Document Template

```markdown
# TouchDesigner Shader Plan: [Effect Name]

## Overview
[One paragraph describing what this shader does and its visual outcome]

## Effect Category
- [ ] Generator (creates visuals from nothing)
- [ ] Filter (processes input textures)
- [ ] Multi-buffer (uses multiple render targets)
- [ ] Compute shader

## Requirements Summary

### Visual Description
[Detailed description of the visual effect - what does it look like at rest? In motion? With audio?]

### Inputs
| Input | Type | Purpose |
|-------|------|---------|
| [input name] | [2D/3D/Cube] | [what it's used for] |

### Custom Uniforms
| Uniform | Type | Range | Default | Purpose |
|---------|------|-------|---------|---------|
| [name] | [float/vec2/vec3/vec4] | [min-max] | [default] | [what it controls] |

### Audio Integration
[How audio affects the shader, if applicable. Which uniforms? How mapped?]

## Technical Approach

### Algorithm
[Step-by-step description of how the effect works mathematically/visually]

1. [First operation]
2. [Second operation]
3. [How results combine]

### Key Techniques
[List specific GLSL techniques that will be used]
- [Technique 1]: [why/how]
- [Technique 2]: [why/how]

### TouchDesigner-Specific Considerations
- Texture sampling method: [sTD2DInputs[], etc.]
- Output handling: [TDOutputSwizzle usage]
- Resolution handling: [uTDOutputInfo.res usage]
- [Any other TD-specific patterns]

## Shader Structure

### Main Flow
```
[Pseudocode outline of the shader logic]
1. Get UV coordinates
2. [Processing step]
3. [Processing step]
4. Apply TDOutputSwizzle and output
```

### Helper Functions Needed
| Function | Purpose | Signature |
|----------|---------|-----------|
| [name] | [what it does] | [return type and params] |

## Edge Cases & Fallbacks

| Condition | Handling |
|-----------|----------|
| No input connected | [behavior] |
| Uniform at minimum | [behavior] |
| Uniform at maximum | [behavior] |
| [Other edge case] | [behavior] |

## Performance Notes
- [Anticipated bottlenecks]
- [Optimization strategies]
- [Recommended resolution limits if any]

## TouchDesigner Network Setup
[How this GLSL TOP should be connected in the network]
- Input sources: [what TOPs feed into this]
- Output destination: [where the result goes]
- Required companion nodes: [any CHOPs, DATs, etc. needed for uniforms]

## Implementation Checklist
- [ ] Create GLSL TOP with correct settings
- [ ] Set input count to [N]
- [ ] Configure uniforms on Vectors page: [list]
- [ ] Write shader code
- [ ] Test at target resolution
- [ ] Verify uniform ranges work as expected

## Reference Code Snippets

### Critical Patterns for This Effect
[Pre-selected code patterns from reference/glsl-reference.md that apply to this specific shader]

---

**Ready for review.** Once approved, implementation can begin in a new session using this plan.
```

## Reference Material

For TouchDesigner GLSL syntax, built-in variables, and common patterns, see [reference/glsl-reference.md](reference/glsl-reference.md).

## Key Platform Constraints

When planning, account for these TouchDesigner-specific requirements:

1. **Always use `TDOutputSwizzle()`** on final color output
2. **No `#version` statements** - TD adds this automatically
3. **Use `vUV.st`** for texture coordinates (without custom vertex shader)
4. **Sample via arrays**: `sTD2DInputs[0]`, not named samplers
5. **Compute shaders**: No `vUV` - calculate coordinates manually
6. **Multiple outputs**: Requires "# of Color Buffers" parameter set

## After User Approval

Once the user approves the plan:
1. The plan document serves as the complete specification
2. A new session (or the user) can implement by following the plan exactly
3. Reference material in `reference/glsl-reference.md` provides syntax details
4. The implementation checklist ensures nothing is missed
