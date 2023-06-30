using OpenTK.Graphics.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Rasterization;

public class Texture
{
    public int ID;
    private readonly string? filename;        // regular texture
    private readonly string[]? faces;         // cube map textures
    
    public Texture(string filename)
    {
        this.filename = filename;
        Load(false);
    }
    public Texture(string[] faces)
    {
        this.faces = faces;
        Load(true);
    }

    /// <summary>
    /// Loads an image file into an actual texture.
    /// </summary>
    /// <param name="isCubeMap">Indicates whether to generate a cube map with six textures, or one regular texture.</param>
    public void Load(bool isCubeMap)
    {
        TextureTarget target = isCubeMap ? TextureTarget.TextureCubeMap : TextureTarget.Texture2D;

        ID = GL.GenTexture();
        GL.BindTexture(target, ID);

        for (int i = 0; i < (isCubeMap ? faces!.Length : 1); i++)
        {
            Image<Bgra32> bmp = Image.Load<Bgra32>(isCubeMap ? faces![i] : filename);
            int width = bmp.Width;
            int height = bmp.Height;
            int[] pixels = new int[width * height];

            for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                pixels[y * width + x] = (int)bmp[x, y].Bgra;

            if (!isCubeMap) GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                width, height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, pixels);
            else
                GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, PixelInternalFormat.Rgba,
                    width, height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, pixels);
        }

        GL.TexParameter(target, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(target, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        GL.TexParameter(target, TextureParameterName.TextureWrapS, (int)TextureParameterName.ClampToEdge);
        GL.TexParameter(target, TextureParameterName.TextureWrapT, (int)TextureParameterName.ClampToEdge);
        if (isCubeMap) GL.TexParameter(TextureTarget.TextureCubeMap, 
            TextureParameterName.TextureWrapR, (int)TextureParameterName.ClampToEdge);
    }
}
