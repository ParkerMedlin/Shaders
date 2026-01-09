# ISF Quick Start – Introduction Anchor Digest

## Orientation
- ISF (Interactive Shader Format) is an annotated GLSL fragment shader: a top-of-file JSON dictionary tells hosts how to run it, expose controls, and chain passes, creating a minimal but flexible format for generators, filters, and transitions.
- The spec purposely stays small; anything GLSL can express (fragment code plus an optional vertex stage) is fair game, and new capabilities land by recognizing extra JSON keys without breaking existing files.
- Spec version 2.0 is the current baseline. Hosts may still load version 1 shaders, and the spec documents the differences so integrators can keep legacy content alive.
- This digest blends the Quick Start intro with the variable and JSON references so you can hand LLMs everything they need to produce ISF shaders that match your workflow.

## Tooling & Sample Libraries
- ISF Editor (Mac + Win) previews shaders with built-in or Syphon/Spout/AVCapture feeds, publishes output via Syphon/Spout, records movies with optional anti-aliasing, autogenerates UI for JSON inputs, offers a syntax-colored editor with inline errors and logs, lets you pause and inspect render passes, and can auto-convert most Shadertoy/GLSL Sandbox pieces.
- Explore the ISF Test/Tutorial filters zip for per-feature samples and the ISF Files GitHub repo for 200+ production generators, effects, and transitions.
- Source implementations span ISFMSLKit (Objective-C/Metal transpilation used in VDMX6), VVISF-GL (cross-platform C++ OpenGL, the backbone of the editor), legacy VVISFKit (Objective-C/OpenGL), and the browser renderer at interactiveshaderformat.com powered by ISF-JS-Renderer.
- Check the ISF integrations page for the latest list of host applications and frameworks that accept ISF compositions.

## Using ISF Compositions
- ISF shaders run across desktop, mobile, and web hosts; use each host manual alongside the integrations list to confirm loading paths and capabilities.
- System folders for shared ISF files: `/Library/Graphics/ISF` (all users) or `~/Library/Graphics/ISF` (per user) on macOS; `/ProgramData/ISF` on Windows, with hosts potentially watching app-specific directories.
- Author, browse, and share shaders at interactiveshaderformat.com, download community compositions, and experiment with the Glitch-hosted standalone renderer to exercise UI bindings in a browser.
- The ISF Primer chapter “Using ISF Compositions” expands on sharing and library management strategies.

## Host Rendering Lifecycle
- On load, the host parses the JSON, auto-declares uniforms for each input, injects helper macros, and generates a default vertex shader if you did not provide a `.vs` file.
- Every frame, the host updates uniform values, executes passes sequentially (incrementing `PASSINDEX`), and writes to temporary or persistent buffers that subsequent passes—or later frames, if persistence is enabled—can sample.
- Image inputs and buffers receive additional uniforms (for dimensions and related metadata); the host recompiles the shader automatically if texture formats change, so sampler declarations stay in sync.

## Creating ISF Compositions
- Build shaders in the online ISF Editor, the desktop editor (macOS/Windows), or any plaintext editor (e.g., TextEdit must be set to “Make Plain Text”).
- Starter exercise: `All Orange.fs` with JSON metadata (`DESCRIPTION: "Every pixel is orange"`, `CREDIT: "by VIDVOX"`, `ISFVSN: "2.0"`, `CATEGORIES: ["TEST-GLSL FX"]`) and fragment code `gl_FragColor = vec4(1.0, 0.5, 0.0, 1.0);`.
- Every ISF file starts with JSON (many attributes optional—full list lives in the JSON reference) followed by GLSL. Hosts call `void main()` per pixel, so you are free to swap in any logic that respects GLSL.
- The Primer chapter “Anatomy of an ISF Composition” dives deeper into file layout and best practices.

