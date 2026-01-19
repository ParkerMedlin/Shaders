---
name: write-madmapper-shader
description: Writes ISF shaders for MadMapper. Use when creating visual effects, generators, or filters for MadMapper projection mapping software.
---

# Writing ISF Shaders for MadMapper

MadMapper uses ISF (Interactive Shader Format) for custom shaders. ISF is a JSON+GLSL format that defines inputs, outputs, and shader code in a single file.

## ISF File Structure

```glsl
/*{
    "DESCRIPTION": "What this shader does",
    "CREDIT": "Author Name",
    "ISFVSN": "2.0",
    "CATEGORIES": ["Generator", "Color Effect"],
    "INPUTS": [
        // Input definitions
    ]
}*/

void main() {
    vec4 color = vec4(1.0, 0.0, 0.0, 1.0);
    gl_FragColor = color;
}
```

## Input Types

### Float (Slider)

```json
{
    "NAME": "intensity",
    "TYPE": "float",
    "DEFAULT": 1.0,
    "MIN": 0.0,
    "MAX": 2.0,
    "LABEL": "Intensity"
}
```

### Boolean (Toggle)

```json
{
    "NAME": "invert",
    "TYPE": "bool",
    "DEFAULT": false,
    "LABEL": "Invert Colors"
}
```

### Color Picker

```json
{
    "NAME": "tintColor",
    "TYPE": "color",
    "DEFAULT": [1.0, 0.5, 0.0, 1.0],
    "LABEL": "Tint Color"
}
```

### Point 2D (XY Pad)

```json
{
    "NAME": "center",
    "TYPE": "point2D",
    "DEFAULT": [0.5, 0.5],
    "MIN": [0.0, 0.0],
    "MAX": [1.0, 1.0],
    "LABEL": "Center Point"
}
```

### Long (Menu/Dropdown)

```json
{
    "NAME": "mode",
    "TYPE": "long",
    "DEFAULT": 0,
    "VALUES": [0, 1, 2],
    "LABELS": ["Normal", "Additive", "Multiply"],
    "LABEL": "Blend Mode"
}
```

### Image Input

```json
{
    "NAME": "inputImage",
    "TYPE": "image",
    "LABEL": "Input"
}
```

### Audio FFT Input

```json
{
    "NAME": "audioFFT",
    "TYPE": "audioFFT",
    "LABEL": "Audio Spectrum"
}
```

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

**Always use ISF functions instead of `texture2D`:**

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

## Audio Input

MadMapper provides audio analysis through special input types:

```json
{
    "NAME": "audioFFT",
    "TYPE": "audioFFT"
}
```

```glsl
void main() {
    vec2 uv = isf_FragNormCoord;

    // Sample FFT - x position determines frequency (0=bass, 1=treble)
    float bass = IMG_NORM_PIXEL(audioFFT, vec2(0.05, 0.0)).r;
    float mid = IMG_NORM_PIXEL(audioFFT, vec2(0.3, 0.0)).r;
    float treble = IMG_NORM_PIXEL(audioFFT, vec2(0.7, 0.0)).r;

    // Use audio values
    vec3 color = vec3(bass, mid, treble);
    gl_FragColor = vec4(color, 1.0);
}
```

## Multipass Rendering

MadMapper ISF supports multipass rendering via the `PASSES` array:

```json
{
    "PASSES": [
        {
            "TARGET": "blurPass1",
            "WIDTH": "floor($WIDTH/4.0)",
            "HEIGHT": "floor($HEIGHT/4.0)"
        },
        {
            "TARGET": "blurPass2"
        },
        {}
    ]
}
```

Use `PASSINDEX` to determine which pass is executing:

```glsl
void main() {
    if (PASSINDEX == 0) {
        // First pass - downsample
        gl_FragColor = IMG_THIS_PIXEL(inputImage);
    } else if (PASSINDEX == 1) {
        // Second pass - blur
        gl_FragColor = blur(blurPass1, isf_FragNormCoord);
    } else {
        // Final pass - composite
        vec4 original = IMG_THIS_PIXEL(inputImage);
        vec4 blurred = IMG_NORM_PIXEL(blurPass2, isf_FragNormCoord);
        gl_FragColor = mix(original, blurred, blurAmount);
    }
}
```

## Persistent Buffers (Feedback)

For feedback effects, use persistent buffers:

