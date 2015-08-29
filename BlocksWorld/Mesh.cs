using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlocksWorld
{
    public sealed partial class Mesh : IRenderer, IDisposable
    {
        private int vao;
        private int indexBuffer;
        private int vertexBuffer;

        private readonly int vertexCount;
        private readonly int indexCount;

        readonly int[] indices;
        readonly BlockVertex[] vertices;

        private int texture;

        public Mesh(int[] indices, BlockVertex[] vertices, int texture)
        {
            this.texture = texture;
            this.indexCount = indices.Length;
            this.vertexCount = vertices.Length;

            if ((this.indexCount == 0) || (this.vertexCount == 0))
                throw new ArgumentException();

            this.vertices = vertices;
            this.indices = indices;

            this.vao = GL.GenVertexArray();
            this.vertexBuffer = GL.GenBuffer();
            this.indexBuffer = GL.GenBuffer();

            // Enforce passed texture
            this.SetTexture(this.texture, false);

            // Initialize vertex array
            InitializeVertexArrayObject();

            // Initialize vertex buffer
            UploadVertices();

            // Initialize index buffer
            UploadIndices();
        }

        private void InitializeVertexArrayObject()
        {
            GL.BindVertexArray(this.vao);
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, this.vertexBuffer);

                BlockVertex.InitializeVertexBinding();

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.indexBuffer);
            }
            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        public void SetTexture(int texture)
        {
            this.SetTexture(texture, true);
        }

        private void SetTexture(int texture, bool upload)
        {
            this.texture = texture;
            for (int i = 0; i < this.Vertices.Length; i++)
            {
                this.Vertices[i].uv.Z = texture;
            }
            if (upload)
            {
                this.UploadVertices();
            }
        }

        public int Texture
        {
            get { return this.texture; }
        }

        public BlockVertex[] Vertices
        {
            get
            {
                return vertices;
            }
        }

        private void UploadIndices()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.indexBuffer);
            {
                GL.BufferData(BufferTarget.ElementArrayBuffer, new IntPtr(sizeof(int) * this.indices.Length), this.indices, BufferUsageHint.StaticDraw);
            }
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        private void UploadVertices()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, this.vertexBuffer);
            {
                GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(BlockVertex.SizeInBytes * this.Vertices.Length), this.Vertices, BufferUsageHint.StaticDraw);
            }
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        ~Mesh()
        {
            this.Dispose();
        }

        public void Update()
        {
            this.UploadVertices();
        }

        public void Render(Camera camera, double time)
        {
            if (this.vao == 0) throw new ObjectDisposedException("Mesh");
            if ((this.vertexCount > 0) && (this.indexCount > 0))
            {
                GL.BindVertexArray(this.vao);
                GL.DrawElements(PrimitiveType.Triangles, this.indexCount, DrawElementsType.UnsignedInt, IntPtr.Zero);
                GL.BindVertexArray(0);
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            GL.DeleteVertexArray(this.vao);
            GL.DeleteBuffer(this.indexBuffer);
            GL.DeleteBuffer(this.vertexBuffer);

            this.vao = 0;
            this.indexBuffer = 0;
            this.vertexBuffer = 0;
        }
    }
}
