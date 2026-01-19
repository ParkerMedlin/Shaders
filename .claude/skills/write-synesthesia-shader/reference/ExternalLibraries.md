External Libraries
Overview
In addition to the built in SSF library, Synesthesia includes some external libraries to help create your scenes. These libraries are not compiled by default -- you need to include the files in your main.glsl file to use them.

So far, there are two external libraries available in Synesthesia: lygia and hg_sdf.

Custom Library
You can create your own GLSL library by adding a folder called glsl in your user data folder. Any .glsl files in this folder can be included in your shader. Here is the location of your user data folder:

Windows: C:\Users\{username}\AppData\Local\Synesthesia\Data
Mac: ~/Library/Application Support/com.gravitycurrent.synesthesia/Data
For example, if you create the file glsl/lib.glsl within this Data folder, you could add #include "lib.glsl" at the top of your shader and use any functions from the file.

NOTE: if you plan to share a scene that uses custom library code, you'll need to copy paste the library code into your shader, since other users won't have these library files.

lygia
Lygia is an open-source library by Patricio Gonzalez Vivo, creator of many incredible shader resources including The Book of Shaders.

The library includes a wide array of functions that touch nearly every aspect of shader programming. It is very granular -- each function has its own file. There are also some files that group multiple functions. For example:

// include a single function
#include "lygia/math/rotate3d.glsl"
#include "lygia/space/displace.glsl"
#include "lygia/color/tonemap.glsl"

#include "lygia/math.glsl"                  // all math functions
#include "lygia/sdf.glsl"                   // all sdf functions
#include "lygia/color/blend.glsl"       // all blend functions
To learn more about the library and all its functions/files, check out the github or website.

Here are links to documentation for each section of the library (copied from the website). NOTE: Synesthesia includes Lygia version 1.1.4.

math/: general math functions and constants: PI, SqrtLength(), etc.
space/: general spatial operations: scale(), rotate(), etc.
color/: general color operations: luma(), saturation(), blend modes, palettes, color space conversion, and tonemaps.
animation/: animation operations: easing
generative/: generative functions: random(), noise(), etc.
sdf/: signed distance field functions.
draw/: drawing functions like digits(), stroke(), fill, etc/.
sample/: sample operations
filter/: typical filter operations: different kind of blurs, mean and median filters.
distort/: distort sampling operations
simulate/: simulate sampling operations
lighting/: different lighting models and functions for foward/deferred/raymarching rendering
geometry/: operation related to geometries: intersections and AABB accelerating structures.
morphological/: morphological filters: dilation, erosion, alpha and poisson fill.
hg_sdf
hg_sdf is an open-source library that provides a powerful toolset for creating raymarched scenes with signed distance functions (sdf). The following documentation is provided by the Mercury team. For more information, check out their website.

NOTE: to use hg_sdf, add this include line to the top of your main.glsl file:

#include "hg_sdf.glsl"
Helper Functions and Macros
sgn()
Sign function that doesn't return 0

float signedValue = sgn(float x);
vec2 signedValue = sgn(vec2 v);
Params

x float | vec2 - the value to be signed
Returns: float - 1 or -1

square()
square the value

float squared = square(float x);
vec2 squared = square(vec2 x);
vec3 squared = square(vec3 x);
vec4 squared = square(vec4 x);
Params

x float | vec2 | vec3 | vec4- the value to be squared
Returns: float - squared value

lengthSqr()
float squared = lengthSqr(vec2 x);
float squared = lengthSqr(vec3 x);
Params

x vec2 | vec3 - a vector to find the square of its length
Returns: float - squared length value

vmax()
Maximum components of a vector

float vectorMax = vmax(vec2 v);
float vectorMax = vmax(vec3 v);
float vectorMax = vmax(vec4 v);
Params

v vec2 | vec3 | vec4 - the vector to be evaluated
Returns: float - the largest component of the vector

vmin()
Minimum components of a vector

