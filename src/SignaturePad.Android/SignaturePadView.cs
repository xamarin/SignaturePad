using System;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Xamarin.Controls
{
	public partial class SignaturePadView : RelativeLayout
	{
		private static Random rnd = new Random ();

		public SignaturePadView (Context context)
			: base (context)
		{
			Initialize (null);
		}

		public SignaturePadView (Context context, IAttributeSet attrs)
			: base (context, attrs)
		{
			Initialize (attrs);
		}

		public SignaturePadView (Context context, IAttributeSet attrs, int defStyle)
			: base (context, attrs, defStyle)
		{
			Initialize (attrs);
		}

		private void Initialize (IAttributeSet attrs)
		{
			Inflate (Context, Resource.Layout.signature_pad_layout, this);

			// find the views
			BackgroundImageView = FindViewById<ImageView> (Resource.Id.background_image);
			SignaturePadCanvas = FindViewById<SignaturePadCanvasView> (Resource.Id.signature_canvas);
			Caption = FindViewById<TextView> (Resource.Id.caption);
			SignatureLine = FindViewById<View> (Resource.Id.signature_line);
			SignaturePrompt = FindViewById<TextView> (Resource.Id.signature_prompt);
			ClearLabel = FindViewById<TextView> (Resource.Id.clear_label);

			// set the properties from the attributes
			if (attrs != null)
			{
				using (var a = Context.Theme.ObtainStyledAttributes (attrs, Resource.Styleable.SignaturePadView, 0, 0))
				{
					if (a.HasValue (Resource.Styleable.SignaturePadView_strokeColor))
						StrokeColor = a.GetColor (Resource.Styleable.SignaturePadView_strokeColor, ImageConstructionSettings.DefaultStrokeColor);
					if (a.HasValue (Resource.Styleable.SignaturePadView_strokeWidth))
						StrokeWidth = a.GetDimension (Resource.Styleable.SignaturePadView_strokeWidth, ImageConstructionSettings.DefaultStrokeWidth);

					if (a.HasValue (Resource.Styleable.SignaturePadView_captionText))
						CaptionText = a.GetString (Resource.Styleable.SignaturePadView_captionText);
					if (a.HasValue (Resource.Styleable.SignaturePadView_captionTextColor))
						CaptionTextColor = a.GetColor (Resource.Styleable.SignaturePadView_captionTextColor, SignaturePadDarkColor);
					if (a.HasValue (Resource.Styleable.SignaturePadView_captionTextSize))
						CaptionTextSize = a.GetDimension (Resource.Styleable.SignaturePadView_captionTextSize, DefaultFontSize);

					if (a.HasValue (Resource.Styleable.SignaturePadView_clearLabelText))
						ClearLabelText = a.GetString (Resource.Styleable.SignaturePadView_clearLabelText);
					if (a.HasValue (Resource.Styleable.SignaturePadView_clearLabelTextColor))
						ClearLabelTextColor = a.GetColor (Resource.Styleable.SignaturePadView_clearLabelTextColor, SignaturePadDarkColor);
					if (a.HasValue (Resource.Styleable.SignaturePadView_clearLabelTextSize))
						ClearLabelTextSize = a.GetDimension (Resource.Styleable.SignaturePadView_clearLabelTextSize, DefaultFontSize);

					if (a.HasValue (Resource.Styleable.SignaturePadView_signaturePromptText))
						SignaturePromptText = a.GetString (Resource.Styleable.SignaturePadView_signaturePromptText);
					if (a.HasValue (Resource.Styleable.SignaturePadView_signaturePromptTextColor))
						SignaturePromptTextColor = a.GetColor (Resource.Styleable.SignaturePadView_signaturePromptTextColor, SignaturePadDarkColor);
					if (a.HasValue (Resource.Styleable.SignaturePadView_signaturePromptTextSize))
						SignaturePromptTextSize = a.GetDimension (Resource.Styleable.SignaturePadView_signaturePromptTextSize, DefaultFontSize);

					if (a.HasValue (Resource.Styleable.SignaturePadView_signatureLineColor))
						SignatureLineColor = a.GetColor (Resource.Styleable.SignaturePadView_signatureLineColor, SignaturePadDarkColor);
					if (a.HasValue (Resource.Styleable.SignaturePadView_signatureLineSpacing))
						SignatureLineSpacing = a.GetInt (Resource.Styleable.SignaturePadView_signatureLineSpacing, (int)DefaultNarrowSpacing);
					if (a.HasValue (Resource.Styleable.SignaturePadView_signatureLineWidth))
						SignatureLineWidth = a.GetInt (Resource.Styleable.SignaturePadView_signatureLineWidth, (int)DefaultLineThickness);

					a.Recycle ();
				}
			}

			// attach the events
			SignaturePadCanvas.StrokeCompleted += (sender, e) => OnSignatureStrokeCompleted ();
			SignaturePadCanvas.Cleared += (sender, e) => OnSignatureCleared ();
			ClearLabel.Click += (sender, e) => OnClearTapped ();

			// initialize the view
			UpdateUi ();
		}

		public SignaturePadCanvasView SignaturePadCanvas { get; private set; }

		public View SignatureLine { get; private set; }

		public TextView Caption { get; private set; }

		public TextView SignaturePrompt { get; private set; }

		public TextView ClearLabel { get; private set; }

		[Obsolete ("Set the background instead.")]
		public ImageView BackgroundImageView { get; private set; }

		public Color StrokeColor
		{
			get => SignaturePadCanvas.StrokeColor;
			set => SignaturePadCanvas.StrokeColor = value;
		}

		public float StrokeWidth
		{
			get => SignaturePadCanvas.StrokeWidth;
			set => SignaturePadCanvas.StrokeWidth = value;
		}

		public Color SignatureLineColor
		{
			get => (SignatureLine.Background as ColorDrawable)?.Color ?? Color.Transparent;
			set => SignatureLine.SetBackgroundColor (value);
		}

		public int SignatureLineWidth
		{
			get => SignatureLine.Height;
			set
			{
				var param = SignatureLine.LayoutParameters;
				param.Height = value;
				SignatureLine.LayoutParameters = param;
			}
		}

		public int SignatureLineSpacing
		{
			get => SignatureLine.PaddingBottom;
			set => SignatureLine.SetPadding (PaddingLeft, value, PaddingRight, value);
		}

		public string CaptionText
		{
			get => Caption.Text;
			set => Caption.Text = value;
		}

		public float CaptionTextSize
		{
			get => Caption.TextSize;
			set => Caption.TextSize = value;
		}

		public Color CaptionTextColor
		{
			get => new Color (Caption.CurrentTextColor);
			set => Caption.SetTextColor (value);
		}

		public string SignaturePromptText
		{
			get => SignaturePrompt.Text;
			set => SignaturePrompt.Text = value;
		}

		public float SignaturePromptTextSize
		{
			get => SignaturePrompt.TextSize;
			set => SignaturePrompt.TextSize = value;
		}

		public Color SignaturePromptTextColor
		{
			get => new Color (SignaturePrompt.CurrentTextColor);
			set => SignaturePrompt.SetTextColor (value);
		}

		public string ClearLabelText
		{
			get => ClearLabel.Text;
			set => ClearLabel.Text = value;
		}

		public float ClearLabelTextSize
		{
			get => ClearLabel.TextSize;
			set => ClearLabel.TextSize = value;
		}

		public Color ClearLabelTextColor
		{
			get => new Color (ClearLabel.CurrentTextColor);
			set => ClearLabel.SetTextColor (value);
		}

		[Obsolete ("Set the background instead.")]
		public Color BackgroundColor
		{
			get => (Background as ColorDrawable)?.Color ?? Color.Transparent;
			set => SetBackgroundColor (value);
		}

		private void UpdateUi ()
		{
			ClearLabel.Visibility = IsBlank ? ViewStates.Invisible : ViewStates.Visible;
		}

		public override bool OnInterceptTouchEvent (MotionEvent ev)
		{
			// don't accept touch when the view is disabled
			if (!Enabled)
				return true;

			return base.OnInterceptTouchEvent (ev);
		}
	}
}
