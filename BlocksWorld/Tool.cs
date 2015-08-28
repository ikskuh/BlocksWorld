using OpenTK;

namespace BlocksWorld
{
    public abstract class Tool
    {
        private readonly IInteractiveEnvironment environment;

        protected Tool(IInteractiveEnvironment environment)
        {
            this.environment = environment;
        }

        public IInteractiveEnvironment Environment
        {
            get { return this.environment; }
        }

        public Network Network
        {
            get
            {
                return this.environment.Network;
            }
        }

        public World World
        {
            get
            {
                return this.environment.World;
            }
        }

        public abstract void PrimaryUse(Vector3 origin, Vector3 direction);

        public virtual void SecondaryUse(Vector3 origin, Vector3 direction) { }
    }
}