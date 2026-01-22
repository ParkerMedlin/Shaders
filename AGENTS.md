# AGENTS.md - Shaders Project

## Working Agreements

- Always plan before writing code. Create a written plan and get approval before implementing.
- Ask clarifying questions before making assumptions about requirements.
- Use extended thinking for complex shader algorithms and visual effects logic.
- Explore the codebase to understand existing patterns before adding new code.

## Questions to Ask Before Starting

Before implementing any shader or effect, ask about:
- Target platform/environment (TouchDesigner, Resolume, Synesthesia, MadMapper, etc.)
- Audio reactivity requirements (bass, mids, highs, beat detection, FFT)
- Desired visual style or aesthetic
- Reference images, videos, or existing shaders to draw from
- Performance constraints and target frame rate
- Integration with existing effects in the project

## Repository Expectations

- Check `project-specs/` for detailed project requirements before starting work.
- Follow platform-specific conventions (ISF JSON headers, TouchDesigner uniforms, etc.).
- Use descriptive variable names for shader parameters.
- Comment complex math operations and algorithmic choices.
- Group related uniforms together at the top of shader files.

## When to Plan vs Execute Directly

**Require a plan first:**
- Creating a new shader from scratch
- Adapting effects between platforms (e.g., MilkDrop to ISF)
- Implementing complex visual algorithms
- Tasks involving multiple files or components
- Any request where the creative vision is unclear

**Can execute directly:**
- Fixing syntax errors
- Adjusting single parameter values
- Adding simple uniforms or inputs
- Minor formatting changes

## Key Principle

When in doubt, ask. A few clarifying questions upfront prevent wasted iterations. Understanding the creative vision and technical constraints is essential for shader work.
