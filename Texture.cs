using OpenTK.Graphics.ES11;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Drawing.Imaging;
using System.Windows;
using OpenTK.Mathematics;

namespace INFOGR2023Template
{
    public class Skybox
    {
        public Image<Bgra32>[] bmps;

        //Image<Bgra32> bmp = Image.Load<Bgra32>(fileName);

        public Skybox(string[] files)
        {
            load(files);
        }

        public void load(string[] files)
        {
            for (int i = 0; i < files.Length; i++)
            {
                bmps[i] = Image.Load<Bgra32>(files[i]);
            }            
        }

        public Vector3 GetColor(float x, float y)
        {
           
            int face;
            float skyu, skyv;

            getCoords(x,y,out face, out skyu, out skyv);

             //𝐹(𝑢,𝑣)=lookup texel(int)(𝑢×𝑤𝑖𝑑𝑡ℎ),(int)(𝑣×ℎ𝑒𝑖𝑔ℎ𝑡))

             Bgra32 bmpcolor = bmps[face][(int)(skyu * bmps[face].Width), (int)(skyv * bmps[face].Height)];

             Vector3 color = new Vector3(bmpcolor.R / 255f, bmpcolor.G / 255f, bmpcolor.B / 255f);

             return color;
        }

        public void getCoords(float u, float v, out int face, out float skyu, out float skyv)
        {
            float absU = Math.Abs(u);
            float absV = Math.Abs(v);

            // Determine face index based on maximum component
            if (absU >= absV)
            {
                if (u >= 0)
                    face = 0; // +X
                else
                    face = 1; // -X

                skyu = v / absU;
                skyv = u / absU;
            }
            else
            {
                if (v >= 0)
                    face = 2; // +Y
                else
                    face = 3; // -Y

                skyu = u / absV;
                skyv = v / absV;
            }

            //CHANGE THIS
            // Map skyu and skyv to range [0, 1]
            skyu = (skyu + 1) * 0.5f;
            skyv = (skyv + 1) * 0.5f;
        }
    }

    
}
