using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Rasterization;

public class Camera
{
    private float x, y, z; 
    public Vector3 Position;
    private float pitch, yaw;

    public Camera(float x, float y, float z)
    {
        this.x = x; 
        this.y = y;
        this.z = z;
        Position = new Vector3(x, y, z);
        pitch = 0.0f;
        yaw = -90.0f;
    }

    public Matrix4 Load()
    {
        Vector3 front;
        front.X = MathF.Cos(MathHelper.DegreesToRadians(yaw)) * MathF.Cos(MathHelper.DegreesToRadians(pitch));
        front.Y = MathF.Sin(MathHelper.DegreesToRadians(pitch));
        front.Z = MathF.Sin(MathHelper.DegreesToRadians(yaw)) * MathF.Cos(MathHelper.DegreesToRadians(pitch));
        front = Vector3.Normalize(front);

        return Matrix4.LookAt(new Vector3(x, y, z), new Vector3(x, y, z) + front, Vector3.UnitY);
    }

    /// <summary>
    /// Moves the camera on one of the axis based on certain key binds.
    /// </summary>
    public void MovementInput(KeyboardKeyEventArgs kea)
    {
        // Constant to set the movement speed.
        const float speed = 0.5f;

        switch (kea.Key)
        {
            // forwards
            case Keys.W or Keys.Up:
                z -= speed;
                break;
            // backwards
            case Keys.S or Keys.Down:
                z += speed;
                break;
            // right
            case Keys.D or Keys.Right:
                x += speed;
                break;
            // left
            case Keys.A or Keys.Left:
                x -= speed;
                break;
            // up
            case Keys.Space:
                y += speed;
                break;
            // down
            case Keys.LeftShift:
                y -= speed;
                break;
        }

        Position = new Vector3(x, y, z);
        Console.WriteLine(Position);
    }

    /// <summary>
    /// Rotates the camera accordingly based on mouse movement.
    /// </summary>
    public void MouseInput(MouseMoveEventArgs mea)
    {
        const float sensitvity = 0.1f;

        yaw += mea.DeltaX * sensitvity;
        pitch += mea.DeltaY * sensitvity;

        if (pitch > 89.0f)
            pitch = 89.0f;
        if (pitch < -89.0f)
            pitch = -89.0f;
    }
}
