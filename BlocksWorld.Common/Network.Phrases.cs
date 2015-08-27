using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlocksWorld
{
    partial class Network
    {
        public void SendWorld(World world)
        {
            this.Send(NetworkPhrase.LoadWorld, (w) =>
            {
                world.Save(w.BaseStream, true);
            });
        }

        public void SpawnPlayer(float x, float y, float z)
        {
            this.Send(NetworkPhrase.SpawnPlayer, (w) =>
            {
                w.Write(x);
                w.Write(y);
                w.Write(z);
            });
        }

        public void RemoveBlock(int x, int y, int z)
        {
            this.Send(NetworkPhrase.RemoveBlock, (s) =>
            {
                s.Write((int)x);
                s.Write((int)y);
                s.Write((int)z);
            });
        }

        public void SetBlock(int x, int y, int z, Block b)
        {
            this.Send(NetworkPhrase.SetBlock, (s) =>
                {
                    s.Write(x);
                    s.Write(y);
                    s.Write(z);
                    s.Write(b.GetType().AssemblyQualifiedName);
                    b.Serialize(s);
                });
        }
    }
}
