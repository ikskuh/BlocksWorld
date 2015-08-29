namespace BlocksWorld
{
    public class DetailEventArgs
    {
        public DetailEventArgs(DetailObject obj)
        {
            this.Detail = obj;
        }

        public DetailObject Detail { get; private set; }
    }
}