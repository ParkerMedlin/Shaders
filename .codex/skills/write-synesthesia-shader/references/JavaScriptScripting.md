JavaScript Scripting
Overview
In addition to a scene's GLSL and JSON files, you can add an optional JavaScript scripting file. This script will be run on every frame before main.glsl, calculating variables on a frame-wide basis and passing them into the shader as uniforms.

Unlike GLSL, objects and data declared in the JavaScript persist across multiple frames, enabling useful functionality like timers and Object Oriented Programming. The JavaScript engine is also equipped with a powerful event system and built-in functions to update controls, allowing complex control over your scene.

Check out this video tutorial to see JavaScript in action:

v1.19 JavaScript Updates Walkthrough
Update and Setup Functions
There are two main functions involved in a script.js file: the setup() function and the update(dt) function. They are both optional, but you must define at least one for anything to happen in the script.

The setup() function which will be run once when the scene loads. This is useful for initializing variables and registering event handlers. By registering event handlers in the setup function, you can completely forgo the update function.

The update(dt) function which will be run on every frame. The argument dt stores the time elapsed since the previous frame, which is useful for a variety of purposes (physics simulations, timers, consistent speeds, etc.).

Getting/Setting Uniforms
One of the primary functions of script.js is to take the current scene uniforms as input and create new uniforms as output.

Accessing Input Uniforms
All of the scene's uniforms are available as global variables in the script, so you can access them like you would in the GLSL. This includes built-in standard GLSL uniforms, audio reactive uniforms, and any uniforms created by the controls defined in scene.json. It does not include uniforms that apply to individual pixels and passes, like _uvc, PASSINDEX, and RENDERSIZE. Vector uniforms are available as objects with xyzw properties.

Here are some examples of these global variables:

TIME (standard uniform)
syn_BassLevel (audio reactive uniform)
mySlider (control uniform)
myXy.y (access the components of a vector uniform)
TIP: You can print out all the available global variables and functions to Synesthesia's console using the utility function printGlobalKeys()

Setting Output Uniforms
To set a uniform for the shader, use the setUniform() function. There are three ways to use this function: you can pass in up to four float values, pass in an array containing up to four values, or pass in an object containing xyzw or rgba properties.

setUniform(uniformName, x, y, z, w);
setUniform(uniformName, [x, y, z, w]);
setUniform(uniformName, { x, y, z, w });
setUniform(uniformName, { r, g, b, a });
This function will automatically create a uniform available in your shader called uniformName, determining the uniform type based on the amount of data you pass in.

Examples

// set a float uniform
setUniform("size", 0.5);
// set a vec2 uniform
setUniform("position", 0.5, 0.3);
setUniform("position", [0.5, 0.3]);
setUniform("position", { x: 0.5, y: 0.3 });
// set a vec3 uniform
setUniform("color", 0.1, 0.0, 1.0);
setUniform("color", [0.1, 0.0, 1.0]);
setUniform("color", { r: 0.1, g: 0.0, b: 1.0 });
// set a vec4 uniform
setUniform("colorWithAlpha", 0.1, 0.0, 1.0, 1.0);
setUniform("colorWithAlpha", [0.1, 0.0, 1.0, 1.0]);
setUniform("colorWithAlpha", { r: 0.1, g: 0.0, b: 1.0, a: 1.0 });
It is good practice to initialize all the output uniforms you'd like to use within the setup() function to ensure that they will always be defined in the shader.

Modifying Uniforms
The most common usage of script.js is to modify Synesthesia's built-in uniforms — especially the audio reactive variety. To achieve a desired effect, sometimes these uniforms need to be larger, faster, squared, combined, inverted, etc.

For example, let's say you want to modify syn_BassLevel to be more reactive, so you square it. You want its effects to be more subtle, so you scale it by 0.5. And you want it to turn on/off based on a toggle control you've created called "move_with_bass". Here's what your code might look like:

function update(dt) {
  var modified_BassLevel = 0.5 * Math.pow(syn_BassLevel, 2) * move_with_bass;
  setUniform("modified_BassLevel", modified_BassLevel);
}
For more examples of scenes that modify audio uniforms, check out kaleidoWHOA, MAN, Meta Experiment 3, Circles5, or Hills, Eels.

DEPRECATED: inputs and uniforms
If you look at the script.js code for some scenes in Synesthesia, you'll notice that uniforms are accessed and set using two global objects called inputs and uniforms. You can access all input uniforms as properties of inputs, and you can set uniforms by adding a property to the uniforms object.

These global objects are still supported and available in the JavaScript, but they have been deprecated in favor of global input variables and the setUniform function.

Updating Controls
Using the following built-in JavaScript functions, you can update the controls of a scene.

