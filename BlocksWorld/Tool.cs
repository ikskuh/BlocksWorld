namespace BlocksWorld
{
    public abstract class Tool
    {
        public abstract void PrimaryUse(FirstPersonCamera cam);

        public virtual void SecondaryUse(FirstPersonCamera cam) { }
    }
}