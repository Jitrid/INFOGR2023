using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace INFOGR2023Template;

public class Raytracer
{
    /// <summary>
    /// The main window.
    /// </summary>
    public Surface Screen;

    /// <summary>
    /// The debug window (on the right) of the application.
    /// </summary>
    public Debug Debug;

    /// <summary>
    /// The scene that contains the primitives and light sources.
    /// </summary>
    public Scene Scene;

    /// <summary>
    /// The main camera of the application.
    /// </summary>
    public Camera Camera;

    public Pathtracer Pathtracer;

    public Raytracer(Surface screen)
    {
        Screen = screen;
        Scene = new Scene();

        Debug = new Debug(this);

        Camera = new Camera(Screen, new Vector3(0f, 1.5f, 0f));

        Pathtracer = new Pathtracer(this);

    }

    /// <summary>
    /// Renders the visualisation of generated rays shot to all pixels on the screen.
    /// </summary>
    public void Render()
    {
        Stopwatch sw = Stopwatch.StartNew();

        Screen.Clear(0);
        Debug.DrawPrimitives();

        // Define the number of threads to use
        int numThreads = Environment.ProcessorCount;
        int subpixels = 1;

        Parallel.For(0, numThreads, threadIndex =>
        {
            int startY = threadIndex * (Screen.height / numThreads);
            int endY = startY + (Screen.height / numThreads);
           

            for (int y = startY; y < endY; y++)
            {
                for (int x = 0; x < Screen.width / 2; x++)
                {
                    Vector3 color = Vector3.Zero;
                    for (int sub = 0; sub < subpixels; sub++)
                    //Parallel.For(0, subpixels, sub =>
                    {
                        float offsetX = (float)((sub % Math.Sqrt(subpixels) + 0.5f) / Math.Sqrt(subpixels));
                        float offsetY = (float)((sub / Math.Sqrt(subpixels) + 0.5f) / Math.Sqrt(subpixels));

                        Vector3 point = Camera.P0 + ((x + offsetX) / (Screen.width / 2f)) * Camera.U +
                                        ((y + offsetY) / Screen.height) * Camera.V;
                        point = Vector3.Normalize(point - Camera.Position);

                        Ray viewRay = new Ray(Camera.Position, point, 0);
                        Intersection intersect = new Intersection(this);

                        Vector3 color2 = intersect.TraceRay(viewRay);
                        color += color2;
                        //Vector3 colorV = intersect.TraceRay(viewRay);
                        //Screen.pixels[y * Screen.width + x] = Utilities.ColorToInt(colorV);

                    }

                    Vector3 averagedColor = color / subpixels;
                    Screen.pixels[y * Screen.width + x] = Utilities.ColorToInt(averagedColor);


                }
            }
        });


        sw.Stop();
        Console.WriteLine(sw.ElapsedMilliseconds);
        // Prints useful information to the user's window.
        Screen.Print($"FOV: {Camera.FOV}", 15, 20, 0xffffff);
        Screen.Print($"Pitch: {Camera.Pitch}", 15, 50, 0xffffff);
        Screen.Print($"Yaw: {Camera.Yaw}", 15, 80, 0xffffff);
    }
}
