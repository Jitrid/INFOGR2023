using Vector3 = OpenTK.Mathematics.Vector3;

namespace INFOGR2023Template;

public class Scene
{
    public List<Light> Lights = new();
    public List<Primitive> Primitives = new();
    Random random = new Random();

    public Scene()
    {
        // Light source(s)
        Lights.Add(new Light(new Vector3(-5f, 10f, -1.5f), Vector3.One * 50));
        Lights.Add(new Light(new Vector3(5f, 7f, 2f), Vector3.One * 50));
        Lights.Add(new Light(new Vector3(-2f, 10f, 11f), Vector3.One * 50));

        // Plane
        Primitives.Add(new Plane(new Vector3(0f, -1f, 0f), 0f, new Vector3(1f, 1f, 1f), 
            new Vector3(1f, 1f, 1f), Vector3.One, 50f, 0f));
        // Primitives.Add(new CheckeredPlane(new Vector3(0f, -1f, 0f), 0, 0f));

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
        Primitives.Add(new Sphere(new Vector3(2.5f, 1.5f, 4f), 1f, new Vector3(1f, 0f, 0f),
            new Vector3(1f, 1f, 1f), new Vector3(.89f, .63f, .5f), 50f, 0f));
        Primitives.Add(new Sphere(new Vector3(2.5f, 2f, 6f), 1f, new Vector3(1f, 1f, 1f),
            new Vector3(1f, 1f, 1f), Vector3.One, 5f, 1f));

        Primitives.Add(new Triangle(new Vector3(-2f, 1f, 9f), new Vector3(2f, 1, 9f), new Vector3(0f, 4f, 9f), 
            new Vector3(1, 0, 0), new Vector3(1f, 1f, 1f), new Vector3(.89f, .63f, .5f), 50f, 0f));

        for (int i = 0; i < 1000; i++)
        {
            // Generate random coordinates within a specified range
            float xCoord = (float)random.NextDouble() * 20;
            float yCoord = (float)random.NextDouble() * 5;
            float zCoord = (float)random.NextDouble() * 20;

            // Generate random radius within a specified range
            float radius = (float)random.NextDouble() * 1;

            // Generate random RGB color values
            Vector3 color = new Vector3((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());

            // Generate random RGB diffuse color values
            Vector3 diffuseColor = new Vector3((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());

            // Generate random RGB specular color values
            Vector3 specularColor = new Vector3((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());

            // Generate random specular power within a specified range
            float specularPower = (float)random.NextDouble() * 500;

            // Generate random reflection coefficient (either 0 or 1)
            float reflectionCoefficient = random.Next(2);

            // Create a new sphere with the generated properties and add it to the Primitives list
            Primitives.Add(new Sphere(new Vector3(xCoord, yCoord, zCoord), radius, color, diffuseColor, specularColor, specularPower, reflectionCoefficient));
        }
    }
}
