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

    /// <summary>
    /// The main shader of the program.
    /// </summary>
    private Shader? main;
    /// <summary>
    /// The post-processing shader of the program.
    /// </summary>
    private Shader? postproc;

    // scene graph
    private SceneGraph? sceneGraph;

    // meshes
    private List<Mesh>? meshes;
    private Mesh? floor;
    private Mesh? teapot, teapotChild;
    private Mesh? spotlightTeapot;
    
    // textures
    private Texture? wood, trippy;
    private Texture? brick, brickNormal; // textures affected by normal mapping have two variants.
    private Texture? lut;

    // post-processing
    private RenderTarget? target;                   // intermediate render target
    private ScreenQuad? quad;                       // screen filling quad for post processing
    private readonly bool useRenderTarget = true;   // required for post-processing

    // Post-processing settings (glsl booleans)
    private int vignette = 1;
    private int aberration = 1;
    private int togglelut = 1;

    public Program(Surface screen)
    {
        Screen = screen;
        Camera = new Camera(10f, 10f, 35f);
    }
    
    public void Init()
    {
        // textures
        wood = new Texture("../../../assets/textures/wood.jpg");
        trippy = new Texture("../../../assets/textures/trippy.jpg");
        brick = new Texture("../../../assets/textures/brick.jpg");
        // normal textures
        brickNormal = new Texture("../../../assets/textures/brick_normal.jpg");
        
        lut = new Texture("../../../assets/textures/LUT.png");

        // meshes
        Matrix4 floorMatrix = Matrix4.CreateScale(new Vector3(2.5f, 0.5f, 2.5f)) * Matrix4.CreateRotationZ(75.0f);
        Matrix4 teapotMatrix = Matrix4.CreateTranslation(new Vector3(2f, 0.5f, 0));
        Matrix4 teapotChildMatrix = Matrix4.CreateTranslation(new Vector3(35f, 10f, 5f)) * Matrix4.CreateScale(0.7f);

        floor = new Mesh("../../../assets/objects/floor.obj", floorMatrix, brick, brickNormal);
        teapot = new Mesh("../../../assets/objects/teapot.obj", teapotMatrix, wood);
        teapotChild = new Mesh("../../../assets/objects/teapot.obj", teapotChildMatrix, trippy);

        spotlightTeapot = new Mesh("../../../assets/objects/teapot.obj", Matrix4.CreateTranslation(200f, 0, -40), wood);

        meshes = new List<Mesh>
        {
            floor,
            teapot,
            spotlightTeapot // demonstrates the spotlight
        };
        sceneGraph = new SceneGraph(meshes);
        sceneGraph.Nodes[1].AddChild(new SceneNode(teapotChild));

        // create the shaders
        main = new Shader("../../../shaders/scene_vs.glsl", "../../../shaders/scene_fs.glsl");
        postproc = new Shader("../../../shaders/postproc/post_vs.glsl", "../../../shaders/postproc/post_fs.glsl");

        // create the render target
        if (useRenderTarget) target = new RenderTarget(Screen.width, Screen.height);
        quad = new ScreenQuad();

        Skybox.Load();
    }

    /// <summary>
    /// Renders the scene each frame.
    /// </summary>
    public void RenderGL()
    {
        Matrix4 view = Camera.Load();
        Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60.0f),
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

            sceneGraph!.Render(main,Matrix4.Identity,view,projection);
        }

        // render quad
        target.Unbind();
        if (postproc != null)
            quad!.Render(postproc, target.GetTextureID());
        
        // look-up table
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
            main!.SetVec3($"lights[{i}].Direction", lights[i].Direction);
            main.SetFloat($"lights[{i}].Cutoff", lights[i].CutOff);
            main.SetFloat($"lights[{i}].CutoffOut", lights[i].CutoffOut);
            main.SetInt($"lights[{i}].Type", lights[i].Type);
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

        switch (kea.Key)
        {
            case Keys.Z:
                postproc.SetInt("applyChrom", aberration);
                postproc.SetInt("applyVignette", vignette);
                vignette = vignette == 1 ? 0 : 1;
                aberration = aberration == 1 ? 0 : 1;
                break;
            case Keys.X:
                postproc.SetInt("toggleLUT", togglelut);
                togglelut = togglelut == 1 ? 0 : 1;
                break;
        }
    }
}
