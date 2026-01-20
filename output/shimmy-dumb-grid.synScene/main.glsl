// Shimmy Dumb Grid - Adapted from MilkDrop preset by suksma/Hexcollie/Julian Carnival
// Features: Conway's Game of Life, Moebius transformation, orbiting circles, audio reactivity

//============================================================================
// AUDIO SMOOTHING
//============================================================================

// Recreate MilkDrop's smoothed audio values
vec3 getSmoothedAudio() {
    float bass = syn_BassLevel;
    float mid = syn_MidLevel;
    float treb = syn_HighLevel;

    // Apply smoothing similar to MilkDrop's vvb, vvm, vvt
    // syn_*Level uniforms are already smoothed, so we use them directly
    return vec3(bass, mid, treb) * audioReactivity;
}

float getCombinedAudio() {
    vec3 audio = getSmoothedAudio();
    return (audio.x + audio.y + audio.z) * 2.0;
}

//============================================================================
// UTILITY FUNCTIONS
//============================================================================

float hash21(vec2 p) {
    p = fract(p * vec2(234.34, 435.345));
    p += dot(p, p + 34.23);
    return fract(p.x * p.y);
}

// Soft circle SDF
float circle(vec2 uv, vec2 center, float radius) {
    return 1.0 - smoothstep(radius - 0.02, radius + 0.02, length(uv - center));
}

// Gradient circle (center to edge)
float gradientCircle(vec2 uv, vec2 center, float radius) {
    float d = length(uv - center);
    return smoothstep(radius, 0.0, d);
}

//============================================================================
// CONWAY'S GAME OF LIFE
//============================================================================

float countNeighbors(vec2 uv, vec2 pixelSize) {
    float count = 0.0;

    count += texture(gameOfLife, uv + vec2(-1.0, -1.0) * pixelSize).r;
    count += texture(gameOfLife, uv + vec2( 0.0, -1.0) * pixelSize).r;
    count += texture(gameOfLife, uv + vec2( 1.0, -1.0) * pixelSize).r;
    count += texture(gameOfLife, uv + vec2(-1.0,  0.0) * pixelSize).r;
    count += texture(gameOfLife, uv + vec2( 1.0,  0.0) * pixelSize).r;
    count += texture(gameOfLife, uv + vec2(-1.0,  1.0) * pixelSize).r;
    count += texture(gameOfLife, uv + vec2( 0.0,  1.0) * pixelSize).r;
    count += texture(gameOfLife, uv + vec2( 1.0,  1.0) * pixelSize).r;

    return floor(count + 0.5);
}

vec4 renderGameOfLife(vec2 uv) {
    vec2 pixelSize = 1.0 / RENDERSIZE;

    float neighbors = countNeighbors(uv, pixelSize);
    float current = texture(gameOfLife, uv).r;
    current = current > 0.5 ? 1.0 : 0.0;

    float next = 0.0;

    // Conway's rules
    if (current > 0.5) {
        // Cell is alive
        if (neighbors >= 2.0 && neighbors <= 3.0) {
            next = 1.0; // Survives
        }
    } else {
        // Cell is dead
        if (neighbors >= 2.5 && neighbors <= 3.5) {
            next = 1.0; // Born
        }
    }

    // Spawn new cells based on audio
    float spawnChance = golSpawnRate * (1.0 + getCombinedAudio() * 0.5);
    float randomVal = hash21(uv * 1000.0 + TIME * golSpeed);
    if (randomVal < spawnChance) {
        next = 1.0;
    }

    // Also spawn where circles are (seeds the simulation)
    vec2 aspect = vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0);
    vec2 uvAspect = (uv - 0.5) * aspect;

    for (int i = 0; i < 3; i++) {
        float speed1 = 1.23 + float(i) * 0.1;
        float speed2 = 1.43 - float(i) * 0.15;
        vec2 center = vec2(
            0.3 * cos(TIME * speed1) + 0.03 * cos(TIME * 0.7),
            0.3 * sin(TIME * speed2) + 0.03 * sin(TIME * 0.7 + float(i))
        );

        if (length(uvAspect - center) < 0.05) {
            if (hash21(uv * 500.0 + TIME) < 0.1) {
                next = 1.0;
            }
        }
    }

    // Store in red channel
    // Green channel: flow buffer Y value (edge-enhanced feedback)
    // Blue channel: vertical trail
    float prevY = texture(gameOfLife, uv).g;
    float flowY = prevY + (prevY - texture(syn_FinalPass, uv).g - 0.1) * 0.1 + 0.02;
    flowY = clamp(flowY, 0.0, 1.0);

    float trail = texture(gameOfLife, uv - vec2(0.0, pixelSize.y)).b - 0.004;
    trail = max(0.0, trail);

    return vec4(next, flowY * decay, trail, 1.0);
}

