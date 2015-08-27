using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace BlocksWorld
{
    public abstract class Block
    {
        internal static bool IsSimilar(Block block1, Block block2)
        {
            if ((block1 == null) && (block2 == null))
                return true;
            if ((block1 != null) && (block2 != null))
                return true;
            // TODO: Extend behaviour
            return false;
        }

        public struct AdjacentBlocks
        {
            public bool Top;
            public bool Bottom;
            public bool NegativeX;
            public bool PositiveX;
            public bool NegativeZ;
            public bool PositiveZ;
        }

        public abstract IEnumerable<WorldRenderer.Vertex> CreateMesh(AdjacentBlocks neighborhood);

        public virtual void Serialize(BinaryWriter bw)
        {

        }

        public virtual void Deserialize(BinaryReader br)
        {

        }
    }

    public sealed partial class BasicBlock : Block
    {
        int texture;

        public BasicBlock() :
            this(0)
        {

        }

        public BasicBlock(int texture)
        {
            this.texture = texture;
        }

        public BasicBlock(Vector3 color, int texture)
             : this(texture)
        {
            this.Color = color;
        }

        public Vector3 Color { get; set; } = Vector3.One;

        public override IEnumerable<WorldRenderer.Vertex> CreateMesh(AdjacentBlocks neighborhood)
        {
            Vector3 color = this.Color;
            List<WorldRenderer.Vertex> vertices = new List<WorldRenderer.Vertex>();
            if (neighborhood.Top)
                vertices.AddRange(CreateInstance(topSideTemplate, color));

            if (neighborhood.Bottom)
                vertices.AddRange(CreateInstance(bottomSideTemplate, color));

            if (neighborhood.NegativeX)
                vertices.AddRange(CreateInstance(negativeXSideTemplate, color));

            if (neighborhood.PositiveX)
                vertices.AddRange(CreateInstance(positiveXSideTemplate, color));

            if (neighborhood.NegativeZ)
                vertices.AddRange(CreateInstance(negativeZSideTemplate, color));

            if (neighborhood.PositiveZ)
                vertices.AddRange(CreateInstance(positiveZSideTemplate, color));

            return vertices.Select(v => { v.uv.Z = this.texture; return v; });
        }

        WorldRenderer.Vertex[] CreateInstance(WorldRenderer.Vertex[] template, Vector3 color)
        {
            WorldRenderer.Vertex[] instance = (WorldRenderer.Vertex[])template.Clone();
            for (int i = 0; i < instance.Length; i++)
            {
                instance[i].color = color;
            }
            return instance;
        }

        public override void Serialize(BinaryWriter bw)
        {
            bw.Write(this.texture);
            bw.Write(this.Color.X);
            bw.Write(this.Color.Y);
            bw.Write(this.Color.Z);
            base.Serialize(bw);
        }

        public override void Deserialize(BinaryReader br)
        {
            Vector3 color;
            this.texture = br.ReadInt32();
            color.X = br.ReadSingle();
            color.Y = br.ReadSingle();
            color.Z = br.ReadSingle();
            this.Color = color;
            base.Deserialize(br);
        }
    }
}