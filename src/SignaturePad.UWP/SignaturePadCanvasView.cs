using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.Graphics.Canvas;

namespace Xamarin.Controls
{
	public partial class SignaturePadCanvasView : Grid
	{
		private Color strokeColor;
		private float lineWidth;

		private InkCanvas inkCanvas;
		private InkPresenter inkPresenter;

		public SignaturePadCanvasView ()
		{
			Initialize ();
		}

		private void Initialize ()
		{
			inkCanvas = new InkCanvas ();
			inkCanvas.SetValue (Grid.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
			inkCanvas.SetValue (Grid.VerticalAlignmentProperty, VerticalAlignment.Stretch);
			Children.Add (inkCanvas);

			inkPresenter = inkCanvas.InkPresenter;
			inkPresenter.StrokesCollected += (sender, e) => OnStrokeCompleted ();
			inkPresenter.InputDeviceTypes = CoreInputDeviceTypes.Touch | CoreInputDeviceTypes.Pen | CoreInputDeviceTypes.Mouse;

			// get some defaults
			var settings = new ImageConstructionSettings ();
			settings.ApplyDefaults ();

			StrokeWidth = settings.StrokeWidth.Value;
			StrokeColor = settings.StrokeColor.Value;
		}

		public Color StrokeColor
		{
			get { return strokeColor; }
			set
			{
				strokeColor = value;
				foreach (var stroke in inkPresenter.StrokeContainer.GetStrokes ())
				{
					stroke.DrawingAttributes.Color = strokeColor;
				}
				var da = inkPresenter.CopyDefaultDrawingAttributes ();
				da.Color = strokeColor;
				inkPresenter.UpdateDefaultDrawingAttributes (da);
			}
		}

		public float StrokeWidth
		{
			get { return lineWidth; }
			set
			{
				lineWidth = value;
				foreach (var stroke in inkPresenter.StrokeContainer.GetStrokes ())
				{
					stroke.DrawingAttributes.Size = new Size (lineWidth, lineWidth);
				}
				var da = inkPresenter.CopyDefaultDrawingAttributes ();
				da.Size = new Size (lineWidth, lineWidth);
				inkPresenter.UpdateDefaultDrawingAttributes (da);
			}
		}

		public void Clear ()
		{
			inkPresenter.StrokeContainer.Clear ();

			OnCleared ();
		}

		private async Task<Stream> GetImageStreamInternal (SignatureImageFormat format, Size scale, Rect signatureBounds, Size imageSize, float strokeWidth, Color strokeColor, Color backgroundColor)
		{
			CanvasBitmapFileFormat cbff;
			if (format == SignatureImageFormat.Jpeg)
			{
				cbff = CanvasBitmapFileFormat.Jpeg;
			}
			else if (format == SignatureImageFormat.Png)
			{
				cbff = CanvasBitmapFileFormat.Png;
			}
			else
			{
				return null;
			}

			using (var offscreen = GetRenderTarget (scale, signatureBounds, imageSize, strokeWidth, strokeColor, backgroundColor))
			{
				var fileStream = new InMemoryRandomAccessStream ();
				await offscreen.SaveAsync (fileStream, cbff);

				var stream = fileStream.AsStream ();
				stream.Position = 0;

				return stream;
			}
		}

		private WriteableBitmap GetImageInternal (Size scale, Rect signatureBounds, Size imageSize, float strokeWidth, Color strokeColor, Color backgroundColor)
		{
			using (var offscreen = GetRenderTarget (scale, signatureBounds, imageSize, strokeWidth, strokeColor, backgroundColor))
			{
				var bitmap = new WriteableBitmap ((int)offscreen.SizeInPixels.Width, (int)offscreen.SizeInPixels.Height);
				offscreen.GetPixelBytes (bitmap.PixelBuffer);
				return bitmap;
			}
		}

		private CanvasRenderTarget GetRenderTarget (Size scale, Rect signatureBounds, Size imageSize, float strokeWidth, Color strokeColor, Color backgroundColor)
		{
			var device = CanvasDevice.GetSharedDevice ();
			var offscreen = new CanvasRenderTarget (device, (int)imageSize.Width, (int)imageSize.Height, 96);

			using (var session = offscreen.CreateDrawingSession ())
			{
				session.Clear (backgroundColor);

				session.Transform = Matrix3x2.Multiply (
					Matrix3x2.CreateTranslation ((float)-signatureBounds.X, (float)-signatureBounds.Y),
					Matrix3x2.CreateScale ((float)scale.Width, (float)scale.Height));

				// apply the specified colors/style
				var strokes = inkPresenter.StrokeContainer.GetStrokes ().Select (s =>
				{
					// clone first, since this will change the UI if we don't
					s = s.Clone ();
					var attr = s.DrawingAttributes;
					attr.Color = strokeColor;
					attr.Size = new Size (lineWidth, lineWidth);
					s.DrawingAttributes = attr;
					return s;
				});

				session.DrawInk (strokes.ToArray ());
			}

			return offscreen;
		}
	}
}
