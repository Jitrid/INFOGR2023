using System.Globalization;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

// The template provides you with a window which displays a 'linear frame buffer', i.e.
// a 1D array of pixels that represents the graphical contents of the window.

// Under the hood, this array is encapsulated in a 'Surface' object, and copied once per
// frame to an OpenGL texture, which is then used to texture 2 triangles that exactly
// cover the window. This is all handled automatically by the template code.

// Before drawing the two triangles, the template calls the Tick method in MyApplication,
// in which you are expected to modify the contents of the linear frame buffer.

// After (or instead of) rendering the triangles you can add your own OpenGL code.

// We will use both the pure pixel rendering as well as straight OpenGL code in the
// tutorial. After the tutorial you can throw away this template code, or modify it at
// will, or maybe it simply suits your needs.

namespace Rasterization.Template;

public class OpenTKApp : GameWindow
{
    private static int screenID;       // unique integer identifier of the OpenGL texture
    private static Program? app;       // instance of the application
    private static bool terminated;    // application terminates gracefully when this is true

    private ScreenQuad quad = null!;
    private Shader screenShader = null!;

    /// <summary>
    /// Indicates the current state of the cursor.
    /// Has to be custom written because our GameWindow for some reason doesn't have it.
    /// </summary>
    public enum CursorState
    {
        Grabbed,
        Normal
    }
    public CursorState State = CursorState.Normal;

    public OpenTKApp()
        : base(GameWindowSettings.Default, new NativeWindowSettings()
        {
            Title = "The ultimate rasterizer",
            Size = new Vector2i(1280, 720),
            Profile = ContextProfile.Core,  // required for fixed-function, which is probably not supported on MacOS
            Flags = (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? ContextFlags.Default : ContextFlags.Debug) // enable error reporting (not supported on MacOS)
                    | ContextFlags.ForwardCompatible, // required for MacOS
        })
    {
    }

    public void DebugCallback(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
    {
        Dictionary<DebugSource, string> sourceStrings = new Dictionary<DebugSource, string>
        {
            { DebugSource.DebugSourceApi, "API - A call to the OpenGL API" },
            { DebugSource.DebugSourceWindowSystem, "Window System - A call to a window system API" },
            { DebugSource.DebugSourceShaderCompiler, "Shader Compiler" },
            { DebugSource.DebugSourceThirdParty, "Third Party - A third party application associated with OpenGL" },
            { DebugSource.DebugSourceApplication, "Application - A call to GL.DebugMessageInsert() in this application" },
            { DebugSource.DebugSourceOther, "Other" },
            { DebugSource.DontCare, "Ignored" },
        };
        string? sourceString;
        if (!sourceStrings.TryGetValue(source, out sourceString)) sourceString = "Unknown";
        string? typeString = Enum.GetName(type);
        if (typeString != null) typeString = typeString.Substring(9);
        string? severityString = Enum.GetName(severity);
        if (severityString != null) severityString = severityString.Substring(13);
        Console.Error.WriteLine("OpenGL Error:\n  Source: " + sourceString + "\n  Type: " + typeString + "\n  Severity: " + severityString
                                + "\n  Message ID: " + id + "\n  Message: " + Marshal.PtrToStringAnsi(message, length) + "\n");
    } // put a breakpoint here and inspect the stack to pinpoint where the error came from

    protected override void OnLoad()
    {
        base.OnLoad();

        // called during application initialization
        Console.WriteLine("OpenGL Version: " + GL.GetString(StringName.Version) + " (" + (Profile == ContextProfile.Compatability ? "Compatibility" : Profile) + " profile)");
        Console.WriteLine("OpenGL Renderer: " + GL.GetString(StringName.Renderer) + (GL.GetString(StringName.Vendor) == "Intel" ? " (read DiscreteGPU.txt if you have another GPU that you would like to use)" : ""));
        // configure debug output (not supported on MacOS)
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            GL.Enable(EnableCap.DebugOutput);
            // disable all debug messages
            GL.DebugMessageControl(DebugSourceControl.DontCare, DebugTypeControl.DontCare, DebugSeverityControl.DontCare, 0, Array.Empty<int>(), false);
            // enable selected debug messages based on source, type, and severity
            foreach (DebugSourceControl source in new DebugSourceControl[] { DebugSourceControl.DebugSourceApi, DebugSourceControl.DebugSourceShaderCompiler })
            {
                foreach (DebugTypeControl type in new DebugTypeControl[] { DebugTypeControl.DebugTypeError, DebugTypeControl.DebugTypeDeprecatedBehavior, DebugTypeControl.DebugTypeUndefinedBehavior, DebugTypeControl.DebugTypePortability })
                {
                    foreach (DebugSeverityControl severity in new DebugSeverityControl[] { DebugSeverityControl.DebugSeverityHigh })
                    {
                        GL.DebugMessageControl(source, type, severity, 0, Array.Empty<int>(), true);
                    }
                }
            }
            // GL.DebugMessageCallback(DebugCallback, (IntPtr)0);
        }
        // prepare for rendering
        GL.ClearColor(0, 0, 0, 0);
        GL.Disable(EnableCap.DepthTest);
        Surface screen = new(ClientSize.X, ClientSize.Y);
        app = new Program(screen);
        screenID = app.Screen.GenTexture();

        quad = new ScreenQuad();
        screenShader = new Shader("../../../shaders/other/screen_vs.glsl", "../../../shaders/other/screen_fs.glsl");
        
        // Register events to adjust the camera based on mouse and keyboard input.
        KeyDown += kea => app.KeyboardInput(kea);

        app.Init();
    }
    protected override void OnUnload()
    {
        base.OnUnload();
        // called upon app close
        GL.DeleteTextures(1, ref screenID);
    }
    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        // called upon window resize. Note: does not change the size of the pixel buffer.
        GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
    }
    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);

        KeyboardState? keyboard = KeyboardState;
        if (keyboard[Keys.Escape]) terminated = true;
    }

    protected override void OnMouseDown(MouseButtonEventArgs mea)
    {
        State = mea.Button switch
        {
            MouseButton.Left => CursorState.Grabbed,
            MouseButton.Right => CursorState.Normal,
            _ => State
        };
    }

    protected override void OnMouseMove(MouseMoveEventArgs mea)
    {
        if (app == null) return;
        if (State == CursorState.Grabbed)
            app.Camera.MouseInput(mea);
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        if (terminated)
        {
            Close();
            return;
        }

        // convert MyApplication.screen to OpenGL texture
        if (app != null)
        {
            // prepare for generic OpenGL rendering
            GL.Enable(EnableCap.DepthTest);
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
            // do OpenGL rendering
            app.RenderGL();
        }

        // tell OpenTK we're done rendering
        SwapBuffers();
    }
    public static void Main()
    {
        // entry point
        Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
        using OpenTKApp app = new();
        app.RenderFrequency = 30.0;
        app.Run();
    }
}