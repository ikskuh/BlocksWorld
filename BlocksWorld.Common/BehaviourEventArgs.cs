using System;

namespace BlocksWorld
{
	public class BehaviourEventArgs : EventArgs
	{
		private Behaviour behaviour;

		public BehaviourEventArgs(Behaviour behaviour)
		{
			this.behaviour = behaviour;
		}
	}
}