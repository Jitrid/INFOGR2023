using OpenTK.Mathematics;

namespace INFOGR2023Template;

public struct Intersection
{
    public Vector3 Position;
    public Vector3 Normal;
    public float T;
}

public abstract class Intersectable
{
    public abstract bool HitOrMiss(Ray ray, float tmin, float tmax, out Intersection intersection);
}
