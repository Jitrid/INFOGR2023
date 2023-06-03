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

    public void Render() => DrawPrimitives();

    private void DrawPrimitives()
    {
        foreach (Primitive p in _scene.Primitives)
        {
            if (p is Sphere sphere)
            {
                // Increment the denominator for smoother circles.
                const float smoothness = MathHelper.TwoPi / 20000f;

                int x1 = -1, y1 = -1;

                for (float theta = 0; theta < MathHelper.TwoPi; theta += smoothness)
                {
                    float tempX = (float)(sphere.Center.X + sphere.Radius * MathHelper.Cos(theta));
                    float tempY = (float)(sphere.Center.Z + sphere.Radius * MathHelper.Sin(theta)); // / 2f

                    int x = Utilities.TranslateX(_screen, tempX);
                    int y = Utilities.TranslateZ(_screen, tempY);

                    if (x1 > -1 && y1 > -1)
                        _screen.Line(x1, y1, x, y, Utilities.ColorToInt(sphere.Color));

                    x1 = x;
                    y1 = y;
                }
            }
        }
    }

    public void DrawRays(Vector3 start, Vector3 end, Utilities.Ray ray, int i)
    {
        // Every <max>th amount of rays will be printed onto the debug window.
        int max = (ray switch
        {
            Utilities.Ray.Primary => 100,
            Utilities.Ray.Shadow => 200,
            _ => 500
        });

        // Determine which color to use for the ray.
        int color = (ray switch
        {
            Utilities.Ray.Primary => Utilities.ColorToInt(new Vector3(1, 1, 0)), // yellow
            Utilities.Ray.Shadow => 0xbbbbbb, // (light) gray
            _ => 255 // dark blue
        });

        // Determine which count to increment.
        switch (ray)
        {
            case Utilities.Ray.Primary:
                _count++;
                break;
            case Utilities.Ray.Shadow:
                _count1++;
                break;
            default:
                _count2++;
                break;
        }

        // Actually (attempt to) draw the ray on the debug window with the appropriate color.
        // if (i != _screen.height / 2) return;
        if (ray switch
            {
                Utilities.Ray.Primary => _count == 1,
                Utilities.Ray.Shadow => _count1 > 1,
                _ => _count2 == 1
            })
            _screen.Line(Utilities.TranslateX(_screen, start.X), Utilities.TranslateZ(_screen, start.Z),
                Utilities.TranslateX(_screen, end.X), Utilities.TranslateZ(_screen, end.Z), color);
    }
}