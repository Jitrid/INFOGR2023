using OpenTK.Mathematics;

namespace INFOGR2023Template;

public class Ray
{
    public Vector3 Origin { get; set; }
    public Vector3 Direction { get; set; }

    public Ray(Vector3 origin, Vector3 direction)
    {
        Origin = origin;
        Direction = direction;
    }

    public Vector3 Parametric(float t) => Origin + t * Direction;
}