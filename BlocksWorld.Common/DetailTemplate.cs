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
		static readonly Regex regexVector3 = new Regex(
			@"^(?<x>-?\d+(?:\.\d*)?);(?<y>-?\d+(?:\.\d*)?);(?<z>-?\d+(?:\.\d*)?)$", 
			RegexOptions.Compiled);

		public Vector3 GetPosition()
		{
			if (this.Position == null)
				throw new InvalidOperationException("Template has no position");
			return ParseVector3(this.Position);
		}

		public Vector3 GetRotation()
		{
			if (this.Rotation == null)
				throw new InvalidOperationException("Template has no rotation");
			var ang = ParseVector3(this.Rotation);
			ang *= MathHelper.DegreesToRadians(1);
			return ang;
		}

		private Vector3 ParseVector3(string value)
		{
			var match = regexVector3.Match(value);
			if (match.Success == false)
				throw new FormatException("The vector has an invalid format.");
			float x = float.Parse(match.Groups["x"].Value, CultureInfo.InvariantCulture);
			float y = float.Parse(match.Groups["y"].Value, CultureInfo.InvariantCulture);
			float z = float.Parse(match.Groups["z"].Value, CultureInfo.InvariantCulture);
			return new Vector3(x, y, z);
		}

		[XmlAttribute("id")]
		public string ID { get; set; }

		[XmlElement("model")]
		public string Model { get; set; }

		[XmlElement("position")]
		public string Position { get; set; }

		[XmlElement("rotation")]
		public string Rotation { get; set; }

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
}
