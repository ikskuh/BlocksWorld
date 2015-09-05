using Jitter.Collision.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlocksWorld
{
	[Flags]
	public enum WorldSaveMode
	{
		Default = Blocks | Details | SignalConnections,
		Blocks = 0x1,
		Details = 0x2,
		SignalConnections = 0x4
	}

    partial class World
    {
        public void Load(string fileName)
        {
            using (var fs = File.Open(fileName, FileMode.Open, FileAccess.Read))
            {
                this.Load(fs);
            }
        }

        public void Load(Stream fs)
        {
            this.Load(fs, false);
        }

        public void Load(Stream fs, bool leaveOpen)
        {
            // Reset whole world
            foreach (var block in blocks.ToArray())
            {
                this[block.X, block.Y, block.Z] = null;
            }

            Action<bool> assert = (b) => { if (!b) throw new InvalidDataException(); };
            using (var br = new BinaryReader(fs, Encoding.UTF8, leaveOpen))
            {
                assert(br.ReadString() == "BLOCKWORLD");
				int major = br.ReadByte();
				int minor = br.ReadByte();
                assert(major == 1);
                

                int typeCount = br.ReadInt32();
                Type[] types = new Type[typeCount];
                for (int i = 0; i < typeCount; i++)
                {
                    string typeName = br.ReadString();
                    types[i] = Type.GetType(typeName);
                    if (types[i] == null)
                        throw new TypeLoadException("Could not find type " + typeName);
                }

                long blockCount = br.ReadInt64();
                for (long i = 0; i < blockCount; i++)
                {
                    int x = br.ReadInt32();
                    int y = br.ReadInt32();
                    int z = br.ReadInt32();
                    Type type = types[br.ReadInt32()];

                    var block = Activator.CreateInstance(type) as Block;

                    block.Deserialize(br);

                    this[x, y, z] = block;
                }

				// Details are stored here as well
				if(minor == 1)
				{
					int detailCount = br.ReadInt32();
					for (int i = 0; i < detailCount; i++)
					{
						int id = br.ReadInt32();
						string model = br.ReadString();
						int parentID = br.ReadInt32();

						var position = br.ReadVector3();
						var rotation = br.ReadQuaternion();

						Shape shape = null;

						bool hasShape = br.ReadBoolean();
						if (hasShape)
						{
							var size = br.ReadVector3();
							shape = new BoxShape(size.Jitter());
						}
						
						DetailObject detail = new DetailObject(this.GetDetail(parentID), id, shape);

						if (string.IsNullOrWhiteSpace(model) == false)
							detail.Model = model;

						detail.Position = position;
						detail.Rotation = rotation;

						this.RegisterDetail(detail);

						int behaviourCount = br.ReadInt32();
						for (int j = 0; j < behaviourCount; j++)
						{
							var typeName = br.ReadString();
							var type = Type.GetType(typeName, true);

							int behaviourID = br.ReadInt32();
							bool isEnabled = br.ReadBoolean();

							var behaviour = (Behaviour)Activator.CreateInstance(type);

							detail.CreateBehaviour(behaviour, behaviourID, isEnabled);
						}
					}



					int signalCount = br.ReadInt32();
					for (int j = 0; j < signalCount; j++)
					{
						var signalDetailID = br.ReadInt32();
						var signalBehaviourID = br.ReadInt32();
						var signalName = br.ReadString();

						var signal = this.GetDetail(signalDetailID).Behaviours[signalBehaviourID].Signals[signalName];
						
						var slotCount = br.ReadInt32();
						for (int k = 0; k < slotCount; k++)
						{
							var slotDetailID = br.ReadInt32();
							var slotBehaviourID = br.ReadInt32();
							var slotName = br.ReadString();

							var slot = this.GetDetail(slotDetailID).Behaviours[slotBehaviourID].Slots[slotName];

							signal.Connect(slot);
						}
					}
				}
            }
		}

		public void Save(string fileName, WorldSaveMode mode)
		{
			using (var fs = File.Open(fileName, FileMode.Create, FileAccess.Write))
			{
				this.Save(fs, mode);
			}
		}

		public void Save(Stream fs, WorldSaveMode mode)
        {
            this.Save(fs, mode, false);
        }

        public void Save(Stream fs, WorldSaveMode mode, bool leaveOpen)
        {
			if (mode.HasFlag(WorldSaveMode.SignalConnections) && !mode.HasFlag(WorldSaveMode.Details))
				throw new ArgumentException("WorldSaveMode.SignalConnections requires WorldSaveMode.Details to be set as well.");

            var blocks = this.blocks.Where(x => (x.Value != null) && (x.Value.Block != null)).ToArray();
            using (var bw = new BinaryWriter(fs, Encoding.UTF8, leaveOpen))
            {
                bw.Write("BLOCKWORLD");
                bw.Write((byte)1);
                bw.Write((byte)1);

				if (mode.HasFlag(WorldSaveMode.Blocks))
				{
					var types = blocks.Select(x => x.Value.Block.GetType()).Distinct().ToList();

					bw.Write(types.Count);
					for (int i = 0; i < types.Count; i++)
					{
						bw.Write(types[i].AssemblyQualifiedName);
					}

					bw.Write(blocks.LongLength);

					foreach (var data in blocks)
					{
						var block = data.Value.Block;

						bw.Write(data.X);
						bw.Write(data.Y);
						bw.Write(data.Z);
						bw.Write(types.IndexOf(block.GetType()));

						block.Serialize(bw);
					}
				}
				else
				{
					bw.Write((int)0);
					bw.Write((long)0);
				}

				if (mode.HasFlag(WorldSaveMode.Details))
				{
					// Sort by ID: Parent.ID must always be smaller than childs ID,
					// so we create the parents before their children
					var details = this.details.OrderBy(d => d.ID).ToArray();

					bw.Write(details.Length);
					for (int i = 0; i < details.Length; i++)
					{
						var detail = details[i];

						bw.Write(detail.ID);
						bw.Write(detail.Model ?? "");
						if (detail.Parent != null)
							bw.Write(detail.Parent.ID);
						else
							bw.Write((Int32)(-1));
						bw.Write(detail.Position);
						bw.Write(detail.Rotation);

						bool hasShape = (detail.Shape != null);
						bw.Write(hasShape);
						if (hasShape)
						{
							var shape = (BoxShape)detail.Shape;
							bw.Write(shape.Size.TK());
						}

						var behaviours = detail.Behaviours.Values.ToArray();
						bw.Write(behaviours.Length);
						for (int j = 0; j < behaviours.Length; j++)
						{
							var behaviour = behaviours[j];

							string type = behaviour.GetType().AssemblyQualifiedName;

							bw.Write(type);
							bw.Write(behaviour.ID);
							bw.Write(behaviour.IsEnabled);
						}
					}

					if (mode.HasFlag(WorldSaveMode.Details))
					{
						var signals = details
						.SelectMany(d => d.Behaviours.Values)
						.SelectMany(b => b.Signals.Values)
						.Where(s => (s.Slots.Count() > 0))
						.ToArray();

						bw.Write(signals.Length);
						for (int j = 0; j < signals.Length; j++)
						{
							var signal = signals[j];

							bw.Write(signal.Behaviour.Detail.ID);
							bw.Write(signal.Behaviour.ID);
							bw.Write(signal.Name);

							var slots = signal.Slots.ToArray();
							bw.Write(slots.Length);
							for (int k = 0; k < slots.Length; k++)
							{
								var slot = slots[k];
								bw.Write(slot.Behaviour.Detail.ID);
								bw.Write(slot.Behaviour.ID);
								bw.Write(slot.Name);
							}
						}
					}
					else
					{
						bw.Write((int)0); // 0 signal connections
					}
				}
				else
				{
					bw.Write((int)0); // 0 details
					bw.Write((int)0); // 0 signal connections
				}

                bw.Flush();
            }
        }
    }
}
