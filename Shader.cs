using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Rasterization;

public class Shader
{
    public int ProgramID, vsID, fsID;
    public int InVertexPositionObject;
    public int InVertexNormalObject;
    public int InVertexUV;
    public int InVertexTangent;
    public int InVertexBiTangent;

    public int UniformObjectToScreen;
    public int UniformObjectToWorld;

    public int UniformViewMatrix;
    public int UniformProjectionMatrix;

    public Shader(string vertexShader, string fragmentShader)
    {
        // compile shaders
        ProgramID = GL.CreateProgram();
        Load(vertexShader, ShaderType.VertexShader, ProgramID, out vsID);
        Load(fragmentShader, ShaderType.FragmentShader, ProgramID, out fsID);
        GL.LinkProgram(ProgramID);

        // Debug info
        string infoLog = GL.GetProgramInfoLog(ProgramID);
        if (infoLog.Length != 0) Console.WriteLine(infoLog);

        // get locations of shader parameters
        InVertexPositionObject = GL.GetAttribLocation(ProgramID, "vertexPosition");
        InVertexNormalObject = GL.GetAttribLocation(ProgramID, "vertexNormal");
        InVertexUV = GL.GetAttribLocation(ProgramID, "vertexUV");
        InVertexTangent = GL.GetAttribLocation(ProgramID, "tangent");
        InVertexBiTangent = GL.GetAttribLocation(ProgramID, "biTangent");

        UniformObjectToScreen = GL.GetUniformLocation(ProgramID, "objectToScreen");
        UniformObjectToWorld = GL.GetUniformLocation(ProgramID, "objectToWorld");

        UniformViewMatrix = GL.GetUniformLocation(ProgramID, "viewMatrix");
        UniformProjectionMatrix = GL.GetUniformLocation(ProgramID, "projectionMatrix");
    }

    /// <summary>
    /// Loads the shaders.
    /// </summary>
    void Load(string filename, ShaderType type, int program, out int ID)
    {
        ID = GL.CreateShader(type);
        using (StreamReader sr = new(filename)) GL.ShaderSource(ID, sr.ReadToEnd());
        GL.CompileShader(ID);
        GL.AttachShader(program, ID);

        // Debug info
        string infoLog = GL.GetShaderInfoLog(ID);
        if (infoLog.Length != 0) Console.WriteLine(infoLog);
    }

    // Methods to set uniform variables of specific data types.
    public void SetVec3(string name, Vector3 value) => GL.Uniform3(GL.GetUniformLocation(ProgramID, name), value);
    public void SetFloat(string name, float value) => GL.Uniform1(GL.GetUniformLocation(ProgramID, name), value);
    public void SetInt(string name, int value) => GL.Uniform1(GL.GetUniformLocation(ProgramID, name), value);
}
