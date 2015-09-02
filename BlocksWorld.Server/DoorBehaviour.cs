using OpenTK;
using System;

namespace BlocksWorld
{
	public sealed class DoorBehaviour : Behaviour
	{
		private bool isOpen = false;
		private Interaction interaction;
		private Signal sigOpened;
		private float? initialRotation = null;

		public DoorBehaviour()
		{
			this.sigOpened = this.CreateSignal("opened");

			this.Enabled += DoorBehaviour_Enabled;
			this.Disabled += DoorBehaviour_Disabled;
			this.Updated += DoorBehaviour_Updated;
		}

		private void DoorBehaviour_Updated(object sender, OpenTK.FrameEventArgs e)
		{
			float dt = (float)e.Time;
			this.initialRotation = this.initialRotation ?? this.Detail.Rotation.Y;

			float targetRotation = this.initialRotation.Value + (this.isOpen ? 0.5f * (float)Math.PI : 0.0f);
			float currentRotation = this.Detail.Rotation.Y;

			float delta = targetRotation - currentRotation;

			currentRotation += Math.Sign(delta) * Math.Min(Math.Abs(delta), dt);
			
			this.Detail.Rotation = new Vector3(
				0,
				currentRotation,
				0);
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

		private void Door_OpenClose(object sender, ActorEventArgs e)
		{
			this.isOpen = !this.isOpen;
        }
	}
}