using System.Security.AccessControl;
using OpenTK.Windowing.Common;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace INFOGR2023Template;

class Application
{
    // member variables
    public Surface Screen;
    public Camera Camera;
    public Plane Plane;
    public Sphere Sphere;
    public Sphere Sphere2;
    public Light Light;
    public List<Primitive> objects;


    /// <summary>
    /// The aspect ratio of the working window.
    /// </summary>
    private float _aspectRatio;

    public int Count;

    // constructor
#pragma warning disable CS8618
    public Application(Surface screen) => Screen = screen;
#pragma warning restore CS8618

    /// <summary>
    /// Initializes the window.
    /// </summary>
    public void Init()
    {
        _aspectRatio = Screen.height / (Screen.width / 2);

        Vector3 cameraPosition = new(0f, 0f, 0f);
        Camera = new Camera(_aspectRatio, cameraPosition);

        Light = new Light(new Vector3(-5f, -7f, -.5f), 255, 255, 255);
        Plane = new Plane(new Vector3(0f, -1f, 0f), new Vector3(0, 0, 2), 0x00FF00, new Vector3(1f, 1f, 1f),
            Vector3.One, 50f, 0f);

        Sphere = new Sphere(new Vector3(0f, 0f, 6f), 3f, 0xFF0000, new Vector3(1f, 1f, 1f), Vector3.One, 50f, 0.5f);
        Sphere2 = new Sphere(new Vector3(9f, 0f, 6f), 3f, 0x0000FF, new Vector3(1f, 1f, 1f), Vector3.One, 50f, 1f);
        objects = new List<Primitive>
        {
            Plane,
            Sphere,
            Sphere2
        };
    }

