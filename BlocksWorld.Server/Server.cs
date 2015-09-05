using System;
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
using OpenTK;
using System.Text.RegularExpressions;

namespace BlocksWorld
{
	class Server : IPhraseSender
	{
		private TcpListener server;
		private HashSet<Player> clients = new HashSet<Player>();
		private World world;
		private bool running = true;
		private volatile int clientIdCounter = 0;

		static void Main(string[] args)
		{
			var pgm = new Server();

			var console = new Thread(pgm.ServerConsole);
			console.IsBackground = true;
			console.Start();

			pgm.Run();
		}

		void ServerConsole()
		{
			var stringMatch = new Regex(@"\""(?<text>.*?)\""|(?<text>\w+)", RegexOptions.Compiled);
			var commands = new Dictionary<string, Action<string[]>>()
			{
				["shutdown"] = this.Shutdown,
				["quit"] = this.Shutdown,
				["save"] = this.Save,
			};
			while(true)
			{
				string line = Console.ReadLine();
				var matches = stringMatch.Matches(line);
				var command = matches.Cast<Match>().Take(1).Select(x => x.Groups["text"].Value).First().ToLower();
				var args = matches.Cast<Match>().Skip(1).Select(x => x.Groups["text"].Value).ToArray();
				if(commands.ContainsKey(command))
				{
					commands[command](args);
				}
				else
				{
					Console.WriteLine("Command ´{0}´ not found.", command);
				}
			}
		}

		private void Save(string[] obj)
		{
			Console.WriteLine("Saving world...");
			this.world.Save("world.dat", WorldSaveMode.Default);
			Console.WriteLine("World saved.");
		}

		void Shutdown(string[] args)
		{
			this.running = false;
		}

		void Run()
		{
			GenerateWorld();

			this.server = new TcpListener(IPAddress.Any, 4523);
			server.Start();
			BeginAccept();

			Stopwatch timer = new Stopwatch();
			while (this.running)
			{
				double deltaTime = timer.Elapsed.TotalSeconds;
				timer.Restart();

				foreach (var detail in this.world.Details)
				{
					detail.Update(deltaTime);
				}

				lock (this.clients)
				{
					foreach (var client in this.clients)
					{
						client.Update(deltaTime);
					}
					this.clients.RemoveWhere(c => (c.IsAlive == false));
				}

				Thread.Sleep((int)Math.Max(0, 30 - deltaTime)); // About 30 FPS
			}

			server.Stop();
			Thread.Sleep(50);

			Console.WriteLine("Disconnecting clients...");
			lock(this.clients)
			{
				foreach(var client in this.clients)
				{
					client.Dispose();
				}
				this.clients.Clear();
			}
			Console.WriteLine("done.");
		}

		private void GenerateWorld()
		{
			this.world = new World();
			this.LoadWorld();
			this.world.DetailInterationTriggered += World_DetailInterationTriggered;

			return;

			var objA = this.world.CreateDetail("table_a", new Vector3(8.0f, 1.3f, 4.0f));
			objA.Rotation = Quaternion.FromAxisAngle(Vector3.UnitY, (float)(0.32f * Math.PI));

			var objB = this.world.CreateDetail("table_b", new Vector3(10.0f, 1.3f, 4.0f));
			objA.Rotation = Quaternion.FromAxisAngle(Vector3.UnitY, (float)(0.1f * Math.PI));

			var behavA = objA.CreateBehaviour<FlipOverBehaviour>();
			var behavA1 = objA.CreateBehaviour<ButtonBehaviour>();
			var behavA2 = objA.CreateBehaviour<ButtonBehaviour>();

			var behavB = objB.CreateBehaviour<FlipOverBehaviour>();
			var behavB1 = objB.CreateBehaviour<ButtonBehaviour>();
			var behavB2 = objB.CreateBehaviour<RotationBehaviour>();

			// Connect the both behaviours
			behavB1.Signals["clicked"].Connect(behavA.Slots["flip"]);
			behavA1.Signals["clicked"].Connect(behavB2.Slots["toggle"]);
			behavA2.Signals["clicked"].Connect(behavB1.Slots["toggle"]);
		}

		private void World_DetailInterationTriggered(object sender, DetailInteractionEventArgs e)
		{
			Console.WriteLine("Interaction '{0}' at object {1}", e.Interaction, e.Detail.ID);
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
						this.World[x, 0, z] = new BasicBlock(3);
					}
				}

				this.World[1, 1, 1] = new BasicBlock(1);

				for (int x = 0; x < 32; x++)
				{
					for (int y = 1; y < 4; y++)
					{
						if ((x != 16) || (y >= 3))
							this.World[x, y, 8] = new BasicBlock(2);
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
			try
			{
				var tcp = this.server.EndAcceptTcpClient(ar);
				var client = new Player(this, tcp, ++clientIdCounter);

				lock (this.clients)
				{
					this.clients.Add(client);
				}

				this.BeginAccept();
			}
			catch(ObjectDisposedException){ }
		}

		void IPhraseSender.Send(NetworkPhrase phrase, PhraseSender sender)
		{
			foreach (var client in this.Clients)
			{
				client.Network.Send(phrase, sender);
			}
		}

		static readonly Regex regexCleanText = new Regex(@"[^A-Za-z0-9_ ]", RegexOptions.Compiled);
		Dictionary<string, DetailTemplate> templates = new Dictionary<string, DetailTemplate>();

		internal DetailTemplate GetPrefab(string model)
		{
			model = regexCleanText.Replace(model, "");

			if (templates.ContainsKey(model) == false)
			{
				var template = Xml.LoadFromFile<DetailTemplate>("./Details/" + model + ".xml");
				templates.Add(model, template);
			}

			return templates[model];
		}

		public World World
		{
			get
			{
				return world;
			}
		}

		public IEnumerable<Player> Clients
		{
			get
			{
				lock (this.clients)
				{
					return this.clients.ToArray();
				}
			}
		}
	}
}
