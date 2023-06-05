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
    public Vector3 Intensity;

    public Light(Vector3 location, Vector3 intensity)
    {
        Location = location;
        Intensity = intensity;
    }
}
