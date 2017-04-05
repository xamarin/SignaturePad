using System;
using System.Collections.Generic;
using System.Linq;

#if __ANDROID__
using Android.Graphics;
#elif __IOS__
using CoreGraphics;
using UIKit;
#elif WINDOWS_PHONE
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
#elif WINDOWS_UWP
using Windows.Foundation;
using System.Numerics;
using Windows.UI;
using Windows.UI.Input.Inking;
#endif

namespace Xamarin.Controls
{
	internal static class Extensions
	{
#if __ANDROID__

		public static System.Drawing.SizeF GetSize (this Bitmap image)
		{
			return new System.Drawing.SizeF (image.Width, image.Height);
		}

#elif __IOS__

		public static CGSize GetSize (this UIImage image)
		{
			return image.Size;
		}

		public static void Invalidate (this UIView view)
		{
			view.SetNeedsDisplay ();
		}

		public static void MoveTo (this UIBezierPath path, nfloat x, nfloat y)
		{
			path.MoveTo (new CGPoint (x, y));
		}

		public static void LineTo (this UIBezierPath path, nfloat x, nfloat y)
		{
			path.AddLineTo (new CGPoint (x, y));
		}

#elif WINDOWS_PHONE

		public static void MoveTo (this Stroke stroke, double x, double y)
		{
			stroke.StylusPoints.Add (new StylusPoint (x, y));
		}

		public static void LineTo (this Stroke stroke, double x, double y)
		{
			stroke.StylusPoints.Add (new StylusPoint (x, y));
		}

		public static void AddStrokes (this InkPresenter inkPresenter, IList<Point[]> strokes, Color color, float width)
		{
			var strokeCollection = new StrokeCollection ();
			foreach (var stroke in strokes.Where(s => s.Length > 0))
			{
				var pointCollection = new StylusPointCollection ();
				foreach (var point in stroke)
				{
					pointCollection.Add (new StylusPoint (point.X, point.Y));
				}

				var newStroke = new Stroke (pointCollection);
				strokeCollection.Add (newStroke);

				newStroke.DrawingAttributes = new DrawingAttributes
				{
					Color = color,
					OutlineColor = color,
					Width = width,
					Height = width
				};
			}

			inkPresenter.Strokes = strokeCollection;
		}

		public static IList<Stroke> GetStrokes (this InkPresenter inkPresenter)
		{
			return inkPresenter.Strokes;
		}

		public static IEnumerable<Point> GetPoints (this Stroke stroke)
		{
			return stroke.StylusPoints.Select (p => new Point (p.X, p.Y));
		}

#elif WINDOWS_UWP

		private const float DefaultPressure = 0.5f;
		
		public static void MoveTo (this List<Point> stroke, double x, double y)
		{
			stroke.Add (new Point (x, y));
		}

		public static void LineTo (this List<Point> stroke, double x, double y)
		{
			stroke.Add (new Point (x, y));
		}

		public static IReadOnlyList<InkStroke> GetStrokes (this InkPresenter inkPresenter)
		{
			return inkPresenter.StrokeContainer.GetStrokes ();
		}

		public static void AddStrokes (this InkPresenter inkPresenter, IList<Point[]> strokes, Color color, float width)
		{
			var strokeBuilder = new InkStrokeBuilder ();

			var da = inkPresenter.CopyDefaultDrawingAttributes ();
			da.Color = color;
			da.Size = new Size (width, width);
			strokeBuilder.SetDefaultDrawingAttributes (da);

			var newStrokes = strokes.Where (s => s.Length > 0).Select (s =>
			{
				var points = s.Select (p => new InkPoint (p, DefaultPressure));
				return strokeBuilder.CreateStrokeFromInkPoints (points, Matrix3x2.Identity);
			});

			inkPresenter.StrokeContainer.AddStrokes (newStrokes);
		}

		public static IEnumerable<Point> GetPoints (this InkStroke stroke)
		{
			return stroke.GetInkPoints ().Select (p => p.Position);
		}

#endif
	}
}
