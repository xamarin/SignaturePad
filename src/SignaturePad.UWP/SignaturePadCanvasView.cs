using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
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
	[TemplatePart (Name = PartInkCanvas, Type = typeof (InkCanvas))]
	public partial class SignaturePadCanvasView : Control
	{
		public static readonly DependencyProperty StrokeColorProperty;
		public static readonly DependencyProperty StrokeWidthProperty;

		private const string PartInkCanvas = "InkCanvas";

		private InkPresenter inkPresenter;

		static SignaturePadCanvasView ()
		{
			StrokeColorProperty = DependencyProperty.Register (
				nameof (StrokeColor),
				typeof (Color),
				typeof (SignaturePadCanvasView),
				new PropertyMetadata (ImageConstructionSettings.DefaultStrokeColor, OnStrokePropertiesChanged));

			StrokeWidthProperty = DependencyProperty.Register (
				nameof (StrokeWidth),
				typeof (double),
				typeof (SignaturePadCanvasView),
				new PropertyMetadata ((double)ImageConstructionSettings.DefaultStrokeWidth, OnStrokePropertiesChanged));
		}

		public SignaturePadCanvasView ()
		{
			DefaultStyleKey = typeof (SignaturePadCanvasView);

			IsEnabledChanged += delegate
			{
				var ip = inkPresenter;
				if (ip != null)
					ip.IsInputEnabled = IsEnabled;
			};
		}

		protected override void OnApplyTemplate ()
		{
			inkPresenter = InkCanvas?.InkPresenter;
			inkPresenter.StrokesCollected += (sender, e) => OnStrokeCompleted ();
			inkPresenter.InputDeviceTypes = CoreInputDeviceTypes.Touch | CoreInputDeviceTypes.Pen | CoreInputDeviceTypes.Mouse;

			OnStrokePropertiesChanged (this, null);
		}

		private InkCanvas InkCanvas => GetTemplateChild (PartInkCanvas) as InkCanvas;

		public Color StrokeColor
		{
			get { return (Color)GetValue (StrokeColorProperty); }
			set { SetValue (StrokeColorProperty, value); }
		}

		public double StrokeWidth
		{
			get { return (double)GetValue (StrokeWidthProperty); }
			set { SetValue (StrokeWidthProperty, value); }
		}

		public void Clear ()
		{
			if (inkPresenter != null)
			{
				inkPresenter.StrokeContainer.Clear ();

				OnCleared ();
			}
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
					attr.Size = new Size (StrokeWidth, StrokeWidth);
					s.DrawingAttributes = attr;
					return s;
				});

				session.DrawInk (strokes.ToArray ());
			}

			return offscreen;
		}

		private static void OnStrokePropertiesChanged (DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var signaturePad = d as SignaturePadCanvasView;

			var inkPresenter = signaturePad.inkPresenter;
			if (inkPresenter != null)
			{
				var da = inkPresenter.CopyDefaultDrawingAttributes ();
				da.Color = signaturePad.StrokeColor;
				da.Size = new Size (signaturePad.StrokeWidth, signaturePad.StrokeWidth);

				inkPresenter.UpdateDefaultDrawingAttributes (da);

				foreach (var stroke in inkPresenter.StrokeContainer.GetStrokes ())
				{
					stroke.DrawingAttributes = da;
				}
			}
		}
	}
}