## Top-Level JSON Requirements
- The first content in the file must be a `/* ... */` comment containing the top-level JSON dictionary; malformed or missing JSON prevents the shader from loading in compliant hosts.
- Required/common keys include `ISFVSN` (string naming the spec version, e.g. `"2"`, `"2.0"`, `"2.1.1"`, with absence implying a legacy v1 shader), optional `VSN` (string for your own revision tracking), metadata strings such as `DESCRIPTION` and `CREDIT`, the `CATEGORIES` array, the ordered `INPUTS` array, and optional collections like `PASSES` or `IMPORTED` (hosts ignore unfamiliar keys for forward compatibility).
- Sample float-threshold shader JSON illustrates the pattern: describe metadata, declare an `inputImage`, then expose a `level` float with `DEFAULT: 0.5`, `MIN: 0.0`, `MAX: 1.0`; GLSL samples the image, computes luma, and compares against the slider.
- Download the “Test____.fs” pack to see each JSON attribute in action; the examples mirror every feature described in the specification.

## Inputs & Interface Controls
- Every dictionary inside `INPUTS` must at least supply `NAME` (no whitespace; doubles as the GLSL uniform name) and `TYPE` (`event`, `bool`, `long`, `float`, `point2D`, `color`, `image`, `audio`, or `audioFFT`).
- ISF auto-declares uniforms for these entries; redeclaring them in GLSL triggers compilation errors surfaced by the editor. Image inputs, persistent buffers, and temporary targets receive appropriately typed samplers with supporting uniforms.
- Optional keys: `DEFAULT`, `MIN`, `MAX`, and `IDENTITY` refine numeric behavior (colors use RGBA arrays; images ignore these fields). `LABEL` offers human-readable text distinct from the GLSL-safe `NAME`.
- `long` inputs implement popup menus via paired `VALUES` (integers sent to the shader, repeats allowed) and `LABELS` (display strings). `event` inputs act as stateless triggers.
- `audio` and `audioFFT` inputs deliver audio data as images (rows = channels, columns = samples or FFT bins, centered around 0.5). Setting `MAX` requests a specific sample count (e.g., `MAX: 1` for aggregate volume, `MAX: 4` for a four-bin FFT); otherwise the host sends its native resolution at the highest precision it can (ideally 32- or 16-bit float textures).
- `Test Float.fs` demonstrates a float slider driving grayscale output, while `Color Input.fs` exposes a color picker defaulting to `[1.0, 0.5, 0.25, 1.0]`.
- Type-to-uniform mapping: `float` → `float`, `bool`/`event` → `bool` (with `event` true for only the triggered pass), `long` → integer selector, `color` → `vec4` RGBA, `point2D` → `vec2` coordinates, and `image`/`audio`/`audioFFT` → textures sampled through the ISF helpers.

## GLSL Data Types
- GLSL’s foundation rests on four building blocks—data types, variables, functions, and statements—with scalars and vectors covering most shader data. Scalars include `bool` (true/false), `int`/`long` (whole numbers), and `float` (decimals for math and color).
- Vector types bundle scalars: `vec2` for UV/XY pairs, `vec3` for RGB or 3D positions, and `vec4` for RGBA/xyzw. Component aliases (`.xyzw`, `.rgba`) are interchangeable, and scalar math broadcasts across all components (e.g., `myPoint += 0.1;`).
- The special `void` type marks functions with no return value, such as `void main()` or helpers that merely set outputs.

```glsl
void main() {
    float myX = 0.25;
    float myY = 0.5;
    vec2  myPoint = vec2(myX, myY) + 0.1;
    vec3  myColor = vec3(myPoint, 0.5);
    gl_FragColor = vec4(myColor, 1.0);
}
```

## Variable Scope & Constants
- Scope depends on where a variable appears. JSON-defined uniforms are injected by the host, globals (e.g., `const float pi = 3.14159265;`) live outside functions, and locals exist only within their block (`{}`) or loop.
- Prefer `const` for immutable values shared across helpers, and declare working variables as locally as possible to keep shaders readable and efficient.
- ISF hosts already declare uniforms for JSON inputs—redefining them in GLSL leads to compile errors—so limit manual declarations to your own globals and locals.

## Custom Functions & Prototypes
- Define helpers with `returnType name(parameterList) { /* code */ }`, returning any GLSL type or `void`. Encapsulate repeated math (e.g., average intensity) to keep `main()` focused.
- If you place function bodies after `main()`, supply forward declarations first (`float grayscaleAmount(vec4 color);`) so the compiler knows the signature.
- Parameters pass by value; adjust and return explicit results instead of expecting edits to feed back automatically.

