using System;

namespace BlocksWorld
{
	internal class ButtonBehaviour : Behaviour
	{
		private Interaction interaction;
		private Signal button;

		public ButtonBehaviour()
		{
			this.button = this.CreateSignal("clicked");

			this.Attached += ButtonBehaviour_Attached;
			this.Detached += ButtonBehaviour_Detached;
		}

		private void ButtonBehaviour_Attached(object sender, DetailEventArgs e)
		{
			this.interaction = new Interaction("Push Button", Button_Triggered);
			this.Detail.Interactions.Add(this.interaction);
		}

		private void ButtonBehaviour_Detached(object sender, DetailEventArgs e)
		{
			this.Detail.Interactions.Remove(this.interaction);
		}

		private void Button_Triggered(object sender, EventArgs e)
		{
			this.button.Send();
		}
	}
}