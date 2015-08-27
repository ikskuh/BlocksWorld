using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using System;

namespace BlocksWorld
{
    public class World : Jitter.World
    {
        private Block[,,] blocks;
        private RigidBody[,,] physics;

        public event EventHandler<BlockEventArgs> BlockChanged;

        public World(int sizeX, int sizeY, int sizeZ) : 
            base(new Jitter.Collision.CollisionSystemPersistentSAP())
        {
            this.blocks = new Block[sizeX, sizeY, sizeZ];
            this.physics = new RigidBody[sizeX, sizeY, sizeZ];
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
                if ((x < 0) || (y < 0) || (z < 0))
                    return null;
                if ((x >= this.SizeX) || (y >= this.SizeY) || (z >= this.SizeZ))
                    return null;
                return this.blocks[x, y, z];
            }
            set
            {
                var prev = this.blocks[x, y, z];
                if (prev == value) // No change at all
                    return;

                if(this.physics[x,y,z] != null)
                {
                    this.RemoveBody(this.physics[x, y, z]);
                    this.physics[x, y, z] = null;
                }

                this.blocks[x, y, z] = value;

                if(value != null)
                {
                    RigidBody rb = new RigidBody(new BoxShape(1.0f, 1.0f, 1.0f));
                    rb.Position = new JVector(x, y, z);
                    rb.IsStatic = true;
                    this.AddBody(rb);
                    this.physics[x, y, z] = rb;
                }
                
                this.OnBlockChanged(value, x, y, z);
            }
        }

        public int SizeX { get { return this.blocks.GetLength(0); } }
        public int SizeY { get { return this.blocks.GetLength(1); } }
        public int SizeZ { get { return this.blocks.GetLength(2); } }
    }
}