## Automatic Variables & ISF Helper Functions
- Core auto uniforms: `PASSINDEX` starts at 0 on the first pass and increments per pass; `RENDERSIZE` reports the current pass dimensions in pixels; `isf_FragNormCoord` gives normalized fragment coordinates (`[0,0]` bottom-left to `[1,1]` top-right); `TIME` counts seconds since the shader started and only updates once per frame; `TIMEDELTA` reports seconds since the previous frame (0 on frame 0); `DATE` is `(year, month, day, secondsWithinDay)`; `FRAMEINDEX` increments per rendered frame.
- JSON-defined inputs (including images, persistent buffers, and temporary targets) automatically become uniforms and samplers; the host recompiles when texture formats change to keep sampler types synced.
- Standard GLSL also exposes `gl_FragCoord` (`vec4` window-space coordinates), so `gl_FragCoord.xy` returns the pixel-space position when you need non-normalized access.
- ISF injects helper macros for image sampling: `IMG_PIXEL` and `IMG_NORM_PIXEL` fetch colors via pixel or normalized coordinates and should replace `texture2D`/`texture2DRect`; `IMG_THIS_PIXEL` and `IMG_NORM_THIS_PIXEL` use the fragment’s current pixel/normalized coordinate automatically; `IMG_SIZE` returns image dimensions.
- `cartesian coordinates.fs` illustrates normalized coordinates by coloring `vec4(isf_FragNormCoord.x, isf_FragNormCoord.y, 0.0, 1.0)`.
- Treat these uniforms and helpers as read-only conveniences—assign new values only to your own locals or outputs.

## GLSL Function Reference
### Basic Number Operations
- `abs`, `sign`, `floor`, `ceil`, `fract`, `min`, `max`, and `mod` accept float or vector inputs (e.g., `float abs(float)`, `vec2 abs(vec2)`). They respectively compute absolute values; sign as `1.0`, `0.0`, or `-1.0`; largest integer ≤ x; smallest integer ≥ x; the fractional component; the smaller or larger of two values; and `x - y * floor(x / y)`. `min`, `max`, and `mod` also offer overloads where the second argument is a scalar applied component-wise.
- `max` performs component-wise comparisons when fed vectors, making it handy for choosing the brightest channel between two colors; swapping in `min` yields the darkest components.

### Angle and Trigonometry
- `radians` and `degrees` convert between degree and radian measures. `sin`, `cos`, and `tan` return the standard trigonometric ratios for radian inputs and operate per component for vectors. `asin`, `acos`, and `atan` are the inverse functions. A two-argument `atan(y, x)` (scalar or vector) converts Cartesian `(x, y)` to the corresponding polar angle.

### Exponential
- `pow` raises `x` to the `y` power; `exp` and `log` operate in base *e* (natural exponential and logarithm); `exp2` and `log2` use base 2. `sqrt` returns the square root, while `inversesqrt` yields `1 / sqrt(x)`. All overloads accept float or vector inputs.

### Clamping and Interpolation
- `clamp` constrains each component of `x` between `minVal` and `maxVal`, with overloads for scalar limits. `mix` performs linear interpolation, returning `(1 - a) * x + a * y` with scalar or vector weight. `step` outputs 0.0 when `x` is below `edge` and 1.0 otherwise, with scalar-edge variants. `smoothstep(edge0, edge1, x)` transitions smoothly from 0.0 to 1.0 using Hermite interpolation; scalar-edge overloads are available.
- `mix` accepts scalar or vector weights; using `isf_FragNormCoord.x` as the weight creates horizontal gradients, while supplying a vector weight enables per-channel blends.

### Geometry
- `length` computes the Euclidean norm, `distance(p0, p1)` measures the separation between points, and `normalize` scales a vector to length 1. `dot` returns the component-wise dot product. `cross` (vec3-only) returns a vector perpendicular to both inputs with magnitude equal to the parallelogram area. `faceforward(N, I, Nref)` flips `N` to match the orientation of `Nref` relative to `I`. `reflect(I, N)` returns the reflection vector; `refract(I, N, eta)` returns the refraction vector given the ratio of refractive indices. Normalize `I` and `N` for correct results.

