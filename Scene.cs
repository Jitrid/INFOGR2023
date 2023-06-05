using Vector3 = OpenTK.Mathematics.Vector3;

namespace INFOGR2023Template;

public class Scene
{
    public List<Light> Lights = new();
    public List<Primitive> Primitives = new();

    public Scene()
    {
        // Light source(s)
        Lights.Add(new Light(new Vector3(-5f, 10f, -1.5f), Vector3.One * 50));
        Lights.Add(new Light(new Vector3(5f, 7f, 2f), Vector3.One * 50));
        Lights.Add(new Light(new Vector3(-2f, 10f, 11f), Vector3.One * 50));

        // Plane - switch the two below to go from a checkered plane to a regular plane.
        Primitives.Add(new CheckeredPlane(new Vector3(0f, -1f, 0f), 0, 0f));
        // Primitives.Add(new Plane(new Vector3(0f, -1f, 0f), 0f, new Vector3(1f, 1f, 1f), 
        //     new Vector3(1f, 1f, 1f), Vector3.One, 50f, 0f));

        // Additional primitives.
        // Primitives.Add(new Sphere(new Vector3(0f, 1.5f, 6f), 1f, new Vector3(.89f, .76f, .71f),
        //     new Vector3(1f, 1f, 1f), new Vector3(.89f, .63f, .5f), 50f, 0f));
        Primitives.Add(new Sphere(new Vector3(0.5f, 1f, 6.3f), 0.5f, new Vector3(0f, 0f, 1f),
            new Vector3(1f, 1f, 1f), new Vector3(.89f, .63f, .5f), 50f, 0f));
        Primitives.Add(new Sphere(new Vector3(-1f, 1.3f, 7.8f), 0.8f, new Vector3(0.83f, 0.33f, 0.69f),
            new Vector3(1f, 1f, 1f), new Vector3(.89f, .63f, .5f), 50f, 0f));

        Primitives.Add(new Triangle(new Vector3(-2f, 1f, 9f), new Vector3(2f, 1, 9f), new Vector3(0f, 4f, 9f),
            new Vector3(1, 0, 0), 0f));

        // Reflective sphere(s).
        Primitives.Add(new Sphere(new Vector3(2.5f, 2f, 6f), 1f, new Vector3(1f, 1f, 1f),
            new Vector3(1f, 1f, 1f), Vector3.One, 5f, 1f));
    }
}
