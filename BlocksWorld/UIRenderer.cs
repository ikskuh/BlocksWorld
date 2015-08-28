using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;

namespace BlocksWorld
{
    internal class UIRenderer : IRenderer, IDisposable
    {
        int shader;
        int vao;
        int vertexBuffer;

        int locPosition, locSize, locTextures;

        Vector2[] vertices = new[]
        {
            new Vector2(0.0f, 0.0f),
            new Vector2(1.0f, 0.0f),
            new Vector2(1.0f, 1.0f),

            new Vector2(0.0f, 0.0f),
            new Vector2(0.0f, 1.0f),
            new Vector2(1.0f, 1.0f),
        };
        private TextureArray textures;
        private int locTextureID;

        ~UIRenderer()
        {
            this.Dispose();
        }

        public void Load()
        {
            this.shader = Shader.CompileFromResource(
                "BlocksWorld.Shaders.ScreenSpace.vs",
                "BlocksWorld.Shaders.Texture.fs");

            this.textures = TextureArray.LoadFromResource(
                "BlocksWorld.Textures.UI.png");

            this.locPosition = GL.GetUniformLocation(this.shader, "uUpperLeft");
            this.locSize = GL.GetUniformLocation(this.shader, "uSize");
            this.locTextureID = GL.GetUniformLocation(this.shader, "uTextureID");
            this.locTextures = GL.GetUniformLocation(this.shader, "uTextures");

            this.vao = GL.GenVertexArray();
            this.vertexBuffer = GL.GenBuffer();

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
        }

        public void Render(Camera camera, double time)
        {
            if (this.vao == 0)
                throw new ObjectDisposedException("UIRenderer");

            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            GL.UseProgram(this.shader);

            GL.Uniform2(this.locPosition, new Vector2(0.4f, 0.4f));
            GL.Uniform2(this.locSize, new Vector2(0.2f, 0.2f));
            GL.Uniform1(this.locTextures, 0);
            GL.Uniform1(this.locTextureID, 0.0f);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(this.textures.Target, this.textures.ID);

            GL.BindVertexArray(this.vao);
            {
                GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 6);
            }
            GL.BindVertexArray(0);

            GL.UseProgram(0);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            GL.DeleteVertexArray(this.vao);
            GL.DeleteBuffer(this.vertexBuffer);
            GL.DeleteProgram(this.shader);

            this.vao = 0;
            this.vertexBuffer = 0;
            this.shader = 0;
        }
    }
}