float vectorMin = vmin(vec2 v);
float vectorMin = vmin(vec3 v);
float vectorMin = vmin(vec4 v);
Params

v vec2 | vec3 | vec4- the vector to be evaluated
Returns: float - the smallest component of the vector

Primitive Objects
Conventions: Everything that is a distance function is called fSomething. The first argument is always a point in 2 or 3-space called p. Unless otherwise noted, (if the object has an intrinsic "up" side or direction) the y axis is "up" and the object is centered at the origin.

fSphere()
float sphere = fSphere(vec3 p, float r);
Params

p vec3 - a point in space
r float - the radius of the sphere
Returns: float - the distance from the object at the given point

fPlane()
Plane with normal n (n is normalized) at some distance from the origin

float plane = fPlane(vec3 p, vec3 n, float distanceFromOrigin);
Params

p vec3 - a point in space
n vec3 - the plane normal
distanceFromOrigin float - the distance from the center
Returns: float - the distance from the object at the given point

fBoxCheap()
Cheap Box: distance to corners is overestimated

float cheapBox = fBoxCheap(vec3 p, vec3 b);
Params

p vec3 - a point in space
b vec3 - the box dimensions
Returns: float - the distance from the object at the given point

fBox()
Box: correct distance to corners

float box = fBox(vec3 p, vec3 b);
Params

p vec3 - a point in space
b vec3 - the box dimensions
Returns: float - the distance from the object at the given point

fBox2Cheap()
Same as above, but in two dimensions (an endless box)

float box = fBox2Cheap(vec2 p, vec2 b);
Params

p vec2 - a point in space
b vec2 - the box dimensions
Returns: float - the distance from the object at the given point

fBox2()
float box = fBox2(vec2 p, vec2 b);
Params

p vec2 - a point in space
b vec2 - the box dimensions
Returns: float - the distance from the object at the given point

fCorner()
Endless "corner"

float corner = fCorner(vec2 p);
Params

p vec2 - a point in space
Returns: float - the distance from the object at the given point

fBlob()
Blobby ball object. You've probably seen it somewhere. This is not a correct distance bound, beware.

float blob = fBlob(vec3 p);
Params

p vec3 - a point in space
Returns: float - the distance from the object at the given point

fCylinder()
Cylinder standing upright on the xz plane

float cylinder = fCylinder(vec3 p, float r, float height);
Params

p vec3 - a point in space
r float - the radius of the cylinder
height float - the height of the cylinder along the y axis
Returns: float - the distance from the object at the given point

fCapsule()
Capsule: A Cylinder with round caps on both sides

float capsule = fCapsule(vec3 p, float r, float c);
Params

p vec3 - a point in space
r float - the radius of the capsule
c float - the height of the capsule along the y axis
Returns: float - the distance from the object at the given point

fLineSegment()
Distance to line segment between a and b, used for fCapsule() version 2 below

float dist = fLineSegment(vec3 p, vec3 a, vec3 b);
Params

p vec3 - a point in space
a vec3 - line segment A
b vec3 - linge segment B
Returns: float - the distance from the object at the given point

fCapsule()
Capsule version 2: between two end points and with radius r

float capsule = fCapsule(vec3 p, vec3 a, vec3 b, float r);
Params

p vec3 - a point in space
a vec3 - line segment A
b vec3 - linge segment B
r float - the radius of the capsule
Returns: float - the distance from the object at the given point

fCapsule()
Capsule version 2: between two end points and with radius r

float capsule = fCapsule(vec3 p, vec3 a, vec3 b, float r);
Params

p vec3 - a point in space
a vec3 - line segment A
b vec3 - linge segment B
r float - the radius of the capsule
Returns: float - the distance from the object at the given point

fTorus()
Torus in the XZ-plane

float donut = fTorus(vec3 p, float smallRadius, float largeRadius);
Params

p vec3 - a point in space
smallRadius float - the 'inside' radius
largeRadius float - the 'outside' radius
Returns: float - the distance from the object at the given point

fCircle()
A circle line. Can also be used to make a torus by subtracting the smaller radius of the torus.

