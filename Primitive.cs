using DrawElementsType = OpenTK.Graphics.ES30.DrawElementsType;
using GL = OpenTK.Graphics.ES30.GL;
using PrimitiveType = OpenTK.Graphics.ES30.PrimitiveType;

namespace INFOGR2023Template
{
    public class Primitive
    {
        public int vertexArrayObject;
        public int indexCount;
        public float[] verticesArray;
        public float distance;


        public Primitive(float[] vertices, float distance)
        {
            verticesArray = vertices;
            this.distance = distance;
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
                -halfSize, halfSize, halfSize    // Vertex 7
            };

            float distance = (float)Math.Sqrt(Math.Pow(2 * halfSize, 2));

            return new Primitive(vertices, distance);
        }
    }
}

