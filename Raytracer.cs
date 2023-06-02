using Vector3 = OpenTK.Mathematics.Vector3;

namespace INFOGR2023Template;

internal class Raytracer
{
    private Surface Screen;
    private Debug Debug;
    private Scene Scene;
    public Camera Camera;
    private int aspectRatio;

    private Sphere Sphere;
    private Light Light;

    public Raytracer(Surface screen)
    {
        Screen = screen;
        Scene = new Scene();
        Debug = new Debug(screen, Scene.Primitives);

        Sphere = (Sphere)Scene.Primitives[1];
        Light = Scene.Lights[0];

        aspectRatio = Screen.height / (Screen.width / 2);
        Camera = new Camera(aspectRatio, new Vector3(0f, 0f, 0f));
    }

    public void Render()
    {
        Screen.Clear(0);
        Debug.Render();

        int count = 0;
        int c = 0;
       // int i;
        Parallel.For(0, Screen.height, i => // y
        {
            Parallel.For (0, Screen.width/2,  j =>  // x
            {
                Screen.pixels[i * Screen.width + j] = 0x304080;
                double y = Camera.P0.Y - i / ((double)(Screen.height / 2) - Camera.P0.Y);
                double x = Camera.P0.X + j / ((double)(Screen.width / 4) - Camera.P0.X); // pixel indices

                Vector3 punty = Camera.P0 + (float)x * Camera.U + (float)y * Camera.V;

                // Create a ray from the camera position through the current pixel
                Ray viewRay = Camera.GetRay(punty);
                    Vector3 colorV = Intersection.TraceRay(viewRay, Scene, 5);
                    int color = ConvertToHexColor(colorV);
                
                
                    Screen.pixels[i * Screen.width + j] = color;
                




















                //foreach (Primitive p in Scene.Primitives)
                //{




























                //if (p.HitRay(viewRay, out Vector3 intersect))
                //{
                //    Vector3 lightDirection = Vector3.Normalize(Light.Location - intersect);
                //    Ray lightRay = new Ray(intersect, lightDirection);

                //    if (!Intersection.Shadowed(intersect, Light.Location, out Vector3 lightD, Scene.Primitives, p))
                //    {
                //        Vector3 normal = p.GetNormal(intersect);
                //        float diffuseCoefficient = Math.Max(0, Vector3.Dot(normal, lightDirection));

                //        // Apply diffuse shading
                //        Vector3 diffuseColor = p.GetColor();


                //        Vector3 shadedColor = diffuseColor * diffuseCoefficient;

                //        // Add specular reflection
                //        Vector3 reflectionDirection = Intersection.ReflectRay(viewRay.Direction, normal);
                //        Ray reflectedRay = new Ray(intersect, reflectionDirection);
                //        if (Intersection.TraceRay(reflectedRay, Scene.Primitives, Light, 50 - 1, out Vector3 reflectedColor))
                //        {
                //            // Apply reflection coefficient or material properties here
                //            shadedColor += reflectedColor;
                //        }

                //        // Convert the shaded color to integer representation
                //        int color = ((int)shadedColor.X << 16 & 255) | ((int)shadedColor.Y << 8 & 255) | (int)shadedColor.Z & 255;

                //        Screen.pixels[i * Screen.width + j] = color;
                //    }
                //    else
                //    {
                //        //// Handle shadows
                //        //Screen.pixels[i * Screen.width + j] = 0x000000;
                //    }
                //}
                //}
            });
        





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
        // Screen.pixels[
        //         Utilities.TranslateZ(Screen, Camera.Position.Z) * Screen.width +
        //         Utilities.TranslateX(Screen, Camera.Position.X)] = 255;
        });


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
        // Screen.Line(Utilities.TranslateX(Screen, Sphere.Center.X) - 5,
        //     Utilities.TranslateZ(Screen, Sphere.Center.Z) - 5,
        //     Utilities.TranslateX(Screen, Sphere.Center.X) + 5, Utilities.TranslateZ(Screen, Sphere.Center.Z) + 5,
        //     255 << 16);

        Screen.pixels[
            Utilities.TranslateZ(Screen, Sphere.Center.Z) * Screen.width +
            Utilities.TranslateX(Screen, Sphere.Center.X)] = 255;

        Screen.Line(1065, 956, 1124, 1008, 255);
    }

    //NAAR UTILS
    public static int ConvertToHexColor(Vector3 color)
    {
        int red = (int)(color.X * 255);
        int green = (int)(color.Y * 255);
        int blue = (int)(color.Z * 255);

        return (red << 16) | (green << 8) | blue;
    }

}