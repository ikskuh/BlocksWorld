﻿using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BlocksWorld
{
    public partial class WorldRenderer : IRenderer
    {
        private readonly World world;
        private int vao;
        private int vertexBuffer;
        private int vertexCount;
        private bool invalid = true;

        public struct Vertex
        {
            public static readonly int SizeInBytes =
                Vector3.SizeInBytes + // position
                Vector3.SizeInBytes + // normal
                Vector3.SizeInBytes + // color
                Vector3.SizeInBytes;  // uv

            public Vector3 position;

            public Vector3 normal;

            public Vector3 color;

            public Vector3 uv;
        }

        public WorldRenderer(World world)
        {
            this.world = world;
            this.world.BlockChanged += (s, e) => this.invalid = true;
        }

        internal void Load()
        {
            this.vao = GL.GenVertexArray();
            this.vertexBuffer = GL.GenBuffer();

            GL.BindVertexArray(this.vao);
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, this.vertexBuffer);

                GL.EnableVertexAttribArray(0);
                GL.VertexAttribPointer(
                    0,
                    3,
                    VertexAttribPointerType.Float,
                    false,
                    Vertex.SizeInBytes,
                    0);

                GL.EnableVertexAttribArray(1);
                GL.VertexAttribPointer(
                    1,
                    3,
                    VertexAttribPointerType.Float,
                    false,
                    Vertex.SizeInBytes,
                    Vector3.SizeInBytes);

                GL.EnableVertexAttribArray(2);
                GL.VertexAttribPointer(
                    2,
                    3,
                    VertexAttribPointerType.Float,
                    false,
                    Vertex.SizeInBytes,
                    2 * Vector3.SizeInBytes);

                GL.EnableVertexAttribArray(3);
                GL.VertexAttribPointer(
                    3,
                    3,
                    VertexAttribPointerType.Float,
                    false,
                    Vertex.SizeInBytes,
                    3 * Vector3.SizeInBytes);
            }
            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            this.UpdateVertexBuffer();
        }

        void UpdateVertexBuffer()
        {
            if (this.invalid == false)
                return;
            var watch = Stopwatch.StartNew();
            List<Vertex> vertices = new List<Vertex>();
            Vector3[] colors = new[]
            {
                Vector3.UnitX, Vector3.UnitY, Vector3.UnitZ,
                new Vector3(1, 1, 0), new Vector3(1, 0, 1), new Vector3(0, 1, 1),
                new Vector3(1, 1, 1)
            };

            for (int x = 0; x < this.world.SizeX; x++)
            {
                for (int z = 0; z < this.world.SizeZ; z++)
                {
                    for (int y = 0; y < this.world.SizeY; y++)
                    {
                        Block block = this.world[x, y, z];
                        if (block == null)
                            continue;

                        Vector3 origin = new Vector3(x, y, z);

                        Block.AdjacentBlocks neighborhood = new Block.AdjacentBlocks();
                        neighborhood.Bottom = Block.IsSimilar(block, this.world[x, y - 1, z]) == false;
                        neighborhood.Top = Block.IsSimilar(block, this.world[x, y + 1, z]) == false;
                        neighborhood.NegativeX = Block.IsSimilar(block, this.world[x - 1, y, z]) == false;
                        neighborhood.PositiveX = Block.IsSimilar(block, this.world[x + 1, y, z]) == false;
                        neighborhood.NegativeZ = Block.IsSimilar(block, this.world[x, y, z - 1]) == false;
                        neighborhood.PositiveZ = Block.IsSimilar(block, this.world[x, y, z + 1]) == false;
                        
                        Vertex[] blockVertices = block.CreateMesh(neighborhood).ToArray();
                        for (int i = 0; i < blockVertices.Length; i++)
                        {
                            blockVertices[i].position += origin;
                        }
                        vertices.AddRange(blockVertices);
                    }
                }
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, this.vertexBuffer);
            {
                Vertex[] data = vertices.ToArray();
                this.vertexCount = data.Length;
                GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(Vertex.SizeInBytes * data.Length), data, BufferUsageHint.StaticDraw);
            }
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            watch.Stop();
            Console.WriteLine("Regenereted world in {0}", watch.Elapsed);

            this.invalid = false;
        }

        public void Render(Camera camera, double time)
        {
            if(this.invalid)
            {
                this.UpdateVertexBuffer();
            }
            if (this.vertexCount > 0)
            {
                GL.BindVertexArray(this.vao);
                GL.DrawArrays(PrimitiveType.Triangles, 0, this.vertexCount);
                GL.BindVertexArray(0);
            }
        }

        public World World
        {
            get
            {
                return world;
            }
        }
    }
}