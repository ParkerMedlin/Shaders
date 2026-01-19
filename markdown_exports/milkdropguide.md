# MilkDrop Preset Authoring Guide

This is an LLM-optimized reference for MilkDrop preset authoring. For the original verbose tutorial, see the MilkDrop documentation at milkdrop.co.uk.

---

## Table of Contents

1. [Preset Architecture](#1-preset-architecture)
2. [Variable Reference](#2-variable-reference)
   - [Per-Frame Variables](#per-frame-variables)
   - [Per-Vertex Variables](#per-vertex-variables)
   - [Custom Shape Variables](#custom-shape-variables)
   - [Custom Wave Variables](#custom-wave-variables)
3. [Expression Functions](#3-expression-functions)
4. [Pixel Shaders](#4-pixel-shaders)
   - [Data Types & Operators](#data-types--operators)
   - [Intrinsic Functions](#intrinsic-functions)
   - [Shader Inputs](#shader-inputs)
   - [Texture Sampling](#texture-sampling)
5. [Common Patterns](#5-common-patterns)
6. [Best Practices](#6-best-practices)

---

## 1. Preset Architecture

A MilkDrop preset (.milk file) consists of code sections executed each frame:

```
┌─────────────────┐
│  Preset Init    │ ─── Runs once at load. Sets base Q values.
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│   Per-Frame     │ ─── Runs once per frame. Sets global parameters.
└────────┬────────┘
         │
    ┌────┴────┐
    ▼         ▼
┌────────┐ ┌─────────────────────┐
│Per-Vert│ │Custom Shape/Wave    │
│ex Eqs  │ │Init + Per-Frame     │
└────┬───┘ └──────────┬──────────┘
     │                │
     │         ┌──────┴──────┐
     │         ▼             ▼
     │    ┌─────────┐  ┌───────────┐
     │    │Shape    │  │Wave       │
     │    │Drawing  │  │Per-Point  │
     │    └─────────┘  └───────────┘
     │
     ▼
┌─────────────────┐
│  Warp Shader    │ ─── Warps feedback texture. Effects persist.
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│Composite Shader │ ─── Final display. Effects don't persist.
└─────────────────┘
```

### Variable Bridging

**Q Variables (q1-q32)**: Bridge values between code sections.
- Values set in init become "sticky" defaults
- Reset each frame to init values before per-frame code
- Flow: Init → Per-Frame → Per-Vertex → Shaders
- Also flow: Per-Frame → Custom Shape/Wave Per-Frame

**T Variables (t1-t8)**: For custom waves/shapes only.
- Flow: Custom Init → Custom Per-Frame → Per-Point (waves only)

---

## 2. Variable Reference

### Per-Frame Variables

#### Motion Parameters (Writable)

| Variable | Range | Default | Description |
|----------|-------|---------|-------------|
| `zoom` | >0 | 1.0 | Zoom per frame. 0.9=out 10%, 1.1=in 10% |
| `zoomexp` | >0 | 1.0 | Zoom curvature |
| `rot` | any | 0.0 | Rotation. Positive=CCW |
| `warp` | ≥0 | 1.0 | Warp magnitude |
| `cx`, `cy` | 0..1 | 0.5 | Center of rotation/stretch |
| `dx`, `dy` | any | 0.0 | Translation. -0.01=left/up 1%/frame |
| `sx`, `sy` | >0 | 1.0 | Stretch. 0.99=shrink, 1.01=stretch |
| `decay` | 0..1 | 0.98 | Fade to black. 1=none |

#### Waveform Parameters (Writable)

| Variable | Range | Description |
|----------|-------|-------------|
| `wave_mode` | 0-7 | Waveform type (8 built-in modes) |
| `wave_x`, `wave_y` | 0..1 | Position (0.5=center) |
| `wave_r`, `wave_g`, `wave_b` | 0..1 | RGB color |
| `wave_a` | 0..1 | Opacity |
| `wave_mystery` | -1..1 | Mode-specific parameter |
| `wave_usedots` | 0/1 | Dots instead of lines |
| `wave_thick` | 0/1 | Double thickness |
| `wave_additive` | 0/1 | Additive blending |
| `wave_brighten` | 0/1 | Auto-normalize colors |

#### Border Parameters (Writable)

| Variable | Range | Description |
|----------|-------|-------------|
| `ob_size` | 0..0.5 | Outer border thickness |
| `ob_r`, `ob_g`, `ob_b`, `ob_a` | 0..1 | Outer border RGBA |
| `ib_size` | 0..0.5 | Inner border thickness |
| `ib_r`, `ib_g`, `ib_b`, `ib_a` | 0..1 | Inner border RGBA |

#### Motion Vector Parameters (Writable)

| Variable | Range | Description |
|----------|-------|-------------|
| `mv_x` | 0..64 | Vectors in X direction |
| `mv_y` | 0..48 | Vectors in Y direction |
| `mv_r`, `mv_g`, `mv_b`, `mv_a` | 0..1 | Vector RGBA |
| `mv_l` | 0..5 | Trail length |
| `mv_dx`, `mv_dy` | -1..1 | Placement offset |

#### Visual Effect Parameters (Writable)

| Variable | Range | Description |
|----------|-------|-------------|
| `gamma` | >0 | Brightness multiplier |
| `echo_zoom` | >0 | Second layer size |
| `echo_alpha` | ≥0 | Second layer opacity |
| `echo_orient` | 0-3 | Second layer flip (0=normal, 1=flipX, 2=flipY, 3=both) |
| `darken_center` | 0/1 | Dim center point |
| `wrap` | 0/1 | Edge wrapping |
| `invert` | 0/1 | Invert colors |
| `brighten` | 0/1 | Square root filter |
| `darken` | 0/1 | Squaring filter |
| `solarize` | 0/1 | Emphasize midtones |

#### Blur Controls (Writable)

| Variable | Range | Description |
|----------|-------|-------------|
| `blur1_min`, `blur1_max` | 0..1 | Blur level 1 clamp range |
| `blur2_min`, `blur2_max` | 0..1 | Blur level 2 clamp range |
| `blur3_min`, `blur3_max` | 0..1 | Blur level 3 clamp range |
| `blur1_edge_darken` | 0..1 | Edge darkening amount |

#### Read-Only Inputs

| Variable | Type | Description |
|----------|------|-------------|
| `time` | float | Seconds since MilkDrop started |
| `fps` | float | Current framerate |
| `frame` | int | Frame count |
| `progress` | 0..1 | Progress through preset (freezes with Scroll Lock) |
| `bass` | float | Bass level (1.0=normal, >1.3=loud) |
| `mid` | float | Mid frequency level |
| `treb` | float | Treble level |
| `bass_att` | float | Smoothed bass |
| `mid_att` | float | Smoothed mid |
| `treb_att` | float | Smoothed treble |
| `meshx`, `meshy` | int | Mesh resolution |
| `pixelsx`, `pixelsy` | int | Canvas pixel dimensions |
| `aspectx`, `aspecty` | float | Aspect ratio multipliers |

#### Bridge Variables (Writable)

| Variable | Description |
|----------|-------------|
| `q1` - `q32` | Bridge values to per-vertex code and shaders |
| `monitor` | Debug output (view with 'N' key) |

---

### Per-Vertex Variables

**Additional read-only inputs** (only available in per-vertex code):

| Variable | Range | Description |
|----------|-------|-------------|
| `x` | 0..1 | Horizontal position (0=left, 1=right) |
| `y` | 0..1 | Vertical position (0=top, 1=bottom) |
| `rad` | 0..1 | Distance from center (0=center, ~0.707=edges, 1=corners) |
| `ang` | 0..2π | Angle from center (0=right, π/2=up, π=left, 3π/2=down) |

All motion parameters and Q variables remain writable.

**Performance rule**: If a per-vertex equation doesn't use `x`, `y`, `rad`, or `ang`, move it to per-frame.

---

### Custom Shape Variables

Up to 4 custom shapes per preset. Each has init code and per-frame code. Can instance up to 1024 copies.

#### Shape Per-Frame Variables (Writable)

| Variable | Range | Description |
|----------|-------|-------------|
| `sides` | 3-100 | Polygon sides |
| `x`, `y` | 0..1 | Position |
| `rad` | ≥0 | Radius |
| `ang` | 0..2π | Rotation angle |
| `r`, `g`, `b`, `a` | 0..1 | Center color/opacity |
| `r2`, `g2`, `b2`, `a2` | 0..1 | Edge color/opacity |
| `border_r`, `border_g`, `border_b`, `border_a` | 0..1 | Border RGBA |
| `thick` | 0/1 | Thick border |
| `additive` | 0/1 | Additive blending |
| `textured` | 0/1 | Map previous frame texture |
| `tex_zoom` | >0 | Texture zoom |
| `tex_ang` | 0..2π | Texture rotation |

#### Read-Only

| Variable | Description |
|----------|-------------|
| `num_inst` | Total instances (1-1024) |
| `instance` | Current instance index (0 to num_inst-1) |

Plus all standard read-only inputs (time, fps, audio, etc.) and Q/T variables.

---

### Custom Wave Variables

Up to 4 custom waves per preset. Each has init, per-frame, and per-point code.

#### Wave Per-Frame Variables (Writable)

| Variable | Range | Description |
|----------|-------|-------------|
| `r`, `g`, `b`, `a` | 0..1 | Base wave RGBA |
| `samples` | 0-512 | Number of samples |

#### Wave Per-Point Variables

| Variable | Writable | Description |
|----------|----------|-------------|
| `x`, `y` | yes | Point position (0..1) |
| `r`, `g`, `b`, `a` | yes | Point RGBA |
| `sample` | no | Position in wave (0=first, 1=last) |
| `value1` | no | Left audio channel value |
| `value2` | no | Right audio channel value |

---

## 3. Expression Functions

For preset init, per-frame, and per-vertex equations (NOT shaders).

### Math Functions

| Function | Description |
|----------|-------------|
| `int(x)` | Integer truncation (toward zero) |
| `abs(x)` | Absolute value |
| `sign(x)` | Sign: -1, 0, or 1 |
| `sqr(x)` | Square (x*x) |
| `sqrt(x)` | Square root |
| `pow(x,y)` | x raised to power y |
| `log(x)` | Natural logarithm |
| `log10(x)` | Base-10 logarithm |
| `min(x,y)` | Minimum |
| `max(x,y)` | Maximum |
| `sigmoid(x,c)` | Sigmoid function with constraint c |

### Trigonometric Functions

| Function | Description |
|----------|-------------|
| `sin(x)` | Sine (radians) |
| `cos(x)` | Cosine |
| `tan(x)` | Tangent |
| `asin(x)` | Arcsine |
| `acos(x)` | Arccosine |
| `atan(x)` | Arctangent |

### Logic & Comparison

| Function | Description |
|----------|-------------|
| `if(cond,t,f)` | If cond≠0 return t, else f |
| `equal(x,y)` | Returns 1 if x=y, else 0 |
| `above(x,y)` | Returns 1 if x>y, else 0 |
| `below(x,y)` | Returns 1 if x<y, else 0 |
| `bor(x,y)` | Boolean OR (1 if either ≠0) |
| `bnot(x)` | Boolean NOT (1 if x=0) |

### Other

| Function | Description |
|----------|-------------|
| `rand(n)` | Random integer 0 to n-1 |

### Operators

| Operator | Description |
|----------|-------------|
| `=` | Assignment |
| `+`, `-`, `*`, `/` | Arithmetic |
| `%` | Integer modulo |
| `\|` | Bitwise OR |
| `&` | Bitwise AND |

---

## 4. Pixel Shaders

MilkDrop 2+ uses HLSL pixel shaders. Presets have two shaders:

- **Warp Shader**: Warps the feedback texture. Effects persist to next frame.
- **Composite Shader**: Final display processing. Effects don't persist.

### Shader Structure

```hlsl
// Declarations go here (samplers, texsize variables)
sampler sampler_mytexture;
float4  texsize_mytexture;

shader_body
{
    // Main code - must set 'ret' (float3) as final output
    ret = tex2D(sampler_main, uv).xyz;
    ret *= 0.97;
}
```

### Data Types & Operators

#### Types

| Type | Description |
|------|-------------|
| `float`, `float2`, `float3`, `float4` | Full precision |
| `half`, `half2`, `half3`, `half4` | Half precision (use for colors) |
| `float2x2`, `float3x3`, `float4x3` | Matrices |

#### Swizzling

Access and reorder vector components with `.xyzw`:
```hlsl
vec.xy          // First two components
vec.wzxy        // Reorder components
vec.xxx         // Replicate x three times
```

#### Operators

| Operator | Description |
|----------|-------------|
| `+ - * /` | Arithmetic |
| `+= -= *= /=` | Compound assignment |
| `== < <= > >=` | Comparison |

### Intrinsic Functions

#### Fast Operations

| Function | Description |
|----------|-------------|
| `abs(a)` | Absolute value |
| `frac(a)` | Fractional part (a - floor(a)) |
| `floor(a)` | Integer part (floats only) |
| `saturate(a)` | Clamp to [0,1] - often FREE |
| `max(a,b)` | Component-wise maximum |
| `min(a,b)` | Component-wise minimum |
| `sqrt(a)` | Square root |
| `pow(a,b)` | Power |
| `exp(a)` | 2^a |
| `log(a)` | log2(a) |
| `lerp(a,b,c)` | Linear interpolation: a + c*(b-a) |
| `dot(a,b)` | Dot product (returns single float) |
| `length(a)` | Vector length |
| `normalize(a)` | Unit vector |
| `lum(a)` | Luminance: dot(a, float3(0.32,0.49,0.29)) |

#### Slow Operations (~8 instructions each)

| Function | Description |
|----------|-------------|
| `sin(a)`, `cos(a)` | Trig functions (radians) |
| `atan2(y,x)` | Arctangent |
| `mul(a,b)` | Matrix multiplication |
| `cross(a,b)` | Cross product (float3 only) |

#### Texture Sampling

| Function | Description |
|----------|-------------|
| `tex2D(sampler, uv)` | Sample 2D texture, returns float4 |
| `tex3D(sampler, uvw)` | Sample 3D/volume texture |
| `GetBlur1(uv)` | Slightly blurred main (float3) |
| `GetBlur2(uv)` | More blurred |
| `GetBlur3(uv)` | Very blurred |

### Shader Inputs

#### Warp Shader Per-Vertex Inputs

| Variable | Type | Description |
|----------|------|-------------|
| `uv` | float2 | Warped UV from per-vertex equations |
| `uv_orig` | float2 | Original unwarped UV [0,1] |
| `rad` | float | Distance from center [0,1] |
| `ang` | float | Angle from center [0,2π] |

#### Composite Shader Per-Vertex Inputs

| Variable | Type | Description |
|----------|------|-------------|
| `uv` | float2 | Unwarped UV coords |
| `rad` | float | Distance from center |
| `ang` | float | Angle from center |
| `hue_shader` | float3 | Screen-varying color |

#### Per-Frame Inputs (Both Shaders)

| Variable | Type | Description |
|----------|------|-------------|
| `time` | float | Seconds since preset start (wraps at 10000) |
| `fps` | float | Framerate |
| `frame` | float | Frame number |
| `progress` | float | Preset progress [0,1] |
| `bass`, `mid`, `treb`, `vol` | float | Audio levels |
| `bass_att`, `mid_att`, `treb_att`, `vol_att` | float | Smoothed audio |
| `aspect` | float4 | .xy=fit multiplier, .zw=inverse |
| `texsize` | float4 | .xy=canvas size, .zw=1/size |
| `rand_preset` | float4 | 4 random [0,1] per preset |
| `rand_frame` | float4 | 4 random [0,1] per frame |
| `q1`-`q32` | float | Q variable values |
| `_qa`-`_qh` | float4 | Q vars grouped (q1-4=_qa, q5-8=_qb, etc.) |
| `slow_roam_cos`, `slow_roam_sin` | float4 | Slowly varying [0,1] |
| `roam_cos`, `roam_sin` | float4 | Faster varying [0,1] |
| `rot_s1`-`rot_s4` | float4x3 | Static random rotations |
| `rot_d1`-`rot_d4` | float4x3 | Slow changing rotations |
| `rot_f1`-`rot_f4` | float4x3 | Fast changing rotations |
| `rot_vf1`-`rot_vf4` | float4x3 | Very fast rotations |
| `rot_uf1`-`rot_uf4` | float4x3 | Ultra fast rotations |
| `rot_rand1`-`rot_rand4` | float4x3 | Random each frame |
| `blur1_min/max`, etc. | float | Blur range values |

### Texture Sampling

#### Main Canvas Samplers

| Sampler | Filtering | Edge Mode |
|---------|-----------|-----------|
| `sampler_main` or `sampler_fw_main` | Bilinear | Wrap |
| `sampler_fc_main` | Bilinear | Clamp |
| `sampler_pw_main` | Point | Wrap |
| `sampler_pc_main` | Point | Clamp |

#### Built-in Noise Textures

| Sampler | Type | Size | Quality |
|---------|------|------|---------|
| `sampler_noise_lq` | 2D | 256x256 | Low |
| `sampler_noise_lq_lite` | 2D | 32x32 | Low |
| `sampler_noise_mq` | 2D | 64x64 | Medium |
| `sampler_noise_hq` | 2D | 32x32 | High (smooth) |
| `sampler_noisevol_lq` | 3D | 32³ | Low |
| `sampler_noisevol_hq` | 3D | 8³ | High |

#### Custom Textures

```hlsl
// Above shader_body:
sampler sampler_mytexture;        // Loads mytexture.jpg/.dds/.png
float4  texsize_mytexture;        // .xy=size, .zw=1/size

// Random texture selection:
sampler sampler_rand07;           // Random texture
sampler sampler_rand02_prefix;    // Random from files starting with "prefix"
```

Supported formats: jpg, dds, png, tga, bmp

---

## 5. Common Patterns

### Audio-Reactive Motion

```
// Per-frame equations
zoom = zoom + 0.1*(bass - 1);
rot = rot + 0.05*sin(time)*(treb - 1);
warp = warp + 0.5*(mid_att - 1);
```

### Perspective Zoom (Per-Vertex)

```
// More zoom at edges = perspective
zoom = zoom + rad*0.1;
```

### Color Cycling

```
wave_r = 0.5 + 0.5*sin(time*1.13);
wave_g = 0.5 + 0.5*sin(time*1.23);
wave_b = 0.5 + 0.5*sin(time*1.33);
```

### Passing Values to Shaders

```
// Per-frame:
q1 = sin(time*2);
q2 = bass_att;
q3 = if(above(bass,1.2), 1, 0);  // Beat trigger
```

```hlsl
// Shader:
ret *= 1.0 + q1*0.1;
ret = lerp(ret, float3(1,0.5,0), q3*0.3);
```

### Proper Decay (Warp Shader)

```hlsl
// Pure multiply can leave dark pixels stuck
ret *= 0.97;  // May not reach black

// Pure subtract fades too fast
ret -= 0.004;

// Best: combined approach
ret = (ret - 0.002)*0.99;
```

### Auto Center Darkening (Warp Shader)

```hlsl
// Darkens pixels near zoom center
ret *= 0.97 + 0.03*saturate(length(uv - uv_orig)*200);
```

### Random Dithering

```hlsl
float2 uv_noise = uv_orig*texsize.xy*texsize_noise_lq.zw + rand_frame.xy;
half4 noiseVal = tex2D(sampler_noise_lq, uv_noise);
ret += (noiseVal.xyz*2-1) * 0.01;
```

### Aspect-Correct Calculations

```hlsl
// For correct angles on non-square displays
float2 uv2 = (uv-0.5)*aspect.xy;
float ang = atan2(uv2.y, uv2.x);
float rad = length(uv2);
```

### Glow Effect

```hlsl
// Set blur1_min to threshold (e.g., 0.7) in per-frame
ret += (GetBlur1(uv) - blur1_min)*2;
```

### Edge Detection

```hlsl
float3 crisp = tex2D(sampler_main, uv).xyz;
float3 blurry = GetBlur1(uv);
ret = abs(crisp - blurry)*4;
```

---

## 6. Best Practices

### Performance

1. **Move invariant calculations to per-frame**: If per-vertex code doesn't use `x`, `y`, `rad`, or `ang`, it belongs in per-frame.

2. **Avoid slow shader operations**: `sin()`, `cos()`, `atan2()` are ~8 instructions. Precompute in per-frame and pass via Q variables.

3. **Don't over-tile textures**: Sampling textures at too high frequency thrashes cache. Sample near 1:1 or zoom in, never extreme zoom out.

4. **Use half precision for colors**: Use `half` for color values, `float` for UVs and coordinates.

5. **Optimizer removes dead code**: Anything not contributing to final output is eliminated.

### Quality

1. **Test at default mesh size**: Design/verify presets work at standard mesh resolution.

2. **Test in 32-bit color mode**: Ensures correct brightness levels.

3. **Handle aspect ratio**: Multiply by `aspectx`/`aspecty` in equations, by `aspect.xy` in shaders.

4. **Proper decay**: Use combined constant+linear decay to avoid stuck pixels.

5. **Keep textures small**: 256x256 or less, JPEG at 95% quality for sharing.

### Debugging

- Set `monitor = variable;` in per-frame, then press 'N' to display value.
- Only works for per-frame equations.

### Shader Model Notes

- MilkDrop 2 requires Shader Model 2.0 minimum (64 instruction limit)
- Shader Model 3.0 available for virtually unlimited instructions
- Presets not supported by GPU won't appear in preset list
