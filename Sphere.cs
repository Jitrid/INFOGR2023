using Vector3 = OpenTK.Mathematics.Vector3;

namespace INFOGR2023Template;

public class Sphere
{
    public Vector3 Center { get; }
    public float Radius { get; }

    public Sphere(Vector3 center, float radius)
    {
        Center = center;
        Radius = radius;
    }

    public bool HitRay(Ray ray, Camera camera, out Vector3 intersect)
    {
        intersect = Vector3.Zero;

        float a = Vector3.Dot(ray.Direction, ray.Direction);
        float b = 2 * (Vector3.Dot(camera.Position, ray.Direction) - Vector3.Dot(this.Center, ray.Direction));
        float c = Vector3.Dot(this.Center, this.Center) + Vector3.Dot(camera.Position, camera.Position) - 2 * Vector3.Dot(this.Center, camera.Position) - (this.Radius * this.Radius);
        float discriminant = b * b - 4 * a * c;

        if (discriminant < 0)
        {
            intersect = Vector3.Zero;
            return false;
        }

        float t1 = (float)(-b - Math.Sqrt(discriminant)) / 2 * a;
        float t2 = (float)(-b + Math.Sqrt(discriminant)) / 2 * a;

        if (t1 > 0)
        {
            intersect = ray.Origin + t1 * ray.Direction;
            return true;
        }
        if (t2 > 0)
        {
            intersect = ray.Origin + t2 * ray.Direction;
            return true;
        }

        return false;
    }
}
