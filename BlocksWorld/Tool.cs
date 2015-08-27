namespace BlocksWorld
{
    public abstract class Tool
    {
        private readonly Network network;

        protected Tool(Network network)
        {
            this.network = network;
        }

        public Network Network
        {
            get
            {
                return network;
            }
        }

        public abstract void PrimaryUse(FirstPersonCamera cam);

        public virtual void SecondaryUse(FirstPersonCamera cam) { }
    }
}