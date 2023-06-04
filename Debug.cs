using OpenTK.Mathematics;

namespace INFOGR2023Template;

public class Debug
{
    // Used to access certain methods or fields from the respective classes.
    private readonly Surface _screen;
    private readonly Scene _scene;

    // Counters to keep track of the amount of rays that have been drawn.
    private int _count;  // primary rays
    private int _count1; // shadow rays
    private int _count2; // reflection rays

    public Debug(Surface screen, Scene scene)
    {
        _screen = screen;
        _scene = scene;
    }

    // Must be run separately as the primitives are never drawn on their own in the main Render method.
    public void Render() => DrawPrimitives();

    private void DrawPrimitives()
    {
        foreach (Primitive p in _scene.Primitives)
            if (p is Sphere sphere)
            {
                // Increment the denominator for smoother circles.
                const float smoothness = MathHelper.TwoPi / 200f;

                int x1 = -1, y1 = -1;

                for (float theta = 0; theta < MathHelper.TwoPi; theta += smoothness)
                {
                    int x = Utilities.TranslateX(_screen, (float)(sphere.Center.X + sphere.Radius * MathHelper.Cos(theta)));
                    int y = Utilities.TranslateZ(_screen, (float)(sphere.Center.Z + sphere.Radius * MathHelper.Sin(theta)));

                    // Prevent loose cannons.
                    if (x1 > -1 && y1 > -1)
                        _screen.Line(x1, y1, x, y, Utilities.ColorToInt(sphere.Color));

                    x1 = x; y1 = y;
                }
            }
    }

    /// <summary>
    /// Draw various rays on the debug window.
    /// </summary>
    /// <param name="start">The starting point of the ray.</param>
    /// <param name="end">The destination of the ray.</param>
    /// <param name="ray">The type of ray.</param>
    public void DrawRays(Vector3 start, Vector3 end, Utilities.Ray ray)
    {
        // Every <max>th amount of rays will be printed onto the debug window.
        int max = (ray switch
        {
            Utilities.Ray.Primary => 10000,
            Utilities.Ray.Shadow => 200,
            _ => 500
        });

        // Determine which color to use for the ray.
        int color = (ray switch
        {
            Utilities.Ray.Primary => 0xff0000, // dark red
            Utilities.Ray.Shadow => 0xbbbbbb, // (light) gray
            _ => 0x87ceeb // light blue
        });

        // Determine which count to increment.
        switch (ray)
        {
            case Utilities.Ray.Primary:
                _count++;
                break;
            case Utilities.Ray.Shadow: // TODO: fix shadow rays
                _count1++;
                break;
            default:
                _count2++;
                break;
        }

        // Actually (attempt to) draw the ray on the debug window with the appropriate color.
        if (ray switch
            {
                Utilities.Ray.Primary => _count % max == 0,
                Utilities.Ray.Shadow => _count1 % max == 0,
                _ => _count2 % max == 0
            })
        {
            _screen.Line(Utilities.TranslateX(_screen, start.X), Utilities.TranslateZ(_screen, start.Z),
                Utilities.TranslateX(_screen, end.X), Utilities.TranslateZ(_screen, end.Z), color);
        }
    }
}