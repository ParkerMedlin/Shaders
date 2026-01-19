/*{
    "DESCRIPTION": "CRT TV effect with rounded rectangle phosphor cells, chromatic aberration, vignette, and flicker. Worn but working aesthetic.",
    "CREDIT": "ISF Shader",
    "ISFVSN": "2.0",
    "CATEGORIES": ["Retro", "Stylize"],
    "INPUTS": [
        {
            "NAME": "inputImage",
            "TYPE": "image"
        },
        {
            "NAME": "cellSize",
            "TYPE": "float",
            "DEFAULT": 4.0,
            "MIN": 2.0,
            "MAX": 12.0,
            "LABEL": "Cell Size"
        },
        {
            "NAME": "cellIntensity",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Cell Intensity"
        },
        {
            "NAME": "vignetteAmount",
            "TYPE": "float",
            "DEFAULT": 0.4,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Vignette Amount"
        },
        {
            "NAME": "chromaticAberration",
            "TYPE": "float",
            "DEFAULT": 0.3,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Chromatic Aberration"
        },
        {
            "NAME": "phosphorIntensity",
            "TYPE": "float",
            "DEFAULT": 0.4,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Phosphor Intensity"
        },
        {
            "NAME": "flickerAmount",
            "TYPE": "float",
            "DEFAULT": 0.15,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Flicker Amount"
        }
    ]
}*/

// Signed distance function for a rounded rectangle
// p = point, b = box half-size, r = corner radius
float sdRoundedRect(vec2 p, vec2 b, float r) {
    vec2 q = abs(p) - b + r;
    return length(max(q, 0.0)) + min(max(q.x, q.y), 0.0) - r;
}

void main() {
    vec2 uv = isf_FragNormCoord;
    vec2 pixelCoord = gl_FragCoord.xy;

    // Calculate distance from center for edge-weighted effects
    vec2 centerOffset = uv - 0.5;
    float edgeDistance = length(centerOffset) * 1.414;
    float edgeFactor = edgeDistance * edgeDistance;

    // --- Edge-Weighted Chromatic Aberration ---
    float caOffset = chromaticAberration * 0.01 * edgeFactor;
    vec2 caDirection = normalize(centerOffset + 0.001);

    float r = IMG_NORM_PIXEL(inputImage, uv + caDirection * caOffset).r;
    float g = IMG_NORM_PIXEL(inputImage, uv).g;
    float b = IMG_NORM_PIXEL(inputImage, uv - caDirection * caOffset).b;

    vec4 color = vec4(r, g, b, 1.0);

    // --- Rounded Rectangle Phosphor Grid ---
    // Each cell is a rounded rectangle containing RGB subpixels

    float cellW = cellSize * 3.0; // Width includes 3 RGB subpixels
    float cellH = cellSize;       // Height of each cell

    // Find position within current cell (centered, so -0.5 to 0.5 range)
    vec2 cellPos = vec2(
        mod(pixelCoord.x, cellW) / cellW - 0.5,
        mod(pixelCoord.y, cellH) / cellH - 0.5
    );

    // Rounded rectangle parameters
    vec2 rectSize = vec2(0.42, 0.38); // Slightly smaller than cell to create gaps
    float cornerRadius = 0.15;

    // Calculate distance to rounded rectangle edge
    float dist = sdRoundedRect(cellPos, rectSize, cornerRadius);

    // Create soft edge for the cell (inside = 1, outside = 0)
    float cellMask = 1.0 - smoothstep(-0.02, 0.02, dist);

    // --- RGB Phosphor Subpixels within each cell ---
    vec3 phosphorMask = vec3(1.0);
    if (phosphorIntensity > 0.0) {
        // Position within the cell's width (0 to 1)
        float subpixelPos = mod(pixelCoord.x, cellW) / cellW;

        // Three vertical stripes for R, G, B
        vec3 phosphorPattern;

        // Each subpixel takes 1/3 of the cell width
        // Centered at 1/6, 3/6, 5/6
        float stripeWidth = 0.12;
        phosphorPattern.r = smoothstep(stripeWidth, 0.0, abs(subpixelPos - 0.167));
        phosphorPattern.g = smoothstep(stripeWidth, 0.0, abs(subpixelPos - 0.5));
        phosphorPattern.b = smoothstep(stripeWidth, 0.0, abs(subpixelPos - 0.833));

        // Boost the phosphor pattern so it's visible but not too dark
        phosphorPattern = phosphorPattern * 0.6 + 0.4;

        // More visible on bright content
        float brightness = dot(color.rgb, vec3(0.299, 0.587, 0.114));
        float phosphorVisibility = mix(0.4, 1.0, brightness);

        phosphorMask = mix(vec3(1.0), phosphorPattern, phosphorIntensity * phosphorVisibility);
    }

    // Combine cell mask with phosphor pattern
    float finalCellMask = mix(1.0, cellMask, cellIntensity);
    color.rgb *= phosphorMask * finalCellMask;

    // --- Vignette ---
    if (vignetteAmount > 0.0) {
        float vignette = 1.0 - edgeFactor * vignetteAmount;
        vignette = smoothstep(0.0, 1.0, vignette);
        color.rgb *= vignette;
    }

    // --- Subtle Flicker ---
    if (flickerAmount > 0.0) {
        float flicker = 1.0;
        flicker += sin(TIME * 60.0 * 6.28318) * 0.01;
        flicker += sin(TIME * 30.0 * 6.28318) * 0.005;
        flicker += sin(TIME * 1.5 * 6.28318) * 0.02;
        flicker += sin(TIME * 47.0) * sin(TIME * 13.0) * 0.015;
        flicker = mix(1.0, flicker, flickerAmount);
        color.rgb *= flicker;
    }

    // Brightness boost to compensate for darkening
    color.rgb *= 1.15;

    color.rgb = clamp(color.rgb, 0.0, 1.0);
    color.a = 1.0;

    gl_FragColor = color;
}
