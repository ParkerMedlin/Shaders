Automatically Generated Variables In ISF
When writing shaders in the ISF Specification, the following variables are automatically created and available to use in your compositions.

#Automatically Declared Variables
#PASSINDEX
The uniform int PASSINDEX is automatically declared, and set to 0 on the first rendering pass. Subsequent passes (defined by the dicts in your PASSES array) increment this int.
#RENDERSIZE
The uniform vec2 RENDERSIZE is automatically declared, and is set to the rendering size (in pixels) of the current rendering pass.
#isf_FragNormCoord
The uniform vec2 isf_FragNormCoord is automatically declared. This is a convenience variable, and repesents the normalized coordinates of the current fragment ([0,0] is the bottom-left, [1,1] is the top-right).
#gl_FragCoord
As part of standard GLSL vec4 gl_FragCoord is automatically declared. This holds the values of the fragment coordinate vector are given in the window coordinate system. In 2D space the .xy from this can be used to get the non-normalized pixel location.
#TIME
The uniform float TIME is automatically declared, and is set to the current rendering time (in seconds) of the shader. This variable is updated once per rendered frame- if a frame requires multiple rendering passes, the variable is only updated once for all the passes.
#TIMEDELTA
The uniform float TIMEDELTA is automatically declared, and is set to the time (in seconds) that have elapsed since the last frame was rendered. This value will be 0.0 when rendering the first frame.
#DATE
The uniform vec4 DATE is automatically declared, and is used to pass the date and time to the shader. The first element of the vector is the year, the second element is the month, the third element is the day, and the fourth element is the time (in seconds) within the day.
#FRAMEINDEX
The uniform int FRAMEINDEX is automatically declared, and is used to pass the index of the frame being rendered to the shader- this value is 0 when the first frame is rendered, and is incremented after each frame has finished rendering.

Built-In GLSL / ISF Functions
The base language of GLSL includes many useful functions. If you are writing shaders against the ISF specification there are a few additional functions that can be used for working with image data.

#ISF Exclusive Functions
ISF extends GLSL with the following functions.

vec4 pixelColor = IMG_PIXEL(image imageName, vec2 pixelCoord); 
vec4 pixelColor = IMG_NORM_PIXEL(image imageName, vec2 normalizedPixelCoord);
vec4 pixelColor = IMG_THIS_PIXEL(image imageName); 
vec4 pixelColor = IMG_NORM_THIS_PIXEL(image imageName); 
vec2 imageSize = IMG_SIZE(image imageName);
#IMG_PIXEL() and IMG_NORM_PIXEL()
IMG_PIXEL() and IMG_NORM_PIXEL() fetch the color of a pixel in an image using either pixel-based coords or normalized coords, respectively, and should be used instead of texture2D() or texture2DRect(). In both functions, "imageName" refers to the variable name of the image you want to work with.
#IMG_THIS_PIXEL() and IMG_NORM_THIS_PIXEL()
IMG_THIS_PIXEL() is essentially the same as IMG_PIXEL() but automatically fills in the pixel coordinate for the pixel being rendered.
IMG_NORM_THIS_PIXEL() is essentially the same as IMG_THIS_PIXEL() but automatically fills in the pixel coordinate for the pixel being rendered using a normalized coordinate range.
#IMG_SIZE
IMG_SIZE() returns a two-element vector describing the size of the image in pixels.
#Standard GLSL Functions
This is a reference for many of the commonly used built-in functions from the OpenGL Shading Language, aka GLSL.

#Basic Number Operations
For each of these functions, you can use float input, or as vec2, vec3, or vec4 to perform the operations on multiple values at once. In most cases these functions take a single input parameter (x) and where specified take two parameters (x and y) For example:

float abs(float x)  
vec2 abs(vec2 x)  
vec3 abs(vec3 x)  
vec4 abs(vec4 x)
or

