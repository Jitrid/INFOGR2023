using OpenTK.Mathematics;

namespace INFOGR2023Template;

/// <summary>
/// Represents a primitive in the scene.
/// </summary>
public abstract class Primitive
{
    public Vector3 DiffuseColor { get; set; }
    public Vector3 SpecularColor { get; set; }
    public float SpecularPower { get; set; }

    /// <summary>
    /// Determines the level of reflectivity (between 0f and 1f).
    /// </summary>
    public float ReflectionCoefficient { get; set; }

    /// <summary>
    /// Determines the normal vector of a given point.
    /// </summary>
    /// <param name="point">The point that should be normalized.</param>
    /// <returns>The normalized vector.</returns>
    public abstract Vector3 GetNormal(Vector3 point);
    /// <summary>
    /// Returns the primitive's color.
    /// </summary>
    /// <returns></returns>
    public abstract Vector3 GetColor();

    /// <summary>
    /// Indicates whether a ray has intersected with the primitive.
    /// </summary>
    /// <param name="ray">The ray to determine the intersection for.</param>
    /// <param name="intersect">The location of the intersection.</param>
    /// <returns>True if there is an intersection, false otherwise.</returns>
    public abstract bool HitRay(Ray ray, out Vector3 intersect);
}
