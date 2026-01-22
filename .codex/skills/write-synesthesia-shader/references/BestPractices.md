SSF Best Practices
Here are some of the best practices our team has come to over the years of working with Synesthesia and performing with Scenes at live events.

Audio Reactivity
Audio Reactivity is a core piece of Synesthesia's identity. More than anything else, this is what makes an SSF Scene different from an ISF or Shadertoy work. So aside from the basic utility scenes (where audio reactivity may just be annoying), every scene should have some Audio Reactivity. How you add it to the scene is up to you. Here are some ideas to get you started:

//Basic Color Flashing
col = mix(col, col*vec3(2.0,1.5,0.5), syn_HighHits);

//Camera Time for Raymarched Scene
rayOrigin += vec3(0.0, 0.0, syn_BassTime);

//Slow color shifts with the overall song structure
col *= mix(vec3(0.6,1.0,2.0), vec3(1.5,1.0,0.6), syn_BassPresence);

//Random Location each Beat for a Circle
vec2 circlePos = 0.5+0.5*vec2(sin(syn_RandomOnBeat*2*PI), cos(syn_ToggleOnBeat*2*PI));

//Canvas Shake on Bass Hits
uv += vec2(sin(777.77*syn_BassTime), sin(555.55*syn_HighTime))*0.001*syn_BassHits;

//Up/Down displaced scanline
col += col*_pulse(_uv.y + myPattern, syn_ToggleOnBeat, 0.2);

//Color shifting with BPM
col = mix(col, col*vec3(0.5,0.5,2.0), syn_BPMSin4);

//Add some audio-reactive displacement
uv += _rotate(vec2(0.0, 1.0), 2*PI*texture(syn_Spectrum, _uv.y).g)*0.01;
You can use the Audio Reactive Uniforms page for reference to see all our audio uniforms.

Basic color flashing is always an easy place to start, but it's usually good to try to look for deeper or novel ways to add audio reactivity to a Scene. Some questions that can help you:

What is unique about this scene, that I can highlight with audio reactivity?
What parts of the scene seem boring right now, that I can use the audio to add some life to?
Unfortunately, there is some natural tradeoff between hardcoded audio reactivity and allowing a user to perform with the music in the way they want to. So in certain cases too much audio reactivity can be a nuisance. It needs to be carefully balanced. However, usually in these cases you can simply add a toggle to the control panel that turns the audio reactivity on and off. Then you've got the best of both worlds and users can decide whether the audio animation fit the mood of the music or not.

Controls
Perhaps even more important than Audio Reactivity are the controls. Synesthesia is meant to be a Visual Instrument, something you can play and improvise with alongside live music. It can be surprisingly difficult to create a good control panel. Suddenly you've got to consider all kinds of User Experience issues that you wouldn't if you're just creating something on Shadertoy. Here are some of the classic problems and some solutions:

Think about Organization

Use the UI_GROUP tag to put controls into groups based on what they effect, like "color", "audio reactivity", or "media mix".
Try to keep it "clean" looking. Short, descriptive names for controls and their groups are good.
If there are too many controls, maybe you can combine multiple controls into one control, for instance, a slider that starts at 0 but has a different effect if you move it upwards versus downwards.
Control Interactions

Try to avoid controls that don't do anything if another control is in a certain state. This leads to a bad experience when a user moves a slider and doesn't see any visual change at all.
If two controls have a really complex interaction with each other, consider putting them into their own group and mention the interaction in each control's description.
Sometimes the scene will look great over most parameter spaces, but when two controls both get pushed towards the tops of their ranges the scene starts to break. You can fix this by limiting the range of one control based on the value the other, for instance float controlModded = mix(control, control*0.8, control2); Complex interactions like this may seem confusing for users at first, but it's always nice to keep the scene from "breaking" in the context of a high-stakes live performance.
Control Ranges + Curves

Setting the MIN/MAX for a control is one of the most important things in each scene.
Sometimes a slider is great between 0.0 and 1.0, but when set to exactly 0.0 the scene breaks, so in that case set MIN to 0.001.
Maybe a certain slider goes from 1 to 100, and looks great at both 100 and in the lower part of the slider, between 1 and 10. However, the middle area between 10 and 90 is all pretty boring. This calls for a curve! Just change the Max from 100 to 10 and then in the GLSL do myControlMod = pow(myControl, 2);. Now you've got the same range but a user controlling the slider on a MIDI device will have more room to move around in the "interesting zones" of a certain control.
Control Variants

Almost all controls have a "smooth" variant that can be incredibly useful. When in doubt, try to make your control types "slider smooth" or "xy smooth", which allows for buttery smooth animation. When working with smooth-type controls, it's very important to set the "PARAMS" value to get the right speed of animation for whatever element you've got. A good approach is to start with "PARAMS": 0.05 and go up to 0.2 if you want it faster, or down to 0.005 if you want it slower.
"bang smooth" is an excellent control type! It's always good to add a few "bang" type actions to a scene because this gives a user an easy place to start when VJing a scene live, and the smooth allows an animation to occupy more than a single frame -- pretty important in all but the glitchiest music.
Starting Defaults

At the very end, think about how the scene starts when a user clicks on it for the first time. The "DEFAULT" values should all be carefully set. Ideally you want the scene to pull someone in right off the bat, but still leave plenty of room to build and grow over the course of a song.
Miscellaneous Tips
Use > 0.5 rather than == 1.0 because sometimes a Uniform (or any float value) will be slightly off of it's true "1.0" because it gets lost in conversion from CPU to GPU, so it'll be something like "0.9999937". > is a safer test.
Best way to learn is experimenting with existing scenes:

Change the values of numbers in the code.
Change multiplication to addition or subtraction to division.
Delete random pieces of the code and see if that breaks anything.
If deleting something didnt break anything, what did it change?