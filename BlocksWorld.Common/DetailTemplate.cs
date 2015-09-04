using OpenTK;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BlocksWorld
{
	[XmlRoot("detail")]
	public class DetailTemplate
	{
		public Vector3 GetPosition()
		{
			if (this.Position == null)
				throw new InvalidOperationException("Template has no position");
			return DetailHelper.ParseVector3(this.Position);
		}

		public Quaternion GetRotation()
		{
			if (this.Rotation == null)
				throw new InvalidOperationException("Template has no rotation");
			var ang = DetailHelper.ParseVector3(this.Rotation);
			ang *= MathHelper.DegreesToRadians(1);
			return 
				Quaternion.FromAxisAngle(Vector3.UnitY, ang.Y);// *
				// Quaternion.FromAxisAngle(Vector3.UnitY, ang.Y) *
				// Quaternion.FromAxisAngle(Vector3.UnitY, ang.Y);
		}

		[XmlAttribute("id")]
		public string ID { get; set; }

		[XmlElement("model")]
		public string Model { get; set; }

		[XmlElement("position")]
		public string Position { get; set; }

		[XmlElement("rotation")]
		public string Rotation { get; set; }

		[XmlElement("shape")]
		public ShapeTemplate Shape { get; set; }

		[XmlElement("behaviour")]
		public BehaviourTemplate[] Behaviours { get; set; }

		[XmlElement("sub-detail")]
		public DetailTemplate[] Children { get; set; }
	}

	public class BehaviourTemplate
	{
		[XmlAttribute("id")]
		public string ID { get; set; }

		[XmlElement("class")]
		public string Class { get; set; }

		[XmlElement("slot")]
		public SlotConnectionTemplate[] SlotConnections { get; set; }
	}

	public class SlotConnectionTemplate
	{
		[XmlAttribute("name")]
		public string Slot { get; set; }

		[XmlElement("behaviour")]
		public string Source { get; set; }

		[XmlElement("signal")]
		public string Signal { get; set; }
	}

	public class ShapeTemplate
	{
		[XmlElement("size")]
		public string Size { get; set; }

		public Vector3 GetSize()
		{
			if (this.Size == null)
				throw new InvalidOperationException("Template has no size");
			return DetailHelper.ParseVector3(this.Size);
		}
	}
}
