using Vector3 = OpenTK.Mathematics.Vector3;

namespace INFOGR2023Template;

public class Plane : Primitive
{
    public Vector3 Normal { get; }
    public float S { get; } //distance to origin

    public Plane(Vector3 normal, Vector3 s)
    {
        Normal = normal.Normalized();
        S = s.Length;
    }

    public override Vector3 GetNormal(Vector3 point)
    {
     return Normal;
    }

    public override bool HitRay(Ray ray, out Vector3 intersect)
    {
        float denom = Vector3.Dot(ray.Direction, Normal);
        if (denom == 0)
        {
            //parallel 
            intersect = Vector3.Zero;
            return false;
        }
        float t = (Vector3.Dot(ray.Origin, Normal) - S) / denom;
        if (t < 0)
        {
            //Voorbij de camera
            intersect = Vector3.Zero;
            return false;
        }
        intersect = ray.Origin +  t* ray.Direction;
        return true;
    }

    public override bool IntersectsWithLight(Vector3 intersectionPoint, Vector3 lightPosition, out Vector3 direction)
    {
        throw new NotImplementedException();
    }
}