float mod(float x, float y)  
vec2 mod(vec2 x, vec2 y)  
vec3 mod(vec3 x, vec3 y)  
vec4 mod(vec4 x, vec4 y)
pow: The abs function returns the absolute value of x, i.e. x when x is positive or zero and -x for negative x.
sign: The sign function returns 1.0 when x is positive, 0.0 when x is zero and -1.0 when x is negative.
floor: The floor function returns the largest integer number that is smaller or equal to x. The return value is of type floating scalar or float vector although the result of the operation is an integer.
ceil: The ceiling function returns the smallest number that is larger or equal to x. The return value is of type floating scalar or float vector although the result of the operation is an integer.
fract: The fract function returns the fractional part of x, i.e. x minus floor(x).
min: The min function returns the smaller of the two arguments, x and y.
max: The max function returns the larger of the two arguments, x and y.
mod: The mod, short for Modulo, function returns x minus the product of y and floor(x/y).
There are also a variations of the min, max and mod functions where the second parameter is always a floating scalar. For example,

float mod(float x, float y)  
vec2 mod(vec2 x, float y)  
vec3 mod(vec3 x, float y)  
vec4 mod(vec4 x, float y)
#Angle and Trigonometry
For each of these functions, you can use float input, or as vec2, vec3, or vec4 to perform the operations on multiple values at once. For example:

float radians(float degrees)  
vec2 radians(vec2 degrees)  
vec3 radians(vec3 degrees)  
vec4 radians(vec4 degrees)
radians: The radians function converts degrees to radians.
degrees: The degrees function converts radians to degrees.
sin: The sin function returns the sine of an angle in radians. The input parameter can be a floating scalar or a float vector. In case of a float vector the sine is calculated separately for every component.
cos: The cos function returns the cosine of an angle in radians.
tan: The tan function returns the tangent of an angle in radians
asin: The asin function returns the arcsine of an angle in radians. It is the inverse function of sine.
acos: The acos function returns the arccosine of an angle in radians. It is the inverse function of cosine.
atan: The atan function returns the arctangent of an angle in radians. It is the inverse function of tangent.
There is also a two-argument variation of the atan function. For a point with Cartesian coordinates (x, y) the function returns the angle θ of the same point with polar coordinates (r, θ).

float atan(float y, float x)  
vec2 atan(vec2 y, vec2 x)  
vec3 atan(vec3 y, vec3 x)  
vec4 atan(vec4 y, vec4 x)
#Exponential
For each of these functions, you can use float input, or as vec2, vec3, or vec4 to perform the operations on multiple values at once. For example:

float sqrt(float x)  
vec2 sqrt(vec2 x)  
vec3 sqrt(vec3 x)  
vec4 sqrt(vec4 x)
or

float pow(float x, float y)  
vec2 pow(vec2 x, vec2 y)  
vec3 pow(vec3 x, vec3 y)  
vec4 pow(vec4 x, vec4 y)
pow: The power function returns x raised to the power of y.
exp: The exp function returns the constant e raised to the power of x.
log: The log function returns the power to which the constant e has to be raised to produce x, also known as the natural logarithm function.
exp2: The exp2 function returns 2 raised to the power of x. Exponential function (base 2)
log2: The log2 function returns the power to which 2 has to be raised to produce x.
sqrt: The sqrt function returns the square root of x.
inversesqrt: The inversesqrt function returns the inverse square root of x, i.e. the reciprocal of the square root.
#Clamping and Interpolation
GLSL provides several useful functions for clamping and interpolating between values. Many of these functions can be used with a variety of different input parameter arrangements.

clamp: The clamp function returns x if it is larger than minVal and smaller than maxVal. In case x is smaller than minVal, minVal is returned. If x is larger than maxVal, maxVal is returned.

float clamp(float x, float minVal, float maxVal)  
vec2 clamp(vec2 x, vec2 minVal, vec2 maxVal)  
vec3 clamp(vec3 x, vec3 minVal, vec3 maxVal)  
vec4 clamp(vec4 x, vec4 minVal, vec4 maxVal)
There is also a variation of the clamp function where the second and third parameters are always a floating scalars:

