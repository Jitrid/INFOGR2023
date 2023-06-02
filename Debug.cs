using OpenTK.Mathematics;

namespace INFOGR2023Template;

public class Debug
{
    public Surface Screen;
    public Camera Camera;
    public List<Primitive> Primitives;

    public Debug(Surface screen, Camera camera, List<Primitive> primitives)
    {
        Screen = screen;
        Camera = camera;
        Primitives = primitives;
    }

    public void Render() => DrawPrimitives();

    private void DrawPrimitives()
    {
        foreach (Primitive p in Primitives)
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

                    int x = Utilities.TranslateX(Screen, tempX);
                    int y = Utilities.TranslateZ(Screen, tempY);

                    if (x1 > -1 && y1 > -1)
                        Screen.Line(x1, y1, x, y, Utilities.ColorToInt(sphere.Color));

                    x1 = x;
                    y1 = y;
                }
            }
        }
    }

    private int _count;

    public void DrawRays(Vector3 start, Vector3 end, Utilities.Ray ray, int i)
    {
        // Primary rays
        if (ray == Utilities.Ray.Primary)
        {
            _count++;
            // Console.WriteLine(i + " - " + Screen.height / 2);
            if (i == Screen.height / 2)
                if (_count % 50 == 0)
                    Screen.Line(Utilities.TranslateX(Screen, start.X), Utilities.TranslateZ(Screen, start.Z),
                        Utilities.TranslateX(Screen, end.X), Utilities.TranslateZ(Screen, end.Z), Utilities.ColorToInt(new Vector3(1, 1, 0)));
        }

        //if (c == 0)
        //{
        //    Screen.Line(Utilities.TranslateX(Screen, Camera.Position.X),
        //        Utilities.TranslateZ(Screen, Camera.Position.Z),
        //        Utilities.TranslateX(Screen, (float)(x)), Utilities.TranslateZ(Screen, (float)y), 255 << 8);

        //    // Console.WriteLine(x + ", " + y);
        //}

        //c++;
    }
}