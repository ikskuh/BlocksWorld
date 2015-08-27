using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using System;

namespace BlocksWorld
{
    public class World : Jitter.World
    {
        private SparseArray3D<Atom> chunks;

        class Atom
        {
            public Block Block { get; set; }

            public RigidBody Body { get; set; }
        }
        
        public event EventHandler<BlockEventArgs> BlockChanged;

        public World(int sizeX, int sizeY, int sizeZ) :
            base(new Jitter.Collision.CollisionSystemPersistentSAP())
        {
            this.chunks = new SparseArray3D<Atom>();
        }

        protected void OnBlockChanged(Block block, int x, int y, int z)
        {
            if (this.BlockChanged != null)
                this.BlockChanged(this, new BlockEventArgs(block, x, y, z));
        }

        public Block this[int x, int y, int z]
        {
            get
            {
                return this.chunks[x, y, z]?.Block;
            }
            set
            {
                this.chunks[x, y, z] = this.chunks[x, y, z] ?? new Atom();
                var prev = this.chunks[x, y, z].Block;
                if (prev == value) // No change at all
                    return;

                if (this.chunks[x, y, z].Body != null)
                {
                    this.RemoveBody(this.chunks[x, y, z].Body);
                    this.chunks[x, y, z].Body = null;
                }

                this.chunks[x, y, z].Block = value;

                if (value != null)
                {
                    RigidBody rb = new RigidBody(new BoxShape(1.0f, 1.0f, 1.0f));
                    rb.Position = new JVector(x, y, z);
                    rb.IsStatic = true;
                    this.AddBody(rb);
                    this.chunks[x, y, z].Body = rb;
                }

                this.OnBlockChanged(value, x, y, z);
            }
        }

        public int LowerX { get { return this.chunks.GetLowerX(); } }
        public int LowerY { get { return this.chunks.GetLowerY(); } }
        public int LowerZ { get { return this.chunks.GetLowerZ(); } }

        public int UpperX { get { return this.chunks.GetUpperX(); } }
        public int UpperY { get { return this.chunks.GetUpperY(); } }
        public int UpperZ { get { return this.chunks.GetUpperZ(); } }
    }
}