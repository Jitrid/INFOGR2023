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

    public static bool FindClosestIntersection(Debug debug, Camera camera, Ray ray, List<Primitive> objects, out Vector3 intersectionPoint, out Primitive closestPrimitive, int i)
    {
        intersectionPoint = Vector3.Zero;
        closestPrimitive = null;
        float closestDistance = float.MaxValue;

        foreach (Primitive primitive in objects)
        {
            if (primitive.HitRay(ray, out Vector3 intersect))
            {
                if (primitive.GetType() == typeof(Sphere))
                {
                    debug.DrawRays(camera.Position, intersect, Utilities.Ray.Primary, i);
                }

                float distance = Vector3.Distance(ray.Origin, intersect) - 0.001f;

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

    public static Vector3 TraceRay(Debug debug, Camera camera, Ray ray, Scene s, int maxbounce, int i)
    {
        Vector3 color = Vector3.Zero;
        FindClosestIntersection(debug, camera, ray, s.Primitives, out Vector3 closestIntersectionPoint, out Primitive closestPrimitive, i);

        if (closestPrimitive == null) return Vector3.UnitZ;
        if (closestPrimitive.GetType() == typeof(Plane))
        {
            Vector3 lightDirection = Vector3.Normalize(s.Lights[0].Location - closestIntersectionPoint);
            Vector3 normal = closestPrimitive.GetNormal(closestIntersectionPoint);
            Vector3 viewDirection = Vector3.Normalize(ray.Direction);

            if (!Shadowed(closestIntersectionPoint, s.Lights[0].Location, out lightDirection, s.Primitives,
                    closestPrimitive))
            {

                color = Utilities.ShadeColor(new Vector3(1, 1, 1), lightDirection, viewDirection, normal,
                    closestPrimitive.GetColor(), 0.25f, (float).1, 1f);
            
                return color;
            }
            else
            {
                return Vector3.Zero;
            }
        }
        
        if (closestPrimitive.GetReflectionCoefficient() == 1)
        {
            
            Vector3 surfaceNormal = closestPrimitive.GetNormal(closestIntersectionPoint);
            Vector3 reflectionDirection = ReflectRay(ray.Direction, surfaceNormal); 
            Ray reflectedRay = new Ray(closestIntersectionPoint, Vector3.Normalize(reflectionDirection));
            if (maxbounce > 0)
            { 
                color += closestPrimitive.GetColor() * closestPrimitive.GetReflectionCoefficient() * TraceRay(debug, camera, reflectedRay, s, maxbounce - 1, i);
                if (color.X > 1f) color.X = 1f;
                if (color.Y > 1f) color.Y = 1f;
                if (color.Z > 1f) color.Z = 1f;
            }
           
            
            
        }
        return color;
        //else
        //{
        //    Vector3 lightDirection = Vector3.Normalize(s.Lights[0].Location - closestIntersectionPoint);
        //    Vector3 normal = closestPrimitive.GetNormal(closestIntersectionPoint);
        //    Vector3 viewDirection = Vector3.Normalize(ray.Direction);

        //    if (!Shadowed(closestIntersectionPoint, s.Lights[0].Location, out lightDirection, s.Primitives,
        //            closestPrimitive))
        //    {

        //        color = Utilities.ShadeColor(new Vector3(1, 1, 1), lightDirection, viewDirection, normal,
        //            closestPrimitive.GetColor(), 0.25f, (float).1, 1f);
        //        return color;
        //    }
        //    else
        //    {
        //        return Vector3.Zero;
        //    }
        //}






















        //color = Vector3.Zero;

        //// Check if maximum recursion depth is reached
        //if (maxDepth <= 0)
        //    return false;

        //if (FindClosestIntersection(ray, objects, out Vector3 intersectionPoint, out Primitive closestPrimitive))
        //{
        //    Vector3 surfaceNormal = closestPrimitive.GetNormal(intersectionPoint);
        //    Vector3 reflectionDirection = ReflectRay(ray.Direction, surfaceNormal);
        //    Ray reflectedRay = new Ray(intersectionPoint, reflectionDirection);

        //    // Check if the point is shadowed by other objects
        //    //if (Intersection.Shadowed(intersectionPoint, light.Location, out _, objects, closestPrimitive))
        //    //{
        //    //    // Apply shadow color or shading here
        //    //    color = Vector3.Zero;
        //    //    return true;
        //    //}
        //    Vector3 shadedColor = closestPrimitive.GetColor();
        //    // Recursive call to trace the reflected ray
        //    if (Intersection.TraceRay(reflectedRay, objects, light, 50 - 1, out Vector3 reflectedColor))
        //    {

        //        float reflectionCoefficient = closestPrimitive.GetReflectionCoefficient();
        //        shadedColor += reflectionCoefficient * reflectedColor;
        //        color += closestPrimitive.GetColor() * shadedColor;
        //    }


        //    // Add other shading calculations here (e.g., diffuse, specular, etc.)

        //    // Add the final color contribution of the closest object


        //    return true;
        //}

        //return false;
    }







    //invalhoek = uitschiethoek
    public static Vector3 ReflectRay(Vector3 incidentDirection, Vector3 surfaceNormal)
    {
        return incidentDirection - 2 * Vector3.Dot(incidentDirection, surfaceNormal) * surfaceNormal;
    }

      

}
