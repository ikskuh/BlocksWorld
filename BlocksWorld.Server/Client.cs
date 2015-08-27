using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace BlocksWorld
{
    internal partial class Client
    {
        private Server program;
        private Network network;
        private Dictionary<NetworkPhrase, Action> dispatcher = new Dictionary<NetworkPhrase, Action>();
        
        public Client(Server program, TcpClient tcp)
        {
            this.program = program;
            this.network = new Network(tcp);
            
            this.SendWorld();

            // this.SpawnPlayer(16.0f, 4.0f, 3.0f);
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
                this.program.World.Save(w.BaseStream, true);
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