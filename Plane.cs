using OpenTK.Mathematics;

namespace INFOGR2023Template;

public class Plane : Primitive
{
    public Vector3 Normal;

    public Plane(float[] vertices, float distance, Vector3 normal) : base(vertices, distance)
    {
        this.Normal = normal;
    }

    public static Plane CreatePlane(float width, float height, float x, float y, float z)
    {
        float[] vertices =
        {
            -width / 2, 0, -height / 2, // -50, 0, -50 BL
            width / 2, 0, -height / 2,  // 50, 0, -50  TL
            width / 2, 0, height / 2,   // 50, 0, 50   TR
            -width / 2, 0, height / 2   // -50, 0, 50  BR
        };
        float distance = (float)Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2));

        // Check if the ray intersects the plane
        Vector3 planeNormal;
        Vector3 v0 = new(vertices[0], vertices[1], vertices[2]); // BL
        Vector3 v1 = new(vertices[3], vertices[4], vertices[5]); // TL
        Vector3 v2 = new(vertices[6], vertices[7], vertices[8]); // TR
        planeNormal = Vector3.Cross(v1 - v0, v2 - v0);
        planeNormal = Vector3.Normalize(planeNormal);

        return new Plane(vertices, distance, planeNormal);
    }

    public static bool IntersectPlane(Ray ray, Plane plane, out float intersectionDistance, out Vector3 intersectionPoint)
    {
        // Get the vertices of the plane
        float[] vertices = plane.GetVertices();

        float planeD = -plane.distance; // The distance of the plane from the origin (can be adjusted as needed)

        // Calculate the denominator of the ray-plane intersection formula
        float denominator = Vector3.Dot(plane.Normal, ray.Direction);

        // If the denominator is zero, the ray is parallel to the plane and has no intersection
        if (Math.Abs(denominator) < float.Epsilon)
        {
            intersectionDistance = 0.0f;
            intersectionPoint = Vector3.Zero;
            return false;
        }

        // Calculate the distance from the ray origin to the intersection point
        float numerator = Vector3.Dot(plane.Normal, ray.Origin) + planeD;
        intersectionDistance = numerator / denominator;

        // Console.WriteLine(intersectionDistance);

        // Check if the intersection point is in front of the ray
        if (intersectionDistance >= 0.0f)
        {
            // Calculate the intersection point
            intersectionPoint = ray.Origin + ray.Direction * intersectionDistance;

            // Check if the intersection point is within the plane's bounds
            float minX = float.MaxValue;
            float maxX = float.MinValue;
            float minZ = float.MaxValue;
            float maxZ = float.MinValue;

            for (int i = 0; i < vertices.Length; i += 3)
            {
                Vector3 vertex = new(vertices[i], vertices[i + 1], vertices[i + 2]);

                // Update the minimum and maximum coordinates
                minX = Math.Min(minX, vertex.X);
                maxX = Math.Max(maxX, vertex.X);
                minZ = Math.Min(minZ, vertex.Z);
                maxZ = Math.Max(maxZ, vertex.Z);
            }

            // Check if the intersection point is within the bounds of the plane
            if (intersectionPoint.X >= minX && intersectionPoint.X <= maxX &&
                intersectionPoint.Z >= minZ && intersectionPoint.Z <= maxZ)
            {
                return true;
            }
        }

        intersectionPoint = Vector3.Zero;
        return false;
    }
}