using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace INFOGR2023Template
{
    public class Path
    {
        public List<Ray> Rays { get; } = new();
        public List<Vector3> Intersections { get; } = new();
        public List<Primitive> Primitives { get; } = new();
        public List<Vector3> Colors { get; } = new();

        public Path(Ray r)
        {
            Rays.Add(r);
        }

        public void Add(Ray ray, Vector3 intersect, Primitive obj, Vector3 color) 
        {
            Rays.Add(ray);
            Intersections.Add(intersect);
            Primitives.Add(obj);
            Colors.Add(color);
        }
        public Ray GetRay(int index)
        {
            Ray r = Rays[index];
            return r;
        }

        public Vector3 GetIntersection(int index)
        {
            Vector3 intersection = Intersections[index];
            return intersection;
        }

        public Primitive GetPrimitive(int index)
        {
            Primitive obj = Primitives[index];
            return obj;
        }

        public Vector3 GetColor(int index)
        {
            Vector3 c = Colors[index];
            return c;
        }
    }

}
