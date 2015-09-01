using System;

namespace BlocksWorld
{
	public class ActorEventArgs : EventArgs
	{
		private IActor actor;

		public ActorEventArgs(IActor actor)
		{
			this.actor = actor;
		}

		public IActor Actor
		{
			get
			{
				return actor;
			}
		}
	}
}