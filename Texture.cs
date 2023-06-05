using OpenTK.Graphics.ES11;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Drawing.Imaging;
using System.Windows;
using OpenTK.Mathematics;
using System;

namespace INFOGR2023Template
{
    public class Skybox
    {
        public Image<Bgra32>[] bmps;

        //Image<Bgra32> bmp = Image.Load<Bgra32>(fileName);

        public Skybox(string[] files)
        {
            bmps = new Image<Bgra32>[files.Length];
            load(files);
        }

        public void load(string[] files)
        {
            for (int i = 0; i < files.Length; i++)
            {
                //Console.WriteLine(files[i]);
                bmps[i] = Image.Load<Bgra32>(files[i]);
            }
        }

        public Vector3 GetColor(float x, float y, float z)
        {
            //Console.WriteLine($"x: {x} y:{y}");
            int face;
            float skyu, skyv;

            getCoords(x, y, z, out face, out skyu, out skyv);

            int width = bmps[face].Width;
            int height = bmps[face].Height;

            if (skyu < 0 || skyu >= width || skyv < 0 || skyv >= height)
            {
                // Invalid pixel coordinates, return a default color
                return Vector3.Zero;
            }

            //𝐹(𝑢,𝑣)=lookup texel(int)(𝑢×𝑤𝑖𝑑𝑡ℎ),(int)(𝑣×ℎ𝑒𝑖𝑔ℎ𝑡))
            //Console.WriteLine($"u: {skyu} v:{skyv}");
            int pixX = (int)(skyu * (float)(width - 1));
            int pixY = (int)(skyv * (float)(height - 1));

            //if (pixX <= 0 || pixY <= 0) { return Vector3.Zero; }
            //if (pixY <= 0 || pixX >= height) {  return Vector3.Zero; }

            //  Console.WriteLine($"{pixX} {pixY}");

            Bgra32 bmpcolor = bmps[face][pixX, pixY];

            Vector3 color = new Vector3(bmpcolor.R / 255f, bmpcolor.G / 255f, bmpcolor.B / 255f);

            return color;
        }

        public void getCoords(float x, float y, float z, out int face, out float skyu, out float skyv)
        {
            float absX = Math.Abs(x);
            float absY = Math.Abs(y);
            float absZ = Math.Abs(z);

            int isXPositive = x > 0 ? 1 : 0;
            int isYPositive = y > 0 ? 1 : 0;
            int isZPositive = z > 0 ? 1 : 0;

            float maxAxis, uc, vc;

            // POSITIVE X
            if (isXPositive != 0 && absX >= absY && absX >= absZ)
            {
                // u (0 to 1) goes from +z to -z
                // v (0 to 1) goes from -y to +y
                maxAxis = absX;
                uc = -z;
                vc = y;
                face = 0;
            }
            // NEGATIVE X
            else if (isXPositive == 0 && absX >= absY && absX >= absZ)
            {
                // u (0 to 1) goes from -z to +z
                // v (0 to 1) goes from -y to +y
                maxAxis = absX;
                uc = z;
                vc = y;
                face = 1;
            }
            // POSITIVE Y
            else if (isYPositive != 0 && absY >= absX && absY >= absZ)
            {
                // u (0 to 1) goes from -x to +x
                // v (0 to 1) goes from +z to -z
                maxAxis = absY;
                uc = x;
                vc = -z;
                face = 3;
            }
            // NEGATIVE Y
            else if (isYPositive == 0 && absY >= absX && absY >= absZ)
            {
                // u (0 to 1) goes from -x to +x
                // v (0 to 1) goes from -z to +z
                maxAxis = absY;
                uc = x;
                vc = z;
                face = 2;
            }
            // POSITIVE Z
            else if (isZPositive != 0 && absZ >= absX && absZ >= absY)
            {
                // u (0 to 1) goes from -x to +x
                // v (0 to 1) goes from -y to +y
                maxAxis = absZ;
                uc = x;
                vc = y;
                face = 4;
            }
            // NEGATIVE Z
            else
            {
                // u (0 to 1) goes from +x to -x
                // v (0 to 1) goes from -y to +y
                maxAxis = absZ;
                uc = -x;
                vc = y;
                face = 5;
            }

            // Convert range from -1 to 1 to 0 to 1
            skyu = 0.5f * (uc / maxAxis + 1.0f);
            skyv = 0.5f * (vc / maxAxis + 1.0f);












            //    float absU = Math.Abs(u);
            //    float absV = Math.Abs(v);

            //    // Determine face index based on maximum component
            //    if (absU >= absV)
            //    {
            //        if (u >= 0)
            //            face = 0; // +X
            //        else
            //            face = 1; // -X

            //        skyu = u / absU;
            //        skyv = v / absU;
            //    }
            //    else
            //    {
            //        if (v >= 0)
            //            face = 2; // +Y
            //        else
            //            face = 3; // -Y

            //        skyu = u / absV;
            //        skyv = v / absV;
            //    }

            //    //CHANGE THIS
            //    // Map skyu and skyv to range [0, 1]
            //    //skyu = (skyu + 1) * 0.5f;
            //    //skyv = (skyv + 1) * 0.5f;
            //    skyu = (float)(u - Math.Floor(u));
            //    skyv = (float)(v - Math.Floor(v));
            //}
        }


    }
}
