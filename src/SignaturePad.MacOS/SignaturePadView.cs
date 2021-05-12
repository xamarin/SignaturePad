using System;
using System.ComponentModel;
using CoreGraphics;
using Foundation;
using AppKit;

namespace Xamarin.Controls
{
	[Register ("SignaturePadView")]
	[DesignTimeVisible (true)]
	public partial class SignaturePadView : NSView
	{
		private NSEdgeInsets padding;

		public SignaturePadView ()
		{
			Initialize ();
		}

		public SignaturePadView (NSCoder coder)
			: base (coder)
		{
			Initialize (/* ? baseProperties: false ? */);
		}

		public SignaturePadView (IntPtr ptr)
			: base (ptr)
		{
			Initialize (false);
		}

		public SignaturePadView (CGRect frame)
			: base (frame)
		{
			Initialize ();
		}

		private void Initialize (bool baseProperties = true)
		{
			// add the background view
			{
				BackgroundImageView = new NSImageView ();
				AddSubview (BackgroundImageView);
			}

			// add the main signature view
			{
				SignaturePadCanvas = new SignaturePadCanvasView ();
				SignaturePadCanvas.StrokeCompleted += delegate
				{
					OnSignatureStrokeCompleted ();
				};
				SignaturePadCanvas.Cleared += delegate
				{
					OnSignatureCleared ();
				};
				AddSubview (SignaturePadCanvas);
			}

			// add the caption
			{
				Caption = new NSTextField
				{
					BackgroundColor = NSColor.Clear,
					Alignment = NSTextAlignment.Center,
					Font = NSFont.SystemFontOfSize (DefaultFontSize),
					StringValue = DefaultCaptionText,
					TextColor = SignaturePadDarkColor,
				};
				AddSubview (Caption);
			}

			// add the signature line
			{
				SignatureLine = new NSView
				{
					//BackgroundColor = SignaturePadDarkColor,
				};
				SignatureLineWidth = DefaultLineThickness;
				SignatureLineSpacing = DefaultNarrowSpacing;
				AddSubview (SignatureLine);
			}

			// add the prompt
			{
				SignaturePrompt = new NSTextField ()
				{
					BackgroundColor = NSColor.Clear,
					Font = NSFont.BoldSystemFontOfSize (DefaultFontSize),
					StringValue = DefaultPromptText,
					TextColor = SignaturePadDarkColor,
				};
				AddSubview (SignaturePrompt);
			}

			// add the clear label
			{
				ClearLabel = NSButton.CreateButton(DefaultClearLabelText,OnClearTapped);
				//ClearLabel.Layer.BackgroundColor = CGColor.Clear;
				ClearLabel.Font = NSFont.BoldSystemFontOfSize (DefaultFontSize);
				ClearLabel.BezelColor=SignaturePadDarkColor;
				AddSubview (ClearLabel);

				// attach the "clear" command
				//ClearLabel.Activated += delegate
				//{
				//	OnClearTapped ();
				//};
			}

			Padding = new NSEdgeInsets (DefaultWideSpacing, DefaultWideSpacing, DefaultNarrowSpacing, DefaultWideSpacing);

			// clear / initialize the view
			UpdateUi ();
		}

		public SignaturePadCanvasView SignaturePadCanvas { get; private set; }

		/// <summary>
		/// Gets the horizontal line that goes in the lower part of the pad.
		/// </summary>
		public NSView SignatureLine { get; private set; }

		/// <summary>
		/// The caption displayed under the signature line.
		/// </summary>
		public NSTextField Caption { get; private set; }

		/// <summary>
		/// The prompt displayed at the beginning of the signature line.
		/// </summary>
		public NSTextField SignaturePrompt { get; private set; }

		/// <summary>
		/// Gets the label that clears the pad when clicked.
		/// </summary>
		public NSButton ClearLabel { get; private set; }

		/// <summary>
		///  Gets the image view that may be used as a watermark or as a texture
		///  for the signature pad.
		/// </summary>
		public NSImageView BackgroundImageView { get; private set; }

		[Export ("StrokeColor"), Browsable (true)]
		public NSColor StrokeColor
		{
			get => SignaturePadCanvas.StrokeColor;
			set => SignaturePadCanvas.StrokeColor = value;
		}

		[Export ("StrokeWidth"), Browsable (true)]
		public float StrokeWidth
		{
			get => SignaturePadCanvas.StrokeWidth;
			set => SignaturePadCanvas.StrokeWidth = value;
		}

		/// <summary>
		/// Gets or sets the color of the signature line.
		/// </summary>
		[Export ("SignatureLineColor"), Browsable (true)]
		public CGColor SignatureLineColor
		{
			get => SignatureLine.Layer.BackgroundColor;
			set => SignatureLine.Layer.BackgroundColor = value;
		}

		/// <summary>
		/// Gets or sets the width of the signature line.
		/// </summary>
		[Export ("SignatureLineWidth"), Browsable (true)]
		public nfloat SignatureLineWidth
		{
			get => SignatureLine.Bounds.Height;
			set
			{
				var bounds = SignatureLine.Bounds;
				bounds.Height = value;
				SignatureLine.Bounds = bounds;
				NeedsLayout = true;
			}
		}

		/// <summary>
		/// Gets or sets the spacing between the signature line and the caption and prompt.
		/// </summary>
		[Export ("SignatureLineSpacing"), Browsable (true)]
		public nfloat SignatureLineSpacing
		{
			get => SignatureLine.Bounds.Bottom;
			set
			{
				var margins = SignatureLine.Bounds;
				//margins.Top = value;
				//margins.Bottom = value;
				SignatureLine.Bounds = margins;
				NeedsLayout = true;
			}
		}

