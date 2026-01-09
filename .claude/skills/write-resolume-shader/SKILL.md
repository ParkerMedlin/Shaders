---
name: write-resolume-shader
description: Writes ISF (Interactive Shader Format) shaders for Resolume Wire. Use when creating visual effects, generators, or filters for Resolume, or when the user mentions ISF, GLSL shaders for VJ software, Wire, or visual effects programming.
---

# Writing ISF Shaders for Resolume Wire

## Wire-Specific Limitations

**CRITICAL: Resolume Wire does NOT support multipass rendering or persistent buffers.**

- No `PASSES` array in JSON - Wire ignores it
- No `PERSISTENT: true` buffers
- No `PASSINDEX` variable (always 0)
- No frame-to-frame state retention within the shader

**Feedback workaround**: Route shader output back into the shader as a second `image` input through Wire's node system. Design shaders expecting feedback as an explicit input parameter.

## ISF File Structure

```glsl
/*{
    "DESCRIPTION": "What this shader does",
    "CREDIT": "by Author",
    "ISFVSN": "2.0",
    "CATEGORIES": ["Category"],
    "INPUTS": [
        // Input definitions here
    ]
}*/

void main() {
    gl_FragColor = vec4(1.0, 0.0, 0.0, 1.0);
}
```

## Input Types

| TYPE | GLSL Variable | Description |
|------|---------------|-------------|
| `float` | `float` | Slider control |
| `bool` | `bool` | Toggle/checkbox |
| `event` | `bool` | Momentary trigger (true for one frame) |
| `long` | `int` | Popup menu selection |
| `color` | `vec4` | RGBA color picker |
| `point2D` | `vec2` | XY coordinate control |
| `image` | sampler | Image/video input |

**Input example**:
```json
{
    "NAME": "amount",
    "TYPE": "float",
    "DEFAULT": 0.5,
    "MIN": 0.0,
    "MAX": 1.0,
    "LABEL": "Effect Amount"
}
```

For complete input specifications, see [reference/inputs.md](reference/inputs.md).

## Auto-Declared Variables

| Variable | Type | Description |
|----------|------|-------------|
| `RENDERSIZE` | `vec2` | Output dimensions in pixels |
| `TIME` | `float` | Seconds since shader started |
| `TIMEDELTA` | `float` | Seconds since last frame |
| `FRAMEINDEX` | `int` | Frame counter |
| `DATE` | `vec4` | (year, month, day, secondsInDay) |
| `isf_FragNormCoord` | `vec2` | Normalized coords [0,0] to [1,1] |
| `gl_FragCoord` | `vec4` | Pixel coordinates |

## Image Sampling Functions

**Always use ISF functions instead of `texture2D`**:

```glsl
// Sample at normalized coordinates (0-1)
vec4 color = IMG_NORM_PIXEL(inputImage, isf_FragNormCoord);

// Sample at pixel coordinates
vec4 color = IMG_PIXEL(inputImage, gl_FragCoord.xy);

// Sample current fragment (shorthand)
vec4 color = IMG_THIS_PIXEL(inputImage);

// Get image dimensions
vec2 size = IMG_SIZE(inputImage);
```

## Common Patterns

### Generator (no image input)
```glsl
/*{
    "ISFVSN": "2.0",
    "CATEGORIES": ["Generator"],
    "INPUTS": [
        {"NAME": "speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0}
    ]
}*/

void main() {
    vec2 uv = isf_FragNormCoord;
    float pattern = sin(uv.x * 10.0 + TIME * speed);
    gl_FragColor = vec4(vec3(pattern), 1.0);
}
```

### Filter (processes inputImage)
```glsl
/*{
    "ISFVSN": "2.0",
    "CATEGORIES": ["Color Effect"],
    "INPUTS": [
        {"NAME": "inputImage", "TYPE": "image"},
        {"NAME": "intensity", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0}
    ]
}*/

void main() {
    vec4 color = IMG_THIS_PIXEL(inputImage);
    color.rgb = mix(color.rgb, 1.0 - color.rgb, intensity);
    gl_FragColor = color;
}
```

### Feedback-Ready Filter (for Wire routing)
```glsl
/*{
    "ISFVSN": "2.0",
    "DESCRIPTION": "Route output back to feedbackImage for feedback effects",
    "INPUTS": [
        {"NAME": "inputImage", "TYPE": "image"},
        {"NAME": "feedbackImage", "TYPE": "image"},
        {"NAME": "feedbackAmount", "TYPE": "float", "DEFAULT": 0.9, "MIN": 0.0, "MAX": 1.0}
    ]
}*/

void main() {
    vec4 fresh = IMG_THIS_PIXEL(inputImage);
    vec4 feedback = IMG_THIS_PIXEL(feedbackImage);
    gl_FragColor = mix(fresh, feedback, feedbackAmount);
}
```

## Best Practices for Wire

1. **Keep shaders single-pass** - All computation in one `main()` function
2. **Use explicit feedback inputs** - Design for external feedback routing
3. **Avoid frame-dependent state** - No persistent buffers available
4. **Set alpha explicitly** - Always set `gl_FragColor.a` to avoid transparency issues
5. **Use normalized coordinates** - `isf_FragNormCoord` for resolution independence

## Additional References

- [Input Types & JSON Structure](reference/inputs.md)
- [ISF & GLSL Functions](reference/functions.md)
- [Converting Shadertoy/GLSL to ISF](reference/conversion.md)
