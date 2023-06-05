using Vector3 = OpenTK.Mathematics.Vector3;

namespace INFOGR2023Template;

public class Plane : Primitive
{
    public Vector3 Normal { get; set; }
    public float Distance { get; set; }

    public Vector3 Color;

    public Plane(Vector3 normal, float distance, Vector3 color, Vector3 diffusecolor, Vector3 specularcolor, float specularpower, float reflectionCoefficient)
    {
        Normal = Vector3.Normalize(normal);
        Distance = distance;
        Color = color;
        DiffuseColor = diffusecolor;
        SpecularColor = specularcolor;
        SpecularPower = specularpower;
        ReflectionCoefficient = reflectionCoefficient;
        //boundingBox = getBox();
    }

    public Plane() { }

    public override Vector3 GetNormal(Vector3 point) => Normal;
    public override Vector3 GetColor() => Color;

    public override BoundingBox GetBox()
    {
        Vector3 p0 = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
        Vector3 p3 = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
        return new BoundingBox(p0, p3);
    }

    public override bool HitRay(Ray ray, out Vector3 intersect)
    {
        intersect = Vector3.Zero;
        //if (boundingBox == null || !boundingBox.intersectBox(ray))
        //{
        //    return false;
        //}

        float denom = Vector3.Dot(ray.Direction, Normal);
        if (denom < float.Epsilon)
            return false;

        Vector3 center = Normal * Distance;
        float t = (Vector3.Dot(center - ray.Origin, Normal)) / denom;

        if (t < 0)
            return false;

        intersect = ray.Origin + t * ray.Direction;
        return true;
    }
}

public class CheckeredPlane : Plane
{
    public CheckeredPlane(Vector3 normal, float distance, float reflectionCoefficient)
    {
        Normal = Vector3.Normalize(normal);
        Distance = distance;
        ReflectionCoefficient = reflectionCoefficient;
    }

    public Vector3 GetCheckeredColor(Vector3 point) => ((int)(Math.Floor(2 * point.X) + Math.Floor(point.Z)) & 1) * Vector3.One + new Vector3(0.01f, 0.01f, 0.01f);
}
