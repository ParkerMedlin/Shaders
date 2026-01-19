/*{
    "DESCRIPTION": "Procedural circuit board pattern with scrolling and animated growth/shrinking",
    "CREDIT": "by Claude",
    "ISFVSN": "2.0",
    "CATEGORIES": ["Generator"],
    "INPUTS": [
        {
            "NAME": "complexity",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0.1,
            "MAX": 1.0,
            "LABEL": "Complexity"
        },
        {
            "NAME": "speed",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0.0,
            "MAX": 2.0,
            "LABEL": "Speed"
        },
        {
            "NAME": "scrollSpeed",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -1.0,
            "MAX": 1.0,
            "LABEL": "Scroll Speed"
        },
        {
            "NAME": "growthAmount",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Growth Amount"
        },
        {
            "NAME": "growthPulse",
            "TYPE": "bool",
            "DEFAULT": true,
            "LABEL": "Pulse Growth"
        },
        {
            "NAME": "traceColor",
            "TYPE": "color",
            "DEFAULT": [0.0, 0.8, 0.4, 1.0],
            "LABEL": "Trace Color"
        },
        {
            "NAME": "nodeColor",
            "TYPE": "color",
            "DEFAULT": [0.2, 1.0, 0.6, 1.0],
            "LABEL": "Node Color"
        },
        {
            "NAME": "bgColor",
            "TYPE": "color",
            "DEFAULT": [0.02, 0.05, 0.03, 1.0],
            "LABEL": "Background"
        },
        {
            "NAME": "traceWidth",
            "TYPE": "float",
            "DEFAULT": 0.3,
            "MIN": 0.1,
            "MAX": 0.8,
            "LABEL": "Trace Width"
        },
        {
            "NAME": "glowAmount",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Glow"
        }
    ]
}*/

// Hash functions for procedural generation
float hash11(float p) {
    p = fract(p * 0.1031);
    p *= p + 33.33;
    p *= p + p;
    return fract(p);
}

float hash21(vec2 p) {
    vec3 p3 = fract(vec3(p.xyx) * 0.1031);
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.x + p3.y) * p3.z);
}

vec2 hash22(vec2 p) {
    vec3 p3 = fract(vec3(p.xyx) * vec3(0.1031, 0.1030, 0.0973));
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.xx + p3.yz) * p3.zy);
}

// Distance to line segment
float sdSegment(vec2 p, vec2 a, vec2 b) {
    vec2 pa = p - a;
    vec2 ba = b - a;
    float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
    return length(pa - ba * h);
}

// Get trace direction from cell (0=none, 1=right, 2=up, 3=left, 4=down)
int getTraceDir(vec2 cell, float seed) {
    float h = hash21(cell + seed);
    if (h < 0.15) return 0; // no trace
    if (h < 0.4) return 1;  // right
    if (h < 0.65) return 2; // up
    if (h < 0.8) return 3;  // left
    return 4;               // down
}

// Check if cell has a node
bool hasNode(vec2 cell, float complexity) {
    float h = hash21(cell * 1.7 + 42.0);
    return h < complexity * 0.4;
}

// Get branch count for a node (0-3)
int getBranchCount(vec2 cell, float complexity) {
    float h = hash21(cell * 2.3 + 17.0);
    return int(h * complexity * 4.0);
}

// Calculate growth factor for a cell based on distance from growth centers
float getGrowthFactor(vec2 cell, float growth, float time) {
    // Multiple growth centers that expand over time
    float factor = 0.0;

    for (int i = 0; i < 4; i++) {
        vec2 center = hash22(vec2(float(i) * 7.3, float(i) * 13.7)) * 20.0 - 10.0;
        float dist = length(cell - center);
        float wave = growth * 15.0 - dist + sin(time * 0.5 + float(i)) * 2.0;
        factor = max(factor, smoothstep(0.0, 3.0, wave));
    }

    return factor;
}

