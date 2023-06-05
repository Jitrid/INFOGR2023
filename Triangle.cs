using Vector3 = OpenTK.Mathematics.Vector3;

namespace INFOGR2023Template;

public class Triangle : Primitive
{
    public Vector3 p0, p1, p2;

    public float Distance;
    public Vector3 Normal;

    public Vector3 Color { get; }

    public Triangle(Vector3 c1, Vector3 c2, Vector3 c3, Vector3 color, float reflectionCoefficient)
    {
        p0 = c1;
        p1 = c2 - c1;
        p2 = c3 - c1;

        Normal = Vector3.Normalize(Vector3.Cross(p1, p2));
        Distance = -Vector3.Dot(Normal, p0);

        Color = color;
        ReflectionCoefficient = reflectionCoefficient;
    }

    public override Vector3 GetNormal(Vector3 point) => Normal;
    public override Vector3 GetColor() => Color;

    public override bool HitRay(Ray ray, out Vector3 intersect)
    {
        intersect = Vector3.Zero;

        float denom = Vector3.Dot(ray.Direction, Normal);

        if (denom < float.Epsilon)
            return false;

        float t = -(Vector3.Dot(ray.Origin, Normal) + Distance) / denom;
        if (t < 0)
            return false;

        intersect = ray.Origin + (t - 0.01f) * ray.Direction;

        if (IsInTriangle(intersect))
            return true;

        intersect = Vector3.Zero;
        return false;
    }

    /// <summary>
    /// Determine whether an intersection is inside of the triangle or not.
    /// </summary>
    public bool IsInTriangle(Vector3 intersect)
    {
        // All logic is obtained from various mathematical sources, such as the slides and general mathematics books.
        Vector3 p3 = intersect - p0;

        // Create variables for all of the dot products to reduce calculations.
        float dot1 = Vector3.Dot(p1, p2);
        float dot2 = Vector3.Dot(p1, p1);
        float dot3 = Vector3.Dot(p2, p2);
        float dot4 = Vector3.Dot(p3, p2);
        float dot5 = Vector3.Dot(p3, p1);

        float denom = dot1 * dot1 - dot2 * dot3;

        float s = (dot1 * dot4 - dot3 * dot5) / denom;

        if (!(s >= 0)) return false;
        float t = (dot1 * dot5 - dot2 * dot4) / denom;

        if (!(t >= 0)) return false;

        return s + t <= 1;
    }
}
