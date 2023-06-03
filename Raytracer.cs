using Vector3 = OpenTK.Mathematics.Vector3;

namespace INFOGR2023Template;

internal class Raytracer
{
    private readonly Surface _screen;
    private readonly Debug _debug;
    private readonly Scene _scene;
    public Camera Camera;

    public Raytracer(Surface screen)
    {
        _screen = screen;
        _scene = new Scene();

        Camera = new Camera(new Vector3(0f, 1.5f, -4f));

        _debug = new Debug(screen, _scene);
    }

    public void DebugRender() => _debug.Render();

    public void Render()
    {
        _screen.Clear(0);
        _debug.Render();

        Parallel.For(0, _screen.height, i => // y
        {
            Parallel.For (0, _screen.width/2,  j =>  // x
            {
                _screen.pixels[i * _screen.width + j] = 0x304080;

                Vector3 punty = Camera.P0 + ((float)j / ((float)_screen.width / 2)) * Camera.U + ((float)i / (float)_screen.height) * Camera.V;
                punty = Vector3.Normalize(punty - Camera.Position);

                Ray viewRay = new(Camera.Position, punty);

                Vector3 colorV = Intersection.TraceRay(_debug, Camera, viewRay, _scene, 5, i);
                int color = Utilities.ColorToInt(colorV);
                _screen.pixels[i * _screen.width + j] = color;
            });
        });

        // Prints additional information to the debug window as displayable text.
        _screen.Print($"P0: {Camera.P0}", (_screen.width / 2) + 20, _screen.height - 30, 0xffffff);
        _screen.Print($"P1: {Camera.P1}", (_screen.width / 2) + 20, _screen.height - 60, 0xffffff);
        _screen.Print($"P2: {Camera.P2}", (_screen.width / 2) + 20, _screen.height - 90, 0xffffff);
        _screen.Print($"Pos: {Camera.Position}", (_screen.width / 2) + 20, _screen.height - 120, 0xffffff);
    }
}
