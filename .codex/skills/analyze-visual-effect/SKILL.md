---
name: analyze-visual-effect
description: Thoroughly analyzes and explains shaders, presets, and visual effect files. Use before adapting effects between platforms to ensure deep understanding of algorithms, techniques, and principles. Creates detailed breakdowns that serve as intermediate documentation for adaptation workflows.
metadata:
  short-description: Deep analysis of shaders and visual effects
---

# Visual Effect Analysis

This skill produces a comprehensive breakdown of an existing shader, preset, or visual effect file. The analysis captures the algorithms, techniques, and principles that make the effect work - essential knowledge before attempting adaptation to another platform.

## When to Use This Skill

- **Before adaptation**: Run this analysis before using `adapt-milkdrop-preset` or converting between platforms
- **Learning**: When studying interesting effects to understand techniques
- **Documentation**: Creating reference material for complex effects
- **Recreation**: When you want to build something similar from scratch

## Workflow

### Step 1: Receive the Source File

The user provides a shader, preset, or effect file. Supported formats include:
- MilkDrop presets (.milk)
- ISF shaders (.fs)
- GLSL shaders (.glsl, .frag)
- TouchDesigner GLSL
- Synesthesia scenes
- Shadertoy code
- Any other shader/effect format

### Step 2: Perform Deep Analysis

Read through the code carefully, tracing execution flow and understanding each component. Take time to understand the "why" behind implementation choices.

### Step 3: Create Analysis Document

Save the analysis to `/project-specs/analysis-[effect-name].md`.

**CRITICAL: Present the analysis to the user for review before any adaptation work begins.**

---

## Analysis Document Template

```markdown
# Effect Analysis: [Effect Name]

## Source Information
- **Format**: [MilkDrop / ISF / GLSL / Synesthesia / etc.]
- **Original Author**: [if known]
- **Source File**: [filename or path]

---

## Visual Description

### What You See
[Describe the visual output in plain language. What does it look like? What's the overall aesthetic? Describe as if to someone who can't see it.]

### Motion & Dynamics
[How does it move? What changes over time? Rhythms and patterns of motion.]

### Color Behavior
[Color palette, how colors shift, any notable color techniques.]

### Audio Response (if applicable)
[How does it react to music? Which frequencies affect what? Beat response?]

---

## Architectural Overview

### High-Level Flow
```
[Diagram or step-by-step of how the effect is structured]
Input → [Stage 1] → [Stage 2] → ... → Output
```

### Key Components
| Component | Purpose | Lines/Section |
|-----------|---------|---------------|
| [name] | [what it does] | [where in code] |

### Data Flow
[How do values flow through the shader? What feeds into what?]

---

## Algorithm Breakdown

### Core Technique: [Name of Primary Technique]

**What it is**: [Brief explanation of the algorithm/technique]

**How it works here**:
```glsl
// Relevant code snippet with annotations
[code]
```

**Step-by-step**:
1. [First operation and why]
2. [Second operation and why]
3. [How they combine]

**Why this technique**: [What makes this appropriate for this effect]

---

### Secondary Technique: [Name]
[Repeat the above structure for each significant technique]

---

## Mathematical Concepts

### Coordinate Systems
- **Primary coordinates**: [Cartesian, polar, UV, etc.]
- **Transformations applied**: [rotation, scaling, domain warping, etc.]
- **Space manipulation**: [repetition, mirroring, folding, etc.]

### Key Formulas

#### [Formula Name]
```
[Mathematical notation or code]
```
**Purpose**: [What this achieves visually]
**Parameters**: [What each variable controls]

### Noise & Randomness
- **Type(s) used**: [Perlin, simplex, value, FBM, etc.]
- **Application**: [How noise is used - displacement, color, alpha, etc.]
- **Octaves/Layers**: [If FBM, how many layers and their contribution]

---

## Visual Layers

### Layer Breakdown
| Layer | Generation Method | Blending | Purpose |
|-------|-------------------|----------|---------|
| [1] | [how created] | [blend mode] | [visual role] |
| [2] | [how created] | [blend mode] | [visual role] |

### Composition Strategy
[How layers combine to create the final image. Order of operations, masking, etc.]

---

## Feedback & Persistence (if applicable)

### Feedback Architecture
- **Method**: [How previous frame is accessed]
- **Decay**: [How old content fades - multiply, subtract, etc.]
- **Motion**: [Zoom, rotation, translation applied to feedback]

### Accumulation Behavior
[What builds up over time? How is infinite accumulation prevented?]

---

## Audio Reactivity Analysis (if applicable)

### Audio Inputs Used
| Input | Range | What It Affects | How |
|-------|-------|-----------------|-----|
| [bass/mid/treb/etc] | [typical values] | [visual parameter] | [mapping function] |

### Temporal Response
- **Smoothing**: [How audio is smoothed/filtered]
- **Attack/Release**: [Response characteristics]
- **Transient handling**: [How beats/hits are detected and used]

### Frequency-to-Visual Mapping
```
Bass → [visual effect]
Mid → [visual effect]
High → [visual effect]
```

---

## Parameter Analysis

### User Controls
| Parameter | Type | Range | Visual Effect | Sensitivity |
|-----------|------|-------|---------------|-------------|
| [name] | [type] | [min-max] | [what changes] | [subtle/dramatic] |

### Internal Parameters
| Parameter | Value | Purpose | Could Be Exposed? |
|-----------|-------|---------|-------------------|
| [name] | [value] | [why this value] | [yes/no and why] |

### Magic Numbers
[Document any hardcoded values that significantly affect the output]
- `0.7` at line X: [what it does, why this value]
- `3.14159` at line Y: [obvious - pi for rotation]

---

## Platform-Specific Features

### Features Used
- [Feature 1]: [how it's used]
- [Feature 2]: [how it's used]

### Platform Dependencies
[What aspects are tied to this specific platform vs. portable concepts?]

| Aspect | Platform-Specific? | Portable Equivalent |
|--------|-------------------|---------------------|
| [feature] | [yes/no] | [how to achieve elsewhere] |

---

## What Makes This Effect Interesting

### Key Innovations
[What's clever or novel about this effect?]

### Visual Impact Sources
[Why does it look good? What creates the appeal?]

### Technical Elegance
[Any particularly elegant solutions or efficient approaches?]

---

## Transferable Principles

### Techniques to Preserve in Adaptation
1. [Technique]: [why it's essential to the effect's character]
2. [Technique]: [why it's essential]

### Acceptable Approximations
1. [Aspect]: [can be simplified because...]
2. [Aspect]: [platform limitations make exact replication impossible, but...]

### What Could Be Lost
[Honest assessment of what might not translate to other platforms]

---

## Complexity Assessment

### Computational Complexity
- **Per-pixel operations**: [count/estimate]
- **Texture samples**: [count]
- **Loop iterations**: [if any]
- **Performance notes**: [any concerns]

### Conceptual Complexity
- **Techniques involved**: [simple/intermediate/advanced]
- **Understanding required**: [what background helps]

---

## Questions for Deeper Understanding

[List any aspects that remain unclear or would benefit from further investigation]

1. [Question about unclear aspect]
2. [Question about design choice]

---

## Glossary of Techniques Used

| Term | Definition | Use in This Effect |
|------|------------|-------------------|
| [technique] | [brief definition] | [how it's applied here] |

---

## References & Further Reading

- [Technique 1]: [link or resource for learning more]
- [Technique 2]: [link or resource]

---

**Analysis complete.** This document can now inform adaptation to other platforms using the appropriate platform skill.
```

