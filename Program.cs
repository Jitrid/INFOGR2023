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

    private Mesh? teapot, floor;                    // meshes to draw using OpenGL
    private Shader? shader;                         // shader to use for rendering
    private Shader? postproc;                       // shader to use for post processing
    private Texture? wood, bluerock;                // texture to use for rendering
    private RenderTarget? target;                   // intermediate render target
    private ScreenQuad? quad;                       // screen filling quad for post processing
    private readonly bool useRenderTarget = true;   // required for post processing
    
    public Program(Surface screen) => Screen = screen;

    // initialize
    public void Init()
    {
        Camera = new Camera(10f, 10f, 20f);

        // textures
        wood = new Texture("../../../assets/wood.jpg");
        bluerock = new Texture("../../../assets/bluerock.jpg");

        // meshes
        Matrix4 teapotMatrix = Matrix4.CreateTranslation(new Vector3(0, 0.5f, 0));
        teapot = new Mesh("../../../assets/teapot.obj", teapotMatrix, wood);

        Matrix4 floorMatrix = Matrix4.CreateScale(new Vector3(5f, 0.1f, 5f));
        floor = new Mesh("../../../assets/floor.obj", floorMatrix, bluerock);

        // shaders
        shader = new Shader("../../../shaders/vs.glsl", "../../../shaders/fs.glsl");
        postproc = new Shader("../../../shaders/postproc/vs_post.glsl", "../../../shaders/postproc/fs_post.glsl");

        // the render target
        if (useRenderTarget) target = new RenderTarget(Screen.width, Screen.height);
        quad = new ScreenQuad();
    }

    // tick for background surface
    public void Tick() {}

    // tick for OpenGL rendering code
    public void RenderGL()
    {
        Matrix4 worldToCamera = Camera.Load(); // camera
        Matrix4 cameraToScreen = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60.0f), // viewport
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
                teapot?.Render(shader, worldToCamera, cameraToScreen);
                floor?.Render(shader, worldToCamera, cameraToScreen);
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
                teapot?.Render(shader, worldToCamera, cameraToScreen);
                floor?.Render(shader, worldToCamera, cameraToScreen);
            }
        }
    }

    public void KeyboardInput(KeyboardKeyEventArgs kea)
    {
        Light.AdjustLights(kea);
        Camera.MovementInput(kea);
    }
}
