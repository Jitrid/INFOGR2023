using OpenTK.Graphics.OpenGL;

// based on http://www.opentk.com/doc/graphics/frame-buffer-objects

namespace Rasterization;

class RenderTarget
{
    private uint FBO;
    private int colorTexture;
    private uint depthBuffer;
    private int width, height;

    public RenderTarget(int screenWidth, int screenHeight)
    {
        width = screenWidth;
        height = screenHeight;

        // create color texture
        GL.GenTextures(1, out colorTexture);
        GL.BindTexture(TextureTarget.Texture2D, colorTexture);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, width, height,
            0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
        GL.BindTexture(TextureTarget.Texture2D, 0);

        // bind color and depth textures to FBO
        GL.GenFramebuffers(1, out FBO);
        GL.GenRenderbuffers(1, out depthBuffer);
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, FBO);
        GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, colorTexture, 0);
        GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthBuffer);
        GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, (RenderbufferStorage)All.DepthComponent24, width, height);
        GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, depthBuffer);
        
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0); // return to regular framebuffer
    }

    public int GetTextureID() => colorTexture;
    public void Bind()
    {
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, FBO);
        GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
    }
    public void Unbind()
    {
        // return to regular framebuffer
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }
}
