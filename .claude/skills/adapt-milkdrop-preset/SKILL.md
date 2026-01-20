---
name: adapt-milkdrop-preset
description: Adapts MilkDrop (.milk) presets for use in other shader environments. Use when converting MilkDrop visualizations to ISF/Resolume Wire, Synesthesia, MadMapper, or other platforms.
user-invocable: true
---

# MilkDrop Preset Adaptation Planning

This skill produces a detailed adaptation plan for user review before any conversion work begins. The plan should be thorough enough for a separate session to execute the conversion flawlessly.

## Workflow

### Step 1: Analyze the MilkDrop Preset

When given a .milk preset file, thoroughly analyze it and document:

#### Core Motion System
- **Zoom**: Base `zoom` value? Audio modulation?
- **Rotation**: Base `rot` value? Audio modulation?
- **Warp**: `warp` parameter strength? Custom warp shader?
- **Translation**: `dx`, `dy` motion?
- **Per-vertex variations**: Any mesh-based motion effects?

#### Audio Reactivity
- Which audio inputs? (`bass`, `mid`, `treb`, attenuated versions?)
- How do they modulate parameters?
- Direct vs smoothed (`_att`) response?

#### Visual Elements
- Waveform drawing? Which mode?
- Custom shapes? (count, type, textures)
- Custom waves?
- Borders?

#### Shader Effects (MilkDrop 2+)
- Warp shader modifications?
- Composite shader post-processing?
- Custom textures?
- Blur usage (`sampler_blur1`, `sampler_blur2`, `sampler_blur3`)?

#### Key Techniques
- Feedback/decay patterns
- Color manipulation
- Geometric patterns
- Noise usage

### Step 2: Identify Target Platform

Ask the user which platform they want to target:

| Target | Best For | Key Capabilities |
|--------|----------|------------------|
| **Resolume Wire** | VJ performance | Single-pass ISF, external audio/feedback routing |
| **Synesthesia** | Music visualization | Built-in audio, simple feedback, BPM detection |
| **MadMapper** | Projection mapping | Multipass, persistent buffers, audio FFT |
| **TouchDesigner** | Complex installations | Full GLSL, network integration |

### Step 3: Create Adaptation Plan

After analysis and target selection, create a comprehensive plan document. Save it to `/project-specs/milkdrop-[preset-name]-to-[platform]-plan.md`.

**CRITICAL: Stop here and present the plan to the user for review. Do NOT proceed to implementation.**

## Plan Document Template

