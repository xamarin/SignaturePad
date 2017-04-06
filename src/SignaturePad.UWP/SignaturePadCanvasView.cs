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
		}

		private async Task<Stream> GetImageStreamInternal (SignatureImageFormat format, float scale, Rect imageBounds, float strokeWidth, Color strokeColor, Color backgroundColor)
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

			using (var offscreen = GetRenderTarget (scale, imageBounds, strokeWidth, strokeColor, backgroundColor))
			{
				var fileStream = new InMemoryRandomAccessStream ();
				await offscreen.SaveAsync (fileStream, cbff);

				var stream = fileStream.AsStream ();
				stream.Position = 0;

				return stream;
			}
		}

		private WriteableBitmap GetImageInternal (float scale, Rect imageBounds, float strokeWidth, Color strokeColor, Color backgroundColor)
		{
			using (var offscreen = GetRenderTarget (scale, imageBounds, strokeWidth, strokeColor, backgroundColor))
			{
				var bitmap = new WriteableBitmap ((int)offscreen.SizeInPixels.Width, (int)offscreen.SizeInPixels.Height);
				offscreen.GetPixelBytes (bitmap.PixelBuffer);
				return bitmap;
			}
		}

		private CanvasRenderTarget GetRenderTarget (float scale, Rect imageBounds, float strokeWidth, Color strokeColor, Color backgroundColor)
		{
			var device = CanvasDevice.GetSharedDevice ();
			var currentDpi = DisplayInformation.GetForCurrentView ().LogicalDpi;

			var offscreen = new CanvasRenderTarget (device, (int)imageBounds.Width, (int)imageBounds.Height, currentDpi);

			using (var session = offscreen.CreateDrawingSession ())
			{
				session.Clear (backgroundColor);
				if (imageBounds.X != 0 || imageBounds.Y != 0)
				{
					session.Transform = Matrix3x2.CreateTranslation ((float)-imageBounds.X, (float)-imageBounds.Y);
				}

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

				session.DrawInk (strokes);
			}

			return offscreen;
		}
	}
}
