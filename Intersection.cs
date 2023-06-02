using System.Reflection.Metadata;
using OpenTK.Mathematics;

namespace INFOGR2023Template;

public class Intersection
{
    public static bool Shadowed(Vector3 intersectionPoint, Vector3 lightPosition, out Vector3 direction, List<Primitive> objects, Primitive sender)
    {
        direction = lightPosition - intersectionPoint;
        Ray ray = new Ray(intersectionPoint, direction);

        foreach (Primitive o in objects)
        {
            if (o == sender) continue;
            if (o.HitRay(ray, out Vector3 intersect))
            {
                
                // Check if the intersection point is between the intersection point and the light position
                Vector3 intersectToLight = lightPosition - intersect;
                float distanceToLight = intersectToLight.Length;

                // Check if the distance to the light is smaller than the distance to the intersection point
                if (distanceToLight == intersectToLight.Length- 0.000001f)
                {
                    // The intersection point is shadowed by the sphere
                    return true;
                }
            }

        }

        // Find the intersection point of the ray with the plane
        return false;
    }

    public static bool FindClosestIntersection(Ray ray, List<Primitive> objects, out Vector3 intersectionPoint, out Primitive closestPrimitive)
    {
        intersectionPoint = Vector3.Zero;
        closestPrimitive = null;
        float closestDistance = float.MaxValue;

        foreach (Primitive primitive in objects)
        {
            if (primitive.HitRay(ray, out Vector3 intersect))
            {
                float distance = Vector3.Distance(ray.Origin, intersect);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    intersectionPoint = intersect;
                    closestPrimitive = primitive;
                }
            }
        }

        return closestPrimitive != null;
    }

    public static bool TraceRay(Ray ray, List<Primitive> objects, Light light, int maxDepth, out int color)
    {
        color = 0;

        // Check if maximum recursion depth is reached
        if (maxDepth <= 0)
            return false;

        if (FindClosestIntersection(ray, objects, out Vector3 intersectionPoint, out Primitive closestPrimitive))
        {
            Vector3 surfaceNormal = closestPrimitive.GetNormal(intersectionPoint);
            Vector3 reflectionDirection = ReflectRay(ray.Direction, surfaceNormal);
            Ray reflectedRay = new Ray(intersectionPoint, reflectionDirection);

            // Check if the point is shadowed by other objects
            if (Intersection.Shadowed(intersectionPoint, light.Location, out _, objects, closestPrimitive))
            {
                // Apply shadow color or shading here
                color = 0;
                return true;
            }

            // Recursive call to trace the reflected ray
            if (TraceRay(reflectedRay, objects, light, maxDepth - 1, out int reflectedColor))
            {
                // Apply reflection coefficient or material properties here
                float reflectionCoefficient = closestPrimitive.GetReflectionCoefficient();
                color += (int)(reflectionCoefficient * reflectedColor);
            }

            // Add other shading calculations here (e.g., diffuse, specular, etc.)

            // Add the final color contribution of the closest object
            color += closestPrimitive.GetColor();

            return true;
        }

        return false;
    }







    //invalhoek = uitschiethoek
    public static Vector3 ReflectRay(Vector3 incidentDirection, Vector3 surfaceNormal)
    {
        return incidentDirection - 2 * Vector3.Dot(incidentDirection, surfaceNormal) * surfaceNormal;
    }

      

}
