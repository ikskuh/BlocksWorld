using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using System;
using OpenTK;
using Jitter.Collision;

namespace BlocksWorld
{
    public partial class World : Jitter.World
    {
        class Atom
        {
            public Block Block { get; set; }

            public RigidBody Body { get; set; }
        }

        private SparseArray3D<Atom> blocks;

        public event EventHandler<BlockEventArgs> BlockChanged;

        public World() :
            base(new Jitter.Collision.CollisionSystemPersistentSAP())
        {
            this.blocks = new SparseArray3D<Atom>();
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
                return this.blocks[x, y, z]?.Block;
            }
            set
            {
                this.blocks[x, y, z] = this.blocks[x, y, z] ?? new Atom();
                var prev = this.blocks[x, y, z].Block;
                if (prev == value) // No change at all
                    return;

                if (this.blocks[x, y, z].Body != null)
                {
                    this.RemoveBody(this.blocks[x, y, z].Body);
                    this.blocks[x, y, z].Body = null;
                }

                this.blocks[x, y, z].Block = value;

                if (value != null)
                {
                    RigidBody rb = new RigidBody(new BoxShape(1.0f, 1.0f, 1.0f));
                    rb.Position = new JVector(x, y, z);
                    rb.IsStatic = true;
                    this.AddBody(rb);
                    this.blocks[x, y, z].Body = rb;
                }

                this.OnBlockChanged(value, x, y, z);
            }
        }

        public TraceResult Trace(JVector origin, JVector direction, TraceOptions options)
        {
            if (direction.Length() < 0.1f)
                return null;

            RaycastCallback callback = (b, n, f) =>
            {
                return b.IsStatic;
            };
            RigidBody body;
            JVector normal;
            float friction;

            if (this.CollisionSystem.Raycast(
                origin,
                direction,
                callback,
                out body,
                out normal,
                out friction))
            {
                return new TraceResult(body, friction, origin + friction * direction, normal);
            }
            return null;
        }

        public int LowerX { get { return this.blocks.GetLowerX(); } }
        public int LowerY { get { return this.blocks.GetLowerY(); } }
        public int LowerZ { get { return this.blocks.GetLowerZ(); } }

        public int UpperX { get { return this.blocks.GetUpperX(); } }
        public int UpperY { get { return this.blocks.GetUpperY(); } }
        public int UpperZ { get { return this.blocks.GetUpperZ(); } }
    }
}