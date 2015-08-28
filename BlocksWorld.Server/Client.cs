using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace BlocksWorld
{
    internal partial class Client
    {
        private readonly int id;
        private Server server;
        private Network network;
        private Dictionary<NetworkPhrase, Action> dispatcher = new Dictionary<NetworkPhrase, Action>();
        private BasicReceiver receiver;

        public Client(Server server, TcpClient tcp, int id)
        {
            this.id = id;
            this.server = server;
            this.server.World.BlockChanged += World_BlockChanged;

            this.network = new Network(tcp);
            this.receiver = new BasicReceiver(this.Network, this.server.World);

            this.Network[NetworkPhrase.SetPlayer] = this.SetPlayer;

            this.Network.SendWorld(this.server.World);

            this.Network.SpawnPlayer(16.0f, 4.0f, 3.0f);
        }

        private void SentToOthers(NetworkPhrase phrase, PhraseSender sender)
        {
            foreach(var client in this.server.Clients)
            {
                if (client == this) continue;
                client.network.Send(phrase, sender);
            }
        }

        private void SetPlayer(BinaryReader reader)
        {
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            float z = reader.ReadSingle();
            float rot = reader.ReadSingle();

            this.SentToOthers(NetworkPhrase.UpdateProxy, (s) =>
            {
                s.Write(this.id);
                s.Write(x);
                s.Write(y);
                s.Write(z);
                s.Write(rot);
            });
        }

        private void World_BlockChanged(object sender, BlockEventArgs e)
        {
            if(e.Block == null)
                this.Network.RemoveBlock(e.X, e.Y, e.Z);
            else
                this.Network.SetBlock(e.X, e.Y, e.Z, e.Block);
        }

        internal void Update(double deltaTime)
        {
            this.Network.Dispatch();
        }

        private void Kill()
        {
            this.SentToOthers(NetworkPhrase.DestroyProxy, (s) =>
            {
                s.Write(this.id);
            });

            this.IsAlive = false;
            this.Network.Disconnect();
        }

        public bool IsAlive { get; private set; } = true;

        public Network Network
        {
            get
            {
                return network;
            }
        }
    }
}