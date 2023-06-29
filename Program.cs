using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using Rasterization.Template;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace Rasterization;

class Program
{
    public Surface Screen;
    public Camera Camera;
    public Light Light = new();

    private Skybox cubemap = new Skybox();

    private Mesh? teapot, floor;                    // meshes to draw using OpenGL
    private Shader? shader;                         // shader to use for rendering
    private Shader? postproc;                       // shader to use for post processing
    private Texture? wood;                          // texture to use for rendering
    private Texture? brick, brickNormal;
    private RenderTarget? target;                   // intermediate render target
    private ScreenQuad? quad;                       // screen filling quad for post processing
    private readonly bool useRenderTarget = true;   // required for post processing
    
    public Program(Surface screen) => Screen = screen;

    // initialize
    public void Init()
    {
        Camera = new Camera(10f, 10f, 20f);

        // textures
        wood = new Texture("../../../assets/textures/wood.jpg");
        brick = new Texture("../../../assets/textures/brick.jpg");
        // normal textures
        brickNormal = new Texture("../../../assets/textures/brick_normal.jpg");

        // meshes
        Matrix4 teapotMatrix = Matrix4.CreateTranslation(new Vector3(0, 0.5f, 0));
        teapot = new Mesh("../../../assets/objects/teapot.obj", teapotMatrix, wood);

        Matrix4 floorMatrix = Matrix4.CreateScale(new Vector3(1.5f, 0.1f, 1.5f)) * Matrix4.CreateRotationZ(75.0f);
        floor = new Mesh("../../../assets/objects/floor.obj", floorMatrix, brick, brickNormal);

        // shaders
        shader = new Shader("../../../shaders/scene_vs.glsl", "../../../shaders/scene_fs.glsl");
        postproc = new Shader("../../../shaders/postproc/post_vs.glsl", "../../../shaders/postproc/post_fs.glsl");

        // the render target
        if (useRenderTarget) target = new RenderTarget(Screen.width, Screen.height);
        quad = new ScreenQuad();

        // cubemap.Load();
    }

    // tick for OpenGL rendering code
    public void RenderGL()
    {
        Matrix4 worldToCamera = Camera.Load(); // view
        Matrix4 cameraToScreen = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60.0f), // projection
            (float)Screen.width/Screen.height, .1f, 1000);

        GL.UseProgram(shader!.ProgramID);

        Lightsource[] lights = Light.Lights;
        for (int i = 0; i < lights.Length; i++)
        {
            shader.SetVec3($"lights[{i}].Position", lights[i].Position);
            shader.SetVec3($"lights[{i}].Color", 
                Light.TriggerWarningDoNotTurnOnIfEpilepticWeAreNotLiableInCourt ? Light.GenerateDisco() : lights[i].Color);
        }
        shader.SetInt("lightsCount", lights.Length);

        shader.SetVec3("cameraPosition", Camera.Position);

        if (useRenderTarget && target != null && quad != null)
        {
            // enable render target
            target.Bind();

            // render scene to render target
            if (shader != null && wood != null)
            {
                SceneNode node1 = new(floor);
                SceneNode node2 = new(teapot);
                // node1.AddChild(node2);
                node1.Render(shader, floor.ObjectToWorld, worldToCamera, cameraToScreen);
                node2.Render(shader, teapot.ObjectToWorld, worldToCamera, cameraToScreen);
                // teapot?.Render(shader, worldToCamera, cameraToScreen);
                // floor?.Render(shader, worldToCamera, cameraToScreen);
            }

            // render quad
            target.Unbind();
            if (postproc != null)
                quad.Render(postproc, target.GetTextureID());
        }
        // else
        // {
        //     // render scene directly to the screen
        //     if (shader != null && wood != null)
        //     {
        //         teapot?.Render(shader, worldToCamera, cameraToScreen);
        //         floor?.Render(shader, worldToCamera, cameraToScreen);
        //     }
        // }

        // cubemap.Render(worldToCamera, cameraToScreen);
    }

    public void KeyboardInput(KeyboardKeyEventArgs kea)
    {
        Light.AdjustLights(kea);
        Camera.MovementInput(kea);
    }
}
