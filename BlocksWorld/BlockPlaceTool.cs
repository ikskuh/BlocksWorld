using Jitter;
using Jitter.Collision;
using Jitter.Dynamics;
using Jitter.LinearMath;
using OpenTK;
using System;

namespace BlocksWorld
{
    internal class BlockPlaceTool : Tool
    {
        public BlockPlaceTool(IInteractiveEnvironment env) : 
            base(env)
        {

        }

        public override void PrimaryUse(Vector3 origin, Vector3 direction)
        {
            var focus = this.World.Trace(origin.Jitter(), direction.Jitter(), TraceOptions.IgnoreDynamic);
            if (focus == null)
                return;

            JVector block = focus.Position - 0.5f * focus.Normal;
            int x = (int)Math.Round(block.X);
            int y = (int)Math.Round(block.Y);
            int z = (int)Math.Round(block.Z);

            this.Network.RemoveBlock(x, y, z);
        }

        public override void SecondaryUse(Vector3 origin, Vector3 direction)
        {
            var focus = this.World.Trace(origin.Jitter(), direction.Jitter(), TraceOptions.IgnoreDynamic);
            if (focus == null)
                return;

            JVector block = focus.Position + 0.5f * focus.Normal;
            int x = (int)Math.Round(block.X);
            int y = (int)Math.Round(block.Y);
            int z = (int)Math.Round(block.Z);

            this.Network.SetBlock(x, y, z, new BasicBlock(4));
        }
    }
}