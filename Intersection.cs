using OpenTK.Mathematics;

namespace INFOGR2023Template;

public class Intersection
{
    public static bool FindClosestIntersection(Scene scene, Debug debug, Camera camera, Ray ray, out Vector3 intersectionPoint, out Primitive closestPrimitive)
    {
        intersectionPoint = ray.Direction * 500;
        closestPrimitive = null;
        float closestDistance = float.MaxValue;

        foreach (Primitive primitive in scene.Primitives)
        {
            if (primitive.HitRay(ray, out Vector3 intersect))
            {
                float distance = Vector3.Distance(ray.Origin, intersect) - 0.001f;

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    intersectionPoint = intersect;
                    closestPrimitive = primitive;
                }
            }

            // Draw the primary rays.
            debug.DrawRays(camera.Position, intersectionPoint, Utilities.Ray.Primary);
        }

        return closestPrimitive != null;
    }

    /// <summary>
    /// // todo
    /// </summary>
    /// <param name="scene">The scene from which to pull the primitives.</param>
    /// <param name="debug">The debug window on which to draw the 2D shadow rays.</param>
    /// <param name="intersection">The closest intersection point.</param>
    /// <param name="direction">The light direction that is dynamically determined for each light source.</param>
    /// <param name="primitive">The primitive to check.</param>
    /// <returns></returns> // todo
    public static bool Shadowed(Scene scene, Debug debug, Vector3 intersection, out Vector3 direction, Primitive primitive)
    {
        direction = Vector3.Zero;

        foreach (Light light in scene.Lights)
        {
            direction = Vector3.Normalize(light.Location - intersection);
            Ray ray = new(intersection, direction);

            if (primitive.HitRay(ray, out Vector3 intersect))
            {
                // Check if the intersection point is between the intersection point and the light position
                Vector3 intersectToLight = light.Location - intersect;
                float distanceToLight = intersectToLight.Length;

                // Check if the distance to the light is smaller than the distance to the intersection point
                if (distanceToLight == intersectToLight.Length - 0.000001f)
                {
                    // The intersection point is shadowed by the sphere
                    if (primitive.GetType() == typeof(Sphere))
                        debug.DrawRays(intersection, intersection + Vector3.One, Utilities.Ray.Shadow);

                    return true;
                }
            }
        }

        return false;
    }

    public static Vector3 TraceRay(Debug debug, Camera camera, Ray ray, Scene scene, int maxbounce)
    {
        Vector3 color = Vector3.Zero;
        FindClosestIntersection(scene, debug, camera, ray, out Vector3 closestIntersectionPoint, out Primitive closestPrimitive);

        if (closestPrimitive == null) return Vector3.UnitZ;
        // if (closestPrimitive.GetType() == typeof(Plane))
        if (closestPrimitive.ReflectionCoefficient == 0f)
        {
            Vector3 normal = closestPrimitive.GetNormal(closestIntersectionPoint);
            Vector3 viewDirection = Vector3.Normalize(ray.Direction);

            if (!Shadowed(scene, debug, closestIntersectionPoint, out Vector3 lightDirection, closestPrimitive))
            {
                // if (closestPrimitive.GetType() == typeof(Plane))
                // {
                //     CheckeredPlane plane = (CheckeredPlane)closestPrimitive;
                //     color = Utilities.ShadeColor(new Vector3(0.3f, 0.3f, 0.3f), lightDirection, viewDirection, normal,
                //         plane.GetCheckeredColor(closestIntersectionPoint), 0.7f);
                // }
                // else
                // {
                    color = Utilities.ShadeColor(new Vector3(0.3f, 0.3f, 0.3f), lightDirection, viewDirection, normal,
                        closestPrimitive.GetColor(), 0.7f);
                // }

                return color;
            }
            
            return Vector3.Zero;
        }
        
        if (closestPrimitive.ReflectionCoefficient == 1f)
        {
            Vector3 surfaceNormal = closestPrimitive.GetNormal(closestIntersectionPoint);
            Vector3 reflectionDirection = ReflectRay(ray.Direction, surfaceNormal); 
            Ray reflectedRay = new(closestIntersectionPoint, Vector3.Normalize(reflectionDirection));
            if (maxbounce > 0)
            { 
                color += closestPrimitive.GetColor() * closestPrimitive.ReflectionCoefficient * TraceRay(debug, camera, reflectedRay, scene, maxbounce - 1);
                if (color.X > 1f) color.X = 1f;
                if (color.Y > 1f) color.Y = 1f;
                if (color.Z > 1f) color.Z = 1f;

                // debug.DrawRays(closestIntersectionPoint, reflectionDirection * 500, Utilities.Ray.Reflection);
            }
        }

        return color;
    }
    
    public static Vector3 ReflectRay(Vector3 incidentDirection, Vector3 surfaceNormal)
    {
        return incidentDirection - 2 * Vector3.Dot(incidentDirection, surfaceNormal) * surfaceNormal;
    }
}
