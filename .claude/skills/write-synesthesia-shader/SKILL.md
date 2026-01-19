---
name: write-synesthesia-shader
description: Writes GLSL shaders for Synesthesia music visualizer. Use when creating audio-reactive visuals for Synesthesia, or converting effects to Synesthesia format.
---

# Writing Shaders for Synesthesia

Synesthesia is a real-time music visualizer that uses GLSL fragment shaders. It provides built-in audio analysis (FFT, waveform, beat detection) through standardized uniforms.

## Shader Structure

Synesthesia shaders are standard GLSL fragment shaders:

```glsl
// Synesthesia auto-declares these uniforms - don't redeclare them
// uniform vec2 iResolution;
// uniform float iTime;
// uniform sampler2D iChannel0;  // Audio FFT texture
// uniform sampler2D iChannel1;  // Audio waveform texture
// etc.

void main() {
    vec2 uv = gl_FragCoord.xy / iResolution.xy;

    // Your shader code here

    gl_FragColor = vec4(color, 1.0);
}
```

## Built-in Uniforms

### Resolution & Time

| Uniform | Type | Description |
|---------|------|-------------|
| `iResolution` | `vec2` | Viewport resolution in pixels |
| `iTime` | `float` | Time in seconds since shader start |
| `iTimeDelta` | `float` | Time since last frame |
| `iFrame` | `int` | Frame counter |

### Audio Analysis

| Uniform | Type | Description |
|---------|------|-------------|
| `iChannel0` | `sampler2D` | FFT frequency data (256x1 or similar) |
| `iChannel1` | `sampler2D` | Audio waveform data |
| `iAudioBands` | `vec4` | Pre-analyzed frequency bands (bass, low-mid, high-mid, treble) |
| `iBeat` | `float` | Beat detection value (0.0 to 1.0, spikes on beats) |
| `iBPM` | `float` | Detected BPM |
| `iVolume` | `float` | Overall volume level |

### Sampling Audio Textures

```glsl
// Sample FFT at a specific frequency band (0.0 = bass, 1.0 = treble)
float bass = texture2D(iChannel0, vec2(0.05, 0.0)).r;
float mid = texture2D(iChannel0, vec2(0.3, 0.0)).r;
float treble = texture2D(iChannel0, vec2(0.8, 0.0)).r;

// Sample waveform
float waveformSample = texture2D(iChannel1, vec2(uv.x, 0.0)).r;
```

## User Controls

Synesthesia supports custom parameters via special comments:

```glsl
// @param speed 0.0 5.0 1.0
uniform float speed;

// @param color #ff0000
uniform vec3 color;

// @param intensity 0.0 2.0 1.0
uniform float intensity;
```

Format: `// @param name min max default` for floats, or `// @param name #hexcolor` for colors.

## Feedback / Previous Frame

Synesthesia provides access to the previous frame:

```glsl
uniform sampler2D iPreviousFrame;  // or iBackbuffer

void main() {
    vec2 uv = gl_FragCoord.xy / iResolution.xy;

    // Sample previous frame with slight zoom for trail effect
    vec2 feedbackUV = (uv - 0.5) * 0.99 + 0.5;
    vec4 feedback = texture2D(iPreviousFrame, feedbackUV);

    // Mix with new content
    vec4 newContent = vec4(/* ... */);
    gl_FragColor = mix(newContent, feedback * 0.95, 0.8);
}
```

## Common Patterns

### Audio-Reactive Zoom

```glsl
float bass = texture2D(iChannel0, vec2(0.05, 0.0)).r;
vec2 uv = gl_FragCoord.xy / iResolution.xy;
vec2 centered = uv - 0.5;

// Zoom based on bass
float zoom = 1.0 + bass * 0.1;
uv = centered / zoom + 0.5;
```

### Beat-Synced Flash

```glsl
// Flash white on beats
vec3 color = baseColor;
color = mix(color, vec3(1.0), iBeat * 0.5);
```

### Frequency Spectrum Visualization

```glsl
void main() {
    vec2 uv = gl_FragCoord.xy / iResolution.xy;

    // Sample FFT at this x position
    float fft = texture2D(iChannel0, vec2(uv.x, 0.0)).r;

    // Draw bar
    float bar = step(uv.y, fft);

    gl_FragColor = vec4(vec3(bar), 1.0);
}
```

### Waveform Visualization

```glsl
void main() {
    vec2 uv = gl_FragCoord.xy / iResolution.xy;

    // Sample waveform
    float wave = texture2D(iChannel1, vec2(uv.x, 0.0)).r;
    wave = wave * 0.5 + 0.5;  // Map from [-1,1] to [0,1]

    // Draw line
    float dist = abs(uv.y - wave);
    float line = smoothstep(0.02, 0.0, dist);

    gl_FragColor = vec4(vec3(line), 1.0);
}
```

## MilkDrop Conversion Notes

When converting from MilkDrop:

| MilkDrop | Synesthesia |
|----------|-------------|
| `time` | `iTime` |
| `bass`, `mid`, `treb` | Sample `iChannel0` at different x positions |
| `bass_att`, etc. | Use smoothing or `iAudioBands` |
| `uv` | `gl_FragCoord.xy / iResolution.xy` |
| `texsize` | `iResolution` |
| Feedback texture | `iPreviousFrame` |
| `decay` | Multiply feedback sample |

### Key Differences

1. **Audio is texture-based**: MilkDrop provides pre-analyzed `bass/mid/treb` values; Synesthesia gives you raw FFT data to sample yourself.

2. **No mesh system**: MilkDrop's per-vertex motion must be converted to per-pixel UV manipulation.

3. **Feedback is explicit**: Use `iPreviousFrame` uniform instead of implicit feedback loop.

4. **Different coordinate conventions**: Check if Y is flipped compared to MilkDrop.

## Platform Limitations

- Single-pass rendering (no multipass)
- Limited texture inputs beyond audio channels
- Performance varies by device - keep shaders efficient

## Best Practices

1. **Smooth audio values**: Raw FFT can be noisy. Apply smoothing:
   ```glsl
   // Simple smoothing with previous value
   float smoothBass = mix(prevBass, currentBass, 0.1);
   ```

2. **Use frequency bands wisely**: Bass is typically 0.0-0.1, mids 0.1-0.5, treble 0.5-1.0 in the FFT texture.

3. **Test with different music**: Ensure visuals work across genres (EDM, classical, ambient).

4. **Provide meaningful controls**: Expose parameters that let users customize the visual experience.

---

*Note: This skill is a work in progress. Uniform names and conventions may vary between Synesthesia versions. Test shaders in the actual application.*
