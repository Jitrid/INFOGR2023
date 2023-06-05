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

    /// <summary>
    /// The accumulation of previously generated colors.
    /// </summary>
    public Vector3[] Accumulation;
    /// <summary>
    /// How many frames (accumulations) have passed.
    /// </summary>
    public int frames;

    public Raytracer(Surface screen)
    {
        Screen = screen;
        Scene = new Scene();
        Accumulation = new Vector3[Screen.height * Screen.width];

        Debug = new Debug(this);

        Camera = new Camera(this, new Vector3(0f, 1.5f, 0f));
    }

    /// <summary>
    /// Renders the visualisation of generated rays shot to all pixels on the screen.
    /// </summary>
    public void Render()
    {
        Screen.Clear(0);
        Debug.DrawPrimitives();

        // Define the number of threads to use.
        int threads = Environment.ProcessorCount;
        const int samples = 20; // adjust accordingly with your own performance.

        frames++;

        Parallel.For(0, threads, threadIndex =>
        {
            int startY = threadIndex * (Screen.height / threads);
            int endY = startY + (Screen.height / threads);

            for (int y = startY; y < endY; y++)
            {
                for (int x = 0; x < Screen.width / 2; x++)
                {
                    Vector3 mainColor = Vector3.Zero;
                    for (int sub = 0; sub < samples; sub++)
                    {
                        float offsetX = (float)((sub % Math.Sqrt(samples) + 0.5f) / Math.Sqrt(samples));
                        float offsetY = (float)((sub / Math.Sqrt(samples) + 0.5f) / Math.Sqrt(samples));

                        Vector3 point = Camera.P0 + ((x + offsetX) / (Screen.width / 2f)) * Camera.U +
                                        ((y + offsetY) / Screen.height) * Camera.V;
                        point = Vector3.Normalize(point - Camera.Position);
                        Ray viewRay = new Ray(Camera.Position, point);

                        Intersection intersect = new Intersection(this);
                        mainColor += intersect.TraceRay(viewRay, Vector3.One, 10);
                    }

                    // Add the frame's color to the accumulation of colors.
                    // This resets when the screen is closed and/or camera movement is initiated.
                    Accumulation[y * Screen.width + x] += mainColor / samples;

                    // Set the pixel's color accordingly, cramped between 0f and 1f.
                    Screen.pixels[y * Screen.width + x] = Utilities.ColorToInt(Vector3.Clamp(Accumulation[y * Screen.width + x] / frames, Vector3.Zero, Vector3.One));
                }
            }
        });

        // Prints useful information to the user's window.
        Screen.Print($"FOV: {Camera.FOV}", 15, 20, 0xffffff);
        Screen.Print($"Pitch: {Camera.Pitch}", 15, 50, 0xffffff);
        Screen.Print($"Yaw: {Camera.Yaw}", 15, 80, 0xffffff);
        Screen.Print($"Samples: {frames}", 15, 110, 0xffffff);
    }
}
