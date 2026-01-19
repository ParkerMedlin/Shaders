JSON Configuration
Overview
Every .synScene directory contains a scene.json file, which configures the scene (learn more about JSON). It provides metadata, specifies the controls that will render in the control panel, and allows added functionality like multiple passes, transitions, and texture loading.

Generally you won't need to manually edit this JSON — you can access all of its functionality through the edit tab of the right panel.

Here is the complete list of JSON keys that can be included:

"TITLE"
"CREDIT"
"DESCRIPTION"
"IMAGE_PATH"
"WIDTH"
"HEIGHT"
"CONTROLS"
"PASSES"
"HARD_TRANSITIONS"
"SMOOTH_TRANSITIONS"
"IMAGES"
All of these keys and their values exist within one JSON object, like so:

{
  "TITLE": "Mind Shredder",
  "CREDIT": "Gravity Current",
  ...
}
Read on to learn more about the usage of each JSON key.

Metadata
The first six keys define the basic metadata of the scene:

Key	Description
"TITLE"	The scene title that displays within Synesthesia.
"CREDIT"	The artist name of the scene creator.
"DESCRIPTION"	A text description of the scene.
"IMAGE_PATH"	The path to the thumbnail image that will be displayed in the scene library. By default, this will be set to 'new_scene.png', but you can add any image and update the path as you like.
"WIDTH" and "HEIGHT"	The final output resolution of the scene in pixels. By default, this will be set to 1920 x 1080. Note that output resolution can also be controlled through the in-app settings, and the two resolutions will be compounded.
Controls
The "CONTROLS" key determines the controls that are rendered to the Synesthesia GUI in the control panel. The current values of these GUI controls are automatically made available in main.glsl as uniforms, allowing you to easily create interactive visuals. The "CONTROLS" value is an array of JSON values, each of which defines an individual control. Here is an overview of the structure:

"CONTROLS" : [{CONTROL_1}, {CONTROL_2}, ...]
Each control object has its own set of keys and values. Here is an example of a control object with a complete set of keys:

{
  "TYPE": "slider smooth",
  "NAME": "slider1",
  "DEFAULT": 0.5,
  "MIN": 0.0,
  "MAX": 1.0,
  "UI_GROUP": "crazy effects",
  "DESCRIPTION": "Use at your own risk",
  "PARAMS": 0.1
}
Control Types
"TYPE" is the most important key in the control object -- it defines the type of control that will be rendered to the GUI, along with the type of uniform that will be available in main.glsl. There are currently 7 main types: slider, knob, toggle, bang, xy, color, and dropdown. Each type has additional specialized versions. For example, you can add "smooth" to any control type to make it smoothly transition between changes in value (see the "PARAMS" key to customize smoothing).

Here is a complete list of available control types:

Type	Description
"slider"	The most common type of control. Outputs a float value that can range from "MIN" to "MAX".
---- "slider smooth"	Same as "slider" but with smooth transitions between changes in value. See "PARAMS" key to customize smoothing.
---- "slider speed"	Outputs a constantly changing value based on the speed set by the slider. The speed is set in units per second — if the control value is 1, its output will increase by 1 per second. If it is -2, it will decrease by 2 per second. This is useful for controlling infinite movement like camera position or rotation.
"knob"	Provides the same functionality as "slider" in a smaller package -- two knobs can be stacked in a column.
---- "knob smooth"	Same as "knob" but with smooth transitions.
---- "knob speed"	Outputs a constantly changing value based on the speed set by the knob.
"toggle"	Toggles between "MIN" and "MAX" values on click. Useful for controlling booleans and settings.
---- "toggle smooth"	Smoothly transitions between "MIN" and "MAX" on click.
"bang"	Outputs "MAX" for one frame on click, otherwise outputs "MIN". Used to trigger events or change settings.
---- "bang smooth"	Triggers a brief envelope on click (smoothly rises to 1.0 then falls to 0.0). See "PARAMS" key to customize the duration of the envelope.
---- "bang counter"	Increments by 1 on click. Useful for cycling through options.
---- "bang counter smooth"	Same as "bang counter" but with smooth transitions.
"xy"	Outputs a vec2 that is controlled by a two dimensional pad with individual sliders for each dimension. Useful for controlling position.
---- "xy smooth"	Same as "xy" but with smooth transitions.
---- "xy speed"	Outputs a constantly changing vec2, based on the speeds set by the values of each dimension.
"color"	Outputs a vec3 color in the form [r, g, b], with values in the range 0.0 to 1.0. It creates a color picker in the control panel with a variety of input options. NOTE: to set the default value for a "color" control, you must use the special key "DEF_COLOR" (see below).
---- "color smooth"	Same as "color" but with smooth transitions between colors.
"dropdown"	Creates a dropdown menu that allows the user to select one of multiple options. NOTE: this control type requires additional keys called "LABELS" and "VALUES" (see below). The "DEFAULT" key specifies the default index value -- 0 will select the first option by default, 1 the second, etc.
---- "dropdown smooth"	Same as "dropdown", but it smoothly interpolates between the values of each option.
Control Options
Here are the rest of the keys that make up a control object:

