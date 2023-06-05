using OpenTK.Mathematics;

namespace INFOGR2023Template;

public class Path
{
    public List<Ray> Rays { get; } = new();
    public List<Vector3> Intersections { get; } = new();
    public List<Primitive> Primitives { get; } = new();
    public List<Vector3> Colors { get; } = new();
    public int BounceCount { get; set; }

    public Path(Ray r) => Rays.Add(r);

    public void Add(Ray ray, Vector3 intersect, Primitive obj, Vector3 color)
    {
        Rays.Add(ray);
        Intersections.Add(intersect);
        Primitives.Add(obj);
        Colors.Add(color);
    }

    public Ray GetRay(int index) => Rays[index];

    public Vector3 GetIntersection(int index)
    {
        if (index >= 0 && index < Intersections.Count)
        {
            Vector3 intersection = Intersections[index];
            return intersection;
        }

        return Vector3.Zero;
    }

    public Primitive GetPrimitive(int index)
    {
        if (index >= 0 && index < Primitives.Count)
        {
            Primitive obj = Primitives[index];
            return obj;
        }

        return null;
    }

    public Vector3 GetColor(int index) => Colors[index];
}