    /// <summary>
    /// Renders the scene each frame.
    /// </summary>
    public void Tick()
    {
        Screen.Clear(0);

        int count = 0;
        int c = 0;




        for (int i = 0; i < Screen.height; i++) // y
        {
            for (int j = 0; j < Screen.width / 2; j++) // x
            {
                Screen.pixels[i * Screen.width + j] = 0x304080;
                double y = Camera.P0.Y - i / ((double)(Screen.height / 2) - Camera.P0.Y);
                double x = Camera.P0.X + j / ((double)(Screen.width / 4) - Camera.P0.X); // pixel indices

                Vector3 punty = Camera.P0 + (float)x * Camera.U + (float)y * Camera.V;

                // Create a ray from the camera position through the current pixel
                Ray viewRay = Camera.GetRay(punty);

                foreach (Primitive p in objects)
                {
                    if (p.HitRay(viewRay, out Vector3 intersect))
                    {
                        Vector3 lightDirection = Vector3.Normalize(Light.Location - intersect);
                        Ray lightRay = new Ray(intersect, lightDirection);

                        if (!Intersection.Shadowed(intersect, Light.Location, out Vector3 lightD, objects, p))
                        {
                            Vector3 normal = p.GetNormal(intersect);
                            float diffuseCoefficient = Math.Max(0, Vector3.Dot(normal, lightDirection));

                            // Apply diffuse shading
                            Vector3 diffuseColor = new Vector3(
                                (int)((p.GetColor() >> 16) & 255),
                                (int)((p.GetColor() >> 8) & 255),
                                (int)(p.GetColor() & 255)
                            );

                            Vector3 shadedColor = diffuseColor * diffuseCoefficient;

                            // Add specular reflection
                            Vector3 reflectionDirection = Intersection.ReflectRay(viewRay.Direction, normal);
                            Ray reflectedRay = new Ray(intersect, reflectionDirection);
                            if (Intersection.TraceRay(reflectedRay, objects, Light, 50 - 1, out int reflectedColor))
                            {
                                // Apply reflection coefficient or material properties here
                                shadedColor += new Vector3(
                                    (int)((reflectedColor >> 16) & 255),
                                    (int)((reflectedColor >> 8) & 255),
                                    (int)(reflectedColor & 255)
                                );
                            }

                            // Convert the shaded color to integer representation
                            int color = ((int)shadedColor.X << 16) | ((int)shadedColor.Y << 8) | (int)shadedColor.Z;

                            Screen.pixels[i * Screen.width + j] = color;
                        }
                        else
                        {
                            // Handle shadows
                            Screen.pixels[i * Screen.width + j] = 0x000000;
                        }
                    }
                }
            }
        





        //if (c == 0)
        //{
        //    Screen.Line(Utilities.TranslateX(Screen, Camera.Position.X),
        //        Utilities.TranslateZ(Screen, Camera.Position.Z),
        //        Utilities.TranslateX(Screen, (float)(x)), Utilities.TranslateZ(Screen, (float)y), 255 << 8);

        //    // Console.WriteLine(x + ", " + y);
        //}

        //c++;



        //////plane
        //if (Plane.HitRay(viewRay, out Vector3 intersectP))
        //{
        //    Vector3 lightDirection = Vector3.Normalize(Light.Location - intersectP);
        //    Ray lightRay = new(intersectP, lightDirection);
        //    if (!Sphere.HitRay(lightRay, out Vector3 lightIntersect))
        //    {
        //        int color = ItTakesAllColoursToMakeARainbow(0xFFFFFF, Vector3.Normalize(lightRay.Direction), viewRay.Direction,
        //            Plane.GetNormal(intersectP), 0x00FF00, .001f, 10, 0.051f);
        //        Screen.pixels[i * Screen.width + j] = color;
        //    }
        //    else
        //    {
        //        Screen.pixels[i * Screen.width + j] = 0;
        //    }
        //}

        //if (Sphere.HitRay(viewRay, out Vector3 intersectionPoint))
        //{
        //    if (count == 0)
        //    {
        //        Screen.Line(Utilities.TranslateX(Screen, Camera.Position.X), Utilities.TranslateZ(Screen, Camera.Position.Z),
        //            Utilities.TranslateX(Screen, intersectionPoint.X), Utilities.TranslateZ(Screen, intersectionPoint.Z), 255);
        //    }
        //    count++;

        //    // hap slik weg ofzo idk anymore
        //    Vector3 normal = Sphere.GetNormal(intersectionPoint);
        //    if (Sphere.IntersectsWithLight(intersectionPoint, Light.Location, out Vector3 lightDir))
        //    {
        //        // 1 roze teelbal, you are welcome
        //        int color = ItTakesAllColoursToMakeARainbow(0xFF00FF, Vector3.Normalize(lightDir), viewRay.Direction,
        //            normal, 0xFFFFFF, .001f, 10, 0.051f);
        //        Screen.pixels[i * Screen.width + j] = color;
        //    }
        //}



        // else
        // {
        //     if (Count % 100000 == 0)
        //     {
        //         Screen.Line(Utilities.TranslateX(Screen, Camera.Position.X), Utilities.TranslateZ(Screen, Camera.Position.Z), 
        //             Utilities.TranslateX(Screen, viewRay.Direction.X), 0, 255 << 16);
        //     }
        // }

        // TODO: out of bounds als je te ver naar achteren gaat.
        Screen.pixels[
                Utilities.TranslateZ(Screen, Camera.Position.Z) * Screen.width +
                Utilities.TranslateX(Screen, Camera.Position.X)] = 255;
        }


        // Prints additional information to the debug window as displayable text.
        Screen.Print($"P0: {Camera.P0}", (Screen.width / 2) + 20, Screen.height - 30, 0xffffff);
        Screen.Print($"P1: {Camera.P1}", (Screen.width / 2) + 20, Screen.height - 60, 0xffffff);
        Screen.Print($"P2: {Camera.P2}", (Screen.width / 2) + 20, Screen.height - 90, 0xffffff);
        Screen.Print($"P3: ({Camera.P1.X}, {Camera.P2.Y}, {Camera.P0.Z})", (Screen.width / 2) + 20, Screen.height - 120,
            0xffffff);
        Screen.Print($"Pos: {Camera.Position}", (Screen.width / 2) + 20, Screen.height - 150, 0xffffff);

        Screen.Line(Utilities.TranslateX(Screen, Camera.P1.X), Utilities.TranslateZ(Screen, Camera.P1.Z),
            Utilities.TranslateX(Screen, Camera.P2.X), Utilities.TranslateZ(Screen, Camera.P2.Z), 255 << 8);

        // Console.WriteLine($"1: {Sphere.Center.X}, 2: {Sphere.Center.Z}");
        // Console.WriteLine($"Answer: {TranslateX(-4f)} - {TranslateX(4f)} | {TranslateX(-16f)} - {TranslateX(16f)}");
        Screen.Line(Utilities.TranslateX(Screen, Sphere.Center.X) - 5,
            Utilities.TranslateZ(Screen, Sphere.Center.Z) - 5,
            Utilities.TranslateX(Screen, Sphere.Center.X) + 5, Utilities.TranslateZ(Screen, Sphere.Center.Z) + 5,
            255 << 16);

    }


public int ItTakesAllColoursToMakeARainbow(int lightColor, Vector3 lightDirection, Vector3 viewDirection,
        Vector3 normal, int objectColor, float specularIntensity, float specularPower,
        float distancelichtsterkteAfname)
    {
        float lightRed = ((lightColor >> 16) & 255) / 255f;
        float lightGreen = ((lightColor >> 8) & 255) / 255f;
        float lightBlue = (lightColor & 255) / 255f;

        float objectRed = ((objectColor >> 16) & 255) / 255f;
        float objectGreen = ((objectColor >> 8) & 255) / 255f;
        float objectBlue = (objectColor & 255) / 255f;

        if (Vector3.Dot(normal, lightDirection) < 0)
            return 0x000000;

        float diffuseCoefficient = Math.Max(0, Vector3.Dot(normal, lightDirection));

        //reflection direction kut vectoren uit GL hebben geen .Reflect dus dan maar ombouwen... Kms
        System.Numerics.Vector3 systemReflectionDirection =
            System.Numerics.Vector3.Reflect(Utilities.VectorToSystem(-lightDirection), Utilities.VectorToSystem(normal));
        Vector3 reflectionDirection = Utilities.VectorToGL(systemReflectionDirection);

        float specularCoefficient =
            (float)Math.Pow(Math.Max(0, Vector3.Dot(viewDirection, reflectionDirection)), specularPower) *
            specularIntensity;

        float lichtsterkteAfname = 1 / (distancelichtsterkteAfname * distancelichtsterkteAfname);

        float shadedRed = lightRed * objectRed * (diffuseCoefficient + specularCoefficient) * lichtsterkteAfname;
        float shadedGreen = lightGreen * objectGreen * (diffuseCoefficient + specularCoefficient) * lichtsterkteAfname;
        float shadedBlue = lightBlue * objectBlue * (diffuseCoefficient + specularCoefficient) * lichtsterkteAfname;

        int fixRed = (int)(shadedRed * 255f);
        int fixGreen = (int)(shadedGreen * 255f);
        int fixBlue = (int)(shadedBlue * 255f);

        return (fixRed << 16) | (fixGreen << 8) | fixBlue;
    }


    // The following three methods call their respective methods in regards to camera movement.
    public void CallMovement(KeyboardKeyEventArgs kea, float t) => Camera.CameraKeyboardInput(kea, t);
    public void CallRotation(MouseMoveEventArgs mea) => Camera.RotationInput(mea);
    public void CallZoom(MouseWheelEventArgs mea) => Camera.CameraZoom(mea);
}

