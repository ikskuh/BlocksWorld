using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace BlocksWorld
{
    public sealed class PhraseTranslator
    {
        private readonly IPhraseSender sender;

        public PhraseTranslator(IPhraseSender sender)
        {
            this.sender = sender;
        }

        public void SendWorld(World world)
        {
            this.sender.Send(NetworkPhrase.LoadWorld, (w) =>
            {
                world.Save(w.BaseStream, true);
            });
        }

        public void SpawnPlayer(Vector3 spawn)
        {
            this.sender.Send(NetworkPhrase.SpawnPlayer, (w) =>
            {
                w.Write(spawn);
            });
        }

        public void RemoveBlock(int x, int y, int z)
        {
            this.sender.Send(NetworkPhrase.RemoveBlock, (s) =>
            {
                s.Write((int)x);
                s.Write((int)y);
                s.Write((int)z);
            });
        }

        public void SetBlock(int x, int y, int z, Block b)
        {
            this.sender.Send(NetworkPhrase.SetBlock, (s) =>
                {
                    s.Write(x);
                    s.Write(y);
                    s.Write(z);
                    s.Write(b.GetType().AssemblyQualifiedName);
                    b.Serialize(s);
                });
        }

        public void DestroyDetail(DetailObject detail)
        {
            this.sender.Send(NetworkPhrase.DestroyDetail, (s) =>
            {
                s.Write(detail.ID);
            });
        }

        public void CreateDetail(DetailObject detail)
        {
            this.sender.Send(NetworkPhrase.CreateDetail, (s) =>
            {
                s.Write(detail.ID);
                s.Write(detail.Model);
                s.Write(detail.Position);
                s.Write(detail.Rotation);
            });
        }

        public void SetInteractions(DetailObject detail)
        {
            this.sender.Send(NetworkPhrase.SetInteractions, (s) =>
            {
                s.Write(detail.ID);
                s.Write(detail.Interactions.Count);
				foreach (var interaction in detail.Interactions)
				{
					s.Write(interaction.ID);
					s.Write(interaction.Name);
				}
			});
        }

        public void CreateNewDetail(string model, Vector3 pos)
        {
            this.sender.Send(NetworkPhrase.CreateNewDetail, (s) =>
            {
                s.Write(model);
                s.Write(pos);
            });
        }

        public void UpdateDetail(DetailObject detail)
        {
            this.sender.Send(NetworkPhrase.UpdateDetail, (s) =>
            {
                s.Write(detail.ID);
                s.Write(detail.Position);
                s.Write(detail.Rotation);
            });
        }

        public void TriggerInteraction(DetailObject detail, Interaction interaction)
        {
            this.sender.Send(NetworkPhrase.TriggerInteraction, (s) =>
            {
                s.Write(detail.ID);
				s.Write(interaction.ID);
            });
        }

        public void UpdateProxy(int id, Vector3 pos, float rot)
        {
            this.sender.Send(NetworkPhrase.UpdateProxy, (s) =>
            {
                s.Write(id);
                s.Write(pos);
                s.Write(rot);
            });
        }

        public void DestroyProxy(int id)
        {
            this.sender.Send(NetworkPhrase.DestroyProxy, (s) =>
            {
                s.Write(id);
            });
        }

        public void SetPlayer(Vector3 feetPosition, float bodyRotation)
        {
            this.sender.Send(NetworkPhrase.SetPlayer, (s) =>
            {
                s.Write(feetPosition);
                s.Write(bodyRotation);
            });
        }
    }
}
