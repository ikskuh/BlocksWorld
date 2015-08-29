using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;

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

        private List<Quads> quads = new List<Quads>();

        public void Reset()
        {
            this.quads.Clear();
        }

        public void Draw(int textureID, Vector2 position, Vector2 size, ImageAnchor anchor)
        {
            position.X /= this.VirtualScreenSize.X;
            position.Y /= this.VirtualScreenSize.Y;
            
            size.X /= this.VirtualScreenSize.X;
            size.Y /= this.VirtualScreenSize.Y;

            // First/second byte encoding in anchor defines position
            switch((int)anchor & 0xF0)
            {
                case 0x00: break;
                case 0x10: position.X -= 0.5f * size.X; break;
                case 0x20: position.X -= size.X; break;
            }
            switch ((int)anchor & 0x0F)
            {
                case 0x00: break;
                case 0x01: position.Y -= 0.5f * size.Y; break;
                case 0x02: position.Y -= size.Y; break;
            }

            position.Y = 1.0f - position.Y;

            this.quads.Add(new Quads()
            {
                TextureID = textureID,
                Position = position,
                Size = size
            });
        }

        public void Render(Camera camera, double time)
        {
            if (this.vao == 0)
                throw new ObjectDisposedException("UIRenderer");

            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Disable(EnableCap.CullFace);

            GL.UseProgram(this.shader);

            GL.Uniform1(this.locTextures, 0);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(this.textures.Target, this.textures.ID);

            GL.BindVertexArray(this.vao);
            {
                foreach (var quad in this.quads)
                {                    
                    GL.Uniform2(this.locPosition, quad.Position);
                    GL.Uniform2(this.locSize, quad.Size);
                    GL.Uniform1(this.locTextureID, (float)quad.TextureID);

                    GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 6);
                }
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

            this.textures?.Dispose();

            this.vao = 0;
            this.vertexBuffer = 0;
            this.shader = 0;
        }

        public Vector2 VirtualScreenSize { get; set; } = new Vector2(1280.0f, 720.0f);

        private class Quads
        {
            public Vector2 Position { get; internal set; }
            public Vector2 Size { get; internal set; }
            public int TextureID { get; internal set; }
        }
    }
}