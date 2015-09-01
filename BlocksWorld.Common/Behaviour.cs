using OpenTK;
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
		public event EventHandler<FrameEventArgs> Updated;

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
			if (detail == null)
				throw new ArgumentNullException("detail");
			if (this.World == null)
				throw new InvalidOperationException("Invalid behaviour: Not created via World.CreateBehaviour");
			if (this.detail != null)
				throw new InvalidOperationException("Cannot attach an already attached behaviour.");
			this.detail = detail;
			this.OnAttach(this.detail);
		}

		public void Detach()
		{
			this.OnDetach(this.detail);
			this.detail = null;
		}

		/// <summary>
		/// Calls the event Updated. Should be overhauled into a better solution.
		/// </summary>
		/// <param name="deltaTime"></param>
		public void Update(double deltaTime)
		{
			if (deltaTime <= 0.0)
				return;
			if (this.Updated != null)
				this.Updated(this, new FrameEventArgs(deltaTime));
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

		public bool IsAttached { get { return this.detail != null; } }

		public IReadOnlyDictionary<string, Signal> Signals { get { return this.signals; } }

		public IReadOnlyDictionary<string, Slot> Slots { get { return this.slots; } }

		public World World { get; internal set; }
	}
}
