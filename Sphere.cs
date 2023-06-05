using System.Numerics;
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
        BoundingBox = GetBox();
        DiffuseColor = diffcolor;
        SpecularColor = speccolor;
        SpecularPower = specularPower;
        ReflectionCoefficient = reflectionCoefficient;
    }

    public override Vector3 GetNormal(Vector3 point) => Vector3.Normalize(point - Center);

    public override Vector3 GetColor() => Color;

    public override BoundingBox GetBox()
    {
        Vector3 p0 = Center - new Vector3(Radius, Radius, Radius);
        Vector3 p3 = Center + new Vector3(Radius, Radius, Radius);
        return new BoundingBox(p0, p3);
    }

    public override bool HitRay(Ray ray, out Vector3 intersect)
    {
        intersect = Vector3.Zero;

        // if (BoundingBox == null || !BoundingBox.intersectBox(ray))
        //     return false;

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
}

public class Triangle : Primitive
{
    public Vector3 v1, a, b, c;
    private float dot1, dot2, dot3;
    public float Distance;
    public Vector3 Normal;

    public Vector3 Color { get; }

    public Triangle(Vector3 c1, Vector3 c2, Vector3 c3, Vector3 color, Vector3 diffcolor, Vector3 speccolor, float specularPower, float reflectionCoefficient)
    {
        this.v1 = c1;
        a = c2 - c1;
        b = c3 - c1;

        dot1 = Vector3.Dot(a, b);
        dot2 = Vector3.Dot(a, a);
        dot3 = Vector3.Dot(b, b);

        Normal = Vector3.Normalize(Vector3.Cross(a, b));
        Distance = -Vector3.Dot(Normal, v1);

        Color = color;
        DiffuseColor = diffcolor;
        SpecularColor = speccolor;
        SpecularPower = specularPower;
        ReflectionCoefficient = reflectionCoefficient;
    }

    public override Vector3 GetNormal(Vector3 point) => Normal;
    public override Vector3 GetColor() => Color;

    public override BoundingBox GetBox()
    {
        throw new NotImplementedException();
    }

    public override bool HitRay(Ray ray, out Vector3 intersect)
    {
        intersect = Vector3.Zero;

        float denom = Vector3.Dot(ray.Direction, Normal);

        if (denom < float.Epsilon)
            return false;

        float t = -(Vector3.Dot(ray.Origin, Normal) + Distance) / Vector3.Dot(ray.Direction, Normal);
        if (t < 0)
            return false;

        intersect = ray.Origin + t * ray.Direction;

        if (IsInTriangle(intersect))
            return true;

        intersect = Vector3.Zero;
        return false;
    }

    // TODO: Dit is volledig plagiaat. Als we dit willen gebruiken, moeten we het aanpassen. Bovenstaande functie is zelf geschreven.
    public bool IsInTriangle(Vector3 intersect)
    {
        c = intersect - v1;

        float dot4 = Vector3.Dot(c, b);
        float dot5 = Vector3.Dot(c, a);

        float denom = dot1 * dot1 - dot2 * dot3;
        float s = (dot1 * dot4 - dot3 * dot5) / denom;

        if (s < 0) return false;

        float t = (dot1 * dot5 - dot2 * dot4) / denom;
        if (t < 0) return false;

        if (s + t <= 1)
            return true;

        return false;
    }
}
