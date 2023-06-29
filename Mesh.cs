using System.Runtime.InteropServices;
using Assimp;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Face = Assimp.Face;
using PrimitiveType = OpenTK.Graphics.OpenGL.PrimitiveType;

namespace Rasterization;

// Mesh and MeshLoader based on work by JTalton; https://web.archive.org/web/20160123042419/www.opentk.com/node/642
// Only triangles and quads with vertex positions, normals, and texture coordinates are supported
public class Mesh
{
    // data members
    public readonly string Filename; // for improved error reporting
    public ObjVertex[]? Vertices; // vertices (positions and normals in Object Space, and texture coordinates)
    public ObjTriangle[]? Triangles; // triangles (3 indices into the vertices array)
    public ObjQuad[]? Quads; // quads (4 indices into the vertices array)
    private int vertexBufferId; // vertex buffer object (VBO) for vertex data
    private int triangleBufferId; // element buffer object (EBO) for triangle vertex indices

    public Matrix4 ObjectToWorld;
    public Texture Texture;
    public Texture TextureNormal;
    private bool normalMappingEnabled;

    public Mesh(string filename, Matrix4 model, Texture texture, Texture textureNormal = null)
    {
        Filename = filename;
        Load(filename);

        ObjectToWorld = model;
        Texture = texture;
        TextureNormal = textureNormal;
        normalMappingEnabled = textureNormal != null;
    }

    // initialization; called during first render
    public void Prepare()
    {
        if (vertexBufferId != 0) return;
        // generate interleaved vertex data array (uv/normal/position per vertex)
        GL.GenBuffers(1, out vertexBufferId);
        GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferId);
        GL.ObjectLabel(ObjectLabelIdentifier.Buffer, vertexBufferId, 8 + Filename.Length, "VBO for " + Filename);
        GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(Vertices?.Length * Marshal.SizeOf(typeof(ObjVertex))),
            Vertices, BufferUsageHint.StaticDraw);

        // generate triangle index array
        GL.GenBuffers(1, out triangleBufferId);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, triangleBufferId);
        GL.BufferData(BufferTarget.ElementArrayBuffer,
            (IntPtr)(Triangles?.Length * Marshal.SizeOf(typeof(ObjTriangle))), Triangles, BufferUsageHint.StaticDraw);
    }

    // render the mesh using the supplied shader and matrix
    public void Render(Shader shader, Matrix4 objectToCamera, Matrix4 cameraToScreen)
    {
        Matrix4 objectToScreen = ObjectToWorld * objectToCamera * cameraToScreen;

        Prepare();

        // enable shader
        GL.UseProgram(shader.ProgramID);

        // enable diffuse texture
        GL.Uniform1(GL.GetUniformLocation(shader.ProgramID, "diffuseTexture"), 0);
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, Texture.ID);
        // enable normal texture
        if (normalMappingEnabled)
        {
            GL.Uniform1(GL.GetUniformLocation(shader.ProgramID, "normalTexture"), 1);
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, TextureNormal.ID);
        }

        // pass transforms to vertex shader
        GL.UniformMatrix4(shader.UniformObjectToScreen, false, ref objectToScreen);
        GL.UniformMatrix4(shader.UniformObjectToWorld, false, ref ObjectToWorld);
        shader.SetInt("mappingEnabled", normalMappingEnabled ? 1 : 0);

        // enable position, normal and uv attribute arrays corresponding to the shader "in" variables
        GL.EnableVertexAttribArray(shader.InVertexPositionObject);
        GL.EnableVertexAttribArray(shader.InVertexNormalObject);
        GL.EnableVertexAttribArray(shader.InVertexUV);
        GL.EnableVertexAttribArray(shader.InVertexTangent);
        GL.EnableVertexAttribArray(shader.InVertexBiTangent);

        // bind vertex data
        GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferId);

        // link vertex attributes to shader parameters 
        int stride = Marshal.SizeOf<ObjVertex>();
        GL.VertexAttribPointer(shader.InVertexUV, 2, VertexAttribPointerType.Float, false, stride, 0);
        GL.VertexAttribPointer(shader.InVertexNormalObject, 3, VertexAttribPointerType.Float, true, stride, 2 * 4);
        GL.VertexAttribPointer(shader.InVertexPositionObject, 3, VertexAttribPointerType.Float, false, stride, 5 * 4);
        GL.VertexAttribPointer(shader.InVertexTangent, 3, VertexAttribPointerType.Float, false, stride, 8 * 4);
        GL.VertexAttribPointer(shader.InVertexBiTangent, 3, VertexAttribPointerType.Float, false, stride, 11 * 4);

        // bind triangle index data and render
        if (Triangles is { Length: > 0 })
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, triangleBufferId);
            GL.DrawArrays(PrimitiveType.Triangles, 0, Triangles.Length * 3);
        }

        // bind quad index data and render
        if (Quads is { Length: > 0 })
            throw new Exception("Quads not supported in Modern OpenGL");

        // restore previous OpenGL state
        GL.UseProgram(0);
    }

    public void Load(string filename)
    {
        using AssimpContext assimp = new();
        Scene scene =
            assimp.ImportFile(filename, PostProcessSteps.Triangulate | PostProcessSteps.CalculateTangentSpace);

        Assimp.Mesh mesh = scene.Meshes[0];

        List<ObjVertex> objVertices = new();
        List<ObjTriangle> objTriangles = new();

        for (int i = 0; i < mesh.VertexCount; i++)
            objVertices.Add(new ObjVertex
            {
                Normal = ConvertToTKVec(mesh.Normals[i]),
                TexCoord = ConvertToTKVec(mesh.TextureCoordinateChannels[0][i]).Xy,
                Vertex = ConvertToTKVec(mesh.Vertices[i]),
                Tangent = ConvertToTKVec(mesh.Tangents[i]),
                BiTangent = ConvertToTKVec(mesh.BiTangents[i])
            });
        
        foreach (Face face in mesh.Faces) 
            objTriangles.Add(new ObjTriangle
            {
                Index0 = face.Indices[0],
                Index1 = face.Indices[1],
                Index2 = face.Indices[2]
            });

        Vertices = objVertices.ToArray();
        Triangles = objTriangles.ToArray();

        objVertices.Clear();
        objTriangles.Clear();
    }
    public Vector3 ConvertToTKVec(Vector3D vector) => new(vector.X, vector.Y, vector.Z);
}

// layout of a single vertex
[StructLayout(LayoutKind.Sequential)]
public struct ObjVertex
{
    public Vector2 TexCoord;
    public Vector3 Normal;
    public Vector3 Vertex;
    public Vector3 Tangent;
    public Vector3 BiTangent;
}

// layout of a single triangle
[StructLayout(LayoutKind.Sequential)]
public struct ObjTriangle
{
    public int Index0, Index1, Index2;
}

// layout of a single quad
[StructLayout(LayoutKind.Sequential)]
public struct ObjQuad
{
    public int Index0, Index1, Index2, Index3;
}
