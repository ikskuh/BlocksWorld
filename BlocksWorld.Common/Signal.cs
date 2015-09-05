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

		public Behaviour Behaviour
		{
			get
			{
				return behaviour;
			}
		}

		public string Name { get { return this.name; } }

		public void Connect(Slot slot)
		{
			if (slot == null)
				throw new ArgumentNullException("slot");
			slot.signals.Add(this);
			this.slots.Add(slot);
		}

		public void Disconnect(Slot slot)
		{
			if (slot == null)
				throw new ArgumentNullException("slot");
			slot.signals.Remove(this);
			this.slots.Remove(slot);
		}

		public void Send()
		{
			foreach (var slot in this.slots)
				slot.ReceiveSignal(this);
		}

		public IEnumerable<Slot> Slots { get { return this.slots; } }
	}
}