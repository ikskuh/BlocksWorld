using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TexturePacker
{
	class Program
	{
		static readonly Regex regex = new Regex(@"^(?<id>\d+)(?:_(?<hint>\w+))*\.(?<ext>\w+)$", RegexOptions.Compiled);
		static int Main(string[] args)
		{
			foreach(var input in args)
			{
				if (Directory.Exists(input) == false)
					continue;
				int max = 0;
				Dictionary<int, Bitmap> frames = new Dictionary<int, Bitmap>();
				foreach (var file in Directory.GetFiles(input, "*.png"))
				{
					var match = regex.Match(Path.GetFileName(file));
					if (match.Success == false)
						continue;
					int id = int.Parse(match.Groups["id"].Value);
					string hint = match.Groups["hint"].Value;
					string ext = match.Groups["ext"].Value;

					var bmp = Bitmap.FromFile(file) as Bitmap;
					if (bmp == null)
						continue;

					frames.Add(id, bmp);
					max = Math.Max(max, id);
				}

				if (frames.Count == 0)
					return 1;

				int width = frames.First().Value.Width;
				int height = width * (max + 1);

				using (var result = new Bitmap(width, height))
				{
					using (var g = Graphics.FromImage(result))
					{
						foreach (var frame in frames)
						{
							Rectangle target = new Rectangle(0, width * frame.Key, width, width);
							g.DrawImage(frame.Value, target);
						}
						g.Flush();
					}
					result.Save(input + ".png");
				}
			}
			return 0;
		}
	}
}
