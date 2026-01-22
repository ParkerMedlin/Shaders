# MilkDrop Preset Reference

MilkDrop is a music visualization plugin that uses presets (.milk files) to define visual effects. Presets contain equations and shader code that animate in response to audio.

## Preset Architecture

A MilkDrop preset consists of these code sections executed in order each frame:

1. **Preset Init Code** - Runs once when preset loads. Initializes variables and sets base Q values.
2. **Per-Frame Equations** - Runs once per frame. Sets global motion/color parameters.
3. **Per-Vertex Equations** - Runs at each mesh vertex. Creates spatial variation in motion.
4. **Custom Shapes** (up to 4) - Draws polygonal shapes with per-frame control.
5. **Custom Waves** (up to 4) - Draws waveforms with per-frame and per-point control.
6. **Warp Shader** (HLSL) - Pixel shader that warps the feedback texture. Effects persist.
7. **Composite Shader** (HLSL) - Pixel shader for final display. Effects don't persist.

## Variable Flow

```
Preset Init → Per-Frame → Per-Vertex → Warp Shader
                ↓
         Custom Shape/Wave Per-Frame → Custom Wave Per-Point
                ↓
         Composite Shader
```

**Q Variables (q1-q32)**: Bridge values between code sections. Values set in init become "sticky" defaults, reset each frame before per-frame code runs.

**T Variables (t1-t8)**: Bridge values within custom waves/shapes only (init → per-frame → per-point).

---

## Per-Frame Variables

### Motion Parameters (Writable)

| Variable | Range | Description |
|----------|-------|-------------|
| `zoom` | >0 | Inward/outward motion. 0.9=out 10%, 1.0=none, 1.1=in 10% |
| `zoomexp` | >0 | Curvature of zoom. 1=normal |
| `rot` | any | Rotation amount. 0=none, positive=CCW |
| `warp` | >0 | Warping magnitude. 0=none, 1=normal |
| `cx`, `cy` | 0..1 | Center of rotation/stretching. 0.5=center |
| `dx`, `dy` | any | Constant motion. -0.01=left/up 1%/frame |
| `sx`, `sy` | >0 | Stretching. 0.99=shrink, 1=none, 1.01=stretch |
| `decay` | 0..1 | Fade to black. 1=none, 0.98=recommended |

### Waveform Parameters (Writable)

| Variable | Range | Description |
|----------|-------|-------------|
| `wave_mode` | 0-7 | Which of 8 waveform types |
| `wave_x`, `wave_y` | 0..1 | Waveform position |
| `wave_r`, `wave_g`, `wave_b` | 0..1 | Wave color RGB |
| `wave_a` | 0..1 | Wave opacity |
| `wave_mystery` | -1..1 | Mode-specific parameter |
| `wave_usedots` | 0/1 | Draw as dots vs lines |
| `wave_thick` | 0/1 | Double thickness |
| `wave_additive` | 0/1 | Additive blending |
| `wave_brighten` | 0/1 | Auto-brighten colors |

### Border Parameters (Writable)

| Variable | Range | Description |
|----------|-------|-------------|
| `ob_size`, `ib_size` | 0..0.5 | Outer/inner border thickness |
| `ob_r/g/b/a`, `ib_r/g/b/a` | 0..1 | Border colors and opacity |

### Motion Vectors (Writable)

| Variable | Range | Description |
|----------|-------|-------------|
| `mv_x`, `mv_y` | 0..64/48 | Number of vectors in X/Y |
| `mv_r/g/b/a` | 0..1 | Vector color and opacity |
| `mv_l` | 0..5 | Vector trail length |
| `mv_dx`, `mv_dy` | -1..1 | Placement offset |

### Visual Effects (Writable)

| Variable | Range | Description |
|----------|-------|-------------|
| `gamma` | >0 | Brightness. 1=normal |
| `echo_zoom` | >0 | Second layer size |
| `echo_alpha` | >0 | Second layer opacity |
| `echo_orient` | 0-3 | Second layer flip mode |
| `darken_center` | 0/1 | Dim center point |
| `wrap` | 0/1 | Edge wrapping |
| `invert` | 0/1 | Invert colors |
| `brighten` | 0/1 | Square root filter |
| `darken` | 0/1 | Squaring filter |
| `solarize` | 0/1 | Emphasize midtones |

### Blur Controls (Writable)

| Variable | Range | Description |
|----------|-------|-------------|
| `blur1_min/max` | 0..1 | Blur1 value clamp range |
| `blur2_min/max` | 0..1 | Blur2 value clamp range |
| `blur3_min/max` | 0..1 | Blur3 value clamp range |
| `blur1_edge_darken` | 0..1 | Edge darkening |

### Read-Only Inputs

