using System;

namespace BlocksWorld
{
    public abstract class Scene : IUpdateable, IDisposable
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

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        ~Scene()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}