namespace INFOGR2023Template;

class Application
{
    public Surface Screen;
    public Raytracer Raytracer;
    
    public Application(Surface screen) => Screen = screen;

    /// <summary>
    /// Initializes the window.
    /// </summary>
    public void Init() => Raytracer = new Raytracer(Screen);

    /// <summary>
    /// Renders the scene each frame.
    /// </summary>
    public void Tick() => Raytracer.Render();
}