		/// <summary>
		/// Gets or sets the text for the caption displayed under the signature line.
		/// </summary>
		[Export ("CaptionText"), Browsable (true)]
		public string CaptionText
		{
			get => Caption.StringValue;
			set
			{
				Caption.StringValue = value;
				NeedsLayout = true;
			}
		}

		/// <summary>
		/// Gets or sets the font size text for the caption displayed under the signature line.
		/// </summary>
		[Export ("CaptionFontSize"), Browsable (true)]
		public nfloat CaptionFontSize
		{
			get => Caption.Font.PointSize;
			set
			{
				Caption.Font = NSFont.FromFontName (Caption.Font.FontName, value);
				NeedsLayout = true;
			}
		}

		/// <summary>
		/// Gets or sets the text color for the caption displayed under the signature line.
		/// </summary>
		[Export ("CaptionTextColor"), Browsable (true)]
		public NSColor CaptionTextColor
		{
			get => Caption.TextColor;
			set => Caption.TextColor = value;
		}

		/// <summary>
		/// Gets or sets the text for the prompt displayed at the beginning of the signature line.
		/// </summary>
		[Export ("SignaturePromptText"), Browsable (true)]
		public string SignaturePromptText
		{
			get => SignaturePrompt.StringValue;
			set
			{
				SignaturePrompt.StringValue = value;
				NeedsLayout = true;
			}
		}

		/// <summary>
		/// Gets or sets the font size the prompt displayed at the beginning of the signature line.
		/// </summary>
		[Export ("SignaturePromptFontSize"), Browsable (true)]
		public nfloat SignaturePromptFontSize
		{
			get => SignaturePrompt.Font.PointSize;
			set
			{
				SignaturePrompt.Font = NSFont.FromFontName(SignaturePrompt.Font.FontName, value);
				NeedsLayout = true;
			}
		}

		/// <summary>
		/// Gets or sets the text color for the prompt displayed at the beginning of the signature line.
		/// </summary>
		[Export ("SignaturePromptTextColor"), Browsable (true)]
		public NSColor SignaturePromptTextColor
		{
			get => SignaturePrompt.TextColor;
			set => SignaturePrompt.TextColor = value;
		}

		/// <summary>
		/// Gets or sets the text for the label that clears the pad when clicked.
		/// </summary>
		[Export ("ClearLabelText"), Browsable (true)]
		public string ClearLabelText
		{
			get => ClearLabel.Title;
			set
			{
				ClearLabel.Title= value;
				NeedsLayout = true;
			}
		}

		/// <summary>
		/// Gets or sets the font size the label that clears the pad when clicked.
		/// </summary>
		[Export ("ClearLabelFontSize"), Browsable (true)]
		public nfloat ClearLabelFontSize
		{
			get => ClearLabel.Font.PointSize;
			set
			{
				ClearLabel.Font = NSFont.FromFontName (ClearLabel.Font.FontName, value);
				NeedsLayout = true;
			}
		}

		/// <summary>
		/// Gets or sets the text color for the label that clears the pad when clicked.
		/// </summary>
		[Export ("ClearLabelTextColor"), Browsable (true)]
		public NSColor ClearLabelTextColor
		{
			get => ClearLabel.BezelColor;
			set => ClearLabel.BezelColor = value;
		}

		[Export ("BackgroundImage"), Browsable (true)]
		public NSImage BackgroundImage
		{
			get => BackgroundImageView.Image;
			set => BackgroundImageView.Image = value;
		}

		[Export ("BackgroundImageContentMode"), Browsable (true)]
		public NSImageView BackgroundImageContentMode
		{
			get => BackgroundImageView;
			set => BackgroundImageView = value;
		}

		[Export ("BackgroundImageAlpha"), Browsable (true)]
		public nfloat BackgroundImageAlpha
		{
			get => BackgroundImageView.AlphaValue;
			set => BackgroundImageView.AlphaValue = value;
		}

		[Export ("Padding"), Browsable (true)]
		public NSEdgeInsets Padding
		{
			get => padding;
			set
			{
				padding = value;
				NeedsLayout = true;
			}
		}

		private void UpdateUi ()
		{
			ClearLabel.Hidden = IsBlank;
		}

		public override void Layout ()
		{
			var w = Frame.Width;
			var h = Frame.Height;
			var currentY = h;

			SignaturePrompt.SizeToFit ();
			ClearLabel.SizeToFit ();

			var captionHeight = Caption.SizeThatFits (Caption.Frame.Size).Height;
			var clearButtonHeight = (int)ClearLabel.Font.CapHeight + 1;

			var rect = new CGRect (0, 0, w, h);
			SignaturePadCanvas.Frame = rect;
			BackgroundImageView.Frame = rect;

			currentY = currentY - Padding.Bottom - captionHeight;
			Caption.Frame = new CGRect (Padding.Left, currentY, w - Padding.Left - Padding.Right, captionHeight);

			currentY = currentY - SignatureLine.Bounds.Bottom - SignatureLine.Frame.Height;
			SignatureLine.Frame = new CGRect (Padding.Left, currentY, w - Padding.Left - Padding.Right, SignatureLine.Frame.Height);

			currentY = currentY - SignatureLine.Bounds.Top - SignaturePrompt.Frame.Height;
			SignaturePrompt.Frame = new CGRect (Padding.Left, currentY, SignaturePrompt.Frame.Width, SignaturePrompt.Frame.Height);

			ClearLabel.Frame = new CGRect (w - Padding.Right - ClearLabel.Frame.Width, Padding.Top, ClearLabel.Frame.Width, clearButtonHeight);
		}
	}
}