//============================================================================
// FLOW DISTORTION BUFFER
//============================================================================

vec4 renderFlowBuffer(vec2 uv) {
    vec2 pixelSize = 1.0 / RENDERSIZE;
    float d = 0.02;

    // Sample Game of Life for flow field
    float dx = texture(gameOfLife, uv + vec2(d, 0.0)).g - texture(gameOfLife, uv - vec2(d, 0.0)).g;
    float dy = texture(gameOfLife, uv + vec2(0.0, d)).g - texture(gameOfLife, uv - vec2(0.0, d)).g;

    vec2 flowUV = uv - vec2(dx, dy) * flowAmount * 10.0;

    vec4 flowed = texture(syn_FinalPass, flowUV) * decay;

    // Add some edge enhancement
    float gol = texture(gameOfLife, uv).r;
    flowed.rgb += gol * 0.1;

    return flowed;
}

//============================================================================
// MOEBIUS TRANSFORMATION
//============================================================================

vec2 complexMul(vec2 a, vec2 b) {
    return vec2(a.x * b.x - a.y * b.y, a.x * b.y + a.y * b.x);
}

vec2 complexDiv(vec2 a, vec2 b) {
    float denom = b.x * b.x + b.y * b.y;
    return vec2(a.x * b.x + a.y * b.y, a.y * b.x - a.x * b.y) / denom;
}

vec2 moebiusTransform(vec2 z, float t) {
    float q = getCombinedAudio() * 5.0;

    // Moebius parameters animated by time and audio (matching q11-q18 from original)
    vec2 ac = vec2(sin(t * 0.3) * q, cos(t * 0.4) * q);
    vec2 mu = vec2(cos(t * 0.5) * q, sin(t * 0.6) * q);
    vec2 c = vec2(sin(t * 0.7) * q * 0.1 + 1.0, cos(t * 0.8) * q * 0.1);
    vec2 d = vec2(cos(t * 0.2) * q * 0.5, sin(t * 0.3) * q * 0.5);

    // (c*z + d)
    vec2 czd = complexMul(c, z) + d;

    // mu / (c*z + d) + ac
    vec2 result = complexDiv(mu, czd) + ac;

    return result;
}

vec2 mirrorFold(vec2 uv) {
    // Triangle wave folding - creates kaleidoscopic mirror effect
    return 0.5 + (1.0 - abs(fract(uv * 0.5) * 2.0 - 1.0) - 0.5) * 0.95;
}

//============================================================================
// ORBITING CIRCLES WITH AUDIO-REACTIVE COLORS
//============================================================================

vec3 getCircleColor(int index, float audioQ) {
    // Color animation matching original shape_*_per_frame equations
    float r = 0.5 + 0.5 * sin(audioQ * 0.613 + 1.0 + float(index) * 0.5);
    float g = 0.5 + 0.5 * sin(audioQ * 0.763 + 2.0 + float(index) * 0.7);
    float b = 0.5 + 0.5 * sin(audioQ * 0.771 + 5.0 + float(index) * 0.3);
    return vec3(r, g, b);
}

vec3 getCircleColor2(int index, float audioQ) {
    float r = 0.5 + 0.5 * sin(audioQ * 0.635 + 4.0 + float(index) * 0.4);
    float g = 0.5 + 0.5 * sin(audioQ * 0.616 + 1.0 + float(index) * 0.6);
    float b = 0.5 + 0.5 * sin(audioQ * 0.538 + 3.0 + float(index) * 0.8);
    return vec3(r, g, b);
}

