using OpenTK;
using System;

namespace BlocksWorld
{
    public class DetailObject
    {
        public event EventHandler Changed;

        private Vector3 position;
        private float rotation;

        public DetailObject(int id)
        {
            this.ID = id;
        }

        public int ID { get; private set; }
        public string Model { get; set; }

        public float Rotation
        {
            get
            {
                return rotation;
            }

            set
            {
                bool changed = Math.Abs(this.rotation - value) > 0.02f;
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
    }
}