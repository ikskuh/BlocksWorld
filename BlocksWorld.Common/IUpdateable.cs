namespace BlocksWorld
{
    public interface IUpdateable
    {
        void UpdateFrame(IGameInputDriver input, double time);
    }
}