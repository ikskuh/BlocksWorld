using OpenTK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace BlocksWorld
{
    internal partial class Client : IPhraseSender
    {
        private readonly int id;
        private Server server;
        private Network network;
        private Dictionary<NetworkPhrase, Action> dispatcher = new Dictionary<NetworkPhrase, Action>();
        private BasicReceiver receiver;

        private PhraseTranslator client, others, broadcast;

        public Client(Server server, TcpClient tcp, int id)
        {
            this.id = id;
            this.server = server;

            this.network = new Network(tcp);
            this.receiver = new BasicReceiver(this.Network, this.server.World);

            this.network[NetworkPhrase.SetPlayer] = this.SetPlayer;
            this.network[NetworkPhrase.CreateNewDetail] = this.CreateNewDetail;
            this.network[NetworkPhrase.DestroyDetail] = this.DestroyDetail;
            this.network[NetworkPhrase.TriggerInteraction] = this.TriggerInteraction;

            this.client = new PhraseTranslator(this.network);
            this.broadcast = new PhraseTranslator(this.server);
            this.others = new PhraseTranslator(this);

            this.client.SendWorld(this.server.World);

            this.client.SpawnPlayer(new Vector3(16.0f, 4.0f, 3.0f));

            foreach (var detail in this.server.World.Details)
            {
                this.CreateDetail(detail);
            }

            this.server.World.BlockChanged += World_BlockChanged;
            this.server.World.DetailCreated += (s, e) =>
            {
                this.CreateDetail(e.Detail);
            };
            this.server.World.DetailChanged += (s, e) =>
            {
                this.client.UpdateDetail(e.Detail);
            };
            this.server.World.DetailRemoved += (s, e) =>
            {
                e.Detail.InteractionsChanged -= Detail_InteractionsChanged;
                this.client.DestroyDetail(e.Detail);
            };
        }

        private void CreateDetail(DetailObject detail)
        {
            this.client.CreateDetail(detail);
            this.client.SetInteractions(detail);
            detail.InteractionsChanged += Detail_InteractionsChanged;
        }

        private void Detail_InteractionsChanged(object sender, EventArgs e)
        {
            this.client.SetInteractions(sender as DetailObject);
        }

        private void TriggerInteraction(BinaryReader reader)
        {
            int id = reader.ReadInt32();
            int iid = reader.ReadInt32();

            var detail = this.server.World.GetDetail(id);
            if (detail == null)
                return;

			var interaction = detail.Interactions.FirstOrDefault(i => i.ID == iid);
			if (interaction != null)
            {
				interaction.Trigger();
            }
            else
            {
                Console.WriteLine("Interaction {0} not found on {1}.", iid, id);
            }
        }

        private void DestroyDetail(BinaryReader reader)
        {
            int id = reader.ReadInt32();
            this.server.World.RemoveDetail(id);
        }

        private void CreateNewDetail(BinaryReader reader)
        {
            string model = reader.ReadString();
            var pos = reader.ReadVector3();

            var obj = this.server.World.CreateDetail(model, pos);
        }

        private void SetPlayer(BinaryReader reader)
        {
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            float z = reader.ReadSingle();
            float rot = reader.ReadSingle();

            this.others.UpdateProxy(this.id, new Vector3(x, y, z), rot);
        }

        private void World_BlockChanged(object sender, BlockEventArgs e)
        {
            if (e.Block == null)
                this.client.RemoveBlock(e.X, e.Y, e.Z);
            else
                this.client.SetBlock(e.X, e.Y, e.Z, e.Block);
        }

        internal void Update(double deltaTime)
        {
            this.Network.Dispatch();
        }

        private void Kill()
        {
            this.others.DestroyProxy(this.id);

            this.IsAlive = false;
            this.Network.Disconnect();
        }

        void IPhraseSender.Send(NetworkPhrase phrase, PhraseSender sender)
        {
            foreach (var client in this.server.Clients)
            {
                if (client == this) continue;
                client.network.Send(phrase, sender);
            }
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