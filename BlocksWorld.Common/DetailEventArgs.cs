using System;

namespace BlocksWorld
{
    public class DetailEventArgs : EventArgs
    {
        public DetailEventArgs(DetailObject obj)
        {
            this.Detail = obj;
        }

        public DetailObject Detail { get; private set; }
    }
}