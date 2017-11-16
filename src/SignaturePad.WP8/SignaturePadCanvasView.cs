using ImageTools;
using ImageTools.IO.Png;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Xamarin.Controls
{
	public partial class SignaturePadCanvasView : ContentControl
	{
		private Color strokeColor;
		private float lineWidth;

		private InkPresenter inkPresenter;
		private Stroke currentStroke;

		public SignaturePadCanvasView ()
		{
			Initialize ();
		}

		private void Initialize ()
		{
			var grid = new Grid ();

			inkPresenter = new InkPresenter ();
			inkPresenter.SetValue (Grid.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
			inkPresenter.SetValue (Grid.VerticalAlignmentProperty, VerticalAlignment.Stretch);
			inkPresenter.Background = new SolidColorBrush (Colors.Transparent);
			grid.Children.Add (inkPresenter);

			inkPresenter.MouseLeftButtonDown += OnMouseDown;
			inkPresenter.MouseMove += OnMouseMove;
			inkPresenter.MouseLeftButtonUp += OnMouseUp;
			inkPresenter.LostMouseCapture += OnMouseLost;

			// get some defaults
			var settings = new ImageConstructionSettings ();
			settings.ApplyDefaults ();

			StrokeWidth = settings.StrokeWidth.Value;
			StrokeColor = settings.StrokeColor.Value;

			HorizontalContentAlignment = HorizontalAlignment.Stretch;
			VerticalContentAlignment = VerticalAlignment.Stretch;
			Content = grid;
		}

		public Color StrokeColor
		{
			get { return strokeColor; }
			set
			{
				strokeColor = value;
				foreach (var stroke in inkPresenter.Strokes)
				{
					stroke.DrawingAttributes.Color = strokeColor;
					stroke.DrawingAttributes.OutlineColor = strokeColor;
				}
			}
		}

		public float StrokeWidth
		{
			get { return lineWidth; }
			set
			{
				lineWidth = value;
				foreach (var stroke in inkPresenter.Strokes)
				{
					stroke.DrawingAttributes.Width = lineWidth;
					stroke.DrawingAttributes.Height = lineWidth;
				}
			}
		}

		public void Clear ()
		{
			inkPresenter.Strokes.Clear ();

			OnCleared ();
		}

		private WriteableBitmap GetImageInternal (Size scale, Rect signatureBounds, Size imageSize, float strokeWidth, Color strokeColor, Color backgroundColor)
		{
			var presenter = new InkPresenter
			{
				Width = imageSize.Width / scale.Width,
				Height = imageSize.Height / scale.Height,
				Strokes = new StrokeCollection (),
				Background = new SolidColorBrush (backgroundColor)
			};

			foreach (var stroke in inkPresenter.Strokes)
			{
				Stroke tempStroke;
				if (signatureBounds.X != 0 || signatureBounds.Y != 0)
				{
					var newCollection = new StylusPointCollection ();
					foreach (var point in stroke.StylusPoints)
					{
						var newPoint = new StylusPoint
						{
							X = point.X - signatureBounds.X,
							Y = point.Y - signatureBounds.Y
						};
						newCollection.Add (newPoint);
					}

					tempStroke = new Stroke (newCollection);
				}
				else
				{
					tempStroke = new Stroke (stroke.StylusPoints);
				}

				tempStroke.DrawingAttributes.Color = strokeColor;
				tempStroke.DrawingAttributes.Width = lineWidth;
				tempStroke.DrawingAttributes.Height = lineWidth;
				presenter.Strokes.Add (tempStroke);
				tempStroke = null;
			}

			return new WriteableBitmap (presenter, new ScaleTransform { ScaleX = scale.Width, ScaleY = scale.Height });
		}

		private Task<Stream> GetImageStreamInternal (SignatureImageFormat format, Size scale, Rect signatureBounds, Size imageSize, float strokeWidth, Color strokeColor, Color backgroundColor)
		{
			var image = GetImageInternal (scale, signatureBounds, imageSize, strokeWidth, strokeColor, backgroundColor);
			if (image != null)
			{
				if (format == SignatureImageFormat.Jpeg)
				{
					var stream = new MemoryStream ();
					image.SaveJpeg (stream, image.PixelWidth, image.PixelHeight, 0, 100);
					return Task.FromResult<Stream> (stream);
				}
				else if (format == SignatureImageFormat.Png)
				{
					var stream = new MemoryStream ();
					var encoder = new PngEncoder ();
					encoder.Encode (image.ToImage (), stream);
					return Task.FromResult<Stream> (stream);
				}
			}
			return Task.FromResult<Stream> (null);
		}

		private void OnMouseDown (object sender, MouseButtonEventArgs e)
		{
			if (!IsEnabled)
			{
				return;
			}

			inkPresenter.CaptureMouse ();

			var points = new StylusPointCollection ();
			points.Add (e.StylusDevice.GetStylusPoints (inkPresenter));

			currentStroke = new Stroke (points);
			currentStroke.DrawingAttributes = new DrawingAttributes
			{
				Color = StrokeColor,
				OutlineColor = StrokeColor,
				Width = StrokeWidth,
				Height = StrokeWidth,
			};

			inkPresenter.Strokes.Add (currentStroke);
		}

		private void OnMouseMove (object sender, MouseEventArgs e)
		{
			if (!IsEnabled)
			{
				return;
			}

			if (currentStroke != null)
			{
				currentStroke.StylusPoints.Add (e.StylusDevice.GetStylusPoints (inkPresenter));
			}
		}

		private void OnMouseLost (object sender, MouseEventArgs e)
		{
			if (!IsEnabled)
			{
				return;
			}

			var curr = currentStroke;

			if (curr != null)
			{
				var smoothed = PathSmoothing.SmoothedPathWithGranularity (curr, 2);

				// swap the old path with the smoothed one
				inkPresenter.Strokes.Remove (curr);
				inkPresenter.Strokes.Add (smoothed);

				currentStroke = null;
			}

			OnStrokeCompleted ();
		}

		private void OnMouseUp (object sender, MouseButtonEventArgs e)
		{
			if (!IsEnabled)
			{
				return;
			}
		}
	}
}
