using OpenTK.Mathematics;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace INFOGR2023Template;

public class Debug
{
    public Surface Screen;
    public List<Primitive> Primitives;

    private float scale;
    private int offsetX, translate;

    private int color;

    public Debug(Surface screen, List<Primitive> primitives)
    {
        Screen = screen;
        Primitives = primitives;

        scale = (screen.width / 2) / 30f;
        offsetX = screen.width / 2;
        translate = (screen.width / 2) / 2;

        color = Utilities.ColorToInt(new Vector3(1, 1, 0));
    }

    public void Render() => DrawPrimitives();

    private void DrawPrimitives()
    {
        foreach (Primitive p in Primitives)
        {
            if (p is Sphere sphere)
            {
                Vector2 center = new(sphere.Center.X, sphere.Center.Z);
                float radius = sphere.Radius;

                const float step = MathHelper.TwoPi / 100f;
                int x1 = -1, y1 = -1;

                for (float t = 0; t < MathHelper.TwoPi; t += step)
                {
                    float tempX = center.X + radius * (float)MathHelper.Cos(t);
                    float tempY = center.Y + radius * (float)MathHelper.Sin(t);

                    // tempX /= 5f;
                    // tempY /= 5f;
                    
                    int x = Utilities.TranslateX(Screen, tempX);
                    int y = Utilities.TranslateX(Screen, tempY) / 2;

                    if (x1 > -1 && y1 > -1)
                    {
                        Screen.Line(x1, y1, x, y, 255);
                        Console.WriteLine($"Center: {Utilities.TranslateX(Screen, center.X)}, {Utilities.TranslateZ(Screen, center.Y)}");
                        // Console.WriteLine(x1 + ", " + y1 + ", " + x + ", " + y);
                    }

                    x1 = x;
                    y1 = y;
                }

                // int x = (int)((sphere.Center.X + sphere.Radius) * scale + offsetX + translate);
                // // Console.WriteLine($"Cx: {Sphere.Center.X}, Cz: {Sphere.Center.Z}, R: {Sphere.Radius}, S: {scale}, O: {offsetX}, T: {translate}");
                // int y = (int)((-sphere.Center.Z) * scale + translate);

                // for (float t = 0; t <= 2 * Math.PI + 1; t += (float)(2 * Math.PI / 100))
                // {
                //     int tempX = (int)((sphere.Center.X + Math.Cos(t) * sphere.Radius) * scale + offsetX + 2 * translate);
                //     // Console.WriteLine(Utilities.TranslateX(Screen, Sphere.Center.X));
                //     // Console.WriteLine(Utilities.TranslateX(Screen, (int)((Sphere.Center.X + Math.Cos(t) * Sphere.Radius))));
                //     int tempY = (int)((-sphere.Center.Z + Math.Sin(t) * sphere.Radius) * scale + translate);
                //     Screen.Line(x, y,
                //         tempX, tempY, 255);
                //     x = tempX;
                //     y = tempY;
                // }
            }
        }

        // Screen.Line(Utilities.TranslateX(Screen, Sphere.Center.X) - 5,
        //     Utilities.TranslateZ(Screen, Sphere.Center.Z) - 5,
        //     Utilities.TranslateX(Screen, Sphere.Center.X) + 5, Utilities.TranslateZ(Screen, Sphere.Center.Z) + 5,
        //     255 << 16);
    }
}