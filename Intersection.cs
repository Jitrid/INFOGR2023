using OpenTK.Mathematics;

namespace INFOGR2023Template;

public class Intersection
{
    public static bool Shadowed(Vector3 intersectionPoint, Vector3 lightPosition, Vector3 direction, List<Primitive> objects, Primitive sender, Debug debug, int i)
    {
        Ray ray = new(intersectionPoint, direction);

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
                    if (o.GetType() == typeof(Sphere))
                        debug.DrawRays(intersectionPoint, intersectionPoint + Vector3.One, Utilities.Ray.Shadow, i);

                    return true;
                }
            }
        }

        // Find the intersection point of the ray with the plane
        return false;
    }

    public static bool FindClosestIntersection(Debug debug, Camera camera, Ray ray, List<Primitive> objects, out Vector3 intersectionPoint, out Primitive closestPrimitive, int i)
    {
        intersectionPoint = ray.Direction * 500;
        closestPrimitive = null;
        float closestDistance = float.MaxValue;

        foreach (Primitive primitive in objects)
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

            if (closestPrimitive != null && primitive.GetType() == typeof(Sphere))
                debug.DrawRays(camera.Position, intersectionPoint, Utilities.Ray.Primary, i);
        }

        return closestPrimitive != null;
    }

    public static Vector3 TraceRay(Debug debug, Camera camera, Ray ray, Scene s, int maxbounce, int i)
    {
        Vector3 color = Vector3.Zero;
        FindClosestIntersection(debug, camera, ray, s.Primitives, out Vector3 closestIntersectionPoint, out Primitive closestPrimitive, i);

        if (closestPrimitive == null) return Vector3.UnitZ;
        // if (closestPrimitive.GetType() == typeof(Plane))
        if (closestPrimitive.GetReflectionCoefficient() == 0f)
        {
            Vector3 lightDirection = Vector3.Normalize(s.Lights[0].Location - closestIntersectionPoint);
            Vector3 normal = closestPrimitive.GetNormal(closestIntersectionPoint);
            Vector3 viewDirection = Vector3.Normalize(ray.Direction);

            if (!Shadowed(closestIntersectionPoint, s.Lights[0].Location, lightDirection, s.Primitives, closestPrimitive, debug, i))
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
                // return closestPrimitive.GetColor();
            }
            
            return Vector3.Zero;
        }
        
        if (closestPrimitive.GetReflectionCoefficient() == 1f)
        {
            
            Vector3 surfaceNormal = closestPrimitive.GetNormal(closestIntersectionPoint);
            Vector3 reflectionDirection = ReflectRay(ray.Direction, surfaceNormal); 
            Ray reflectedRay = new(closestIntersectionPoint, Vector3.Normalize(reflectionDirection));
            if (maxbounce > 0)
            { 
                color += closestPrimitive.GetColor() * closestPrimitive.GetReflectionCoefficient() * TraceRay(debug, camera, reflectedRay, s, maxbounce - 1, i);
                if (color.X > 1f) color.X = 1f;
                if (color.Y > 1f) color.Y = 1f;
                if (color.Z > 1f) color.Z = 1f;

                debug.DrawRays(closestIntersectionPoint, reflectionDirection * 500, Utilities.Ray.Reflection, i);
            }
        }

        Vector3 dir = Vector3.Normalize(s.Lights[0].Location - closestIntersectionPoint);

        return color;
        // return Utilities.ShadeColor(Vector3.One, dir, Vector3.Normalize(ray.Direction), 
        //     closestPrimitive.GetNormal(closestIntersectionPoint), closestPrimitive.GetColor(), 0.25f, 0.1f, 1f);
    }
    
    public static Vector3 ReflectRay(Vector3 incidentDirection, Vector3 surfaceNormal)
    {
        return incidentDirection - 2 * Vector3.Dot(incidentDirection, surfaceNormal) * surfaceNormal;
    }
}
