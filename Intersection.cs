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
        // direction = Vector3.Zero;

        // foreach (Light light in scene.Lights)
        // {
        Light light = scene.Lights[0];
        direction = Vector3.Normalize(light.Location - intersection);
        Ray ray = new(intersection, direction);

        foreach (Primitive p in scene.Primitives)
        {
            if (p == primitive) continue;
            if (p.HitRay(ray, out Vector3 intersect))
            {
                // Check if the intersection point is between the intersection point and the light position
                Vector3 intersectToLight = light.Location - intersect;
                float distanceToLight = intersectToLight.Length;

                // Check if the distance to the light is smaller than the distance to the intersection point
                if (distanceToLight == intersectToLight.Length - 0.000001f)
                {
                    // The intersection point is shadowed by the sphere
                    if (p.GetType() == typeof(Sphere))
                        debug.DrawRays(intersection, intersection + Vector3.One, Utilities.Ray.Shadow);

                    return true;
                }
            }
        }
        // }

        return false;
    }

    public static Vector3 TraceRay(Debug debug, Camera camera, Ray ray, Scene scene, int maxbounce)
    {
        Vector3 color = Vector3.Zero;

        FindClosestIntersection(scene, debug, camera, ray, out Vector3 closestIntersectionPoint, out Primitive closestPrimitive);

        if (closestPrimitive == null) return Vector3.UnitZ;

        // Ignore reflections if the primitive's reflectivity is disabled (0f).
        if (closestPrimitive.ReflectionCoefficient != 1f)
        {
            Vector3 normal = closestPrimitive.GetNormal(closestIntersectionPoint);

            if (!Shadowed(scene, debug, closestIntersectionPoint, out Vector3 lightDirection, closestPrimitive))
                return ShadeColor(new Vector3(0.3f, 0.3f, 0.3f), 
                    lightDirection, Vector3.Normalize(ray.Direction), normal, closestPrimitive.GetColor(), 0.7f);
            
            return Vector3.Zero;
        }
        
        // Adjust for reflectivity if the primitive's reflectivity is enabled (>0f).
        if (closestPrimitive.ReflectionCoefficient == 1f)
        {
            Vector3 surfaceNormal = closestPrimitive.GetNormal(closestIntersectionPoint);
            Vector3 reflectionDirection = ReflectRay(Vector3.Normalize(ray.Direction), Vector3.Normalize(surfaceNormal));
            
            Ray reflectedRay = new(closestIntersectionPoint, Vector3.Normalize(reflectionDirection));
            if (maxbounce > 0)
            { 
                color += (closestPrimitive.GetColor() * closestPrimitive.ReflectionCoefficient) * TraceRay(debug, camera, reflectedRay, scene, maxbounce - 1);
                color = Utilities.ResolveOutOfBounds(color);

                // debug.DrawRays(closestIntersectionPoint, reflectionDirection * 500, Utilities.Ray.Reflection);
            }
        }

        return color;
    }
    
    public static Vector3 ReflectRay(Vector3 direction, Vector3 normal) => direction - 2 * Vector3.Dot(direction, normal) * normal;

    public static Vector3 ShadeColor(Vector3 lightIntensity, Vector3 lightDirection, Vector3 viewDirection,
        Vector3 normal, Vector3 diffuseColor, float r)
    {
        Vector3 ambientLightning = diffuseColor * new Vector3(0.3f, 0.3f, 0.3f);

        Vector3 radiance = lightIntensity * (1 / (r * r));

        // Determine specifics for diffuse materials.
        float dot = Vector3.Dot(normal, lightDirection);
        if (dot < 0) // > 90dg
            return ambientLightning;

        float diffuseCoefficient = Math.Max(0, dot);

        // Determine specifics for specular (glossy) materials.
        Vector3 reflectionDirection = lightDirection - 2 * (dot) * normal;

        float specularPower = 10f;
        float specularCoefficient = (float)Math.Pow(Math.Max(0, Vector3.Dot(viewDirection, reflectionDirection)), specularPower);
        float k = 1f;
        Vector3 specularColor = new(k, k, k);

        // Combine both materials.
        Vector3 shadedColor = radiance * ((diffuseCoefficient * diffuseColor) + (specularCoefficient * specularColor));

        // Add ambient lighting.
        shadedColor += ambientLightning;

        shadedColor = Utilities.ResolveOutOfBounds(shadedColor);

        return shadedColor;
    }
}
