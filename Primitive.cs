using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using All = OpenTK.Graphics.ES30.All;
using BeginMode = OpenTK.Graphics.OpenGL.BeginMode;
using BufferTarget = OpenTK.Graphics.ES30.BufferTarget;
using BufferUsageHint = OpenTK.Graphics.ES30.BufferUsageHint;
using DrawElementsType = OpenTK.Graphics.ES30.DrawElementsType;
using GL = OpenTK.Graphics.ES30.GL;
using PrimitiveType = OpenTK.Graphics.ES30.PrimitiveType;
using VertexAttribPointerType = OpenTK.Graphics.ES30.VertexAttribPointerType;

namespace INFOGR2023Template
{
    public class Primitive
    {
        public int vertexBufferObject;
         public int indexBufferObject;
        public  int vertexArrayObject;
        public int indexCount;
        public float[] verticesArray;


        public Primitive(float[] vertices, int[] indices)
        {
            verticesArray = vertices;

            // Create and bind the vertex buffer object
            vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            // Create and bind the index buffer object
            indexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(int), indices, BufferUsageHint.StaticDraw);

            // Create and bind the vertex array object
            vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(vertexArrayObject);

            // Bind the vertex buffer object to the vertex array object
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            GL.BindVertexArray(vertexArrayObject);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(0);

            // Bind the index buffer object to the vertex array object
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBufferObject);

            // Unbind the vertex array object
            GL.BindVertexArray(0);

            indexCount = indices.Length;
        }


        public void Render()
        {
            GL.BindVertexArray(vertexArrayObject);
            GL.DrawElements(PrimitiveType.Triangles, indexCount, DrawElementsType.UnsignedShort, IntPtr.Zero);
            GL.BindVertexArray(0);
        }

        public float[] GetVertices()
        {
            return verticesArray;
        }
        public static Primitive CreatePlane(float width, float height)
        {
            float[] vertices =
            {
                -width / 2, 0, -height / 2,
                width / 2, 0, -height / 2,
                width / 2, 0, height / 2,
                -width / 2, 0, height / 2
            };
            
            int[] indices = { 0, 1, 2, 0, 2, 3 };

            return new Primitive(vertices, indices);
        }
        public static Primitive CreateCube(float size)
        {
            float halfSize = size / 2f;

            float[] vertices =
            {
                -halfSize, -halfSize, -halfSize, // Vertex 0
                halfSize, -halfSize, -halfSize,  // Vertex 1
                halfSize, halfSize, -halfSize,   // Vertex 2
                -halfSize, halfSize, -halfSize,  // Vertex 3
                -halfSize, -halfSize, halfSize,  // Vertex 4
                halfSize, -halfSize, halfSize,   // Vertex 5
                halfSize, halfSize, halfSize,    // Vertex 6
                -halfSize, halfSize, halfSize     // Vertex 7
            };

            int[] indices =
            {
                0, 1, 2, 0, 2, 3, // Front face
                1, 5, 6, 1, 6, 2, // Right face
                5, 4, 7, 5, 7, 6, // Back face
                4, 0, 3, 4, 3, 7, // Left face
                3, 2, 6, 3, 6, 7, // Top face
                4, 5, 1, 4, 1, 0  // Bottom face
            };

            return new Primitive(vertices, indices);
        }

        //public static Primitive CreateSphere(float radius, int sub)
        //{

        //    return new Primitive();
        //}
    



        public bool IntersectPlane(Ray ray, Primitive plane, out float intersectionDistance, out Vector3 intersectionPoint)
        {
            // Get the vertices of the plane
            float[] vertices = plane.GetVertices();

            // Check if the ray intersects the plane
            Vector3 planeNormal;
            Vector3 v0 = new Vector3(vertices[0], vertices[1], vertices[2]);
            Vector3 v1 = new Vector3(vertices[3], vertices[4], vertices[5]);
            Vector3 v2 = new Vector3(vertices[6], vertices[7], vertices[8]);
            planeNormal = Vector3.Cross(v1 - v0, v2 - v0);
            planeNormal = Vector3.Normalize(planeNormal);
            float planeD = 10.0f; // The distance of the plane from the origin (can be adjusted as needed)

            // Calculate the denominator of the ray-plane intersection formula
            float denominator = Vector3.Dot(planeNormal, ray.Direction);

            // If the denominator is zero, the ray is parallel to the plane and has no intersection
            if (Math.Abs(denominator) < float.Epsilon)
            {
                intersectionDistance = 0.0f;
                intersectionPoint = Vector3.Zero;
                return false;
            }

            // Calculate the distance from the ray origin to the intersection point
            float numerator = -(Vector3.Dot(planeNormal, ray.Origin) + planeD);
            intersectionDistance = numerator / denominator;

            // Check if the intersection point is in front of the ray
            if (intersectionDistance >= 0.0f)
            {
                // Calculate the intersection point
                intersectionPoint = ray.Origin + ray.Direction * intersectionDistance;

                // Check if the intersection point is within the plane's bounds
                float minX = float.MaxValue;
                float maxX = float.MinValue;
                float minZ = float.MaxValue;
                float maxZ = float.MinValue;

                for (int i = 0; i < vertices.Length; i += 3)
                {
                    Vector3 vertex = new Vector3(vertices[i], vertices[i + 1], vertices[i + 2]);

                    // Update the minimum and maximum coordinates
                    minX = Math.Min(minX, vertex.X);
                    maxX = Math.Max(maxX, vertex.X);
                    minZ = Math.Min(minZ, vertex.Z);
                    maxZ = Math.Max(maxZ, vertex.Z);
                }

                // Check if the intersection point is within the bounds of the plane
                if (intersectionPoint.X >= minX && intersectionPoint.X <= maxX &&
                    intersectionPoint.Z >= minZ && intersectionPoint.Z <= maxZ)
                {
                    return true;
                }
            }

            intersectionPoint = Vector3.Zero;
            return false;
        }

    }



}

