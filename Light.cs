using Vector3 = OpenTK.Mathematics.Vector3;

namespace INFOGR2023Template;

/// <summary>
/// Represents a light source in the scene that omits light at a certain intensity.
/// </summary>
public class Light
{
    /// <summary>
    /// The location of the light source.
    /// </summary>
    public Vector3 Location;

    /// <summary>
    /// Represent their respective color values.
    /// </summary>
    public float R, G, B;

    public Light(Vector3 location, float red, float green, float blue)
    {
        Location = location;
        R = red;
        G = green;
        B = blue;
    }
}
