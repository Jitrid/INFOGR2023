using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Rasterization;

public class Skybox
{
    // https://learnopengl.com/code_viewer.php?code=advanced/cubemaps_skybox_data
    private readonly float[] skyboxVertices = {
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

    private int VAO = -1, VBO = -1;

    private Texture? cubemapTexture;
    private Shader? cubemap;

    public void Load()
    {
        cubemap = new Shader("../../../shaders/cubemap_vs.glsl", "../../../shaders/cubemap_fs.glsl");

        VAO = GL.GenVertexArray();
        VBO = GL.GenBuffer();
        GL.BindVertexArray(VAO);
        GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
        GL.BufferData(BufferTarget.ArrayBuffer, skyboxVertices.Length * sizeof(float), skyboxVertices, BufferUsageHint.StaticDraw);

        string[] faces =
        {
            "../../../assets/skybox/right.png",
            "../../../assets/skybox/left.png",
            "../../../assets/skybox/top.png",
            "../../../assets/skybox/bottom.png",
            "../../../assets/skybox/front.png",
            "../../../assets/skybox/back.png"
        };
        cubemapTexture = new Texture(faces);

        cubemap.SetInt("skybox", 0);

        GL.UseProgram(cubemap.ProgramID);
    }

    public void Render(Matrix4 worldToCamera, Matrix4 cameraToScreen)
    {
        GL.UseProgram(cubemap.ProgramID);
        
        GL.ActiveTexture(TextureUnit.Texture1);
        GL.BindTexture(TextureTarget.TextureCubeMap, cubemapTexture.ID);

        Matrix4 matrix = new(new Matrix3(worldToCamera));
        matrix = Matrix4.Identity;
        GL.UniformMatrix4(cubemap.UniformViewMatrix, false, ref matrix);
        GL.UniformMatrix4(cubemap.UniformProjectionMatrix, false, ref matrix);

        GL.EnableVertexAttribArray(cubemap.InVertexPositionObject);

        GL.BindVertexArray(VAO);
        GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);

        GL.VertexAttribPointer(cubemap.InVertexPositionObject, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 8);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, VBO);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
    }
}
