using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlocksWorld
{
    internal class MeshBlock : Block
    {
        private WorldRenderer.Vertex[] mesh;

        public MeshBlock(Vector3[] mesh, Vector3 color)
        {
            this.mesh = mesh.Select(p => new WorldRenderer.Vertex()
            {
                position = p,
                color = color
            }).ToArray();

            for (int i = 0; i < this.mesh.Length; i += 3)
            {
                Vector3 a = this.mesh[i + 0].position;
                Vector3 b = this.mesh[i + 1].position;
                Vector3 c = this.mesh[i + 2].position;

                Vector3 normal =
                    Vector3.Cross(
                        Vector3.Normalize(b - a),
                        Vector3.Normalize(c - a));

                for (int j = 0; j < 3; j++)
                {
                    this.mesh[i + j].normal = normal;
                }
            }
        }

        public override IEnumerable<WorldRenderer.Vertex> CreateMesh(AdjacentBlocks neighborhood)
        {
            return this.mesh;
        }
    }
}