using OpenTK.Graphics.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Rasterization;

public class Texture
{
    public int ID;

    public Texture(string filename, bool isCubeMap = false)
    {
        if (string.IsNullOrEmpty(filename)) throw new ArgumentException(filename);
        TextureTarget target = isCubeMap ? TextureTarget.TextureCubeMap : TextureTarget.Texture2D;
        ID = GL.GenTexture();
        GL.BindTexture(target, ID);

        // We will not upload mipmaps, so disable mipmapping (otherwise the texture will not appear).
        // We can use GL.GenerateMipmaps() or GL.Ext.GenerateMipmaps() to create
        // mipmaps automatically. In that case, use TextureMinFilter.LinearMipmapLinear to enable them.
        GL.TexParameter(target, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(target, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        // remove maybe?
        GL.TexParameter(target, TextureParameterName.TextureWrapS, (int)TextureParameterName.ClampToEdge);
        GL.TexParameter(target, TextureParameterName.TextureWrapT, (int)TextureParameterName.ClampToEdge);
        GL.TexParameter(target, TextureParameterName.TextureWrapR, (int)TextureParameterName.ClampToEdge);

        Image<Bgra32> bmp = Image.Load<Bgra32>(filename);
        int width = bmp.Width;
        int height = bmp.Height;
        int[] pixels = new int[width * height];

        for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
            pixels[y * width + x] = (int)bmp[x, y].Bgra;

        GL.TexImage2D(target, 0, PixelInternalFormat.Rgba, 
            width, height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, pixels);
    }
}
