using Vector3 = OpenTK.Mathematics.Vector3;

namespace INFOGR2023Template;

public class Scene
{
    public List<Primitive> Primitives = new();

    public Scene()
    {
        // Camera = new Camera(this, new Vector3(0f, 1.5f, 0f));

        // Plane
        Primitives.Add(new Plane(new Vector3(0f, -1f, 0f), 0f, new Vector3(1f, 1f, 1f), 
            new Vector3(1f, 1f, 1f), Vector3.One, 50f, 0.2f));

        // Area lights
        Primitives.Add(new Sphere(new Vector3(0f, 12f, 0f), 3f, Vector3.Zero,
            Vector3.Zero, Vector3.Zero, 0f, 0f, Vector3.One * 10f));
        Primitives.Add(new Sphere(new Vector3(2.5f, 1f, 3f), 0.5f, Vector3.Zero,
            Vector3.Zero, Vector3.Zero, 0f, 0f, Vector3.One * 10f));

        // Spheres
        Primitives.Add(new Sphere(new Vector3(-0.2f, 2.2f, 3.3f), 0.3f, new Vector3(0, 0, 1),
            new Vector3(1f, 1f, 1f), new Vector3(.89f, .63f, .5f), 50f, 0.5f, Vector3.Zero));
        Primitives.Add(new Sphere(new Vector3(-0.2f, 1.2f, 2.8f), 0.3f, new Vector3(.89f, .76f, .71f), // beige
            new Vector3(1f, 1f, 1f), new Vector3(.89f, .63f, .5f), 50f, 0.5f, Vector3.Zero));
        Primitives.Add(new Sphere(new Vector3(1f, 0.8f, 2f), 0.2f, new Vector3(1f, 0f, 0f),
            new Vector3(1f, 1f, 1f), new Vector3(.89f, .63f, .5f), 50f, 0.2f, Vector3.Zero));
        // Spheres - full mirrors
        Primitives.Add(new Sphere(new Vector3(-0.8f, 1.2f, 2.5f), 0.2f, new Vector3(1f, 1f, 1f),
            new Vector3(1f, 1f, 1f), Vector3.One, 5f, 1f, Vector3.Zero));
        Primitives.Add(new Sphere(new Vector3(0.7f, 1f, 2.6f), 0.3f, new Vector3(1f, 1f, 1f),
            new Vector3(1f, 1f, 1f), Vector3.One, 5f, 0.65f, Vector3.Zero));
    }
}