### Vector Logic Comparisons
- Component-wise comparison helpers (`lessThan`, `lessThanEqual`, `greaterThan`, `greaterThanEqual`, `equal`, `notEqual`) return boolean vectors (`bvec*`) when applied to float or integer vectors. `any` tests whether any component of a boolean vector is true, while `all` requires every component to be true.

## Timed Animations
- `Timed Animation.fs` demonstrates time-driven behavior: it compares `isf_FragNormCoord.x` against `fract(TIME)` to animate a moving bar, outputting `vec4(1.0, 0.5, 0.75, val)`.
- Combine `TIME`, `TIMEDELTA`, and `FRAMEINDEX` for loops, tempo-locking, or frame-step logic; the Primer’s automatic-uniform chapter covers advanced timing tricks.

## Image Filter Conventions
- Any shader meant to process imagery should declare an `INPUTS` entry named `inputImage` with `TYPE: "image"`; hosts treat that as the default filter input.
- macOS editors provide a shortcut to `~/Library/Graphics/ISF` for storing shared filters.
- `firstFX.fs` shows the pattern: metadata plus `inputImage`, a helper `invertColor(vec3)` returning `1.0 - c`, and fragment code that samples with `IMG_NORM_PIXEL` before writing the inverted RGB.
- Multiple image inputs enable masks, displacement maps, or blend sources—sample each via the `IMG_*` helpers.

## Transition Conventions
- Transition shaders require `startImage`, `endImage`, and a normalized float `progress`. Hosts swap these inputs into the render pipeline automatically, so any shader declaring them can operate as a transition.
- Use `progress` for interpolation, wipes, or custom blend logic; persistent buffers or other inputs can add feedback or modulation.

## Audio Waveforms & FFT Data
- `audio` inputs deliver raw waveforms as textures: width = sample count, height = channel count, pixels hold grayscale amplitudes centered at 0.5 (hosts may supply 32-bit floats for precision).
- Declare audio inputs in JSON with `TYPE: "audio"` or `"audioFFT"`, plus `NAME`; optional `MAX` requests a specific sample count (e.g., 1 for overall volume, 256 for detailed waveforms). Add `LABEL` for UI-friendly names.
- Waveform visualizer pattern: sample the first channel with `IMG_NORM_PIXEL(waveImage, vec2(loc.x, channel))`, compare to `loc.y`, and use `smoothstep` plus a configurable `waveSize` to draw the band.
- FFT inputs map columns to frequency bins and rows to channels; an FFT histogram samples `fftImage`, multiplies by `gain`, clamps to a `minRange`/`maxRange` window, and colors bars based on amplitude and `strokeSize`.
- Build spectrograms by combining FFT data with multi-pass persistence: pass 0 copies the newest FFT column into a persistent `fftValues` buffer (scrolling older frames), pass 1 reads that buffer, applies `range`, `axis_scale`, `gain`, and maps to colors via `lumaMode` or frequency-based palettes.
- Expose events (e.g., `clear`) to reset buffers, floats for range/gain, and colors for top/bottom/stroke to let hosts style visualizers without editing GLSL.

## Control Flow & Statements
- Comparison operators (`>`, `>=`, `<`, `<=`, `==`, `!=`) evaluate to booleans and can be chained with `||` (OR) and `&&` (AND) for compound conditions.
- `if` / `else if` / `else` blocks branch logic as needed—use them to short-circuit work or guard against out-of-range texture lookups.
- The ternary operator `condition ? optionA : optionB` is a concise, GPU-friendly alternative to simple `if` statements; it’s ideal for selecting between two colors or values.
- Loop constructs run per pixel, so keep them lightweight: `for` loops advance a counter, `while` loops repeat while a condition remains true, and `do { ... } while (...)` ensures at least one iteration. `break` exits early when extra conditions are satisfied.