float circle = fCircle(vec3 p, float r);
Params

p vec3 - a point in space
r float - the radius
Returns: float - the distance from the object at the given point

fCircle()
A circle line. Can also be used to make a torus by subtracting the smaller radius of the torus.

float circle = fCircle(vec3 p, float r);
Params

p vec3 - a point in space
r float - the radius
Returns: float - the distance from the object at the given point

fDisc()
A circular disc with no thickness (i.e. a cylinder with no height). Subtract some value to make a flat disc with rounded edge.

float disc = fDisc(vec3 p, float r);
Params

p vec3 - a point in space
r float - the radius
Returns: float - the distance from the object at the given point

fHexagonCircumcircle()
Hexagonal prism, circumcircle variant

float prism = fHexagonCircumcircle(vec3 p, vec2 h);
Params

p vec3 - a point in space
h vec2
Returns: float - the distance from the object at the given point

fHexagonIncircle()
Hexagonal prism, circumcircle variant

float prism = fHexagonIncircle(vec3 p, vec2 h);
Params

p vec3 - a point in space
h vec2
Returns: float - the distance from the object at the given point

fCone()
Cone with correct distances to tip and base circle. Y is up, 0 is in the middle of the base.

float cone = fCone(vec3 p, float radius, float height);
Params

p vec3 - a point in space
radius float - the radius of the cone at the base
height float - the height of the cone
Returns: float - the distance from the object at the given point

fOctahedron()
float polyhedron = fOctahedron(vec3 p, float r, float e);
Params

p vec3 - a point in space
r float
e float
Returns: float - the distance from the object at the given point

fDodecahedron()
float polyhedron = fDodecahedron(vec3 p, float r, float e);
Params

p vec3 - a point in space
r float
e float
Returns: float - the distance from the object at the given point

fIcosahedron()
float polyhedron = fIcosahedron(vec3 p, float r, float e);
Params

p vec3 - a point in space
r float
e float
Returns: float - the distance from the object at the given point

fTruncatedOctahedron()
float polyhedron = fTruncatedOctahedron(vec3 p, float r, float e);
Params

p vec3 - a point in space
r float
e float
Returns: float - the distance from the object at the given point

fTruncatedIcosahedron()
float polyhedron = fTruncatedIcosahedron(vec3 p, float r, float e);
Params

p vec3 - a point in space
r float
e float
Returns: float - the distance from the object at the given point

fOctahedron()
float polyhedron = fOctahedron(vec3 p, float r);
Params

p vec3 - a point in space
r float
Returns: float - the distance from the object at the given point

fDodecahedron()
float polyhedron = fDodecahedron(vec3 p, float r);
Params

p vec3 - a point in space
r float
Returns: float - the distance from the object at the given point

fIcosahedron()
float polyhedron = fIcosahedron(vec3 p, float r);
Params

p vec3 - a point in space
r float
Returns: float - the distance from the object at the given point

fTruncatedOctahedron()
float polyhedron = fIcosahedron(vec3 p, float r);
Params

p vec3 - a point in space
r float
Returns: float - the distance from the object at the given point

fTruncatedIcosahedron()
float polyhedron = fTruncatedIcosahedron(vec3 p, float r);
Params

p vec3 - a point in space
r float
Returns: float - the distance from the object at the given point

Domain Manipulation Operators
Conventions: Everything that modifies the domain is named pSomething. Many operate only on a subset of the three dimensions. For those, you must choose the dimensions that you want manipulated by supplying e.g. p.x or p.zx inout p is always the first argument and modified in place. Many of the operators partition space into cells. An identifier or cell index is returned, if possible. This return value is intended to be optionally used e.g. as a random seed to change parameters of the distance functions inside the cells. Unless stated otherwise, for cell index 0,

is unchanged and cells are centered on the origin so objects don't have to be moved to fit.

pR()
Rotate around a coordinate axis (i.e. in a plane perpendicular to that axis) by angle a. Read like this: R(p.xz, a) rotates "x towards z". This is fast if a is a compile-time constant and slower (but still practical) if not.

