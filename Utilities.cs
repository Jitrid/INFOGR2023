using Vector3 = OpenTK.Mathematics.Vector3;

namespace INFOGR2023Template;

public class Utilities
{
    public enum Ray
    {
        Primary,
        Shadow,
        Reflection
    }

    /// <summary>
    /// The amount of steps to be used for the coordinate system.
    /// </summary>
    public const int Steps = 32;

    /// <summary>
    /// Translates an x-axis coordinate into an appropriate screen width.
    /// </summary>
    public static int TranslateX(Surface screen, float x)
    {
        float range = screen.width - screen.width / 2;
        float stepSize = range / Steps;

        float translatedX = screen.width / 2 + (x + Steps / 2) * stepSize;
        
        if (translatedX < screen.width / 2)
            return screen.width / 2;
        if (translatedX > screen.width)
            return screen.width;

        return (int)translatedX;
    }
    /// <summary>
    /// Translates a z-axis coordinate into an appropriate screen width.
    /// </summary>
    public static int TranslateZ(Surface screen, float z)
    {
        float range = screen.height;
        float stepSize = range / Steps;

        float translatedZ = (-z + Steps / 2) * stepSize;

        if (translatedZ > screen.height)
            return screen.height;
        if (translatedZ < 0)
            return 0;

        return (int)translatedZ;
    }

    public static System.Numerics.Vector3 VectorToSystem(Vector3 input)
    {
        System.Numerics.Vector3 output = new()
        {
            X = input.X,
            Y = input.Y,
            Z = input.Z
        };
        return output;
    }

    //Nog meer onzin
    public static Vector3 VectorToGL(System.Numerics.Vector3 input)
    {
        Vector3 output = new()
        {
            X = input.X,
            Y = input.Y,
            Z = input.Z
        };
        return output;
    }

    /// <summary>
    /// Converts a vector (0-1) to RGB values in order to display colors.
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static int ColorToInt(Vector3 color)
    {
        int r = (int)(color.X * 255);
        int g = (int)(color.Y * 255);
        int b = (int)(color.Z * 255);

        return (r << 16) | (g << 8) | b;
    }

    public static Vector3 ResolveOutOfBounds(Vector3 color)
    {
        if (color.X > 1f) color.X = 1f;
        if (color.Y > 1f) color.Y = 1f;
        if (color.Z > 1f) color.Z = 1f;

        return color;
    }
}
