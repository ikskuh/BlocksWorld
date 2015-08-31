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

		public event EventHandler<DetailEventArgs> Attached;
		public event EventHandler<DetailEventArgs> Detached;
		
		protected Behaviour()
		{

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
	}
}
