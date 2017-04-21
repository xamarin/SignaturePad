using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SignaturePad.Forms
{
	[RenderWith (typeof (SignaturePadRenderer))]
	public class SignaturePadView : View
	{
		public static readonly BindableProperty CaptionTextProperty = BindableProperty.Create (nameof (CaptionText), typeof (string), typeof (SignaturePadView), null);
		public static readonly BindableProperty CaptionTextColorProperty = BindableProperty.Create (nameof (CaptionTextColor), typeof (Color), typeof (SignaturePadView), Color.Default);
		public static readonly BindableProperty ClearTextProperty = BindableProperty.Create (nameof (ClearText), typeof (string), typeof (SignaturePadView), null);
		public static readonly BindableProperty ClearTextColorProperty = BindableProperty.Create (nameof (ClearTextColor), typeof (Color), typeof (SignaturePadView), Color.Default);
		public static readonly BindableProperty PromptTextProperty = BindableProperty.Create (nameof (PromptText), typeof (string), typeof (SignaturePadView), null);
		public static readonly BindableProperty PromptTextColorProperty = BindableProperty.Create (nameof (PromptTextColor), typeof (Color), typeof (SignaturePadView), Color.Default);
		public static readonly BindableProperty SignatureLineColorProperty = BindableProperty.Create (nameof (SignatureLineColor), typeof (Color), typeof (SignaturePadView), Color.Default);
		public static readonly BindableProperty StrokeColorProperty = BindableProperty.Create (nameof (StrokeColor), typeof (Color), typeof (SignaturePadView), Color.Default);
		public static readonly BindableProperty StrokeWidthProperty = BindableProperty.Create (nameof (StrokeWidth), typeof (float), typeof (SignaturePadView), (float)0);

		public bool IsBlank
		{
			get { return RequestIsBlank (); }
		}

		public string CaptionText
		{
			get { return (string)GetValue (CaptionTextProperty); }
			set { SetValue (CaptionTextProperty, value); }
		}

		public Color CaptionTextColor
		{
			get { return (Color)GetValue (CaptionTextColorProperty); }
			set { SetValue (CaptionTextColorProperty, value); }
		}

		public string ClearText
		{
			get { return (string)GetValue (ClearTextProperty); }
			set { SetValue (ClearTextProperty, value); }
		}

		public Color ClearTextColor
		{
			get { return (Color)GetValue (ClearTextColorProperty); }
			set { SetValue (ClearTextColorProperty, value); }
		}

		public string PromptText
		{
			get { return (string)GetValue (PromptTextProperty); }
			set { SetValue (PromptTextProperty, value); }
		}

		public Color PromptTextColor
		{
			get { return (Color)GetValue (PromptTextColorProperty); }
			set { SetValue (PromptTextColorProperty, value); }
		}

		public Color SignatureLineColor
		{
			get { return (Color)GetValue (SignatureLineColorProperty); }
			set { SetValue (SignatureLineColorProperty, value); }
		}

		public float StrokeWidth
		{
			get { return (float)GetValue (StrokeWidthProperty); }
			set { SetValue (StrokeWidthProperty, value); }
		}

		public Color StrokeColor
		{
			get { return (Color)GetValue (StrokeColorProperty); }
			set { SetValue (StrokeColorProperty, value); }
		}

		public IEnumerable<Point> Points
		{
			get { return GetSignaturePoints (); }
			set { SetSignaturePoints (value); }
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
		public Task<Stream> GetImageStreamAsync (SignatureImageFormat format, Size size, bool shouldCrop = true, bool keepAspectRatio = true)
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
		public Task<Stream> GetImageStreamAsync (SignatureImageFormat format, Color strokeColor, bool shouldCrop = true, bool keepAspectRatio = true)
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
		public Task<Stream> GetImageStreamAsync (SignatureImageFormat format, Color strokeColor, Size size, bool shouldCrop = true, bool keepAspectRatio = true)
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
		public Task<Stream> GetImageStreamAsync (SignatureImageFormat format, Color strokeColor, float scale, bool shouldCrop = true, bool keepAspectRatio = true)
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
		public Task<Stream> GetImageStreamAsync (SignatureImageFormat format, Color strokeColor, Color fillColor, bool shouldCrop = true, bool keepAspectRatio = true)
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
		public Task<Stream> GetImageStreamAsync (SignatureImageFormat format, Color strokeColor, Color fillColor, Size size, bool shouldCrop = true, bool keepAspectRatio = true)
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
		public Task<Stream> GetImageStreamAsync (SignatureImageFormat format, Color strokeColor, Color fillColor, float scale, bool shouldCrop = true, bool keepAspectRatio = true)
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
		public Task<Stream> GetImageStreamAsync (SignatureImageFormat imageFormat, ImageConstructionSettings settings)
		{
			var args = new ImageStreamRequestedEventArgs (imageFormat, settings);
			ImageStreamRequested?.Invoke (this, args);
			return args.ImageStreamTask;
		}

		public void Clear ()
		{
			ClearRequested?.Invoke (this, null);
		}

		private IEnumerable<Point> GetSignaturePoints ()
		{
			var args = new PointsEventArgs ();
			PointsRequested?.Invoke (this, args);
			return args.Points;
		}

		private void SetSignaturePoints (IEnumerable<Point> points)
		{
			PointsSpecified?.Invoke (this, new PointsEventArgs { Points = points });
		}

		private bool RequestIsBlank ()
		{
			var args = new IsBlankRequestedEventArgs ();
			IsBlankRequested?.Invoke (this, args);
			return args.IsBlank;
		}

		internal event EventHandler<ImageStreamRequestedEventArgs> ImageStreamRequested;
		internal event EventHandler<IsBlankRequestedEventArgs> IsBlankRequested;
		internal event EventHandler<PointsEventArgs> PointsRequested;
		internal event EventHandler<PointsEventArgs> PointsSpecified;
		internal event EventHandler<EventArgs> ClearRequested;

		internal class ImageStreamRequestedEventArgs : EventArgs
		{
			public ImageStreamRequestedEventArgs (SignatureImageFormat imageFormat, ImageConstructionSettings settings)
			{
				ImageFormat = imageFormat;
				Settings = settings;
			}

			public SignatureImageFormat ImageFormat { get; private set; }

			public ImageConstructionSettings Settings { get; private set; }

			public Task<Stream> ImageStreamTask { get; set; } = Task.FromResult<Stream> (null);
		}

		internal class IsBlankRequestedEventArgs : EventArgs
		{
			public bool IsBlank { get; set; } = true;
		}

		internal class PointsEventArgs : EventArgs
		{
			public IEnumerable<Point> Points { get; set; } = new Point[0];
		}
	}
}
