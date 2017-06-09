using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Xamarin.Controls
{
	partial class InkPresenter : Canvas
	{
		private Path tempPathShape;
		private bool clipToBounds;

		static InkPresenter ()
		{
			var di = DisplayInformation.GetForCurrentView ();
			ScreenDensity = di.LogicalDpi / 96.0f;
		}

		public InkPresenter ()
		{
			Initialize ();
		}

		private void Initialize ()
		{
			tempPathShape = new Path ();
			Children.Add (tempPathShape);

			Background = new SolidColorBrush (Colors.Transparent);

			//PointerEntered += OnPointerEntered;
			//PointerExited += OnPointerExited;
			PointerPressed += OnPointerPressed;
			PointerMoved += OnPointerMoved;
			PointerReleased += OnPointerReleased;
			PointerCanceled += OnPointerCancelled;
		}

		public bool ClipToBounds
		{
			get { return clipToBounds; }
			set
			{
				if (clipToBounds != value)
				{
					clipToBounds = value;
					if (clipToBounds)
					{
						UpdateClip ();
						SizeChanged += OnSizeChanged;
					}
					else
					{
						SizeChanged -= OnSizeChanged;
					}
				}
			}
		}

		private void OnSizeChanged (object sender, SizeChangedEventArgs e)
		{
			UpdateClip ();
		}

		private void UpdateClip ()
		{
			var clipRect = new Rect (0, 0, ActualWidth, ActualHeight);
			Clip = new RectangleGeometry { Rect = clipRect };
		}

		//private void OnPointerEntered (object sender, PointerRoutedEventArgs e)
		//{
		//}

		//private void OnPointerExited (object sender, PointerRoutedEventArgs e)
		//{
		//}

		private void OnPointerPressed (object sender, PointerRoutedEventArgs e)
		{
			// capture the pointer
			CapturePointer (e.Pointer);

			// create a new path and set the options
			currentPath = new InkStroke (new PathGeometry (), new List<Point> (), StrokeColor, StrokeWidth);
			tempPathShape.Data = currentPath.Path;
			tempPathShape.Tag = currentPath;
			SetPathProperties (tempPathShape);

			// obtain the location of the touch
			var point = e.GetCurrentPoint (this);
			var touchX = point.Position.X;
			var touchY = point.Position.Y;

			// move to the touched point
			currentPath.Path.MoveTo (touchX, touchY);
			currentPath.GetPoints ().Add (point.Position);

			// update the dirty rectangle
			ResetBounds ((float)touchX, (float)touchY);
		}

		private void OnPointerMoved (object sender, PointerRoutedEventArgs e)
		{
			// obtain the location of the touch
			var point = e.GetCurrentPoint (this);
			var touchX = point.Position.X;
			var touchY = point.Position.Y;

			if (HasMovedFarEnough (currentPath, touchX, touchY))
			{
				// add it to the current path
				currentPath.Path.LineTo (touchX, touchY);
				currentPath.GetPoints ().Add (point.Position);

				// update the dirty rectangle
				UpdateBounds ((float)touchX, (float)touchY);
			}
		}

		private void OnPointerReleased (object sender, PointerRoutedEventArgs e)
		{
			OnPointerMoved (sender, e);

			// add the current path and points to their respective lists.
			var smoothed = PathSmoothing.SmoothedPathWithGranularity (currentPath, 4);
			paths.Add (smoothed);

			// reset the drawing
			currentPath = null;
			tempPathShape.Tag = null;
			tempPathShape.Data = new GeometryGroup ();

			// replace the temp path with a smoothed one
			var pathShape = new Path { Data = smoothed.Path, Tag = smoothed };
			SetPathProperties (pathShape);
			Children.Add (pathShape);

			// we are done with drawing
			OnStrokeCompleted ();
		}

		private void OnPointerCancelled (object sender, PointerRoutedEventArgs e)
		{
			OnPointerReleased (sender, e);
		}

		public void Invalidate ()
		{
			var children = Children.OfType<Path> ().ToArray ();
			foreach (var path in children)
			{
				if (path == tempPathShape)
				{
					SetPathProperties (path);
				}
				else
				{
					var stroke = path.Tag as InkStroke;
					if (stroke != null && paths.Contains (stroke))
					{
						SetPathProperties (path);
					}
					else
					{
						Children.Remove (path);
					}
				}
			}
			var childStrokes = children.Select (c => c.Tag).OfType<InkStroke> ().ToArray ();
			foreach (var path in paths)
			{
				if (!childStrokes.Contains (path))
				{
					var pathShape = new Path { Data = path.Path, Tag = path };
					SetPathProperties (pathShape);
					Children.Add (pathShape);
				}
			}
		}

		private void SetPathProperties (Path path)
		{
			var stroke = path?.Tag as InkStroke;
			if (stroke != null)
			{
				path.StrokeStartLineCap = PenLineCap.Round;
				path.StrokeEndLineCap = PenLineCap.Round;
				path.StrokeLineJoin = PenLineJoin.Round;

				path.Stroke = new SolidColorBrush (stroke.Color);
				path.StrokeThickness = stroke.Width;
			}
		}
	}
}
