using System;
using OpenTK;

namespace BlocksWorld
{
    internal class Proxy : IUpdateable
    {
        public Vector3 CurrentPosition { get; private set; }
        public Vector3 SetpointPosition { get; private set; }

        public float CurrentBodyRotation { get; private set; }
        public float SetpointBodyRotation { get; private set; }

        public Proxy(Vector3 position, float bodyRotation)
        {
            this.SetpointPosition = this.CurrentPosition = position;
            this.SetpointBodyRotation = this.CurrentBodyRotation = bodyRotation;
        }

        public void UpdateFrame(IGameInputDriver input, double time)
        {
            // TODO: Implement nicer interpolation
            this.CurrentPosition = Vector3.Lerp(this.CurrentPosition, this.SetpointPosition, 0.2f);
            this.CurrentBodyRotation = 0.8f * this.CurrentBodyRotation + 0.2f * this.SetpointBodyRotation;
        }

        internal void Set(Vector3 pt, float rot)
        {
            this.SetpointPosition = pt;
            this.SetpointBodyRotation = rot;
        }
    }
}