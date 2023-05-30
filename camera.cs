using OpenTK.Mathematics;

namespace INFOGR2023Template
{
    internal class Camera
    {
        public Vector3 Position { get; set; }
        public Vector3 Direction { get; set; } // look-at direction

        public Vector3 Up { get; set; }
        public Vector3 Right { get; set; }

        public float FOV { get; set; }

        public float ScreenDistance { get; set; }

        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }

        public float AspectRatio;

        public Vector3 ImagePlaneCenter;
        public Vector3 p0;
        public Vector3 p1;
        public Vector3 p2;

        // screen plane orthonormal basis
        public Vector3 u;
        public Vector3 v;

        public Camera(int width, int height, Vector3 pos, Vector3 dir, Vector3 up, Vector3 right, float screenDistance)
        {

            Position = pos;
            Direction = dir;
            Up = up;
            Right = right;
            ScreenDistance = screenDistance;
            ScreenWidth = width;
            ScreenHeight = height;

            AspectRatio = ScreenWidth / ScreenHeight;

            ImagePlaneCenter = Position + ScreenDistance * Direction;
            p0 = ImagePlaneCenter + Up - AspectRatio * Right;
            p1 = ImagePlaneCenter + Up + AspectRatio * Right;
            p2 = ImagePlaneCenter - Up - AspectRatio * Right;
            
            u = p1 - p0;
            v = p2 - p0;

            Console.WriteLine($"Camera: P0: {p0} P1: {p1} P2: {p2} AR: {AspectRatio} dir{Direction}");
        }

        public Matrix4 Projection(float aspectRatio)
        {
            float left = -ScreenWidth / 2;
            float right = ScreenWidth / 2;
            float top = ScreenHeight / 2;
            float bottom = -ScreenHeight / 2;
            return Matrix4.CreateOrthographicOffCenter(left, right, bottom, top, ScreenDistance, 1000f);
        }

        public Ray GetRay(Vector3 point)
        {
            Vector3 rayDirection = Vector3.Normalize(point - Position);
            
            return new Ray(Position, rayDirection);
        }
    }
}
