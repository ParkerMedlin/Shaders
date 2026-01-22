---
name: write-synesthesia-shader
description: Writes GLSL shaders for Synesthesia music visualizer. Use when creating audio-reactive visuals for Synesthesia, or converting effects to Synesthesia format.
metadata:
  short-description: Plan and write Synesthesia audio-reactive shaders
---

# Synesthesia Shader Planning

This skill produces a detailed implementation plan for user review before any code is written. The plan should be thorough enough for a separate session to execute flawlessly.

## Workflow

### Step 1: Gather Requirements

Ask clarifying questions to understand:

1. **Visual Concept**: What should it look like? Abstract, geometric, organic, waveform-based?
2. **Audio Reactivity**: Which audio aspects drive the effect?
   - Frequency bands (bass, mid, high)?
   - Beat detection?
   - Overall energy/presence?
   - BPM sync?
3. **User Controls**: What should be adjustable? (colors, speed, intensity, style variations)
4. **Feedback/Persistence**: Does it need frame-to-frame memory? Trails? Accumulation?
5. **Media Integration**: Should it incorporate user media (images, video, webcam)?
6. **Multipass Needs**: Multiple render passes? Blur? Post-processing?
7. **JavaScript Scripting**: Any complex per-frame logic beyond GLSL?

### Step 2: Create Implementation Plan

After gathering requirements, create a comprehensive plan document. Save it to `/project-specs/syn-[effect-name]-plan.md`.

**CRITICAL: Stop here and present the plan to the user for review. Do NOT proceed to implementation.**

## Plan Document Template

```markdown
# Synesthesia Shader Plan: [Effect Name]

## Overview
[One paragraph describing what this shader does and how it responds to music]

## Scene Structure
```
[sceneName].synScene/
├── main.glsl        # Shader code
├── scene.json       # Configuration and controls
├── thumbnail.png    # Preview image (create after testing)
├── script.js        # [Include if needed]
└── images/          # [Include if bundling textures]
```

## Requirements Summary

### Visual Description
[Detailed description - what does it look like at rest? How does it move with music? What's the vibe?]

### Audio Mapping Strategy

| Audio Input | Visual Response |
|-------------|-----------------|
| `syn_BassLevel` | [what happens] |
| `syn_BassHits` | [what happens] |
| `syn_MidLevel` | [what happens] |
| `syn_HighHits` | [what happens] |
| `syn_BPMSin` | [what happens] |
| [other uniforms] | [what happens] |

### User Controls (scene.json)

| Control | Type | Default | Range | Purpose |
|---------|------|---------|-------|---------|
| [name] | slider/color/toggle/xy/knob | [default] | [min-max] | [what it does] |

### Media Integration
- [ ] Uses `syn_Media` for user images/video
- [ ] Requires webcam support
- [ ] No media integration needed

### Feedback/Multipass
- [ ] Single pass, no feedback
- [ ] Uses `syn_FinalPass` for simple feedback
- [ ] Custom multipass setup (detail below)

## Technical Approach

### Algorithm
[Step-by-step description of how the effect works]

1. [First visual layer/operation]
2. [Second visual layer/operation]
3. [How audio modulates the above]
4. [How layers combine]

### Key Techniques
- [Technique 1]: [purpose and approach]
- [Technique 2]: [purpose and approach]

### Coordinate System
- Primary coordinates: `_uv` / `_uvc` / polar
- Transformations applied: [rotation, scaling, mirroring, etc.]

## scene.json Configuration

```json
{
  "TITLE": "[Effect Name]",
  "CREDIT": "[Author]",
  "CONTROLS": [
    // Full control definitions with types, defaults, ranges, UI groups
  ],
  "PASSES": [
    // If multipass needed
  ]
}
```

## Shader Structure

### Main Flow (Pseudocode)
```
renderMain():
    1. Get coordinates (_uv, _uvc)
    2. [Audio sampling]
    3. [Coordinate transformations]
    4. [Core visual generation]
    5. [Color processing]
    6. [Feedback mixing if applicable]
    7. Return final color
