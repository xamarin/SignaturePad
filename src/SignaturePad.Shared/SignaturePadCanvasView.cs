﻿using System;
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
#elif __MACOS__
using NativeRect = CoreGraphics.CGRect;
using NativePoint = CoreGraphics.CGPoint;
using NativeSize = CoreGraphics.CGSize;
using NativeColor = AppKit.NSColor;
using NativeImage = AppKit.NSImage;
#elif WINDOWS_PHONE
using NativeRect = System.Windows.Rect;
using NativePoint = System.Windows.Point;
using NativeSize = System.Windows.Size;
using NativeColor = System.Windows.Media.Color;
using NativeImage = System.Windows.Media.Imaging.WriteableBitmap;
#elif WINDOWS_UWP || WINDOWS_APP
using NativeRect = Windows.Foundation.Rect;
using NativePoint = Windows.Foundation.Point;
using NativeSize = Windows.Foundation.Size;
using NativeColor = Windows.UI.Color;
using NativeImage = Windows.UI.Xaml.Media.Imaging.WriteableBitmap;
#elif WINDOWS_PHONE_APP
using NativeRect = Windows.Foundation.Rect;
using NativeSize = Windows.Foundation.Size;
using NativePoint = Windows.Foundation.Point;
using NativeColor = Windows.UI.Color;
using NativeImage = Windows.UI.Xaml.Media.Imaging.WriteableBitmap;
#elif GTK
using NativeRect = System.Drawing.RectangleF;
using NativePoint = Gdk.Point;
using NativeSize = System.Drawing.SizeF;
using NativeColor = Gdk.Color;
using NativeImage = System.Drawing.Bitmap;
using System.Windows.Ink;
using System.Windows.Input;
#elif WPF
using NativeRect = System.Drawing.RectangleF;
using NativePoint = System.Windows.Input.StylusPoint;
using NativeSize = System.Drawing.SizeF;
using NativeColor = System.Windows.Media.Color;
using NativeImage = System.Drawing.Bitmap;
using System.Windows.Ink;
using System.Windows.Input;
#endif

namespace Xamarin.Controls
{
	partial class SignaturePadCanvasView
	{
		public event EventHandler StrokeCompleted;

		public event EventHandler Cleared;

		public bool IsBlank => inkPresenter == null || inkPresenter.GetStrokes ().Count == 0;

#if WPF
		private bool isSingleLine;

		public bool IsSingleLine
		{
			get => isSingleLine;
			set
			{
				if (value && isSingleLine != true)
				{
					inkPresenter.PreviewMouseDown += InkPresenter_MouseDown;
				}
				else
				{
					inkPresenter.PreviewMouseDown -= InkPresenter_MouseDown;
				}

				isSingleLine = value;
			}
		}

		private void InkPresenter_MouseDown (object sender, MouseButtonEventArgs e)
		{
			Clear();
		}
#elif WINDOWS_UWP || GTK
		public bool IsSingleLine { get; set; }
#else
		public bool IsSingleLine
		{
			get => inkPresenter.IsSingleLine;
			set => inkPresenter.IsSingleLine = value;
		}
#endif


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
					.Skip (1) // skip the first empty
					.ToArray ();
			}
		}

#if WPF
		public new StrokeCollection Strokes
		{
			get
			{
				if (IsBlank)
				{
					return new StrokeCollection (new List<Stroke> ());
				}

				// make a deep copy
				var points = inkPresenter.GetStrokes ().Select (s => s.GetPoints ());
				var strokes = points.Select (point => new Stroke (new StylusPointCollection (point))).ToList ();
				var col = new StrokeCollection (strokes);
				return col;
			}
		}

