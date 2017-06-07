using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using System.Linq;
using Windows.Graphics.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Xamarin.Controls
{
	public partial class SignaturePadCanvasView : Grid
	{
		private Color strokeColor;
		private float lineWidth;

		private InkPresenter inkPresenter;

		public SignaturePadCanvasView ()
		{
			Initialize ();
		}

		private void Initialize ()
		{
			inkPresenter = new InkPresenter ();
			inkPresenter.ClipToBounds = true;
			inkPresenter.StrokeCompleted += OnStrokeCompleted;
			Children.Add (inkPresenter);

			// get some defaults
			var settings = new ImageConstructionSettings ();
			settings.ApplyDefaults ();

			StrokeWidth = settings.StrokeWidth.Value;
			StrokeColor = settings.StrokeColor.Value;
		}

		/// <summary>
		/// Gets or sets the color of the strokes for the signature.
		/// </summary>
		/// <value>The color of the stroke.</value>
		public Color StrokeColor
		{
			get { return inkPresenter.StrokeColor; }
			set
			{
				inkPresenter.StrokeColor = value;
				foreach (var stroke in inkPresenter.GetStrokes ())
				{
					stroke.Color = value;
				}
				inkPresenter.Invalidate ();
			}
		}

		/// <summary>
		/// Gets or sets the width in pixels of the strokes for the signature.
		/// </summary>
		/// <value>The width of the line.</value>
		public float StrokeWidth
		{
			get { return inkPresenter.StrokeWidth; }
			set
			{
				inkPresenter.StrokeWidth = value;
				foreach (var stroke in inkPresenter.GetStrokes ())
				{
					stroke.Width = value;
				}
				inkPresenter.Invalidate ();
			}
		}

		public void Clear ()
		{
			inkPresenter.Clear ();
		}

		private WriteableBitmap GetImageInternal (Size scale, Rect signatureBounds, Size imageSize, float strokeWidth, Color strokeColor, Color backgroundColor)
		{
			var bitmap = BitmapFactory.New ((int)imageSize.Width, (int)imageSize.Height);
			using (var context = bitmap.GetBitmapContext ())
			{
				var w = context.Width;
				var h = context.Height;
				var mainScale = Math.Max (scale.Width, scale.Height);

				bitmap.Clear (backgroundColor);

				foreach (var stroke in inkPresenter.GetStrokes ())
				{
					var color = WriteableBitmapExtensions.ConvertColor (stroke.Color);
					var width = (int)(stroke.Width * mainScale); // TODO: scaling 2 axis

					var points = stroke.GetPoints ();
					var count = points.Count;

					var x1 = (int)((points[0].X - signatureBounds.X) * scale.Width);
					var y1 = (int)((points[0].Y - signatureBounds.Y) * scale.Height);

					if (count == 1)
					{
						bitmap.FillEllipseCentered (x1, y1, width, width, color);
					}

					for (var i = 1; i < count; i += 1)
					{
						var x2 = (int)((points[i].X - signatureBounds.X) * scale.Width);
						var y2 = (int)((points[i].Y - signatureBounds.Y) * scale.Height);
						WriteableBitmapExtensions.DrawLineAa (context, w, h, x1, y1, x2, y2, color, width);
						x1 = x2;
						y1 = y2;
					}
				}
			}
			return bitmap;
		}

		private async Task<Stream> GetImageStreamInternal (SignatureImageFormat format, Size scale, Rect signatureBounds, Size imageSize, float strokeWidth, Color strokeColor, Color backgroundColor)
		{
			Guid encoderId;
			if (format == SignatureImageFormat.Jpeg)
			{
				encoderId = BitmapEncoder.JpegEncoderId;
			}
			else if (format == SignatureImageFormat.Png)
			{
				encoderId = BitmapEncoder.PngEncoderId;
			}
			else
			{
				return null;
			}

			var image = GetImageInternal (scale, signatureBounds, imageSize, strokeWidth, strokeColor, backgroundColor);
			if (image != null)
			{
				var width = (uint)image.PixelWidth;
				var height = (uint)image.PixelHeight;

				// copy buffer to pixels
				byte[] pixels;
				using (var pixelStream = image.PixelBuffer.AsStream ())
				{
					pixels = new byte[(uint)pixelStream.Length];
					await pixelStream.ReadAsync (pixels, 0, pixels.Length);
				}

				return await Task.Run (async () =>
				{
					// encode pixels into stream
					var ms = new MemoryStream ();
					var encoder = await BitmapEncoder.CreateAsync (encoderId, ms.AsRandomAccessStream ());
					encoder.SetPixelData (BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, width, height, 96, 96, pixels);
					await encoder.FlushAsync ();

					// reset the stream cursor
					ms.Position = 0;
					return ms;
				});
			}

			return null;
		}
	}
}
