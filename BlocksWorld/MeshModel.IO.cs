using System;
using Assimp;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using OpenTK;

namespace BlocksWorld
{
    partial class MeshModel
    {
        public static MeshModel LoadFromFile(string v)
        {
            using (var fs = File.Open(v, FileMode.Open, FileAccess.Read))
            {
                return Load(fs, Path.GetExtension(v));
            }
        }

        public static MeshModel Load(Stream stream, string formatHint)
        {
            using (AssimpContext importer = new AssimpContext())
            {
                var scene = importer.ImportFileFromStream(
                    stream,
                    PostProcessSteps.Triangulate |
                    PostProcessSteps.PreTransformVertices |
                    PostProcessSteps.GenerateUVCoords |
                    PostProcessSteps.GenerateNormals,
                    formatHint);

                List<Mesh> meshes = new List<Mesh>();
                for (int i = 0; i < scene.MeshCount; i++)
                {
                    var mesh = scene.Meshes[i];
                    var indices = mesh.GetIndices();

                    int texture = 6;

                    BlockVertex[] vertices = new BlockVertex[mesh.VertexCount];
                    var src = mesh.Vertices;
                    var normals = mesh.HasNormals ? mesh.Normals : null;
                    var uvs = mesh.HasTextureCoords(0) ? mesh.TextureCoordinateChannels[0] : null;
                    for (int v = 0; v < vertices.Length; v++)
                    {
                        vertices[v].position = src[v].TK();
                        vertices[v].color = Vector3.One;
                        if (normals != null)
                            vertices[v].normal = normals[v].TK();
                        if (uvs != null)
                            vertices[v].uv = uvs[v].TK();
                        else
                            vertices[v].uv = 0.5f * Vector3.One;

                        // Set texture ID of the model
                        vertices[v].uv.Z = texture;
                    }

                    meshes.Add(new Mesh(indices, vertices));
                }

                return new MeshModel(meshes);
            }
        }
    }
}