| Variable | Range | Description |
|----------|-------|-------------|
| `time` | >0 | Seconds since MilkDrop started |
| `fps` | >0 | Current framerate |
| `frame` | int | Frame count since start |
| `progress` | 0..1 | Progress through preset duration |
| `bass`, `mid`, `treb` | >0 | Audio levels. 1=normal, >1.3=loud |
| `bass_att`, `mid_att`, `treb_att` | >0 | Smoothed audio levels |
| `meshx`, `meshy` | int | Mesh resolution |
| `pixelsx`, `pixelsy` | int | Canvas dimensions |
| `aspectx`, `aspecty` | >0 | Aspect ratio multipliers |

---

## Per-Vertex Variables

**Additional read-only inputs:**

| Variable | Range | Description |
|----------|-------|-------------|
| `x` | 0..1 | Horizontal position. 0=left, 1=right |
| `y` | 0..1 | Vertical position. 0=top, 1=bottom |
| `rad` | 0..1 | Distance from center. 0=center, 1=corners |
| `ang` | 0..2π | Angle from center. 0=right, π/2=up |

All motion parameters (zoom, rot, warp, dx, dy, sx, sy, cx, cy) and Q variables are writable per-vertex.

**Key principle**: If a per-vertex equation doesn't use x, y, rad, or ang, move it to per-frame for performance.

---

## Custom Shapes

Up to 4 shapes, each with init code and per-frame code. Can be instanced (drawn multiple times with varying `instance` value).

### Shape Variables (Writable)

| Variable | Range | Description |
|----------|-------|-------------|
| `sides` | 3-100 | Number of polygon sides |
| `x`, `y` | 0..1 | Shape position |
| `rad` | >0 | Shape radius |
| `ang` | 0..2π | Rotation angle |
| `r`, `g`, `b`, `a` | 0..1 | Center color/opacity |
| `r2`, `g2`, `b2`, `a2` | 0..1 | Edge color/opacity |
| `border_r/g/b/a` | 0..1 | Border color/opacity |
| `thick` | 0/1 | Thick border |
| `additive` | 0/1 | Additive blending |
| `textured` | 0/1 | Map previous frame onto shape |
| `tex_zoom` | >0 | Texture zoom |
| `tex_ang` | 0..2π | Texture rotation |

### Read-Only

| Variable | Description |
|----------|-------------|
| `num_inst` | Total instances (1-1024) |
| `instance` | Current instance (0 to num_inst-1) |

---

## Custom Waves

Up to 4 waves with init, per-frame, and per-point code.

### Per-Frame Variables (Writable)

| Variable | Range | Description |
|----------|-------|-------------|
| `r`, `g`, `b`, `a` | 0..1 | Base wave color/opacity |
| `samples` | 0-512 | Number of wave samples |

### Per-Point Variables

| Variable | Writable | Description |
|----------|----------|-------------|
| `x`, `y` | yes | Point position (0..1) |
| `r`, `g`, `b`, `a` | yes | Point color/opacity |
| `sample` | no | Position in wave (0..1) |
| `value1` | no | Left audio channel value |
| `value2` | no | Right audio channel value |

---

## Pixel Shaders (HLSL)

MilkDrop 2+ uses HLSL pixel shaders with `shader_body { }` syntax.

### Data Types

```hlsl
float, float2, float3, float4    // Full precision
half, half2, half3, half4        // Half precision (use for colors)
float2x2, float3x3, float4x3     // Matrices
```

### Swizzling

```hlsl
vec.x, vec.xy, vec.wzxy          // Access/reorder components
```

### Math Functions

| Function | Description |
|----------|-------------|
| `abs(a)` | Absolute value |
| `frac(a)` | Fractional part |
| `floor(a)` | Integer part |
| `saturate(a)` | Clamp to [0,1] (often free) |
| `max(a,b)`, `min(a,b)` | Component-wise max/min |
| `sqrt(a)` | Square root |
| `pow(a,b)` | Power |
| `exp(a)` | 2^a |
| `log(a)` | log2(a) |
| `lerp(a,b,c)` | Linear interpolation: a + c*(b-a) |
| `dot(a,b)` | Dot product (returns float) |
| `length(a)` | Vector length |
| `normalize(a)` | Unit vector |
| `lum(a)` | Luminance: dot(a, float3(0.32,0.49,0.29)) |

### Slow Operations (Use Sparingly)

| Function | Description |
|----------|-------------|
| `sin(a)`, `cos(a)` | ~8 instructions |
| `atan2(y,x)` | Arctangent |
| `cross(a,b)` | Cross product |
| `mul(a,b)` | Matrix multiply |

### Texture Sampling

```hlsl
tex2D(sampler_name, uv)    // Sample 2D texture, returns float4
tex3D(sampler_name, uvw)   // Sample 3D texture
GetBlur1(uv)               // Slightly blurred main (float3)
GetBlur2(uv)               // More blurred
GetBlur3(uv)               // Very blurred
```

### Sampler Naming

| Prefix | Filtering | Edge Mode |
|--------|-----------|-----------|
| `sampler_fw_` or `sampler_` | Bilinear | Wrap |
| `sampler_fc_` | Bilinear | Clamp |
| `sampler_pw_` | Point | Wrap |
| `sampler_pc_` | Point | Clamp |

