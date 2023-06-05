using Vector3 = OpenTK.Mathematics.Vector3;

namespace INFOGR2023Template;

public class Utilities
{
    /// <summary>
    /// Represents the different types of rays available in the application.
    /// </summary>
    public enum Ray
    {
        Primary,
        Shadow,
        Reflection // secondary
    }

    /// <summary>
    /// The amount of steps to be used for the coordinate system.
    /// This should be increased to show more of the scene, decreased for larger primitives.
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

    /// <summary>
    /// Converts a vector (0-1) to RGB values in order to display colors.
    /// </summary>
    /// <param name="color"></param>
    public static int ColorToInt(Vector3 color)
    {
        int r = (int)(color.X * 255);
        int g = (int)(color.Y * 255);
        int b = (int)(color.Z * 255);

        return (r << 16) | (g << 8) | b;
    }

    /// <summary>
    /// Adjust a color vector to never go out of bounds and be restricted to a maximum of 1f (255).
    /// </summary>
    public static Vector3 ResolveOutOfBounds(Vector3 color)
    {
        if (color.X > 1f) color.X = 1f;
        if (color.Y > 1f) color.Y = 1f;
        if (color.Z > 1f) color.Z = 1f;

        return color;
    }
}
