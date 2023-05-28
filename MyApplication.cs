using System.Numerics;
using System.Runtime.CompilerServices;
using INFOGR2023Template;
//using OpenTK.Graphics.ES11;
using OpenTK.Mathematics;
//using Vector3 = System.Numerics.Vector3;
using OpenTK.Graphics.ES30;
using OpenTK.Windowing.Desktop;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace Template
{
    class MyApplication
    {
        // member variables
        public Surface screen;
        public Surface debug;
        public Camera camera;
        public Primitive plane;
        public Intersection intersection;
        
        // constructor
        public MyApplication(Surface screen)
        {
            this.screen = screen;
            this.debug = debug;
            this.camera = new Camera();
        }
        // initialize
        public void Init()
        {
            camera.Position = new Vector3(1f, 0f, 1f);
            camera.Direction = new Vector3(0f, 0f, 1f); // Pointing towards the positive Z-axis
            camera.Up = Vector3.UnitY;
            camera.ScreenDistance = 100f; // Set a smaller screen distance
            camera.ScreenWidth = screen.width; // Use the screen dimensions
            camera.ScreenHeight = screen.height;


            plane = Primitive.CreatePlane(28.25f, 27f);
            

        }
        // tick: renders one frame
        public void Tick()
        {
            screen.Clear(0);

            for (int y = 0; y < screen.height; y++)
            {
                for (int x = 0; x < screen.width; x++)
                {
                    // Calculate the normalized device coordinates of the current pixel
                    float screenX = (float)(2 * x + 1) / screen.width - 1;
                    float screenY = 1 - (float)(2 * y) / screen.height;

                    // Create a ray from the camera position through the current pixel
                    Ray ray = camera.GetRay(screenX, screenY);

                    // Check if the ray intersects the plane
                    if (plane.IntersectPlane(ray, plane, out float intersectionDistance, out Vector3 intersectionPoint))
                    {
                      // Console.Write(intersectionDistance.ToString());
                        // Set the pixel color to the desired color
                        int color = (int)intersectionDistance*10; // Blue color (adjust as needed)
                        
                        screen.pixels[y * screen.width + x] = color;
                    }
                }
            }

            screen.Print("hello world", 2, 2, 0xffffff);
            screen.Line(2, 20, 160, 20, 0xff0000);

            


            // Debug output
            //Console.WriteLine("Ray direction: " + ray.Direction);
            //    Console.WriteLine("Intersection distance: " + intersectionDistance);
            //    Console.WriteLine("Intersection point: " + intersectionPoint);
        }



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