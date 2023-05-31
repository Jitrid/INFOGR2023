using OpenTK.Mathematics;

namespace INFOGR2023Template;


public abstract class Primitive
{
    public abstract bool HitRay(Ray ray, out Vector3 intersect);
    public abstract Vector3 GetNormal(Vector3 point);

    public abstract bool IntersectsWithLight(Vector3 intersectionPoint, Vector3 lightPosition, out Vector3 direction);
}
