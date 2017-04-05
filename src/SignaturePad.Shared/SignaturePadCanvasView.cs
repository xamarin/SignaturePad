using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

#if __ANDROID__
using NativeRect = System.Drawing.RectangleF;
using NativePoint = System.Drawing.PointF;
using NativeSize = System.Drawing.SizeF;
using NativeColor = Android.Graphics.Color;
using NativeImage = Android.Graphics.Bitmap;
#elif __IOS__
using NativeRect = CoreGraphics.CGRect;
using NativePoint = CoreGraphics.CGPoint;
using NativeSize = CoreGraphics.CGSize;
using NativeColor = UIKit.UIColor;
using NativeImage = UIKit.UIImage;
#elif WINDOWS_PHONE
using NativeRect = System.Windows.Rect;
using NativePoint = System.Windows.Point;
using NativeSize = System.Windows.Size;
using NativeColor = System.Windows.Media.Color;
using NativeImage = System.Windows.Media.Imaging.WriteableBitmap;
#elif WINDOWS_UWP
using NativeRect = Windows.Foundation.Rect;
using NativePoint = Windows.Foundation.Point;
using NativeSize = Windows.Foundation.Size;
using NativeColor = Windows.UI.Color;
using NativeImage = Windows.UI.Xaml.Media.Imaging.WriteableBitmap;
#endif

namespace Xamarin.Controls
{
	partial class SignaturePadCanvasView
	{
#if __IOS__
		private float Width => (float)Bounds.Width;

		private float Height => (float)Bounds.Height;
#endif

		public event EventHandler StrokeCompleted;

		public bool IsBlank => inkPresenter.GetStrokes ().Count == 0;

		public NativePoint[] Points
		{
			get
			{
				if (IsBlank)
				{
					return new NativePoint[0];
				}

				// make a deep copy, with { 0, 0 } line starter
				return inkPresenter.GetStrokes ()
					.SelectMany (s => new[] { new NativePoint (0, 0) }.Concat (s.GetPoints ()))
					.ToArray ();
			}
		}

		public NativePoint[][] Strokes
		{
			get
			{
				if (IsBlank)
				{
					return new NativePoint[0][];
				}

				// make a deep copy
				return inkPresenter.GetStrokes ().Select (s => s.GetPoints ().ToArray ()).ToArray ();
			}
		}

		public NativeRect GetSignatureBounds (float padding = 5f)
		{
			if (IsBlank)
			{
				return NativeRect.Empty;
			}

			double xMin = Width, xMax = 0, yMin = Height, yMax = 0;
			foreach (var point in inkPresenter.GetStrokes ().SelectMany (stroke => stroke.GetPoints ()))
			{
				xMin = point.X <= 0 ? 0 : Math.Min (xMin, point.X);
				yMin = point.Y <= 0 ? 0 : Math.Min (yMin, point.Y);
				xMax = point.X >= Width ? Width : Math.Max (xMax, point.X);
				yMax = point.Y >= Height ? Height : Math.Max (yMax, point.Y);
			}

			var spacing = (StrokeWidth / 2f) + padding;
			xMin = Math.Max (0, xMin - spacing);
			yMin = Math.Max (0, yMin - spacing);
			xMax = Math.Min (Width, xMax + spacing);
			yMax = Math.Min (Height, yMax + spacing);

			return new NativeRect (
				(float)xMin,
				(float)yMin,
				(float)xMax - (float)xMin,
				(float)yMax - (float)yMin);
		}

		/// <summary>
		/// Create an image of the currently drawn signature.
		/// </summary>
		public NativeImage GetImage (bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return GetImage (new ImageConstructionSettings
			{
				ShouldCrop = shouldCrop,
				ShouldKeepAspectRatio = keepAspectRatio,
			});
		}

		/// <summary>
		/// Create an image of the currently drawn signature at the specified size.
		/// </summary>
		public NativeImage GetImage (NativeSize size, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return GetImage (new ImageConstructionSettings
			{
				ShouldCrop = shouldCrop,
				ShouldKeepAspectRatio = keepAspectRatio,
				DesiredSizeOrScale = size,
			});
		}

