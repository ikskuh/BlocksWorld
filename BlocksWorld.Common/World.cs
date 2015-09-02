using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using System;
using OpenTK;
using Jitter.Collision;
using System.Collections.Generic;
using System.Linq;

namespace BlocksWorld
{
	public partial class World : Jitter.World
	{
		class Atom
		{
			public Block Block { get; set; }

			public RigidBody Body { get; set; }
		}

		private int detailCounter = 0;
		private SparseArray3D<Atom> blocks = new SparseArray3D<Atom>();
		private List<DetailObject> details = new List<DetailObject>();
		private Dictionary<int, RigidBody> detailBodies = new Dictionary<int, RigidBody>();

		private Material blockMaterial = new Material()
		{
			Restitution = 0.0f,
			KineticFriction = 0.0f,
			StaticFriction = 0.1f
		};

		public event EventHandler<BlockEventArgs> BlockChanged;

		public event EventHandler<DetailEventArgs> DetailCreated;
		public event EventHandler<DetailEventArgs> DetailChanged;
		public event EventHandler<DetailEventArgs> DetailRemoved;

		public event EventHandler<DetailInteractionEventArgs> DetailInterationTriggered;

		public World() :
			base(new CollisionSystemPersistentSAP())
		{

		}

		protected void OnBlockChanged(Block block, int x, int y, int z)
		{
			if (this.BlockChanged != null)
				this.BlockChanged(this, new BlockEventArgs(block, x, y, z));
		}

		public void RegisterDetail(DetailObject obj)
		{
			if (this.GetDetail(obj.ID) != null)
				throw new InvalidOperationException();

			obj.Changed += Obj_Changed;
			obj.InteractionTriggered += Obj_InterationTriggered;
			this.details.Add(obj);

			var body = new RigidBody(new BoxShape(1.0f, 0.2f, 1.5f));
			body.Position = obj.Position.Jitter();
			body.Orientation =
				JMatrix.CreateRotationX(obj.Rotation.X) *
				JMatrix.CreateRotationY(obj.Rotation.Y) *
				JMatrix.CreateRotationZ(obj.Rotation.Z);
			body.IsStatic = true;
			body.Tag = obj;
			body.Material = this.blockMaterial;
			this.detailBodies.Add(obj.ID, body);

			this.AddBody(body);

			obj.Changed += (s, e) =>
			{
				body.Position = obj.Position.Jitter();
				body.Orientation =
					JMatrix.CreateRotationX(obj.Rotation.X) *
					JMatrix.CreateRotationY(obj.Rotation.Y) *
					JMatrix.CreateRotationZ(obj.Rotation.Z);
			};

			this.OnDetailCreated(obj);
		}

		private void Obj_InterationTriggered(object sender, DetailInteractionEventArgs e)
		{
			this.OnDetailInterationTriggered(e);
		}

		private void OnDetailInterationTriggered(DetailInteractionEventArgs e)
		{
			if (this.DetailInterationTriggered != null)
				this.DetailInterationTriggered(this, e);
		}

		private void Obj_Changed(object sender, EventArgs e)
		{
			this.OnDetailChanged(sender as DetailObject);
		}

		public DetailObject CreateDetail(string model, Vector3 position)
		{
			var obj = new DetailObject(++this.detailCounter)
			{
				Model = model,
				Position = position
			};

			this.RegisterDetail(obj);

			return obj;
		}

		public void RemoveDetail(int id)
		{
			var detail = this.GetDetail(id);
			if (detail == null)
				return;
			detail.Changed -= Obj_Changed;
			detail.InteractionTriggered -= Obj_InterationTriggered;
			if (this.detailBodies.ContainsKey(id))
			{
				this.RemoveBody(this.detailBodies[id]);
			}
			this.detailBodies.Remove(id);
			this.details.RemoveAll(d => d.ID == id);

			this.OnDetailRemoved(detail);
		}

		private void OnDetailCreated(DetailObject obj)
		{
			if (this.DetailCreated != null)
				this.DetailCreated(this, new DetailEventArgs(obj));
		}

		private void OnDetailChanged(DetailObject obj)
		{
			if (this.DetailChanged != null)
				this.DetailChanged(this, new DetailEventArgs(obj));
		}

		private void OnDetailRemoved(DetailObject obj)
		{
			if (this.DetailRemoved != null)
				this.DetailRemoved(this, new DetailEventArgs(obj));
		}

		public DetailObject GetDetail(int id)
		{
			return this.details.FirstOrDefault(d => d.ID == id);
		}

		public Block this[int x, int y, int z]
		{
			get
			{
				return this.blocks[x, y, z]?.Block;
			}
			set
			{
				this.blocks[x, y, z] = this.blocks[x, y, z] ?? new Atom();
				var prev = this.blocks[x, y, z].Block;
				if (prev == value) // No change at all
					return;

				if (this.blocks[x, y, z].Body != null)
				{
					this.RemoveBody(this.blocks[x, y, z].Body);
					this.blocks[x, y, z].Body = null;
				}

				this.blocks[x, y, z].Block = value;

				if (value != null)
				{
					RigidBody rb = new RigidBody(new BoxShape(1.0f, 1.0f, 1.0f));
					rb.Position = new JVector(x, y, z);
					rb.IsStatic = true;
					this.AddBody(rb);
					this.blocks[x, y, z].Body = rb;
				}

				this.OnBlockChanged(value, x, y, z);
			}
		}

		public TraceResult Trace(JVector origin, JVector direction, TraceOptions options)
		{
			if (direction.Length() < 0.1f)
				return null;

			RaycastCallback callback = (b, n, f) =>
			{
				if ((b.IsStatic == true) && (options.HasFlag(TraceOptions.IgnoreStatic)))
					return false;
				if ((b.IsStatic == false) && (options.HasFlag(TraceOptions.IgnoreDynamic)))
					return false;
				return true;
			};
			RigidBody body;
			JVector normal;
			float friction;

			if (this.CollisionSystem.Raycast(
				origin,
				direction,
				callback,
				out body,
				out normal,
				out friction))
			{
				return new TraceResult(body, friction, origin + friction * direction, normal);
			}
			return null;
		}

		public int LowerX { get { return this.blocks.GetLowerX(); } }
		public int LowerY { get { return this.blocks.GetLowerY(); } }
		public int LowerZ { get { return this.blocks.GetLowerZ(); } }

		public int UpperX { get { return this.blocks.GetUpperX(); } }
		public int UpperY { get { return this.blocks.GetUpperY(); } }
		public int UpperZ { get { return this.blocks.GetUpperZ(); } }


		public IReadOnlyList<DetailObject> Details { get { return this.details; } }
	}
}