float clamp(float x, float minVal, float maxVal)  
vec2 clamp(vec2 x, float minVal, float maxVal)  
vec3 clamp(vec3 x, float minVal, float maxVal)  
vec4 clamp(vec4 x, float minVal, float maxVal)
mix: The mix function returns the linear blend of x and y, i.e. the product of x and (1 - a) plus the product of y and a.

float mix(float x, float y, float a)  
vec2 mix(vec2 x, vec2 y, vec2 a)  
vec3 mix(vec3 x, vec3 y, vec3 a)  
vec4 mix(vec4 x, vec4 y, vec4 a)
There is also a variation of the mix function where the third parameter is always a floating scalar.

float mix(float x, float y, float a)  
vec2 mix(vec2 x, vec2 y, float a)  
vec3 mix(vec3 x, vec3 y, float a)  
vec4 mix(vec4 x, vec4 y, float a)
step: The step function returns 0.0 if x is smaller then edge and otherwise 1.0.

float step(float edge, float x)  
vec2 step(vec2 edge, vec2 x)  
vec3 step(vec3 edge, vec3 x)  
vec4 step(vec4 edge, vec4 x)
There is also a variation of the step function where the edge parameter is always a floating scalar:

float step(float edge, float x)  
vec2 step(float edge, vec2 x)  
vec3 step(float edge, vec3 x)  
vec4 step(float edge, vec4 x)
smoothstep: The smoothstep function returns 0.0 if x is smaller then edge0 and 1.0 if x is larger than edge1. Otherwise the return value is interpolated between 0.0 and 1.0 using Hermite polynomials.

float smoothstep(float edge0, float edge1, float x)  
vec2 smoothstep(vec2 edge0, vec2 edge1, vec2 x)  
vec3 smoothstep(vec3 edge0, vec3 edge1, vec3 x)  
vec4 smoothstep(vec4 edge0, vec4 edge1, vec4 x)
There is also a variation of the smoothstep function where the edge0 and edge1 parameters are always floating scalars:

float smoothstep(float edge0, float edge1, float x)  
vec2 smoothstep(float edge0, float edge1, vec2 x)  
vec3 smoothstep(float edge0, float edge1, vec3 x)  
vec4 smoothstep(float edge0, float edge1, vec4 x)
#Geometry
Unless otherwise specified these functions take the following form where applicable:

float length(float x)  
float length(vec2 x)  
float length(vec3 x)  
float length(vec4 x)
or

