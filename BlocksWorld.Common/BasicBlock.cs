using OpenTK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlocksWorld
{
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

        public override IEnumerable<BlockVertex> CreateMesh(AdjacentBlocks neighborhood)
        {
            Vector3 color = this.Color;
            List<BlockVertex> vertices = new List<BlockVertex>();
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

        BlockVertex[] CreateInstance(BlockVertex[] template, Vector3 color)
        {
            BlockVertex[] instance = (BlockVertex[])template.Clone();
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
