using OpenTK.Mathematics;

namespace INFOGR2023Template;

public class Intersection
{
    // Necessary to access the camera, debug, and scene instances.
    public Raytracer raytracer;
    private readonly Random _random = new();
    public Intersection(Raytracer raytracer) => this.raytracer = raytracer;

    public bool FindClosestIntersection(Ray ray, out Vector3 intersectionPoint, out Primitive closestPrimitive)
    {
        intersectionPoint = Vector3.Zero;
        closestPrimitive = null;
        float closestDistance = float.MaxValue;

        foreach (Primitive primitive in raytracer.Scene.Primitives)
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

                // Draw the primary rays.
                if (ray.Origin == raytracer.Camera.Position)
                    raytracer.Debug.DrawRays(raytracer.Camera.Position, intersectionPoint, Utilities.Ray.Primary);
            }

            // Draw the primary rays.
            if (ray.Origin == raytracer.Camera.Position && closestPrimitive == null)
                raytracer.Debug.DrawRays(raytracer.Camera.Position, ray.Direction * 50, Utilities.Ray.Primary);
        }

        return closestPrimitive != null;
    }

    public Vector3 TraceRay(Ray ray, Vector3 mask, int bounceLimit)
    {
        Vector3 color = Vector3.Zero;

        FindClosestIntersection(ray, out Vector3 closestIntersectionPoint, out Primitive closestPrimitive);

        // Sets the sky color for the path tracer.
        // Inside of reflection, the sky is instead perceived as black; this is intentional!
        // Otherwise it'd be quite difficult to distinguish the mirrors from the sky, at least their top parts. 
        if (closestPrimitive == null) return new Vector3(0.15f, 0.15f, 0.15f);

        // Adjust for reflectivity if the primitive's reflectivity is enabled (>0f).
        Vector3 surfaceNormal = (closestPrimitive.GetType() == typeof(Plane) || closestPrimitive.GetType() == typeof(CheckeredPlane))
                ? -closestPrimitive.GetNormal(closestIntersectionPoint) : closestPrimitive.GetNormal(closestIntersectionPoint);
        Vector3 reflectionDirection = Vector3.Normalize(ReflectRay(ray.Direction, surfaceNormal));

        // Randomly mix the reflection directio with a randomly selected double.
        Vector3 lerp = Vector3.Lerp(reflectionDirection, Uniform((float)_random.NextDouble(), (float)_random.NextDouble()), 1 - closestPrimitive.ReflectionCoefficient);

        if (Vector3.Dot(surfaceNormal, lerp) < 0)
            lerp = -lerp;

        reflectionDirection = lerp;

        Ray reflectionRay = new(closestIntersectionPoint, reflectionDirection);

        if (bounceLimit > 0)
        {
            if (FindClosestIntersection(reflectionRay, out Vector3 reflectionPoint, out Primitive reflectedPrimitive))
            {
                if (closestPrimitive is Sphere sphere) color += sphere.Emission * mask;
                mask *= (closestPrimitive is CheckeredPlane plane ? plane.GetCheckeredColor(closestIntersectionPoint) : closestPrimitive.GetColor());

                color += TraceRay(reflectionRay, mask, bounceLimit - 1);

                if (reflectionRay.Origin == closestIntersectionPoint)
                    raytracer.Debug.DrawRays(closestIntersectionPoint, reflectionPoint, Utilities.Ray.Reflection);
            }

            if (reflectionRay.Origin == closestIntersectionPoint && closestIntersectionPoint == Vector3.Zero)
                raytracer.Debug.DrawRays(closestIntersectionPoint, reflectionRay.Direction * 50, Utilities.Ray.Reflection);
        }

        return color;
    }

    // Reference: https://github.com/RobertBeckebans/OpenGL-PathTracer/blob/master/PathTracer/src/shaders/Progressive/PathTraceFrag.glsl#L397
    public Vector3 Uniform(float u1, float u2)
    {
        float z = 1 - 2 * u1;
        float r = (float)Math.Sqrt(Math.Max(0, 1 - z * z));
        float phi = MathHelper.TwoPi * u2;
        float x = (float)(r * MathHelper.Cos(phi));
        float y = (float)(r * MathHelper.Sin(phi));

        return new Vector3(x, y, z);
    }

    /// <summary>
    /// Calculates the corresponding reflect ray.
    /// </summary>
    public Vector3 ReflectRay(Vector3 direction, Vector3 normal) => direction - 2 * Vector3.Dot(direction, normal) * normal;
}
