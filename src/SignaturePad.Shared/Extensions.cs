﻿using System;
using System.Collections.Generic;
using System.Linq;

#if __ANDROID__
using Android.Graphics;
using Android.Views;
#elif __IOS__
using CoreGraphics;
using UIKit;
#elif __MACOS__
using CoreGraphics;
using AppKit;
#elif GTK
using Gtk;
using Gdk;
using Cairo;
using Color = Gdk.Color;
using Point = Gdk.Point;
#elif WPF
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using InkPresenter = System.Windows.Ink.StrokeCollection;
#elif WINDOWS_PHONE
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Drawing.Drawing2D;
#elif WINDOWS_UWP || WINDOWS_APP
using Windows.Foundation;
using System.Numerics;
using Windows.UI;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
#elif WINDOWS_PHONE_APP
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
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

		public static System.Drawing.SizeF GetSize (this View view)
		{
			return new System.Drawing.SizeF (view.Width, view.Height);
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

		public static CGSize GetSize (this UIView view)
		{
			return view.Bounds.Size;
		}

#elif __MACOS__

		public static CGSize GetSize (this NSImage image)
		{
			return image.Size;
		}

		public static void Invalidate (this NSView view)
		{
			view.NeedsDisplay = true;
		}

		public static void MoveTo (this CGPath path, nfloat x, nfloat y)
		{
			path.MoveToPoint (new CGPoint (x, y));
		}

		public static void LineTo (this CGPath path, nfloat x, nfloat y)
		{
			path.AddLineToPoint(new CGPoint (x, y));
		}

		public static CGSize GetSize (this NSView view)
		{
			return view.Bounds.Size;
		}

#elif GTK

		public static void MoveTo (this Cairo.Path stroke, double x, double y)
		{
			//stroke.StylusPoints.Add (new Point ((int)x, (int)y));
		}

		public static void LineTo (this Cairo.Path stroke, double x, double y)
		{
			//stroke.StylusPoints.Add (new Point ((int)x, (int)y));
		}

		public static void AddStrokes (this DrawingArea inkPresenter, IList<Point[]> strokes, Color color, float width)
		{
			foreach (var stroke in strokes.Where (s => s.Length > 0))
			{
				var pointCollection = new List<Point> ();
				foreach (var point in stroke)
				{
					pointCollection.Add (new Point (point.X, point.Y));
				}

				var newStroke = new List<Point> (pointCollection);
				//strokeCollection.Add (newStroke);
			}

			//inkPresenter.Strokes = strokeCollection;
		}

		public static void Invalidate (this InkPresenter control)
		{
			
		}

		public static IList<Cairo.Path> GetStrokes (this DrawingArea canvas)
		{
			return new List<Path> ();
			//return canvas.Strokes;
		}

		public static IEnumerable<Point> GetPoints (this Cairo.Path stroke)
		{
			return new List<Point> ();
			//return stroke.Select (p => new Point (p.X, p.Y));
		}

		public static Size GetSize (this SignaturePadCanvasView element)
		{
			return new Size (element.inkPresenter.WidthRequest, element.inkPresenter.HeightRequest);
		}
#elif WPF

		public static void MoveTo (this Stroke stroke, double x, double y)
		{
			stroke.StylusPoints.Add (new StylusPoint (x, y));
		}

		public static void LineTo (this Stroke stroke, double x, double y)
		{
			stroke.StylusPoints.Add (new StylusPoint (x, y));
		}

		public static void AddStrokes (this InkCanvas inkPresenter, IList<StylusPoint[]> strokes, Color color, float width)
		{
			var strokeCollection = new StrokeCollection ();
			foreach (var stroke in strokes.Where (s => s.Length > 0))
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
					Width = width,
					Height = width
				};
			}

			inkPresenter.Strokes = strokeCollection;
		}

		public static void Invalidate (this InkPresenter control)
		{
			
		}

		public static IList<Stroke> GetStrokes (this InkCanvas canvas)
		{
			return canvas.Strokes;
		}

		public static IEnumerable<StylusPoint> GetPoints (this Stroke stroke)
		{
			return stroke.StylusPoints.Select (p => new StylusPoint (p.X, p.Y));
		}

		public static Size GetSize (this FrameworkElement element)
		{
			return new Size (element.Width, element.Height);
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

		public static Size GetSize (this FrameworkElement element)
		{
			return new Size (element.ActualWidth, element.ActualHeight);
		}

#elif WINDOWS_PHONE_APP || WINDOWS_APP

		public static void MoveTo (this List<Point> stroke, double x, double y)
		{
			stroke.Add (new Point (x, y));
		}

		public static void LineTo (this List<Point> stroke, double x, double y)
		{
			stroke.Add (new Point (x, y));
		}

		public static void MoveTo (this PathGeometry stroke, double x, double y)
		{
			var figure = new PathFigure ();
			figure.StartPoint = new Point (x, y);
			var segment = new PolyLineSegment ();
			segment.Points.Add (new Point (x, y));
			figure.Segments.Add (segment);
			stroke.Figures.Add (figure);
		}

		public static void LineTo (this PathGeometry stroke, double x, double y)
		{
			var figure = stroke.Figures.LastOrDefault ();
			if (figure == null)
			{
				figure = new PathFigure ();
				stroke.Figures.Add (figure);
			}
			var segment = figure.Segments.LastOrDefault () as PolyLineSegment;
			if (segment == null)
			{
				segment = new PolyLineSegment ();
				figure.Segments.Add (segment);
			}
			segment.Points.Add (new Point (x, y));
		}

		public static Size GetSize (this FrameworkElement element)
		{
			return new Size (element.ActualWidth, element.ActualHeight);
		}

		public static Size GetSize (this WriteableBitmap image)
		{
			return new Size (image.PixelWidth, image.PixelHeight);
		}

#if WINDOWS_APP

		public static IEnumerable<Point> GetPoints (this InkStroke stroke)
		{
			var empty = new Point ();

			var segments = stroke.GetRenderingSegments ();
			if (segments.Any (s => s.BezierControlPoint1 != empty || s.BezierControlPoint2 != empty))
			{
				// we assume strokes with bezier controls are generated by the inking
				// so we know that the detail will be low - thus we smooth
				return PathSmoothing.BezierToLinear (segments, 0.1f);
			}
			return segments.Select (p => p.Position);
		}

#endif

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

		public static Size GetSize (this FrameworkElement element)
		{
			return new Size (element.ActualWidth, element.ActualHeight);
		}

#endif
	}
}
