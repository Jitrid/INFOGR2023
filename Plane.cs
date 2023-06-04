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
    }

    public Plane() { }

    public override Vector3 GetNormal(Vector3 point) => Normal;
    public override Vector3 GetColor() => Color;

    public override bool HitRay(Ray ray, out Vector3 intersect)
    {
        float denom = Vector3.Dot(ray.Direction, Normal);
        if (denom == 0)
        {
            // Parallel.
            intersect = Vector3.Zero;
            return false;
        }
        float t = (Vector3.Dot(ray.Origin, Normal) - Distance) / denom;
        if (t < 0)
        {
            // Beyond the camera.
            intersect = Vector3.Zero;
            return false;
        }
        intersect = ray.Origin + t * ray.Direction;
        return true;
    }

    public override bool IntersectsWithLight(Vector3 intersectionPoint, Vector3 lightPosition, out Vector3 direction)
    {
        direction = lightPosition - intersectionPoint;
        Ray ray = new(intersectionPoint, direction, 0);

        // Find the intersection point of the ray with the plane
        if (HitRay(ray, out Vector3 intersect))
        {
            // Check if the intersection point is between the intersection point and the light position
            Vector3 intersectToLight = lightPosition - intersect;
            float distanceToLight = intersectToLight.Length;

            // Check if the distance to the light is smaller than the distance to the intersection point
            if (distanceToLight < intersectToLight.Length)
            {
                // The intersection point is shadowed by the sphere
                return false;
            }
        }

        return true;
    }
}

public class CheckeredPlane : Plane
{
    public Vector3 GetCheckeredColor(Vector3 point) => ((int)(Math.Floor(2 * point.X) + Math.Floor(point.Z)) & 1) * Vector3.One; // create a new vector.
}