```markdown
# MilkDrop Adaptation Plan: [Preset Name] → [Target Platform]

## Source Preset Analysis

### Preset Overview
[One paragraph describing what this preset looks like visually]

### Visual Signature
- Primary movement: [describe the dominant motion]
- Color behavior: [how colors change]
- Audio response: [how it reacts to music]
- Distinctive features: [what makes this preset recognizable]

### MilkDrop Components Used

#### Per-Frame Variables
| Variable | Value/Expression | Purpose |
|----------|------------------|---------|
| `zoom` | [value] | [effect] |
| `rot` | [value] | [effect] |
| `warp` | [value] | [effect] |
| `dx`, `dy` | [value] | [effect] |
| `decay` | [value] | [effect] |
| [Q vars] | [value] | [effect] |

#### Per-Vertex Effects
[Describe any per-vertex motion - this is tricky to convert]

#### Warp Shader
- [ ] Uses default warp only
- [ ] Custom warp shader (describe below)

[If custom, describe what it does]

#### Composite Shader
- [ ] No composite shader
- [ ] Custom composite shader (describe below)

[If custom, describe post-processing effects]

#### Audio Mapping
| MilkDrop Input | Value Typical | What It Affects |
|----------------|---------------|-----------------|
| `bass` | [range] | [effect] |
| `bass_att` | [range] | [effect] |
| `mid` | [range] | [effect] |
| `treb` | [range] | [effect] |
| [other] | [range] | [effect] |

#### Visual Elements
- Waveform: [mode and settings, or "none"]
- Shapes: [description, or "none"]
- Borders: [settings, or "none"]

## Target Platform: [Platform Name]

### Platform Capabilities
[Brief summary of what this platform can/cannot do]

### Conversion Feasibility

| MilkDrop Feature | Can Convert? | Strategy |
|------------------|--------------|----------|
| Per-frame zoom/rot | Yes | [approach] |
| Per-vertex motion | Partial | [approach or limitation] |
| Warp shader | [Yes/No/Partial] | [approach] |
| Composite shader | [Yes/No/Partial] | [approach] |
| Feedback/decay | [Yes/Workaround] | [approach] |
| Audio reactivity | [Yes/Different] | [approach] |
| Blur textures | [Yes/No/Workaround] | [approach] |

### What Will Be Lost or Changed
[Honestly describe any features that cannot be replicated]

### What Will Be Gained
[Platform-specific features that enhance the adaptation]

## Conversion Strategy

### Motion System Translation

#### Zoom
```
MilkDrop: zoom = [expression]
Target:   [equivalent approach]
```

#### Rotation
```
MilkDrop: rot = [expression]
Target:   [equivalent approach]
```

#### Warp
```
MilkDrop: warp = [expression]
Target:   [equivalent approach]
```

### Feedback/Decay Translation
```
MilkDrop: decay = [value], feedback via sampler_main
Target:   [approach - persistent buffer, external routing, syn_FinalPass, etc.]
```

### Audio Translation

| MilkDrop | Target Platform Equivalent |
|----------|---------------------------|
| `bass` | [equivalent] |
| `bass_att` | [equivalent] |
| `mid` | [equivalent] |
| `treb` | [equivalent] |

### Color/Post-Processing Translation
[How composite shader effects will be converted]

## Implementation Outline

### Controls to Expose
| Control | Type | Purpose | Maps to |
|---------|------|---------|---------|
| [name] | [type] | [what it does] | [MilkDrop equivalent] |

### Shader Structure
```
[Pseudocode outline of how the shader will be structured]
1. [Setup/coordinates]
2. [Audio sampling]
3. [Motion/UV transformation]
4. [Feedback handling]
5. [Color/post-processing]
6. [Output]
```

### Platform-Specific Configuration
[JSON/scene.json/uniforms configuration needed]

## Fidelity Assessment

### Visual Accuracy: [High/Medium/Low]
[Explain how close the result will be to the original]

### Audio Accuracy: [High/Medium/Low]
[Explain how similarly it will respond to music]

### Key Compromises
1. [Compromise 1 and why it's necessary]
2. [Compromise 2 and why it's necessary]

## Implementation Checklist

### Before Coding
- [ ] User approves this plan
- [ ] Understand all MilkDrop expressions used
- [ ] Confirm platform constraints are acceptable
- [ ] Agree on acceptable fidelity level

### During Implementation
- [ ] Translate motion system
- [ ] Set up feedback mechanism
- [ ] Map audio inputs
- [ ] Recreate color processing
- [ ] Add user controls
- [ ] Test audio reactivity

### Validation
- [ ] Compare side-by-side with original (if possible)
- [ ] Test with various music
- [ ] Verify all controls work
- [ ] Check performance

## Reference

### MilkDrop Variable Reference
[Link to reference/milkdrop-reference.md]

### Target Platform Reference
[Link to relevant platform skill]

---

**Ready for review.** Once approved, implementation can begin in a new session using this plan and the target platform's skill reference.
```

## Reference Material

- [MilkDrop Variable & Function Reference](reference/milkdrop-reference.md)

For target platforms:
- Resolume Wire: See `write-resolume-shader` skill
- Synesthesia: See `write-synesthesia-shader` skill
- MadMapper: See `write-madmapper-shader` skill
- TouchDesigner: See `write-touchdesigner-shader` skill

## Key Architecture Differences

### MilkDrop Has:
- Implicit feedback loop (previous frame always available)
- Separate mesh-based motion system (per-vertex interpolation)
- Built-in audio analysis (bass/mid/treb already processed)
- Blur textures pre-computed at multiple scales

### Target Platforms Typically Have:
- Explicit feedback mechanisms (varies by platform)
- All computation per-pixel (no mesh)
- Audio from textures or external sources
- Manual blur implementation (or none)

## Common Conversion Challenges

| Challenge | Typical Solution |
|-----------|------------------|
| Per-vertex motion | Approximate with per-pixel math or simplify |
| Blur textures | Multipass blur (MadMapper) or skip (Wire) |
| Complex per-frame equations | Pre-compute constants, convert math |
| Q variables communication | Local variables or extra uniforms |
| Waveform drawing | Separate layer or omit |
| Custom shapes | Procedural SDF or omit |

## After User Approval

Once the user approves the plan:
1. The plan document serves as the complete specification
2. A new session uses the plan + target platform skill to implement
3. The fidelity assessment sets realistic expectations
4. The implementation checklist ensures completeness
