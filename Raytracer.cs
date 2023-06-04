using Vector3 = OpenTK.Mathematics.Vector3;

namespace INFOGR2023Template;

internal class Raytracer
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

    public Raytracer(Surface screen)
    {
        Screen = screen;
        Scene = new Scene();

        Debug = new Debug(screen, Scene);

        Camera = new Camera(Screen, new Vector3(0f, 1.5f, -4f));
    }

    /// <summary>
    /// Renders the visualisation of generated rays shot to all pixels on the screen.
    /// </summary>
    public void Render()
    {
        Screen.Clear(0);
        Debug.DrawPrimitives();

        Parallel.For(0, Screen.height, y =>
        {
            Parallel.For (0, Screen.width/2,  x =>
            {
                Vector3 point = Camera.P0 + ((float)x / ((float)Screen.width / 2)) * Camera.U + ((float)y / (float)Screen.height) * Camera.V;
                point = Vector3.Normalize(point - Camera.Position);

                Ray viewRay = new(Camera.Position, point);

                Vector3 colorV = Intersection.TraceRay(Debug, Camera, viewRay, Scene, 16);
                Screen.pixels[y * Screen.width + x] = Utilities.ColorToInt(colorV);
            });
        });

        // Prints useful information to the user's window.
        Screen.Print($"FOV: {Camera.FOV}", 15, 20, 0xffffff);
        Screen.Print($"Pitch: {Camera.Pitch}", 15, 50, 0xffffff);
        Screen.Print($"Yaw: {Camera.Yaw}", 15, 80, 0xffffff);
    }
}
