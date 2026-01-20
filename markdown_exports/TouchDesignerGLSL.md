# TouchDesigner GLSL Reference

GLSL version: 4.60 (Vulkan-based). Do not include `#version` statements.

## Quick Reference

### Minimal Pixel Shader
```glsl
layout(location = 0) out vec4 fragColor;
void main() {
    vec4 color = texture(sTD2DInputs[0], vUV.st);
    fragColor = TDOutputSwizzle(color);
}
```

### Minimal Compute Shader
```glsl
void main() {
    vec4 color = vec4(1.0, 0.0, 0.0, 1.0);
    TDImageStoreOutput(0, ivec3(gl_GlobalInvocationID.xy, 0), color);
}
```

---

## Built-in Variables

### Pixel Shader Inputs
| Variable | Type | Description |
|----------|------|-------------|
| `vUV` | `vec3` | Texture coordinates (0-1). Use `vUV.st` for 2D sampling. Only available without custom vertex shader. |

### Input Sampler Arrays
```glsl
uniform sampler2D sTD2DInputs[TD_NUM_2D_INPUTS];
uniform sampler3D sTD3DInputs[TD_NUM_3D_INPUTS];
uniform sampler2DArray sTD2DArrayInputs[TD_NUM_2D_ARRAY_INPUTS];
uniform samplerCube sTDCubeInputs[TD_NUM_CUBE_INPUTS];
```

### Input Count Constants
```glsl
TD_NUM_2D_INPUTS
TD_NUM_3D_INPUTS
TD_NUM_2D_ARRAY_INPUTS
TD_NUM_CUBE_INPUTS
```

### Built-in Samplers
```glsl
uniform sampler2D sTDNoiseMap;   // 256x256 random noise (red channel)
uniform sampler1D sTDSineLookup; // Sine wave lookup (red channel, 0-1)
```

### Texture Info Structure
```glsl
struct TDTexInfo {
    vec4 res;   // (1/width, 1/height, width, height)
    vec4 depth; // (1/depth, depth, depthOffset, undefined)
};

uniform TDTexInfo uTD2DInfos[TD_NUM_2D_INPUTS];
uniform TDTexInfo uTD3DInfos[TD_NUM_3D_INPUTS];
uniform TDTexInfo uTD2DArrayInfos[TD_NUM_2D_ARRAY_INPUTS];
uniform TDTexInfo uTDCubeInfos[TD_NUM_CUBE_INPUTS];
uniform TDTexInfo uTDOutputInfo;
```

### Other Built-in Uniforms
```glsl
uniform int uTDCurrentDepth; // Current slice index (3D/2DArray output)
uniform int uTDPass;         // Current render pass (0-indexed)
```

---

## Built-in Functions

### Output (Required for Pixel Shaders)
```glsl
vec4 TDOutputSwizzle(vec4 c); // Pass all output colors through this
```

### Compute Shader Output
```glsl
void TDImageStoreOutput(uint index, ivec3 coord, vec4 color);
void TDImageStoreOutput(uint index, uvec3 coord, vec4 color);
vec4 TDImageLoadOutput(uint index, ivec3 coord);
vec4 TDImageLoadOutput(uint index, uvec3 coord);
// Note: TDOutputSwizzle applied automatically
```

### Noise (Returns -1 to 1)
```glsl
float TDPerlinNoise(vec2 v);
float TDPerlinNoise(vec3 v);
float TDPerlinNoise(vec4 v);
float TDSimplexNoise(vec2 v);
float TDSimplexNoise(vec3 v);
float TDSimplexNoise(vec4 v);
```

### Color Conversion
```glsl
vec3 TDHSVToRGB(vec3 c);
vec3 TDRGBToHSV(vec3 c);
vec4 TDDither(vec4 color); // Reduces banding
```

### Matrix Functions
```glsl
mat4 TDTranslate(float x, float y, float z);
mat3 TDRotateX(float radians);
mat3 TDRotateY(float radians);
mat3 TDRotateZ(float radians);
mat3 TDRotateOnAxis(float radians, vec3 axis); // axis must be normalized
mat3 TDScale(float x, float y, float z);
mat3 TDRotateToVector(vec3 forward, vec3 up);  // vectors auto-normalized
mat3 TDCreateRotMatrix(vec3 from, vec3 to);    // vectors must be normalized
```

---

## Sampling Patterns

### Basic 2D Sampling
```glsl
vec4 color = texture(sTD2DInputs[0], vUV.st);
```

### Sampling with Pixel Offset
```glsl
vec2 input2DOffset(int texIndex, int xOffset, int yOffset) {
    return vec2(vUV.s + float(xOffset) * uTD2DInfos[texIndex].res.s,
                vUV.t + float(yOffset) * uTD2DInfos[texIndex].res.t);
}
// Usage: texture(sTD2DInputs[0], input2DOffset(0, 1, 0)); // 1 pixel right
```

