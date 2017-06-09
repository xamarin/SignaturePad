using System;
using System.Collections.Generic;
using System.Linq;

#if __ANDROID__
using NativeRect = System.Drawing.RectangleF;
using NativeSize = System.Drawing.SizeF;
using NativePoint = System.Drawing.PointF;
using NativeColor = Android.Graphics.Color;
using NativeImage = Android.Graphics.Bitmap;
using NativePath = Android.Graphics.Path;
#elif __IOS__
using NativeRect = CoreGraphics.CGRect;
using NativeSize = CoreGraphics.CGSize;
using NativePoint = CoreGraphics.CGPoint;
using NativeColor = UIKit.UIColor;
using NativeImage = UIKit.UIImage;
using NativePath = UIKit.UIBezierPath;
#elif WINDOWS_PHONE_APP
using NativeRect = Windows.Foundation.Rect;
using NativeSize = Windows.Foundation.Size;
using NativePoint = Windows.Foundation.Point;
using NativeColor = Windows.UI.Color;
using NativeImage = Windows.UI.Xaml.Media.Imaging.WriteableBitmap;
using NativePath = Windows.UI.Xaml.Media.PathGeometry;
#endif

namespace Xamarin.Controls
{
	internal partial class InkPresenter
	{
		private const float MinimumPointDistance = 2.0f;

		public static float ScreenDensity;

		private readonly List<InkStroke> paths = new List<InkStroke> ();
		private InkStroke currentPath;

		// used to determine rectangle that needs to be redrawn
		private float dirtyRectLeft;
		private float dirtyRectTop;
		private float dirtyRectRight;
		private float dirtyRectBottom;

		private NativeImage bitmapBuffer;

		// public properties

		public NativeColor StrokeColor { get; set; } = ImageConstructionSettings.Black;

		public float StrokeWidth { get; set; } = 1f;

		// private properties

#if __IOS__
		private float Width => (float)Bounds.Width;

		private float Height => (float)Bounds.Height;
#endif

		private bool ShouldRedrawBufferImage
		{
			get
			{
				var sizeChanged = false;
				if (bitmapBuffer != null)
				{
					var s = bitmapBuffer.GetSize ();
					sizeChanged = s.Width != Width || s.Height != Height;
				}

				return sizeChanged ||
					(bitmapBuffer != null && paths.Count == 0) ||
					paths.Any (p => p.IsDirty);
			}
		}

		private NativeRect DirtyRect
		{
			get
			{
				var x = Math.Min (dirtyRectLeft, dirtyRectRight);
				var y = Math.Min (dirtyRectTop, dirtyRectBottom);
				var w = Math.Abs (dirtyRectRight - dirtyRectLeft);
				var h = Math.Abs (dirtyRectBottom - dirtyRectTop);
				var half = StrokeWidth / 2f;
				return new NativeRect (x - half, y - half, w + StrokeWidth, h + StrokeWidth);
			}
		}

		// public events

		public event EventHandler StrokeCompleted;

		// public methods

		public IReadOnlyList<InkStroke> GetStrokes ()
		{
			return paths;
		}

		public void Clear ()
		{
			paths.Clear ();
			currentPath = null;

			this.Invalidate ();
		}

		public void AddStroke (NativePoint[] strokePoints, NativeColor color, float width)
		{
			if (AddStrokeInternal (strokePoints, color, width))
			{
				this.Invalidate ();
			}
		}

		public void AddStrokes (IEnumerable<NativePoint[]> strokes, NativeColor color, float width)
		{
			var changed = false;

			foreach (var stroke in strokes)
			{
				if (AddStrokeInternal (stroke, color, width))
				{
					changed = true;
				}
			}

			if (changed)
			{
				this.Invalidate ();
			}
		}

		private bool AddStrokeInternal (IEnumerable<NativePoint> points, NativeColor color, float width)
		{
			var strokePoints = points?.ToList ();

			if (strokePoints == null || strokePoints.Count == 0)
			{
				return false;
			}

			var newpath = new NativePath ();
			newpath.MoveTo (strokePoints[0].X, strokePoints[0].Y);
			foreach (var point in strokePoints.Skip (1))
			{
				newpath.LineTo (point.X, point.Y);
			}

			paths.Add (new InkStroke (newpath, strokePoints, color, width));

			return true;
		}

		// private methods

		private bool HasMovedFarEnough (InkStroke stroke, double touchX, double touchY)
		{
			var lastPoint = stroke.GetPoints ().LastOrDefault ();
			var deltaX = touchX - lastPoint.X;
			var deltaY = touchY - lastPoint.Y;

			var distance = Math.Sqrt (Math.Pow (deltaX, 2) + Math.Pow (deltaY, 2));
			return distance >= MinimumPointDistance;
		}

		/// <summary>
		/// Update the bounds for the rectangle to be redrawn if necessary for the given point.
		/// </summary>
		private void UpdateBounds (NativePoint touch)
		{
			UpdateBounds ((float)touch.X, (float)touch.Y);
		}

		/// <summary>
		/// Update the bounds for the rectangle to be redrawn if necessary for the given point.
		/// </summary>
		private void UpdateBounds (float touchX, float touchY)
		{
			if (touchX < dirtyRectLeft)
				dirtyRectLeft = touchX;
			else if (touchX > dirtyRectRight)
				dirtyRectRight = touchX;

			if (touchY < dirtyRectTop)
				dirtyRectTop = touchY;
			else if (touchY > dirtyRectBottom)
				dirtyRectBottom = touchY;
		}

		/// <summary>
		/// Set the bounds for the rectangle that will need to be redrawn to show the drawn path.
		/// </summary>
		private void ResetBounds (NativePoint touch)
		{
			ResetBounds ((float)touch.X, (float)touch.Y);
		}

		/// <summary>
		/// Set the bounds for the rectangle that will need to be redrawn to show the drawn path.
		/// </summary>
		private void ResetBounds (float touchX, float touchY)
		{
			dirtyRectLeft = touchX;
			dirtyRectRight = touchX;
			dirtyRectTop = touchY;
			dirtyRectBottom = touchY;
		}

		private void OnStrokeCompleted ()
		{
			StrokeCompleted?.Invoke (this, EventArgs.Empty);
		}
	}
}