float dot(float x, float y)  
float dot(vec2 x, vec2 y)  
float dot(vec3 x, vec3 y)  
float dot(vec4 x, vec4 y)
length: The length function returns the length of a vector defined by the Euclidean norm, i.e. the square root of the sum of the squared components.
distance: The distance function returns the distance between two points. The distance of two points is the length of the vector d = p0 - p1, that starts at p1 and points to p0.
normalize: The normalize function returns a vector with length 1.0 that is parallel to x, i.e. x divided by its length.
dot: The dot function returns the dot product of the two input parameters, i.e. the sum of the component-wise products. If x and y are the same the square root of the dot product is equivalent to the length of the vector.
cross: The cross function returns the cross product of the two input parameters, i.e. a vector that is perpendicular to the plane containing x and y and has a magnitude that is equal to the area of the parallelogram that x and y span. The cross product is equivalent to the product of the length of the vectors times the sinus of the(smaller) angle between x and y. The cross function will only take a pair of vec3 variables as input parameters and will always return a vec3 as a result.
vec3 cross(vec3 x, vec3 y)
faceforward: The faceforward function returns a vector that points in the same direction as a reference vector. The function has three input parameters of the type floating scalar or float vector: N, the vector to orient, I, the incident vector, and Nref, the reference vector. If the dot product of I and Nref is smaller than zero the return value is N. Otherwise -N is returned.
float faceforward(float N, float I, float Nref)  
vec2 faceforward(vec2 N, vec2 I, vec2 Nref)  
vec3 faceforward(vec3 N, vec3 I, vec3 Nref)  
vec4 faceforward(vec4 N, vec4 I, vec4 Nref)
reflect: The reflect function returns a vector that points in the direction of reflection. The function has two input parameters of the type floating scalar or float vector: I, the incident vector, and N, the normal vector of the reflecting surface. Side note: To obtain the desired result the vector N has to be normalized.
float reflect(float I, float N)  
vec2 reflect(vec2 I, vec2 N)  
vec3 reflect(vec3 I, vec3 N)  
vec4 reflect(vec4 I, vec4 N)
refract: The refract function returns a vector that points in the direction of refraction. The function has two input parameters of the type floating scalar or float vector and one input parameter of the type floating scalar: I, the incident vector, N, the normal vector of the refracting surface, and eta, the ratio of indices of refraction. To obtain the desired result the vectors I and N have to be normalized.
float refract(float I, float N, float eta)  
vec2 refract(vec2 I, vec2 N, float eta)  
vec3 refract(vec3 I, vec3 N, float eta)  
vec4 refract(vec4 I, vec4 N, float eta)
#Vector Logic Comparisons
Unless otherwise specified, these functions all work on both floating and integer vector inputs these two forms:

bvec2 lessThan(vec2 x, vec2 y)  
bvec3 lessThan(vec3 x, vec3 y)    
bvec4 lessThan(vec4 x, vec4 y)  
and

bvec2 lessThan(ivec2 x, ivec2 y)  
bvec3 lessThan(ivec3 x, ivec3 y)  
bvec4 lessThan(ivec4 x, ivec4 y)
lessThan: The lessThan function returns a boolean vector as result of a component-wise comparison in the form of x[i] < y[i].
lessThanEqual: The lessThan function returns a boolean vector as result of a component-wise comparison in the form of x[i] <= y[i].
greaterThan: The lessThan function returns a boolean vector as result of a component-wise comparison in the form of x[i] > y[i].
greaterThanEqual: The lessThan function returns a boolean vector as result of a component-wise comparison in the form of x[i] >= y[i].
equal: The lessThan function returns a boolean vector as result of a component-wise comparison in the form of x[i] == y[i].
notEqual: The lessThan function returns a boolean vector as result of a component-wise comparison in the form of x[i] != y[i].
any: The any function returns a boolean value as result of the evaluation whether any component of the input vector is TRUE.
bool any(bvec2 x)  
bool any(bvec3 x)  
bool any(bvec4 x)
all: The any function returns a boolean value as result of the evaluation whether all component of the input vector is TRUE.
bool all(bvec2 x)  
bool all(bvec3 x)  
bool all(bvec4 x)

ISF Multi-Pass and Persistent Buffer Reference
Two extremely powerful concepts that ISF adds on to GLSL are the ability to retain image information between render passes (persistent buffers) and creating compound shaders that have multiple rendering stages (multi-pass shaders) at potentially varying sizes.

#Persistent Buffers
ISF files can define persistent buffers. These buffers are images (GL textures) that stay with the ISF file for as long as it exists. This is useful if you want to "build up" an image over time- you can repeatedly query and update the contents of persistent buffers by rendering into them- or if you want to perform calculations across the entire image, storing the results somewhere for later evaluation. Further details on exactly how to do this are in the full ISF Specification Page.

For each buffer that you wish to retain between passes, the PERSISTENT can be set to true. If you wish to have the value stored as a 32-bit floating point value the additional FLOAT attribute can be included and set to true. Using 32-bit textures will use up more memory, but in some cases can be extremely useful.

