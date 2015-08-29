using System;

namespace BlocksWorld
{
    public class DetailInteractionEventArgs : EventArgs
    {
        public DetailInteractionEventArgs(DetailObject detail, string interaction)
        {
            this.Detail = detail;
            this.Interaction = interaction;
        }

        public DetailObject Detail { get; private set; }
        public string Interaction { get; private set; }
    }
}