using OpenTK;
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

			this.Enabled += FlipOverBehaviour_Enabled;
			this.Disabled += FlipOverBehaviour_Disabled;
		}

		private void FlipOverBehaviour_Enabled(object sender, EventArgs e)
		{
			this.interaction = new Interaction("Flip Over", (s, _) => this.FlipTable());
			this.Detail.Interactions.Add(this.interaction);
		}

		private void FlipOverBehaviour_Disabled(object sender, EventArgs e)
		{
			this.Detail.Interactions.Remove(this.interaction);
		}

		private void FlipTable()
		{
			this.Detail.Rotation *= Quaternion.FromAxisAngle(Vector3.UnitZ, MathHelper.Pi);
		}

		public float Rotation { get; set; } = (float)Math.PI;
	}
}