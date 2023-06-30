using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Rasterization;

public class Skybox
{
    // https://learnopengl.com/code_viewer.php?code=advanced/cubemaps_skybox_data
    private static readonly float[] skyboxVertices = {
        // positions          
        -1.0f,  1.0f, -1.0f,
        -1.0f, -1.0f, -1.0f,
        1.0f, -1.0f, -1.0f,
        1.0f, -1.0f, -1.0f,
        1.0f,  1.0f, -1.0f,
        -1.0f,  1.0f, -1.0f,

        -1.0f, -1.0f,  1.0f,
        -1.0f, -1.0f, -1.0f,
        -1.0f,  1.0f, -1.0f,
        -1.0f,  1.0f, -1.0f,
        -1.0f,  1.0f,  1.0f,
        -1.0f, -1.0f,  1.0f,

        1.0f, -1.0f, -1.0f,
        1.0f, -1.0f,  1.0f,
        1.0f,  1.0f,  1.0f,
        1.0f,  1.0f,  1.0f,
        1.0f,  1.0f, -1.0f,
        1.0f, -1.0f, -1.0f,

        -1.0f, -1.0f,  1.0f,
        -1.0f,  1.0f,  1.0f,
        1.0f,  1.0f,  1.0f,
        1.0f,  1.0f,  1.0f,
        1.0f, -1.0f,  1.0f,
        -1.0f, -1.0f,  1.0f,

        -1.0f,  1.0f, -1.0f,
        1.0f,  1.0f, -1.0f,
        1.0f,  1.0f,  1.0f,
        1.0f,  1.0f,  1.0f,
        -1.0f,  1.0f,  1.0f,
        -1.0f,  1.0f, -1.0f,

        -1.0f, -1.0f, -1.0f,
        -1.0f, -1.0f,  1.0f,
        1.0f, -1.0f, -1.0f,
        1.0f, -1.0f, -1.0f,
        -1.0f, -1.0f,  1.0f,
        1.0f, -1.0f,  1.0f
    };

    private static int VAO = -1, VBO = -1;

    private static Texture? texture;
    private static Shader? cubemap;

    /// <summary>
    /// Initializes the cube map.
    /// </summary>
    public static void Load()
    {
        cubemap = new Shader("../../../shaders/cubemap_vs.glsl", "../../../shaders/cubemap_fs.glsl");

        VAO = GL.GenVertexArray();
        VBO = GL.GenBuffer();
        GL.BindVertexArray(VAO);
        GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
        GL.BufferData(BufferTarget.ArrayBuffer, skyboxVertices.Length * sizeof(float), skyboxVertices, BufferUsageHint.StaticDraw);

        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), IntPtr.Zero);

        string[] faces =
        {
            "../../../assets/skybox/right.png",
            "../../../assets/skybox/left.png",
            "../../../assets/skybox/top.png",
            "../../../assets/skybox/bottom.png",
            "../../../assets/skybox/front.png",
            "../../../assets/skybox/back.png"
        };
        texture = new Texture(faces);

        cubemap.SetInt("skybox", 0);

        GL.UseProgram(cubemap.ProgramID);
    }

    /// <summary>
    /// Updates the values for the cube map each frame.
    /// </summary>
    /// <param name="view">The view matrix of the scene.</param>
    /// <param name="projection">The projection matrix of the scene.</param>
    public static void Render(Matrix4 view, Matrix4 projection)
    {
        Matrix4 matrix = new(new Matrix3(view));
        GL.UniformMatrix4(cubemap.ViewTransformation, false, ref matrix);
        GL.UniformMatrix4(cubemap.ProjectionTransformation, false, ref projection);

        GL.UseProgram(cubemap!.ProgramID);

        GL.DepthFunc(DepthFunction.Lequal);

        GL.BindVertexArray(VAO);
        GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);

        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(cubemap.InVertexPositionObject, 3,
            VertexAttribPointerType.Float, false, 3 * sizeof(float), IntPtr.Zero);

        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.TextureCubeMap, texture!.ID);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 36);

        GL.DepthFunc(DepthFunction.Less);
        GL.UseProgram(0);
    }
}
