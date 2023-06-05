﻿using System.Diagnostics;
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

        Parallel.For(0, Screen.height, y =>
        {
            Parallel.For (0, Screen.width/2,  x =>
            {
                Vector3 point = Camera.P0 + ((float)x / ((float)Screen.width / 2)) * Camera.U + ((float)y / (float)Screen.height) * Camera.V;
                point = Vector3.Normalize(point - Camera.Position);

                Ray viewRay = new(Camera.Position, point, 16);
               Intersection intersect = new(this);

                Vector3 colorV = intersect.TraceRay(viewRay);
                Screen.pixels[y * Screen.width + x] = Utilities.ColorToInt(colorV);
            });
        });

        sw.Stop();
        Console.WriteLine(sw.ElapsedMilliseconds);
        // Prints useful information to the user's window.
        Screen.Print($"FOV: {Camera.FOV}", 15, 20, 0xffffff);
        Screen.Print($"Pitch: {Camera.Pitch}", 15, 50, 0xffffff);
        Screen.Print($"Yaw: {Camera.Yaw}", 15, 80, 0xffffff);
    }
}
