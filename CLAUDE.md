# CLAUDE.md - Shaders Project

## Planning First Workflow

**IMPORTANT: Always plan before writing code.**

When starting any task:
1. Explore the codebase to understand existing patterns and structure
2. Ask clarifying questions before making assumptions
3. Create a written plan and get approval before implementing
4. Use extended thinking (`think hard` or `ultrathink`) for complex shader logic

## Questions to Ask Before Starting

Before implementing anything, consider and ask about:
- What is the target platform/environment? (TouchDesigner, Resolume, Synesthesia, MadMapper, etc.)
- What audio reactivity is needed? (bass, mids, highs, beat detection)
- What visual style or aesthetic is desired?
- Are there reference images, videos, or existing shaders to draw from?
- What performance constraints exist?
- How will this integrate with existing effects?

## When to Plan vs Execute

**Plan first when:**
- Creating a new shader from scratch
- Adapting effects between platforms (e.g., MilkDrop to ISF)
- Implementing complex visual algorithms
- The request involves multiple files or components

**Execute directly when:**
- Fixing a syntax error
- Adjusting a single parameter value
- Adding a simple uniform or input

## Code Style

- Use descriptive variable names for shader parameters
- Comment complex math operations
- Group related uniforms together
- Follow platform-specific conventions (ISF JSON headers, TD uniforms, etc.)

## Workflow Commands

- Check existing skills with `/help` for platform-specific shader writing
- Use subagents for exploring shader patterns across the codebase
- Reference `project-specs/` for detailed project requirements

## Key Principle

When in doubt, ask. A few clarifying questions upfront prevent wasted iterations later. Understanding the creative vision and technical constraints is essential for shader work.
