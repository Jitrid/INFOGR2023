using Vector3 = OpenTK.Mathematics.Vector3;

namespace INFOGR2023Template;

internal class Raytracer
{
    private Surface Screen;
    private Debug Debug;
    private Scene Scene;
    public Camera Camera;

    private Sphere Sphere;
    private Light Light;

    public Raytracer(Surface screen)
    {
        Screen = screen;
        Scene = new Scene();

        Sphere = (Sphere)Scene.Primitives[1];
        Light = Scene.Lights[0];
        
        Camera = new Camera(new Vector3(0f, 0.25f, 0f), new Vector3(0, 0.5f, 7f));

        Debug = new Debug(screen, Camera, Scene.Primitives);
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

                Vector3 punty = (Camera.P0 - Camera.Position) + (float)x * Camera.U + (float)y * Camera.V;
                punty.Normalize();

                // Create a ray from the camera position through the current pixel
                Ray viewRay = new(Camera.Position, punty);

                Vector3 colorV = Intersection.TraceRay(Debug, Camera, viewRay, Scene, 5, i);
                int color = ConvertToHexColor(colorV);
                Screen.pixels[i * Screen.width + j] = color;
            });
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