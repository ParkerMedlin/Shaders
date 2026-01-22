# TouchDesigner GLSL Reference

TouchDesigner uses GLSL 4.60 (Vulkan-based). Shaders run once per pixel on a full-screen quad.

## Shader Structure

### Pixel Shader (most common)

```glsl
layout(location = 0) out vec4 fragColor;

void main() {
    vec4 color = texture(sTD2DInputs[0], vUV.st);
    // Your processing here
    fragColor = TDOutputSwizzle(color);
}
```

### Compute Shader

```glsl
void main() {
    vec4 color = vec4(1.0, 0.0, 0.0, 1.0);
    TDImageStoreOutput(0, ivec3(gl_GlobalInvocationID.xy, 0), color);
}
```

## Critical Rules

1. **Always use `TDOutputSwizzle()`** on final color output in pixel shaders
2. **Do NOT include `#version` statements** - TouchDesigner adds this automatically
3. **Use `vUV.st`** for texture coordinates (only available without custom vertex shader)
4. **Sample inputs via ISF-style arrays**: `sTD2DInputs[0]`, `sTD3DInputs[0]`, etc.

## Built-in Variables

| Variable | Type | Description |
|----------|------|-------------|
| `vUV` | `vec3` | Normalized texture coords (0-1). Use `.st` for 2D. |
| `sTD2DInputs[]` | `sampler2D[]` | 2D texture inputs |
| `sTD3DInputs[]` | `sampler3D[]` | 3D texture inputs |
| `sTDCubeInputs[]` | `samplerCube[]` | Cube map inputs |
| `sTDNoiseMap` | `sampler2D` | 256x256 random noise |
| `uTDOutputInfo.res` | `vec4` | (1/w, 1/h, width, height) |
| `uTD2DInfos[n].res` | `vec4` | Input resolution info |
| `uTDCurrentDepth` | `int` | Current slice (3D/2DArray) |
| `uTDPass` | `int` | Current render pass |

### Input Count Constants

```glsl
TD_NUM_2D_INPUTS
TD_NUM_3D_INPUTS
TD_NUM_2D_ARRAY_INPUTS
TD_NUM_CUBE_INPUTS
```

## Built-in Functions

### Required

```glsl
vec4 TDOutputSwizzle(vec4 c);  // Always wrap final output
```

### Compute Shader Output

```glsl
void TDImageStoreOutput(uint index, ivec3 coord, vec4 color);
vec4 TDImageLoadOutput(uint index, ivec3 coord);
// TDOutputSwizzle applied automatically
```

### Noise (returns -1 to 1)

```glsl
float TDPerlinNoise(vec2/vec3/vec4 v);
float TDSimplexNoise(vec2/vec3/vec4 v);
```

### Color

```glsl
vec3 TDHSVToRGB(vec3 c);
vec3 TDRGBToHSV(vec3 c);
vec4 TDDither(vec4 color);
```

### Matrix

```glsl
mat4 TDTranslate(float x, float y, float z);
mat3 TDRotateX/Y/Z(float radians);
mat3 TDRotateOnAxis(float radians, vec3 normalizedAxis);
mat3 TDScale(float x, float y, float z);
```

## Common Patterns

### Passthrough Filter

```glsl
layout(location = 0) out vec4 fragColor;

void main() {
    vec4 color = texture(sTD2DInputs[0], vUV.st);
    fragColor = TDOutputSwizzle(color);
}
```

### Generator (no input)

```glsl
layout(location = 0) out vec4 fragColor;
uniform float uTime;

void main() {
    vec2 uv = vUV.st;
    float pattern = sin(uv.x * 20.0 + uTime);
    fragColor = TDOutputSwizzle(vec4(vec3(pattern * 0.5 + 0.5), 1.0));
}
```

### Color Effect

```glsl
layout(location = 0) out vec4 fragColor;
uniform float uIntensity;

void main() {
    vec4 color = texture(sTD2DInputs[0], vUV.st);
    color.rgb = mix(color.rgb, 1.0 - color.rgb, uIntensity);
    fragColor = TDOutputSwizzle(color);
}
```

### Sampling Neighbor Pixels

```glsl
vec2 pixelOffset(int texIndex, int x, int y) {
    return vec2(vUV.s + float(x) * uTD2DInfos[texIndex].res.s,
                vUV.t + float(y) * uTD2DInfos[texIndex].res.t);
}

// Usage: sample 1 pixel to the right
vec4 rightPixel = texture(sTD2DInputs[0], pixelOffset(0, 1, 0));
```

### 3x3 Box Blur

```glsl
layout(location = 0) out vec4 fragColor;

vec2 offset(int x, int y) {
    return vec2(vUV.s + float(x) * uTD2DInfos[0].res.s,
                vUV.t + float(y) * uTD2DInfos[0].res.t);
}

void main() {
    vec4 sum = vec4(0.0);
    for (int y = -1; y <= 1; y++) {
        for (int x = -1; x <= 1; x++) {
            sum += texture(sTD2DInputs[0], offset(x, y));
        }
    }
    fragColor = TDOutputSwizzle(sum / 9.0);
}
```

### Multiple Output Buffers

Set "# of Color Buffers" parameter, then:

```glsl
layout(location = 0) out vec4 fragColor;
layout(location = 1) out vec4 normalOut;
layout(location = 2) out vec4 dataOut;

void main() {
    fragColor = TDOutputSwizzle(vec4(1.0, 0.0, 0.0, 1.0));
    normalOut = TDOutputSwizzle(vec4(0.0, 1.0, 0.0, 1.0));
    dataOut = TDOutputSwizzle(vec4(0.0, 0.0, 1.0, 1.0));
}
```

Access additional buffers via Render Select TOP.

## Custom Uniforms

Declare uniforms matching names in GLSL TOP's Vectors pages:

```glsl
uniform float uTime;
uniform vec4 uColor;
uniform vec2 uMouse;
```

## Compute Shader Specifics

- No `vUV` available - calculate coordinates manually:

```glsl
vec2 uv = vec2(gl_GlobalInvocationID.xy) / uTDOutputInfo.res.zw;
```

- Or use `texelFetch` for integer coordinates:

```glsl
vec4 color = texelFetch(sTD2DInputs[0], ivec2(gl_GlobalInvocationID.xy), 0);
```

## Non-Uniform Sampler Access

When sampler index depends on runtime values:

```glsl
int idx = (vUV.s > 0.5) ? 0 : 1;
vec4 col = texture(sTD2DInputs[nonuniformEXT(idx)], vUV.st);
```
