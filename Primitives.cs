namespace INFOGR2023Template;

// TODO: PLAGIAATTTTTT

public class Primitives : Intersectable
{
    public List<Intersectable> _primitives = new List<Intersectable>();

    public Primitives() {}

    public void Add(Intersectable obj) => _primitives.Add(obj);

    public override bool HitOrMiss(Ray ray, float tmin, float tmax, out Intersection intersection)
    {
        intersection = new Intersection();
        Intersection temp = new();
        bool hit = false;

        float closest = tmax;

        foreach (Intersectable p in _primitives)
            if (p.HitOrMiss(ray, tmin, closest, out temp))
            {
                hit = true;
                closest = temp.T;
                intersection = temp;
            }

        return hit;
    }
}