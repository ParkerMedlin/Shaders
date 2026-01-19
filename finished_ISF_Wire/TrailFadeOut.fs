/*{
    "ISFVSN": "2.0",
    "DESCRIPTION": "Triggered sequence: feedback trails build, source stutters and fades, then trails shrink to nothing. Route output back to feedbackImage in Wire.",
    "CREDIT": "Custom ISF",
    "CATEGORIES": ["Transition", "Feedback"],
    "INPUTS": [
        {"NAME": "inputImage", "TYPE": "image"},
        {"NAME": "feedbackImage", "TYPE": "image"},
        {"NAME": "active", "TYPE": "bool", "DEFAULT": false, "LABEL": "Active"},
        {"NAME": "triggerTime", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 10000.0, "LABEL": "Trigger Time (set to TIME)"},
        {"NAME": "duration", "TYPE": "float", "DEFAULT": 3.0, "MIN": 0.5, "MAX": 10.0, "LABEL": "Duration (seconds)"},
        {"NAME": "trailDecay", "TYPE": "float", "DEFAULT": 0.92, "MIN": 0.5, "MAX": 0.99, "LABEL": "Trail Decay"},
        {"NAME": "shrinkScale", "TYPE": "float", "DEFAULT": 0.95, "MIN": 0.8, "MAX": 0.99, "LABEL": "Shrink Scale"},
        {"NAME": "stutterSpeed", "TYPE": "float", "DEFAULT": 20.0, "MIN": 5.0, "MAX": 60.0, "LABEL": "Stutter Speed"}
    ]
}*/

void main() {
    vec2 uv = isf_FragNormCoord;

    vec4 source = IMG_NORM_PIXEL(inputImage, uv);

    // Not active - pass through source
    if (!active) {
        gl_FragColor = source;
        return;
    }

    // Calculate progress based on time since trigger
    float elapsed = TIME - triggerTime;
    float progress = clamp(elapsed / duration, 0.0, 1.0);

    // Phase boundaries
    // 0-30%: trails building + full source visible
    // 30-60%: source stutters and fades out
    // 60-100%: source gone, trails shrink to center
    float trailPhaseEnd = 0.3;
    float stutterPhaseEnd = 0.6;

    bool inShrinkPhase = progress > stutterPhaseEnd;

    // Feedback UV - constant scale applied every frame during shrink phase
    // This compounds over time, making trails progressively smaller
    vec2 feedbackUV = uv;
    if (inShrinkPhase) {
        // Constant scale toward center - compounds each frame
        feedbackUV = (uv - 0.5) / shrinkScale + 0.5;
    }

    // Sample feedback (black if outside bounds after scaling)
    vec4 feedback = vec4(0.0);
    if (feedbackUV.x >= 0.0 && feedbackUV.x <= 1.0 &&
        feedbackUV.y >= 0.0 && feedbackUV.y <= 1.0) {
        feedback = IMG_NORM_PIXEL(feedbackImage, feedbackUV);
    }

    // Source visibility based on phase
    float sourceVis = 0.0;

    if (progress <= trailPhaseEnd) {
        // Trail phase: full source with trails building
        sourceVis = 1.0;
    } else if (progress <= stutterPhaseEnd) {
        // Stutter phase: flickering and fading source
        float stutterProgress = (progress - trailPhaseEnd) / (stutterPhaseEnd - trailPhaseEnd);
        float flicker = step(0.4, fract(TIME * stutterSpeed + stutterProgress * 5.0));
        sourceVis = (1.0 - stutterProgress) * flicker;
    }
    // else: shrink phase - sourceVis stays 0

    // Feedback strength - no fade during shrink, let the scaling do the work
    float fbStrength = trailDecay;

    // Composite: source over decayed feedback
    vec4 result = source * sourceVis + feedback * fbStrength;
    result.a = 1.0;

    gl_FragColor = result;
}
