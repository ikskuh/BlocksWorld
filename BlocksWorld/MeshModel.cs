using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BlocksWorld
{
    public sealed partial class MeshModel : IRenderer, IDisposable
    {
        readonly List<Mesh> meshes = new List<Mesh>();

        public MeshModel(IEnumerable<Mesh> meshes)
        {
            this.meshes.AddRange(meshes);
        }

        ~MeshModel()
        {
            this.Dispose();
        }

        public void Render(Camera camera, double time)
        {
            foreach (var mesh in this.meshes)
            {
                mesh.Render(camera, time);
            }
        }

        public IReadOnlyList<Mesh> Meshes { get { return this.meshes; } }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            foreach (var mesh in this.meshes)
                mesh.Dispose();
        }
    }
}