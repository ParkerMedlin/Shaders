---
name: write-synesthesia-shader
description: Writes GLSL shaders for Synesthesia music visualizer. Use when creating audio-reactive visuals for Synesthesia, or converting effects to Synesthesia format.
---

# Writing Shaders for Synesthesia

Synesthesia uses the Synesthesia Shader Format (SSF). Scenes are directories with a `.synScene` extension containing:

```
sceneName.synScene/
├── main.glsl        # GLSL fragment shader (required)
├── scene.json       # JSON configuration (required)
├── thumbnail.png    # Preview image
├── script.js        # JavaScript scripting (optional)
└── images/          # Bundled textures (optional)
```

## Shader Structure

```glsl
// Synesthesia provides built-in uniforms - don't redeclare them
// vec2 RENDERSIZE, float TIME, vec2 _uv, vec2 _uvc, etc.

vec4 renderMain() {
    // _uv: normalized coordinates (0-1), origin at bottom-left
    // _uvc: aspect-corrected coordinates, origin at center

    vec3 color = vec3(0.0);

    // Your shader code here

    return vec4(color, 1.0);
}
```

## Built-in Uniforms

### Standard Uniforms

| Uniform | Type | Description |
|---------|------|-------------|
| `RENDERSIZE` | `vec2` | Viewport resolution in pixels |
| `TIME` | `float` | Seconds since scene started |
| `FRAMECOUNT` | `float` | Frames since scene started |
| `_xy` | `vec2` | Current pixel position (gl_FragCoord.xy) |
| `_uv` | `vec2` | Normalized position (0-1), origin bottom-left |
| `_uvc` | `vec2` | Aspect-corrected position, origin center |
| `PI` | `float` | 3.14159... |

### Mouse Uniforms

| Uniform | Type | Description |
|---------|------|-------------|
| `_mouse` | `vec4` | Shadertoy-style mouse (x, y, click x, click y) |
| `_click` | `vec3` | Button states (left, right, middle) - 1.0 when pressed |
| `_muv` | `vec2` | Normalized mouse position (0-1) |
| `_muvc` | `vec2` | Aspect-corrected mouse position |

### Audio Textures

**`syn_Spectrum`** (sampler2D) - Audio frequency and waveform data:
- **r channel**: Raw FFT (unprocessed)
- **g channel**: Juiced FFT (smoothed, log-scaled - **use this by default**)
- **b channel**: Smooth FFT (low frequency resolution, smoother shape)
- **a channel**: Waveform (raw audio waveform)

**`syn_LevelTrail`** (sampler2D) - Frequency band levels over time:
- **r**: Whole spectrum | **g**: Bass | **b**: Mid | **a**: High

```glsl
// Sample the processed FFT (recommended)
float bass = texture(syn_Spectrum, vec2(0.05, 0.0)).g;
float mid = texture(syn_Spectrum, vec2(0.3, 0.0)).g;
float treble = texture(syn_Spectrum, vec2(0.8, 0.0)).g;

// Sample waveform
float wave = texture(syn_Spectrum, vec2(_uv.x, 0.0)).a;
```

### Audio Level Uniforms

Normalized 0.0-1.0, smoothed to reduce jitter:

| Uniform | Description |
|---------|-------------|
| `syn_Level` | Overall volume |
| `syn_BassLevel` | Bass frequency level |
| `syn_MidLevel` | Mid frequency level |
| `syn_MidHighLevel` | Mid-high frequency level |
| `syn_HighLevel` | High frequency level |

### Audio Hit Uniforms

Spike on transients (drums, percussive sounds):

| Uniform | Description |
|---------|-------------|
| `syn_Hits` | Whole spectrum hits |
| `syn_BassHits` | Bass hits (kicks) |
| `syn_MidHits` | Mid hits |
| `syn_MidHighHits` | Mid-high hits |
| `syn_HighHits` | High hits (hi-hats) |

### Audio Time Uniforms

Clocks that advance faster when volume is high:

| Uniform | Description |
|---------|-------------|
| `syn_Time` | Whole spectrum time |
| `syn_BassTime` | Bass-driven time |
| `syn_MidTime` | Mid-driven time |
| `syn_HighTime` | High-driven time |
| `syn_CurvedTime` | Higher acceleration on volume increases |

### Audio Presence Uniforms

Track rising/falling energy without reacting to individual sounds:

