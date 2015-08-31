using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlocksWorld
{
	public abstract class Behaviour
	{
		private DetailObject detail;
		private Dictionary<string, Signal> signals = new Dictionary<string, Signal>();
		private Dictionary<string, Slot> slots = new Dictionary<string, Slot>();

		public event EventHandler<DetailEventArgs> Attached;
		public event EventHandler<DetailEventArgs> Detached;
		
		protected Behaviour()
		{

        }

		protected Signal CreateSignal(string name)
		{
			var signal = new Signal(this, name);
			this.signals.Add(signal.Name, signal);
			return signal;
		}

		protected Slot CreateSlot(string name)
		{
			var slot = new Slot(this, name);
			this.slots.Add(slot.Name, slot);
			return slot;
		}

		protected Slot CreateSlot(string name, EventHandler handler)
		{
			var slot = this.CreateSlot(name);
			slot.SignalRecevied += handler;
			return slot;
		}

		public void Attach(DetailObject detail)
		{
			if (detail == null) throw new ArgumentNullException("detail");
			this.detail = detail;
			this.OnAttach(this.detail);
		}

		public void Detach()
		{
			this.OnDetach(this.detail);
			this.detail = null;
		}

		protected virtual void OnAttach(DetailObject detail)
		{
			if (this.Attached != null)
				this.Attached(this, new DetailEventArgs(detail));
		}

		protected virtual void OnDetach(DetailObject detail)
		{
			if (this.Detached != null)
				this.Detached(this, new DetailEventArgs(detail));
		}

		public DetailObject Detail { get { return this.detail; } }

		public IReadOnlyDictionary<string, Signal> Signals { get { return this.signals; } }

		public IReadOnlyDictionary<string, Slot> Slots { get { return this.slots; } }
	}
}
