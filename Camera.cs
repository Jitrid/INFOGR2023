using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Rasterization;

public class Camera
{
    private float x, y, z; 

    public Vector3 Position;
    public Vector3 Front;
    public Vector3 Up;

    private float pitch, yaw;

    public Camera(float x, float y, float z)
    {
        this.x = x; 
        this.y = y;
        this.z = z;
        Position = new Vector3(x, y, z);

        Up = Vector3.UnitY;

        pitch = 0.0f;
        yaw = -90.0f;
    }

    /// <summary>
    /// Updates the camera front direction based on rotation input.
    /// </summary>
    public Matrix4 Load()
    {
        Front.X = MathF.Cos(MathHelper.DegreesToRadians(yaw)) * MathF.Cos(MathHelper.DegreesToRadians(pitch));
        Front.Y = MathF.Sin(MathHelper.DegreesToRadians(pitch));
        Front.Z = MathF.Sin(MathHelper.DegreesToRadians(yaw)) * MathF.Cos(MathHelper.DegreesToRadians(pitch));
        Front = Vector3.Normalize(Front);

        return Matrix4.LookAt(Position, Position + Front, Up);
    }

    /// <summary>
    /// Moves the camera on one of the axis based on certain key binds.
    /// </summary>
    public void MovementInput(KeyboardKeyEventArgs kea)
    {
        
        // Constant to set the movement speed.
        const float speed = 0.5f;
        Vector3 right = Vector3.Normalize(Vector3.Cross(Front, Up));

        switch (kea.Key)
        {
            // forwards
            case Keys.W or Keys.Up:
                Position += speed * Front;
                break;
            // backwards
            case Keys.S or Keys.Down:
                Position -= speed * Front;
                break;
            // right
            case Keys.D or Keys.Right:
                Position += speed * right;
                break;
            // left
            case Keys.A or Keys.Left:
                Position -= speed * right;
                break;
            // up
            case Keys.Space:
                Position += speed * Up;
                break;
            // down
            case Keys.LeftShift:
                Position -= speed * Up;
                break;
        }

        //Position = new Vector3(x, y, z);
    }

    /// <summary>
    /// Rotates the camera accordingly based on mouse movement.
    /// </summary>
    public void MouseInput(MouseMoveEventArgs mea)
    {
        const float sensitvity = 0.1f;

        yaw += mea.DeltaX * sensitvity;
        pitch -= mea.DeltaY * sensitvity;

        if (pitch > 89.0f)
            pitch = 89.0f;
        if (pitch < -89.0f)
            pitch = -89.0f;
    }
}
