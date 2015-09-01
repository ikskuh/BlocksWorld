using System;

namespace BlocksWorld
{
    public class DetailInteractionEventArgs : ActorEventArgs
    {
        public DetailInteractionEventArgs(DetailObject detail, Interaction interaction, IActor actor) :
			base(actor)
        {
            this.Detail = detail;
            this.Interaction = interaction;
        }

        public DetailObject Detail { get; private set; }
        public Interaction Interaction { get; private set; }
    }
}