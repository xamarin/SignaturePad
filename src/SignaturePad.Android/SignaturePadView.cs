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
			Initialize ();
		}

		public SignaturePadView (Context context, IAttributeSet attrs)
			: base (context, attrs)
		{
			Initialize ();
		}

		public SignaturePadView (Context context, IAttributeSet attrs, int defStyle)
			: base (context, attrs, defStyle)
		{
			Initialize ();
		}

		private void Initialize ()
		{
			Inflate (Context, Resource.Layout.signature_pad_layout, this);

			SetBackgroundResource (Resource.Drawable.signature_pad_background);

			BackgroundImageView = FindViewById<ImageView> (Resource.Id.background_image);

			SignaturePadCanvas = FindViewById<SignaturePadCanvasView> (Resource.Id.signature_canvas);
			SignaturePadCanvas.StrokeCompleted += (sender, e) => OnSignatureStrokeCompleted ();
			SignaturePadCanvas.Cleared += (sender, e) => OnSignatureCleared ();

			Caption = FindViewById<TextView> (Resource.Id.caption);

			SignatureLine = FindViewById<View> (Resource.Id.signature_line);

			SignaturePrompt = FindViewById<TextView> (Resource.Id.signature_prompt);

			ClearLabel = FindViewById<TextView> (Resource.Id.clear_label);
			ClearLabel.Click += (sender, e) => OnClearTapped ();

			// clear / initialize the view
			Clear ();
		}

		public SignaturePadCanvasView SignaturePadCanvas { get; private set; }

		/// <summary>
		/// Gets or sets the color of the strokes for the signature.
		/// </summary>
		/// <value>The color of the stroke.</value>
		public Color StrokeColor
		{
			get { return SignaturePadCanvas.StrokeColor; }
			set { SignaturePadCanvas.StrokeColor = value; }
		}

		/// <summary>
		/// Gets or sets the width in pixels of the strokes for the signature.
		/// </summary>
		/// <value>The width of the line.</value>
		public float StrokeWidth
		{
			get { return SignaturePadCanvas.StrokeWidth; }
			set { SignaturePadCanvas.StrokeWidth = value; }
		}

		/// <summary>
		/// The text for the prompt displayed at the beginning of the signature line.
		/// </summary>
		/// <remarks>
		/// Text value defaults to 'X'.
		/// </remarks>
		/// <value>The signature prompt.</value>
		public string SignaturePromptText
		{
			get { return SignaturePrompt.Text; }
			set { SignaturePrompt.Text = value; }
		}

		/// <summary>
		/// The text for the caption displayed under the signature line.
		/// </summary>
		/// <remarks>
		/// Text value defaults to 'Sign here.'
		/// </remarks>
		/// <value>The caption.</value>
		public string CaptionText
		{
			get { return Caption.Text; }
			set { Caption.Text = value; }
		}

		/// <summary>
		/// Gets the label that clears the pad when clicked.
		/// </summary>
		/// <value>The clear label.</value>
		public string ClearLabelText
		{
			get { return ClearLabel.Text; }
			set { ClearLabel.Text = value; }
		}

		/// <summary>
		/// The prompt displayed at the beginning of the signature line.
		/// </summary>
		/// <remarks>
		/// Text value defaults to 'X'.
		/// </remarks>
		/// <value>The signature prompt.</value>
		public TextView SignaturePrompt { get; private set; }

		/// <summary>
		/// The caption displayed under the signature line.
		/// </summary>
		/// <remarks>
		/// Text value defaults to 'Sign here.'
		/// </remarks>
		/// <value>The caption.</value>
		public TextView Caption { get; private set; }

		/// <summary>
		/// The color of the signature line.
		/// </summary>
		/// <value>The color of the signature line.</value>
		public Color SignatureLineColor
		{
			get
			{
				var dc = SignatureLine.Background as ColorDrawable;
				return dc == null ? Color.Transparent : dc.Color;
			}
			set { SignatureLine.SetBackgroundColor (value); }
		}

		/// <summary>
		/// The color of the background.
		/// </summary>
		/// <value>The color of the background.</value>
		public Color BackgroundColor
		{
			get
			{
				var dc = Background as ColorDrawable;
				return dc == null ? Color.Transparent : dc.Color;
			}
			set { SetBackgroundColor (value); }
		}

		/// <summary>
		/// Gets the background image view.
		/// </summary>
		/// <value>The background image view.</value>
		public ImageView BackgroundImageView { get; private set; }

		/// <summary>
		/// Gets the label that clears the pad when clicked.
		/// </summary>
		/// <value>The clear label.</value>
		public TextView ClearLabel { get; private set; }

		/// <summary>
		/// Gets the horizontal line that goes in the lower part of the pad.
		/// </summary>
		/// <value>The signature line.</value>
		public View SignatureLine { get; private set; }

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