Type	Description
"NAME"	The identifier of the control, which will automatically become a uniform available in main.glsl. For example, if you create a control with "NAME" : "slider1", you can access the current value of the slider in your shader using the variable slider1. This string will also be displayed as a label beneath the control in the control panel. Use an underscore to add a space in the label -- Synesthesia will automatically replace it (control_name -> control name)
"DEFAULT"	The default value of the control. When a scene is started, the control will be initialized with this value. The value is also stored in the "default" preset, which is always available at the top of the preset tab. For multidimensional controls like "xy" and "color", this should be an array of values defining the defaults for each dimension, like "DEFAULT": [0.0, 0.5, 1.0].
"MIN" and "MAX"	The minimum and maximum values that the control can output. Much of the work of scene creation is finding this range of safe/interesting values for each control. For multidimensional controls like "xy" and "color", these should be arrays of values defining the range for each dimension, like "MIN": [0.0, 0.0], "MAX": [1.0, 2.0].
"UI_GROUP"	The name of the control panel group the control will be placed in. If the group name doesn't already exist in previous controls, Synesthesia will automatically create a new group.
"DESCRIPTION"	A text description of the control and its effects. If the user has the tooltip turned on, the description will be displayed there when a user hovers over the control.
"PARAMS"	For smooth controls, determines the speed of transitions between values. The smaller the value, the longer it will take. It takes roughly 1/PARAMS frames to transition (1.0 takes 1 frame, .1 takes ten frames, etc.). The default value of "PARAMS" is 0.01.
"LABELS" and "VALUES"	These keys define the options for a dropdown control. "LABELS" is an array of strings that appear as the options in the dropdown menu, like "LABELS": ["cold", "warm", "hot"]. "VALUES" is an array of numbers that get passed into the shader for each option, like "VALUES": [0.0, 0.5, 1.0]. These arrays are linked: if the user selects the second label in the menu, the second value will be outputted to the shader ("warm" -> 0.5). You only need one of these keys to define a dropdown control. If you only define the "LABELS" key, the values will automatically be set to the index value (0, 1, 2, etc.). If you only define the "VALUES" key, the labels will automatically be set to "option 1", "option 2", "option 3", etc.
Media
The optional MEDIA key allows you to modify the texture lookup WRAP and FILTER methods used when accessing the user media with syn_Media or the various media loading functions provided by Synesthesia. It should be an array with one object containing the following keys (more than one media object will be supported in future versions of Synesthesia):

Key	Description
"WRAP"	The texture wrapping method. This determines how the texture is sampled when the UV coordinates are outside the range of 0.0 to 1.0. The default value is "repeat" which repeats the image in a grid, equivalent to fract(uv). The other options are "clamp", which will sample an edge pixel if the coordinate is out of the 0.0 to 1.0 range, and "mirror" which reflects out-of-range coordinates.
"FILTER"	The texture filtering method. This determines how the texture is sampled when the UV coordinates are between pixels. The default value is "linear", which uses bilinear interpolation to smooth the texture. The other options are "nearest", which uses nearest neighbor interpolation to preserve the pixelated look of the texture (like texelFetch), and "mipmap", which uses mipmapping to provide level of detail lookups - great for minimizing aliasing when scaling down textures or creating effects that involve blurring.
"MEDIA": [
  {
    "WRAP": "mirror",
    "FILTER": "mipmap"
  }
]
Passes
SSF allows you to easily create shaders with multiple passes. Multipass rendering is useful for creating "feedback," where the results of previous frames are used in the current frame to create iterative, compounding effects (like fractals, fluid simulation, reaction diffusion, etc.). It's also a useful way to structure shader code by separating distinct code into different passes (e.g. rendering an image in pass 1, then blurring the result in pass 2). The majority of built-in Synesthesia scenes involve multi-pass effects — Churning, Fluid Body, and Biopsy are good examples.

To create a multipass shader, add the "PASSES" key to the root JSON object. It should contain an array of individual objects defining each pass. Pass objects contain the following keys:

