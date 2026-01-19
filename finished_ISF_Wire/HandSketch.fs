/*{
    "DESCRIPTION": "Makes input look hand-sketched with pencil strokes and cross-hatching",
    "CREDIT": "by Claude",
    "ISFVSN": "2.0",
    "CATEGORIES": ["Stylize"],
    "INPUTS": [
        {"NAME": "inputImage", "TYPE": "image"},
        {"NAME": "edgeStrength", "TYPE": "float", "DEFAULT": 1.5, "MIN": 0.0, "MAX": 3.0, "LABEL": "Edge Strength"},
        {"NAME": "hatchDensity", "TYPE": "float", "DEFAULT": 80.0, "MIN": 20.0, "MAX": 200.0, "LABEL": "Hatch Density"},
        {"NAME": "hatchIntensity", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.0, "MAX": 1.0, "LABEL": "Hatching Amount"},
        {"NAME": "sketchWobble", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0, "LABEL": "Line Wobble"},
        {"NAME": "paperTone", "TYPE": "color", "DEFAULT": [0.95, 0.93, 0.88, 1.0], "LABEL": "Paper Color"},
        {"NAME": "inkTone", "TYPE": "color", "DEFAULT": [0.15, 0.12, 0.1, 1.0], "LABEL": "Ink Color"},
        {"NAME": "animate", "TYPE": "bool", "DEFAULT": false, "LABEL": "Animate Wobble"}
    ]
}*/

// Simple hash functions for noise
float hash(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
}

float hash2(vec2 p) {
    return fract(sin(dot(p, vec2(269.5, 183.3))) * 43758.5453);
}

// Value noise
float noise(vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);
    f = f * f * (3.0 - 2.0 * f);

    float a = hash(i);
    float b = hash(i + vec2(1.0, 0.0));
    float c = hash(i + vec2(0.0, 1.0));
    float d = hash(i + vec2(1.0, 1.0));

    return mix(mix(a, b, f.x), mix(c, d, f.x), f.y);
}

// Get luminance from color
float getLuma(vec3 c) {
    return dot(c, vec3(0.299, 0.587, 0.114));
}

// Sobel edge detection
float detectEdges(vec2 uv) {
    vec2 texel = 1.0 / RENDERSIZE;

    // Add wobble to sampling positions for hand-drawn feel
    float wobbleTime = animate ? TIME * 2.0 : 0.0;
    float wobbleAmount = sketchWobble * 0.002;
    vec2 wobble = vec2(
        noise(uv * 50.0 + wobbleTime) - 0.5,
        noise(uv * 50.0 + 100.0 + wobbleTime) - 0.5
    ) * wobbleAmount;

    // Sample 3x3 neighborhood
    float tl = getLuma(IMG_NORM_PIXEL(inputImage, uv + wobble + vec2(-texel.x, texel.y)).rgb);
    float t  = getLuma(IMG_NORM_PIXEL(inputImage, uv + wobble + vec2(0.0, texel.y)).rgb);
    float tr = getLuma(IMG_NORM_PIXEL(inputImage, uv + wobble + vec2(texel.x, texel.y)).rgb);
    float l  = getLuma(IMG_NORM_PIXEL(inputImage, uv + wobble + vec2(-texel.x, 0.0)).rgb);
    float r  = getLuma(IMG_NORM_PIXEL(inputImage, uv + wobble + vec2(texel.x, 0.0)).rgb);
    float bl = getLuma(IMG_NORM_PIXEL(inputImage, uv + wobble + vec2(-texel.x, -texel.y)).rgb);
    float b  = getLuma(IMG_NORM_PIXEL(inputImage, uv + wobble + vec2(0.0, -texel.y)).rgb);
    float br = getLuma(IMG_NORM_PIXEL(inputImage, uv + wobble + vec2(texel.x, -texel.y)).rgb);

    // Sobel kernels
    float gx = -tl - 2.0*l - bl + tr + 2.0*r + br;
    float gy = -tl - 2.0*t - tr + bl + 2.0*b + br;

    return sqrt(gx*gx + gy*gy);
}

// Cross-hatching pattern
float hatchPattern(vec2 uv, float darkness, float angle) {
    float wobbleTime = animate ? TIME * 1.5 : 0.0;

    // Rotate coordinates for hatching direction
    float c = cos(angle);
    float s = sin(angle);
    vec2 rotUV = vec2(
        uv.x * c - uv.y * s,
        uv.x * s + uv.y * c
    );

    // Add hand-drawn wobble to lines
    float lineWobble = noise(rotUV * 5.0 + wobbleTime) * sketchWobble * 0.1;

    // Create line pattern
    float line = sin((rotUV.x + lineWobble) * hatchDensity);

    // Vary line thickness based on noise for imperfect strokes
    float thicknessVar = noise(rotUV * 20.0 + wobbleTime * 0.5) * 0.3 + 0.7;

    // Threshold based on darkness level
    float threshold = 1.0 - darkness * 2.0;
    return smoothstep(threshold - 0.1 * thicknessVar, threshold + 0.1 * thicknessVar, line);
}

// Multi-layer cross-hatching
float crossHatch(vec2 uv, float darkness) {
    float result = 1.0;

    // Layer 1: Main diagonal hatching (appears in lighter areas)
    if (darkness > 0.15) {
        result *= 1.0 - hatchPattern(uv, darkness * 0.6, 0.785) * 0.5;
    }

    // Layer 2: Cross hatching (appears in medium areas)
    if (darkness > 0.35) {
        result *= 1.0 - hatchPattern(uv, (darkness - 0.2) * 0.8, -0.785) * 0.5;
    }

    // Layer 3: Horizontal lines (appears in darker areas)
    if (darkness > 0.55) {
        result *= 1.0 - hatchPattern(uv, (darkness - 0.4) * 0.9, 0.0) * 0.4;
    }

    // Layer 4: Dense fill for very dark areas
    if (darkness > 0.75) {
        result *= 1.0 - hatchPattern(uv, (darkness - 0.6), 1.57) * 0.4;
    }

    return result;
}

void main() {
    vec2 uv = isf_FragNormCoord;

    // Get original color and luminance
    vec4 original = IMG_NORM_PIXEL(inputImage, uv);
    float luma = getLuma(original.rgb);
    float darkness = 1.0 - luma;

    // Detect edges
    float edges = detectEdges(uv);
    edges = pow(edges * edgeStrength, 1.2);
    edges = clamp(edges, 0.0, 1.0);

    // Add noise variation to edges for pencil texture
    float wobbleTime = animate ? TIME : 0.0;
    float edgeNoise = noise(uv * RENDERSIZE * 0.5 + wobbleTime) * 0.3 + 0.7;
    edges *= edgeNoise;

    // Cross-hatching for shading
    vec2 hatchUV = uv * vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0);
    float hatching = crossHatch(hatchUV, darkness);

    // Combine: start with paper, add hatching and edges
    vec3 result = paperTone.rgb;

    // Apply cross-hatching
    result = mix(result, inkTone.rgb, (1.0 - hatching) * hatchIntensity);

    // Apply edges (always dark)
    result = mix(result, inkTone.rgb, edges);

    // Add subtle paper texture
    float paperNoise = noise(uv * RENDERSIZE * 0.3) * 0.03;
    result += paperNoise - 0.015;

    // Add slight pencil grain
    float grain = hash(uv * RENDERSIZE + wobbleTime) * 0.02 - 0.01;
    result += grain;

    gl_FragColor = vec4(result, original.a);
}
