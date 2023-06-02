using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace INFOGR2023Template;

/// <summary>
/// Represents the camera from which the user looks at the scene.
/// </summary>
public class Camera
{
    /// <summary>
    /// The current position of the camera (default: 0, 0, 0).
    /// </summary>
    public Vector3 Position;
    /// <summary>
    /// The direction in which the camera is pointed (z-axis).
    /// </summary>
    public Vector3 Direction;
    /// <summary>
    /// The "up" direction of the camera (y-axis).
    /// </summary>
    public Vector3 Up;
    /// <summary>
    /// The "right" direction of the camera (x-axis).
    /// </summary>
    public Vector3 Right;
    /// <summary>
    /// The distance between the camera and the screen plane.
    /// </summary>
    public float ScreenDistance;

    /// <summary>
    /// The Field of View of the camera.
    /// </summary>
    public float FOV;
    /// <summary>
    /// The sensitivity for moving the camera.
    /// </summary>
    public float Sensitivity;

    /// <summary>
    /// The rotation around the side-to-side axis.
    /// </summary>
    public float Pitch;
    /// <summary>
    /// The rotation around the vertical axis.
    /// </summary>
    public float Yaw;

    /// <summary>
    /// The current aspect ratio of screen, excluding the debug window.
    /// </summary>
    public float AspectRatio;

    /// <summary>
    /// The image plane center.
    /// </summary>
    public Vector3 ImagePlaneCenter;
    /// <summary>
    /// The top-left position on the screen.
    /// </summary>
    public Vector3 P0;
    /// <summary>
    /// The top-right position on the screen.
    /// </summary>
    public Vector3 P1;
    /// <summary>
    /// The bottom-left position on the screen.
    /// </summary>
    public Vector3 P2;
    
    // These together form an orthonormal basis.
    public Vector3 U;
    public Vector3 V;

    public Camera(float ratio, Vector3 pos)
    {
        Position = pos; // Starting value: (0, 0, 0)
        Direction = Vector3.UnitZ; // (0, 0, 1)
        Up = Vector3.UnitY;        // (0, 1, 0)
        Right = Vector3.UnitX;     // (1, 0, 0)

        AspectRatio = ratio;

        Pitch = 0f;
        Yaw = 0f;
        FOV = 60f;
        Sensitivity = 0.1f;

        // Initialize the first values of the vectors.
        UpdateVectors(AspectRatio);
    }

    /// <summary>
    /// Generates a ray from the camera to a certain point.
    /// </summary>
    /// <param name="point">The point to send the ray to.</param>
    public Ray GetRay(Vector3 point)
    {
        Vector3 rayDirection = Vector3.Normalize(point - Position);
            
        return new Ray(Position, rayDirection);
    }

    /// <summary>
    /// Updates the various vectors available within the camera class when the position and/or aspect ratio are updated.
    /// </summary>
    /// <param name="ratio">The current aspect ratio of the left side of the screen.</param>
    public void UpdateVectors(float ratio)
    {
        ScreenDistance = Vector3.Distance(Vector3.Zero, ratio * Right) / 
                         (float)MathHelper.Tan(MathHelper.DegreesToRadians(0.5 * FOV));

        ImagePlaneCenter = Position + ScreenDistance * Direction;
        P0 = ImagePlaneCenter + Up - ratio * Right;
        P1 = ImagePlaneCenter + Up + ratio * Right;
        P2 = ImagePlaneCenter - Up - ratio * Right;

        U = P1 - P0;
        V = P2 - P0;
    }

    /// <summary>
    /// Moves the camera on one of the axis based on certain key binds.
    /// </summary>
    /// <param name="time">The frame's "delta time" to determine performance and generalize the effect for all systems.</param>
    public void CameraKeyboardInput(KeyboardKeyEventArgs kea, float time)
    {
        const float speed = 0.15f;

        switch (kea.Key)
        {
            // forwards
            case Keys.W or Keys.Up:
                Position.Z += speed * time;
                Direction.Z += speed * time;
                break;
            // backwards
            case Keys.S or Keys.Down:
                Position.Z -= speed * time;
                Direction.Z -= speed * time;
                break;
            // left
            case Keys.A or Keys.Left:
                Position.X -= speed * time;
                Direction.X -= speed * time;
                break;
            // right
            case Keys.D or Keys.Right:
                Position.X += speed * time;
                Direction.X += speed * time;
                break;
            // up
            case Keys.Space:
                Position.Y += speed * time;
                Direction.Y += speed * time;
                break;
            // down
            case Keys.LeftShift:
                Position.Y -= speed * time;
                Direction.Y -= speed * time;
                break;
        }

        UpdateVectors(AspectRatio);
    }

    /// <summary>
    /// Rotates the camera accordingly based on mouse movement.
    /// </summary>
    public void RotationInput(MouseMoveEventArgs mea)
    {
        Yaw += mea.DeltaX * Sensitivity;
        Pitch -= mea.DeltaY * Sensitivity;

        switch (Pitch)
        {
            case > 89.0f:
                Pitch = 89.0f;
                break;
            case < -89.0f:
                Pitch = -89.0f;
                break;
            default:
                Pitch -= mea.DeltaX * Sensitivity;
                break;
        }

        Quaternion xRotation = Quaternion.FromEulerAngles(MathHelper.DegreesToRadians(-Pitch), 0, 0);
        Direction = Vector3.Transform(Vector3.UnitZ, xRotation);
        Up = Vector3.Transform(Vector3.UnitY, xRotation);
        Right = Vector3.Transform(Vector3.UnitX, xRotation);

        Quaternion yRotation = Quaternion.FromEulerAngles(0, MathHelper.DegreesToRadians(Yaw), 0);
        Direction = Vector3.Transform(Direction, yRotation);
        Up = Vector3.Transform(Up, yRotation);
        Right = Vector3.Transform(Right, yRotation);

        UpdateVectors(AspectRatio);
    }

    /// <summary>
    /// Adjusts the camera's FOV to create a "zoom" effect.
    /// </summary>
    public void CameraZoom(MouseWheelEventArgs mea)
    {
        FOV -= mea.OffsetY;

        UpdateVectors(AspectRatio);
    }
}