Key	Description
"TARGET"	The name of the rendering target. This name will automatically be available as a sampler2D uniform in main.glsl, and it can be referenced anywhere in the code. To access the color of the pass target at the current pixel, use texture(target, _uv).
"WIDTH" and "HEIGHT"	The width and height of the target buffer in pixels. These values involve a tradeoff between higher resolution and faster rendering. Especially in shaders with more than a few passes, it can be a good idea to limit the resolution of some passes to optimize performance -- the final output will still come out to be the "WIDTH" and "HEIGHT" defined in the root JSON object.
"FLOAT"	Determines the precision of the pixel values of the pass. If true, the pass will use the RGBA32F 32-bit float format, which is suitable for high precision simulations. If false, the pass will use the lower-precision RGBA8 format, which can improve performance. This is set to true by default.
"WRAP"	The texture wrapping method. This determines how the pass is sampled when the UV coordinates are outside the range of 0.0 to 1.0. The default value is "repeat" which repeats the image in a grid, equivalent to fract(uv). The other options are "clamp", which will sample an edge pixel if the coordinate is out of the 0.0 to 1.0 range, and "mirror" which reflects out-of-range coordinates.
"FILTER"	The texture filtering method. This determines how the pass is sampled when the UV coordinates are between pixels. The default value is "linear", which uses bilinear interpolation to smooth the texture. The other options are "nearest", which uses nearest neighbor interpolation to preserve the pixelated look of the texture (like texelFetch), and "mipmap", which uses mipmapping to provide level of detail lookups - great for minimizing aliasing when scaling down textures or creating effects that involve blurring.
Here's what the overall structure should look like:

"PASSES" : [
  {
    "TARGET": "buffer1",
    "WIDTH": 1920,
    "HEIGHT": 1080,
    "FLOAT": true,
    "WRAP": "mirror",
    "FILTER": "mipmap"
  }, 
  {
    "TARGET": "buffer2",
    "WIDTH": 1280,
    "HEIGHT": 720,
    "FLOAT": true
    // WRAP and FILTER will default to "repeat" and "linear" if excluded
  },
  ...
]
}
Writing Multipass Code
The renderMain() function defined in main.glsl gets called once per pass. To run different code for each pass, use the PASSINDEX uniform, which gets incremented by 1 for each successive pass:

if (PASSINDEX == 0) {
 //...code for first pass...
}
else if (PASSINDEX == 1) {
 //...code for second pass...
}
else if (PASSINDEX == x) {
 //...code for (x+1)th pass...
}
When a scene is started, all passes initialize to vec4(0.0) for every pixel. To create multipass effects, you will likely want to access the results of the first pass within the code for the second pass (and so on), using code like this:

vec4 firstPassColor = texture(firstPassTarget, _uv);
Simple Feedback
You can create simple feedback shaders even without using the "PASSES" key. Every shader's previous frame is automatically made available as a sampler2D uniform called syn_FinalPass. You can access the previous value of the current pixel with texture(syn_FinalPass, _uv), allowing you to create feedback effects as you would with multiple passes.

Transitions
Hard Transitions
Define a variable with X number of preset values. The variable will only ever be set to one of these values. When a significant song change is detected, the variable randomly chooses a new value from the list provided.
Any number of values from 1 to X is allowed.
Every time a song transition is detected, it will select a NEW value. However, you can put in the same value twice to increase the odds of that being chosen.
When a song transition is detected, all "HARD_TRANSITION" variables change at the same time.
"HARD_TRANSITIONS": [
  {
    "UNIFORM": "myVariable",
    "VALUES": [0.9, 0.8, 0.2, 0.0]
  },
  ...
],
Smooth Transitions
Define a list of variables in one "family" that will be codependent.
Within a family, only one variable will be set to 1.0 (or "ON"), the others will be set to 0.0.
When a song transition is detected, a new member of the family will be randomly chosen to be the new variable that is "ON".
A linear interpolation between the old "ON" variable and the new "ON" variable will take place, smoothly transitioning between 1.0 and 0.0.
The duration of the transition is set in the variable "DURATION", which is given in seconds.
A great way to use these is alongside the built-in GLSL mix(thing1, thing2, mixVar) function.
"SMOOTH_TRANSITIONS": [
  {
    "UNIFORMS": ["color1", "color2", "color3"],
    "DURATION": 3.0
  },
  ...
],
And then in your GLSL:

vec3 color = vec3(0.0);
color = mix(color, vec3(1.0,0.5,0.0), color1);
color = mix(color, vec3(0.0,1.0,0.5), color2);
color = mix(color, vec3(0.5,0.0,1.0), color3);
Images
The "IMAGES" key allows you to bundle image files with your scene and easily access them as sampler2D uniforms in the shader. Image files should be added to a directory within the .synScene folder called "images". They must be one of the following formats: .jpg, .jpeg, .png. Synesthesia will load the image at any given resolution.

The "IMAGES" key should contain an array of objects, each of which defines an individual image to import. Each image object should have two keys:

Key	Description
"NAME"	The variable name assigned to the image. This string will automatically become available as a sampler2D uniform in main.glsl. To sample the image at the current pixel, use texture(imageName, _uv);
"PATH"	The path to the image file in relation to scene.json
The overall structure should look something like this:

"IMAGES": [
  {"NAME": "myTexture", "PATH":"images/asphalt.png"},
  {"NAME": "cityScape", "PATH":"images/bwcity.jpg"},
  ...
]