﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.IO;

namespace BlocksWorld
{
    class Server
    {
        private TcpListener server;
        private HashSet<Client> clients = new HashSet<Client>();
        private World world;

        static void Main(string[] args)
        {
            var pgm = new Server();
            pgm.Run();
        }

        void Run()
        {
            this.world = new World();

            LoadWorld();

            this.server = new TcpListener(IPAddress.Any, 4523);
            server.Start();
            BeginAccept();

            Stopwatch timer = Stopwatch.StartNew();
            while(true)
            {
                double deltaTime = timer.Elapsed.TotalSeconds;
                lock(this.clients)
                {
                    foreach (var client in this.clients)
                    {
                        client.Update(deltaTime);
                    }
                    this.clients.RemoveWhere(c =>(c.IsAlive == false));
                }
                Thread.Sleep((int)Math.Max(0, 30 - deltaTime)); // About 30 FPS
                timer.Restart();
            }
        }

        private void LoadWorld()
        {
            if (File.Exists("world.dat"))
            {
                this.World.Load("world.dat");
            }
            else
            {
                for (int x = 0; x <= 32; x++)
                {
                    for (int z = 0; z < 32; z++)
                    {
                        this.World[x, 0, z] = new BasicBlock(2);
                    }
                }

                this.World[1, 1, 1] = new BasicBlock(1);

                for (int x = 0; x < 32; x++)
                {
                    for (int y = 1; y < 4; y++)
                    {
                        if ((x != 16) || (y >= 3))
                            this.World[x, y, 8] = new BasicBlock(3);
                    }
                }
            }
        }

        private void BeginAccept()
        {
            this.server.BeginAcceptTcpClient(this.EndAccept, null);
        }

        private void EndAccept(IAsyncResult ar)
        {
            var tcp = this.server.EndAcceptTcpClient(ar);
            var client = new Client(this, tcp);

            lock(this.clients)
            {
                this.clients.Add(client);
            }
        }

        public World World
        {
            get
            {
                return world;
            }
        }
    }
}