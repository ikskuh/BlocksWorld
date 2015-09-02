using OpenTK;

namespace BlocksWorld
{
	public sealed class RotationBehaviour : Behaviour
	{
		public RotationBehaviour() 
		{
			this.Updated += RotationBehaviour_Updated;
		}

		private void RotationBehaviour_Updated(object sender, FrameEventArgs e)
		{
			this.Detail.Rotation += Vector3.UnitY * (float)e.Time;
        }
	}
}