/*{
	"DESCRIPTION": "demonstrates the use of a persistent buffer to create a motion-blur type effect. also demonstrates the simplest use of steps: a one-step rendering pass",
	"CREDIT": "by zoidberg",
	"ISFVSN": "2.0",
	"CATEGORIES": [
		"TEST-GLSL FX"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "blurAmount",
			"TYPE": "float"
		}
	],
	"PASSES": [
		{
			"TARGET": "bufferVariableNameA",
			"PERSISTENT": true,
			"FLOAT": true
		}
	]
	
}*/

void main()
{
	vec4		freshPixel = IMG_THIS_PIXEL(inputImage);
	vec4		stalePixel = IMG_THIS_PIXEL(bufferVariableNameA);
	gl_FragColor = mix(freshPixel,stalePixel,blurAmount);
}
#Multi-Pass Shaders
The ISF file format defines the ability to execute a shader multiple times in the process of rendering a frame for output- each time the shader's executed (each pass), the uniform int variable PASSINDEX is incremented. Details on how to accomplish this are described below in the spec, but the basic process involves adding an array of dicts to the PASSES key in your top-level JSON dict. Each dict in the PASSES array describes a different rendering pass- the ISF host will automatically create buffers to render into, and those buffers (and therefore the results of those rendering passes) can be accessed like any other buffer/input image/imported image (you can render to a texture in one pass, and then read that texture back in and render something else in another pass). The dicts in PASSES recognize a number of different keys to specify different properties of the rendering passes- more details are in the spec below.

/*{
	"DESCRIPTION": "demonstrates the use of two-pass rendering- the first pass renders to a persistent buffer which is substantially smaller than the res of the image being drawn.  the second pass renders at the default requested size and scales up the image from the first pass",
	"CREDIT": "by zoidberg",
	"ISFVSN": "2.0",
	"CATEGORIES": [
		"TEST-GLSL FX"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		}
	],
	"PASSES": [
		{
			"TARGET":"bufferVariableNameA",
			"PERSISTENT": true,
			"WIDTH": "$WIDTH/16.0",
			"HEIGHT": "$HEIGHT/16.0"
		},
		{
		
		}
	]
	
}*/

void main()
{
	//	first pass: read the "inputImage"- remember, we're drawing to the persistent buffer "bufferVariableNameA" on the first pass
	if (PASSINDEX == 0)	{
		gl_FragColor = IMG_THIS_NORM_PIXEL(inputImage);
	}
	//	second pass: read from "bufferVariableNameA".  output looks chunky and low-res.
	else if (PASSINDEX == 1)	{
		gl_FragColor = IMG_THIS_NORM_PIXEL(bufferVariableNameA);
	}
}

ISF JSON Reference
The ISF specification requires that each shader include a JSON blob that includes attributes describing the rendering setup, type of shader (generator, FX or transition), input parameters and other meta-data that host applications may want to make use of.

In addition to this reference you may find it useful to download the "Test____.fs" sample filters located here: ISF Test/Tutorial filters These demonstrate the basic set of attributes available and provides examples of each input parameter type. You will probably learn more, faster, from the examples than you'll get by reading this document: each example describes a single aspect of the ISF file format, and they're extremely handy for testing, reference, or as a tutorial.

#Including JSON in an ISF
The first thing in your ISF file needs to be a comment (delineated using "/*" and "*/") containing a JSON dict. If the comment doesn't exist- or the JSON dict is malformed or can't be parsed- your ISF file can't be loaded (ISF files can be tested with the ISF Editor linked to elsewhere on this page). This JSON dict is referred to as your "top-level dict" throughout the rest of this document.

A basic ISF may have a JSON blob that looks something like this:

/*{
	"DESCRIPTION": "demonstrates the use of float-type inputs",
	"CREDIT": "by zoidberg",
	"ISFVSN": "2.0",
	"VSN": "2.0",
	"CATEGORIES": [
		"TEST-GLSL FX"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "level",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		}
	]
}*/

