
# Rasterizer

Please find a properly formatted version of our readme at https://github.com/Jitrid/INFOGR2023/ for ease of reading.

INFOGR23 assignment #2 by Robin Schenkels (2602032) and Mathieu Chappin (9669280) at Utrecht University.

## Minimum Requirements

We have implemented all the minimum requirements. This includes a camera class (**Camera.cs**) with a fully integrated system for camera movement and rotation, a scene graph (**SceneNode.cs**) datastructure which has been recursively implemented to support an arbitrary number of child nodes, and a full implementation of Phongs shading model.

## Bonus Assignments
The following bonus assignments have been implemented:

- The program supports an arbitrary amount of light sources (cap set to 64). All logic behind the light sources can be found in **Light.cs**, which includes the array of light sources. The light sources can be modified at run-time, which is explained later in this document.

- Spotlights have been added, and a demonstration set has been added to the scene: moving the camera to the right will show a separate teapot, which demonstrates the spotlight.

- Environment mapping has been implemented to simulate a cube map in the background of the scene. The main logic for this may be found in **Skybox.cs**. This unfortunately took us a huge amount of time, at least six hours, as there were a bunch of problems that not even TAs could assist us with. Therefore, also having spoken to other students who struggled with this, we feel this does not warrant a mere 0.5 extra points, and would like you to consider adjusting this to a full bonus point, considering the complexity.

- Normal mapping has been implemented, which can be seen on the floor mesh in the scene. The logic of this is spread out between a number of files, though most of it can be found in **scene_vs/fs.glsl** and **Mesh.cs**.

- Vignetting and chromatic aberration has been implemented as a toggle.

- Generic colour grading using a colour look-up table (LUT) has been implemented. This, along with the previous bonus requirement, can be found in the **post_vs/fs.glsl** shader files.

### Camera Movement
The camera supports two types of movement: direct movement through keyboard input, and rotation at the current point through mouse input. Movement can trivially be accessed with the regular "WASD" keys, and rotation can be enabled with the left mouse button, and even disabled with the right mouse button. 

### Run-time Light Modification
In order to demonstrate that our light sources can be modified at run-time, we have implemented two additional settings: by pressing Q on the keyboard, all light sources can be switched on or off, and by pressing E, "disco mode" can be enabled. **Warning**: do not turn on disco mode if you're epileptic.

### Post-processing settings
In order to save our eyes from eternal suffering, we have disabled all post-processing settings by default, and associated keybinds to each setting. To enable vignetting and chromatic aberration, press Z. In order to enable generic colour grading using a LUT, press X.