setControl()
Set the value of a control. You can include up to three values based on the dimension of the target control (include three to change colors, two to change xys, and one for anything else).

setControl(controlName, [values], { options })
Params

controlName string - the name of the control (insensitive to case and spacing characters). To target a meta control, you can add "meta/" in front of the name, like "meta/brightness" (or use the "bank" option discussed below)
value float array - the new value of the control. Can be either a single float or an array of floats depending on the control type. These should be raw, scaled values — the same as the value the control will have within the shader. To set a control with normalized values, use the setControlNormalized() function
options object - an object that can customize the behavior of the control update, using the following keys:
dimension int - choose a single dimension of a multidimensional control to update
bank string - select which bank (scene or meta) to target in cases of overlap
Examples

// pass in up to three values within an array
setControl("mySlider", 5.0);
setControl("myXyPad", [-1.0, 2.0]);
setControl("myColor", [0.1, 0.5, 0.0]);

// target a meta control using "meta/" or the "bank" option
setControl("meta/brightness", -0.5);
setControl("brightness", -0.5, { bank: "meta" });

// target specific dimensions using the "dimension" option
setControl("myXyPad", -1.0, { dimension: 0 });
setControl("myColor", 1.0, { dimension: 2 });
setControlNormalized()
Set a control using values normalized between 0 and 1. This is helpful to update controls regardless of their range.

setControlNormalized(controlName, [values], { options })
Params

controlName string - the name of the control (insensitive to case and spacing characters). To target a meta control, you can add "meta/" in front of the name, like "meta/brightness" (or use the "bank" option discussed below)
values float array - the new values of the control. Can be either a single float or an array of floats depending on the control type. These values should be normalized between 0 and 1, and they will be scaled to the range of the control. To set a control with raw values, use the setControl() function
options object - an object that can customize the behavior of the control update, using the following keys:
dimension int - choose a single dimension of a multidimensional control to update
bank string - select which bank (scene or meta) to target in cases of overlap
Examples

// pass in up to three values within an array
setControlNormalized("mySlider", 1.0);
setControlNormalized("myXyPad", [0.0, 1.0]);
setControlNormalized("myColor", [0.1, 0.5, 0.0]);

// target a meta control using "meta/" or the "bank" option
setControlNormalized("meta/brightness", 0.5);
setControlNormalized("brightness", 0.5, { bank: "meta" });

// target specific dimensions using the "dimension" option
setControlNormalized("myXyPad", 0.0, { dimension: 0 });
setControlNormalized("myColor", 1.0, { dimension: 2 });
randomizeControl()
Randomize the value of a control.

randomizeControl(controlName)
Params

controlName string - the name of the control (insensitive to case and spacing characters). To target a meta control, you can add "meta/" in front of the name, like "meta/brightness"
defaultControl()
Set a control to its default value.

defaultControl(controlName)
Params

controlName string - the name of the control (insensitive to case and spacing characters). To target a meta control, you can add "meta/" in front of the name, like "meta/brightness"
randomizeGroup()
Randomize a group of controls.

randomizeGroup(groupName)
Params

groupName string - the name of the control group to randomize (insensitive to case and spacing characters). To target a group of meta controls, you can add "meta/" in front of the name, like "meta/color"
defaultGroup()
Set a control to its default value.

defaultGroup(groupName)
Params

groupName string - the name of the control group to randomize (insensitive to case and spacing characters). To target a group of meta controls, you can add "meta/" in front of the name, like "meta/color"
Event System
Synesthesia's JavaScript engine includes an event system — by defining event handlers, you can run code conditionally based on the state of the script's input variables.

Registering Event Handlers
There are five built-in functions you can use to register different types of event handlers. These functions should be called within the setup() function, since they only need to be registered once. Event handler registration functions all have the signature:

eventType(target, callback)
Params

target string — the name of the input variable you'd like to track. You can use any of the script's global input uniforms (the name of a slider, audio uniform, standard uniform, etc.). This argument is insensitive to case and spacing characters like "_" and "-", so "my_slider", "MySlider", and "m-Y-s-L-i-D-e-R" would all work to track your slider
callback string — the name of the JavaScript callback function you'd like to call whenever the event occurs (must be an exact match)
Whenever an event occurs and an event handler callback function is called, two arguments will be passed in:

value — the current value of the target
previousValue — the value of the target in the previous frame
Here's a complete list of the event handler registration functions:

Function	Effect
onChange(target, callback)	callback will be called whenever target changes value
onOffToOn(target, callback)	callback will be called whenever target changes from less than 0.5 to greater than 0.5
onOnToOff(target, callback)	callback will be called whenever target changes from greater than 0.5 to less than 0.5
whileOn(target, callback)	callback will be called on every frame that target is greater than 0.5
whileOff(target, callback)	callback will be called on every frame that target is less than 0.5
Examples
Here are some examples of how the event system could be used, incorporating the built-in JavaScript functions to update controls:

