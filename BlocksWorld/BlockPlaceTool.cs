using Jitter;
using Jitter.Collision;
using Jitter.Dynamics;
using Jitter.LinearMath;
using System;

namespace BlocksWorld
{
    internal class BlockPlaceTool : Tool
    {
        private World world;

        public BlockPlaceTool(World world)
        {
            this.world = world;
        }

        public override void PrimaryUse(FirstPersonCamera cam)
        {
            var focus = this.TraceFromScreen(cam);
            if (focus == null)
                return;

            JVector block = focus.Position - 0.5f * focus.Normal;
            int x = (int)Math.Round(block.X);
            int y = (int)Math.Round(block.Y);
            int z = (int)Math.Round(block.Z);

            this.world[x, y, z] = null;
        }

        public override void SecondaryUse(FirstPersonCamera cam)
        {
            var focus = this.TraceFromScreen(cam);
            if (focus == null)
                return;

            JVector block = focus.Position + 0.5f * focus.Normal;
            int x = (int)Math.Round(block.X);
            int y = (int)Math.Round(block.Y);
            int z = (int)Math.Round(block.Z);

            this.world[x, y, z] = new BasicBlock(4);
        }

        private Focus TraceFromScreen(FirstPersonCamera firstPersonCamera)
        {
            if (firstPersonCamera == null)
                return null;

            RaycastCallback callback = (b, n, f) =>
            {
                return b.IsStatic;
            };
            RigidBody body;
            JVector normal;
            float friction;

            var from = firstPersonCamera.GetEye().Jitter();
            var dir = firstPersonCamera.GetForward().Jitter();

            if (this.world.CollisionSystem.Raycast(
                from,
                dir,
                callback,
                out body,
                out normal,
                out friction))
            {
                return new Focus()
                {
                    Position = from + friction * dir,
                    Normal = normal
                };
            }
            return null;
        }

        class Focus
        {
            public JVector Position
            {
                get; set;
            }

            public JVector Normal
            {
                get; set;
            }
        }
    }
}