### Warp Shader Inputs

| Variable | Type | Description |
|----------|------|-------------|
| `uv` | float2 | Warped UV coords from per-vertex |
| `uv_orig` | float2 | Original unwarped UV [0,1] |
| `rad` | float | Distance from center [0,1] |
| `ang` | float | Angle from center [0,2π] |

### Composite Shader Inputs

| Variable | Type | Description |
|----------|------|-------------|
| `uv` | float2 | Unwarped UV coords |
| `rad`, `ang` | float | Polar coordinates |
| `hue_shader` | float3 | Varying color across screen |

### Per-Frame Shader Inputs

| Variable | Type | Description |
|----------|------|-------------|
| `time` | float | Seconds since preset start |
| `fps`, `frame`, `progress` | float | Timing info |
| `bass`, `mid`, `treb`, `vol` | float | Audio levels |
| `bass_att`, `mid_att`, `treb_att`, `vol_att` | float | Smoothed audio |
| `aspect` | float4 | .xy=fit multiplier, .zw=inverse |
| `texsize` | float4 | .xy=canvas size, .zw=1/size |
| `rand_preset` | float4 | 4 random [0,1], per preset |
| `rand_frame` | float4 | 4 random [0,1], per frame |
| `q1`-`q32` | float | Q variable values |
| `_qa`-`_qh` | float4 | Q vars grouped (q1-4, q5-8, etc.) |
| `slow_roam_cos/sin` | float4 | Slowly varying [0,1] |
| `roam_cos/sin` | float4 | Faster varying [0,1] |
| `rot_s1`-`rot_s4` | float4x3 | Static random rotations |
| `rot_d1`-`rot_d4` | float4x3 | Slowly changing rotations |
| `rot_f1`-`rot_f4` | float4x3 | Fast changing rotations |
| `rot_rand1`-`rot_rand4` | float4x3 | Random each frame |

### Built-in Textures

| Sampler | Type | Description |
|---------|------|-------------|
| `sampler_main` | 2D | Internal canvas |
| `noise_lq` | 2D 256x256 | Low quality noise |
| `noise_lq_lite` | 2D 32x32 | Small low quality |
| `noise_mq` | 2D 64x64 | Medium quality |
| `noise_hq` | 2D 32x32 | High quality (smooth) |
| `noisevol_lq` | 3D 32³ | Low quality volume |
| `noisevol_hq` | 3D 8³ | High quality volume |

### Loading Custom Textures

```hlsl
sampler sampler_mytexture;        // Loads mytexture.jpg/.dds/.png
float4  texsize_mytexture;        // .xy=size, .zw=1/size

sampler sampler_rand07;           // Random texture
sampler sampler_rand02_prefix;    // Random from files starting with "prefix"
```

---

## Expression Functions

For init, per-frame, and per-vertex equations (not shaders):

| Function | Description |
|----------|-------------|
| `int(x)` | Integer (toward zero) |
| `abs(x)` | Absolute value |
| `sin(x)`, `cos(x)`, `tan(x)` | Trig functions (radians) |
| `asin(x)`, `acos(x)`, `atan(x)` | Inverse trig |
| `sqr(x)` | Square |
| `sqrt(x)` | Square root |
| `pow(x,y)` | x^y |
| `log(x)` | Natural log |
| `log10(x)` | Base-10 log |
| `sign(x)` | Sign (-1, 0, 1) |
| `min(x,y)`, `max(x,y)` | Min/max |
| `sigmoid(x,c)` | Sigmoid function |
| `rand(n)` | Random integer 0 to n-1 |
| `if(cond,t,f)` | Conditional |
| `equal(x,y)`, `above(x,y)`, `below(x,y)` | Comparisons (return 0/1) |
| `bor(x,y)`, `bnot(x)` | Boolean or/not |

**Operators**: `=`, `+`, `-`, `*`, `/`, `%` (mod), `|` (bitwise or), `&` (bitwise and)

---

## Common Patterns

### Audio-Reactive Motion
```
zoom = zoom + 0.1*(bass - 1);
rot = rot + 0.1*sin(time)*treb;
```

### Perspective Zoom (Per-Vertex)
```
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
q1 = sin(time);
q2 = bass_att;

// Shader:
ret *= q1 * q2;
```

### Auto Center Darkening (Warp Shader)
```hlsl
ret *= 0.97 + 0.03*saturate(length(uv - uv_orig)*200);
```

### Random Dithering (Shader)
```hlsl
float2 uv_noise = uv_orig*texsize.xy*texsize_noise_lq.zw + rand_frame.xy;
half4 noiseVal = tex2D(sampler_noise_lq, uv_noise);
ret += (noiseVal.xyz*2-1) * 0.01;
```

### Proper Decay
```hlsl
ret = (ret - 0.002)*0.99;  // Combined constant + linear
```

### Aspect-Correct Angles
```hlsl
float2 uv2 = (uv-0.5)*aspect.xy;
float ang = atan2(uv2.y, uv2.x);
```
