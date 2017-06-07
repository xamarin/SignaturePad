using System.Collections.Generic;

#if __ANDROID__
using NativePoint = System.Drawing.PointF;
using NativeColor = Android.Graphics.Color;
using NativePath = Android.Graphics.Path;
#elif __IOS__
using NativePoint = CoreGraphics.CGPoint;
using NativeColor = UIKit.UIColor;
using NativePath = UIKit.UIBezierPath;
#elif WINDOWS_PHONE_APP
using NativePoint = Windows.Foundation.Point;
using NativeColor = Windows.UI.Color;
using NativePath = Windows.UI.Xaml.Media.PathGeometry;
#endif

namespace Xamarin.Controls
{
	internal class InkStroke
	{
		private NativeColor color;
		private float width;
		private IList<NativePoint> inkPoints;

		public InkStroke (NativePath path, IList<NativePoint> points, NativeColor color, float width)
		{
			Path = path;
			inkPoints = points;
			Color = color;
			Width = width;
		}

		public NativePath Path { get; private set; }

		public IList<NativePoint> GetPoints ()
		{
			return inkPoints;
		}

		public NativeColor Color
		{
			get { return color; }
			set
			{
				color = value;
				IsDirty = true;
			}
		}

		public float Width
		{
			get { return width; }
			set
			{
				width = value;
				IsDirty = true;
			}
		}

		internal bool IsDirty { get; set; }
	}
}
