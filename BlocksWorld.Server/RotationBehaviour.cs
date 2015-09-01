using OpenTK;

namespace BlocksWorld
{
	public sealed class RotationBehaviour : Behaviour
	{
		private bool enabled = true;

		public RotationBehaviour() 
		{
			this.Updated += RotationBehaviour_Updated;

			this.CreateSlot("enable", (s, e) => this.enabled = true);
			this.CreateSlot("disable", (s, e) => this.enabled = false);
			this.CreateSlot("toggle", (s, e) => this.enabled ^= true);
		}

		private void RotationBehaviour_Updated(object sender, FrameEventArgs e)
		{
			if (this.enabled == false)
				return;
			this.Detail.Rotation += Vector3.UnitY * (float)e.Time;
        }
	}
}