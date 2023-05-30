using OpenTK.Mathematics;

namespace INFOGR2023Template;

public class Sphere : Intersectable
{
    public Vector3 Center { get; }
    public float Radius { get; }

    public Sphere(Vector3 center, float radius)
    {
        Center = center;
        Radius = radius;
    }

    public override bool HitOrMiss(Ray ray, float tmin, float tmax, out Intersection intersection)
    {
        intersection = new Intersection();

        Vector3 distanceToCenter = ray.Origin - this.Center;
        float a = (float)Math.Pow(ray.Direction.Length, 2);
        float b = Vector3.Dot(distanceToCenter, ray.Direction);
        float c = (float)Math.Pow(distanceToCenter.Length, 2) - this.Radius * this.Radius;
        
        float discriminant = (float)(Math.Pow(b, 2) - 4 * a * c);
        float sqrtd = (float)Math.Sqrt(discriminant);
        
        if (discriminant < 0)
            return false;
        
        float root = (-b - sqrtd) / a;
        if (root < tmin || tmax < root)
        {
            root = (-b  + sqrtd) / a;
            if (root < tmin || tmax < root)
                return false;
        }
        
        intersection.T = root;
        intersection.Position = ray.Parametric(intersection.T);
        intersection.Normal = (intersection.Position - Center) / Radius;
        
        return true;
    }
}