void main() {
    vec2 uv = isf_FragNormCoord;

    // Early discard for pixels that will definitely be background
    // (optimization for offscreen values)
    if (uv.x < -0.1 || uv.x > 1.1 || uv.y < -0.1 || uv.y > 1.1) {
        gl_FragColor = bgColor;
        return;
    }

    // Calculate grid scale based on complexity
    float gridScale = mix(6.0, 20.0, complexity);

    // Apply scrolling
    float scroll = TIME * scrollSpeed * speed;
    vec2 scrolledUV = uv + vec2(0.0, scroll);

    // Scale to grid space
    vec2 gridUV = scrolledUV * gridScale;
    vec2 cell = floor(gridUV);
    vec2 cellUV = fract(gridUV);

    // Calculate current growth level
    float growth = growthAmount;
    if (growthPulse) {
        growth = growthAmount * (0.5 + 0.5 * sin(TIME * speed * 0.8));
    }

    // Initialize distances
    float traceDist = 1000.0;
    float nodeDist = 1000.0;
    float branchDist = 1000.0;

    // Check current cell and neighbors for traces
    for (int dx = -2; dx <= 2; dx++) {
        for (int dy = -2; dy <= 2; dy++) {
            vec2 neighborCell = cell + vec2(float(dx), float(dy));
            vec2 neighborCenter = neighborCell + 0.5;

            // Get growth factor for this cell
            float cellGrowth = getGrowthFactor(neighborCell, growth, TIME * speed);

            // Skip cells that haven't "grown" yet
            if (cellGrowth < 0.01) continue;

            // Main trace from this cell
            int dir = getTraceDir(neighborCell, 0.0);

            if (dir > 0 && cellGrowth > 0.2) {
                vec2 traceEnd = neighborCenter;
                float traceLength = cellGrowth; // Trace grows with growth factor

                if (dir == 1) traceEnd += vec2(traceLength, 0.0);
                else if (dir == 2) traceEnd += vec2(0.0, traceLength);
                else if (dir == 3) traceEnd += vec2(-traceLength, 0.0);
                else traceEnd += vec2(0.0, -traceLength);

                float d = sdSegment(gridUV, neighborCenter, traceEnd);
                traceDist = min(traceDist, d);
            }

            // Check for node
            if (hasNode(neighborCell, complexity) && cellGrowth > 0.3) {
                float d = length(gridUV - neighborCenter);
                nodeDist = min(nodeDist, d);

                // Add branches from node
                int branches = getBranchCount(neighborCell, complexity);
                for (int b = 0; b < 4; b++) {
                    if (b >= branches) break;

                    float angle = hash21(neighborCell + vec2(float(b) * 3.7, 0.0)) * 6.283;
                    float branchLen = (0.3 + hash21(neighborCell + vec2(0.0, float(b) * 2.3)) * 0.7) * cellGrowth;
                    vec2 branchEnd = neighborCenter + vec2(cos(angle), sin(angle)) * branchLen;

                    float d = sdSegment(gridUV, neighborCenter, branchEnd);
                    branchDist = min(branchDist, d);
                }
            }

            // Secondary traces (more complex patterns)
            if (complexity > 0.5 && cellGrowth > 0.5) {
                int dir2 = getTraceDir(neighborCell, 100.0);
                if (dir2 > 0 && dir2 != dir) {
                    vec2 start2 = neighborCenter + hash22(neighborCell + 50.0) * 0.4 - 0.2;
                    vec2 end2 = start2;
                    float len2 = 0.5 * cellGrowth;

                    if (dir2 == 1) end2 += vec2(len2, 0.0);
                    else if (dir2 == 2) end2 += vec2(0.0, len2);
                    else if (dir2 == 3) end2 += vec2(-len2, 0.0);
                    else end2 += vec2(0.0, -len2);

                    float d = sdSegment(gridUV, start2, end2);
                    traceDist = min(traceDist, d);
                }
            }
        }
    }

    // Calculate widths
    float baseWidth = traceWidth * 0.1;
    float traceW = baseWidth;
    float nodeW = baseWidth * 2.5;
    float branchW = baseWidth * 0.7;

    // Calculate intensities with anti-aliasing
    float pixelSize = 1.0 / gridScale / min(RENDERSIZE.x, RENDERSIZE.y) * 2.0;

    float traceIntensity = 1.0 - smoothstep(traceW - pixelSize, traceW + pixelSize, traceDist);
    float nodeIntensity = 1.0 - smoothstep(nodeW - pixelSize, nodeW + pixelSize, nodeDist);
    float branchIntensity = 1.0 - smoothstep(branchW - pixelSize, branchW + pixelSize, branchDist);

    // Glow effect
    float glowDist = min(min(traceDist, nodeDist), branchDist);
    float glow = exp(-glowDist * 3.0) * glowAmount * 0.5;

    // Combine colors
    vec3 color = bgColor.rgb;

    // Add glow
    color = mix(color, traceColor.rgb * 0.5, glow);

    // Add traces
    color = mix(color, traceColor.rgb, traceIntensity);
    color = mix(color, traceColor.rgb * 0.8, branchIntensity);

    // Add nodes (brightest)
    color = mix(color, nodeColor.rgb, nodeIntensity);

    // Node center highlight
    float nodeCenterIntensity = 1.0 - smoothstep(nodeW * 0.3, nodeW * 0.5, nodeDist);
    color = mix(color, vec3(1.0), nodeCenterIntensity * 0.5);

    gl_FragColor = vec4(color, 1.0);
}