---

## Analysis Guidelines

### Go Deep, Not Wide
Focus on truly understanding each technique rather than superficially cataloging everything. Explain the "why" behind implementation choices.

### Trace the Signal Path
Follow values from input to output. How do coordinates transform? How do colors evolve? What modulates what?

### Identify the Essence
What makes this effect THIS effect? What's the core idea that must be preserved vs. implementation details that could change?

### Be Honest About Uncertainty
If something is unclear, say so. Better to flag questions than to guess wrong.

### Use Visual Language
Describe visual outcomes, not just code operations. "Creates a pulsing glow" is more useful than "multiplies by a sine wave" when explaining the visual result.

### Document Magic Numbers
Hardcoded values often encode important aesthetic choices or mathematical relationships. Note what they do.

---

## Integration with Other Skills

### Before Adaptation
Run this analysis skill, then use the resulting document as input to:
- `adapt-milkdrop-preset` - for MilkDrop conversions
- Platform-specific writing skills - for recreation on new platforms

### Workflow Example
```
1. User provides: cool_preset.milk
2. Run: analyze-visual-effect → analysis-cool_preset.md
3. User reviews analysis, asks questions
4. Run: adapt-milkdrop-preset (referencing the analysis)
5. Produce adaptation plan with deep understanding
```

### Analysis Document as Reference
The analysis document becomes a reference that:
- Guides what must be preserved
- Identifies what can be approximated
- Documents techniques for future learning
- Serves as specification for adaptation

---

## Common Techniques to Look For

When analyzing, watch for these common shader techniques:

### Coordinate Manipulation
- UV scaling, rotation, translation
- Polar coordinate conversion
- Domain repetition (mod, fract)
- Domain warping (displacing UVs with noise)

### Noise & Patterns
- Perlin/Simplex noise
- Fractional Brownian Motion (FBM)
- Voronoi/cellular patterns
- Checkerboards, stripes, grids

### Distance Fields
- Signed Distance Functions (SDFs)
- Shape primitives (circle, box, etc.)
- Boolean operations (union, intersection)
- Smooth blending between shapes

### Color Techniques
- HSV/HSL manipulation
- Palette functions
- Gradient mapping
- Color cycling

### Blending & Compositing
- Additive, multiplicative, screen
- Alpha blending
- Masking
- Layer ordering

### Motion
- Time-based animation
- Audio modulation
- Feedback trails
- Particle-like motion

### Post-Processing
- Bloom/glow
- Blur
- Color grading
- Vignette
