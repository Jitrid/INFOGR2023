using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Rasterization.Template;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace Rasterization;

class Program
{
    public Surface Screen;
    public Camera Camera;

    private Shader? main;                           // main to use for rendering
    private Shader? postproc;                       // shader to use for post processing

    private Mesh? teapot, floor;                    // meshes to draw using OpenGL
    private Texture? wood;                          // texture to use for rendering
    private Texture? brick, brickNormal;
    private Texture? lut;

    private RenderTarget? target;                   // intermediate render target
    private ScreenQuad? quad;                       // screen filling quad for post processing
    private readonly bool useRenderTarget = true;   // required for post processing

    private int vignette = 1;
    private int aberration = 1;
    private int togglelut = 1;

    public Program(Surface screen)
    {
        Screen = screen;
        Camera = new Camera(10f, 10f, 35f);
    }

    // initialize
    public void Init()
    {
        Skybox.Load();

        // textures
        wood = new Texture("../../../assets/textures/wood.jpg");
        brick = new Texture("../../../assets/textures/brick.jpg");
        // normal textures
        brickNormal = new Texture("../../../assets/textures/brick_normal.jpg");
        // lut
        lut = new Texture("../../../assets/textures/LUT.png");

        // meshes
        Matrix4 floorMatrix = Matrix4.CreateScale(new Vector3(1.5f, 0.5f, 1.5f)) * Matrix4.CreateRotationZ(75.0f);
        floor = new Mesh("../../../assets/objects/floor.obj", floorMatrix, brick, brickNormal);

        Matrix4 teapotMatrix = Matrix4.CreateTranslation(new Vector3(2f, 0.5f, 0));
        teapot = new Mesh("../../../assets/objects/teapot.obj", teapotMatrix, wood);

        // shaders
        main = new Shader("../../../shaders/scene_vs.glsl", "../../../shaders/scene_fs.glsl");
        postproc = new Shader("../../../shaders/postproc/post_vs.glsl", "../../../shaders/postproc/post_fs.glsl");

        // the render target
        if (useRenderTarget) target = new RenderTarget(Screen.width, Screen.height);
        quad = new ScreenQuad();
    }

    /// <summary>
    /// Renders the scene each frame.
    /// </summary>
    public void RenderGL()
    {
        Matrix4 view = Camera.Load(); // view
        Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60.0f), // projection
            (float)Screen.width/Screen.height, .1f, 1000);

        // enable render target
        target!.Bind();

        Skybox.Render(view, projection);
        
        // render scene to render target
        if (main != null)
        {
            GL.UseProgram(main!.ProgramID);
            RenderLights();
            main.SetVec3("cameraPosition", Camera.Position);

            // SceneGraph.Render(main, ...);

            SceneNode node1 = new(floor!);
            SceneNode node2 = new(teapot!);
            // node1.AddChild(node2);
            node1.Render(main, floor!.AffineTransformation, view, projection);
            node2.Render(main, teapot!.AffineTransformation, view, projection);
        }

       
        // render quad
        target.Unbind();
        if (postproc != null)
            quad!.Render(postproc, target.GetTextureID());
        
        //LUT
        GL.ActiveTexture(TextureUnit.Texture4);
        GL.BindTexture(TextureTarget.Texture2D, lut!.ID);
        GL.Uniform1(GL.GetUniformLocation(postproc!.ProgramID, "lut"), 4);

    }

    // Separated code to tidy up the RenderGL method.
    public void RenderLights()
    {
        LightSource[] lights = Light.Lights;
        for (int i = 0; i < lights.Length; i++)
        {
            main!.SetVec3($"lights[{i}].Position", lights[i].Position);
            main.SetVec3($"lights[{i}].Color",
                Light.TriggerWarningDoNotTurnOnIfEpilepticWeAreNotLiableInCourt ? Light.GenerateDisco() : lights[i].Color);
        }
        main!.SetInt("lightsCount", lights.Length);
    }

    /// <summary>
    /// Calls all methods associated with keyboard input.
    /// </summary>
    public void KeyboardInput(KeyboardKeyEventArgs kea)
    {
        Camera.MovementInput(kea);
        Light.AdjustLights(kea);
        SwitchPostProcOptions(kea);
    }

    /// <summary>
    /// Toggle certain post-processing settings.
    /// </summary>
    public void SwitchPostProcOptions(KeyboardKeyEventArgs kea)
    {
        GL.UseProgram(postproc!.ProgramID);

        if (kea.Key == Keys.Z)
        {
            postproc.SetInt("applyChrom", aberration);
            postproc.SetInt("applyVignette", vignette);
            vignette = vignette == 1 ? 0 : 1;
            aberration = aberration == 1 ? 0 : 1;
        }

        if (kea.Key == Keys.X)
        {
            postproc.SetInt("toggleLUT", togglelut);
            togglelut = togglelut == 1 ? 0 : 1;
        }
    }
}
