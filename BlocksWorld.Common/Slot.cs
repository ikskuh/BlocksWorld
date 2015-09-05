using System;
using System.Collections.Generic;

namespace BlocksWorld
{
	public sealed class Slot
	{
		private readonly Behaviour behaviour;
		private readonly string name;
		internal readonly HashSet<Signal> signals = new HashSet<Signal>();

		public event EventHandler SignalRecevied;

		internal Slot(Behaviour behaviour, string name)
		{
			this.behaviour = behaviour;
			this.name = name;
		}

		internal void ReceiveSignal(Signal sender)
		{
			if (this.SignalRecevied != null)
				this.SignalRecevied(this, EventArgs.Empty);
		}

		public void ConnectTo(Signal signal)
		{
			signal.Connect(this);
		}

		public void DisconnectFrom(Signal signal)
		{
			signal.Disconnect(this);
		}

		public string Name { get { return this.name; } }

		public Behaviour Behaviour
		{
			get
			{
				return behaviour;
			}
		}

		public IEnumerable<Signal> Signals
		{
			get
			{
				return signals;
			}
		}
	}
}