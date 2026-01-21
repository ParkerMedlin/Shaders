# MilkDrop Adaptation Plan: fed - DSC-011 LSD Edit 2 -> Resolume Wire ISF

## Simplified Time-Based Version

No audio reactivity. All animation driven by `TIME`.

## Core Effects to Preserve

1. **Feedback with zoom** - Constant slight zoom creates tunnel/infinite regression
2. **Luminosity/hue separation** - The distinctive color processing
3. **Cross-channel gamma curve** - `pow(ret, ret.zxy + 0.8)` creates the psychedelic color shifting
4. **Center darkening** - Prevents center blowout
5. **Gradual decay** - `-0.002` darkening per frame

## Conversion Strategy

### Zoom
Replace audio-reactive zoom with constant or time-varying:
```glsl
float zoom = 1.0 + zoomAmount * (0.5 + 0.5 * sin(TIME * zoomSpeed));  // Gentle pulse
// Or just constant: float zoom = 1.0 + zoomAmount;
```

### Color Tinting
Replace audio-reactive q1/q2/q3 with time-based cycling:
```glsl
float q1 = 0.6 + 0.2 * sin(TIME * colorSpeed * 1.0);   // G channel
float q2 = 0.6 + 0.2 * sin(TIME * colorSpeed * 1.3);   // B channel
float q3 = 0.5 + 0.3 * sin(TIME * colorSpeed * 0.7);   // R channel
```

### Core Color Algorithm
```glsl
vec3 fb = IMG_NORM_PIXEL(feedbackInput, zoomedUV).rgb;

// Luminosity = min of RGB
float lum = min(min(fb.r, fb.g), fb.b);

// Hue = color minus luminosity
vec3 hue = fb - lum;

// Time-based color tinting
vec3 tint = vec3(q3, q1, q2);
vec3 result = tint * lum + hue * 1.01;

// Cross-channel gamma (the magic)
result = pow(result, result.zxy + 0.8);

// Decay
result -= 0.002;
```

### Center Darkening
```glsl
float d = length(uv - 0.5);
result *= smoothstep(0.0, 0.1, d);
```

## Controls to Expose

| Control | Type | Default | Purpose |
|---------|------|---------|---------|
| `zoomSpeed` | float | 0.5 | Speed of zoom oscillation |
| `zoomAmount` | float | 0.01 | Zoom intensity |
| `colorSpeed` | float | 0.7 | Speed of color cycling |

## ISF Metadata

```json
{
  "DESCRIPTION": "Fed DSC-011 LSD - Time-based color separation effect",
  "CREDIT": "Original by Fed, adapted for ISF",
  "CATEGORIES": ["Generator"],
  "INPUTS": [
    { "NAME": "feedbackInput", "TYPE": "image" },
    { "NAME": "zoomSpeed", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "zoomAmount", "TYPE": "float", "DEFAULT": 0.01, "MIN": 0.0, "MAX": 0.05 },
    { "NAME": "colorSpeed", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.0, "MAX": 2.0 }
  ]
}
```

## Shader Structure

```
1. Calculate time-based zoom
2. Transform UV around center
3. Sample feedback
4. Extract luminosity (min RGB)
5. Extract hue
6. Calculate time-based color tint
7. Recombine: tint * lum + hue * 1.01
8. Apply cross-channel gamma
9. Apply decay (-0.002)
10. Apply center darkening
11. Output
```

## Usage in Resolume Wire

1. Add the ISF effect
2. Create feedback routing: output -> Feedback node -> feedbackInput
3. Adjust speed/amount controls to taste

---

**Ready for implementation.**
