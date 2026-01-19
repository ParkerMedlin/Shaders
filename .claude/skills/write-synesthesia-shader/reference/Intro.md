Synesthesia Shader Format (SSF)
Overview
Synesthesia scenes are created using the Synesthesia Shader Format (SSF). The core of every scene is a fragment shader (also called pixel shader) written in GLSL, which renders the visuals on every frame. SSF provides a backend and file structure around this shader, adding extra functionality like audio reactivity, custom GUI controls, multiple passes, and integration with the Synesthesia app.

Scenes exist within a directory with a .synScene ending. When a new scene is created in the Library panel, the directory is initialized with three files:

main.glsl — GLSL fragment shader
scene.json — JSON configuration
new_scene.png — Thumbnail image
Additionally, there are two optional components that you can include:

script.js — JavaScript script
images/any_name.jpg — Image directory
Here's what a complete file structure should look like:

sceneName.synScene
└── main.glsl
└── scene.json
└── script.js
└── thumbnail.png
└── images
    └── image1.jpg
    └── image2.png
Read on to learn about the function of each part of the file structure.

GLSL Fragment Shader
The code written in main.glsl generates all the imagery you see in Synesthesia. It is a fragment shader — a program that gets executed directly by a computer's GPU, rendering each pixel simultaneously based on its position in the frame. This parallel technique is extremely powerful, allowing people to create incredibly complex visuals while maintaining high framerates. To learn more about fragment shaders, check out the GLSL resources page — The Book of Shaders is a great place to start.

Beyond standard GLSL, Synesthesia adds a host of built-in uniforms and functions, which are automatically available in every shader. Synesthesia's core feature is custom audio uniforms — it automatically analyzes music in real-time and provides uniforms with audio data for each frame.

JSON configuration
The scene.json file contains a JSON object that configures the scene. It provides metadata like scene and artist names, specifies the controls that will render in the control panel, and allows added functionality like multiple passes, transitions, and texture loading. Check out the JSON Configuration page to learn more about all the options.

JavaScript script
Fragment shaders are very powerful, but they have a few serious limitations. There is no memory of the previous frame, and you can't calculate anything on a "scene-wide" basis — every calulcation is limited to an individual pixel.

To overcome some of these limitations, Synesthesia allows you to create an additional JavaScript scripting file called script.js. This script will be run on every frame before the shader, calculating variables on a frame-wide basis and passing them into the shader as uniforms. Check out the JavaScript Scripting page to learn more.

Thumbnail image
The thumbnail image is displayed in the Library panel, allowing you to preview the scene you're about to load. The default thumbnail is generated with the name new_scene.png, but you can upload any image and change the name as you like — just make sure to update the path set with the "IMAGE_PATH" key in scene.json.

Image directory
By creating an images directory in your scene folder and adding images, you can bundle images with your scene and easily access them in your shader as textures. To properly import the images, you must configure them with the "IMAGES" key in scene.json. Learn more on the JSON Configuration page.