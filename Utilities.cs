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

    // Color methods.
    public static int GetColor(float x, float y, float z) => (int)(255 * x + 255 * y + 255 * z);

    public static Vector3 CalculateColor(Vector3 intersectionPoint)
    {
        // Determine the color based on the position of the intersection point
        if (intersectionPoint.X < 0 && intersectionPoint.Z < 0)
        {
            return new Vector3(1f, 0f, 0f); // Red
        }
        else if (intersectionPoint.X >= 0 && intersectionPoint.Z >= 0)
        {
            return new Vector3(0f, 1f, 0f); // Green
        }
        else
        {
            return new Vector3(0f, 0f, 1f); // Blue
        }
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

    public static Vector3 ShadeColor(Vector3 lightIntensity, Vector3 lightDirection, Vector3 viewDirection,
        Vector3 normal, Vector3 diffuseColor, float r)
    {
        Vector3 ambientLightning = diffuseColor * new Vector3(0.3f, 0.3f, 0.3f);

        Vector3 radiance = lightIntensity * (1 / (r * r));

        // Determine specifics for diffuse materials.
        float dot = Vector3.Dot(normal, lightDirection);
        if (dot < 0) // > 90dg
            return ambientLightning;

        float diffuseCoefficient = Math.Max(0, dot);

        // Determine specifics for specular (glossy) materials.
        Vector3 reflectionDirection = lightDirection - 2 * (dot) * normal;

        float specularPower = 10f;
        float specularCoefficient = (float)Math.Pow(Math.Max(0, Vector3.Dot(viewDirection, reflectionDirection)), specularPower);
        float k = 1f;
        Vector3 specularColor = new(k, k, k);

        // Combine both materials.
        Vector3 shadedColor = radiance * ((diffuseCoefficient * diffuseColor) + (specularCoefficient * specularColor));

        // Add ambient lighting.
        shadedColor += ambientLightning;

        if (shadedColor.X > 1) shadedColor.X = 1;
        if (shadedColor.Y > 1) shadedColor.Y = 1;
        if (shadedColor.Z > 1) shadedColor.Z = 1;

        return shadedColor;
    }
}