| Uniform | Description |
|---------|-------------|
| `syn_Presence` | Overall presence |
| `syn_BassPresence` | "There is bass presence right now" |
| `syn_MidPresence` | Mid presence |
| `syn_HighPresence` | High presence |

### Beat Detection Uniforms

| Uniform | Description |
|---------|-------------|
| `syn_OnBeat` | 1.0 on beat, quickly falls to 0.0 |
| `syn_ToggleOnBeat` | Toggles between 0.0 and 1.0 each beat (smooth transition) |
| `syn_RandomOnBeat` | New random 0.0-1.0 each beat (smooth transition) |
| `syn_BeatTime` | Increments by 1.0 each beat |

### BPM Uniforms

| Uniform | Description |
|---------|-------------|
| `syn_BPM` | Detected BPM (50-220) |
| `syn_BPMConfidence` | BPM stability (0.0 = shifting, 1.0 = consistent) |
| `syn_BPMTwitcher` | Clock incrementing each beat with smooth jump |
| `syn_BPMSin` | Sine wave synced to BPM (0.0-1.0) |
| `syn_BPMSin2` | Half-speed sine wave |
| `syn_BPMSin4` | Quarter-speed sine wave |
| `syn_BPMTri` | Triangle wave synced to BPM |
| `syn_BPMTri2` / `syn_BPMTri4` | Slower triangle waves |

### Large Feature Uniforms

| Uniform | Description |
|---------|-------------|
| `syn_FadeInOut` | Rises as music starts, falls as it ends |
| `syn_Intensity` | Accumulates based on song intensity |

### Media Uniforms

| Uniform | Type | Description |
|---------|------|-------------|
| `syn_Media` | `sampler2D` | User-selected media texture |
| `syn_MediaType` | `float` | 0=None, 1=Image, 2=Video, 3=Webcam |

### Multipass Uniforms

| Uniform | Type | Description |
|---------|------|-------------|
| `PASSINDEX` | `int` | Current pass index (0, 1, 2...) |
| `syn_FinalPass` | `sampler2D` | Previous frame (for feedback effects) |

## Built-in Functions

### Math

```glsl
float _scale(float value, float min, float max);  // Scale 0-1 to min-max
float _map(float val, float min, float max, float newMin, float newMax);
float _smin(float a, float b, float k);           // Smooth minimum
float _rand(float seed);                          // Pseudo-random 0-1
float _rand(vec2 seed);
float _pulse(float pos, float center, float size); // Smooth pulse
float _sqPulse(float pos, float center, float size); // Square pulse
float _inRange(float val, float min, float max);  // Returns 1.0 if in range
float _triWave(float pos, float period);          // Triangle wave
float _pixelate(float val, float amount);         // Quantize values
float _nsin(float x);  // Normalized sin (0-1)
float _ncos(float x);  // Normalized cos (0-1)
float _nclamp(float x); // Clamp to 0-1

// Mix between 3-5 values
vec3 _mix3(vec3 a, vec3 b, vec3 c, float t);
vec3 _mix4(vec3 a, vec3 b, vec3 c, vec3 d, float t);
vec3 _mix5(vec3 a, vec3 b, vec3 c, vec3 d, vec3 e, float t);
```

### Noise

```glsl
float _noise(float seed);   // 1D noise
float _noise(vec2 seed);    // 2D noise
float _noise(vec3 seed);    // 3D noise
float _fbm(vec2 seed);      // Fractal Brownian motion
float _fbm(vec2 seed, float octaves);

// Hash functions: _hash{outputDim}{inputDim}
float _hash11(float p);
vec2 _hash21(float p);
vec3 _hash33(vec3 p);
// etc.
```

### Coordinates

```glsl
void _uv2uvc(inout vec2 pos);  // Convert _uv to _uvc style
void _uvc2uv(inout vec2 pos);  // Convert _uvc to _uv style
vec2 _rotate(vec2 v, float theta);  // Rotate around origin
vec2 _mirror(vec2 v);          // Mirror for seamless tiling
vec2 _toPolar(vec2 xy);        // Cartesian to polar (r, theta)
vec2 _toPolarTrue(vec2 xy);    // With quadrant correction
vec2 _toRect(vec2 rt);         // Polar to cartesian
```

### Color

