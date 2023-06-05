using OpenTK.Mathematics;

namespace INFOGR2023Template;

public class Intersection
{
    // Necessary to access the camera, debug, and scene instances.
    public Raytracer raytracer;
    public Intersection(Raytracer raytracer) => this.raytracer = raytracer;

    public bool FindClosestIntersection(Ray ray, out Vector3 intersectionPoint, out Primitive closestPrimitive)
    {
        intersectionPoint = ray.Direction * 500;
        closestPrimitive = null;
        float closestDistance = float.MaxValue;

        foreach (Primitive primitive in raytracer.Scene.Primitives)
        {
            if (primitive.GetType() != typeof(Triangle))
            {
                BoundingBox box = primitive.getBox();
                if (!box.intersectBox(ray)) continue;
            }

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
            if (ray.Origin == raytracer.Camera.Position)
                raytracer.Debug.DrawRays(raytracer.Camera.Position, intersectionPoint, Utilities.Ray.Primary);
        }

        return closestPrimitive != null;
    }

    /// <summary>
    /// // todo
    /// </summary>
    /// <param name="intersection">The closest intersection point.</param>
    /// <param name="direction">The light direction that is dynamically determined for each light source.</param>
    /// <param name="primitive">The primitive to check.</param>
    /// <returns></returns> // todo
    public bool Shadowed(Vector3 intersection, out Vector3 direction, Primitive primitive)
    {
        direction = Vector3.Zero;

        foreach (Light light in raytracer.Scene.Lights)
        {
            direction = Vector3.Normalize(light.Location - intersection);
            Ray ray = new(intersection, direction, 0);

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
        }

        return false;
    }

    public Vector3 TraceRay(Ray ray)
    {
        Vector3 color = Vector3.Zero;

        FindClosestIntersection(ray, out Vector3 closestIntersectionPoint, out Primitive closestPrimitive);

        if (closestPrimitive == null) return Vector3.UnitZ;

        // Ignore reflections if the primitive's reflectivity is disabled (0f).
        if (closestPrimitive.ReflectionCoefficient == 0f)
        {
            Vector3 normal = closestPrimitive.GetNormal(closestIntersectionPoint);
            Vector3 viewDirection = Vector3.Normalize(ray.Direction);

            if (!Shadowed(closestIntersectionPoint, out Vector3 lightDirection, closestPrimitive))
                return ShadeColor(new Vector3(0.3f, 0.3f, 0.3f),
                    lightDirection, viewDirection, normal,
                    (closestPrimitive is CheckeredPlane plane ? plane.GetCheckeredColor(closestIntersectionPoint) : closestPrimitive.GetColor()), 0.7f);

            return Vector3.Zero;
        }

        Vector3 point = Vector3.Zero;

        // Adjust for reflectivity if the primitive's reflectivity is enabled (>0f).
        if (closestPrimitive.ReflectionCoefficient > 0f)
        {
            Vector3 surfaceNormal = closestPrimitive.GetNormal(closestIntersectionPoint);
            Vector3 reflectionDirection = ReflectRay(Vector3.Normalize(ray.Direction), Vector3.Normalize(surfaceNormal));

            Ray reflectedRay = new(closestIntersectionPoint, Vector3.Normalize(reflectionDirection), 16);
            if (reflectedRay.MaxBounces > 0)
            {
                if (FindClosestIntersection(reflectedRay, out Vector3 reflectionPoint, out Primitive reflectPrimitive))
                {
                    if (reflectPrimitive != closestPrimitive)
                    {
                        reflectedRay.MaxBounces--;

                        color += (closestPrimitive is CheckeredPlane plane ? plane.GetCheckeredColor(reflectionPoint) : closestPrimitive.GetColor())
                                 * closestPrimitive.ReflectionCoefficient * TraceRay(reflectedRay);
                        color = Utilities.ResolveOutOfBounds(color);
                    }

                    point = reflectionPoint;
                    // Console.WriteLine(reflectionPoint);
                    if (reflectPrimitive == closestPrimitive)
                    {
                        Vector3 lightDirection = Vector3.Normalize(raytracer.Scene.Lights[0].Location - closestIntersectionPoint);
                        // color = ShadeColor(Vector3.One * 1f, lightDirection, Vector3.Normalize(ray.Direction), surfaceNormal,
                        //     (closestPrimitive is CheckeredPlane plane ? plane.GetCheckeredColor(reflectionPoint) : closestPrimitive.GetColor()), 0.7f);
                        // Console.WriteLine(reflectionPoint);
                        // color = Vector3.UnitZ * closestPrimitive.ReflectionCoefficient;
                    }

                    // TODO: fix reflection rays in debug window dat ze vrijwel allemaal naar de hoeken gaan.
                    // if (reflectedRay.Origin == closestIntersectionPoint) debug.DrawRays(closestIntersectionPoint, reflectionDirection * 500, Utilities.Ray.Reflection);
                }
            }
        }

        // if (color == Vector3.Zero)
        // {
        //     Vector3 lightDirection = Vector3.Normalize(scene.Lights[0].Location - closestIntersectionPoint);
        //     Vector3 normal = closestPrimitive.GetNormal(closestIntersectionPoint);
        //     color = ShadeColor(Vector3.One, lightDirection, ray.Direction, normal,
        //         (closestPrimitive is CheckeredPlane plane ? plane.GetCheckeredColor(point) : closestPrimitive.GetColor()), 0.7f);
        // }
        return color;
    }

    /// <summary>
    /// Calculates the corresponding reflect ray.
    /// </summary>
    public Vector3 ReflectRay(Vector3 direction, Vector3 normal) => direction - 2 * Vector3.Dot(direction, normal) * normal;

    /// <summary>
    /// Calculates the color of a pixel based on Phong's shading model.
    /// </summary>
    public Vector3 ShadeColor(Vector3 lightIntensity, Vector3 lightDirection, Vector3 viewDirection,
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
        Vector3 reflectionDirection = lightDirection - 2 * dot * normal;

        float specularPower = 50f;
        float specularCoefficient = (float)Math.Pow(Math.Max(0, Vector3.Dot(viewDirection, reflectionDirection)), specularPower);
        Vector3 specularColor = Vector3.Zero * 0.8f; // Ks

        // Combine both materials.
        Vector3 shadedColor = radiance * (diffuseCoefficient * diffuseColor) + radiance * (specularCoefficient * specularColor);

        // Add ambient lighting.
        shadedColor += ambientLightning;

        shadedColor = Utilities.ResolveOutOfBounds(shadedColor);

        return shadedColor;
    }
}
