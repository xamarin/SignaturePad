using System.Collections.Generic;
using System.Linq;

#if __ANDROID__
using NativePoint = System.Drawing.PointF;
using NativePath = Android.Graphics.Path;
#elif __IOS__
using NativePoint = CoreGraphics.CGPoint;
using NativePath = UIKit.UIBezierPath;
#elif WINDOWS_PHONE
using System.Windows.Ink;
using NativePoint = System.Windows.Point;
using NativePath = System.Windows.Ink.Stroke;
using InkStroke = System.Windows.Ink.Stroke;
#elif WINDOWS_UWP || WINDOWS_APP
using Windows.UI.Input.Inking;
using NativePoint = Windows.Foundation.Point;
using NativePath = System.Collections.Generic.List<Windows.Foundation.Point>;
using InkStroke = Windows.UI.Input.Inking.InkStroke;
#elif WINDOWS_PHONE_APP
using NativePoint = Windows.Foundation.Point;
using NativePath = Windows.UI.Xaml.Media.PathGeometry;
#endif

namespace Xamarin.Controls
{
	internal static class PathSmoothing
	{
		/// <summary>
		/// Obtain a smoothed path with the specified granularity from the current path using Catmull-Rom spline.
		/// Also outputs a List of the points corresponding to the smoothed path.
		/// </summary>
		/// <remarks>
		/// Implemented using a modified version of the code in the solution at 
		/// http://stackoverflow.com/questions/8702696/drawing-smooth-curves-methods-needed
		/// </remarks>
		public static InkStroke SmoothedPathWithGranularity (InkStroke currentPath, int granularity)
		{
			var currentPoints = currentPath.GetPoints ().ToList ();

			NativePath smoothedPath;
			List<NativePoint> smoothedPoints;
			SmoothedPathWithGranularity (currentPoints, granularity, out smoothedPath, out smoothedPoints);
			if (smoothedPath == null)
			{
				return currentPath;
			}

			// create the new path with the old attributes
#if __ANDROID__ || __IOS__ || WINDOWS_PHONE_APP
			return new InkStroke (smoothedPath, smoothedPoints.ToList (), currentPath.Color, currentPath.Width);
#elif WINDOWS_PHONE
			var da = currentPath.DrawingAttributes;
			smoothedPath.DrawingAttributes = new DrawingAttributes
			{
				Color = da.Color,
				OutlineColor = da.OutlineColor,
				Width = da.Width,
				Height = da.Height,
			};
			return smoothedPath;
#elif WINDOWS_UWP || WINDOWS_APP
			var da = currentPath.DrawingAttributes;
			var builder = new InkStrokeBuilder ();
			builder.SetDefaultDrawingAttributes (new InkDrawingAttributes
			{
				Color = da.Color,
				Size = da.Size
			});
			return builder.CreateStroke (smoothedPath);
#endif
		}

		public static void SmoothedPathWithGranularity (List<NativePoint> currentPoints, int granularity, out NativePath smoothedPath, out List<NativePoint> smoothedPoints)
		{
			// not enough points to smooth effectively, so return the original path and points.
			if (currentPoints.Count < 4)
			{
				smoothedPath = null;
				smoothedPoints = null;
				return;
			}

			// create a new bezier path to hold the smoothed path.
			smoothedPath = new NativePath ();
			smoothedPoints = new List<NativePoint> ();

			// duplicate the first and last points as control points.
			currentPoints.Insert (0, currentPoints[0]);
			currentPoints.Add (currentPoints[currentPoints.Count - 1]);

			// add the first point
			smoothedPath.MoveTo (currentPoints[0].X, currentPoints[0].Y);
			smoothedPoints.Add (currentPoints[0]);

			for (var index = 1; index < currentPoints.Count - 2; index++)
			{
				var p0 = currentPoints[index - 1];
				var p1 = currentPoints[index];
				var p2 = currentPoints[index + 1];
				var p3 = currentPoints[index + 2];

				// add n points starting at p1 + dx/dy up until p2 using Catmull-Rom splines
				for (var i = 1; i < granularity; i++)
				{
					var t = (float)i * (1f / (float)granularity);
					var tt = t * t;
					var ttt = tt * t;

					// intermediate point
					var mid = new NativePoint
					{
						X = 0.5f * (2f * p1.X + (p2.X - p0.X) * t +
							(2f * p0.X - 5f * p1.X + 4f * p2.X - p3.X) * tt +
							(3f * p1.X - p0.X - 3f * p2.X + p3.X) * ttt),

						Y = 0.5f * (2 * p1.Y + (p2.Y - p0.Y) * t +
							(2 * p0.Y - 5 * p1.Y + 4 * p2.Y - p3.Y) * tt +
							(3 * p1.Y - p0.Y - 3 * p2.Y + p3.Y) * ttt)
					};
					smoothedPath.LineTo (mid.X, mid.Y);
					smoothedPoints.Add (mid);
				}

				// add p2
				smoothedPath.LineTo (p2.X, p2.Y);
				smoothedPoints.Add (p2);
			}

			// add the last point
			var last = currentPoints[currentPoints.Count - 1];
			smoothedPath.LineTo (last.X, last.Y);
			smoothedPoints.Add (last);
		}

#if WINDOWS_APP

		public static List<NativePoint> BezierToLinear (IReadOnlyList<InkStrokeRenderingSegment> segments, float step)
		{
			var linear = new List<NativePoint> ();
			linear.Add (segments[0].Position);

			var prev = segments[0].Position;
			foreach (var segment in segments.Skip (1))
			{
				var t = 0f;
				while (t < 1.0f)
				{
					t += step;

					var p1 = prev;
					var c1 = segment.BezierControlPoint1;
					var c2 = segment.BezierControlPoint2;
					var p2 = segment.Position;

					linear.Add (Qubic (t, p1, c1, c2, p2));
				}

				prev = segment.Position;
			}

			return linear;
		}

		private static NativePoint Quadratic (float t, NativePoint p1, NativePoint p2, NativePoint p3)
		{
			return new NativePoint
			{
				X = (1 - t) * (1 - t) * p1.X + 2 * (1 - t) * t * p2.X + t * t * p3.X,
				Y = (1 - t) * (1 - t) * p1.Y + 2 * (1 - t) * t * p2.Y + t * t * p3.Y
			};
		}

		private static NativePoint Qubic (float t, NativePoint p0, NativePoint p1, NativePoint p2, NativePoint p3)
		{
			return new NativePoint
			{
				X = (1 - t) * (1 - t) * (1 - t) * p0.X + 3 * (1 - t) * (1 - t) * t * p1.X + 3 * (1 - t) * t * t * p2.X + t * t * t * p3.X,
				Y = (1 - t) * (1 - t) * (1 - t) * p0.Y + 3 * (1 - t) * (1 - t) * t * p1.Y + 3 * (1 - t) * t * t * p2.Y + t * t * t * p3.Y
			};
		}

		// P = (1-t)•A + t•B = (1-t)2•P1 + 2•(1-t) •t•C + t2•P2
#endif
	}
}
