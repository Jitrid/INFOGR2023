
# Ray and Path Tracer

INFO GR 2023 assignment by Robin Schenkels (2602032) and Mathieu Chappin (9669280) at Utrecht University. Includes the first SHA assignment.


## Minimum requirements
We have fully implemented the camera with various ways to move around the scene (see below). We have implemented two primitives: planes (+ checkered plane) and spheres. We have implemented light sources with support for multiple at once. We have fully implemented Phong's shading model with recursive reflection. The "recursion depth" can be capped by adjusting the maximum amount of bounces. The demonstration scene includes all of our work displayed neatly in place.
## Camera Movement
Our program has a fully integrated system to move around the scene with keyboard and mouse input. To move around on the axis, the regular wasd and arrow keys may be used. To elevate the movement and rotate around the scene, press the left-mouse button once. To stop rotating around the scene, press the right-mouse button once. Finally, the FOV may be adjusted by "zooming" with the mouse wheel. The code for these mechanics may be found at the bottom of the "Camera" class.


## Bonus Assignments
The following bonus assignments were implemented:

- A triangle primitive: Find this in triange.cs, visible in the scene.
- Stochastic sampling of glossy reflections: visible in the path tracer, the right mirror orb.
- Anti Aliasing: Find this in Raytracer.cs, multiple samples (rays) per pixel are fired, visible in Pathtracer.
- Multi-Threading: Find this in Raytracer.cs.
- Environment Mapping: Find this in Intersection.cs and Texture.cs.
- Add Stochastic sampling of area lights: find this in our Path tracer.


## Demo

![Logo](https://cdn.discordapp.com/attachments/1115222860064116756/1115377772576125030/image.png) 
- The scene of our Raytracer  
![Logo](https://i.imgur.com/2PaQizy.png)
- The scene of our Path tracer with 800 samples. this takes a long time to render so this is a reference. Expect loading times upwards of half an hour for 800 samples, as there is no performance requirement for this application.
## Structure
Our Application class solely initializes the raytracer, and runs its render method every tick. The Raytracer class initializes the first instances of other classes, such as the camera, and runs the nested for-loop for iterating over all pixels on the screen. It also renders the debug window. The Camera class contains all logic for the camera, including all movement. This is (primarily) based off the slides. The Debug class contains all logic for the 2D debug window. The Primitive class is an abstract class that contains the skeleton of our primitives. The primitives themselves can be found in their respective classes (e.g., spheres in Sphere.cs and the different types of planes in Plane.cs). The Light and Ray classes contain simple fields to store their information. The Intersection class contains all logic for the Phong shading model, different types of rays, etc. Finally, we have implemented several genral-use methods that are used throughout the application, and have stored those in the general Utilities class. There have been some small adjustments in surface.cs and template.cs, but these were primarily to enable other features.
## Acknowledgements
We have used materials in our code, if something was borrowed, credits will be available in a comment along the code.
The skybox was generated using [wwwtyro](https://tools.wwwtyro.net/space-3d/index.html)



