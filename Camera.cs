using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Rasterization;

public class Camera
{
    private float x, y, z;

    public Camera(float x, float y, float z)
    {
        this.x = x; 
        this.y = y;
        this.z = z;
    }

    public Matrix4 Load()
    {
        Matrix4 translation = Matrix4.CreateTranslation(new Vector3(x, y, z));
        Matrix4 rotation = Matrix4.CreateFromAxisAngle(new Vector3(1, 0, 0), MathF.PI / 2);
        
        return translation * rotation;
    }

    /// <summary>
    /// Moves the camera on one of the axis based on certain key binds.
    /// </summary>
    /// <param name="time">The frame's "delta time" to determine performance and generalize the effect for all systems.</param>
    public void MovementInput(KeyboardKeyEventArgs kea, float time)
    {
        // Constant to set the movement speed.
        const float speed = 1f;
        time = 1f;

        switch (kea.Key)
        {
            // forwards
            case Keys.W or Keys.Up:
                z += speed * time;
                break;
            // backwards
            case Keys.S or Keys.Down:
                z -= speed * time;
                break;
            // left
            case Keys.A or Keys.Left:
                x += speed * time;
                break;
            // right
            case Keys.D or Keys.Right:
                x -= speed * time;
                break;
            // up
            case Keys.Space:
                y -= speed * time;
                break;
            // down
            case Keys.LeftShift:
                y += speed * time;
                break;
        }

        Load();
    }
}
