using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.ES11;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace INFOGR2023Template
{
    public class Pathtracer
    {
        private Raytracer raytracer;
        private Intersection intersector;

        public Pathtracer(Raytracer raytracer)
        {
            this.raytracer = raytracer;
            intersector = new Intersection(raytracer);
        }

        public Vector3 TracePath(Ray r)
        {
            Path ph = new Path(r);

            for (int bounce = 0; bounce < r.MaxBounces; bounce++)
            {
                if (!intersector.FindClosestIntersection(ph.Rays[bounce], out Vector3 intersectPoint, out Primitive closestPrimitive))
                {
                    return new Vector3(0, 0, 1f);
                }

                ph.Add(ph.Rays[bounce], intersectPoint, closestPrimitive, closestPrimitive.GetColor());

                if (closestPrimitive.ReflectionCoefficient == 0f)
                {
                    return pathColor(ph);
                }

                Vector3 reflectD = intersector.ReflectRay(ph.GetRay(bounce).Direction, intersectPoint.Normalized());
                r = new Ray(intersectPoint, reflectD, bounce - 1);
            }

            return new Vector3(0, 0, 1f);
        }

        public Vector3 pathColor(Path p)
        {
            Vector3 c = Vector3.Zero;
            for (int i = 0; i < p.Primitives.Count; i++)
            {
                Vector3 intersect = p.GetIntersection(i);

                Scene s = raytracer.Scene;
                float r = Vector3.Distance(s.Lights[0].Location, intersect);

                Vector3 cl = intersector.ShadeColor(s, p.GetPrimitive(i), intersect, (s.Lights[0].Location- intersect),
                    p.GetPrimitive(i).GetNormal(intersect), p.GetColor(i));

                c += cl;
            }
            return c;
        }
    }

}



