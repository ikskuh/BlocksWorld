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
        private TcpClient tcp;
        private BinaryReader reader;
        private BinaryWriter writer;
        private Dictionary<string, Action> dispatcher = new Dictionary<string, Action>();
        
        public Client(Server program, TcpClient tcp)
        {
            this.program = program;
            this.tcp = tcp;

            var stream = this.tcp.GetStream();
            this.reader = new BinaryReader(stream, Encoding.UTF8);
            this.writer = new BinaryWriter(stream, Encoding.UTF8);

            this.SendWorld();

            this.SpawnPlayer(16.0f, 4.0f, 3.0f);
        }

        internal void Update(double deltaTime)
        {
            if (this.tcp.Available > 0)
            {
                string command = reader.ReadString();
                if (this.dispatcher.ContainsKey(command))
                {
                    this.dispatcher[command]();
                }
                else
                {
                    Console.WriteLine("Command '{0}' not recognized, going down...");
                    this.Kill();
                }
            }
        }

        private void Kill()
        {
            this.IsAlive = false;
            this.writer = BinaryWriter.Null;
            this.reader = new BinaryReader(new MemoryStream());
            this.tcp.Close();
        }

        public bool IsAlive { get; private set; }
    }

    partial class Client
    {
        private void SendWorld()
        {
            lock(this.writer)
            {
                this.writer.Write(NetworkPhrase.LOADWORLD);
                this.writer.Flush();
                this.program.World.Save(this.writer.BaseStream, true);
                this.writer.Flush();
            }
        }

        private void SpawnPlayer(float x, float y, float z)
        {
            lock(this.writer)
            {
                this.writer.Write(NetworkPhrase.SPAWNPLAYER);
                this.writer.Write(x);
                this.writer.Write(y);
                this.writer.Write(z);
            }
        }
    }
}