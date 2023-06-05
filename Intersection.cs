using OpenTK.Mathematics;

namespace INFOGR2023Template;

public class Intersection
{
    private static readonly string[] _path = 
    {
        "C:\\Users\\Data Impact\\source\\repos\\INFOGR2023\\skybox\\back.png",
        "C:\\Users\\Data Impact\\source\\repos\\INFOGR2023\\skybox\\bottom.png",
        "C:\\Users\\Data Impact\\source\\repos\\INFOGR2023\\skybox\\front.png",
        "C:\\Users\\Data Impact\\source\\repos\\INFOGR2023\\skybox\\left.png",
        "C:\\Users\\Data Impact\\source\\repos\\INFOGR2023\\skybox\\right.png",
        "C:\\Users\\Data Impact\\source\\repos\\INFOGR2023\\skybox\\top.png"
    };
    // Necessary to access the camera, debug, and scene instances.
    public Raytracer raytracer;
    public static Skybox sky = new(_path);

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

    public bool Shadowed(Vector3 light, Vector3 intersection, Primitive primitive)
    {
        Ray ray = new(intersection, Vector3.Normalize(light - intersection), 0);

        foreach (Primitive p in raytracer.Scene.Primitives)
        {
            if (p == primitive) continue;
            if (p.HitRay(ray, out Vector3 intersect))
            {
                // The intersection point is shadowed by the sphere
                if (p.GetType() == typeof(Sphere))
                    raytracer.Debug.DrawRays(intersection, intersect, Utilities.Ray.Shadow);

                return true;
            }
        }

        return false;
    }

    public Vector3 TraceRay(Ray ray)
    {
        Vector3 color = Vector3.Zero;

        FindClosestIntersection(ray, out Vector3 closestIntersectionPoint, out Primitive closestPrimitive);

        if (closestPrimitive == null)
        {
            color = sky.GetColor(ray.Direction.X, ray.Direction.Y, ray.Direction.Z);

            return color;
        }

        // Ignore reflections if the primitive's reflectivity is disabled (0f).
        if (closestPrimitive.ReflectionCoefficient == 0f)
        {
            Vector3 normal = (closestPrimitive.GetType() == typeof(Plane) || closestPrimitive.GetType() == typeof(CheckeredPlane))
                ? -closestPrimitive.GetNormal(closestIntersectionPoint) : closestPrimitive.GetNormal(closestIntersectionPoint);
            Vector3 viewDirection = Vector3.Normalize(ray.Direction);

            return ShadeColor(raytracer.Scene, closestPrimitive, closestIntersectionPoint, viewDirection, normal,
                (closestPrimitive is CheckeredPlane plane ? plane.GetCheckeredColor(closestIntersectionPoint) : closestPrimitive.GetColor()));
        }

        // Adjust for reflectivity if the primitive's reflectivity is enabled (>0f).
        if (closestPrimitive.ReflectionCoefficient > 0f)
        {
            Vector3 surfaceNormal = (closestPrimitive.GetType() == typeof(Plane) || closestPrimitive.GetType() == typeof(CheckeredPlane))
                ? -closestPrimitive.GetNormal(closestIntersectionPoint) : closestPrimitive.GetNormal(closestIntersectionPoint);
            Vector3 reflectionDirection = Vector3.Normalize(ReflectRay(ray.Direction, surfaceNormal));

            Ray reflectionRay = new(closestIntersectionPoint, reflectionDirection, 3);
            if (reflectionRay.MaxBounces > 0)
            {
                if (FindClosestIntersection(reflectionRay, out Vector3 reflectionPoint, out Primitive reflectedPrimitive))
                {

                    reflectionRay.MaxBounces--;

                    color += (reflectedPrimitive is CheckeredPlane plane ? plane.GetCheckeredColor(reflectionPoint) : reflectedPrimitive.GetColor())
                             * closestPrimitive.ReflectionCoefficient * TraceRay(reflectionRay);

                    if (reflectionRay.Origin == closestIntersectionPoint)
                        raytracer.Debug.DrawRays(closestIntersectionPoint, reflectionPoint, Utilities.Ray.Reflection);
                }
                else
                {
                    color = sky.GetColor(reflectionRay.Direction.X, reflectionRay.Direction.Y, reflectionRay.Direction.Z);
                }

                if (reflectionRay.Origin == closestIntersectionPoint && closestIntersectionPoint == Vector3.Zero)
                    raytracer.Debug.DrawRays(closestIntersectionPoint, reflectionRay.Direction * 50, Utilities.Ray.Reflection);
            }

            return color;
        }

        return color;
    }

    /// <summary>
    /// Calculates the corresponding reflect ray.
    /// </summary>
    public Vector3 ReflectRay(Vector3 direction, Vector3 normal) => direction - 2 * Vector3.Dot(direction, normal) * normal;

    /// <summary>
    /// Calculates the color of a pixel based on Phong's shading model.
    /// </summary>
    public Vector3 ShadeColor(Scene scene, Primitive closestPrimitive, Vector3 intersection, Vector3 viewDirection, Vector3 normal, Vector3 diffuseColor)
    {
        Vector3 shadedColor = Vector3.Zero;
        Vector3 ambientLighting = diffuseColor * new Vector3(0.1f, 0.1f, 0.1f);

        foreach (Light light in scene.Lights)
        {
            if (!Shadowed(light.Location, intersection, closestPrimitive))
            {
                Vector3 direction = Vector3.Normalize(light.Location - intersection);
                float r = Vector3.Distance(light.Location, intersection);

                Vector3 radiance = light.Intensity / (r * r);

                // Determine specifics for diffuse materials.
                float dot = Vector3.Dot(normal, direction);
                float diffuseCoefficient = Math.Max(0, dot);
                Vector3 diffuse = diffuseCoefficient * diffuseColor;

                // Determine specifics for specular (glossy) materials.
                Vector3 reflectionDirection = direction - 2 * dot * normal;

                float specularPower = 50f;
                float specularCoefficient = (float)Math.Pow(Math.Max(0, Vector3.Dot(viewDirection, reflectionDirection)), specularPower);
                Vector3 specular = specularCoefficient * Vector3.One;

                // Combine both materials.
                shadedColor += radiance * (diffuse + specular);
            }
        }

        // Add ambient lighting.
        shadedColor += ambientLighting;

        shadedColor = Utilities.ResolveOutOfBounds(shadedColor);

        return shadedColor;
    }
}
