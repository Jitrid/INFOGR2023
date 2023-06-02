using Vector3 = OpenTK.Mathematics.Vector3;

namespace INFOGR2023Template;

public class Scene
{
    public List<Light> Lights = new();
    public List<Primitive> Primitives = new();

    public Scene()
    {
        // Light source(s)
        Lights.Add(new Light(new Vector3(-5f, -7f, -.5f), 255, 255, 255));

        // Plane
        Primitives.Add(new Plane(new Vector3(0f, -1f, 0f), new Vector3(0, 0, 2), new Vector3(0f,1f,0f), 
            new Vector3(1f, 1f, 1f), Vector3.One, 50f, 0f));

        // Spheres
        Primitives.Add(new Sphere(new Vector3(0f, 0f, 6f), 3f, new Vector3(1f, 1f, 1f), 
            new Vector3(1f, 1f, 1f), Vector3.One, 50f, 1f));
        Primitives.Add(new Sphere(new Vector3(9f, 0f, 6f), 3f, new Vector3(1f, 1f, 1f), 
            new Vector3(1f, 1f, 1f), Vector3.One, 50f, .51f));
    }
}
