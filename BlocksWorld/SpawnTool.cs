using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlocksWorld
{
    public sealed class SpawnTool : Tool
    {
        public SpawnTool(IInteractiveEnvironment env) : 
            base(env)
        {

        }

        public override void PrimaryUse(Vector3 origin, Vector3 direction)
        {
            var focus = this.World.Trace(origin.Jitter(), direction.Jitter(), TraceOptions.IgnoreDynamic);
            if (focus == null)
                return;

            this.Server.CreateNewDetail("door", focus.Position.TK());
        }

        public override void SecondaryUse(Vector3 origin, Vector3 direction)
        {
            var focus = this.World.Trace(origin.Jitter(), direction.Jitter(), TraceOptions.IgnoreDynamic);
            var detail = focus?.Body?.Tag as DetailObject;
            if (detail == null)
                return;
            this.Server.DestroyDetail(detail);
        }
    }
}
