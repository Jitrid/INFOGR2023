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
    public Light Light;

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

        Light = new Light(new Vector3(-5f, 7f, -.5f), 255, 255, 255);
        Plane = new Plane(new Vector3(0f, 1f, 0f), new Vector3(1,0,2));
            
        Sphere = new Sphere(new Vector3(0f, 0f, 6f), 3f);
    }
        
    /// <summary>
    /// Renders the scene each frame.
    /// </summary>
    public void Tick()
    {
        Screen.Clear(0);
            
        for (int i = 0; i < Screen.height; i++) // y
        {
            for (int j = 0; j < Screen.width / 2; j++) // x
            {
                Screen.pixels[i * Screen.width + j] = 0x304080;
                // double y = -1.0 + i / 299.0;
                // double x = -1.0 + j / 299.0;
                double y = -1.0 + i / ((double)(Screen.height / 2) - 1);
                double x = -1.0 + j / ((double)(Screen.width / 4) - 1);

                Vector3 punty = Camera.P0 + (float)x * Camera.U + (float)y * Camera.V;

                // Create a ray from the camera position through the current pixel
                Ray viewRay = Camera.GetRay(punty);
                //plane
                if (Plane.HitRay(viewRay, out Vector3 intersectP))
                {
                    Vector3 lightDirection = Vector3.Normalize(Light.Location - intersectP);
                    Ray lightRay = new(intersectP, lightDirection);
                    if (!Sphere.HitRay(lightRay, out Vector3 lightIntersect))
                    {
                        int color = ItTakesAllColoursToMakeARainbow(0xFFFFFF, Vector3.Normalize(lightRay.Direction), viewRay.Direction,
                            Plane.GetNormal(intersectP), 0x00FF00, .001f, 10, 0.051f);
                        Screen.pixels[i * Screen.width + j] = color;
                    }
                    else
                    {
                        Screen.pixels[i * Screen.width + j] = 0;
                    }
                }

                if (Sphere.HitRay(viewRay, out Vector3 intersectionPoint))
                {
                    if (Count % 10 == 0)
                    {
                        Screen.Line(Utilities.TranslateX(Screen, Camera.Position.X), Utilities.TranslateZ(Screen, Camera.Position.Z), 
                            Utilities.TranslateX(Screen, intersectionPoint.X), Utilities.TranslateZ(Screen, intersectionPoint.Z), 255);
                    }

                    // hap slik weg ofzo idk anymore
                    Vector3 normal = Sphere.GetNormal(intersectionPoint);
                    if (Sphere.IntersectsWithLight(intersectionPoint, Light.Location, out Vector3 lightDir))
                    {
                        // 1 roze teelbal, you are welcome
                        int color = ItTakesAllColoursToMakeARainbow(0xFF00FF, Vector3.Normalize(lightDir), viewRay.Direction,
                            normal, 0xFFFFFF, .001f, 10, 0.051f);
                        Screen.pixels[i * Screen.width + j] = color;
                    }
                }
                else
                {
                    if (Count % 100000 == 0)
                    {
                        Screen.Line(Utilities.TranslateX(Screen, Camera.Position.X), Utilities.TranslateZ(Screen, Camera.Position.Z), 
                            Utilities.TranslateX(Screen, viewRay.Direction.X), 0, 255 << 16);
                    }
                }
                Count++;

                // TODO: out of bounds als je te ver naar achteren gaat.
                Screen.pixels[Utilities.TranslateZ(Screen, Camera.Position.Z) * Screen.width + Utilities.TranslateX(Screen, Camera.Position.X)] = 255;
            }
        }

        // Console.WriteLine($"1: {Sphere.Center.X}, 2: {Sphere.Center.Z}");
        // Console.WriteLine($"Answer: {TranslateX(-4f)} - {TranslateX(4f)} | {TranslateX(-16f)} - {TranslateX(16f)}");
        Screen.Line(Utilities.TranslateX(Screen, Sphere.Center.X) - 5, Utilities.TranslateZ(Screen, Sphere.Center.Z) - 5, 
            Utilities.TranslateX(Screen, Sphere.Center.X) + 5, Utilities.TranslateZ(Screen, Sphere.Center.Z) + 5, 255 << 16);
    }

    public int ItTakesAllColoursToMakeARainbow(int lightColor, Vector3 lightDirection, Vector3 viewDirection,
        Vector3 normal, int objectColor, float specularIntensity, float specularPower,
        float distancelichtsterkteAfname)
    {
        // Genakt van een andere plek, ik ben echt klaar met RGB
        int lightRed = (lightColor >> 16) & 255;
        int lightGreen = (lightColor >> 8) & 255;
        int lightBlue = lightColor & 255;

        int objectRed = (objectColor >> 16) & 255;
        int objectGreen = (objectColor >> 8) & 255;
        int objectBlue = objectColor & 255;

        //die clamp Slide set 5 slide 19
        if (Vector3.Dot(normal, lightDirection) < 0)
            return 0x000000;

        // Diffuse material Slideset 5 slide 19
        float diffuseCoefficient = Math.Max(0, Vector3.Dot(normal, lightDirection));



        //reflection direction kut vectoren uit GL hebben geen .Reflect dus dan maar ombouwen... Kms
        System.Numerics.Vector3 systemReflectionDirection =
            System.Numerics.Vector3.Reflect(Utilities.VectorToSystem(-lightDirection), Utilities.VectorToSystem(normal));
        Vector3 reflectionDirection = Utilities.VectorToGL(systemReflectionDirection);

        //specular reflection Slide 5 26 formule
        float specularCoefficient =
            (float)Math.Pow(Math.Max(0, Vector3.Dot(viewDirection, reflectionDirection)), specularPower) *
            specularIntensity;

        // Sterkte van licht neemt af over afstand enzo  ( 1/ r*r)
        float lichtsterkteAfname = 1 / (distancelichtsterkteAfname * distancelichtsterkteAfname);

        // losse kleuren berekenen lightRed = van het licht niet dat het licht is. Beetje erbij dat je geen harde schaduw rand krijgt, als je dat wilt moet je een batman film gaan kijken of startrek uit 1979
        int shadedRed = objectRed/50 + (int)(lightRed * objectRed * (diffuseCoefficient + specularCoefficient) *
            lichtsterkteAfname / (255 * 255));
        int shadedGreen = objectGreen/50 +(int)(lightGreen * objectGreen * (diffuseCoefficient + specularCoefficient) *
            lichtsterkteAfname / (255 * 255));
        int shadedBlue =  objectBlue/50 + (int)(lightBlue * objectBlue * (diffuseCoefficient + specularCoefficient) *
            lichtsterkteAfname / (255 * 255));

        // Blijkbaar kan dit ook out of bounds
        int fixRed = Math.Max(0, Math.Min(shadedRed, 255));
        int fixGreen = Math.Max(0, Math.Min(shadedGreen, 255));
        int fixBlue = Math.Max(0, Math.Min(shadedBlue, 255));

        // En weer terug naar 0xRRGGBB
        return (fixRed << 16) | (fixGreen << 8) | fixBlue;
    }

    // The following three methods call their respective methods in regards to camera movement.
    public void CallMovement(KeyboardKeyEventArgs kea, float t) => Camera.CameraKeyboardInput(kea, t);
    public void CallRotation(MouseMoveEventArgs mea) => Camera.RotationInput(mea);
    public void CallZoom(MouseWheelEventArgs mea) => Camera.CameraZoom(mea);
}