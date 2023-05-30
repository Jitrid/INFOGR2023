using System.Numerics;
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
        public Primitives Primitives;
        public Camera camera;
        public Sphere sphere;
        public Sphere sphere2;
        public Light light;
        public Intersection intersection;
        
        // constructor
        public MyApplication(Surface screen)
        {
            this.screen = screen;
        }
        // initialize
        public void Init()
        {
            // E  V  U  d  (R) | C = E + dV
            camera = new Camera(screen.width, screen.height, new Vector3(0f, 0f, 0f), 
                            new Vector3(0f, 0f, 1f), Vector3.UnitY, Vector3.UnitX, 1f);

            Primitives = new Primitives();
            sphere = new Sphere(new Vector3(0f, 1f, 3f), 5f);

            Primitives.Add(sphere);

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

                Vector3 Punty = camera.p0 + x * camera.u + y * camera.v;

                // Create a ray from the camera position through the current pixel
                Ray viewRay = camera.GetRay(Punty);
                int pixel = Color(viewRay, Primitives._primitives[0]);

                screen.pixels[y * screen.width + x] = pixel;

                // Vector3 drawRay = camera.Position + 10 * viewRay.Direction;
                // float lineX = drawRay.X;
                // float lineY = 4f;
                //
                // if (count % 1000 == 0 & TranslateX(lineX) > screen.width / 2 & TranslateX(lineX) < screen.width)
                // {
                //     screen.Line((int)TranslateX(0), (int)TranslateY(0), (int)TranslateX(lineX), (int)TranslateY(lineY), 255);
                // }
                // count++;
                // screen.Line(640 + (int)sphere.Center.X, (int)sphere.Center.Y, 640 + (int)sphere.Center.X + 100, (int)sphere.Center.Y + 100, 255 << 16);
            }

            screen.Line(800, 580 - 75, 1200, 580 - 75, 255 << 16);
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

            Console.WriteLine(camera.Position);
        }

        public int Color(Ray ray, Intersectable i)
        {
            Intersection intersection;
            if (i.HitOrMiss(ray, 0, float.MaxValue, out intersection))
                return (int)(0.5 * (GetColor(intersection.Normal.X, intersection.Normal.Y, intersection.Normal.Z)));

            return 0;

            // Vector3 Direction = Vector3.Normalize(ray.Direction);
            // int t = (int)(0.5 * (Direction.Y + 1.0));
            // return (int)((1.0 - t) * GetColor(1.0f, 1.0f, 1.0f) + t * GetColor(0.5f, 0.7f, 1.0f));
        }
        public int GetColor(float x, float y, float z) => (int)(255 * x + 255 * y + 255 * z);


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