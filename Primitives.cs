using OpenTK.Mathematics;

namespace INFOGR2023Template;

/// <summary>
/// Represents a primitive in the scene.
/// </summary>
public abstract class Primitive
{
    /// <summary>
    /// Indicates whether a ray has intersected with the primitive.
    /// </summary>
    /// <param name="ray">The ray to determine the intersection for.</param>
    /// <param name="intersect">The location of the intersection.</param>
    /// <returns>True if there is an intersection, false otherwise.</returns>
    public abstract bool HitRay(Ray ray, out Vector3 intersect);

    /// <summary>
    /// Determines the normal vector of a given point.
    /// </summary>
    /// <param name="point">The point that should be normalized.</param>
    /// <returns>The normalized vector.</returns>
    public abstract Vector3 GetNormal(Vector3 point);

    /// <summary>
    /// Determines whether a light source intersects with the primitive.
    /// </summary>
    /// TODO: add paramater descriptions.
    /// <param name="intersectionPoint"></param>
    /// <param name="lightPosition"></param>
    /// <param name="direction"></param>
    /// <returns>True if there is an intersection, false otherwise.</returns>
    public abstract bool IntersectsWithLight(Vector3 intersectionPoint, Vector3 lightPosition, out Vector3 direction);
}