```

### Helper Functions Needed
| Function | Purpose |
|----------|---------|
| [name] | [what it does] |

### Synesthesia Functions to Use
[List built-in functions from reference that apply]
- `_scale()`, `_map()` for value remapping
- `_palette()` for color generation
- `_fbm()` for noise
- [etc.]

## Audio Reactivity Details

### Frequency Response
```
Bass (0.0-0.2):  [visual behavior]
Mid (0.2-0.5):   [visual behavior]
High (0.5-1.0):  [visual behavior]
```

### Temporal Response
- Continuous modulation: [which uniforms, how smoothed]
- Transient response: [which hit uniforms trigger what]
- BPM sync: [how BPM uniforms affect motion/color]

## Edge Cases & Fallbacks

| Condition | Handling |
|-----------|----------|
| No audio playing | [behavior - should still look good] |
| Control at minimum | [behavior] |
| Control at maximum | [behavior] |
| No media selected | [behavior if media-enabled] |

## Performance Notes
- [Computationally expensive operations]
- [Recommended PARAMS values for smooth controls]
- [Any resolution considerations]

## Implementation Checklist

### scene.json
- [ ] TITLE and CREDIT set
- [ ] All controls defined with proper types and ranges
- [ ] UI_GROUPs organized logically
- [ ] PARAMS values tuned for smooth transitions
- [ ] PASSES configured (if multipass)

### main.glsl
- [ ] `renderMain()` function structure
- [ ] Audio uniforms sampled appropriately
- [ ] Controls integrated and tested at extremes
- [ ] Good visual at default settings (no adjustment needed to look interesting)
- [ ] Feedback decay prevents infinite accumulation (if using feedback)

### Testing
- [ ] Works with various music genres
- [ ] Responsive but not twitchy
- [ ] Looks good at 1080p and 4K
- [ ] All controls have visible effect
- [ ] Thumbnail captured

## Reference Code Snippets

### Audio Sampling Patterns
[Pre-selected patterns from reference that apply to this shader]

### Visual Technique References
[Relevant code examples from reference materials]

---

**Ready for review.** Once approved, implementation can begin in a new session using this plan.
```

## Reference Material

Synesthesia has extensive documentation in the references directory:

- [Intro & Basics](references/Intro.md) - Scene structure, coordinate systems
- [Audio Uniforms](references/SSFAudioUniforms.md) - All audio-reactive uniforms
- [Built-in Functions](references/SSFFunctions.md) - Math, noise, color, coordinate functions
- [Media Uniforms](references/SSFMediaUniforms.md) - User media integration
- [JSON Configuration](references/JSONConfig.md) - scene.json control definitions
- [JavaScript Scripting](references/JavaScriptScripting.md) - Per-frame scripting
- [External Libraries](references/ExternalLibraries.md) - lygia, hg_sdf
- [Best Practices](references/BestPractices.md) - Performance and design guidelines

## Key Platform Features

When planning, leverage these Synesthesia-specific capabilities:

1. **Rich audio uniforms**: Levels, hits, presence, time, BPM detection
2. **Built-in spectrum texture**: `syn_Spectrum` with raw, juiced, smooth FFT + waveform
3. **Simple feedback**: `syn_FinalPass` for previous frame access
4. **Smooth controls**: `slider smooth`, `toggle smooth` with configurable PARAMS
5. **Coordinate helpers**: `_uv`, `_uvc`, `_rotate()`, `_toPolar()`
6. **External libraries**: lygia (noise, color, SDF) and hg_sdf pre-loaded

## After User Approval

Once the user approves the plan:
1. The plan document serves as the complete specification
2. A new session (or the user) can implement by following the plan exactly
3. Reference materials provide syntax details for specific features
4. The implementation checklist ensures nothing is missed
