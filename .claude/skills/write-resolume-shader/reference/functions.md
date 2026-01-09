# ISF & GLSL Functions Reference

## ISF Image Functions

### IMG_PIXEL
Sample image at pixel coordinates.
```glsl
vec4 color = IMG_PIXEL(imageName, vec2(x, y));
```

### IMG_NORM_PIXEL
Sample image at normalized coordinates (0.0-1.0).
```glsl
vec4 color = IMG_NORM_PIXEL(imageName, vec2(u, v));
```

### IMG_THIS_PIXEL
Sample current fragment from image.
```glsl
vec4 color = IMG_THIS_PIXEL(imageName);
// Equivalent to: IMG_PIXEL(imageName, gl_FragCoord.xy)
```

### IMG_THIS_NORM_PIXEL
Sample current fragment using normalized coords.
```glsl
vec4 color = IMG_THIS_NORM_PIXEL(imageName);
// Equivalent to: IMG_NORM_PIXEL(imageName, isf_FragNormCoord)
```

### IMG_SIZE
Get image dimensions.
```glsl
vec2 size = IMG_SIZE(imageName);
```

## GLSL Math Functions

### Basic Operations
```glsl
abs(x)          // Absolute value
sign(x)         // Returns -1.0, 0.0, or 1.0
floor(x)        // Round down
ceil(x)         // Round up
fract(x)        // Fractional part (x - floor(x))
mod(x, y)       // Modulo (x - y * floor(x/y))
min(x, y)       // Minimum value
max(x, y)       // Maximum value
clamp(x, min, max)  // Constrain to range
```

### Interpolation
```glsl
mix(a, b, t)        // Linear interpolation: a*(1-t) + b*t
step(edge, x)       // 0.0 if x < edge, else 1.0
smoothstep(e0, e1, x)  // Smooth Hermite interpolation
```

### Trigonometry
```glsl
radians(degrees)    // Convert degrees to radians
degrees(radians)    // Convert radians to degrees
sin(x), cos(x), tan(x)
asin(x), acos(x), atan(x)
atan(y, x)          // Two-argument arctangent
```

### Exponential
```glsl
pow(x, y)       // x raised to power y
exp(x)          // e^x
log(x)          // Natural logarithm
exp2(x)         // 2^x
log2(x)         // Base-2 logarithm
sqrt(x)         // Square root
inversesqrt(x)  // 1/sqrt(x)
```

### Geometry
```glsl
length(v)           // Vector length
distance(p0, p1)    // Distance between points
normalize(v)        // Unit vector
dot(a, b)           // Dot product
cross(a, b)         // Cross product (vec3 only)
reflect(I, N)       // Reflection vector
refract(I, N, eta)  // Refraction vector
```

### Vector Comparisons
```glsl
lessThan(x, y)          // Returns bvec, component-wise x < y
lessThanEqual(x, y)     // x <= y
greaterThan(x, y)       // x > y
greaterThanEqual(x, y)  // x >= y
equal(x, y)             // x == y
notEqual(x, y)          // x != y
any(bvec)               // True if any component true
all(bvec)               // True if all components true
```

## Common Utility Functions

### Pseudo-Random Number
```glsl
float rand(vec2 co) {
    return fract(sin(dot(co, vec2(12.9898, 78.233))) * 43758.5453);
}
```

### Grayscale/Luminance
```glsl
float luma(vec3 color) {
    return dot(color, vec3(0.299, 0.587, 0.114));
}
```

### Rotate 2D Point
```glsl
vec2 rotate2D(vec2 p, float angle) {
    float c = cos(angle);
    float s = sin(angle);
    return vec2(p.x * c - p.y * s, p.x * s + p.y * c);
}
```

### Polar Coordinates
```glsl
vec2 toPolar(vec2 cartesian) {
    return vec2(length(cartesian), atan(cartesian.y, cartesian.x));
}

vec2 toCartesian(vec2 polar) {
    return vec2(polar.x * cos(polar.y), polar.x * sin(polar.y));
}
```

### RGB to HSV
```glsl
vec3 rgb2hsv(vec3 c) {
    vec4 K = vec4(0.0, -1.0/3.0, 2.0/3.0, -1.0);
    vec4 p = mix(vec4(c.bg, K.wz), vec4(c.gb, K.xy), step(c.b, c.g));
    vec4 q = mix(vec4(p.xyw, c.r), vec4(c.r, p.yzx), step(p.x, c.r));
    float d = q.x - min(q.w, q.y);
    float e = 1.0e-10;
    return vec3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}
```

### HSV to RGB
```glsl
vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}
```