		/// <summary>
		/// Create an image of the currently drawn signature at the specified scale.
		/// </summary>
		public NativeImage GetImage (float scale, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return GetImage (new ImageConstructionSettings
			{
				ShouldCrop = shouldCrop,
				ShouldKeepAspectRatio = keepAspectRatio,
				DesiredSizeOrScale = scale,
			});
		}

		/// <summary>
		/// Create an image of the currently drawn signature with the specified stroke color.
		/// </summary>
		public NativeImage GetImage (NativeColor strokeColor, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return GetImage (new ImageConstructionSettings
			{
				ShouldCrop = shouldCrop,
				ShouldKeepAspectRatio = keepAspectRatio,
				StrokeColor = strokeColor,
			});
		}

		/// <summary>
		/// Create an image of the currently drawn signature at the specified size with the specified stroke color.
		/// </summary>
		public NativeImage GetImage (NativeColor strokeColor, NativeSize size, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return GetImage (new ImageConstructionSettings
			{
				ShouldCrop = shouldCrop,
				ShouldKeepAspectRatio = keepAspectRatio,
				StrokeColor = strokeColor,
				DesiredSizeOrScale = size,
			});
		}

		/// <summary>
		/// Create an image of the currently drawn signature at the specified scale with the specified stroke color.
		/// </summary>
		public NativeImage GetImage (NativeColor strokeColor, float scale, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return GetImage (new ImageConstructionSettings
			{
				ShouldCrop = shouldCrop,
				ShouldKeepAspectRatio = keepAspectRatio,
				StrokeColor = strokeColor,
				DesiredSizeOrScale = scale,
			});
		}

		/// <summary>
		/// Create an image of the currently drawn signature with the specified stroke and background colors.
		/// </summary>
		public NativeImage GetImage (NativeColor strokeColor, NativeColor fillColor, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return GetImage (new ImageConstructionSettings
			{
				ShouldCrop = shouldCrop,
				ShouldKeepAspectRatio = keepAspectRatio,
				StrokeColor = strokeColor,
				BackgroundColor = fillColor,
			});
		}

		/// <summary>
		/// Create an image of the currently drawn signature at the specified size with the specified stroke and background colors.
		/// </summary>
		public NativeImage GetImage (NativeColor strokeColor, NativeColor fillColor, NativeSize size, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return GetImage (new ImageConstructionSettings
			{
				ShouldCrop = shouldCrop,
				ShouldKeepAspectRatio = keepAspectRatio,
				StrokeColor = strokeColor,
				BackgroundColor = fillColor,
				DesiredSizeOrScale = size,
			});
		}

		/// <summary>
		/// Create an image of the currently drawn signature at the specified scale with the specified stroke and background colors.
		/// </summary>
		public NativeImage GetImage (NativeColor strokeColor, NativeColor fillColor, float scale, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return GetImage (new ImageConstructionSettings
			{
				ShouldCrop = shouldCrop,
				ShouldKeepAspectRatio = keepAspectRatio,
				StrokeColor = strokeColor,
				BackgroundColor = fillColor,
				DesiredSizeOrScale = scale,
			});
		}

		/// <summary>
		/// Create an image of the currently drawn signature using the specified settings.
		/// </summary>
		public NativeImage GetImage (ImageConstructionSettings settings)
		{
			float scale;
			NativeRect croppedRectangle;
			NativeSize imageSize;
			float strokeWidth;
			NativeColor strokeColor;
			NativeColor backgroundColor;

			if (GetImageConstructionArguments (settings, out scale, out croppedRectangle, out imageSize, out strokeWidth, out strokeColor, out backgroundColor))
			{
				return GetImageInternal (scale, croppedRectangle, imageSize, strokeWidth, strokeColor, backgroundColor);
			}

			return null;
		}

		/// <summary>
		/// Create an encoded image stream of the currently drawn signature.
		/// </summary>
		public Task<Stream> GetImageStreamAsync (SignatureImageFormat format, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return GetImageStreamAsync (format, new ImageConstructionSettings
			{
				ShouldCrop = shouldCrop,
				ShouldKeepAspectRatio = keepAspectRatio,
			});
		}

