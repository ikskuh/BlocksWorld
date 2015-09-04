using Jitter.Collision.Shapes;
using Jitter.LinearMath;
using OpenTK;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace BlocksWorld
{
	public static class DetailHelper
	{
		static readonly Regex regexVector3 = new Regex(
			@"^(?<x>-?\d+(?:\.\d*)?);(?<y>-?\d+(?:\.\d*)?);(?<z>-?\d+(?:\.\d*)?)$",
			RegexOptions.Compiled);

		public static Vector3 ParseVector3(string value)
		{
			var match = regexVector3.Match(value);
			if (match.Success == false)
				throw new FormatException("The vector has an invalid format.");
			float x = float.Parse(match.Groups["x"].Value, CultureInfo.InvariantCulture);
			float y = float.Parse(match.Groups["y"].Value, CultureInfo.InvariantCulture);
			float z = float.Parse(match.Groups["z"].Value, CultureInfo.InvariantCulture);
			return new Vector3(x, y, z);
		}

		public static Shape CreateShape(Vector3 size)
		{
			return new BoxShape(size.Jitter());
		}
	}
}