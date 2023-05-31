using INFOGR2023Template;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Plane = INFOGR2023Template.Plane;
using Vector2 = System.Numerics.Vector2;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace Template
{
    class MyApplication
    {
        // member variables
        public Surface screen;
        public Camera Camera;
        public Plane plane;
        public Sphere sphere;
        public Light light;

        public bool FirstMove = true;
        public Vector2 LastMousePosition;

        private float _aspectRatio;
        private int count = 0;

        // constructor
        public MyApplication(Surface screen)
        {
            this.screen = screen;
        }

        // initialize
        public void Init()
        {
            _aspectRatio = screen.height / (screen.width / 2);

            Vector3 cameraPosition = new(0f, 0f, 0f);
            Camera = new Camera(_aspectRatio, cameraPosition);

            light = new Light(new Vector3(-5f, 7f, -.5f), 255, 255, 255);
            plane = new Plane(new Vector3(0f, 1f, 0f), new Vector3(1,0,2));
            
            sphere = new Sphere(new Vector3(0f, 0f, 6f), 3f);
        }

        // tick: renders one frame
        public void Tick()
        {
            screen.Clear(0);
            
            for (int i = 0; i < screen.height; i++) // y
            {
                for (int j = 0; j < screen.width / 2; j++) // x
                {
                    screen.pixels[i * screen.width + j] = 0x304080;
                    // double y = -1.0 + i / 299.0;
                    // double x = -1.0 + j / 299.0;
                    double y = -1.0 + i / ((double)(screen.height / 2) - 1);
                    double x = -1.0 + j / ((double)(screen.width / 4) - 1);

                    Vector3 Punty = Camera.p0 + (float)x * Camera.u + (float)y * Camera.v;

                    // Create a ray from the camera position through the current pixel
                    Ray viewRay = Camera.GetRay(Punty);
                    //plane
                    if (plane.HitRay(viewRay, out Vector3 intersectP))
                    {
                        Vector3 lightDirection = Vector3.Normalize(light.Location - intersectP);
                        Ray lightRay = new(intersectP, lightDirection);
                        if (!sphere.HitRay(lightRay, out Vector3 lightIntersect))
                        {
                            int color = ItTakesAllColoursToMakeARainbow(0xFFFFFF, Vector3.Normalize(lightRay.Direction), viewRay.Direction,
                                plane.GetNormal(intersectP), 0x00FF00, .001f, 10, 0.051f);
                            screen.pixels[i * screen.width + j] = color;
                        }
                        else
                        {
                            screen.pixels[i * screen.width + j] = 0;
                        }
                    }

                    Vector3 intersectionPoint;
                    if (sphere.HitRay(viewRay, out intersectionPoint))
                    {
                        // if (count % 10 == 0)
                        // {
                        //     screen.Line(TranslateX(Camera.Position.X), TranslateZ(Camera.Position.Z), TranslateX(intersectionPoint.X), TranslateZ(intersectionPoint.Z), 255);
                        // }

                        // hap slik weg ofzo idk anymore
                        Vector3 normal = sphere.GetNormal(intersectionPoint);
                        if (sphere.IntersectsWithLight(intersectionPoint, light.Location, out Vector3 lightDir))
                        {
                            // 1 roze teelbal, you are welcome
                            int color = ItTakesAllColoursToMakeARainbow(0xFF00FF, Vector3.Normalize(lightDir), viewRay.Direction,
                                normal, 0xFFFFFF, .001f, 10, 0.051f);
                            screen.pixels[i * screen.width + j] = color;
                        }
                    }
                    // else
                    // {
                    //     if (count % 1000 == 0)
                    //     {
                    //         screen.Line(TranslateX(Camera.Position.X), TranslateZ(Camera.Position.Z), TranslateX(viewRay.Direction.X), 0, 255 << 16);
                    //     }
                    // }
                    count++;

                    screen.pixels[TranslateZ(Camera.Position.Z) * screen.width + TranslateX(Camera.Position.X)] = 255;
                }
            }

            screen.Line(TranslateX(sphere.Center.X) - 5, TranslateZ(sphere.Center.Z) - 5, TranslateX(sphere.Center.X) + 5, TranslateZ(sphere.Center.Z) + 5, 255 << 16);
        }

        public void CameraKeyboardInput(KeyboardKeyEventArgs kea, float time)
        {
            const float speed = 1.5f;
            float ratio = screen.height / (screen.width / 2);

            switch (kea.Key)
            {
                case Keys.W: // forwards
                    Camera.Position += Camera.Direction * speed * time;
                    Camera.UpdateVectors(ratio);
                    break;
                case Keys.S: // backwards
                    Camera.Position -= Camera.Direction * speed * time;
                    Camera.UpdateVectors(ratio);
                    break;
                case Keys.A: // left
                    Camera.Position -= Camera.Right * speed * time;
                    Camera.UpdateVectors(ratio);
                    break;
                case Keys.D: // right
                    Camera.Position += Camera.Right * speed * time;
                    Camera.UpdateVectors(ratio);
                    break;
                case Keys.Space: // up
                    Camera.Position += Camera.Up * speed * time;
                    Camera.UpdateVectors(ratio);
                    break;
                case Keys.LeftShift: // down
                    Camera.Position -= Camera.Up * speed * time;
                    Camera.UpdateVectors(ratio);
                    break;
            }
        }

        public void MouseDownKeyboardInput(MouseMoveEventArgs mea)
        {
            Camera.Yaw += mea.DeltaX * Camera.Sensitivity;
            Camera.Pitch -= mea.DeltaY * Camera.Sensitivity;
        
            switch (Camera.Pitch)
            {
                case > 89.0f:
                    Camera.Pitch = 89.0f;
                    break;
                case < -89.0f:
                    Camera.Pitch = -89.0f;
                    break;
                default:
                    Camera.Pitch -= mea.DeltaX * Camera.Sensitivity;
                    break;
            }
            Camera.Direction = Vector3.Normalize(new Vector3(
                (float)Math.Cos(MathHelper.DegreesToRadians(Camera.Pitch)) * (float)Math.Cos(MathHelper.DegreesToRadians(Camera.Yaw)),
                (float)Math.Sin(MathHelper.DegreesToRadians(Camera.Pitch)),
                (float)Math.Cos(MathHelper.DegreesToRadians(Camera.Pitch)) * (float)Math.Sin(MathHelper.DegreesToRadians(Camera.Yaw)))
            );

            Camera.UpdateVectors(_aspectRatio);
        }

        public void CameraZoom(MouseWheelEventArgs mea)
        {
            Camera.fov -= mea.OffsetY;

            Camera.UpdateVectors(_aspectRatio);
        }

        public void AdjustAspectRatio()
        {
            Camera.AspectRatio = _aspectRatio;
            Camera.UpdateVectors(_aspectRatio);
        }

        public int ItTakesAllColoursToMakeARainbow(int lightColor, Vector3 lightDirection, Vector3 viewDirection,
            Vector3 normal, int objectColor, float specularIntensity, float specularPower,
            float distancelichtsterkteAfname)
        {
            // Genakt van een andere plek, ik ben echt klaar met RGB
            int lightRed = (lightColor >> 16) & 255;
            int lightGreen = (lightColor >> 8) & 255;
            int lightBlue = lightColor & 255;

            int objectRed = (objectColor >> 16) & 255;
            int objectGreen = (objectColor >> 8) & 255;
            int objectBlue = objectColor & 255;

            //die clamp Slide set 5 slide 19
            if (Vector3.Dot(normal, lightDirection) < 0)
            {
                // En daar gaat je licht
                return 0x000000;
            }

            // Diffuse material Slideset 5 slide 19
            float diffuseCoefficient = Math.Max(0, Vector3.Dot(normal, lightDirection));



            //reflection direction kut vectoren uit GL hebben geen .Reflect dus dan maar ombouwen... Kms
            System.Numerics.Vector3 systemReflectionDirection =
                System.Numerics.Vector3.Reflect(VectorToSystem(-lightDirection), VectorToSystem(normal));
            Vector3 reflectionDirection = VectorToGL(systemReflectionDirection);


            //specular reflection Slide 5 26 formule
            float specularCoefficient =
                (float)Math.Pow(Math.Max(0, Vector3.Dot(viewDirection, reflectionDirection)), specularPower) *
                specularIntensity;

            // Sterkte van licht neemt af over afstand enzo  ( 1/ r*r)
            float lichtsterkteAfname = 1 / (distancelichtsterkteAfname * distancelichtsterkteAfname);

            // losse kleuren berekenen lightRed = van het licht niet dat het licht is. Beetje erbij dat je geen harde schaduw rand krijgt, als je dat wilt moet je een batman film gaan kijken of startrek uit 1979
            int shadedRed = objectRed/50 + (int)(lightRed * objectRed * (diffuseCoefficient + specularCoefficient) *
                lichtsterkteAfname / (255 * 255));
            int shadedGreen = objectGreen/50 +(int)(lightGreen * objectGreen * (diffuseCoefficient + specularCoefficient) *
                lichtsterkteAfname / (255 * 255));
            int shadedBlue =  objectBlue/50 + (int)(lightBlue * objectBlue * (diffuseCoefficient + specularCoefficient) *
                lichtsterkteAfname / (255 * 255));

            // Blijkbaar kan dit ook out of bounds
            int fixRed= Math.Max(0, Math.Min(shadedRed, 255));
            int fixGreen = Math.Max(0, Math.Min(shadedGreen, 255));
            int fixBlue = Math.Max(0, Math.Min(shadedBlue, 255));

            // En weer terug naar 0xRRGGBB
            int kleurtje = (fixRed << 16) | (fixGreen << 8) | fixBlue;

            return kleurtje;
        }
    



    //Kutzooi
        public System.Numerics.Vector3 VectorToSystem(Vector3 input)
    {
        System.Numerics.Vector3 output = new System.Numerics.Vector3();
        output.X = input.X;
        output.Y = input.Y;
        output.Z = input.Z;
        return output;
    }

        //Nog meer onzin
    public Vector3 VectorToGL(System.Numerics.Vector3 input)
    {
        Vector3 output = new Vector3();
        output.X = input.X;
        output.Y = input.Y;
        output.Z =input.Z;
        return output;
    }


        public int GetColor(float x, float y, float z) => (int)(255 * x + 255 * y + 255 * z);


        public int Steps = 8; // -4 = 600, 0 = 900, 4 = 1200

        public int TranslateX(float x)
        {
            if (x < -4) return screen.width / 2;
            if (x > 4) return screen.width;

            return (int)(screen.width / 2 + (x + Steps / 2) * (screen.width / (Steps * 2)));
        }

        public int TranslateZ(float z)
        {
            if (z < -4) return screen.height;
            if (z > 4) return 0;

            return (int)((-z + Steps / 2) * screen.height / Steps);
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