using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace BlocksWorld
{
    internal class UIRenderer : IRenderer, IDisposable
    {
        Shader shader;
        int vao;
        int vertexBuffer;
        int texture;

        public event EventHandler<PaintEventArgs> Paint;

        Vector2[] vertices = new[]
        {
            new Vector2(0.0f, 0.0f),
            new Vector2(1.0f, 0.0f),
            new Vector2(1.0f, 1.0f),

            new Vector2(0.0f, 0.0f),
            new Vector2(0.0f, 1.0f),
            new Vector2(1.0f, 1.0f),
        };


        public UIRenderer(int virtualWidth, int virtualHeight)
        {
            this.backbuffer = new Bitmap(virtualWidth, virtualHeight);
        }

        ~UIRenderer()
        {
            this.Dispose();
        }

        public void Load()
        {
            this.shader = Shader.CompileFromResource(
                "BlocksWorld.Shaders.ScreenSpace.vs",
                "BlocksWorld.Shaders.Texture.fs");

            this.vao = GL.GenVertexArray();
            this.vertexBuffer = GL.GenBuffer();
            this.texture = GL.GenTexture();

            GL.BindVertexArray(this.vao);
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, this.vertexBuffer);

                GL.EnableVertexAttribArray(0);
                GL.VertexAttribPointer(
                    0,
                    2,
                    VertexAttribPointerType.Float,
                    false,
                    0,
                    Vector2.SizeInBytes);
            }
            GL.BindVertexArray(0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, this.vertexBuffer);
            {
                GL.BufferData(
                    BufferTarget.ArrayBuffer,
                    new IntPtr(Vector2.SizeInBytes * vertices.Length),
                    vertices,
                    BufferUsageHint.StaticDraw);
            }
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.BindTexture(TextureTarget.Texture2D, this.texture);
            {
                GL.TexImage2D(
                    TextureTarget.Texture2D,
                    0,
                    PixelInternalFormat.Rgba,
                    this.backbuffer.Width, this.backbuffer.Height,
                    0,
                    PixelFormat.Bgra,
                    PixelType.UnsignedByte,
                    IntPtr.Zero);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            }
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        private Bitmap backbuffer;

        public void Render(Camera camera, double time)
        {
            if (this.vao == 0)
                throw new ObjectDisposedException("UIRenderer");

            // Upload texture image
            {
                this.OnPaint();
                
                GL.BindTexture(TextureTarget.Texture2D, this.texture);
                var data = this.backbuffer.LockBits(
                    new Rectangle(0, 0, this.backbuffer.Width, this.backbuffer.Height),
                    System.Drawing.Imaging.ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, this.backbuffer.Width, this.backbuffer.Height,
                    PixelFormat.Bgra,
                    PixelType.UnsignedByte,
                    data.Scan0);
                this.backbuffer.UnlockBits(data);
                GL.BindTexture(TextureTarget.Texture2D, 0);
            }

            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Disable(EnableCap.CullFace);

            this.shader.UseProgram();

            GL.Uniform1(this.shader["uTexture"], 0);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, this.texture);

            GL.BindVertexArray(this.vao);
            {
                GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 6);
            }
            GL.BindVertexArray(0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.UseProgram(0);
        }

        private void OnPaint()
        {
            var rect = new Rectangle(0, 0, this.backbuffer.Width, this.backbuffer.Height);
            using (var g = Graphics.FromImage(this.backbuffer))
            {
                if (this.Paint != null)
                    this.Paint(this, new PaintEventArgs(g, rect));
                g.Flush();
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            GL.DeleteVertexArray(this.vao);
            GL.DeleteBuffer(this.vertexBuffer);
            GL.DeleteTexture(this.texture);

            this.shader?.Dispose();
			this.backbuffer?.Dispose();

            this.vao = 0;
            this.vertexBuffer = 0;
            this.texture = 0;
        }
    }
}