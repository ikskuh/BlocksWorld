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

			this.Enabled += ButtonBehaviour_Enabled;
			this.Disabled += ButtonBehaviour_Disabled;
		}

		private void ButtonBehaviour_Enabled(object sender, EventArgs e)
		{
			this.interaction = new Interaction("Push Button", Button_Triggered);
			this.Detail.Interactions.Add(this.interaction);
		}

		private void ButtonBehaviour_Disabled(object sender, EventArgs e)
		{
			this.Detail.Interactions.Remove(this.interaction);
		}

		private void Button_Triggered(object sender, ActorEventArgs e)
		{
			this.button.Send();
		}
	}
}