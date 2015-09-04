using Jitter.Collision.Shapes;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BlocksWorld
{
	public class DetailObject
	{
		public event EventHandler Changed;
		public event EventHandler PositionChanged;
		public event EventHandler InteractionsChanged;
		public event EventHandler<DetailInteractionEventArgs> InteractionTriggered;

		public event EventHandler<BehaviourEventArgs> BehaviourCreated;
		public event EventHandler<BehaviourEventArgs> BehaviourChanged;
		public event EventHandler<BehaviourEventArgs> BehaviourDestroyed;

		private int lastBehaviourID = 0;
		private Vector3 position;
		private Quaternion rotation = Quaternion.Identity;
		private ObservableCollection<Interaction> interactions = new ObservableCollection<Interaction>();

		private Dictionary<int, Behaviour> behaviours = new Dictionary<int, Behaviour>();

		private readonly DetailObject parent;
		private readonly Shape shape;

		public DetailObject(DetailObject parent, int id, Shape shape)
		{
			this.parent = parent;
			this.shape = shape;
			this.ID = id;
			this.interactions.CollectionChanged += (s, e) =>
			{
				if (e.NewItems != null)
				{
					foreach (Interaction item in e.NewItems)
						item.Triggered += Item_Triggered;
				}
				if (e.OldItems != null)
				{
					foreach (Interaction item in e.OldItems)
						item.Triggered -= Item_Triggered;
				}
				this.OnInteractonsChanged();
			};
			if(this.parent != null)
			{
				this.parent.PositionChanged += (s, e) => this.OnPositionChanged();
			}
		}

		public void Update(double deltaTime)
		{
			foreach (var behaviour in this.behaviours.Values)
			{
				behaviour.Update(deltaTime);
			}
		}

		public T CreateBehaviour<T>()
			where T : Behaviour, new()
		{
			return this.CreateBehaviour<T>(true);
		}

		public T CreateBehaviour<T>(bool enabled)
			where T : Behaviour, new()
		{
			var behaviour = new T();
			return CreateBehaviour(behaviour, enabled) as T;
		}

		public Behaviour CreateBehaviour(Type type)
		{
			return this.CreateBehaviour(type, true);
		}

		public Behaviour CreateBehaviour(Type type, bool enabled)
		{
			var behaviour = Activator.CreateInstance(type) as Behaviour;
			if (behaviour == null)
				throw new InvalidOperationException(type.Name + " is not a Behaviour");
			return CreateBehaviour(behaviour, enabled);
		}

		private Behaviour CreateBehaviour(Behaviour behaviour, bool enabled)
		{
			behaviour.detail = this;
			behaviour.ID = this.lastBehaviourID++;
			this.behaviours.Add(behaviour.ID, behaviour);
			behaviour.IsEnabled = enabled;
			this.OnBehaviourCreated(behaviour);
			return behaviour;
		}

		public void DestroyBehaviour(Behaviour behaviour)
		{
			if (behaviour == null)
				throw new ArgumentNullException(nameof(behaviour));
			if (this.behaviours.ContainsKey(behaviour.ID) == false)
				throw new InvalidOperationException("The behaviour was not created by this world.");
			behaviour.IsEnabled = false;
			this.behaviours.Remove(behaviour.ID);
		}

		private void OnBehaviourCreated(Behaviour behaviour)
		{
			if (this.BehaviourCreated != null)
				this.BehaviourCreated(this, new BehaviourEventArgs(behaviour));
		}

		private void OnBehaviourChanged(Behaviour behaviour)
		{
			if (this.BehaviourChanged != null)
				this.BehaviourChanged(this, new BehaviourEventArgs(behaviour));
		}

		private void OnBehaviourDestroyed(Behaviour behaviour)
		{
			if (this.BehaviourDestroyed != null)
				this.BehaviourDestroyed(this, new BehaviourEventArgs(behaviour));
		}

		private void Item_Triggered(object sender, ActorEventArgs e)
		{
			this.OnInteractionTriggered(sender as Interaction, e.Actor);
		}

		private void OnInteractionTriggered(Interaction interaction, IActor actor)
		{
			if (interaction == null)
				return;
			if (this.InteractionTriggered != null)
				this.InteractionTriggered(this, new DetailInteractionEventArgs(this, interaction, actor));
		}

		private void OnInteractonsChanged()
		{
			if (this.InteractionsChanged != null)
				this.InteractionsChanged(this, EventArgs.Empty);
		}

		public int ID { get; private set; }
		public string Model { get; set; }

		public Quaternion Rotation
		{
			get
			{
				return this.rotation;
			}

			set
			{
				bool changed = (value != rotation);
				rotation = value;
				if (changed)
				{
					this.OnChanged();
					this.OnPositionChanged();
				}
			}
		}

		private void OnChanged()
		{
			if (this.Changed != null)
				this.Changed(this, EventArgs.Empty);
		}

		private void OnPositionChanged()
		{
			if (this.PositionChanged != null)
				this.PositionChanged(this, EventArgs.Empty);
		}

		public Vector3 Position
		{
			get
			{
				return position;
			}

			set
			{
				bool changed = (value != position);
				position = value;
				if (changed)
				{
					this.OnChanged();
					this.OnPositionChanged();
				}
			}
		}

		public Vector3 WorldPosition
		{
			get
			{
				return Vector3.Transform(Vector3.Zero, this.Transform);
			}
		}

		public Quaternion WorldRotation
		{
			get
			{
				if (this.parent != null)
					return this.parent.WorldRotation * this.rotation;
				else
					return this.rotation;
			}
		}

		public Matrix4 LocalTransform
		{
			get
			{
				return
					Matrix4.CreateFromQuaternion(this.rotation) *
					Matrix4.CreateTranslation(this.position);
			}
		}

		public Matrix4 Transform
		{
			get
			{
				if (this.parent != null)
					return this.LocalTransform * this.parent.Transform;
				else
					return this.LocalTransform;
			}
		}

		public IList<Interaction> Interactions
		{
			get
			{
				return this.interactions;
			}
		}

		public DetailObject Parent { get { return this.parent; } }

		public Shape Shape { get { return this.shape; } }
	}
}