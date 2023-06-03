using System.Numerics;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace INFOGR2023Template;

public class Plane : Primitive
{
    public Vector3 Normal { get; set; }
    public float S { get; set; } //distance to origin

    public Vector3 Color;

    public Vector3 DiffuseColor { get; set; }
    public Vector3 SpecularColor { get; set; }
    public float SpecularPower { get; set; }
    public float Reflectivity { get; set; }

    public Plane(Vector3 normal, float s, Vector3 color, Vector3 diffusecolor, Vector3 specularcolor, float specularpower, float reflectivity)
    {
        Normal = normal.Normalized();
        S = s;
        Color = color;
        DiffuseColor = diffusecolor;
        SpecularColor = specularcolor;
        SpecularPower = specularpower;
        Reflectivity = reflectivity;
    }

    public Plane() { }

    public override Vector3 GetNormal(Vector3 point) => Normal;
    public override Vector3 GetColor() => Color;

    public override bool HitRay(Ray ray, out Vector3 intersect)
    {
        float denom = Vector3.Dot(ray.Direction, Normal);
        if (denom == 0)
        {
            //parallel 
            intersect = Vector3.Zero;
            return false;
        }
        float t = (Vector3.Dot(ray.Origin, Normal) - S) / denom;
        if (t < 0)
        {
            //Voorbij de camera
            intersect = Vector3.Zero;
            return false;
        }
        intersect = ray.Origin + t * ray.Direction;
        return true;
    }

    public override bool IntersectsWithLight(Vector3 intersectionPoint, Vector3 lightPosition, out Vector3 direction)
    {
        direction = lightPosition - intersectionPoint;
        Ray ray = new(intersectionPoint, direction);

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

    public override float GetReflectionCoefficient()
    {
        // Return the reflection coefficient for the sphere
        // Adjust this value as desired (0 for no reflection, 1 for full reflection)
        return Reflectivity; // Example reflection coefficient
    }
}

public class CheckeredPlane : Plane
{
    public Vector3 GetCheckeredColor(Vector3 point) => ((int)(Math.Floor(2 * point.X) + Math.Floor(point.Z)) & 1) * Vector3.One; // create a new vector.
}
