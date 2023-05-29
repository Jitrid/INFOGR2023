using OpenTK.Mathematics;

namespace INFOGR2023Template
{
    internal class Camera
    {
        public Vector3 Position { get; set; }
        public Vector3 Direction { get; set; } // look-at direction

        public Vector3 Up { get; set; }

        public float FOV { get; set; }

        public float ScreenDistance { get; set; }

        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }

        private const int Steps = 8;

        public Matrix4 Projection(float aspectRatio)
        {
            float left = -ScreenWidth / 2;
            float right = ScreenWidth / 2;
            float top = ScreenHeight / 2;
            float bottom = -ScreenHeight / 2;
            return Matrix4.CreateOrthographicOffCenter(left, right, bottom, top, ScreenDistance, 1000f);
        }

        public Ray GetRay(float screenX, float screenY)
        {
            // Convert screen space coordinates to normalized device coordinates (NDC)
            float ndcX = TransformX(screenX);
            float ndcY = TransformY(screenY);

            // Create a ray direction in view space
            Vector3 rayDirection = new(ndcX, ndcY, -1f);

            // Transform the ray direction from view space to world space
            Matrix4 invViewMatrix = Matrix4.LookAt(Position, Position + Direction, Up).Inverted();
            Vector3 worldSpaceDirection = Vector3.TransformNormal(rayDirection, invViewMatrix);

            // Normalize the ray direction vector
            rayDirection = Vector3.Normalize(worldSpaceDirection);

            // Create and return the ray
            return new Ray(Position, rayDirection);
        }

        public float TransformX(float x) => x / (ScreenWidth / 2 / Steps) - Steps / 2;
        public float TransformY(float y) => y / (ScreenHeight / Steps) - Steps / 2;
    }
}
