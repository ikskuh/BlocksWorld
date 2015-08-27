using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace BlocksWorld
{
    public class World : Jitter.World
    {
        private SparseArray3D<Atom> blocks;

        class Atom
        {
            public Block Block { get; set; }

            public RigidBody Body { get; set; }
        }

        public event EventHandler<BlockEventArgs> BlockChanged;

        public World() :
            base(new Jitter.Collision.CollisionSystemPersistentSAP())
        {
            this.blocks = new SparseArray3D<Atom>();
        }

        protected void OnBlockChanged(Block block, int x, int y, int z)
        {
            if (this.BlockChanged != null)
                this.BlockChanged(this, new BlockEventArgs(block, x, y, z));
        }

        public void Load(string fileName)
        {
            using (var fs = File.Open(fileName, FileMode.Open, FileAccess.Read))
            {
                this.Load(fs);
            }
        }

        public void Save(string fileName)
        {
            using (var fs = File.Open(fileName, FileMode.Create, FileAccess.Write))
            {
                this.Save(fs);
            }
        }

        private void Load(FileStream fs)
        {
            // Reset whole world
            foreach (var block in blocks.ToArray())
            {
                this[block.X, block.Y, block.Z] = null;
            }

            Action<bool> assert = (b) => { if (!b) throw new InvalidDataException(); };
            using (var br = new BinaryReader(fs, Encoding.UTF8))
            {
                assert(br.ReadString() == "BLOCKWORLD");
                assert(br.ReadByte() == 1);
                assert(br.ReadByte() == 0);

                int typeCount = br.ReadInt32();
                Type[] types = new Type[typeCount];
                for (int i = 0; i < typeCount; i++)
                {
                    string typeName = br.ReadString();
                    types[i] = Type.GetType(typeName);
                }

                long blockCount = br.ReadInt64();
                for (long i = 0; i < blockCount; i++)
                {
                    int x = br.ReadInt32();
                    int y = br.ReadInt32();
                    int z = br.ReadInt32();
                    Type type = types[br.ReadInt32()];

                    var block = Activator.CreateInstance(type) as Block;

                    block.Deserialize(br);

                    this[x, y, z] = block;
                }
            }
        }

        private void Save(Stream fs)
        {
            var blocks = this.blocks.Where(x => (x.Value != null) && (x.Value.Block != null)).ToArray();
            using (var bw = new BinaryWriter(fs, Encoding.UTF8))
            {
                bw.Write("BLOCKWORLD");
                bw.Write((byte)1);
                bw.Write((byte)0);

                var types = blocks.Select(x => x.Value.Block.GetType()).Distinct().ToList();

                bw.Write(types.Count);
                for (int i = 0; i < types.Count; i++)
                {
                    bw.Write(types[i].AssemblyQualifiedName);
                }

                bw.Write(blocks.LongLength);

                foreach (var data in blocks)
                {
                    var block = data.Value.Block;

                    bw.Write(data.X);
                    bw.Write(data.Y);
                    bw.Write(data.Z);
                    bw.Write(types.IndexOf(block.GetType()));

                    block.Serialize(bw);
                }
            }
        }

        public Block this[int x, int y, int z]
        {
            get
            {
                return this.blocks[x, y, z]?.Block;
            }
            set
            {
                this.blocks[x, y, z] = this.blocks[x, y, z] ?? new Atom();
                var prev = this.blocks[x, y, z].Block;
                if (prev == value) // No change at all
                    return;

                if (this.blocks[x, y, z].Body != null)
                {
                    this.RemoveBody(this.blocks[x, y, z].Body);
                    this.blocks[x, y, z].Body = null;
                }

                this.blocks[x, y, z].Block = value;

                if (value != null)
                {
                    RigidBody rb = new RigidBody(new BoxShape(1.0f, 1.0f, 1.0f));
                    rb.Position = new JVector(x, y, z);
                    rb.IsStatic = true;
                    this.AddBody(rb);
                    this.blocks[x, y, z].Body = rb;
                }

                this.OnBlockChanged(value, x, y, z);
            }
        }

        public int LowerX { get { return this.blocks.GetLowerX(); } }
        public int LowerY { get { return this.blocks.GetLowerY(); } }
        public int LowerZ { get { return this.blocks.GetLowerZ(); } }

        public int UpperX { get { return this.blocks.GetUpperX(); } }
        public int UpperY { get { return this.blocks.GetUpperY(); } }
        public int UpperZ { get { return this.blocks.GetUpperZ(); } }
    }
}