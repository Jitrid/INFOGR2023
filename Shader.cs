using OpenTK.Graphics.OpenGL;

namespace Rasterization;

public class Shader
{
    public int ProgramID, vsID, fsID;
    public int InVertexPositionObject;
    public int InVertexNormalObject;
    public int InVertexUV;
    public int UniformObjectToScreen;
    public int UniformObjectToWorld;

    public Shader(string vertexShader, string fragmentShader)
    {
        // compile shaders
        ProgramID = GL.CreateProgram();
        GL.ObjectLabel(ObjectLabelIdentifier.Program, ProgramID, -1, vertexShader + " + " + fragmentShader);
        Load(vertexShader, ShaderType.VertexShader, ProgramID, out vsID);
        Load(fragmentShader, ShaderType.FragmentShader, ProgramID, out fsID);
        GL.LinkProgram(ProgramID);
        string infoLog = GL.GetProgramInfoLog(ProgramID);
        if (infoLog.Length != 0) Console.WriteLine(infoLog);

        // get locations of shader parameters
        InVertexPositionObject = GL.GetAttribLocation(ProgramID, "vertexPositionObject");
        InVertexNormalObject = GL.GetAttribLocation(ProgramID, "vertexNormalObject");
        InVertexUV = GL.GetAttribLocation(ProgramID, "vertexUV");
        UniformObjectToScreen = GL.GetUniformLocation(ProgramID, "objectToScreen");
        UniformObjectToWorld = GL.GetUniformLocation(ProgramID, "objectToWorld");
    }

    /// <summary>
    /// Loads the shaders.
    /// </summary>
    void Load(string filename, ShaderType type, int program, out int ID)
    {
        // source: http://neokabuto.blogspot.nl/2013/03/opentk-tutorial-2-drawing-triangle.html
        ID = GL.CreateShader(type);
        GL.ObjectLabel(ObjectLabelIdentifier.Shader, ID, -1, filename);
        using (StreamReader sr = new(filename)) GL.ShaderSource(ID, sr.ReadToEnd());
        GL.CompileShader(ID);
        GL.AttachShader(program, ID);
        string infoLog = GL.GetShaderInfoLog(ID);
        if (infoLog.Length != 0) Console.WriteLine(infoLog);
    }
}