		/// <summary>
		/// Create an encoded image stream of the currently drawn signature at the specified size.
		/// </summary>
		public Task<Stream> GetImageStreamAsync (SignatureImageFormat format, NativeSize size, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return GetImageStreamAsync (format, new ImageConstructionSettings
			{
				ShouldCrop = shouldCrop,
				ShouldKeepAspectRatio = keepAspectRatio,
				DesiredSizeOrScale = size,
			});
		}

		/// <summary>
		/// Create an encoded image stream of the currently drawn signature at the specified scale.
		/// </summary>
		public Task<Stream> GetImageStreamAsync (SignatureImageFormat format, float scale, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return GetImageStreamAsync (format, new ImageConstructionSettings
			{
				ShouldCrop = shouldCrop,
				ShouldKeepAspectRatio = keepAspectRatio,
				DesiredSizeOrScale = scale,
			});
		}

		/// <summary>
		/// Create an encoded image stream of the currently drawn signature with the specified stroke color.
		/// </summary>
		public Task<Stream> GetImageStreamAsync (SignatureImageFormat format, NativeColor strokeColor, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return GetImageStreamAsync (format, new ImageConstructionSettings
			{
				ShouldCrop = shouldCrop,
				ShouldKeepAspectRatio = keepAspectRatio,
				StrokeColor = strokeColor,
			});
		}

		/// <summary>
		/// Create an encoded image stream of the currently drawn signature at the specified size with the specified stroke color.
		/// </summary>
		public Task<Stream> GetImageStreamAsync (SignatureImageFormat format, NativeColor strokeColor, NativeSize size, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return GetImageStreamAsync (format, new ImageConstructionSettings
			{
				ShouldCrop = shouldCrop,
				ShouldKeepAspectRatio = keepAspectRatio,
				StrokeColor = strokeColor,
				DesiredSizeOrScale = size,
			});
		}

		/// <summary>
		/// Create an encoded image stream of the currently drawn signature at the specified scale with the specified stroke color.
		/// </summary>
		public Task<Stream> GetImageStreamAsync (SignatureImageFormat format, NativeColor strokeColor, float scale, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return GetImageStreamAsync (format, new ImageConstructionSettings
			{
				ShouldCrop = shouldCrop,
				ShouldKeepAspectRatio = keepAspectRatio,
				StrokeColor = strokeColor,
				DesiredSizeOrScale = scale,
			});
		}

		/// <summary>
		/// Create an encoded image stream of the currently drawn signature with the specified stroke and background colors.
		/// </summary>
		public Task<Stream> GetImageStreamAsync (SignatureImageFormat format, NativeColor strokeColor, NativeColor fillColor, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return GetImageStreamAsync (format, new ImageConstructionSettings
			{
				ShouldCrop = shouldCrop,
				ShouldKeepAspectRatio = keepAspectRatio,
				StrokeColor = strokeColor,
				BackgroundColor = fillColor,
			});
		}

		/// <summary>
		/// Create an encoded image stream of the currently drawn signature at the specified size with the specified stroke and background colors.
		/// </summary>
		public Task<Stream> GetImageStreamAsync (SignatureImageFormat format, NativeColor strokeColor, NativeColor fillColor, NativeSize size, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return GetImageStreamAsync (format, new ImageConstructionSettings
			{
				ShouldCrop = shouldCrop,
				ShouldKeepAspectRatio = keepAspectRatio,
				StrokeColor = strokeColor,
				BackgroundColor = fillColor,
				DesiredSizeOrScale = size,
			});
		}

		/// <summary>
		/// Create an encoded image stream of the currently drawn signature at the specified scale with the specified stroke and background colors.
		/// </summary>
		public Task<Stream> GetImageStreamAsync (SignatureImageFormat format, NativeColor strokeColor, NativeColor fillColor, float scale, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return GetImageStreamAsync (format, new ImageConstructionSettings
			{
				ShouldCrop = shouldCrop,
				ShouldKeepAspectRatio = keepAspectRatio,
				StrokeColor = strokeColor,
				BackgroundColor = fillColor,
				DesiredSizeOrScale = scale,
			});
		}