// the following functions before setup() are all custom event handlers

function onMacroChange(value, previousValue) {
  setControl("param1", value);
  setControl("param2", value);
  setControl("param3", value);
}

function onPresetChange(value, previousValue) {
  // trigger one of three presets based on the dropdown index
  var dropdownIndex = value;
  if (dropdownIndex === 0) {
    setControl("param1", -1.0);
    setControl("param2", 10.0);
    setControl("param3", -5.1);
  } else if (dropdownIndex === 1) {
    setControl("param1", 2.0);
    setControl("param2", 20.0);
    setControl("param3", 0.0);
  } else if (dropdownIndex === 2) {
    setControl("param1", 0.0);
    setControl("param2", 15.0);
    setControl("param3", 1.0);
  }
}

function onLoudBass(value, previousValue) {
  if (value > 0.9) {
    randomizeControl("meta/hue");
  }
}

function onChangeSeed(value, previousValue) {
  randomSeed = Math.random() * 100;
}

function onRandomizeColors(value, previousValue) {
  randomizeGroup("colors");
}

function onDefaultColors(value, previousValue) {
  defaultGroup("colors");
}

function whileBrightnessLFO(value, previousValue) {
  setControlNormalized("meta/brightness", Math.sin(TIME*2)*0.5 + 0.5);
}

function defaultBrightness(value, previousValue) {
  defaultControl("meta/brightness");
}

function whileDebug(value, previousValue) {
  print("random seed: " + randomSeed);
  print("bass level: " + syn_BassLevel);
}

var randomSeed = 0;

function setup() {
  // create a "macro" slider that sets the value of multiple other controls
  onChange("macro", "onMacroChange");
  // trigger a hard-coded preset based on the value of a dropdown
  onChange("preset_dropdown", "onPresetChange");
  // randomize the "hue" meta control whenever the bass gets loud
  onChange("syn_BassLevel", "onLoudBass");
  // update a JavaScript seed variable whenever a button is pressed
  onOffToOn("change_seed", "onChangeSeed");
  // randomize a group of controls when a button called 'randomize_colors' is pressed
  onOffToOn("randomize_colors", "onRandomizeColors");
  // set those controls back to default when a different button is pressed
  onOffToOn("default_colors", "onDefaultColors");
  // use an LFO to control the "brightness" meta control when a toggle is on
  whileOn("brightness_LFO", "whileBrightnessLFO");
  // return brightness to its default value when the LFO toggle is turned off
  onOnToOff("brightness_LFO", "defaultBrightness");
  // print debug information whenever debug toggle is on
  whileOn("debug", "whileDebug");
}
Object Oriented Programming
Other use cases of script.js involve Object Oriented Programming (OOP), which allows complex functionality that would be otherwise impossible (or at least impractical) with shaders. Generally, this involves creating a custom class, constructing an instance, updating it each frame, and sending its properties into the shader as uniforms. An instance declared above update() will remain available until the scene stops, which allows you to create cohesive behavior over time.

Here are some examples of the custom classes that have been used in Synesthesia's built-in scenes:

camera movement (Molten, Alien Cavern, Deeper)
BPM counter (Alien Cavern, Deeper, Hex Array, etc.)
smooth counter (Hue Review, KIFS Flythrough, Lattix, etc.)
timer (Circles5, Circuit Bending, Voronoi Geode, etc.)
physics simulation (Biopsy)
introducing randomness (Stained Glass, Thresholder)
Reading Pixel Data
textureFinalPass(x,y)
Sample the output color of the final pass at normalized (0 to 1) x, y coordinates and return an array of RGB values.

Params

x float - x coordinate of sample pixel, normalized between 0 and 1
y float - y coordinate of sample pixel, normalized between 0 and 1
Returns: float array - an array of three values for r, g and b, normalized between 0 and 1

Example
Enable color picking from the visualizer by passing in the mouse coordinates and returning the pixel values directly to the setControl function like so:

if (_click.x > 0.5) {
  var pixelColor = textureFinalPass(_muv.x, _muv.y);
  setControl('my_color', pixelColor);
}
Printing to the Console
You can use print() or console.log() to print to Synesthesia's built-in developer console. This can be useful for debugging shaders, since you can display uniform values.

Here's a trick used to only print values on a periodic basis:

var frameCount = 0;
var printPeriod = 50;

function update(dt) {
  if (frameCount % printPeriod == 0){
    print("suh dude");
  }

  frameCount++;
}