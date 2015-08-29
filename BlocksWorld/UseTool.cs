using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlocksWorld
{
    public sealed class UseTool : Tool
    {
        public UseTool(IInteractiveEnvironment env) :
            base(env)
        {

        }

        public override void PrimaryUse(Vector3 origin, Vector3 direction)
        {
            var focus = this.World.Trace(origin.Jitter(), direction.Jitter(), TraceOptions.IgnoreDynamic);
            var detail = focus?.Body?.Tag as DetailObject;
            if (detail == null)
                return;
            var interaction = detail.Interactions.FirstOrDefault();
            if (interaction == null)
                Console.WriteLine("Detail {0} has no interactions!", detail.ID);
            else
                this.Server.TriggerInteraction(detail, interaction);
        }
    }
}
