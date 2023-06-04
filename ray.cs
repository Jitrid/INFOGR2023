using Vector3 = OpenTK.Mathematics.Vector3;

namespace INFOGR2023Template;

/// <summary>
/// Represents a single ray shot from a certain point with a direction.
/// </summary>
public class Ray
{
    /// <summary>
    /// The location from which the ray was shot.
    /// </summary>
    public Vector3 Origin { get; set; }
    /// <summary>
    /// The direction in which the ray is headed.
    /// </summary>
    public Vector3 Direction { get; set; }

    /// <summary>
    /// The maximum amount of times the ray should bounce for reflections.
    /// </summary>
    public int MaxBounces;

    public Ray(Vector3 origin, Vector3 direction, int maxBounces)
    {
        Origin = origin;
        Direction = direction;
        MaxBounces = maxBounces;
    }
}