```glsl
float val = 0.0;
for (int i = 0; i < 100; ++i) {
    if (i > actualMax || val >= 1.0) break;
    val += 0.1;
}

float sum = 0.0;
while (sum < 1.0) {
    sum += 0.1;
}

gl_FragColor = (gray1 >= gray2) ? color1 : color2;
```

## Including Vertex Shaders
- Vertex shaders manipulate the geometry that fragments are drawn on (the four plane vertices hosting your image), while fragment shaders compute per-pixel color. ISF auto-generates a passthrough vertex shader when none is supplied, which is enough for many effects that only touch pixel data.
- Provide a sibling `.vs` file with the same base name as your fragment shader when you need custom vertex work (e.g., rotations, convolutions, precomputed coordinates). Hosts automatically load it alongside the `.fs`.
- A minimal ISF vertex shader:
  ```glsl
  // passthru.vs
  varying vec2 translated_coord;

  void main() {
      isf_vertShaderInit();            // required host setup
      translated_coord = isf_FragNormCoord;
  }
  ```
  The matching fragment shader declares the same `varying` and samples `IMG_NORM_PIXEL(inputImage, translated_coord);`.
- Always call `isf_vertShaderInit()` at the top of `main()` for proper host setup before applying transforms. JSON-declared uniforms (`float angle`, etc.) are automatically available in the vertex stage just like in the fragment stage.

### Sharing Data with Varyings
- Use `varying` variables to pass per-vertex data to the fragment shader; the rasterizer interpolates these values across the surface. Declare the `varying` with identical types in both `.vs` and `.fs`.
- Example rotation pipeline:
  ```glsl
  // Rotate.vs
  varying vec2 translated_coord;
  const float pi = 3.14159265359;

  void main() {
      isf_vertShaderInit();
      vec2 loc = IMG_SIZE(inputImage) * isf_FragNormCoord;
      float r = distance(IMG_SIZE(inputImage) * 0.5, loc);
      float a = atan(loc.y - IMG_SIZE(inputImage).y * 0.5,
                     loc.x - IMG_SIZE(inputImage).x * 0.5);
      loc.x = r * cos(a + 2.0 * pi * angle);
      loc.y = r * sin(a + 2.0 * pi * angle);
      translated_coord = loc / IMG_SIZE(inputImage) + 0.5;
  }

  // Rotate.fs
  varying vec2 translated_coord;

  void main() {
      vec2 loc = translated_coord;
      if (loc.x < 0.0 || loc.y < 0.0 || loc.x > 1.0 || loc.y > 1.0) {
          gl_FragColor = vec4(0.0);
      } else {
          gl_FragColor = IMG_NORM_PIXEL(inputImage, loc);
      }
  }
  ```
  Moving the polar-to-Cartesian rotation math into the vertex shader reduces per-pixel work, leaving the fragment shader to clamp and sample.
- `varying` has been deprecated in newer GLSL revisions; future ISF versions may migrate to `in/out` qualifiers, so watch release notes when updating hosts.

- Use vertex shaders when adjusting full-frame geometry (rotations, scaling, deformations), precomputing coordinates for convolution kernels, or otherwise needing per-vertex setup before fragment execution.

## Convolution Filters
- Convolution applies a weighted kernel to a pixel and its neighbors, enabling blurs, sharpens, embossing, and edge detection; 3×3 kernels are common, though larger grids follow the same principle.
- Example kernels: identity ([0 in neighbors, center 1]) preserves input; box blur averages weights that sum to 1; sharpen boosts the center while subtracting diagonals; edge detectors use weights summing to 0 to highlight contrast.
- Offload neighbor coordinate math to the vertex shader by exporting eight `varying vec2` offsets (left/right/above/below/diagonals) computed from `isf_FragNormCoord` and `1.0 / RENDERSIZE`, clamped to `[0.0, 1.0]`.
- Publish nine float weights (`w00`–`w22`) in JSON (range `-8.0` to `8.0`) so a generalized fragment shader can multiply each sampled neighbor by its weight while keeping the original alpha.

