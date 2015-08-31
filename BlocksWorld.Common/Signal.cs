using System;
using System.Collections.Generic;

namespace BlocksWorld
{
	public sealed class Signal
	{
		private readonly Behaviour behaviour;
		private readonly string name;
		private readonly HashSet<Slot> slots = new HashSet<Slot>();

		internal Signal(Behaviour behaviour, string name)
		{
			this.behaviour = behaviour;
			this.name = name;
		}

		public string Name { get { return this.name; } }

		public void Connect(Slot slot)
		{
			this.slots.Add(slot);
		}

		public void Disconnect(Slot slot)
		{
			this.slots.Remove(slot);
		}

		public void Send()
		{
			foreach (var slot in this.slots)
				slot.ReceiveSignal(this);
		}
	}
}