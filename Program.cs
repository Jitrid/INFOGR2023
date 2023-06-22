using System.Diagnostics;
using OpenTK.Mathematics;
using Rasterization.Template;

namespace Rasterization;

class Program
{
    // member variables
    public Surface Screen;                  // background surface for printing etc.
    public Camera Camera;
    Mesh? teapot, floor;                    // meshes to draw using OpenGL
    float a = 0;                            // teapot rotation angle
    readonly Stopwatch timer = new();       // timer for measuring frame duration
    Shader? shader;                         // shader to use for rendering
    Shader? postproc;                       // shader to use for post processing
    Texture? wood;                          // texture to use for rendering
    RenderTarget? target;                   // intermediate render target
    ScreenQuad? quad;                       // screen filling quad for post processing
    readonly bool useRenderTarget = true;   // required for post processing

    // constructor
    public Program(Surface screen)
    {
        Screen = screen;

        Camera = new Camera(0, -14.5f, 0);
    }
    // initialize
    public void Init()
    {
        // load teapot
        teapot = new Mesh("../../../assets/teapot.obj");
        floor = new Mesh("../../../assets/floor.obj");
        // initialize stopwatch
        timer.Reset();
        timer.Start();
        // create shaders
        shader = new Shader("../../../shaders/vs.glsl", "../../../shaders/fs.glsl");
        postproc = new Shader("../../../shaders/vs_post.glsl", "../../../shaders/fs_post.glsl");
        // load a texture
        wood = new Texture("../../../assets/wood.jpg");
        // create the render target
        if (useRenderTarget) target = new RenderTarget(Screen.width, Screen.height);
        quad = new ScreenQuad();
    }

    // tick for background surface
    public void Tick()
    {
        Screen.Clear(0);
    }

    // tick for OpenGL rendering code
    public void RenderGL()
    {
        a = 0; // disable rotation.

        // prepare matrix for vertex shader
        Matrix4 teapotObjectToWorld = Matrix4.CreateScale(0.5f) * Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), a);
        Matrix4 floorObjectToWorld = Matrix4.CreateScale(4.0f) * Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), a);
        Matrix4 worldToCamera = Camera.Load();
        Matrix4 cameraToScreen = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60.0f), (float)Screen.width/Screen.height, .1f, 1000);

        // update rotation
        // a += 0.001f * frameDuration;
        // float doublePi = 2 * MathF.PI;
        // if (a > doublePi) a -= doublePi;

        if (useRenderTarget && target != null && quad != null)
        {
            // enable render target
            target.Bind();
        
            // render scene to render target
            if (shader != null && wood != null)
            {
                teapot?.Render(shader, teapotObjectToWorld * worldToCamera * cameraToScreen, teapotObjectToWorld, wood);
                floor?.Render(shader, floorObjectToWorld * worldToCamera * cameraToScreen, floorObjectToWorld, wood);
            }
        
            // render quad
            target.Unbind();
            if (postproc != null)
                quad.Render(postproc, target.GetTextureID());
        }
        else
        {
            // render scene directly to the screen
            if (shader != null && wood != null)
            {
                teapot?.Render(shader, teapotObjectToWorld * worldToCamera * cameraToScreen, teapotObjectToWorld, wood);
                floor?.Render(shader, floorObjectToWorld * worldToCamera * cameraToScreen, floorObjectToWorld, wood);
            }
        }
    }
}
