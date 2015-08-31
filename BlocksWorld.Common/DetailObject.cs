using OpenTK;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BlocksWorld
{
    public class DetailObject
    {
        public event EventHandler Changed;
        public event EventHandler InteractionsChanged;
		public event EventHandler<DetailInteractionEventArgs> InteractionTriggered;

		private Vector3 position;
        private Vector3 rotation;
        private ObservableCollection<Interaction> interactions = new ObservableCollection<Interaction>();

        public DetailObject(int id)
        {
            this.ID = id;
            this.interactions.CollectionChanged += (s, e) =>
            {
				if(e.NewItems != null)
				{
					foreach(Interaction item in e.NewItems)
						item.Triggered += Item_Triggered;
				}
				if (e.OldItems != null)
				{
					foreach (Interaction item in e.OldItems)
						item.Triggered -= Item_Triggered;
				}
				this.OnInteractonsChanged();
            };
        }

		private void Item_Triggered(object sender, EventArgs e)
		{
			this.OnInteractionTriggered(sender as Interaction);
		}

		private void OnInteractionTriggered(Interaction interaction)
		{
			if (interaction == null)
				return;
			if (this.InteractionTriggered != null)
				this.InteractionTriggered(this, new DetailInteractionEventArgs(this, interaction));
        }

		private void OnInteractonsChanged()
        {
            if (this.InteractionsChanged != null)
                this.InteractionsChanged(this, EventArgs.Empty);
        }

        public int ID { get; private set; }
        public string Model { get; set; }

        public Vector3 Rotation
        {
            get
            {
                return rotation;
            }

            set
            {
                bool changed = (value - rotation).Length > 0.02f;
				rotation = value;
                if (changed)
                    this.OnChanged();
            }
        }

        private void OnChanged()
        {
            if (this.Changed != null)
                this.Changed(this, EventArgs.Empty);
        }

        public Vector3 Position
        {
            get
            {
                return position;
            }

            set
            {
                bool changed = (value - position).Length > 0.02f;
                position = value;
                if (changed)
                    this.OnChanged();
            }
        }

        public IList<Interaction> Interactions
        {
            get
            {
                return this.interactions;
            }
        }
	}
}