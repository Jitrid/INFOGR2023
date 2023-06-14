using OpenTK.Graphics.OpenGL;

namespace Rasterization;

public class ScreenQuad
{
    // data members
    private int VAO, VBO;

    private readonly float[] _vertices =
    { // x   y  z  u  v
        -1,  1, 0, 0, 1,
        1,  1, 0, 1, 1,
        -1, -1, 0, 0, 0,
        1, -1, 0, 1, 0,
    };

    // initialization; called during first render
    public void Prepare(Shader shader)
    {
        if (VBO == 0)
        {
            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);
            GL.ObjectLabel(ObjectLabelIdentifier.VertexArray, VAO, -1, "VAO for ScreenQuad");
            // prepare VBO for quad rendering
            GL.GenBuffers(1, out VBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.ObjectLabel(ObjectLabelIdentifier.Buffer, VBO, -1, "VBO for ScreenQuad");
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(4 * 5 * 4), _vertices, BufferUsageHint.StaticDraw);
            // VBO contains vertices in correct order so no EBO needed
        }
    }

    // render the mesh using the supplied shader and matrix
    public void Render(Shader shader, int textureID)
    {
        // on first run, prepare buffers
        Prepare(shader);

        // enable shader
        GL.UseProgram(shader.ProgramID);

        // enable texture
        int texLoc = GL.GetUniformLocation(shader.ProgramID, "pixels");
        GL.Uniform1(texLoc, 0);
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, textureID);

        // enable position and uv attributes
        GL.EnableVertexAttribArray(shader.InVertexPositionObject);
        GL.EnableVertexAttribArray(shader.InVertexUV);

        // bind vertex data
        GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);

        // link vertex attributes to shader parameters 
        GL.VertexAttribPointer(shader.InVertexPositionObject, 3, VertexAttribPointerType.Float, false, 20, 0);
        GL.VertexAttribPointer(shader.InVertexUV, 2, VertexAttribPointerType.Float, false, 20, 3 * 4);

        // render (no EBO so use DrawArrays to process vertices in the order they're specified in the VBO)
        GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);

        // disable shader
        GL.UseProgram(0);
    }
}
