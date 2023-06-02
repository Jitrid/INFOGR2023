using Vector3 = OpenTK.Mathematics.Vector3;

namespace INFOGR2023Template;

public class Utilities
{
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

    public int ItTakesAllColoursToMakeARainbow(int lightColor, Vector3 lightDirection, Vector3 viewDirection,
    Vector3 normal, int objectColor, float specularIntensity, float specularPower,
    float distancelichtsterkteAfname)
    {
        float lightRed = ((lightColor >> 16) & 255) / 255f;
        float lightGreen = ((lightColor >> 8) & 255) / 255f;
        float lightBlue = (lightColor & 255) / 255f;

        float objectRed = ((objectColor >> 16) & 255) / 255f;
        float objectGreen = ((objectColor >> 8) & 255) / 255f;
        float objectBlue = (objectColor & 255) / 255f;

        if (Vector3.Dot(normal, lightDirection) < 0)
            return 0x000000;

        float diffuseCoefficient = Math.Max(0, Vector3.Dot(normal, lightDirection));

        //reflection direction kut vectoren uit GL hebben geen .Reflect dus dan maar ombouwen... Kms
        System.Numerics.Vector3 systemReflectionDirection =
            System.Numerics.Vector3.Reflect(Utilities.VectorToSystem(-lightDirection), Utilities.VectorToSystem(normal));
        Vector3 reflectionDirection = Utilities.VectorToGL(systemReflectionDirection);

        float specularCoefficient =
            (float)Math.Pow(Math.Max(0, Vector3.Dot(viewDirection, reflectionDirection)), specularPower) *
            specularIntensity;

        float lichtsterkteAfname = 1 / (distancelichtsterkteAfname * distancelichtsterkteAfname);

        float shadedRed = lightRed * objectRed * (diffuseCoefficient + specularCoefficient) * lichtsterkteAfname;
        float shadedGreen = lightGreen * objectGreen * (diffuseCoefficient + specularCoefficient) * lichtsterkteAfname;
        float shadedBlue = lightBlue * objectBlue * (diffuseCoefficient + specularCoefficient) * lichtsterkteAfname;

        int fixRed = (int)(shadedRed * 255f);
        int fixGreen = (int)(shadedGreen * 255f);
        int fixBlue = (int)(shadedBlue * 255f);

        return (fixRed << 16) | (fixGreen << 8) | fixBlue;
    }
}