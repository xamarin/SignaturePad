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
		private const float DefaultWideSpacing = 12.0f;
		private const float DefaultNarrowSpacing = 3.0f;
		private const float DefaultLineThickness = 1.0f;

		private const float DefaultFontSize = 15.0f;

		private const string DefaultClearLabelText = "clear";
		private const string DefaultPromptText = "▶";
		private const string DefaultCaptionText = "sign above the line";

#if __IOS__
		private static readonly NativeColor SignaturePadDarkColor = NativeColor.Black;
		private static readonly NativeColor SignaturePadLightColor = NativeColor.White;
#elif __ANDROID__
		private static readonly NativeColor SignaturePadDarkColor = NativeColor.Black;
		private static readonly NativeColor SignaturePadLightColor = NativeColor.White;
#elif WINDOWS_UWP
		private static readonly NativeColor SignaturePadDarkColor = Windows.UI.Colors.Black;
		private static readonly NativeColor SignaturePadLightColor = Windows.UI.Colors.White;
#endif


		/// <summary>
		/// Get currents strokes from the canvas. Strokes are represented by an array of array of points.
		/// </summary>
		public NativePoint[][] Strokes => SignaturePadCanvas.Strokes;

		/// <summary>
		/// Get currents points from the canvas. A new line of points start with coordinate (0,0)
		/// </summary>
		public NativePoint[] Points => SignaturePadCanvas.Points;

		public bool IsBlank => SignaturePadCanvas?.IsBlank ?? true;

		public event EventHandler StrokeCompleted;

		public event EventHandler Cleared;

		/// <summary>
		/// Clear the canvas and invoke <see cref="Cleared"/> event
		/// </summary>
		public void Clear ()
		{
			SignaturePadCanvas.Clear ();

			UpdateUi ();
		}

		/// <summary>
		/// Allow the user to import an array of points to be used to draw a signature in the view, with new
		/// lines indicated by a { 0, 0 } point in the array.
		/// <param name="loadedPoints"></param>
		public void LoadPoints (NativePoint[] points)
		{
			SignaturePadCanvas.LoadPoints (points);

			UpdateUi ();
		}

		/// <summary>
		/// Allow the user to import strokes represented by an array of an array of points.
		/// Strokes are used to draw a signature in the view after clearing it.
		/// </summary>
		/// <param name="loadedStrokes">Array of array of points. Each array of points represent a line.</param>
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
