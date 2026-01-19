---
name: adapt-milkdrop-preset
description: Adapts MilkDrop (.milk) presets for use in other shader environments. Use when converting MilkDrop visualizations to ISF/Resolume Wire, Synesthesia, MadMapper, or other platforms.
user-invocable: true
---

# Adapting MilkDrop Presets

This skill guides the conversion of MilkDrop presets to other visual platforms. The process involves understanding the source preset, mapping its concepts to the target platform, and implementing equivalent functionality.

## Workflow

1. **Analyze the MilkDrop preset** - Understand what it does visually and technically
2. **Identify the target platform** - Determine which environment to convert to
3. **Map concepts** - Translate MilkDrop architecture to target platform
4. **Implement** - Write the shader/effect using platform-specific skill

## Step 1: Analyze the MilkDrop Preset

When given a .milk preset file, identify:

### Core Motion
- **Zoom/Rotation**: What are the base `zoom`, `rot` values? Any per-vertex variation?
- **Warp**: How much warping (`warp` parameter)? Custom warp shader effects?
- **Translation**: Any `dx`, `dy` motion?

### Audio Reactivity
- Which audio inputs are used? (`bass`, `mid`, `treb`, attenuated versions?)
- How do they modulate the parameters?
- Direct vs smoothed response?

### Visual Elements
- Waveform drawing? Which mode?
- Custom shapes? (count, instancing, textured?)
- Custom waves?
- Borders?

### Shader Effects (if MilkDrop 2+)
- Warp shader: What modifications beyond basic motion?
- Composite shader: Post-processing effects? Blur usage?
- Custom textures?

### Key Techniques
- Feedback loops
- Color manipulation
- Geometric patterns
- Noise usage

## Step 2: Platform Selection

Ask the user which platform they want to target, then delegate to the appropriate skill:

| Target Platform | Skill to Use | Key Considerations |
|-----------------|--------------|-------------------|
| **Resolume Wire** | `write-resolume-shader` | Single-pass ISF, no persistent buffers, feedback via node routing |
| **Synesthesia** | `write-synesthesia-shader` | Built-in audio FFT via textures, `iTime`/`iResolution` uniforms, `iPreviousFrame` for feedback |
| **MadMapper** | `write-madmapper-shader` | ISF format with multipass and persistent buffer support |
| **TouchDesigner** | (manual guidance) | GLSL TOPs, different architecture |
| **Generic GLSL** | (manual guidance) | Shadertoy-style or standalone |

## Step 3: Concept Mapping

### MilkDrop → ISF/Wire Mapping

| MilkDrop Concept | ISF Equivalent |
|------------------|----------------|
| `time` | `TIME` |
| `bass`, `mid`, `treb` | Audio input (requires external FFT or analysis node) |
| `uv` / `uv_orig` | `isf_FragNormCoord` |
| `texsize` | `RENDERSIZE` |
| Per-frame equations | Compute in `main()` before UV manipulation |
| Per-vertex equations | Compute per-pixel (less efficient but necessary) |
| `sampler_main` + feedback | `feedbackImage` input routed from output |
| `decay` | `mix(color, vec4(0.0), decayAmount)` or multiply |
| Q variables | Local variables or uniforms |
| Custom shapes | Must be drawn procedurally in shader |
| Waveforms | Requires audio texture input or separate node |
| Blur textures | Separate blur nodes or manual blur in shader |

### Key Architecture Differences

**MilkDrop has:**
- Implicit feedback loop (previous frame always available)
- Separate mesh-based motion system
- Built-in audio analysis
- Per-vertex interpolation for motion

**ISF/Wire has:**
- Explicit feedback via node routing
- All computation per-pixel
- Audio must come from external sources
- No built-in mesh system

### Adaptation Strategies

**For feedback effects:**
```glsl
// ISF: Add feedbackImage input, mix with new content
vec4 feedback = IMG_THIS_PIXEL(feedbackImage);
vec4 fresh = /* new content */;
gl_FragColor = mix(fresh, feedback * decay, feedbackAmount);
```

**For motion/warping:**
```glsl
// Convert per-vertex motion to per-pixel UV distortion
vec2 uv = isf_FragNormCoord;
vec2 center = vec2(0.5);
vec2 delta = uv - center;

// Zoom
float zoom = 1.02; // MilkDrop zoom value
uv = center + delta / zoom;

// Rotation
float rot = 0.01; // MilkDrop rot value
float c = cos(rot), s = sin(rot);
delta = uv - center;
uv = center + vec2(delta.x*c - delta.y*s, delta.x*s + delta.y*c);
```

**For warp:**
```glsl
// Warp is typically sine-based distortion
float warpAmount = 1.0;
vec2 warpUV = uv;
warpUV.x += sin(uv.y * 10.0 + TIME) * 0.01 * warpAmount;
warpUV.y += cos(uv.x * 10.0 + TIME) * 0.01 * warpAmount;
```

**For audio reactivity (when available):**
```glsl
// Map MilkDrop audio variables to your audio input
// bass_att, mid_att, treb_att in MilkDrop are smoothed values
float bass = /* from audio texture or uniform */;
float zoom = 1.0 + (bass - 1.0) * 0.1; // React to bass
```

## Step 4: Delegate to Platform Skill

Once analysis is complete and target is identified:

### For Resolume Wire
Reference the `write-resolume-shader` skill at `.claude/skills/write-resolume-shader/`. The skill provides:
- ISF JSON structure
- Input type definitions
- Wire-specific limitations (no multipass, no persistent buffers)
- Feedback workaround via node routing

### For Synesthesia
Reference the `write-synesthesia-shader` skill at `.claude/skills/write-synesthesia-shader/`. The skill provides:
- Standard GLSL structure with Synesthesia uniforms
- Audio FFT/waveform texture sampling
- `iPreviousFrame` for feedback effects
- User parameter definitions

### For MadMapper
Reference the `write-madmapper-shader` skill at `.claude/skills/write-madmapper-shader/`. The skill provides:
- ISF JSON structure (similar to Wire but more capable)
- Multipass rendering support
- Persistent buffers for true feedback loops
- Audio FFT input type

## Example Conversion Prompt

When adapting a preset, structure your response like this:

```
## MilkDrop Preset Analysis

**Visual Effect**: [Describe what it looks like]

**Core Techniques**:
- [List main techniques used]

**Audio Mapping**:
- [How audio affects visuals]

**Conversion Notes for [Target Platform]**:
- [Platform-specific considerations]
- [What can/cannot be directly translated]
- [Workarounds needed]

## Implementation

[Then invoke the appropriate platform skill or provide the implementation]
```

## Reference

For detailed MilkDrop variable and function reference, see [reference/milkdrop-reference.md](reference/milkdrop-reference.md).
