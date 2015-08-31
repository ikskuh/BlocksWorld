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
        public event EventHandler<DetailInteractionEventArgs> InterationTriggered;

        private Vector3 position;
        private Vector3 rotation;
        private ObservableCollection<string> interactions = new ObservableCollection<string>();

        public DetailObject(int id)
        {
            this.ID = id;
            this.interactions.CollectionChanged += (s, e) =>
            {
                this.OnInteractonsChanged();
            };
        }

        private void OnInteractonsChanged()
        {
            if (this.InteractionsChanged != null)
                this.InteractionsChanged(this, EventArgs.Empty);
        }

        public void Interact(string interaction)
        {
            if (interaction == null)
                return; // No interaction means nothing will happen anyways
            if (this.Interactions.Contains(interaction) == false)
                throw new ArgumentException(interaction + " does not exist.", "interaction");
            this.OnInteractionTriggered(interaction);
        }

        private void OnInteractionTriggered(string interaction)
        {
            if (this.InterationTriggered != null)
                this.InterationTriggered(this, new DetailInteractionEventArgs(this, interaction));
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

        public IList<string> Interactions
        {
            get
            {
                return this.interactions;
            }
        }
    }
}