```glsl
vec3 convolution = (w11 * color +
                    w01 * colorL + w21 * colorR +
                    w10 * colorA + w12 * colorB +
                    w00 * colorLA + w20 * colorRA +
                    w02 * colorLB + w22 * colorRB).rgb;
gl_FragColor = vec4(convolution, color.a);
```

- For a single-parameter box blur, derive weights from `blurLevel`: center weight `1.0 - blurLevel`, neighbors `blurLevel / 8.0`, and skip lookups when the strength is zero.
- Dynamic kernels can be evaluated inside nested `for` loops using `gl_FragCoord.xy + vec2(i, j)` with `IMG_PIXEL`, which scales to 5×5 or data-driven offsets when a vertex shader cannot precompute coordinates.
- Optimize by skipping samples for zero weights, caching sums, and remembering that expanding kernels or increasing passes raises GPU cost.
- Related neighbor-based techniques include morphological erode/dilate (min/max over the neighborhood), frosted-glass effects (non-uniform sampling), and emboss filters that combine kernels with post-color adjustments.

## Multiple Passes & Buffers
- Add dictionaries to the `PASSES` array to describe each render stage; hosts execute them sequentially and expose the output of any pass as an image sampler in later passes.
- `TARGET` names the output buffer. Without `PERSISTENT`, the buffer is temporary for that frame; add `PERSISTENT: true` to keep it across frames, or `FLOAT: true` for 32-bit-per-channel precision when you need accurate accumulation.
- Omit the `PASSES` array entirely for the default single-pass behavior; once present, each dictionary runs in order with `PASSINDEX` starting at 0 and incrementing per pass (available in both vertex and fragment shaders for branching).
- `WIDTH`/`HEIGHT` accept expressions evaluated once per frame (e.g., `$WIDTH/16.0`, `max(floor($WIDTH*0.25),1.0)`, `$blurAmount`) and can reference JSON inputs, allowing parameter-driven scaling or custom buffer sizes.
- `Test-MultiPassRendering.fs` writes to a downsampled persistent buffer on pass 0 and reads it back on pass 1, showcasing cross-pass sampling via the generated sampler name (`bufferVariableNameA` in the example).

## Persistent Buffers
- Persistent buffers are GL textures that live with the ISF composition; declare them via `PASSES` entries with `TARGET` and `PERSISTENT: true`, then read them later with `IMG_*` helpers just like any other image.
- Optional `FLOAT: true` gives each channel 32-bit precision for sensitive accumulators, simulations, or motion trails. Hosts that do not support float buffers silently fall back to standard precision.
- Leave the `PASSES` block out entirely if you want traditional single-pass behavior; persistence only exists when the array is present and a pass marks itself as persistent.
- The classic motion-blur feedback shader blends `IMG_THIS_PIXEL(inputImage)` with `IMG_THIS_PIXEL(bufferVariableNameA)` scaled by `blurAmount`, and can be extended with transforms (invert, rotate, zoom) before mixing to build evolving feedback loops.
- Because hosts resize persistent buffers when output dimensions change, feedback effects survive resolution shifts without manual intervention.

### Feedback Patterns
- Modify the stored buffer each frame to create feedback art: invert colors (`stalePixel.rgb = 1.0 - stalePixel.rgb;`), apply convolution, or offset coordinates before mixing with the new frame.
- Small adjustments to blend ratios accumulate over time—expose controls so users can tune the decay rate and avoid runaway brightness.

### Multi-Pass Blur Example
- The “Soft Blur” filter demonstrates a three-pass blur: pass 0 renders into a tiny buffer (`max(floor($WIDTH*0.02),1.0)`), pass 1 into a quarter-size buffer, and pass 2 outputs full resolution; each pass reuses precomputed neighbor coordinates from the convolution vertex shader.
- On passes where `PASSINDEX` equals 1 or 2, the shader mixes the current 3×3 blur with the buffer produced by the previous pass, weighted by `softness` and `depth`, effectively compounding the blur without large kernels.
- This pattern adapts easily to bloom/glow effects or other cumulative processes: shrink, process, and blend back up while controlling contribution per pass.

