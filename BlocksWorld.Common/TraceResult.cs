using Jitter.Dynamics;
using Jitter.LinearMath;

namespace BlocksWorld
{
    public class TraceResult
    {
        public TraceResult(RigidBody body, float distance, JVector position, JVector normal)
        {
            this.Body = body;
            this.Distance = distance;
            this.Position = position;
            this.Normal = normal;
        }

        public RigidBody Body { get; private set; }
        public float Distance { get; private set; }
        public JVector Position { get; private set; }
        public JVector Normal { get; private set; }
    }
}