vec4 renderCircles(vec2 uv, float audioQ) {
    vec2 aspect = vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0);
    vec2 uvAspect = (uv - 0.5) * aspect;

    vec4 result = vec4(0.0);

    // Three circles with different Lissajous patterns (matching original shape equations)
    float speeds1[3] = float[3](1.23, 1.104, 1.23);
    float speeds2[3] = float[3](1.43, 1.27, 1.18);
    float wobbleSpeeds[3] = float[3](0.7, 0.7, 0.9);

    for (int i = 0; i < 3; i++) {
        vec2 center = vec2(
            circleOrbitRadius * cos(TIME * speeds1[i]) + 0.03 * cos(TIME * wobbleSpeeds[i]),
            circleOrbitRadius * sin(TIME * speeds2[i]) + 0.03 * sin(TIME * wobbleSpeeds[i])
        );

        float dist = length(uvAspect - center);
        float size = circleSize * (1.0 + audioQ * 0.1);

        // Gradient fill
        float fill = gradientCircle(uvAspect, center, size);

        // Get colors
        vec3 innerColor = getCircleColor(i, audioQ);
        vec3 outerColor = getCircleColor2(i, audioQ);
        vec3 circleColor = mix(outerColor, innerColor, fill);

        // Alpha based on original (0.5 inner, 0.2 outer)
        float alpha = fill * mix(0.2, 0.5, fill);

        // Border for circles 1 and 2
        if (i > 0) {
            float border = smoothstep(size + 0.01, size, dist) * smoothstep(size - 0.01, size, dist);
            result.rgb += vec3(1.0) * border * 0.1;
        }

        // Blend circle
        result.rgb = mix(result.rgb, circleColor, alpha * 0.7);
        result.a = max(result.a, alpha);
    }

    return result;
}

//============================================================================
// FINAL COMPOSITE
//============================================================================

vec4 renderFinal(vec2 uv) {
    float audioQ = getCombinedAudio();
    float t = TIME * moebiusSpeed;

    // Apply Moebius transformation
    vec2 z = (uv - 0.5) * vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0);
    vec2 moebius = moebiusTransform(z, t) * 0.5 * moebiusStrength;
    float l = length(moebius);

    // Mirror/fold the Moebius coordinates
    vec2 moebiusUV = mirrorFold(moebius);

    // Zoomed UV for certain layers (matching original uvr = 0.5 + (uv-0.5)*0.2)
    vec2 uvZoomed = 0.5 + (uv - 0.5) * 0.2;

    // Sample textures
    float golCell = texture(gameOfLife, uvZoomed).r;
    float flowY = texture(flowBuffer, moebiusUV).g;
    float flowY_local = texture(flowBuffer, uvZoomed).g;
    float golY_local = texture(gameOfLife, uvZoomed).g;

    // Build color layers (matching original comp shader)
    // Base: purple from Game of Life cells
    vec3 col = golCell * vec3(0.4, 0.0, 0.7);

    // Cyan tint through Moebius transform
    float cyanAmount = clamp(flowY, 0.0, 1.0);
    col = mix(col, vec3(0.0, 1.0, 1.0), cyanAmount) * (1.4 - pow(l * 0.8, 0.3));

    // Yellow/orange highlights
    float yellowAmount = clamp(flowY_local - golY_local, 0.0, 1.0) * 4.0 * (l * l);
    col = mix(col, vec3(4.0, 1.0, 0.0), yellowAmount * 0.3);

    // Game of Life cells in black
    col = mix(col, vec3(-4.0), golCell * 0.3);

    // White cell overlay
    col = mix(col, vec3(2.0), texture(gameOfLife, uvZoomed).r * 0.75);

    // Animated color subtraction (replacing roam_sin/roam_cos)
    vec3 roamSin = vec3(sin(TIME * 0.7), sin(TIME * 1.1), sin(TIME * 0.9));
    vec3 roamCos = vec3(cos(TIME * 0.8), cos(TIME * 1.2), cos(TIME * 1.0));
    col -= roamSin.zyx * roamCos.yzx * 0.2;

    // Add orbiting circles
    vec4 circles = renderCircles(uv, audioQ);
    col = mix(col, circles.rgb, circles.a * 0.8);

    // Audio flash
    col += syn_BassHits * vec3(0.1, 0.05, 0.15);

    // Clamp and return
    col = clamp(col, 0.0, 1.0);

    return vec4(col, 1.0);
}

//============================================================================
// MAIN
//============================================================================

vec4 renderMain() {
    vec2 uv = _uv;

    if (PASSINDEX == 0) {
        // Pass 0: Game of Life simulation
        return renderGameOfLife(uv);
    }
    else if (PASSINDEX == 1) {
        // Pass 1: Flow distortion buffer
        return renderFlowBuffer(uv);
    }
    else {
        // Final pass: Composite everything
        return renderFinal(uv);
    }
}
