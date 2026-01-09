# ISF Input Types Reference

## JSON Input Structure

Each input in the `INPUTS` array requires at minimum `NAME` and `TYPE`:

```json
{
    "NAME": "variableName",
    "TYPE": "float",
    "DEFAULT": 0.5,
    "MIN": 0.0,
    "MAX": 1.0,
    "LABEL": "Display Name"
}
```

## Input Types

### float
Creates a slider control.

```json
{
    "NAME": "brightness",
    "TYPE": "float",
    "DEFAULT": 1.0,
    "MIN": 0.0,
    "MAX": 2.0,
    "LABEL": "Brightness"
}
```
- GLSL type: `float`
- Optional: `DEFAULT`, `MIN`, `MAX`, `IDENTITY`

### bool
Creates a toggle/checkbox.

```json
{
    "NAME": "invert",
    "TYPE": "bool",
    "DEFAULT": false
}
```
- GLSL type: `bool`
- Values: `true` or `false`

### event
Momentary trigger, true for exactly one frame when triggered.

```json
{
    "NAME": "reset",
    "TYPE": "event"
}
```
- GLSL type: `bool`
- Use for reset buttons, triggers

### long
Creates a dropdown/popup menu.

```json
{
    "NAME": "blendMode",
    "TYPE": "long",
    "DEFAULT": 0,
    "VALUES": [0, 1, 2, 3],
    "LABELS": ["Normal", "Add", "Multiply", "Screen"]
}
```
- GLSL type: `int`
- `VALUES`: integers sent to shader
- `LABELS`: display names in UI

### color
RGBA color picker.

```json
{
    "NAME": "tintColor",
    "TYPE": "color",
    "DEFAULT": [1.0, 0.5, 0.0, 1.0]
}
```
- GLSL type: `vec4` (RGBA, 0.0-1.0)
- Default as array: `[R, G, B, A]`

### point2D
XY coordinate control.

```json
{
    "NAME": "center",
    "TYPE": "point2D",
    "DEFAULT": [0.5, 0.5],
    "MIN": [0.0, 0.0],
    "MAX": [1.0, 1.0]
}
```
- GLSL type: `vec2`
- Useful for effect centers, offsets

### image
Image/video input. Essential for filters.

```json
{
    "NAME": "inputImage",
    "TYPE": "image"
}
```
- Accessed via `IMG_PIXEL`, `IMG_NORM_PIXEL`, `IMG_THIS_PIXEL`
- `inputImage` is the conventional name for the primary input
- For filters to be recognized by hosts, use `inputImage`

## Complete JSON Example

```json
{
    "DESCRIPTION": "Example filter with all input types",
    "CREDIT": "by Author",
    "ISFVSN": "2.0",
    "CATEGORIES": ["Color Effect", "Stylize"],
    "INPUTS": [
        {
            "NAME": "inputImage",
            "TYPE": "image"
        },
        {
            "NAME": "amount",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0.0,
            "MAX": 1.0
        },
        {
            "NAME": "mode",
            "TYPE": "long",
            "DEFAULT": 0,
            "VALUES": [0, 1, 2],
            "LABELS": ["Soft", "Medium", "Hard"]
        },
        {
            "NAME": "tint",
            "TYPE": "color",
            "DEFAULT": [1.0, 1.0, 1.0, 1.0]
        },
        {
            "NAME": "center",
            "TYPE": "point2D",
            "DEFAULT": [0.5, 0.5]
        },
        {
            "NAME": "bypass",
            "TYPE": "bool",
            "DEFAULT": false
        }
    ]
}
```

## Transitions

Transitions require specific inputs:

```json
{
    "INPUTS": [
        {"NAME": "startImage", "TYPE": "image"},
        {"NAME": "endImage", "TYPE": "image"},
        {"NAME": "progress", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0}
    ]
}
```

Hosts automatically recognize shaders with `startImage`, `endImage`, and `progress` as transitions.
