using Vector3 = OpenTK.Mathematics.Vector3;

namespace INFOGR2023Template;

public class Sphere : Primitive
{
    public Vector3 Center { get; }
    public float Radius { get; }

    public Vector3 Color { get; }

    public Sphere(Vector3 center, float radius, Vector3 color, Vector3 diffcolor, Vector3 speccolor, float specularPower, float reflectionCoefficient)
    {
        Center = center;
        Radius = radius;
        Color = color;
        DiffuseColor = diffcolor;
        SpecularColor = speccolor;
        SpecularPower = specularPower;
        ReflectionCoefficient = reflectionCoefficient;
    }

    public override Vector3 GetNormal(Vector3 point) => Vector3.Normalize(point - Center);

    public override Vector3 GetColor() => Color;

    public override bool HitRay(Ray ray, out Vector3 intersect)
    {
        intersect = Vector3.Zero;

        float a = Vector3.Dot(ray.Direction, ray.Direction);
        float b = 2 * (Vector3.Dot(ray.Origin, ray.Direction) - Vector3.Dot(Center, ray.Direction));
        float c = Vector3.Dot(Center, Center) + Vector3.Dot(ray.Origin, ray.Origin) - 2 * Vector3.Dot(Center, ray.Origin) - (Radius * Radius);
        float discriminant = b * b - 4 * a * c;

        // No intersections.
        if (discriminant < 0)
        {
            intersect = Vector3.Zero;
            return false;
        }

        float t1 = (float)(-b - Math.Sqrt(discriminant)) / 2 * a;
        float t2 = (float)(-b + Math.Sqrt(discriminant)) / 2 * a;

        if (t1 > 0)
        {
            intersect = ray.Origin + (t1 + 0.0001f) * ray.Direction;
            return true;
        }
        if (t2 > 0)
        {
            intersect = ray.Origin + (t1 - 0.0001f) * ray.Direction;
            return true;
        }

        return false;
    }

    public override bool IntersectsWithLight(Vector3 intersectionPoint, Vector3 lightPosition, out Vector3 direction)
    {
        direction = lightPosition - intersectionPoint;
        Ray ray = new(intersectionPoint, direction, 0);

        float a = Vector3.Dot(ray.Direction, ray.Direction);
        float b = 2 * Vector3.Dot(Center, ray.Direction);
        float c = Vector3.Dot(Center, Center) - (Radius * Radius);
        float discriminant = b * b - 4 * a * c;

        if (discriminant < 0)
        {
            return true;
        }

        float t1 = (float)(-b - Math.Sqrt(discriminant)) / (2 * a);
        float t2 = (float)(-b + Math.Sqrt(discriminant)) / (2 * a);

        if (t1 > 0 || t2 > 0)
        {
            return false;
        }

        return true;
    }
}