Directly modifies the input vector as an inout parameter

pR(inout vec2 p, float a);
Params

p vec2 - a point in space
r float - the rotation amount
pR45()
Shortcut for 45-degrees rotation

Directly modifies the input vector as an inout parameter

pR45(inout vec2 p);
Params

p vec2 - a point in space
pMod1()
Repeat space along one axis. .

Directly modifies the input vector as an inout parameter

//Use like this to repeat along the x axis:
float cell = pMod1(inout float p, float size); // using the return value is optional
Params

p float - the axis to repeat along
size float - the size of the "cells" or repeated spaces
Returns: float - the current cell

pModMirror1()
Same, but mirror every second cell so they match at the boundaries

Directly modifies the input vector as an inout parameter

float cell = pModMirror1(inout float p, float size); 
Params

p float - the axis to repeat along
size float - the size of the "cells" or repeated spaces
Returns: float - the current cell

pModSingle1()
Repeat the domain only in positive direction. Everything in the negative half-space is unchanged.

Directly modifies the input vector as an inout parameter

float cell = pModSingle1(inout float p, float size); 
Params

p float - the axis to repeat along
size float - the size of the "cells" or repeated spaces
Returns: float - the current cell

pModInterval1()
Repeat only a few times: from indices start to stop (similar to above, but more flexible)

Directly modifies the input vector as an inout parameter

float cell = pModInterval1(inout float p, float size, float start, float stop); 
Params

p float - the axis to repeat along
size float - the size of the "cells" or repeated spaces
start float - which cell to start the repeat
stop float - which cell to stop the repeat
Returns: float - the current cell

pModPolar()
Repeat around the origin by a fixed angle. For easier use, num of repetitions is used to specify the angle.

Directly modifies the input vector as an inout parameter

float cell = pModPolar(inout vec2 p, float repetitions); 
Params

p vec2 - the axis to repeat along
repetitions float - the angle to repeat
Returns: float - the current cell

pMod2()
Repeat in two dimensions

Directly modifies the input vector as an inout parameter

float cell = pMod2(inout vec2 p, vec2 size); 
Params

p vec2 - the axis to repeat along
size float - the size of the "cells" or repeated spaces
Returns: float - the current cell

pModMirror2()
Same, but mirror every second cell so all boundaries match

Directly modifies the input vector as an inout parameter

float cell = pModMirror2(inout vec2 p, vec2 size); 
Params

p vec2 - the axis to repeat along
size float - the size of the "cells" or repeated spaces
Returns: float - the current cell

pModGrid2()
Same, but mirror every second cell at the diagonal as well

Directly modifies the input vector as an inout parameter

float cell = pModMirror2(inout vec2 p, vec2 size); 
Params

p vec2 - the axis to repeat along
size float - the size of the "cells" or repeated spaces
Returns: float - the current cell

pMod3()
Repeat in three dimensions

Directly modifies the input vector as an inout parameter

float cell = pMod3(inout vec2 p, vec3 size); 
Params

p vec3 - the axis to repeat along
size float - the size of the "cells" or repeated spaces
Returns: float - the current cell

pMirror()
Mirror at an axis-aligned plane which is at a specified distance dist from the origin.

Directly modifies the input vector as an inout parameter

float mirror = pMirror(inout vec2 p, float dist); 
Params

p vec2 - the axis to repeat along
dist float - the distance for the mirror
Returns: float - the sign of the mirror

pMirrorOctant()
Mirror in both dimensions and at the diagonal, yielding one eighth of the space.translate by dist before mirroring.

Directly modifies the input vector as an inout parameter

float cell = pMirrorOctant(inout vec2 p, float dist); 
Params

p vec2 - the axis to repeat along
dist float - the distance for the mirror
Returns: float - the sign of the mirror

pReflect()
Reflect space at a plane

Directly modifies the input vector as an inout parameter

