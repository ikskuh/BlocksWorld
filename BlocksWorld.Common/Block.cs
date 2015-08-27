using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace BlocksWorld
{
    public abstract class Block
    {
        public static bool IsSimilar(Block block1, Block block2)
        {
            if ((block1 == null) && (block2 == null))
                return true;
            if ((block1 != null) && (block2 != null))
                return true;
            // TODO: Extend behaviour
            return false;
        }

        public struct AdjacentBlocks
        {
            public bool Top;
            public bool Bottom;
            public bool NegativeX;
            public bool PositiveX;
            public bool NegativeZ;
            public bool PositiveZ;
        }

        public event EventHandler Changed;

        protected void OnChanged()
        {
            if (this.Changed != null)
                this.Changed(this, EventArgs.Empty);
        }

        public abstract IEnumerable<BlockVertex> CreateMesh(AdjacentBlocks neighborhood);

        public virtual void Serialize(BinaryWriter bw)
        {

        }

        public virtual void Deserialize(BinaryReader br)
        {

        }
    }
}