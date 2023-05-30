using System.Numerics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform.Windows;
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

    public  bool HitRay (Ray ray, float tmin, float tmax, out Vector3 intersect)
    {
        //Console.WriteLine(ray.Direction);
        Vector3 distanceToCenter = ray.Origin - this.Center;
        Vector3 t = ray.Origin + (Vector3.Dot(distanceToCenter, ray.Direction) * ray.Direction);
        float dcp = (t - Center).Length;
        float d  = (float)((t - ray.Origin).Length - Math.Sqrt((Radius * Radius) - dcp * dcp));

        Console.WriteLine($"t{t}. d:{d}");

        if (d > Radius)
        {
            intersect = Vector3.Zero;
            return false;
        }
        else
        {
            intersect = t;
            return true;
        }
        

    
    }
}
