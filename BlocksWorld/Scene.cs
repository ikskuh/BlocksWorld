using System;

namespace BlocksWorld
{
    public abstract class Scene : IUpdateable
    {
        public bool IsEnabled { get; private set; }
        public bool IsLoaded { get; private set; }

        public virtual void RenderFrame(double time)
        {

        }

        public virtual void UpdateFrame(IGameInputDriver input, double time)
        {

        }

        public void Enable()
        {
            this.IsEnabled = true;
            this.OnEnable();
        }

        public void Disable()
        {
            this.IsEnabled = false;
            this.OnDisable();
        }

        public void Load()
        {
            this.OnLoad();
            this.IsLoaded = true;
        }

        protected virtual void OnEnable() { }

        protected virtual void OnDisable() { }

        protected virtual void OnLoad() { }
    }
}