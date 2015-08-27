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
        private BasicReceiver receiver;

        public Client(Server server, TcpClient tcp)
        {
            this.server = server;
            this.server.World.BlockChanged += World_BlockChanged;

            this.network = new Network(tcp);
            this.receiver = new BasicReceiver(this.network, this.server.World);

            this.network.SendWorld(this.server.World);

            this.network.SpawnPlayer(16.0f, 4.0f, 3.0f);
        }

        private void World_BlockChanged(object sender, BlockEventArgs e)
        {
            if(e.Block == null)
                this.network.RemoveBlock(e.X, e.Y, e.Z);
            else
                this.network.SetBlock(e.X, e.Y, e.Z, e.Block);
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
}