using System;

namespace BlocksWorld
{
    public class DetailInteractionEventArgs : EventArgs
    {
        public DetailInteractionEventArgs(DetailObject detail, Interaction interaction)
        {
            this.Detail = detail;
            this.Interaction = interaction;
        }

        public DetailObject Detail { get; private set; }
        public Interaction Interaction { get; private set; }
    }
}