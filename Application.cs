using OpenTK.Windowing.Common;

namespace INFOGR2023Template;

class Application
{
    public Surface Screen;
    public Raytracer Raytracer;
    
#pragma warning disable CS8618
    public Application(Surface screen) => Screen = screen;
#pragma warning restore CS8618

    /// <summary>
    /// Initializes the window.
    /// </summary>
    public void Init() => Raytracer = new Raytracer(Screen);

    /// <summary>
    /// Renders the scene each frame.
    /// </summary>
    public void Tick() => Raytracer.Render();

    // The following three methods call their respective methods in regards to camera movement.
    public void CallMovement(KeyboardKeyEventArgs kea, float t) => Raytracer.Camera.CameraKeyboardInput(kea, t);
    public void CallRotation(MouseMoveEventArgs mea) => Raytracer.Camera.RotationInput(mea);
    public void CallZoom(MouseWheelEventArgs mea) => Raytracer.Camera.CameraZoom(mea);
}
