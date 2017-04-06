using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace Samples.Models
{
	public class Configurations
	{
		public static readonly Dictionary<string, Color> ColorPairs = new Dictionary<string, Color>
		{
			{ "<Accent Color>", Color.Accent },
			{ "Aqua", Color.Aqua },
			{ "Black", Color.Black },
			{ "Blue", Color.Blue },
			{ "Fuchsia", Color.Fuchsia },
			{ "Gray", Color.Gray },
			{ "Green", Color.Green },
			{ "Lime", Color.Lime },
			{ "Maroon", Color.Maroon },
			{ "Navy", Color.Navy },
			{ "Olive", Color.Olive },
			{ "Pink", Color.Pink },
			{ "Purple", Color.Purple },
			{ "Red", Color.Red },
			{ "Silver", Color.Silver },
			{ "Teal", Color.Teal },
			{ "White", Color.White },
			{ "Yellow", Color.Yellow },
		};

		public static List<Color> Colors => ColorPairs.Values.ToList ();

		public static List<string> ColorNames => ColorPairs.Keys.ToList ();
	}
}
