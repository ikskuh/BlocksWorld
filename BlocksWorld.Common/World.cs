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
			if (this.HasDetail(obj.ID))
				throw new InvalidOperationException();

			obj.Changed += Obj_Changed;
			obj.InteractionTriggered += Obj_InterationTriggered;
			this.details.Add(obj);

			if (obj.Shape != null)
			{
				var body = new RigidBody(obj.Shape);
				body.Position = obj.WorldPosition.Jitter();
				body.Orientation = JMatrix.CreateFromQuaternion(obj.WorldRotation.Jitter());
				body.IsStatic = true;
				body.Tag = obj;
				body.EnableDebugDraw = true;
				body.Material = this.blockMaterial;
				this.detailBodies.Add(obj.ID, body);

				this.AddBody(body);

				obj.PositionChanged += (s, e) =>
				{
					body.Position = obj.WorldPosition.Jitter();
					body.Orientation = JMatrix.CreateFromQuaternion(obj.WorldRotation.Jitter());
				};
			}

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

		private DetailObject CreateDetailInstance(
			Dictionary<DetailTemplate, DetailObject> mapping,
			Dictionary<BehaviourTemplate, Behaviour> behaviours,
			DetailTemplate template,
			DetailObject parent)
		{
			Vector3 pos = Vector3.Zero;
			if (template.Position != null)
				pos = template.GetPosition();
			Shape shape = null;
			if(template.Shape != null)
			{
				shape = DetailHelper.CreateShape(template.Shape.GetSize());
            }

			DetailObject root = this.CreateDetail(template.Model, pos, parent, shape);
			if (template.Rotation != null)
				root.Rotation = template.GetRotation();

			if (template.Children != null)
			{
				foreach (var child in template.Children)
				{
					CreateDetailInstance(mapping, behaviours, child, root);
				}
			}
			mapping.Add(template, root);

			if (template.Behaviours != null)
			{
				foreach (var behav in template.Behaviours)
				{
					var type = Type.GetType(behav.Class);
					if (type.IsSubclassOf(typeof(Behaviour)) == false)
						throw new InvalidOperationException("Invalid behaviour type: " + behav.Class);
					Behaviour behaviour = root.CreateBehaviour(type, true);
					behaviours.Add(behav, behaviour);
				}
			}

			return root;
		}

		private List<DetailTemplate> Flatten(DetailTemplate t, List<DetailTemplate> list = null)
		{
			list = list ?? new List<DetailTemplate>();
			list.Add(t);

			if (t.Children != null)
			{
				foreach (var child in t.Children)
				{
					Flatten(child, list);
				}
			}
			return list;
		}

		public DetailObject CreateDetail(DetailTemplate template, Vector3 position)
		{
			var mapping = new Dictionary<DetailTemplate, DetailObject>();
			var behaviours = new Dictionary<BehaviourTemplate, Behaviour>();
			var root = this.CreateDetailInstance(
				mapping,
				behaviours,
				template,
				null);

			// Connect behaviours
			{
				var namedBehaviours = new Dictionary<string, Behaviour>();
				foreach (var kv in behaviours)
				{
					if (kv.Key.ID != null)
						namedBehaviours.Add(kv.Key.ID, kv.Value);
				}
				foreach (var kv in behaviours)
				{
					if (kv.Key.SlotConnections != null)
					{
						foreach (var con in kv.Key.SlotConnections)
						{
							var source = namedBehaviours[con.Source];
							var signal = source.Signals[con.Signal];
							var slot = kv.Value.Slots[con.Slot];

							slot.ConnectTo(signal);
						}
					}
				}
			}

			root.Position = position;

			return root;
		}

		public DetailObject CreateDetail(string model, Vector3 position, DetailObject parent = null, Shape shape = null)
		{
			var obj = new DetailObject(parent, ++this.detailCounter, shape)
			{
				Model = model,
				Position = position,
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

			// Destroy all child objects in the object tree as well
			foreach (var child in GetChildren(obj))
			{
				this.RemoveDetail(child.ID);
			}
		}

		private List<DetailObject> GetChildren(DetailObject obj)
		{
			List<DetailObject> children = new List<DetailObject>();
			foreach (var detail in this.details)
			{
				if (detail.Parent == obj)
					children.Add(detail);
			}
			return children;
		}

		/// <summary>
		/// Gets the detail with the given id.
		/// </summary>
		/// <param name="id">The id of the detail to find.</param>
		/// <returns>DetailObject with the id or null if id was -1</returns>
		public DetailObject GetDetail(int id)
		{
			if (id == -1)
				return null;
			return this.details.First(d => d.ID == id);
		}

		public bool HasDetail(int id)
		{
			return this.details.Any(d => d.ID == id);
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