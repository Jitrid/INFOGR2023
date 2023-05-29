using OpenTK.Mathematics;

namespace INFOGR2023Template;

public class Sphere : Primitive
{
    public Vector3 Position;

    public float Radius;

    public Sphere(float[] vertices, float distance) : base(vertices, distance)
    {

    }
}