using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace INFOGR2023Template;

/// <summary>
/// Represents the camera from which the user looks at the scene.
/// </summary>
public class Camera
{
    private readonly Surface _screen;

    /// <summary>
    /// The current position of the camera.
    /// </summary>
    public Vector3 Position;
    /// <summary>
    /// The direction in which the camera is pointed.
    /// </summary>
    public Vector3 Direction;
    /// <summary>
    /// The "up" direction of the camera.
    /// </summary>
    public Vector3 Up;
    /// <summary>
    /// The "right" direction of the camera.
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

    public Camera(Surface screen, Vector3 pos)
    {
        _screen = screen;
        Position = pos;

        FOV = 70;
        Pitch = 0f;
        Yaw = 0f;

        // Initializes the first values of the vectors.
        UpdateCamera();
    }

    /// <summary>
    /// Updates the camera specifications whenever necessary.
    /// </summary>
    public void UpdateCamera()
    {
        float aspectRatio = _screen.height / (_screen.width / 2);
        Matrix3 rotation = Matrix3.CreateRotationX(MathHelper.DegreesToRadians(Pitch)) * Matrix3.CreateRotationY(MathHelper.DegreesToRadians(Yaw));

        Direction = Vector3.Normalize(Vector3.TransformRow(Vector3.UnitZ, rotation));
        Up = Vector3.Normalize(Vector3.TransformRow(Vector3.UnitY, rotation));
        Right = Vector3.Cross(-Direction, Up);

        ScreenDistance = Vector3.Distance(Vector3.Zero, Right) /
                         (float)MathHelper.Tan(MathHelper.DegreesToRadians(0.5 * FOV));

        ImagePlaneCenter = Position + ScreenDistance * Direction;

        P0 = ImagePlaneCenter + Up - aspectRatio * Right;
        P1 = ImagePlaneCenter + Up + aspectRatio * Right;
        P2 = ImagePlaneCenter - Up - aspectRatio * Right;

        U = P1 - P0;
        V = P2 - P0;
    }

    /// <summary>
    /// Moves the camera on one of the axis based on certain key binds.
    /// </summary>
    /// <param name="time">The frame's "delta time" to determine performance and generalize the effect for all systems.</param>
    public void MovementInput(KeyboardKeyEventArgs kea, float time)
    {
        // Constant to set the movement speed.
        const float speed = 0.5f;

        switch (kea.Key)
        {
            // forwards
            case Keys.W or Keys.Up:
                Position.Z += speed * time;
                break;
            // backwards
            case Keys.S or Keys.Down:
                Position.Z -= speed * time;
                break;
            // left
            case Keys.A or Keys.Left:
                Position.X -= speed * time;
                break;
            // right
            case Keys.D or Keys.Right:
                Position.X += speed * time;
                break;
            // up
            case Keys.Space:
                Position.Y += speed * time;
                break;
            // down
            case Keys.LeftShift:
                Position.Y -= speed * time;
                break;
        }

        UpdateCamera();
    }

    /// <summary>
    /// Rotates the camera accordingly based on mouse movement.
    /// </summary>
    public void RotationInput(MouseMoveEventArgs mea)
    {
        // The sensitivity of camera rotation.
        const float sensitivity = 0.1f;

        Yaw += mea.DeltaX * sensitivity;
        Pitch += mea.DeltaY * sensitivity;

        if (Pitch > 89.0f)
            Pitch = 89.0f;
        if (Pitch < -89.0f) 
            Pitch = -89.0f;

        UpdateCamera();
    }

    /// <summary>
    /// Adjusts the Field of View (FOV) to create a zoom effect.
    /// </summary>
    public void ZoomInput(MouseWheelEventArgs mea)
    {
        if (FOV - mea.OffsetY > 100 || FOV - mea.OffsetY < 0) return;
        FOV -= mea.OffsetY;

        UpdateCamera();
    }
}
