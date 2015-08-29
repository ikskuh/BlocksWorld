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

        public void UpdateDetail(DetailObject detail)
        {
            this.sender.Send(NetworkPhrase.UpdateDetail, (s) =>
            {
                s.Write(detail.ID);
                s.Write(detail.Position);
                s.Write(detail.Rotation);
                s.Write(detail.Model);
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
    }
}
