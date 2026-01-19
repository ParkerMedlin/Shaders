/*{
    "DESCRIPTION": "Magnifies input texture based on B&W mask - white areas zoom in toward center",
    "CREDIT": "ISF Generator",
    "ISFVSN": "2.0",
    "CATEGORIES": ["Distortion Effect"],
    "INPUTS": [
        {"NAME": "inputImage", "TYPE": "image", "LABEL": "Input"},
        {"NAME": "maskImage", "TYPE": "image", "LABEL": "Mask (B&W)"},
        {"NAME": "zoomCenter", "TYPE": "point2D", "DEFAULT": [0.5, 0.5], "LABEL": "Zoom Center"},
        {"NAME": "maskBlur", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 0.1, "LABEL": "Mask Blur"},
        {"NAME": "zoomAmount", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 0.95, "LABEL": "Zoom Amount"},
        {"NAME": "displacementStrength", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Displacement Strength"}
    ]
}*/

void main() {
    vec2 uv = isf_FragNormCoord;

    // Efficient 5-sample cross blur for mask (hyper-efficient)
    float mask;
    if (maskBlur > 0.001) {
        float b = maskBlur;
        mask = IMG_NORM_PIXEL(maskImage, uv).r * 0.4;
        mask += IMG_NORM_PIXEL(maskImage, uv + vec2(b, 0.0)).r * 0.15;
        mask += IMG_NORM_PIXEL(maskImage, uv + vec2(-b, 0.0)).r * 0.15;
        mask += IMG_NORM_PIXEL(maskImage, uv + vec2(0.0, b)).r * 0.15;
        mask += IMG_NORM_PIXEL(maskImage, uv + vec2(0.0, -b)).r * 0.15;
    } else {
        mask = IMG_NORM_PIXEL(maskImage, uv).r;
    }

    // Apply displacement strength to mask
    float effectMask = clamp(mask * displacementStrength, 0.0, 1.0);

    // Magnify: scale UV offset from center based on mask
    // Lower scale = more zoom in
    vec2 offset = uv - zoomCenter;
    float scale = 1.0 - effectMask * zoomAmount;
    vec2 displaced_uv = zoomCenter + offset * scale;

    vec4 color = IMG_NORM_PIXEL(inputImage, displaced_uv);
    gl_FragColor = color;
}
