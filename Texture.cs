using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using OpenTK.Mathematics;

namespace INFOGR2023Template;

// Reference for the skybox feature: https://en.wikipedia.org/wiki/Cube_mapping#Memory_addressing
public class Skybox
{
    /// <summary>
    /// Stores bitmaps of the skybox textures.
    /// </summary>
    public Image<Bgra32>[] bmps;

    public Skybox(string[] files)
    {
        bmps = new Image<Bgra32>[files.Length];
        Load(files);
    }

    /// <summary>
    /// Loads the bitmaps into the bmps array.
    /// </summary>
    public void Load(string[] files)
    {
        for (int i = 0; i < files.Length; i++)
            bmps[i] = Image.Load<Bgra32>(files[i]);
    }

    public Vector3 GetColor(float x, float y, float z)
    {
        GetCoords(x, y, z, out int face, out float skyu, out float skyv);

        int width = bmps[face].Width;
        int height = bmps[face].Height;

        // Invalid pixel coordinates, return a default color.
        if (skyu < 0 || skyu >= width || skyv < 0 || skyv >= height)
            return Vector3.Zero;

        Bgra32 bmpcolor = bmps[face][(int)(skyu * (width - 1)), (int)(skyv * (height - 1))];

        Vector3 color = new Vector3(bmpcolor.R / 255f, bmpcolor.G / 255f, bmpcolor.B / 255f);

        return color;
    }

    public void GetCoords(float x, float y, float z, out int face, out float skyu, out float skyv)
    {
        float absX = Math.Abs(x);
        float absY = Math.Abs(y);
        float absZ = Math.Abs(z);

        int isXPositive = x > 0 ? 1 : 0;
        int isYPositive = y > 0 ? 1 : 0;
        int isZPositive = z > 0 ? 1 : 0;

        float maxAxis, uc, vc;

        // Positive X
        if (isXPositive != 0 && absX >= absY && absX >= absZ)
        {
            maxAxis = absX;
            uc = -z;
            vc = y;
            face = 0;
        }
        // Negative X
        else if (isXPositive == 0 && absX >= absY && absX >= absZ)
        {
            maxAxis = absX;
            uc = z;
            vc = y;
            face = 1;
        }
        // Negative Y
        else if (isYPositive == 0 && absY >= absX && absY >= absZ)
        {
            maxAxis = absY;
            uc = x;
            vc = z;
            face = 2;
        }
        // Positive Y
        else if (isYPositive != 0 && absY >= absX && absY >= absZ)
        {
            maxAxis = absY;
            uc = x;
            vc = -z;
            face = 3;
        }
        // Positive Z
        else if (isZPositive != 0 && absZ >= absX && absZ >= absY)
        {
            maxAxis = absZ;
            uc = x;
            vc = y;
            face = 4;
        }
        // Negative Z
        else
        {
            maxAxis = absZ;
            uc = -x;
            vc = y;
            face = 5;
        }

        // Convert range from -1 to 1 to 0 to 1.
        skyu = 0.5f * (uc / maxAxis + 1.0f);
        skyv = 0.5f * (vc / maxAxis + 1.0f);
    }
}
