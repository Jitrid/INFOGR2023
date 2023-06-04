using System.Numerics;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace INFOGR2023Template;

public class Scene
{
    public List<Light> Lights = new();
    public List<Primitive> Primitives = new();

    public Scene()
    {
        // Light source(s)
        Lights.Add(new Light(new Vector3(-5f, 10f, -1.5f), 255, 255, 255));
        // Lights.Add(new Light(new Vector3(5f, 7f, 2f), 255, 255, 255));

        // Plane
        Primitives.Add(new CheckeredPlane(new Vector3(0f, -1f, 0f), 0,  50f, 0f));

        // Primitives.Add(new Plane(new Vector3(0f, 0f, -1f), 10f, new Vector3(1f, 0.1f, 0.1f),
        //     new Vector3(1f, 1f, 1f), Vector3.One, 0f, 0f));
        // Primitives.Add(new Plane(new Vector3(-1f, 0f, 0f), 10f, new Vector3(0.1f, 1f, 0.1f),
        //     new Vector3(1f, 1f, 1f), Vector3.One, 0f, 0f));
        // Deze hieronder komt uit de foto's die ik heb gestuurd, de andere twee werken niet echt.
        // Primitives.Add(new Plane(new Vector3(1f, 0f, 0f), -10f, new Vector3(0.1f, 0.1f, 1f),
        //     new Vector3(1f, 1f, 1f), Vector3.One, 50f, 0f));

        // Spheres
        Primitives.Add(new Sphere(new Vector3(0f, 1.5f, 6f), 1f, new Vector3(.89f, .76f, .71f),
            new Vector3(1f, 1f, 1f), new Vector3(.89f, .63f, .5f), 50f, 0f));
        Primitives.Add(new Sphere(new Vector3(2.5f, 1f, 6f), 1f, new Vector3(1f, 1f, 1f),
            new Vector3(1f, 1f, 1f), Vector3.One, 5f, 1f));
        // Primitives.Add(new Sphere(new Vector3(7.5f, 4.5f, 6f), 4f, new Vector3(1f, 1f, 0f),
        //     new Vector3(1f, 1f, 1f), Vector3.One, 5f, 0f));
    }
}