float reflection = pReflect(inout vec3 p, vec3 planeNormal, float offset); 
Params

p vec3 - the axis to repeat along
planeNormal vec3 - the normal for the reflection axis
offset float - offset from the plane
Returns: float - the sign of the mirror

Object Combination Operators
We usually need the following boolean operators to combine two objects: Union: OR(a,b) Intersection: AND(a,b) Difference: AND(a,!b) (a and b being the distances to the objects).

The trivial implementations are min(a,b) for union, max(a,b) for intersection and max(a,-b) for difference. To combine objects in more interesting ways to produce rounded edges, chamfers, stairs, etc. instead of plain sharp edges we can use combination operators. It is common to use some kind of "smooth minimum" instead of min(), but we don't like that because it does not preserve Lipschitz continuity in many cases.

Naming convention: since they return a distance, they are called fOpSomething. The different flavours usually implement all the boolean operators above and are called fOpUnionRound, fOpIntersectionRound, etc.

The basic idea: Assume the object surfaces intersect at a right angle. The two distances a and b constitute a new local two-dimensional coordinate system with the actual intersection as the origin. In this coordinate system, we can evaluate any 2D distance function we want in order to shape the edge.

The operators below are just those that we found useful or interesting and should be seen as examples. There are infinitely more possible operators.

They are designed to actually produce correct distances or distance bounds, unlike popular "smooth minimum" operators, on the condition that the gradients of the two SDFs are at right angles. When they are off by more than 30 degrees or so, the Lipschitz condition will no longer hold (i.e. you might get artifacts). The worst case is parallel surfaces that are close to each other.

Most have a float argument r to specify the radius of the feature they represent. This should be much smaller than the object size.

Some of them have checks like "if ((-a < r) && (-b < r))" that restrict their influence (and computation cost) to a certain area. You might want to lift that restriction or enforce it. We have left it as comments in some cases.

usage example:

float fTwoBoxes(vec3 p) {
   float box0 = fBox(p, vec3(1));
   float box1 = fBox(p-vec3(1), vec3(1));
   return fOpUnionChamfer(box0, box1, 0.2);
}
fOpUnionChamfer()
The "Chamfer" flavour makes a 45-degree chamfered edge (the diagonal of a square of size r)

float mergedGeom = fOpUnionChamfer(float a, float b, float r); 
Params

a float - the distance from the first geometry you want to unionize
b float - the distance from the second geometry you want to unionize
r float - the radius of the intersection
Returns: float - the distance of the merged geometry

fOpIntersectionChamfer()
Intersection has to deal with what is normally the inside of the resulting object when using union, which we normally don't care about too much. Thus, intersection implementations sometimes differ from union implementations.

float mergedGeom = fOpIntersectionChamfer(float a, float b, float r); 
Params

a float - the distance from the first geometry you want to unionize
b float - the distance from the second geometry you want to unionize
r float - the radius of the intersection
Returns: float - the distance of the merged geometry

fOpDifferenceChamfer()
Difference can be built from Intersection or Union

float mergedGeom = fOpDifferenceChamfer(float a, float b, float r); 
Params

a float - the distance from the first geometry you want to unionize
b float - the distance from the second geometry you want to unionize
r float - the radius of the intersection
Returns: float - the distance of the merged geometry

fOpUnionRound()
The "Round" variant uses a quarter-circle to join the two objects smoothly:

float mergedGeom = fOpUnionRound(float a, float b, float r); 
Params

a float - the distance from the first geometry you want to unionize
b float - the distance from the second geometry you want to unionize
r float - the radius of the intersection
Returns: float - the distance of the merged geometry

fOpIntersectionRound()
float mergedGeom = fOpIntersectionRound(float a, float b, float r); 
Params

a float - the distance from the first geometry you want to unionize
b float - the distance from the second geometry you want to unionize
r float - the radius of the intersection
Returns: float - the distance of the merged geometry

fOpDifferenceRound()
float mergedGeom = fOpDifferenceRound(float a, float b, float r); 
Params

