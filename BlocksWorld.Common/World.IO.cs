using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlocksWorld
{
    partial class World
    {
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

        public void Load(Stream fs)
        {
            this.Load(fs, false);
        }

        public void Load(Stream fs, bool leaveOpen)
        {
            // Reset whole world
            foreach (var block in blocks.ToArray())
            {
                this[block.X, block.Y, block.Z] = null;
            }

            Action<bool> assert = (b) => { if (!b) throw new InvalidDataException(); };
            using (var br = new BinaryReader(fs, Encoding.UTF8, leaveOpen))
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
                    if (types[i] == null)
                        throw new TypeLoadException("Could not find type " + typeName);
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

		public void Save(Stream fs)
        {
            this.Save(fs, false);
        }

        public void Save(Stream fs, bool leaveOpen)
        {
            var blocks = this.blocks.Where(x => (x.Value != null) && (x.Value.Block != null)).ToArray();
            using (var bw = new BinaryWriter(fs, Encoding.UTF8, leaveOpen))
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
                bw.Flush();
            }
        }
    }
}