#else
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
#endif

		public NativeRect GetSignatureBounds (float padding = 5f)
		{
			if (IsBlank)
			{
				return NativeRect.Empty;
			}

			var size = this.GetSize ();
			double xMin = size.Width, xMax = 0, yMin = size.Height, yMax = 0;
			foreach (var point in inkPresenter.GetStrokes ().SelectMany (stroke => stroke.GetPoints ()))
			{
				xMin = point.X <= 0 ? 0 : Math.Min (xMin, point.X);
				yMin = point.Y <= 0 ? 0 : Math.Min (yMin, point.Y);
				xMax = point.X >= size.Width ? size.Width : Math.Max (xMax, point.X);
				yMax = point.Y >= size.Height ? size.Height : Math.Max (yMax, point.Y);
			}

			var spacing = (StrokeWidth / 2f) + padding;
			xMin = Math.Max (0, xMin - spacing);
			yMin = Math.Max (0, yMin - spacing);
			xMax = Math.Min (size.Width, xMax + spacing);
			yMax = Math.Min (size.Height, yMax + spacing);

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
				DesiredSizeOrScale = new SizeOrScale (1f, SizeOrScaleType.Scale, keepAspectRatio)
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
				DesiredSizeOrScale = new SizeOrScale (size, SizeOrScaleType.Size, keepAspectRatio)
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
				DesiredSizeOrScale = new SizeOrScale (scale, SizeOrScaleType.Scale, keepAspectRatio)
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
				DesiredSizeOrScale = new SizeOrScale (1f, SizeOrScaleType.Scale, keepAspectRatio),
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
				StrokeColor = strokeColor,
				DesiredSizeOrScale = new SizeOrScale (size, SizeOrScaleType.Size, keepAspectRatio)
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
				StrokeColor = strokeColor,
				DesiredSizeOrScale = new SizeOrScale (scale, SizeOrScaleType.Scale, keepAspectRatio)
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
				DesiredSizeOrScale = new SizeOrScale (1f, SizeOrScaleType.Scale, keepAspectRatio),
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
				StrokeColor = strokeColor,
				BackgroundColor = fillColor,
				DesiredSizeOrScale = new SizeOrScale (size, SizeOrScaleType.Size, keepAspectRatio)
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
				StrokeColor = strokeColor,
				BackgroundColor = fillColor,
				DesiredSizeOrScale = new SizeOrScale (scale, SizeOrScaleType.Scale, keepAspectRatio)
			});
		}

		/// <summary>
		/// Create an image of the currently drawn signature using the specified settings.
		/// </summary>
		public NativeImage GetImage (ImageConstructionSettings settings)
		{
			NativeSize scale;
			NativeRect signatureBounds;
			NativeSize imageSize;
			float strokeWidth;
			NativeColor strokeColor;
			NativeColor backgroundColor;

			if (GetImageConstructionArguments (settings, out scale, out signatureBounds, out imageSize, out strokeWidth, out strokeColor, out backgroundColor))
			{
				return GetImageInternal (scale, signatureBounds, imageSize, strokeWidth, strokeColor, backgroundColor);
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
				DesiredSizeOrScale = new SizeOrScale (1f, SizeOrScaleType.Scale, keepAspectRatio)
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
				DesiredSizeOrScale = new SizeOrScale (size, SizeOrScaleType.Size, keepAspectRatio)
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
				DesiredSizeOrScale = new SizeOrScale (scale, SizeOrScaleType.Scale, keepAspectRatio)
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
				DesiredSizeOrScale = new SizeOrScale (1f, SizeOrScaleType.Scale, keepAspectRatio),
				StrokeColor = strokeColor
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
				StrokeColor = strokeColor,
				DesiredSizeOrScale = new SizeOrScale (size, SizeOrScaleType.Size, keepAspectRatio)
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
				StrokeColor = strokeColor,
				DesiredSizeOrScale = new SizeOrScale (scale, SizeOrScaleType.Scale, keepAspectRatio)
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
				DesiredSizeOrScale = new SizeOrScale (1f, SizeOrScaleType.Scale, keepAspectRatio),
				StrokeColor = strokeColor,
				BackgroundColor = fillColor
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
				StrokeColor = strokeColor,
				BackgroundColor = fillColor,
				DesiredSizeOrScale = new SizeOrScale (size, SizeOrScaleType.Size, keepAspectRatio)
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
				StrokeColor = strokeColor,
				BackgroundColor = fillColor,
				DesiredSizeOrScale = new SizeOrScale (scale, SizeOrScaleType.Scale, keepAspectRatio)
			});
		}

		/// <summary>
		/// Create an encoded image stream of the currently drawn signature using the specified settings.
		/// </summary>
		public Task<Stream> GetImageStreamAsync (SignatureImageFormat format, ImageConstructionSettings settings)
		{
			NativeSize scale;
			NativeRect signatureBounds;
			NativeSize imageSize;
			float strokeWidth;
			NativeColor strokeColor;
			NativeColor backgroundColor;

			if (GetImageConstructionArguments (settings, out scale, out signatureBounds, out imageSize, out strokeWidth, out strokeColor, out backgroundColor))
			{
				return GetImageStreamInternal (format, scale, signatureBounds, imageSize, strokeWidth, strokeColor, backgroundColor);
			}

			return Task.FromResult<Stream> (null);
		}

		private bool GetImageConstructionArguments (ImageConstructionSettings settings, out NativeSize scale, out NativeRect signatureBounds, out NativeSize imageSize, out float strokeWidth, out NativeColor strokeColor, out NativeColor backgroundColor)
		{
			settings.ApplyDefaults ((float)StrokeWidth, StrokeColor);

			if (IsBlank || settings.DesiredSizeOrScale?.IsValid != true)
			{
				scale = default (NativeSize);
				signatureBounds = default (NativeRect);
				imageSize = default (NativeSize);
				strokeWidth = default (float);
				strokeColor = default (NativeColor);
				backgroundColor = default (NativeColor);

				return false;
			}

			var sizeOrScale = settings.DesiredSizeOrScale.Value;

			var viewSize = this.GetSize ();

			imageSize = sizeOrScale.GetSize ((float)viewSize.Width, (float)viewSize.Height);
			scale = sizeOrScale.GetScale ((float)imageSize.Width, (float)imageSize.Height);

			if (settings.ShouldCrop == true)
			{
				signatureBounds = GetSignatureBounds (settings.Padding.Value);

				if (sizeOrScale.Type == SizeOrScaleType.Size)
				{
					// if a specific size was set, scale to that
					var scaleX = imageSize.Width / (float)signatureBounds.Width;
					var scaleY = imageSize.Height / (float)signatureBounds.Height;
					if (sizeOrScale.KeepAspectRatio)
					{
						scaleX = scaleY = Math.Min ((float)scaleX, (float)scaleY);
					}
#if WPF || GTK
					scale = new NativeSize ((int)scaleX, (int)scaleY);
				}
				else if (sizeOrScale.Type == SizeOrScaleType.Scale)
				{
					imageSize.Width = (int)(signatureBounds.Width * scale.Width);
					imageSize.Height = (int)(signatureBounds.Height * scale.Height);
				}
			}
			else
			{
				signatureBounds = new NativeRect (0, 0, (float)viewSize.Width, (float)viewSize.Height);
			}
#else
					scale = new NativeSize ((float)scaleX, (float)scaleY);
				}
				else if (sizeOrScale.Type == SizeOrScaleType.Scale)
				{
					imageSize.Width = signatureBounds.Width * scale.Width;
					imageSize.Height = signatureBounds.Height * scale.Height;
				}
			}
			else
			{
				signatureBounds = new NativeRect (0, 0, viewSize.Width, viewSize.Height);
			}
#endif

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

			inkPresenter.AddStrokes (loadedStrokes, StrokeColor, (float)StrokeWidth);

			if (!IsBlank)
			{
				OnStrokeCompleted ();
			}
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

			inkPresenter.AddStrokes (strokes, StrokeColor, (float)StrokeWidth);

			if (!IsBlank)
			{
				OnStrokeCompleted ();
			}
		}

		private void OnCleared ()
		{
			Cleared?.Invoke (this, EventArgs.Empty);
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
