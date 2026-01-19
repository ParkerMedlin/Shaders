/*{
    "DESCRIPTION": "Creates motion trail illusion by sampling input at multiple UV offsets. Works without persistent buffers - uses current frame only.",
    "CREDIT": "Spatial Echo Trail",
    "ISFVSN": "2.0",
    "CATEGORIES": ["Stylize"],
    "INPUTS": [
        {
            "NAME": "inputImage",
            "TYPE": "image"
        },
        {
            "NAME": "trailAngle",
            "TYPE": "float",
            "LABEL": "Trail Direction",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 360.0
        },
        {
            "NAME": "trailLength",
            "TYPE": "float",
            "LABEL": "Trail Length",
            "DEFAULT": 0.1,
            "MIN": 0.0,
            "MAX": 0.5
        },
        {
            "NAME": "echoCount",
            "TYPE": "long",
            "LABEL": "Echo Samples",
            "DEFAULT": 4,
            "VALUES": [2, 4, 6, 8, 12, 16],
            "LABELS": ["2", "4", "6", "8", "12", "16"]
        },
        {
            "NAME": "fadeAmount",
            "TYPE": "float",
            "LABEL": "Echo Fade",
            "DEFAULT": 0.7,
            "MIN": 0.1,
            "MAX": 0.95
        },
        {
            "NAME": "blendMode",
            "TYPE": "long",
            "LABEL": "Blend Mode",
            "DEFAULT": 0,
            "VALUES": [0, 1, 2],
            "LABELS": ["Additive", "Average", "Max"]
        }
    ]
}*/

void main() {
    vec2 uv = isf_FragNormCoord;

    // Convert angle to radians and create direction vector
    float angleRad = radians(trailAngle);
    vec2 trailDir = vec2(cos(angleRad), sin(angleRad));

    // Correct for aspect ratio
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    trailDir.x /= aspect;

    // Get the number of samples from the long input
    int samples = echoCount;

    // Start with the original pixel
    vec4 baseColor = IMG_NORM_PIXEL(inputImage, uv);
    vec4 result = baseColor;
    float totalWeight = 1.0;

    // Sample at offset positions along trail direction
    float currentFade = 1.0;

    for (int i = 1; i <= 16; i++) {
        if (i > samples) break;

        // Calculate offset for this echo
        float t = float(i) / float(samples);
        vec2 offset = trailDir * trailLength * t;
        vec2 sampleUV = uv + offset;

        // Fade multiplier for this echo
        currentFade *= fadeAmount;

        // Sample if within bounds
        if (sampleUV.x >= 0.0 && sampleUV.x <= 1.0 &&
            sampleUV.y >= 0.0 && sampleUV.y <= 1.0) {

            vec4 echoColor = IMG_NORM_PIXEL(inputImage, sampleUV);

            if (blendMode == 0) {
                // Additive blend
                result.rgb += echoColor.rgb * currentFade;
            } else if (blendMode == 1) {
                // Weighted average
                result.rgb += echoColor.rgb * currentFade;
                totalWeight += currentFade;
            } else {
                // Max/Lighten
                result.rgb = max(result.rgb, echoColor.rgb * currentFade);
            }
        }
    }

    // Normalize for average mode
    if (blendMode == 1) {
        result.rgb /= totalWeight;
    }

    // Clamp additive mode to prevent blowout
    if (blendMode == 0) {
        result.rgb = clamp(result.rgb, 0.0, 1.0);
    }

    result.a = baseColor.a;
    gl_FragColor = result;
}
