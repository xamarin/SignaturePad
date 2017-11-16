using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Xamarin.Controls
{
	internal class InkPresenter : Canvas
	{
		private InkManager inkManager;
		private Color strokeColor;
		private float strokeWidth;
		private uint currentPointerId;
		private Path tempPathShape;
		private bool clipToBounds;

		public InkPresenter ()
		{
			inkManager = new InkManager ();

			// for the visual
			tempPathShape = new Path ();
			Children.Add (tempPathShape);

			// touch events
			PointerPressed += OnCanvasPointerPressed;
			PointerMoved += OnCanvasPointerMoved;
			PointerReleased += OnCanvasPointerReleased;

			// defaults
			IsInputEnabled = true;
			Background = new SolidColorBrush (Colors.Transparent);
			StrokeColor = ImageConstructionSettings.Black;
			StrokeWidth = 2f;
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

		public bool IsInputEnabled { get; set; }

		private void OnSizeChanged (object sender, SizeChangedEventArgs e)
		{
			UpdateClip ();
		}

		private void UpdateClip ()
		{
			var clipRect = new Rect (0, 0, ActualWidth, ActualHeight);
			Clip = new RectangleGeometry { Rect = clipRect };
		}

		public Color StrokeColor
		{
			get { return strokeColor; }
			set
			{
				strokeColor = value;
				var da = CreateDefaultDrawingAttributes ();
				inkManager.SetDefaultDrawingAttributes (da);
			}
		}

		public float StrokeWidth
		{
			get { return strokeWidth; }
			set
			{
				strokeWidth = value;
				var da = CreateDefaultDrawingAttributes ();
				inkManager.SetDefaultDrawingAttributes (da);
			}
		}

		public event EventHandler StrokeCompleted;

		public IReadOnlyList<InkStroke> GetStrokes ()
		{
			return inkManager.GetStrokes ();
		}

		public void Clear ()
		{
			foreach (var stroke in inkManager.GetStrokes ())
			{
				stroke.Selected = true;
			}
			inkManager.DeleteSelected ();

			Invalidate ();
		}

		public void AddStrokes (IList<Point[]> strokes, Color color, float width)
		{
			var strokeBuilder = new InkStrokeBuilder ();

			var da = CreateDefaultDrawingAttributes ();
			da.Color = color;
			da.Size = new Size (width, width);
			strokeBuilder.SetDefaultDrawingAttributes (da);

			foreach (var stroke in strokes)
			{
				var inkStroke = strokeBuilder.CreateStroke (stroke);
				inkManager.AddStroke (inkStroke);
			}

			Invalidate ();
		}

		private InkDrawingAttributes CreateDefaultDrawingAttributes ()
		{
			return new InkDrawingAttributes
			{
				Color = strokeColor,
				PenTip = PenTipShape.Circle,
				Size = new Size (strokeWidth, strokeWidth),
				FitToCurve = true
			};
		}

		private void OnCanvasPointerPressed (object sender, PointerRoutedEventArgs e)
		{
			if (!IsInputEnabled)
			{
				return;
			}

			if (currentPointerId != 0)
			{
				// we only handle a single "pen" at a time
				return;
			}

			var pointerDevice = e.Pointer.PointerDeviceType;
			var pointerPoint = e.GetCurrentPoint (this);

			// accept pen, touch and left mouse
			if (pointerDevice != PointerDeviceType.Mouse || pointerPoint.Properties.IsLeftButtonPressed)
			{
				currentPointerId = pointerPoint.PointerId;
				CapturePointer (e.Pointer);

				inkManager.ProcessPointerDown (pointerPoint);

				// update the visual
				var geo = new PathGeometry ();
				geo.MoveTo (pointerPoint.Position.X, pointerPoint.Position.Y);
				tempPathShape.Data = geo;
				SetPathProperties (tempPathShape);
			}
		}

		private void OnCanvasPointerMoved (object sender, PointerRoutedEventArgs e)
		{
			if (!IsInputEnabled)
			{
				return;
			}

			var pointerPoint = e.GetCurrentPoint (this);

			if (pointerPoint.PointerId == currentPointerId)
			{
				inkManager.ProcessPointerUpdate (pointerPoint);

				// update the visual
				var path = (PathGeometry)tempPathShape.Data;
				path.LineTo (pointerPoint.Position.X, pointerPoint.Position.Y);
			}
		}

		private void OnCanvasPointerReleased (object sender, PointerRoutedEventArgs e)
		{
			if (!IsInputEnabled)
			{
				return;
			}

			var pointerPoint = e.GetCurrentPoint (this);

			if (pointerPoint.PointerId == currentPointerId)
			{
				inkManager.ProcessPointerUp (pointerPoint);

				// update the visual
				tempPathShape.Data = new GeometryGroup ();
				Invalidate ();

				OnStrokeCompleted ();
			}

			currentPointerId = 0;
		}

		public void Invalidate ()
		{
			var children = Children.OfType<Path> ().ToArray ();
			var strokes = inkManager.GetStrokes ();
			foreach (var path in children)
			{
				if (path == tempPathShape)
				{
					SetPathProperties (path);
				}
				else
				{
					var stroke = path.Tag as InkStroke;
					if (stroke != null && strokes.Contains (stroke))
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
			foreach (var stroke in strokes)
			{
				if (!childStrokes.Contains (stroke))
				{
					var renderingStrokes = stroke.GetRenderingSegments ();

					var segments = new PathSegmentCollection ();
					var pathShape = new Path
					{
						Tag = stroke,
						Data = new PathGeometry
						{
							Figures = new PathFigureCollection
							{
								new PathFigure
								{
									StartPoint = renderingStrokes.First ().Position,
									Segments = segments
								}
							}
						}
					};
					SetPathProperties (pathShape);

					foreach (var renderStroke in renderingStrokes)
					{
						segments.Add (new BezierSegment
						{
							Point1 = renderStroke.BezierControlPoint1,
							Point2 = renderStroke.BezierControlPoint2,
							Point3 = renderStroke.Position
						});
					}

					Children.Add (pathShape);
				}
			}
			if (Children.Count > 1)
			{
				Children.Move ((uint)Children.IndexOf (tempPathShape), (uint)Children.Count - 1);
			}
		}

		private void SetPathProperties (Path path)
		{
			double width = strokeWidth;
			Color color = strokeColor;

			var stroke = path?.Tag as InkStroke;
			if (stroke != null)
			{
				color = stroke.DrawingAttributes.Color;
				width = stroke.DrawingAttributes.Size.Width;
			}

			path.StrokeStartLineCap = PenLineCap.Round;
			path.StrokeEndLineCap = PenLineCap.Round;
			path.StrokeLineJoin = PenLineJoin.Round;

			path.Stroke = new SolidColorBrush (color);
			path.StrokeThickness = width;
		}

		private void OnStrokeCompleted ()
		{
			StrokeCompleted?.Invoke (this, EventArgs.Empty);
		}
	}
}
