# Converting Shaders to ISF

## From Shadertoy

### Variable Mappings

| Shadertoy | ISF |
|-----------|-----|
| `iResolution` | `RENDERSIZE` |
| `iTime` | `TIME` |
| `iTimeDelta` | `TIMEDELTA` |
| `iFrame` | `FRAMEINDEX` |
| `iDate` | `DATE` |
| `fragCoord` | `gl_FragCoord.xy` |
| `fragColor` | `gl_FragColor` |
| `iChannel0` | Declare in INPUTS, use `IMG_NORM_PIXEL` |

### Function Mappings

| Shadertoy | ISF |
|-----------|-----|
| `texture(iChannel0, uv)` | `IMG_NORM_PIXEL(inputImage, uv)` |
| `texelFetch(...)` | `IMG_PIXEL(inputImage, coord)` |

### Conversion Steps

1. Add JSON header with `ISFVSN`, `DESCRIPTION`, `INPUTS`
2. Replace `mainImage(out vec4 fragColor, in vec2 fragCoord)` with `void main()`
3. Replace Shadertoy variables with ISF equivalents
4. Replace `texture()` calls with `IMG_NORM_PIXEL()`
5. Change `fragColor = ...` to `gl_FragColor = ...`
6. Ensure alpha is set: `gl_FragColor.a = 1.0;`

### Example Conversion

**Shadertoy:**
```glsl
void mainImage(out vec4 fragColor, in vec2 fragCoord) {
    vec2 uv = fragCoord / iResolution.xy;
    vec3 col = 0.5 + 0.5 * cos(iTime + uv.xyx + vec3(0, 2, 4));
    fragColor = vec4(col, 1.0);
}
```

**ISF:**
```glsl
/*{
    "ISFVSN": "2.0",
    "DESCRIPTION": "Converted from Shadertoy",
    "CATEGORIES": ["Generator"]
}*/

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    vec3 col = 0.5 + 0.5 * cos(TIME + uv.xyx + vec3(0, 2, 4));
    gl_FragColor = vec4(col, 1.0);
}
```

## From GLSL Sandbox

### Variable Mappings

| GLSL Sandbox | ISF |
|--------------|-----|
| `resolution` | `RENDERSIZE` |
| `time` | `TIME` |
| `mouse` | Declare as `point2D` input |
| `surfacePosition` | `isf_FragNormCoord` |

## From Book of Shaders

### Variable Mappings

| Book of Shaders | ISF |
|-----------------|-----|
| `u_resolution` | `RENDERSIZE` |
| `u_time` | `TIME` |
| `u_mouse` | Declare as `point2D` input |

### Example

**Book of Shaders:**
```glsl
uniform vec2 u_resolution;
uniform float u_time;

void main() {
    vec2 st = gl_FragCoord.xy / u_resolution;
    gl_FragColor = vec4(st.x, st.y, 0.0, 1.0);
}
```

**ISF:**
```glsl
/*{
    "ISFVSN": "2.0",
    "CATEGORIES": ["Generator"]
}*/

void main() {
    vec2 st = isf_FragNormCoord;
    gl_FragColor = vec4(st.x, st.y, 0.0, 1.0);
}
```

## Multipass Shader Considerations for Wire

Standard ISF multipass uses `PASSES` array, but **Wire doesn't support this**.

### Converting Multipass to Single-Pass

If a shader uses multiple passes for blur/effects:
1. Combine pass logic into single `main()`
2. Use loop-based approaches instead of separate passes
3. Accept quality tradeoffs for real-time performance

### Converting Feedback Effects

**Standard ISF (won't work in Wire):**
```glsl
"PASSES": [{"TARGET": "buffer", "PERSISTENT": true}]
```

**Wire-compatible approach:**
```glsl
/*{
    "INPUTS": [
        {"NAME": "inputImage", "TYPE": "image"},
        {"NAME": "feedbackIn", "TYPE": "image"},
        {"NAME": "decay", "TYPE": "float", "DEFAULT": 0.95}
    ]
}*/

void main() {
    vec4 current = IMG_THIS_PIXEL(inputImage);
    vec4 previous = IMG_THIS_PIXEL(feedbackIn);
    gl_FragColor = max(current, previous * decay);
}
```

Then in Wire: route shader output back to `feedbackIn` input.

## Common Issues

### Black or Transparent Output
- Ensure `gl_FragColor.a = 1.0` for opaque output
- Check for division by zero in UV calculations

### Texture Coordinate Issues
- ISF uses bottom-left origin (0,0)
- Some sources use top-left; flip Y: `uv.y = 1.0 - uv.y`

### Missing Uniforms
- Remove declarations of ISF auto-uniforms (`TIME`, `RENDERSIZE`, etc.)
- ISF declares them automatically; redeclaring causes errors

### Resolution Independence
- Use `isf_FragNormCoord` instead of pixel coords when possible
- Multiply by aspect ratio for correct proportions:
  ```glsl
  vec2 uv = isf_FragNormCoord;
  uv.x *= RENDERSIZE.x / RENDERSIZE.y;
  ```
