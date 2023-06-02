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

    public Ray(Vector3 origin, Vector3 direction)
    {
        Origin = origin;
        Direction = direction;
    }

    public Vector3 Parametric(float t) => Origin + t * Direction;
}