using System;

namespace BlocksWorld
{
	public sealed class Interaction
	{
		private static int counter = 0;

		private readonly string name;
		private readonly int id;

		public event EventHandler Triggered;

		public Interaction(string name) : 
			this(Interaction.counter++, name) // TODO: Is this really such a good idea?
		{

		}

		public Interaction(string name, EventHandler triggered) :
			this(name)
		{
			if (triggered != null)
				this.Triggered += triggered;
		}

		public Interaction(int id, string name)
		{
			this.id = id;

			if (name == null) throw new ArgumentNullException("name");
			this.name = name;
		}

		public void Trigger()
		{
			if (this.Triggered != null)
				this.Triggered(this, EventArgs.Empty);
		}

		public string Name { get { return this.name; } }

		public int ID { get { return this.id; } }
	}
}