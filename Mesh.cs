using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Rasterization;

// Mesh and MeshLoader based on work by JTalton; https://web.archive.org/web/20160123042419/www.opentk.com/node/642
// Only triangles and quads with vertex positions, normals, and texture coordinates are supported
public class Mesh
{
    // data members
    public readonly string Filename;        // for improved error reporting
    public ObjVertex[]? Vertices;           // vertices (positions and normals in Object Space, and texture coordinates)
    public ObjTriangle[]? Triangles;        // triangles (3 indices into the vertices array)
    public ObjQuad[]? Quads;                // quads (4 indices into the vertices array)
    private int vertexBufferId;             // vertex buffer object (VBO) for vertex data
    private int triangleBufferId;           // element buffer object (EBO) for triangle vertex indices

    public Matrix4 ObjectToWorld;
    public Texture Texture;
    
    public Mesh(string filename, Matrix4 model, Texture texture)
    {
        Filename = filename;
        MeshLoader loader = new();
        loader.Load(this, filename);

        ObjectToWorld = model;
        Texture = texture;
    }

    // initialization; called during first render
    public void Prepare()
    {
        if (vertexBufferId != 0) return;
        // generate interleaved vertex data array (uv/normal/position per vertex)
        GL.GenBuffers(1, out vertexBufferId);
        GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferId);
        GL.ObjectLabel(ObjectLabelIdentifier.Buffer, vertexBufferId, 8 + Filename.Length, "VBO for " + Filename);
        GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(Vertices?.Length * Marshal.SizeOf(typeof(ObjVertex))), Vertices, BufferUsageHint.StaticDraw);

        // generate triangle index array
        GL.GenBuffers(1, out triangleBufferId);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, triangleBufferId);
        GL.ObjectLabel(ObjectLabelIdentifier.Buffer, triangleBufferId, 17 + Filename.Length, "triangle EBO for " + Filename);
        GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(Triangles?.Length * Marshal.SizeOf(typeof(ObjTriangle))), Triangles, BufferUsageHint.StaticDraw);
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

        // pass transforms to vertex shader
        GL.UniformMatrix4(shader.UniformObjectToScreen, false, ref objectToScreen);
        GL.UniformMatrix4(shader.UniformObjectToWorld, false, ref ObjectToWorld);

        // enable position, normal and uv attribute arrays corresponding to the shader "in" variables
        GL.EnableVertexAttribArray(shader.InVertexPositionObject);
        GL.EnableVertexAttribArray(shader.InVertexNormalObject);
        GL.EnableVertexAttribArray(shader.InVertexUV);

        // bind vertex data
        GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferId);

        // link vertex attributes to shader parameters 
        GL.VertexAttribPointer(shader.InVertexUV, 2, VertexAttribPointerType.Float, false, 32, 0);
        GL.VertexAttribPointer(shader.InVertexNormalObject, 3, VertexAttribPointerType.Float, true, 32, 2 * 4);
        GL.VertexAttribPointer(shader.InVertexPositionObject, 3, VertexAttribPointerType.Float, false, 32, 5 * 4);

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

    // layout of a single vertex
    [StructLayout(LayoutKind.Sequential)]
    public struct ObjVertex
    {
        public Vector2 TexCoord;
        public Vector3 Normal;
        public Vector3 Vertex;
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
}
