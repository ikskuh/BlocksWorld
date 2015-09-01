using System;

namespace BlocksWorld
{
	public sealed class FlipOverBehaviour : Behaviour
	{
		private Interaction interaction;
		private Slot flip;

		public FlipOverBehaviour()
		{
			this.flip = this.CreateSlot("flip", (s, e) => this.FlipTable());

			this.Attached += FlipOverBehaviour_Attached;
			this.Detached += FlipOverBehaviour_Detached;
		}

		private void FlipOverBehaviour_Attached(object sender, DetailEventArgs e)
		{
			this.interaction = new Interaction("Flip Over", (s, _) => this.FlipTable());
			this.Detail.Interactions.Add(this.interaction);
		}

		private void FlipOverBehaviour_Detached(object sender, DetailEventArgs e)
		{
			this.Detail.Interactions.Remove(this.interaction);
		}

		private void FlipTable()
		{

			var rot = this.Detail.Rotation;
			rot.X += this.Rotation;
			this.Detail.Rotation = rot;
		}

		public float Rotation { get; set; } = (float)Math.PI;
	}
}