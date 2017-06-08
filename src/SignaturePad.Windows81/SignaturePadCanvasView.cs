using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Numerics;
using System.Linq;

namespace Xamarin.Controls
{
	public partial class SignaturePadCanvasView : Grid
	{
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
			inkPresenter.SetValue (Grid.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
			inkPresenter.SetValue (Grid.VerticalAlignmentProperty, VerticalAlignment.Stretch);
			Children.Add (inkPresenter);

			// get some defaults
			var settings = new ImageConstructionSettings ();
			settings.ApplyDefaults ();

			StrokeWidth = settings.StrokeWidth.Value;
			StrokeColor = settings.StrokeColor.Value;
		}

		public Color StrokeColor
		{
			get { return inkPresenter.StrokeColor; }
			set { inkPresenter.StrokeColor = value; }
		}

		public float StrokeWidth
		{
			get { return inkPresenter.StrokeWidth; }
			set { inkPresenter.StrokeWidth = value; }
		}

		public void Clear ()
		{
			inkPresenter.Clear ();
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

				session.Transform = Multiply (
					CreateTranslation ((float)-signatureBounds.X, (float)-signatureBounds.Y),
					CreateScale ((float)scale.Width, (float)scale.Height));

				foreach (var stroke in inkPresenter.GetStrokes ())
				{
					var segments = stroke.GetRenderingSegments ();
					var position = segments.First ().Position;

					var builder = new CanvasPathBuilder (device);
					builder.BeginFigure ((float)position.X, (float)position.Y);
					foreach (var segment in segments.Skip (1))
					{
						builder.AddCubicBezier (
							new Vector2 { X = (float)segment.BezierControlPoint1.X, Y = (float)segment.BezierControlPoint1.Y },
							new Vector2 { X = (float)segment.BezierControlPoint2.X, Y = (float)segment.BezierControlPoint2.Y },
							new Vector2 { X = (float)segment.Position.X, Y = (float)segment.Position.Y });
					}
					builder.EndFigure (CanvasFigureLoop.Open);

					var path = CanvasGeometry.CreatePath (builder);
					var color = stroke.DrawingAttributes.Color;
					var width = (float)stroke.DrawingAttributes.Size.Width;
					session.DrawGeometry (path, color, width);
				}
			}

			return offscreen;
		}

		private static Matrix3x2 CreateTranslation (float xPosition, float yPosition)
		{
			Matrix3x2 result;

			result.M11 = 1.0f; result.M12 = 0.0f;
			result.M21 = 0.0f; result.M22 = 1.0f;

			result.M31 = xPosition;
			result.M32 = yPosition;

			return result;
		}

		private static Matrix3x2 CreateScale (float xScale, float yScale)
		{
			Matrix3x2 result;

			result.M11 = xScale; result.M12 = 0.0f;
			result.M21 = 0.0f; result.M22 = yScale;
			result.M31 = 0.0f; result.M32 = 0.0f;

			return result;
		}

		private static Matrix3x2 Multiply (Matrix3x2 value1, Matrix3x2 value2)
		{
			Matrix3x2 result;

			// First row
			result.M11 = value1.M11 * value2.M11 + value1.M12 * value2.M21;
			result.M12 = value1.M11 * value2.M12 + value1.M12 * value2.M22;

			// Second row
			result.M21 = value1.M21 * value2.M11 + value1.M22 * value2.M21;
			result.M22 = value1.M21 * value2.M12 + value1.M22 * value2.M22;

			// Third row
			result.M31 = value1.M31 * value2.M11 + value1.M32 * value2.M21 + value2.M31;
			result.M32 = value1.M31 * value2.M12 + value1.M32 * value2.M22 + value2.M32;

			return result;
		}
	}
}