```json
{
    "PASSES": [
        {
            "TARGET": "feedbackBuffer",
            "PERSISTENT": true
        },
        {}
    ]
}
```

```glsl
void main() {
    vec2 uv = isf_FragNormCoord;

    if (PASSINDEX == 0) {
        // Read previous frame, apply decay and motion
        vec2 feedbackUV = (uv - 0.5) * 0.99 + 0.5;  // Slight zoom
        vec4 feedback = IMG_NORM_PIXEL(feedbackBuffer, feedbackUV);
        feedback *= 0.95;  // Decay

        // Add new content
        vec4 newContent = IMG_THIS_PIXEL(inputImage);
        gl_FragColor = max(feedback, newContent);
    } else {
        // Final output
        gl_FragColor = IMG_NORM_PIXEL(feedbackBuffer, uv);
    }
}
```

## Common Patterns

### Generator (No Image Input)

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
    float pattern = sin(uv.x * 20.0 + TIME * speed) * sin(uv.y * 20.0 + TIME * speed);
    gl_FragColor = vec4(vec3(pattern * 0.5 + 0.5), 1.0);
}
```

### Filter (Processes Image)

```glsl
/*{
    "ISFVSN": "2.0",
    "CATEGORIES": ["Color Effect"],
    "INPUTS": [
        {"NAME": "inputImage", "TYPE": "image"},
        {"NAME": "brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0}
    ]
}*/

void main() {
    vec4 color = IMG_THIS_PIXEL(inputImage);
    color.rgb *= brightness;
    gl_FragColor = color;
}
```

### Audio-Reactive Generator

```glsl
/*{
    "ISFVSN": "2.0",
    "CATEGORIES": ["Generator"],
    "INPUTS": [
        {"NAME": "audioFFT", "TYPE": "audioFFT"},
        {"NAME": "sensitivity", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0}
    ]
}*/

void main() {
    vec2 uv = isf_FragNormCoord;

    // Get audio level at this x position
    float audio = IMG_NORM_PIXEL(audioFFT, vec2(uv.x, 0.0)).r * sensitivity;

    // Draw bars
    float bar = step(uv.y, audio);

    // Color by frequency
    vec3 color = vec3(uv.x, 0.5, 1.0 - uv.x) * bar;

    gl_FragColor = vec4(color, 1.0);
}
```

## MilkDrop Conversion Notes

When converting from MilkDrop:

| MilkDrop | MadMapper ISF |
|----------|---------------|
| `time` | `TIME` |
| `bass`, `mid`, `treb` | Sample `audioFFT` texture |
| `uv` | `isf_FragNormCoord` |
| `texsize` | `RENDERSIZE` |
| `sampler_main` | `inputImage` (declare as input) |
| `decay` | Use persistent buffer with multiplication |
| Per-vertex motion | Per-pixel UV manipulation |
| Q variables | Shader uniforms/inputs |

### Key Differences from MilkDrop

1. **No implicit feedback**: Must use `PERSISTENT` buffers explicitly
2. **No mesh/vertex system**: All motion is per-pixel UV manipulation
3. **Audio via texture**: No pre-analyzed bass/mid/treb - sample FFT yourself
4. **ISF image functions**: Use `IMG_NORM_PIXEL` etc., not `texture2D`

### Key Differences from Resolume Wire

MadMapper ISF **does** support:
- Multipass rendering (`PASSES` array)
- Persistent buffers (`PERSISTENT: true`)
- `PASSINDEX` variable

This makes it more capable than Wire for complex effects.

## Categories

Common MadMapper categories:
- `Generator` - Creates visuals from nothing
- `Color Effect` - Modifies colors
- `Distortion` - Warps geometry
- `Blur` - Blur effects
- `Stylize` - Artistic effects
- `Transition` - For mixing between sources

## Best Practices

1. **Set alpha explicitly**: Always set `gl_FragColor.a` to avoid transparency issues

2. **Use normalized coordinates**: `isf_FragNormCoord` for resolution independence

3. **Efficient blur**: Use multipass with downsampled targets for blur effects

4. **Audio smoothing**: Raw FFT is noisy - apply temporal smoothing or use multiple samples

5. **Test different resolutions**: MadMapper may run shaders at various sizes for mapping

6. **Provide good defaults**: Users should see something interesting without adjusting parameters

---

*Note: This skill covers general MadMapper ISF conventions. Some features may vary between MadMapper versions. Test shaders in the actual application.*
