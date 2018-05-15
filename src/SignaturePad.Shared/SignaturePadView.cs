using System;
using System.IO;
using System.Threading.Tasks;

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
#elif WINDOWS_UWP
using NativeRect = Windows.Foundation.Rect;
using NativePoint = Windows.Foundation.Point;
using NativeSize = Windows.Foundation.Size;
using NativeColor = Windows.UI.Color;
using NativeImage = Windows.UI.Xaml.Media.Imaging.WriteableBitmap;
#endif


namespace Xamarin.Controls
{
#if WINDOWS_UWP
	partial class SignaturePad
#else
	partial class SignaturePadView
#endif
	{
		private const double DefaultWideSpacing = 12.0;
		private const double DefaultNarrowSpacing = 3.0;
		private const double DefaultLineThickness = 1.0;

		private const double DefaultFontSize = 15.0;

		private const string DefaultClearLabelText = "clear";
		private const string DefaultPromptText = "▶";
		private const string DefaultCaptionText = "sign above the line";

#if __IOS__
		private static readonly NativeColor SignaturePadDarkColor = NativeColor.FromRGBA (184, 134, 11, 255);
		private static readonly NativeColor SignaturePadLightColor = NativeColor.FromRGBA (250, 250, 210, 255);
#elif __ANDROID__
		private static readonly NativeColor SignaturePadDarkColor = NativeColor.Argb (255, 184, 134, 11);
		private static readonly NativeColor SignaturePadLightColor = NativeColor.Argb (255, 250, 250, 210);
#elif WINDOWS_UWP
		private static readonly NativeColor SignaturePadDarkColor = NativeColor.FromArgb (255, 184, 134, 11);
		private static readonly NativeColor SignaturePadLightColor = NativeColor.FromArgb (255, 250, 250, 210);
#endif

		public NativePoint[][] Strokes => SignaturePadCanvas.Strokes;

		public NativePoint[] Points => SignaturePadCanvas.Points;

		public bool IsBlank => SignaturePadCanvas?.IsBlank ?? true;

		public event EventHandler StrokeCompleted;

		public event EventHandler Cleared;

		public void Clear ()
		{
			SignaturePadCanvas.Clear ();

			UpdateUi ();
		}

		public void LoadPoints (NativePoint[] points)
		{
			SignaturePadCanvas.LoadPoints (points);

			UpdateUi ();
		}

		public void LoadStrokes (NativePoint[][] strokes)
		{
			SignaturePadCanvas.LoadStrokes (strokes);

			UpdateUi ();
		}

		/// <summary>
		/// Create an image of the currently drawn signature.
		/// </summary>
		public NativeImage GetImage (bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return SignaturePadCanvas.GetImage (shouldCrop, keepAspectRatio);
		}

		/// <summary>
		/// Create an image of the currently drawn signature at the specified size.
		/// </summary>
		public NativeImage GetImage (NativeSize size, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return SignaturePadCanvas.GetImage (size, shouldCrop, keepAspectRatio);
		}

		/// <summary>
		/// Create an image of the currently drawn signature at the specified scale.
		/// </summary>
		public NativeImage GetImage (float scale, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return SignaturePadCanvas.GetImage (scale, shouldCrop, keepAspectRatio);
		}

		/// <summary>
		/// Create an image of the currently drawn signature with the specified stroke color.
		/// </summary>
		public NativeImage GetImage (NativeColor strokeColor, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return SignaturePadCanvas.GetImage (strokeColor, shouldCrop, keepAspectRatio);
		}

		/// <summary>
		/// Create an image of the currently drawn signature at the specified size with the specified stroke color.
		/// </summary>
		public NativeImage GetImage (NativeColor strokeColor, NativeSize size, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return SignaturePadCanvas.GetImage (strokeColor, size, shouldCrop, keepAspectRatio);
		}

		/// <summary>
		/// Create an image of the currently drawn signature at the specified scale with the specified stroke color.
		/// </summary>
		public NativeImage GetImage (NativeColor strokeColor, float scale, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return SignaturePadCanvas.GetImage (strokeColor, scale, shouldCrop, keepAspectRatio);
		}

		/// <summary>
		/// Create an image of the currently drawn signature with the specified stroke and background colors.
		/// </summary>
		public NativeImage GetImage (NativeColor strokeColor, NativeColor fillColor, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return SignaturePadCanvas.GetImage (strokeColor, fillColor, shouldCrop, keepAspectRatio);
		}

		/// <summary>
		/// Create an image of the currently drawn signature at the specified size with the specified stroke and background colors.
		/// </summary>
		public NativeImage GetImage (NativeColor strokeColor, NativeColor fillColor, NativeSize size, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return SignaturePadCanvas.GetImage (strokeColor, fillColor, size, shouldCrop, keepAspectRatio);
		}

		/// <summary>
		/// Create an image of the currently drawn signature at the specified scale with the specified stroke and background colors.
		/// </summary>
		public NativeImage GetImage (NativeColor strokeColor, NativeColor fillColor, float scale, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return SignaturePadCanvas.GetImage (strokeColor, fillColor, scale, shouldCrop, keepAspectRatio);
		}

		/// <summary>
		/// Create an image of the currently drawn signature using the specified settings.
		/// </summary>
		public NativeImage GetImage (ImageConstructionSettings settings)
		{
			return SignaturePadCanvas.GetImage (settings);
		}

		/// <summary>
		/// Create an encoded image of the currently drawn signature.
		/// </summary>
		public Task<Stream> GetImageStreamAsync (SignatureImageFormat format, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return SignaturePadCanvas.GetImageStreamAsync (format, shouldCrop, keepAspectRatio);
		}

		/// <summary>
		/// Create an encoded image of the currently drawn signature at the specified size.
		/// </summary>
		public Task<Stream> GetImageStreamAsync (SignatureImageFormat format, NativeSize size, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return SignaturePadCanvas.GetImageStreamAsync (format, size, shouldCrop, keepAspectRatio);
		}

		/// <summary>
		/// Create an encoded image of the currently drawn signature at the specified scale.
		/// </summary>
		public Task<Stream> GetImageStreamAsync (SignatureImageFormat format, float scale, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return SignaturePadCanvas.GetImageStreamAsync (format, scale, shouldCrop, keepAspectRatio);
		}

		/// <summary>
		/// Create an encoded image of the currently drawn signature with the specified stroke color.
		/// </summary>
		public Task<Stream> GetImageStreamAsync (SignatureImageFormat format, NativeColor strokeColor, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return SignaturePadCanvas.GetImageStreamAsync (format, strokeColor, shouldCrop, keepAspectRatio);
		}

		/// <summary>
		/// Create an encoded image of the currently drawn signature at the specified size with the specified stroke color.
		/// </summary>
		public Task<Stream> GetImageStreamAsync (SignatureImageFormat format, NativeColor strokeColor, NativeSize size, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return SignaturePadCanvas.GetImageStreamAsync (format, strokeColor, size, shouldCrop, keepAspectRatio);
		}

		/// <summary>
		/// Create an encoded image of the currently drawn signature at the specified scale with the specified stroke color.
		/// </summary>
		public Task<Stream> GetImageStreamAsync (SignatureImageFormat format, NativeColor strokeColor, float scale, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return SignaturePadCanvas.GetImageStreamAsync (format, strokeColor, scale, shouldCrop, keepAspectRatio);
		}

		/// <summary>
		/// Create an encoded image of the currently drawn signature with the specified stroke and background colors.
		/// </summary>
		public Task<Stream> GetImageStreamAsync (SignatureImageFormat format, NativeColor strokeColor, NativeColor fillColor, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return SignaturePadCanvas.GetImageStreamAsync (format, strokeColor, fillColor, shouldCrop, keepAspectRatio);
		}

		/// <summary>
		/// Create an encoded image of the currently drawn signature at the specified size with the specified stroke and background colors.
		/// </summary>
		public Task<Stream> GetImageStreamAsync (SignatureImageFormat format, NativeColor strokeColor, NativeColor fillColor, NativeSize size, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return SignaturePadCanvas.GetImageStreamAsync (format, strokeColor, fillColor, size, shouldCrop, keepAspectRatio);
		}

		/// <summary>
		/// Create an encoded image of the currently drawn signature at the specified scale with the specified stroke and background colors.
		/// </summary>
		public Task<Stream> GetImageStreamAsync (SignatureImageFormat format, NativeColor strokeColor, NativeColor fillColor, float scale, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return SignaturePadCanvas.GetImageStreamAsync (format, strokeColor, fillColor, scale, shouldCrop, keepAspectRatio);
		}

		/// <summary>
		/// Create an encoded image of the currently drawn signature using the specified settings.
		/// </summary>
		public Task<Stream> GetImageStreamAsync (SignatureImageFormat format, ImageConstructionSettings settings)
		{
			return SignaturePadCanvas.GetImageStreamAsync (format, settings);
		}

		private void OnClearTapped ()
		{
			Clear ();
		}

		private void OnSignatureCleared ()
		{
			UpdateUi ();
			Cleared?.Invoke (this, EventArgs.Empty);
		}

		private void OnSignatureStrokeCompleted ()
		{
			UpdateUi ();
			StrokeCompleted?.Invoke (this, EventArgs.Empty);
		}
	}
}
