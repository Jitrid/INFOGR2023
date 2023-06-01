﻿using Vector3 = OpenTK.Mathematics.Vector3;

namespace INFOGR2023Template;

public class Utilities
{
    /// <summary>
    /// The amount of steps to be used for the coordinate system.
    /// </summary>
    public const int Steps = 16;

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
    public static int ColorToInt(Vector3 color)
    {
        int r = (int)(color.X * 255);
        int g = (int)(color.Y * 255);
        int b = (int)(color.Z * 255);

        return (r << 16) | (g << 8) | b;
    }
}