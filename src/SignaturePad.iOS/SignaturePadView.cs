using System;
using System.ComponentModel;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Xamarin.Controls
{
	[Register ("SignaturePadView")]
	[DesignTimeVisible (true)]
	public partial class SignaturePadView : UIView
	{
		private UIEdgeInsets padding;

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
				BackgroundImageView = new UIImageView ();
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
				Caption = new UILabel ()
				{
					BackgroundColor = UIColor.Clear,
					TextAlignment = UITextAlignment.Center,
					Font = UIFont.SystemFontOfSize (DefaultFontSize),
					Text = DefaultCaptionText,
					TextColor = SignaturePadDarkColor,
				};
				AddSubview (Caption);
			}

			// add the signature line
			{
				SignatureLine = new UIView ()
				{
					BackgroundColor = SignaturePadDarkColor,
				};
				SignatureLineWidth = DefaultLineThickness;
				SignatureLineSpacing = DefaultNarrowSpacing;
				AddSubview (SignatureLine);
			}

			// add the prompt
			{
				SignaturePrompt = new UILabel ()
				{
					BackgroundColor = UIColor.Clear,
					Font = UIFont.BoldSystemFontOfSize (DefaultFontSize),
					Text = DefaultPromptText,
					TextColor = SignaturePadDarkColor,
				};
				AddSubview (SignaturePrompt);
			}

			// add the clear label
			{
				ClearLabel = UIButton.FromType (UIButtonType.Custom);
				ClearLabel.BackgroundColor = UIColor.Clear;
				ClearLabel.Font = UIFont.BoldSystemFontOfSize (DefaultFontSize);
				ClearLabel.SetTitle (DefaultClearLabelText, UIControlState.Normal);
				ClearLabel.SetTitleColor (SignaturePadDarkColor, UIControlState.Normal);
				AddSubview (ClearLabel);

				// attach the "clear" command
				ClearLabel.TouchUpInside += delegate
				{
					OnClearTapped ();
				};
			}

			Padding = new UIEdgeInsets (DefaultWideSpacing, DefaultWideSpacing, DefaultNarrowSpacing, DefaultWideSpacing);

			// clear / initialize the view
			UpdateUi ();
		}

		public SignaturePadCanvasView SignaturePadCanvas { get; private set; }

		/// <summary>
		/// Gets the horizontal line that goes in the lower part of the pad.
		/// </summary>
		public UIView SignatureLine { get; private set; }

		/// <summary>
		/// The caption displayed under the signature line.
		/// </summary>
		public UILabel Caption { get; private set; }

		/// <summary>
		/// The prompt displayed at the beginning of the signature line.
		/// </summary>
		public UILabel SignaturePrompt { get; private set; }

		/// <summary>
		/// Gets the label that clears the pad when clicked.
		/// </summary>
		public UIButton ClearLabel { get; private set; }

		/// <summary>
		///  Gets the image view that may be used as a watermark or as a texture
		///  for the signature pad.
		/// </summary>
		public UIImageView BackgroundImageView { get; private set; }

		[Export ("StrokeColor"), Browsable (true)]
		public UIColor StrokeColor
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
		public UIColor SignatureLineColor
		{
			get => SignatureLine.BackgroundColor;
			set => SignatureLine.BackgroundColor = value;
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
				SetNeedsLayout ();
			}
		}

		/// <summary>
		/// Gets or sets the spacing between the signature line and the caption and prompt.
		/// </summary>
		[Export ("SignatureLineSpacing"), Browsable (true)]
		public nfloat SignatureLineSpacing
		{
			get => SignatureLine.LayoutMargins.Bottom;
			set
			{
				var margins = SignatureLine.LayoutMargins;
				margins.Top = value;
				margins.Bottom = value;
				SignatureLine.LayoutMargins = margins;
				SetNeedsLayout ();
			}
		}

		/// <summary>
		/// Gets or sets the text for the caption displayed under the signature line.
		/// </summary>
		[Export ("CaptionText"), Browsable (true)]
		public string CaptionText
		{
			get => Caption.Text;
			set
			{
				Caption.Text = value;
				SetNeedsLayout ();
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
				Caption.Font = Caption.Font.WithSize (value);
				SetNeedsLayout ();
			}
		}

		/// <summary>
		/// Gets or sets the text color for the caption displayed under the signature line.
		/// </summary>
		[Export ("CaptionTextColor"), Browsable (true)]
		public UIColor CaptionTextColor
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
			get => SignaturePrompt.Text;
			set
			{
				SignaturePrompt.Text = value;
				SetNeedsLayout ();
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
				SignaturePrompt.Font = SignaturePrompt.Font.WithSize (value);
				SetNeedsLayout ();
			}
		}

		/// <summary>
		/// Gets or sets the text color for the prompt displayed at the beginning of the signature line.
		/// </summary>
		[Export ("SignaturePromptTextColor"), Browsable (true)]
		public UIColor SignaturePromptTextColor
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
			get => ClearLabel.Title (UIControlState.Normal);
			set
			{
				ClearLabel.SetTitle (value, UIControlState.Normal);
				SetNeedsLayout ();
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
				ClearLabel.Font = ClearLabel.Font.WithSize (value);
				SetNeedsLayout ();
			}
		}

		/// <summary>
		/// Gets or sets the text color for the label that clears the pad when clicked.
		/// </summary>
		[Export ("ClearLabelTextColor"), Browsable (true)]
		public UIColor ClearLabelTextColor
		{
			get => ClearLabel.TitleColor (UIControlState.Normal);
			set => ClearLabel.SetTitleColor (value, UIControlState.Normal);
		}

		[Export ("BackgroundImage"), Browsable (true)]
		public UIImage BackgroundImage
		{
			get => BackgroundImageView.Image;
			set => BackgroundImageView.Image = value;
		}

		[Export ("BackgroundImageContentMode"), Browsable (true)]
		public UIViewContentMode BackgroundImageContentMode
		{
			get => BackgroundImageView.ContentMode;
			set => BackgroundImageView.ContentMode = value;
		}

		[Export ("BackgroundImageAlpha"), Browsable (true)]
		public nfloat BackgroundImageAlpha
		{
			get => BackgroundImageView.Alpha;
			set => BackgroundImageView.Alpha = value;
		}

		[Export ("Padding"), Browsable (true)]
		public UIEdgeInsets Padding
		{
			get => padding;
			set
			{
				padding = value;
				SetNeedsLayout ();
			}
		}

		private void UpdateUi ()
		{
			ClearLabel.Hidden = IsBlank;
		}

		public override void LayoutSubviews ()
		{
			var w = Frame.Width;
			var h = Frame.Height;
			var currentY = h;

			SignaturePrompt.SizeToFit ();
			ClearLabel.SizeToFit ();

			var captionHeight = Caption.SizeThatFits (Caption.Frame.Size).Height;
			var clearButtonHeight = (int)ClearLabel.Font.LineHeight + 1;

			var rect = new CGRect (0, 0, w, h);
			SignaturePadCanvas.Frame = rect;
			BackgroundImageView.Frame = rect;

			currentY = currentY - Padding.Bottom - captionHeight;
			Caption.Frame = new CGRect (Padding.Left, currentY, w - Padding.Left - Padding.Right, captionHeight);

			currentY = currentY - SignatureLine.LayoutMargins.Bottom - SignatureLine.Frame.Height;
			SignatureLine.Frame = new CGRect (Padding.Left, currentY, w - Padding.Left - Padding.Right, SignatureLine.Frame.Height);

			currentY = currentY - SignatureLine.LayoutMargins.Top - SignaturePrompt.Frame.Height;
			SignaturePrompt.Frame = new CGRect (Padding.Left, currentY, SignaturePrompt.Frame.Width, SignaturePrompt.Frame.Height);

			ClearLabel.Frame = new CGRect (w - Padding.Right - ClearLabel.Frame.Width, Padding.Top, ClearLabel.Frame.Width, clearButtonHeight);
		}
	}
}
