SSF Standard Uniforms
When a scene is compiled, Synesthesia adds a host of uniform values that make scene writing easier and more powerful.

Basic Uniforms
Versions of these uniforms are commonly included in all GLSL environments.

Uniform	Description
vec2 RENDERSIZE	The width and height in pixels of the current pass (learn more about passes)
float TIME	The time in seconds since the scene started rendering.
float FRAMECOUNT	The number of frames since the scene started rendering.
vec2 _xy	The current position of the pixel. Equivalent to gl_FragCoord.xy.
vec2 _uv	The current normalized position of the pixel, with components in the range 0.0 to 1.0. Equivalent to gl_FragCoord.xy / RENDERSIZE.
vec2 _uvc	The current normalized position of the pixel with aspect ratio correction, such that (0,0) will be in the center of the screen. A circle drawn with these coordinates will not appear stretched.
const float PI	3.14159...
Mouse Uniforms
These uniforms track mouse movement and action within the preview window. This allows more intuitive and complex interaction with scenes like camera control, drawing, or color picking.

Uniform	Description
vec4 _mouse	Emulates the iMouse uniform available in Shadertoy. It stores x, y, x of previous click, and y of previous click. The previous x and y values are negative if the mouse is not down. These positions are not normalized — they are integer pixel values. Learn more about iMouse here
vec3 _click	The state of mouse buttons in the order: left, right, middle. Each component will be 1 when that button is pressed and 0 when it is not
vec2 _muv	The normalized coordinates of the mouse within the preview window (0-1)
vec2 _muvc	The mouse's normalized coordinates with aspect ratio correction, so they can be used to draw circles (like _uvc)
Multipass Uniforms
Uniform	Description
int PASSINDEX	The index of the current pass. If your shader is a one-pass shader (the default), this will always be zero. It starts at 0 and increments by one for each pass of the shader.
sampler2D syn_FinalPass	The previous frame rendered by the shader. Useful for creating simple visual feedback effects.
Media Uniforms
Uniform	Description
sampler2D syn_Media	The texture of the current media selected in the media section of the control panel. NOTE: to load this texture into your scene, you should generally use the loadMedia() function or textureMedia() function
float syn_MediaType	Indicates the type of the current media: None = 0, Image = 1, Video = 2, Webcam = 3. You can check if media is loaded with syn_MediaType > 0.5 or the _isMediaActive() function.