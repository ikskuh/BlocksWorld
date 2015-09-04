using OpenTK;
using System;

namespace BlocksWorld
{
	public sealed class DoorBehaviour : Behaviour
	{
		private bool isOpen = false;
		private Interaction interaction;
		private Signal sigOpened;
		private Quaternion? initialRotation = null;
		private float fOpen = 0.0f;

		public DoorBehaviour()
		{
			this.sigOpened = this.CreateSignal("opened");

			this.CreateSlot("open-close", this.Door_OpenClose);

			this.Enabled += DoorBehaviour_Enabled;
			this.Disabled += DoorBehaviour_Disabled;
			this.Updated += DoorBehaviour_Updated;
		}

		private void DoorBehaviour_Updated(object sender, OpenTK.FrameEventArgs e)
		{
			float dt = (float)e.Time;
			this.initialRotation = this.initialRotation ?? this.Detail.Rotation;

			float targetRotation = (this.isOpen ? 1.0f : 0.0f);

			float delta = targetRotation - this.fOpen;

			this.fOpen += Math.Sign(delta) * Math.Min(Math.Abs(delta), dt);

			this.Detail.Rotation = this.initialRotation.Value * Quaternion.FromAxisAngle(Vector3.UnitY, 0.7f * MathHelper.Pi * this.fOpen);
		}

		private void DoorBehaviour_Enabled(object sender, EventArgs e)
		{
			this.interaction = new Interaction("Open/Close", Door_OpenClose);
			this.Detail.Interactions.Add(this.interaction);
		}

		private void DoorBehaviour_Disabled(object sender, EventArgs e)
		{
			this.Detail.Interactions.Remove(this.interaction);
		}

		private void Door_OpenClose(object sender, EventArgs e)
		{
			this.isOpen = !this.isOpen;
        }
	}
}