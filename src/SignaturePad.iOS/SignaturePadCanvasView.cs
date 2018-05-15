using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Xamarin.Controls
{
	[Register ("SignaturePadCanvasView")]
	[DesignTimeVisible (true)]
	public partial class SignaturePadCanvasView : UIView
	{
		private InkPresenter inkPresenter;

		public SignaturePadCanvasView ()
		{
			Initialize ();
		}

		public SignaturePadCanvasView (NSCoder coder)
			: base (coder)
		{
			Initialize (/* ? baseProperties: false ? */);
		}

		protected SignaturePadCanvasView (IntPtr ptr)
			: base (ptr)
		{
			Initialize (false);
		}

		public SignaturePadCanvasView (CGRect frame)
			: base (frame)
		{
			Initialize ();
		}

		private void Initialize (bool baseProperties = true)
		{
			inkPresenter = new InkPresenter (Bounds)
			{
				AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight,
			};
			inkPresenter.StrokeCompleted += OnStrokeCompleted;
			AddSubview (inkPresenter);

			StrokeWidth = ImageConstructionSettings.DefaultStrokeWidth;
			StrokeColor = ImageConstructionSettings.DefaultStrokeColor;
		}

		[Export ("StrokeColor"), Browsable (true)]
		public UIColor StrokeColor
		{
			get { return inkPresenter.StrokeColor; }
			set
			{
				inkPresenter.StrokeColor = value;
				foreach (var stroke in inkPresenter.GetStrokes ())
				{
					stroke.Color = value;
				}
				inkPresenter.SetNeedsDisplay ();
			}
		}

		[Export ("StrokeWidth"), Browsable (true)]
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
				inkPresenter.SetNeedsDisplay ();
			}
		}

		public void Clear ()
		{
			inkPresenter.Clear ();

			OnCleared ();
		}

		private UIImage GetImageInternal (CGSize scale, CGRect signatureBounds, CGSize imageSize, float strokeWidth, UIColor strokeColor, UIColor backgroundColor)
		{
			UIGraphics.BeginImageContextWithOptions (imageSize, false, InkPresenter.ScreenDensity);

			// create context and set the desired options
			var context = UIGraphics.GetCurrentContext ();

			// background
			context.SetFillColor (backgroundColor.CGColor);
			context.FillRect (new CGRect (CGPoint.Empty, imageSize));

			// cropping / scaling
			context.ScaleCTM (scale.Width, scale.Height);
			context.TranslateCTM (-signatureBounds.Left, -signatureBounds.Top);

			// strokes
			context.SetStrokeColor (strokeColor.CGColor);
			context.SetLineWidth (strokeWidth);
			context.SetLineCap (CGLineCap.Round);
			context.SetLineJoin (CGLineJoin.Round);
			foreach (var path in inkPresenter.GetStrokes ())
			{
				context.AddPath (path.Path.CGPath);
			}
			context.StrokePath ();

			// get the image
			var image = UIGraphics.GetImageFromCurrentImageContext ();

			UIGraphics.EndImageContext ();

			return image;
		}

		private Task<Stream> GetImageStreamInternal (SignatureImageFormat format, CGSize scale, CGRect signatureBounds, CGSize imageSize, float strokeWidth, UIColor strokeColor, UIColor backgroundColor)
		{
			var image = GetImageInternal (scale, signatureBounds, imageSize, strokeWidth, strokeColor, backgroundColor);
			if (image != null)
			{
				if (format == SignatureImageFormat.Jpeg)
				{
					return Task.Run (() => image.AsJPEG ().AsStream ());
				}
				else if (format == SignatureImageFormat.Png)
				{
					return Task.Run (() => image.AsPNG ().AsStream ());
				}
			}
			return Task.FromResult<Stream> (null);
		}
	}
}
