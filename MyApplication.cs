using INFOGR2023Template;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace Template
{
    class MyApplication
    {
        // member variables
        public Surface screen;
        public Camera camera;
        public Plane plane;
        public Intersection intersection;
        
        // constructor
        public MyApplication(Surface screen)
        {
            this.screen = screen;
            this.camera = new Camera();
        }
        // initialize
        public void Init()
        {
            camera.Position = new Vector3(0f, 1f, 0f);
            camera.Direction = new Vector3(0f, -1f, 1f); // Pointing towards the positive Z-axis
            camera.Up = Vector3.UnitY;
            camera.ScreenDistance = 1000f; // Set a smaller screen distance
            camera.ScreenWidth = screen.width; // Use the screen dimensions
            camera.ScreenHeight = screen.height;

            // plane = Primitive.CreateCube(250f);
            plane = Plane.CreatePlane(150f, 150f, 0, 0, 0);
        } 

        // tick: renders one frame
        public void Tick()
        {
            screen.Clear(0);

            int count = 0;

            for (int n = 0; n < screen.pixels.Length; n++)
            {
                int y = n / screen.width;
                int x = (n - y * screen.width) / 2;

                // Create a ray from the camera position through the current pixel
                Ray ray = camera.GetRay(x, y);

                Vector3 drawRay = camera.Position + 10 * ray.Direction;
                float lineX = drawRay.X;
                float lineY = 4f;

                // Console.WriteLine(ray.Direction);

                // Check if the ray intersects the plane
                if (Plane.IntersectPlane(ray, plane, out float intersectionDistance, out Vector3 intersectionPoint))
                {
                    // Set the pixel color to the desired color
                    int color = (int)intersectionDistance * 10000; // Blue color (adjust as needed)

                    screen.pixels[y * screen.width + x] = color;
                }

                // if (count % (screen.pixels.Length / 100) == 0)
                // {
                    screen.Line((int)TranslateX(0), (int)TranslateY(0), (int)TranslateX(lineX), (int)TranslateY(lineY), 255);
                // }
                count++;
            }

            screen.Line(800, 580 - 75, 1200, 580 - 75, 255 << 16);

            Console.WriteLine($"X: {TranslateX(0)}, Y: {TranslateY(0)}");
        }

        public void AdjustCamera(KeyboardKeyEventArgs ea)
        {
            camera.Position = ea.Key switch
            {
                Keys.Space => (camera.Position.X, camera.Position.Y + 1, camera.Position.Z),
                Keys.LeftShift => (camera.Position.X, camera.Position.Y - 1, camera.Position.Z),
                Keys.W => (camera.Position.X, camera.Position.Y, camera.Position.Z + 1),
                Keys.S => (camera.Position.X, camera.Position.Y, camera.Position.Z - 1),
                Keys.D => (camera.Position.X + 1, camera.Position.Y, camera.Position.Z),
                Keys.A => (camera.Position.X - 1, camera.Position.Y, camera.Position.Z),
                _ => camera.Position
            };
        }

        public int Steps = 8;
        public float TranslateX(float x) => screen.width / 2 + (x + Steps / 2) * (screen.width / (Steps * 2));
        public float TranslateY(float y) => (-y + Steps / 2) * screen.height / Steps;


        private Vector3 CalculateColor(Vector3 intersectionPoint)
        {
            // Determine the color based on the position of the intersection point
            if (intersectionPoint.X < 0 && intersectionPoint.Z < 0)
            {
                return new Vector3(1f, 0f, 0f); // Red
            }
            else if (intersectionPoint.X >= 0 && intersectionPoint.Z >= 0)
            {
                return new Vector3(0f, 1f, 0f); // Green
            }
            else
            {
                return new Vector3(0f, 0f, 1f); // Blue
            }
        }
        private int ColorToInt(Vector3 color)
        {
            int r = (int)(color.X * 255);
            int g = (int)(color.Y * 255);
            int b = (int)(color.Z * 255);

            return (r << 16) | (g << 8) | b;
        }
    }
}