using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BlocksWorld
{
    public partial class MeshModel : IRenderer
    {
        readonly List<Mesh> meshes = new List<Mesh>();

        public MeshModel(IEnumerable<Mesh> meshes)
        {
            this.meshes.AddRange(meshes);
        }

        public void Render(Camera camera, double time)
        {
            foreach (var mesh in this.meshes)
            {
                mesh.Render(camera, time);
            }
        }
        
        public IReadOnlyList<Mesh> Meshes { get { return this.meshes; } }
    }

    public class Mesh : IRenderer
    {
        private int vao;
        private int indexBuffer;
        private int vertexBuffer;
        private int vertexCount;
        private int indexCount;

        public Mesh(int[] indices, BlockVertex[] vertices)
        {
            this.indexCount = indices.Length;
            this.vertexCount = vertices.Length;

            if ((this.indexCount == 0) || (this.vertexCount == 0))
                throw new ArgumentException();

            this.vao = GL.GenVertexArray();
            this.vertexBuffer = GL.GenBuffer();
            this.indexBuffer = GL.GenBuffer();

            // Initialize vertex array
            GL.BindVertexArray(this.vao);
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, this.vertexBuffer);

                BlockVertex.InitializeVertexBinding();

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.indexBuffer);
            }
            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            // Initialize vertex buffer
            GL.BindBuffer(BufferTarget.ArrayBuffer, this.vertexBuffer);
            {
                GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(BlockVertex.SizeInBytes * vertices.Length), vertices, BufferUsageHint.StaticDraw);
            }
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            // Initialize index buffer
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.indexBuffer);
            {
                GL.BufferData(BufferTarget.ElementArrayBuffer, new IntPtr(sizeof(int) * indices.Length), indices, BufferUsageHint.StaticDraw);
            }
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        public void Render(Camera camera, double time)
        {
            if ((this.vertexCount > 0) && (this.indexCount > 0))
            {
                GL.BindVertexArray(this.vao);
                GL.DrawElements(PrimitiveType.Triangles, this.indexCount, DrawElementsType.UnsignedInt, IntPtr.Zero);
                GL.BindVertexArray(0);
            }
        }
    }
}