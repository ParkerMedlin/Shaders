/*{
    "DESCRIPTION": "Fed DSC-011 LSD - Time-based color separation with feedback zoom",
    "CREDIT": "Original by Fed, adapted for ISF",
    "ISFVSN": "2.0",
    "CATEGORIES": ["Generator"],
    "INPUTS": [
        {
            "NAME": "feedbackInput",
            "TYPE": "image",
            "LABEL": "Feedback Input"
        },
        {
            "NAME": "zoomSpeed",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0.0,
            "MAX": 2.0,
            "LABEL": "Zoom Speed"
        },
        {
            "NAME": "zoomAmount",
            "TYPE": "float",
            "DEFAULT": 0.01,
            "MIN": 0.0,
            "MAX": 0.05,
            "LABEL": "Zoom Amount"
        },
        {
            "NAME": "colorSpeed",
            "TYPE": "float",
            "DEFAULT": 0.7,
            "MIN": 0.0,
            "MAX": 2.0,
            "LABEL": "Color Speed"
        },
        {
            "NAME": "seedAmount",
            "TYPE": "float",
            "DEFAULT": 0.02,
            "MIN": 0.0,
            "MAX": 0.1,
            "LABEL": "Seed Amount"
        }
    ]
}*/

// Pseudo-random noise
float rand(vec2 co) {
    return fract(sin(dot(co, vec2(12.9898, 78.233))) * 43758.5453);
}

void main() {
    vec2 uv = isf_FragNormCoord;
    float d = length(uv - 0.5);
    float centerMask = smoothstep(0.35, 0.0, d);

    // Time-based zoom (gentle oscillation around 1.0)
    float zoom = 1.0 + zoomAmount * (0.5 + 0.5 * sin(TIME * zoomSpeed));

    // Transform UV around center for zoom
    vec2 zoomedUV = (uv - 0.5) / zoom + 0.5;

    // Sample feedback
    vec3 fb = IMG_NORM_PIXEL(feedbackInput, zoomedUV).rgb;

    // Luminosity extraction (min of RGB - MilkDrop style)
    float lum = min(min(fb.r, fb.g), fb.b);

    // Hue extraction (color minus luminosity)
    vec3 hue = fb - lum;

    // Time-based color tinting (replaces audio-reactive q1/q2/q3)
    float q1 = 0.6 + 0.2 * sin(TIME * colorSpeed * 1.0);   // G channel
    float q2 = 0.6 + 0.2 * sin(TIME * colorSpeed * 1.3);   // B channel
    float q3 = 0.5 + 0.3 * sin(TIME * colorSpeed * 0.7);   // R channel

    // Recombine: tint * luminosity + hue * 1.01
    vec3 tint = vec3(q3, q1, q2);
    vec3 result = tint * lum + hue * 1.01;

    // Cross-channel gamma curve (the distinctive psychedelic effect)
    // Each channel's gamma is influenced by a different channel's value
    result = pow(result, result.zxy + 0.8);

    // Gradual decay (reduced near center to avoid a persistent hole)
    float decay = mix(0.0006, 0.002, 1.0 - centerMask);
    result -= decay;

    // Seed noise to sustain the reaction (slightly stronger near center)
    vec3 noise = vec3(
        rand(uv + TIME),
        rand(uv + TIME + 1.0),
        rand(uv + TIME + 2.0)
    );
    result += noise * seedAmount * (1.0 + centerMask * 1.5);

    // Clamp to valid range
    result = clamp(result, vec3(0.0), vec3(1.0));

    gl_FragColor = vec4(result, 1.0);
}