```glsl
vec3 _rgb2hsv(vec3 rgb);
vec3 _hsv2rgb(vec3 hsv);
vec3 _normalizeRGB(float r, float g, float b);  // 0-255 to 0-1
vec3 _palette(float t, vec3 bias, vec3 amp, vec3 freq, vec3 phase); // Cosine palette
vec3 _brightness(vec3 col, float brightness);  // 1.0 = no change
vec3 _gamma(vec3 col, float gamma);
vec3 _contrast(vec3 col, float contrast);
vec3 _saturation(vec3 col, float saturation);
vec3 _hueRotate(vec3 col, float degrees);
vec3 _invert(vec3 col, float amount);
float _luminance(vec3 col);
vec3 _grayscale(vec3 col);
vec3 _dichrome(vec3 col, vec3 darkColor, vec3 lightColor);
```

### Media

```glsl
vec4 _loadMedia();                    // Media at current pixel
vec4 _loadMedia(vec2 offset);         // With offset
vec4 _textureMedia(vec2 uv);          // Sample at UV (aspect-corrected)
vec4 _loadMediaAsMask();              // Thresholded to 0 or 1
vec4 _textureMediaAsMask(vec2 uv);
vec2 _correctMediaCoords(vec2 uv);    // Fix aspect ratio
bool _isMediaActive();                // Is media selected?
vec4 _edgeDetectMedia();              // Edge detection (bilinear)
vec4 _edgeDetectSobelMedia();         // Edge detection (Sobel)
```

### Multipass

```glsl
vec4 _edgeDetect(sampler2D tex);
vec4 _edgeDetect(sampler2D tex, vec2 uv);
vec4 _edgeDetectSobel(sampler2D tex);
vec4 _hBlurTiltShift(sampler2D tex, vec2 uv, float blur, float loc);
vec4 _vBlurTiltShift(sampler2D tex, vec2 uv, float blur, float loc);
```

## User Controls (scene.json)

Controls are defined in `scene.json`, not in GLSL comments:

```json
{
  "TITLE": "My Scene",
  "CREDIT": "Artist Name",
  "CONTROLS": [
    {
      "TYPE": "slider smooth",
      "NAME": "speed",
      "DEFAULT": 1.0,
      "MIN": 0.0,
      "MAX": 5.0,
      "UI_GROUP": "animation",
      "PARAMS": 0.05
    },
    {
      "TYPE": "color smooth",
      "NAME": "baseColor",
      "DEFAULT": [1.0, 0.5, 0.0],
      "UI_GROUP": "colors"
    },
    {
      "TYPE": "toggle smooth",
      "NAME": "enableEffect",
      "DEFAULT": 1.0,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "TYPE": "xy smooth",
      "NAME": "position",
      "DEFAULT": [0.5, 0.5],
      "MIN": [0.0, 0.0],
      "MAX": [1.0, 1.0]
    }
  ]
}
```

### Control Types

| Type | Output | Description |
|------|--------|-------------|
| `slider` | float | Basic slider |
| `slider smooth` | float | Smooth transitions |
| `slider speed` | float | Continuously changing value |
| `knob` / `knob smooth` | float | Compact slider |
| `toggle` / `toggle smooth` | float | On/off switch |
| `bang` | float | Trigger (1 frame) |
| `bang smooth` | float | Envelope trigger |
| `bang counter` | float | Increments on click |
| `xy` / `xy smooth` | vec2 | 2D position pad |
| `color` / `color smooth` | vec3 | Color picker |
| `dropdown` | float | Selection menu |

The `PARAMS` key controls smoothing speed (smaller = slower, default 0.01).

## Multipass Rendering

Define passes in `scene.json`:

```json
{
  "PASSES": [
    {
      "TARGET": "bufferA",
      "WIDTH": 1920,
      "HEIGHT": 1080,
      "FLOAT": true
    },
    {
      "TARGET": "bufferB",
      "WIDTH": 960,
      "HEIGHT": 540
    }
  ]
}
```

Use in shader:

```glsl
vec4 renderMain() {
    if (PASSINDEX == 0) {
        // First pass - render to bufferA
        return vec4(someEffect(_uv), 1.0);
    } else if (PASSINDEX == 1) {
        // Second pass - sample bufferA, render to bufferB
        return texture(bufferA, _uv) * 0.99;
    } else {
        // Final pass - combine
        return texture(bufferB, _uv);
    }
}
```

### Simple Feedback

For basic feedback without multipass config, use `syn_FinalPass`:

