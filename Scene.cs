using Vector3 = OpenTK.Mathematics.Vector3;

namespace INFOGR2023Template;

public class Scene
{
    public List<Light> Lights = new();
    public List<Primitive> Primitives = new();

    public Scene()
    {
        // Light source(s)
        Lights.Add(new Light(new Vector3(-5f, 7f, -1.5f), 255, 255, 255));
        // Lights.Add(new Light(new Vector3(5f, 7f, 2f), 255, 255, 255));

        // Plane
        // Primitives.Add(new CheckeredPlane());
        Primitives.Add(new Plane(new Vector3(0f, 1f, 0f), 5f, new Vector3(1f, 1f, 1f),
            new Vector3(1f, 1f, 1f), Vector3.One, 50f, 0f));

        // Spheres
        Primitives.Add(new Sphere(new Vector3(0f, 1f, 6f), 3f, new Vector3(1f, 1f, 0f), 
            new Vector3(1f, 1f, 1f), Vector3.One, 50f, 0f));
        Primitives.Add(new Sphere(new Vector3(9f, 1f, 6f), 3f, new Vector3(1f, 1f, 0f), 
            new Vector3(1f, 1f, 1f), Vector3.One, 5f, 1f));
    }
}
