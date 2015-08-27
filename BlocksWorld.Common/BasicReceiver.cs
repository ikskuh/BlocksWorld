using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlocksWorld
{
    public class BasicReceiver 
    {
        private Network network;
        private World world;

        public BasicReceiver(Network network, World world)
        {
            this.network = network;
            this.world = world;

            // Register basic callbacks
            this.network[NetworkPhrase.RemoveBlock] = this.RemoveBlock;
            this.network[NetworkPhrase.SetBlock] = this.SetBlock;
        }

        private void SetBlock(BinaryReader reader)
        {
            int x = reader.ReadInt32();
            int y = reader.ReadInt32();
            int z = reader.ReadInt32();
            string typeName = reader.ReadString();
            Type type = Type.GetType(typeName);
            if (type == null)
                throw new InvalidDataException();
            Block block = Activator.CreateInstance(type) as Block;
            block.Deserialize(reader);
            this.world[x, y, z] = block;
        }

        private void RemoveBlock(BinaryReader reader)
        {
            int x = reader.ReadInt32();
            int y = reader.ReadInt32();
            int z = reader.ReadInt32();
            this.world[x, y, z] = null;
        }
    }
}
