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
        public static MeshModel LoadFromFile(string fileName)
        {
            using (var fs = File.Open(fileName, FileMode.Open, FileAccess.Read))
            {
                if (Path.GetExtension(fileName) == ".bwm")
                {
                    return Deserialize(fs, true);
                }
                else
                {
                    return Load(fs, Path.GetExtension(fileName));
                }
            }
        }

        public static MeshModel Load(Stream stream, string formatHint)
        {
            using (AssimpContext importer = new AssimpContext())
            {
                var scene = importer.ImportFileFromStream(
                    stream,
                    PostProcessSteps.Triangulate |
                    // PostProcessSteps.PreTransformVertices |
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

                    meshes.Add(new Mesh(indices, vertices, texture));
                }

                return new MeshModel(meshes);
            }
        }

        public void Serialize(Stream stream)
        {
            this.Serialize(stream, false);
        }

        public void Serialize(Stream stream, bool leaveOpen)
        {
            using (var bw = new BinaryWriter(stream, Encoding.UTF8, leaveOpen))
            {
                this.Serialize(bw);
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write("MESHMODEL");
            writer.Write((byte)1);
            writer.Write((byte)0);
            writer.Write(this.meshes.Count);
            for (int i = 0; i < this.meshes.Count; i++)
            {
                this.meshes[i].Serialize(writer);
            }
        }


        public static MeshModel LoadFromResource(string name)
        {
            using (var stream = typeof(MeshModel).Assembly.GetManifestResourceStream(name))
            {
                return Deserialize(stream, true);
            }
        }

        public static MeshModel Load(Stream stream)
        {
            return Deserialize(stream, false);
        }

        public static MeshModel Deserialize(Stream stream, bool leaveOpen)
        {
            using (var reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen))
            {
                return Deserialize(reader);
            }
        }

        public static MeshModel Deserialize(BinaryReader reader)
        {
            Action<bool> assert = (b) => { if (!b) throw new InvalidDataException(); };
            assert(reader.ReadString() == "MESHMODEL");
            assert(reader.ReadByte() == 1);
            assert(reader.ReadByte() == 0);

            int meshCount = reader.ReadInt32();

            List<Mesh> meshes = new List<Mesh>();
            for (int i = 0; i < meshCount; i++)
            {
                var mesh = Mesh.Deserialize(reader);
                meshes.Add(mesh);
            }

            return new MeshModel(meshes);
        }
    }

    partial class Mesh
    {
        public void Serialize(Stream stream)
        {
            this.Serialize(stream, false);
        }

        public void Serialize(Stream stream, bool leaveOpen)
        {
            using (var bw = new BinaryWriter(stream, Encoding.UTF8, leaveOpen))
            {
                this.Serialize(bw);
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write("MESH");
            writer.Write((byte)1); // Version
            writer.Write((byte)0);
            writer.Write(this.texture);
            writer.Write(this.vertexCount);
            writer.Write(this.indexCount);

            for (int i = 0; i < this.vertexCount; i++)
            {
                var vertex = this.Vertices[i];

                writer.Write(vertex.position);
                writer.Write(vertex.normal);
                writer.Write(vertex.color);
                writer.Write(vertex.uv);
            }

            for (int i = 0; i < this.indexCount; i++)
            {
                writer.Write(this.indices[i]);
            }
        }

        public static Mesh Deserialize(Stream stream)
        {
            return Deserialize(stream, false);
        }

        public static Mesh Deserialize(Stream stream, bool leaveOpen)
        {
            using (var reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen))
            {
                return Deserialize(reader);
            }
        }

        public static Mesh Deserialize(BinaryReader reader)
        {
            Action<bool> assert = (b) => { if (!b) throw new InvalidDataException(); };
            assert(reader.ReadString() == "MESH");
            assert(reader.ReadByte() == 1);
            assert(reader.ReadByte() == 0);
            int texture = reader.ReadInt32();
            int vertexCount = reader.ReadInt32();
            int indexCount = reader.ReadInt32();

            var vertices = new BlockVertex[vertexCount];
            for (int i = 0; i < vertexCount; i++)
            {
                var vertex = new BlockVertex();

                vertex.position = reader.ReadVector3();
                vertex.normal = reader.ReadVector3();
                vertex.color = reader.ReadVector3();
                vertex.uv = reader.ReadVector3();

                vertices[i] = vertex;
            }

            var indices = new int[indexCount];
            for (int i = 0; i < indexCount; i++)
            {
                indices[i] = reader.ReadInt32();
            }

            return new Mesh(indices, vertices, texture);
        }
    }
}
