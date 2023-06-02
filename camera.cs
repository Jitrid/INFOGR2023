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
    public Vector3 Forward;
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
    public double FOV;
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

    public Camera(Vector3 pos, Vector3 target)
    {
        Position = pos; // Starting value: (0, 0, 0)
        Direction = target; // (0, 0, 1)
        // Up = Vector3.UnitY;        // (0, 1, 0)
        // Right = Vector3.UnitX;     // (1, 0, 0)

        Pitch = 0f;
        Yaw = 0f;
        FOV = 40 * Math.PI / 180;
        Sensitivity = 0.1f;

        // Initialize the first values of the vectors.
        UpdateVectors();
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
    public void UpdateVectors()
    {
        Forward = Vector3.Normalize(Direction - Position); // T - E
        float screenDistance = (float)(2 * Math.Tan(FOV / 2));

        ImagePlaneCenter = Position + Forward;

        Vector3 right = Vector3.Normalize(Vector3.Cross(new Vector3(0, 1, 0), Forward));
        Vector3 up = Vector3.Cross(Forward, right);

        P0 = ImagePlaneCenter - (screenDistance / 2) * right + (screenDistance / 2) * up;
        P1 = P0 + screenDistance * right;
        P2 = P0 - screenDistance * up;

        U = P1 - P0;
        V = P2 - P0;
    }

    /// <summary>
    /// Moves the camera on one of the axis based on certain key binds.
    /// </summary>
    /// <param name="time">The frame's "delta time" to determine performance and generalize the effect for all systems.</param>
    public void CameraKeyboardInput(KeyboardKeyEventArgs kea, float time)
    {
        const float speed = 0.1f;
        time = 1;

        // forwards
        if (kea.Key is Keys.W or Keys.Up)
        {
            Position.Z += speed * time;
            Direction.Z += speed * time;
        }
        // backwards
        if (kea.Key is Keys.S or Keys.Down)
        {
            Position.Z -= speed * time;
            Direction.Z -= speed * time;
        }
        // left
        if (kea.Key is Keys.A or Keys.Left)
        {
            Position.X -= speed * time;
            Direction.X -= speed * time;
        }
        // right
        if (kea.Key is Keys.D or Keys.Right)
        {
            Position.X += speed * time;
            Direction.X += speed * time;
        }
        // up
        if (kea.Key == Keys.Space)
        {
            Position.Y += speed * time;
            Direction.Y += speed * time;
        }
        // down
        if (kea.Key == Keys.LeftShift)
        {
            Position.Y -= speed * time;
            Direction.Y -= speed * time;
        }

        UpdateVectors();
    }

    /// <summary>
    /// Rotates the camera accordingly based on mouse movement.
    /// </summary>
    public void RotationInput(MouseMoveEventArgs mea, Surface screen)
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
        
        UpdateVectors();
    }

    /// <summary>
    /// Adjusts the camera's FOV to create a "zoom" effect.
    /// </summary>
    public void CameraZoom(MouseWheelEventArgs mea)
    {
        FOV -= mea.OffsetY;

        UpdateVectors();
    }
}