		/// <summary>
		/// Create an encoded image stream of the currently drawn signature using the specified settings.
		/// </summary>
		public Task<Stream> GetImageStreamAsync (SignatureImageFormat format, ImageConstructionSettings settings)
		{
			float scale;
			NativeRect croppedRectangle;
			NativeSize imageSize;
			float strokeWidth;
			NativeColor strokeColor;
			NativeColor backgroundColor;

			if (GetImageConstructionArguments (settings, out scale, out croppedRectangle, out imageSize, out strokeWidth, out strokeColor, out backgroundColor))
			{
				return GetImageStreamInternal (format, scale, croppedRectangle, imageSize, strokeWidth, strokeColor, backgroundColor);
			}

			return Task.FromResult<Stream> (null);
		}

		private bool GetImageConstructionArguments (ImageConstructionSettings settings, out float scale, out NativeRect croppedRectangle, out NativeSize imageSize, out float strokeWidth, out NativeColor strokeColor, out NativeColor backgroundColor)
		{
			settings.ApplyDefaults (StrokeWidth, StrokeColor);

			if (IsBlank || settings.DesiredSizeOrScale?.IsValid != true)
			{
				scale = default (float);
				croppedRectangle = default (NativeRect);
				imageSize = default (NativeSize);
				strokeWidth = default (float);
				strokeColor = default (NativeColor);
				backgroundColor = default (NativeColor);

				return false;
			}

			var sizeOrScale = settings.DesiredSizeOrScale.Value;
			var size = sizeOrScale.GetSize ((float)Width, (float)Height);
			scale = sizeOrScale.GetScale ((float)Width, (float)Height);

			croppedRectangle = NativeRect.Empty;
			if (settings.ShouldCrop == true)
			{
				croppedRectangle = GetSignatureBounds ();
				var scaleX = Width / (float)croppedRectangle.Width;
				var scaleY = Height / (float)croppedRectangle.Height;
				scale = Math.Min ((float)scaleX, (float)scaleY);
			}

			imageSize = size;
			if (settings.ShouldKeepAspectRatio != true)
			{
				imageSize = new NativeSize (croppedRectangle.Width * scale, croppedRectangle.Height * scale);
			}

			strokeWidth = settings.StrokeWidth.Value;
			strokeColor = (NativeColor)settings.StrokeColor;
			backgroundColor = (NativeColor)settings.BackgroundColor;

			return true;
		}

		public void LoadStrokes (NativePoint[][] loadedStrokes)
		{
			// clear any existing paths or points.
			Clear ();

			// there is nothing
			if (loadedStrokes == null || loadedStrokes.Length == 0)
			{
				return;
			}

			inkPresenter.AddStrokes (loadedStrokes, StrokeColor, StrokeWidth);
		}

		/// <summary>
		/// Allow the user to import an array of points to be used to draw a signature in the view, with new
		/// lines indicated by a { 0, 0 } point in the array.
		/// <param name="loadedPoints"></param>
		public void LoadPoints (NativePoint[] loadedPoints)
		{
			// clear any existing paths or points.
			Clear ();

			// there is nothing
			if (loadedPoints == null || loadedPoints.Length == 0)
			{
				return;
			}

			var startIndex = 0;

			var emptyIndex = Array.IndexOf (loadedPoints, new NativePoint (0, 0));
			if (emptyIndex == -1)
			{
				emptyIndex = loadedPoints.Length;
			}

			var strokes = new List<NativePoint[]> ();

			do
			{
				// add a stroke to the ink presenter
				var currentStroke = new NativePoint[emptyIndex - startIndex];
				strokes.Add (currentStroke);
				Array.Copy (loadedPoints, startIndex, currentStroke, 0, currentStroke.Length);

				// obtain the indices for the next line to be drawn.
				startIndex = emptyIndex + 1;
				if (startIndex < loadedPoints.Length - 1)
				{
					emptyIndex = Array.IndexOf (loadedPoints, new NativePoint (0, 0), startIndex);
					if (emptyIndex == -1)
					{
						emptyIndex = loadedPoints.Length;
					}
				}
				else
				{
					emptyIndex = startIndex;
				}
			}
			while (startIndex < emptyIndex);

			inkPresenter.AddStrokes (strokes, StrokeColor, StrokeWidth);
		}

		private void OnStrokeCompleted ()
		{
			OnStrokeCompleted (this, EventArgs.Empty);
		}

		private void OnStrokeCompleted (object sender, EventArgs e)
		{
			StrokeCompleted?.Invoke (this, e);
		}
	}
}
