using System.Runtime.InteropServices.ComTypes;
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
                int x = (int)((sphere.Center.X + sphere.Radius) * scale + offsetX + translate);
                // Console.WriteLine($"Cx: {Sphere.Center.X}, Cz: {Sphere.Center.Z}, R: {Sphere.Radius}, S: {scale}, O: {offsetX}, T: {translate}");
                int y = (int)((-sphere.Center.Z) * scale + translate);

                for (float t = 0; t <= 2 * Math.PI + 1; t += (float)(2 * Math.PI / 100))
                {
                    int tempX = (int)((sphere.Center.X + Math.Cos(t) * sphere.Radius) * scale + offsetX + 2 * translate);
                    // Console.WriteLine(Utilities.TranslateX(Screen, Sphere.Center.X));
                    // Console.WriteLine(Utilities.TranslateX(Screen, (int)((Sphere.Center.X + Math.Cos(t) * Sphere.Radius))));
                    int tempY = (int)((-sphere.Center.Z + Math.Sin(t) * sphere.Radius) * scale + translate);
                    Screen.Line(x, y,
                        tempX, tempY, 255);
                    x = tempX;
                    y = tempY;
                }
            }
        }

        // Screen.Line(Utilities.TranslateX(Screen, Sphere.Center.X) - 5,
        //     Utilities.TranslateZ(Screen, Sphere.Center.Z) - 5,
        //     Utilities.TranslateX(Screen, Sphere.Center.X) + 5, Utilities.TranslateZ(Screen, Sphere.Center.Z) + 5,
        //     255 << 16);
    }
}