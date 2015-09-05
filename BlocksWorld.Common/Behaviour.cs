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
		internal DetailObject detail;
		private bool isEnabled = false;
		private Dictionary<string, Signal> signals = new Dictionary<string, Signal>();
		private Dictionary<string, Slot> slots = new Dictionary<string, Slot>();

		public event EventHandler Enabled;
		public event EventHandler Disabled;
		public event EventHandler<FrameEventArgs> Updated;

		protected Behaviour()
		{
			this.CreateSlot("enable", (s, e) => this.IsEnabled = true);
			this.CreateSlot("disable", (s, e) => this.IsEnabled = false);
			this.CreateSlot("toggle", (s, e) => this.IsEnabled ^= true);
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

		/// <summary>
		/// Calls the event Updated. Should be overhauled into a better solution.
		/// </summary>
		/// <param name="deltaTime"></param>
		public void Update(double deltaTime)
		{
			if (deltaTime <= 0.0)
				return;
			if (this.IsEnabled == false)
				return;
			if (this.Updated != null)
				this.Updated(this, new FrameEventArgs(deltaTime));
		}

		private void OnEnabled()
		{
			if (this.Enabled != null)
				this.Enabled(this, EventArgs.Empty);
		}

		private void OnDisabled()
		{
			if (this.Disabled != null)
				this.Disabled(this, EventArgs.Empty);
		}

		public DetailObject Detail { get { return this.detail; } }

		public IReadOnlyDictionary<string, Signal> Signals { get { return this.signals; } }

		public IReadOnlyDictionary<string, Slot> Slots { get { return this.slots; } }

		public World World { get; internal set; }
		public int ID { get; internal set; }

		public bool IsEnabled
		{
			get { return this.isEnabled; }
			set
			{
				if (this.isEnabled == value)
					return;
				this.isEnabled = value;
				if (this.isEnabled)
					this.OnEnabled();
				else
					this.OnDisabled();
			}
		}
	}
}
