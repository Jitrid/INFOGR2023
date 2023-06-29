using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace Rasterization;

/// <summary>
/// Represents an idividual light source in the scene.
/// </summary>
public struct LightSource
{
    public Vector3 Position;
    public Vector3 Color;
    /// <summary>
    /// Temporary variable to store the colour while the Lights are off.
    /// </summary>
    public Vector3 PreviousColor;

    public LightSource(Vector3 position, Vector3 color)
    {
        Position = position;
        Color = color;
        PreviousColor = Vector3.Zero;
    }
}

/// <summary>
/// A general utilities class for the light sources.
/// </summary>
public class Light
{
    public static bool TriggerWarningDoNotTurnOnIfEpilepticWeAreNotLiableInCourt;
    public static bool LightsEnabled = true;
    public static readonly LightSource[] Lights =
    {
        new(new Vector3(-10f, 40f, -50f), new Vector3(500f)),
        new(new Vector3(0, 50f, 20f), new Vector3(750f)),
        new(new Vector3(-20f, 50f, 0), new Vector3(750f)),
        new(new Vector3(0, 30f, -20f), new Vector3(350f))
    };

    /// <summary>
    /// Generate the disco effect based on random integers.
    /// </summary>
    /// <returns></returns>
    public static Vector3 GenerateDisco()
    {
        Random rnd = new();

        return new Vector3(rnd.Next(0, 256), rnd.Next(0, 256), rnd.Next(0, 256)) * 5;
    }

    /// <summary>
    /// Adjusts certain light settings based on the user input.
    /// </summary>
    public static void AdjustLights(KeyboardKeyEventArgs kea)
    {
        switch (kea)
        {
            case { Key: Keys.Q }:
            {
                LightsEnabled = !LightsEnabled;

                for (int i = 0; i < Lights.Length; i++)
                {
                    // Save the current colour in order to enable it later on.
                    if (!LightsEnabled) Lights[i].PreviousColor = Lights[i].Color;
                    Lights[i].Color = LightsEnabled ? Lights[i].PreviousColor : Vector3.Zero;
                    TriggerWarningDoNotTurnOnIfEpilepticWeAreNotLiableInCourt = false;
                }

                break;
            }
            case { Key: Keys.E }:
                if (!LightsEnabled) break;
                TriggerWarningDoNotTurnOnIfEpilepticWeAreNotLiableInCourt = !TriggerWarningDoNotTurnOnIfEpilepticWeAreNotLiableInCourt;
                break;
        }
    }
}
