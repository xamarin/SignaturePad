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
			// set the border and background
			{
				BackgroundColor = SignaturePadLightColor;
				Layer.BorderWidth = 1f;
				Layer.BorderColor = SignaturePadDarkColor.CGColor;
			}

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
					Text = DefaultCaptionText,
					Font = UIFont.BoldSystemFontOfSize ((nfloat)DefaultFontSize),
					BackgroundColor = UIColor.Clear,
					TextColor = SignaturePadDarkColor,
					TextAlignment = UITextAlignment.Center
				};
				AddSubview (Caption);
			}

			// add the signature line
			{
				SignatureLine = new UIView ()
				{
					BackgroundColor = SignaturePadDarkColor
				};
				AddSubview (SignatureLine);
			}

			// add the prompt
			{
				SignaturePrompt = new UILabel ()
				{
					Text = DefaultPromptText,
					Font = UIFont.BoldSystemFontOfSize ((nfloat)DefaultFontSize),
					BackgroundColor = UIColor.Clear,
					TextColor = SignaturePadDarkColor
				};
				AddSubview (SignaturePrompt);
			}

			// add the clear label
			{
				ClearLabel = UIButton.FromType (UIButtonType.Custom);
				ClearLabel.SetTitle (DefaultClearLabelText, UIControlState.Normal);
				ClearLabel.Font = UIFont.BoldSystemFontOfSize ((nfloat)DefaultFontSize);
				ClearLabel.BackgroundColor = UIColor.Clear;
				ClearLabel.SetTitleColor (SignaturePadDarkColor, UIControlState.Normal);
				AddSubview (ClearLabel);

				// attach the "clear" command
				ClearLabel.TouchUpInside += delegate
				{
					OnClearTapped ();
				};
			}

			// clear / initialize the view
			Clear ();
		}

		public SignaturePadCanvasView SignaturePadCanvas { get; private set; }

		[Export ("StrokeColor"), Browsable (true)]
		public UIColor StrokeColor
		{
			get { return SignaturePadCanvas.StrokeColor; }
			set { SignaturePadCanvas.StrokeColor = value; }
		}

		[Export ("StrokeWidth"), Browsable (true)]
		public float StrokeWidth
		{
			get { return SignaturePadCanvas.StrokeWidth; }
			set { SignaturePadCanvas.StrokeWidth = value; }
		}

		/// <summary>
		/// The prompt displayed at the beginning of the signature line.
		/// </summary>
		/// <remarks>
		/// Text value defaults to 'X'.
		/// </remarks>
		/// <value>The signature prompt.</value>
		public UILabel SignaturePrompt { get; private set; }

		/// <summary>
		/// The caption displayed under the signature line.
		/// </summary>
		/// <remarks>
		/// Text value defaults to 'Sign here.'
		/// </remarks>
		/// <value>The caption.</value>
		public UILabel Caption { get; private set; }

		/// <summary>
		/// The color of the signature line.
		/// </summary>
		/// <value>The color of the signature line.</value>
		[Export ("SignatureLineColor"), Browsable (true)]
		public UIColor SignatureLineColor
		{
			get { return SignatureLine.BackgroundColor; }
			set { SignatureLine.BackgroundColor = value; }
		}

		/// <summary>
		///  An image view that may be used as a watermark or as a texture
		///  for the signature pad.
		/// </summary>
		/// <value>The background image view.</value>
		public UIImageView BackgroundImageView { get; private set; }

		/// <summary>
		///  An image view that may be used as a watermark or as a texture
		///  for the signature pad.
		/// </summary>
		/// <value>The background image.</value>
		[Export ("BackgroundImage"), Browsable (true)]
		public UIImage BackgroundImage
		{
			get { return BackgroundImageView.Image; }
			set { BackgroundImageView.Image = value; }
		}

		/// <summary>
		///  An image view that may be used as a watermark or as a texture
		///  for the signature pad.
		/// </summary>
		/// <value>The background image.</value>
		[Export ("BackgroundImageContentMode"), Browsable (true)]
		public UIViewContentMode BackgroundImageContentMode
		{
			get { return BackgroundImageView.ContentMode; }
			set { BackgroundImageView.ContentMode = value; }
		}

		/// <summary>
		///  The transparency of the watermark.
		/// </summary>
		/// <value>The background image.</value>
		[Export ("BackgroundImageAlpha"), Browsable (true)]
		public nfloat BackgroundImageAlpha
		{
			get { return BackgroundImageView.Alpha; }
			set { BackgroundImageView.Alpha = value; }
		}

		/// <summary>
		/// The text for the prompt displayed at the beginning of the signature line.
		/// </summary>
		/// <remarks>
		/// Text value defaults to 'X'.
		/// </remarks>
		/// <value>The signature prompt.</value>
		[Export ("SignaturePromptText"), Browsable (true)]
		public string SignaturePromptText
		{
			get { return SignaturePrompt.Text; }
			set
			{
				SignaturePrompt.Text = value;
				SetNeedsLayout ();
			}
		}

		/// <summary>
		/// The text for the caption displayed under the signature line.
		/// </summary>
		/// <remarks>
		/// Text value defaults to 'Sign here.'
		/// </remarks>
		/// <value>The caption.</value>
		[Export ("CaptionText"), Browsable (true)]
		public string CaptionText
		{
			get { return Caption.Text; }
			set { Caption.Text = value; }
		}

		/// <summary>
		/// Gets the text for the label that clears the pad when clicked.
		/// </summary>
		/// <value>The clear label.</value>
		[Export ("ClearLabelText"), Browsable (true)]
		public string ClearLabelText
		{
			get { return ClearLabel.Title (UIControlState.Normal); }
			set
			{
				ClearLabel.SetTitle (value, UIControlState.Normal);
				SetNeedsLayout ();
			}
		}

		/// <summary>
		/// Gets the label that clears the pad when clicked.
		/// </summary>
		/// <value>The clear label.</value>
		public UIButton ClearLabel { get; private set; }

		/// <summary>
		/// Gets the horizontal line that goes in the lower part of the pad.
		/// </summary>
		/// <value>The signature line.</value>
		public UIView SignatureLine { get; private set; }

		private void UpdateUi ()
		{
			ClearLabel.Hidden = IsBlank;
		}

		public override void LayoutSubviews ()
		{
			const int ThinPad = 3;
			const int ThickPad = 10;
			const int LineHeight = 1;

			var w = Frame.Width;
			var h = Frame.Height;

			SignaturePrompt.SizeToFit ();
			ClearLabel.SizeToFit ();

			var captionHeight = Caption.SizeThatFits (Caption.Frame.Size).Height;
			var clearButtonHeight = (int)ClearLabel.Font.LineHeight + 1;

			var rect = new CGRect (0, 0, w, h);
			SignaturePadCanvas.Frame = rect;
			BackgroundImageView.Frame = rect;

			var top = h;
			top = top - ThinPad - captionHeight;
			Caption.Frame = new CGRect (ThickPad, top, w - ThickPad - ThickPad, captionHeight);

			top = top - ThinPad - SignatureLine.Frame.Height;
			SignatureLine.Frame = new CGRect (ThickPad, top, w - ThickPad - ThickPad, LineHeight);

			top = top - ThinPad - SignaturePrompt.Frame.Height;
			SignaturePrompt.Frame = new CGRect (ThickPad, top, SignaturePrompt.Frame.Width, SignaturePrompt.Frame.Height);

			ClearLabel.Frame = new CGRect (w - ThickPad - ClearLabel.Frame.Width, ThickPad, ClearLabel.Frame.Width, clearButtonHeight);
		}
	}
}