### Game of Life with Persistent State
- Cellular automata use persistence to carry state between frames. The Game of Life example stores its board in `lastData`, a persistent buffer, and reuses the convolution vertex shader to fetch neighbor coordinates.
- Initialization occurs on the first frame (`FRAMEINDEX < 1`) or when a `restartNow` event fires, seeding cells with a pseudo-random `rand()` helper modulated by `startThresh`.
- Subsequent frames read the eight neighbors, compute totals, and apply Conway’s rules to update the buffer, illustrating how simulations can run entirely on the GPU and feed into later passes or visualizations.

## Importing External Media
- Use the `IMPORTED` dictionary to request additional images. Each key becomes the sampler name inside GLSL; each value is a dictionary that must include `PATH`, specified relative to the shader (e.g., `"asdf.jpg"`, `"./textures/asdf.jpg"`, `"../shared/asdf.jpg"`).
- Imported images are sampled with the same `IMG_PIXEL`/`IMG_NORM_PIXEL` helpers as other textures. Hosts silently skip files they cannot locate.

## Composition Types
- ISF effects with an `inputImage` act as filters. Transitions expose `startImage`, `endImage`, and `progress`. Any shader that declares neither pattern is treated as a generator.

## Converting Non-ISF GLSL to ISF
- Start with the ISF Editor’s importers when possible (`File → Import from Shadertoy/GLSLSandbox`); the tool fetches the GLSL, builds a JSON header, and stores the result in your system ISF folder. Expect to tidy up edge cases such as missing alpha handling or pass-level naming collisions.
- After automatic conversion, add JSON `INPUTS` so host UIs can expose parameters, update `DESCRIPTION`/`CREDIT`, and verify multi-pass shaders have unique variable names across passes.
- Add the JSON header and move UI-facing uniforms into `INPUTS`; let ISF declare them to avoid redeclarations.
- Replace `texture2D`/`texture2DRect` with `IMG_NORM_PIXEL`/`IMG_PIXEL`, rely on `RENDERSIZE` instead of manual resolution uniforms, and swap external timing variables (`u_time`, `iTime`, etc.) for `TIME`.
- Ensure the alpha channel is intentional—set `gl_FragColor.a = 1.0` if you are unsure—to avoid artifacts in alpha-aware hosts.
- `gl_FragCoord.xy` yields pixel coordinates; `isf_FragNormCoord.xy` provides normalized coordinates and typically simplifies porting.
- Custom vertex shaders should share the fragment shader’s base name, live in a `.vs` file, and call `isf_vertShaderInit();` before doing any work.
- Move external image resources into the `IMPORTED` dictionary and sample them with the helper macros. Clamp or wrap texture coordinates as needed; most hosts clamp by default, so out-of-range lookups may produce unexpected seams.
- Example adaptation (Cellular Noise from The Book of Shaders): replace `u_time`/`u_resolution` with `TIME`/`RENDERSIZE` and remove their declarations, add a JSON `scale` float slider, and consider simplifying `gl_FragCoord.xy/RENDERSIZE.xy` to `isf_FragNormCoord`. The final shader matches the original visuals while gaining host-controlled parameters.

## Version Notes
- Spec 2.0 removed the redundant top-level `PERSISTENT_BUFFERS` object; persistence and float precision now belong inside each pass dictionary.
- Legacy identifiers `vv_FragNormCoord` and `vv_vertShaderInit()` were renamed to `isf_FragNormCoord` and `isf_vertShaderInit()` to reflect the open specification.
- Version 2.0 introduced `audio`/`audioFFT` input types, the `IMG_SIZE` helper, and the `TIMEDELTA`, `DATE`, and `FRAMEINDEX` uniforms, along with explicit conventions for filters and transitions and the addition of `ISFVSN`/`VSN` labeling.

## Next Steps
- Continue through the ISF Primer for advanced GLSL lessons, vertex techniques, and multi-pass recipes.
- Keep the ISF Reference Pages handy for quick lookups of uniforms, JSON keys, helper macros, and built-in tables.
- Browse interactiveshaderformat.com for community compositions to learn from, remix, or deploy.
- Review the full ISF Specification on GitHub when implementing host integrations or advanced tooling.
