namespace BlocksWorld
{
    public interface IInteractiveEnvironment
    {
        Network Network { get; }

        World World { get; }
    }
}