a float - the distance from the first geometry you want to unionize
b float - the distance from the second geometry you want to unionize
r float - the radius of the intersection
Returns: float - the distance of the merged geometry

fOpUnionColumns()
The "Columns" flavour makes n-1 circular columns at a 45 degree angle

float mergedGeom = fOpDifferenceRound(float a, float b, float r, float n); 
Params

a float - the distance from the first geometry you want to unionize
b float - the distance from the second geometry you want to unionize
r float - the radius of the intersection
n float - the number of columns to draw
Returns: float - the distance of the merged geometry

fOpDifferenceColumns()
float mergedGeom = fOpDifferenceColumns(float a, float b, float r, float n); 
Params

a float - the distance from the first geometry you want to unionize
b float - the distance from the second geometry you want to unionize
r float - the radius of the intersection
n float - the number of columns to draw
Returns: float - the distance of the merged geometry

fOpIntersectionColumns()
float mergedGeom = fOpIntersectionColumns(float a, float b, float r, float n); 
Params

a float - the distance from the first geometry you want to unionize
b float - the distance from the second geometry you want to unionize
r float - the radius of the intersection
n float - the number of columns to draw
Returns: float - the distance of the merged geometry

fOpUnionStairs()
The "Stairs" flavour produces n-1 steps of a staircase

float mergedGeom = fOpUnionStairs(float a, float b, float r, float n); 
Params

a float - the distance from the first geometry you want to unionize
b float - the distance from the second geometry you want to unionize
r float - the radius of the intersection
n float - the number of stairs to draw
Returns: float - the distance of the merged geometry

fOpIntersectionStairs()
float mergedGeom = fOpIntersectionStairs(float a, float b, float r, float n); 
Params

a float - the distance from the first geometry you want to unionize
b float - the distance from the second geometry you want to unionize
r float - the radius of the intersection
n float - the number of stairs to draw
Returns: float - the distance of the merged geometry

fOpDifferenceStairs()
float mergedGeom = fOpDifferenceStairs(float a, float b, float r, float n); 
Params

a float - the distance from the first geometry you want to unionize
b float - the distance from the second geometry you want to unionize
r float - the radius of the intersection
n float - the number of stairs to draw
Returns: float - the distance of the merged geometry

fOpUnionSoft()
Similar to fOpUnionRound, but more lipschitz-y at acute angles (and less so at 90 degrees). Useful when fudging around too much

float mergedGeom = fOpUnionSoft(float a, float b, float r); 
Params

a float - the distance from the first geometry you want to unionize
b float - the distance from the second geometry you want to unionize
r float - the radius of the intersection
Returns: float - the distance of the merged geometry

fOpPipe()
Produces a cylindical pipe that runs along the intersection. No objects remain, only the pipe. This is not a boolean operator.

float mergedGeom = fOpPipe(float a, float b, float r); 
Params

a float - the distance from the first geometry you want to unionize
b float - the distance from the second geometry you want to unionize
r float - the radius of the intersection
Returns: float - the distanced to the object

fOpEngrave()
First object gets a v-shaped engraving where it intersect the second

float mergedGeom = fOpEngrave(float a, float b, float r); 
Params

a float - the distance from the first geometry you want to unionize
b float - the distance from the second geometry you want to unionize
r float - the radius of the intersection
Returns: float - the distance of the merged geometry

fOpGroove()
First object gets a capenter-style groove cut out

float mergedGeom = fOpGroove(float a, float b, float ra, float rb); 
Params

a float - the distance from the first geometry you want to unionize
b float - the distance from the second geometry you want to unionize
ra float - the first radius
rb float - the second radius
Returns: float - the distance of the merged geometry

fOpTongue()
First object gets a capenter-style tongue attached

float mergedGeom = fOpTongue(float a, float b, float ra, float rb); 
Params

a float - the distance from the first geometry you want to unionize
b float - the distance from the second geometry you want to unionize
ra float - the first radius
rb float - the second radius
Returns: float - the distance of the merged geometry