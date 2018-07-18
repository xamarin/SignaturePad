using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using AppKit;

namespace Xamarin.Controls
{
	[Register ("SignaturePadCanvasView")]
	[DesignTimeVisible (true)]
	public partial class SignaturePadCanvasView : NSView
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
				AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable,
			};
			inkPresenter.StrokeCompleted += OnStrokeCompleted;
			AddSubview (inkPresenter);

			StrokeWidth = ImageConstructionSettings.DefaultStrokeWidth;
			StrokeColor = ImageConstructionSettings.DefaultStrokeColor;
		}

		[Export ("StrokeColor"), Browsable (true)]
		public NSColor StrokeColor
		{
			get { return inkPresenter.StrokeColor; }
			set
			{
				inkPresenter.StrokeColor = value;
				foreach (var stroke in inkPresenter.GetStrokes ())
				{
					stroke.Color = value;
				}

				inkPresenter.NeedsDisplay = true;
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

				inkPresenter.NeedsDisplay = true;
			}
		}

		public void Clear ()
		{
			inkPresenter.Clear ();

			OnCleared ();
		}

		private NSImage GetImageInternal (CGSize scale, CGRect signatureBounds, CGSize imageSize, float strokeWidth, NSColor strokeColor, NSColor backgroundColor)
		{
			return null;
		}

		private Task<Stream> GetImageStreamInternal (SignatureImageFormat format, CGSize scale, CGRect signatureBounds, CGSize imageSize, float strokeWidth, NSColor strokeColor, NSColor backgroundColor)
		{
			var image = GetImageInternal (scale, signatureBounds, imageSize, strokeWidth, strokeColor, backgroundColor);
			if (image != null)
			{
				switch (format)
				{
					case SignatureImageFormat.Jpeg:
						return Task.Run (() => image.AsTiff ().AsStream ());
					case SignatureImageFormat.Png:
						return Task.Run (() => image.AsTiff ().AsStream ());
				}
			}
			return Task.FromResult<Stream> (null);
		}
	}
}