void main()
{
	vec4		srcPixel = IMG_THIS_PIXEL(inputImage);
	float		luma = (srcPixel.r+srcPixel.g+srcPixel.b)/3.0;
	vec4		dstPixel = (luma>level) ? srcPixel : vec4(0,0,0,1);
	gl_FragColor = dstPixel;
}
#ISF Attributes
#ISFVSN
If there's a string in the top-level dict stored at the ISFVSN key, this string will describe the version of the ISF specification this shader was written for. This key should be considered mandatory- if it's missing, the assumption is that the shader was written for version 1.0 of the ISF spec (which didn't specify this key). The string is expected to contain one or more integers separated by dots (eg: '2', or '2.1', or '2.1.1').

#VSN
If there's a string in the top-level dict stored at the VSN key, this string will describe the version of this ISF file. This key is completely optional, and its use is up to the host or editor- the goal is to provide a simple path for tracking changes in ISF files. Like the ISFVSN key, this string is expected to contain one or more integers separated by dots.
#DESCRIPTION
If there's a string in the top-level dict stored at the DESCRIPTION key, this string will be displayed as a description associated with this filter in the host app. the use of this key is optional.
#CATEGORIES
The CATEGORIES key in your top-level dict should store an array of strings. The strings are the category names you want the filter to appear in (assuming the host app displays categories).
#INPUTS
The INPUTS key of your top-level dict should store an array of dictionaries (each dictionary describes a different input- the inputs should appear in the host app in the order they're listed in this array). For each input dictionary:
The value stored with the key NAME must be a string, and it must not contain any whitespaces. This is the name of the input, and will also be the variable name of the input in your shader.
The value stored with the key TYPE must be a string. This string describes the type of the input, and must be one of the following values: "event", "bool", "long", "float", "point2D", "color", "image", "audio", or "audioFFT".
The input types "audio" and "audioFFT" specify that the input will be sent audio data of some sort from an audio source- "audio" expects to receive a raw audio wave, and "audioFFT" expects the results of an FFT performed on the raw audio wave. This audio data is passed to the shader as an image, so "audio"- and "audioFFT"-type inputs should be treated as if they were images within the actual shader. By default, hosts should try to provide this data at a reasonably high precision (32- or 16-bit float GL textures, for example), but if this isn't possible then lower precision is fine.
The images sent to "audio"-type inputs contains one row of image data for each channel of audio data (multiple channels of audio data can be passed in a single image), while each column of the image represents a single sample of the wave, the value of which is centered around 0.5.
The images sent to "audioFFT"-type inputs contains one row of image data for each channel of audio data (multiple channels of audio data can be passed in a single image), while each column of the image represents a single value in the FFT results.
Both "audio"- and "audioFFT"-type inputs allow you to specify the number of samples (the "width" of the images in which the audio data is sent) via the MAX key (more on this later in the discussion of MAX).
Where appropriate, DEFAULT, MIN, MAX, and IDENTITY may be used to further describe value attributes of the input. Note that "image"-type inputs don't have any of these, and that "color"-type inputs use an array of floats to describe min/max/default colors. Everywhere else values are stored as native JSON values where possible (float as float, bool as bool, etc).
"audio"- and "audioFFT"-type inputs support the use of the MAX key- but in this context, MAX specifies the number of samples that the shader wants to receive. This key is optional- if MAX is not defined then the shader will receive audio data with the number of samples that were provided natively. For example, if the MAX of an "audio"-type input is defined as 1, the resulting 1-pixel-wide image is going to accurately convey the "total volume" of the audio wave; if you want a 4-column FFT graph, specify a MAX of 4 on an "audioFFT"-type input, etc.
The value stored with the key LABEL must be a string. This key is optional- the NAME of an input is the variable name, and as such it can't contain any spaces/etc. The LABEL key provides host sofware with the opportunity to display a more human-readable name. This string is purely for display purposes and isn't used for processing at all.
Other notes:
"event" type inputs describe events that do not have an associated value- a momentary click button.
The "long" type input is used to implement pop-up buttons/pop-up menus in the host UI. As such, "long"-type input dictionaries have a few extra keys:
The VALUES key stores an array of integer values. This array may have repeats, and the values correspond to the labels. When you choose an item from the pop-up menu, the corresponding value from this array is sent to your shader.
The LABELS key stores an array of strings. This array may have repeats, and the strings/labels correspond to the array of values.
#PASSES and TARGET
The PASSES key should store an array of dictionaries. Each dictionary describes a different rendering pass. This key is optional: you don't need to include it, and if it's not present your effect will be assumed to be single-pass.
The TARGET string in the pass dict describes the name of the buffer this pass renders to. The ISF host will automatically create a temporary buffer using this name, and you can read the pixels from this temporary buffer back in your shader in a subsequent rendering pass using this name. By default, these temporary buffers are deleted (or returned to a pool) after the ISF file has finished rendering a frame of output- they do not persist from one frame to another. No particular requirements are made for the default texture format- it's assumed that the host will use a common texture format for images of reasonable visual quality.
If the pass dict has a positive value stored at the PERSISTENT key, it indicates that the target buffer will be persistent- that it will be saved across frames, and stay with your effect until its deletion. If you ask the filter to render a frame at a different resolution, persistent buffers are resized to accommodate. Persistent buffers are useful for passing data from one frame to the next- for an image accumulator, or motion blur, for example. This key is optional- if it isn't present (or contains a 0 or false value), the target buffer isn't persistent.
If the pass dict has a positive value stored at the FLOAT key, it indicates that the target buffer created by the host will have 32bit float per channel precision. Float buffers are proportionally slower to work with, but if you need precision- for image accumulators or visual persistence projects, for example- then you should use this key. Float-precision buffers can also be used to store variables or values between passes or between frames- each pixel can store four 32-bit floats, so you can render a low-res pass to a float buffer to store values, and then read them back in subsequent rendering passes. This key is optional- if it isn't present (or contains a 0 or false value), the target buffer will be of normal precision.
If the pass dictionary has a value for the keys WIDTH or HEIGHT (these keys are optional), that value is expected to be a string with an equation describing the width/height of the buffer. This equation may reference variables: the width and height of the image requested from this filter are passed to the equation as $WIDTH and $HEIGHT, and the value of any other inputs declared in INPUTS can also be passed to this equation (for example, the value from the float input "blurAmount" would be represented in an equation as "$blurAmount"). This equation is evaluated once per frame, when you initially pass the filter a frame (it's not evaluated multiple times if the ISF file describes multiple rendering passes to produce a sigle frame). For more information (constants, built-in functions, etc) on math expression evaluations, please see the documentation for the excellent DDMathParser by Dave DeLong, which is what we're presently using.
#IMPORTED images
The IMPORTED key describes buffers that will be created for image files that you want ISF to automatically import. This key is optional: you don't need to include it, and if it's not present your ISF file just won't import any external images. The item stored at this key should be a dictionary.
Each key-value pair in the IMPORTED dictionary describes a single image file to import. The key for each item in the IMPORTED dictionary is the name of the buffer as it will be used in your ISF file, and the value for each item in the IMPORTED dictionary is another dictionary describing the file to be imported.
The dictionary describing the image to import must have a PATH key, and the object stored at that key must be a string. This string should describe the path to the image file, relative to the ISF file being evaluated. For example, a file named "asdf.jpg" in the same folder as the ISF file would have the PATH "asdf.jpg", or "./asdf.jpg" (both describe the same location). If the jpg were located in your ISF file's parent directory, its PATH would be "../asdf.jpg", etc.
#ISF Conventions
Within ISF there are three main usages for compositions: generators, filters and transitions. Though not explicitly an attribute of the JSON blob itself, the usage can be specified by including for specific elements in the INPUTS array. When the ISF is loaded by the host application, instead of the usual matching interface controls, these elements may be connected to special parts of the software rendering pipeline.

#ISF FX: inputImage
ISF shaders that are to be used as image filters are expected to pass the image to be filtered using the "inputImage" variable name. This input needs to be declared like any other image input, and host developers can assume that any ISF shader specifying an "image"-type input named "inputImage" can be operated as an image filter.

#ISF Transitions: startImage, endImage and progress
ISF shaders that are to be used as transitions require three inputs: two image inputs ("startImage" and "endImage"), and a normalized float input ("progress") used to indicate the progress of the transition. Like image filters, all of these inputs need to be declared as you would declare any other input, and any ISF that implements "startImage", "endImage", and "progress" can be assumed to operate as a transition.

#ISF Generators
ISF files that are neither filters nor transitions should be considered to be generators.

Converting Non-ISF GLSL shaders to ISF
In many cases there are only a few minor differences when converting existing GLSL code to the ISF specification. Here are some of the common changes to consider.

For examples on converting shaders, see Chapter 9 - Adapting Existing GLSL Code to the ISF Specification of the ISF Primer.

#texture2D() or texture2DRect()
You should replace any calls in your shader to texture2D() or texture2DRect() with IMG_NORM_PIXEL() or IMG_PIXEL(), respectively.
Images in ISF- inputs, persistent buffers, etc- can be accessed by either IMG_NORM_PIXEL() or IMG_PIXEL(), depending on whether you want to use normalized or non-normalized coordinates to access the colors of the image. If your shader isn't using these- if it's using texture2D() or texture2DRect()- it won't compile if the host application tries to send it a different type of texture.
#Common uniforms: RENDERSIZE and TIME
Many shaders pass in the resolution of the image being rendered (knowing where the fragment being evaluated is located within the output image is frequently useful). By default, ISF automatically declares a uniform vec2 named RENDERSIZE which is passed the dimensions of the image being rendered.
If the shader you're converting requires a time value, note that the uniform float TIME is declared, and passed the duration (in seconds) which the shader's been runing when the shader's rendered.
See the ISF Variables page for more details on automatically declared variables in ISF.
#Uniform Variables and JSON
Variables that you wish to show up as published parameters in host applications can listed in the INPUTS section of the JSON section of your ISF shader. When converting from other shader formats, you may want to consider moving the declared uniform variables to the JSON blob.
See the ISF JSON reference page for more details on declaring INPUTS.
#Alpha Channel
Many shaders don't use (or even acknowledge) the alpha channel of the image being rendered. There's nothing wrong with this- but when the shader's loaded in an application that uses the alpha channel, the output of the shader can look bizarre and unpredictable (though it usually involves something being darker than it should be). If you run into this, try setting gl_FragColor.a to 1.0 at the end of your shader.
#Coordinates: gl_FragCoord vs isf_FragNormCoord
gl_FragCoord.xy contains the coordinates of the fragment being evaluated. isf_FragNormCoord.xy contains the normalized coordinates of the fragment being evaluated.
If your texture doesn't look right, make sure your texture coordinates are ranged properly (textures are typically "clamped" by the host implementation, if you specify an out-of-range texture coordinate it may look funny).
#Vertex Shaders and isf_vertShaderInit
While ISF files are fragment shaders, and the host environment automatically generates a vertex shader, you can use your own vertex shader if you'd like. If you go this route, your vertex shader should have the same base name as your ISF file (just use the extension .vs), and the first thing you do in your vertex shader's main function is call isf_vertShaderInit();.
See Chapter 5 – Vertex Shaders in the ISF Primer for more details.
#IMPORTED image data
If the shader you're converting requires imported graphic resources, note that the ISF format defines the ability to import image files by adding objects to your JSON dict under the IMPORTED key. The imported images are accessed via the usual IMG_PIXEL() or IMG_NORM_PIXEL() methods. See the ISF JSON reference page for more details on including image data.