```glsl
vec4 renderMain() {
    vec2 feedbackUV = (_uv - 0.5) * 0.99 + 0.5;  // Slight zoom
    vec4 feedback = texture(syn_FinalPass, feedbackUV) * 0.95;
    vec4 newContent = vec4(/* ... */);
    return mix(newContent, feedback, 0.8);
}
```

## External Libraries

Include at the top of `main.glsl`:

```glsl
#include "lygia/generative/noise.glsl"
#include "lygia/color/blend.glsl"
#include "hg_sdf.glsl"  // Raymarching SDF library
```

**lygia**: Math, noise, color, SDF, filters, and more. See [lygia documentation](https://lygia.xyz/).

**hg_sdf**: Signed distance functions for raymarching (fSphere, fBox, fTorus, domain repetition, boolean ops).

## Common Patterns

### Audio-Reactive Color Flash

```glsl
vec3 col = baseColor;
col = mix(col, col * vec3(2.0, 1.5, 0.5), syn_HighHits);
```

### Bass-Driven Camera Movement

```glsl
vec3 rayOrigin = vec3(0.0, 0.0, syn_BassTime);
```

### Beat-Synced Position

```glsl
vec2 pos = 0.5 + 0.3 * vec2(
    sin(syn_RandomOnBeat * 2.0 * PI),
    cos(syn_ToggleOnBeat * 2.0 * PI)
);
```

### Canvas Shake on Hits

```glsl
vec2 uv = _uv;
uv += vec2(sin(777.77 * syn_BassTime), sin(555.55 * syn_HighTime))
      * 0.01 * syn_BassHits;
```

### BPM-Synced Color Shift

```glsl
col = mix(col, col * vec3(0.5, 0.5, 2.0), syn_BPMSin4);
```

### Spectrum Visualizer

```glsl
vec4 renderMain() {
    float fft = texture(syn_Spectrum, vec2(_uv.x, 0.0)).g;  // Juiced FFT
    float bar = step(_uv.y, fft);
    return vec4(vec3(bar), 1.0);
}
```

## MilkDrop Conversion Notes

| MilkDrop | Synesthesia |
|----------|-------------|
| `time` | `TIME` |
| `bass`, `mid`, `treb` | Sample `syn_Spectrum.g` at 0.05, 0.3, 0.8 |
| `bass_att`, etc. | `syn_BassLevel`, `syn_MidLevel`, etc. |
| `uv` | `_uv` |
| `texsize` | `RENDERSIZE` |
| Feedback texture | `syn_FinalPass` |
| `decay` | Multiply feedback sample |

### Key Differences

1. **Audio is texture-based**: Sample `syn_Spectrum` instead of using pre-analyzed values
2. **No mesh system**: Convert per-vertex motion to per-pixel UV manipulation
3. **Feedback is explicit**: Use `syn_FinalPass` or configure multipass
4. **Coordinates**: Y-axis may be flipped; `_uvc` provides centered, aspect-corrected coords

## Best Practices

1. **Use Juiced FFT** (`syn_Spectrum.g`) for most audio visualization - it's pre-processed for better visual results

2. **Add audio reactivity** - it's Synesthesia's core feature:
   - Color flashes on hits
   - Movement driven by `syn_BassTime` or `syn_BPMTwitcher`
   - Structural changes with `syn_Presence` uniforms

3. **Use smooth controls** - `slider smooth`, `toggle smooth` create better animations. Adjust `PARAMS` (default 0.01, try 0.05 for faster)

4. **Test comparisons with > 0.5** instead of `== 1.0` (floats may have precision issues)

5. **Set good defaults** - The scene should look compelling immediately when loaded

6. **Avoid controls that do nothing** - If control A has no effect when control B is at certain values, consider coupling them or documenting the interaction

## JavaScript Scripting (Optional)

Add `script.js` for per-frame calculations, timers, OOP, and control manipulation:

```javascript
function setup() {
    // Run once when scene loads
    onChange("syn_BassLevel", "onBassChange");
}

function update(dt) {
    // Run every frame
    var modifiedBass = Math.pow(syn_BassLevel, 2) * 0.5;
    setUniform("modifiedBass", modifiedBass);
}

function onBassChange(value, prevValue) {
    if (value > 0.9) {
        randomizeControl("meta/hue");
    }
}
```

See the JavaScript Scripting reference for full documentation on event handlers, control manipulation, and reading pixel data.