### 3D/Cube/Array Sampling
```glsl
texture(sTD3DInputs[0], vUV.stp);
texture(sTDCubeInputs[0], vUV.stp);
texture(sTD2DArrayInputs[0], vUV.stp);
```

### Non-Uniform Sampler Access
When sampler index depends on runtime values:
```glsl
int inputIndex = (vUV.s > 0.5) ? 0 : 1;
vec4 col = texture(sTD2DInputs[nonuniformEXT(inputIndex)], vUV.st);
```

### Compute Shader Sampling
No `vUV` available. Calculate coordinates manually:
```glsl
vec2 uv = vec2(gl_GlobalInvocationID.xy) / uTDOutputInfo.res.zw;
vec4 color = texture(sTD2DInputs[0], uv);
// Or use texelFetch for integer coords [0, width-1]:
vec4 color = texelFetch(sTD2DInputs[0], ivec2(gl_GlobalInvocationID.xy), 0);
```

---

## Custom Uniforms

Declare uniforms matching names set in GLSL TOP Vectors pages:
```glsl
uniform vec4 uColor;
uniform float uTime;
uniform vec2 uMouse;
```

---

## Multiple Output Buffers

Set "# of Color Buffers" parameter, then declare outputs:
```glsl
layout(location = 0) out vec4 fragColor;
layout(location = 1) out vec4 normalOut;
layout(location = 2) out vec4 dataOut;
```
Access additional buffers via Render Select TOP.

---

## 3D Textures / 2D Arrays

**Pixel Shader:** Runs once per depth slice. Use `uTDCurrentDepth` for slice index.

**Compute Shader:** Runs once. Write all slices via `TDImageStoreOutput`.

### Sampling Latest Slice of 3D Input
```glsl
float sliceCoord = uTD3DInfos[0].depth.x * 0.5 + uTD3DInfos[0].depth.z;
vec4 color = texture(sTD3DInputs[0], vec3(vUV.st, sliceCoord));
```

---

## Atomic Counters

```glsl
uniform atomic_uint ac;
// In main():
uint count = atomicCounterIncrement(ac);
```
Omit `layout(binding = N)` for automatic binding.

---

## Specialization Constants

For rarely-changed values (modes, code paths):
```glsl
layout(constant_id = 0) const int Mode = 0;
layout(constant_id = 1) const int FeatureEnabled = 1;
```
Set values on Constants page. Cached per value combination.

---

## POP Attribute Buffers

```glsl
attribType TDBuffer_AttribName(uint elementIndex, uint arrayIndex);
attribType TDBuffer_AttribName(uint elementIndex); // arrayIndex defaults to 0
const uint TDBufferLength_AttribName();
const uint cTDBufferArraySize_AttribName;
```

---

## Custom Vertex Shader

Usually unnecessary. If used, pass UV manually:
```glsl
// Vertex shader
out vec3 texCoord;
void main() {
    texCoord = uv[0];
    gl_Position = TDSOPToProj(vec4(P, 1.0));
}

// Pixel shader
in vec3 texCoord;
layout(location = 0) out vec4 fragColor;
void main() {
    fragColor = TDOutputSwizzle(texture(sTD2DInputs[0], texCoord.st));
}
```
Do not modify vertex positions (breaks quad alignment).

---

## Common Patterns

### Solid Color
```glsl
layout(location = 0) out vec4 fragColor;
void main() {
    fragColor = TDOutputSwizzle(vec4(1.0, 0.0, 0.0, 1.0));
}
```

### UV Visualization
```glsl
layout(location = 0) out vec4 fragColor;
void main() {
    fragColor = vec4(vUV.s, vUV.t, 0.0, 1.0);
}
```

### 3x3 Box Blur
```glsl
vec2 input2DOffset(int texIndex, int x, int y) {
    return vec2(vUV.s + float(x) * uTD2DInfos[texIndex].res.s,
                vUV.t + float(y) * uTD2DInfos[texIndex].res.t);
}

layout(location = 0) out vec4 fragColor;
void main() {
    vec4 sum = vec4(0.0);
    for (int y = -1; y <= 1; y++) {
        for (int x = -1; x <= 1; x++) {
            sum += texture(sTD2DInputs[0], input2DOffset(0, x, y));
        }
    }
    fragColor = TDOutputSwizzle(sum / 9.0);
}
```

### Get Resolution
```glsl
float width = uTD2DInfos[0].res.z;
float height = uTD2DInfos[0].res.w;
vec2 pixelSize = uTD2DInfos[0].res.xy; // (1/width, 1/height)
```
