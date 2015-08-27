using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace BlocksWorld
{
    internal partial class Client
    {
        private Server server;
        private Network network;
        private Dictionary<NetworkPhrase, Action> dispatcher = new Dictionary<NetworkPhrase, Action>();

        public Client(Server server, TcpClient tcp)
        {
            this.server = server;
            this.server.World.BlockChanged += World_BlockChanged;

            this.network = new Network(tcp);
            this.network[NetworkPhrase.RemoveBlock] = this.RemoveBlock;
            this.network[NetworkPhrase.SetBlock] = this.SetBlock;

            this.SendWorld();

            this.SpawnPlayer(16.0f, 4.0f, 3.0f);
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
            this.server.World[x, y, z] = block;
        }

        private void World_BlockChanged(object sender, BlockEventArgs e)
        {
            if(e.Block == null)
            {
                this.network.Send(NetworkPhrase.RemoveBlock, (w) =>
                {
                    w.Write(e.X);
                    w.Write(e.Y);
                    w.Write(e.Z);
                });
            }
            else
            {
                this.network.Send(NetworkPhrase.SetBlock, (s) =>
                {
                    s.Write(e.X);
                    s.Write(e.Y);
                    s.Write(e.Z);
                    s.Write(e.Block.GetType().AssemblyQualifiedName);
                    e.Block.Serialize(s);
                });
            }
        }

        private void RemoveBlock(BinaryReader reader)
        {
            int x = reader.ReadInt32();
            int y = reader.ReadInt32();
            int z = reader.ReadInt32();
            this.server.World[x, y, z] = null;
        }

        internal void Update(double deltaTime)
        {
            this.network.Dispatch();
        }

        private void Kill()
        {
            this.IsAlive = false;
            this.network.Disconnect();
        }

        public bool IsAlive { get; private set; } = true;
    }

    partial class Client
    {
        private void SendWorld()
        {
            this.network.Send(NetworkPhrase.LoadWorld, (w) =>
            {
                this.server.World.Save(w.BaseStream, true);
            });
        }

        private void SpawnPlayer(float x, float y, float z)
        {
            this.network.Send(NetworkPhrase.SpawnPlayer, (w) =>
            {
                w.Write(x);
                w.Write(y);
                w.Write(z);
            });
        }
    }
}