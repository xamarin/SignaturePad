using System;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Util;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;
using System.IO;

namespace Xamarin.Controls
{
	public partial class SignaturePadView : RelativeLayout
	{
		private static Random rnd = new Random ();

		private static int GenerateId (View view)
		{
			int id;
			do
			{
				id = rnd.Next (1, 0x00FFFFFF);
			}
			while (view.FindViewById<View> (id) != null);
			return id;
		}

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
			// add the background view
			{
				BackgroundImageView = new ImageView (Context)
				{
					Id = GenerateId (this),
					LayoutParameters = new RelativeLayout.LayoutParams (RelativeLayout.LayoutParams.MatchParent, RelativeLayout.LayoutParams.MatchParent)
				};
				AddView (BackgroundImageView);
			}

			// add the main signature view
			{
				SignaturePadCanvas = new SignaturePadCanvasView (Context)
				{
					Id = GenerateId (this),
					LayoutParameters = new RelativeLayout.LayoutParams (RelativeLayout.LayoutParams.MatchParent, RelativeLayout.LayoutParams.MatchParent)
				};
				SignaturePadCanvas.StrokeCompleted += (sender, e) =>
				{
					UpdateUi ();
					StrokeCompleted?.Invoke (this, EventArgs.Empty);
				};
				SignaturePadCanvas.Cleared += (sender, e) => Cleared?.Invoke (this, EventArgs.Empty);
				AddView (SignaturePadCanvas);
			}

			// add the caption
			{
				Caption = new TextView (Context)
				{
					Id = GenerateId (this),
					Text = "Sign Here"
				};
				var layout = new RelativeLayout.LayoutParams (RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent)
				{
					AlignWithParent = true,
					BottomMargin = 6
				};
				layout.AddRule (LayoutRules.AlignBottom);
				layout.AddRule (LayoutRules.CenterHorizontal);
				Caption.LayoutParameters = layout;
				Caption.SetIncludeFontPadding (true);
				Caption.SetPadding (0, 0, 0, 6);
				AddView (Caption);
			}

			// add the signature line
			{
				SignatureLine = new View (Context)
				{
					Id = GenerateId (this)
				};
				SignatureLine.SetBackgroundColor (Color.Gray);
				var layout = new RelativeLayout.LayoutParams (RelativeLayout.LayoutParams.MatchParent, 1);
				layout.SetMargins (10, 0, 10, 5);
				layout.AddRule (LayoutRules.Above, Caption.Id);
				SignatureLine.LayoutParameters = layout;
				AddView (SignatureLine);
			}

			// add the prompt
			{
				SignaturePrompt = new TextView (Context)
				{
					Id = GenerateId (this),
					Text = "X"
				};
				SignaturePrompt.SetTypeface (null, TypefaceStyle.Bold);
				var layout = new RelativeLayout.LayoutParams (RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent)
				{
					LeftMargin = 11
				};
				layout.AddRule (LayoutRules.Above, SignatureLine.Id);
				SignaturePrompt.LayoutParameters = layout;
				AddView (SignaturePrompt);
			}

			// add the clear label
			{
				ClearLabel = new TextView (Context)
				{
					Id = GenerateId (this),
					Text = "Clear",
					Visibility = ViewStates.Invisible
				};
				var layout = new RelativeLayout.LayoutParams (RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent)
				{
					AlignWithParent = true
				};
				layout.SetMargins (0, 10, 22, 0);
				layout.AddRule (LayoutRules.AlignRight);
				layout.AddRule (LayoutRules.AlignTop);
				ClearLabel.LayoutParameters = layout;
				AddView (ClearLabel);

				// attach the "clear" command
				ClearLabel.Click += (sender, e) => Clear ();
			}

			// clear / initialize the view
			Clear ();
		}

		public System.Drawing.PointF[] Points => SignaturePadCanvas.Points;

		public System.Drawing.PointF[][] Strokes => SignaturePadCanvas.Strokes;

		public bool IsBlank => SignaturePadCanvas.IsBlank;

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

		public event EventHandler StrokeCompleted;

		public event EventHandler Cleared;

		public void Clear ()
		{
			SignaturePadCanvas.Clear ();

			UpdateUi ();
		}

		public void LoadPoints (System.Drawing.PointF[] points)
		{
			SignaturePadCanvas.LoadPoints (points);

			UpdateUi ();
		}

		public void LoadStrokes (System.Drawing.PointF[][] strokes)
		{
			SignaturePadCanvas.LoadStrokes (strokes);

			UpdateUi ();
		}

		/// <summary>
		/// Create an image of the currently drawn signature.
		/// </summary>
		public Bitmap GetImage (bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return SignaturePadCanvas.GetImage (shouldCrop, keepAspectRatio);
		}

		/// <summary>
		/// Create an image of the currently drawn signature at the specified size.
		/// </summary>
		public Bitmap GetImage (System.Drawing.SizeF size, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return SignaturePadCanvas.GetImage (size, shouldCrop, keepAspectRatio);
		}

		/// <summary>
		/// Create an image of the currently drawn signature at the specified scale.
		/// </summary>
		public Bitmap GetImage (float scale, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return SignaturePadCanvas.GetImage (scale, shouldCrop, keepAspectRatio);
		}

		/// <summary>
		/// Create an image of the currently drawn signature with the specified stroke color.
		/// </summary>
		public Bitmap GetImage (Color strokeColor, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return SignaturePadCanvas.GetImage (strokeColor, shouldCrop, keepAspectRatio);
		}

		/// <summary>
		/// Create an image of the currently drawn signature at the specified size with the specified stroke color.
		/// </summary>
		public Bitmap GetImage (Color strokeColor, System.Drawing.SizeF size, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return SignaturePadCanvas.GetImage (strokeColor, size, shouldCrop, keepAspectRatio);
		}

		/// <summary>
		/// Create an image of the currently drawn signature at the specified scale with the specified stroke color.
		/// </summary>
		public Bitmap GetImage (Color strokeColor, float scale, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return SignaturePadCanvas.GetImage (strokeColor, scale, shouldCrop, keepAspectRatio);
		}

		/// <summary>
		/// Create an image of the currently drawn signature with the specified stroke and background colors.
		/// </summary>
		public Bitmap GetImage (Color strokeColor, Color fillColor, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return SignaturePadCanvas.GetImage (strokeColor, fillColor, shouldCrop, keepAspectRatio);
		}

		/// <summary>
		/// Create an image of the currently drawn signature at the specified size with the specified stroke and background colors.
		/// </summary>
		public Bitmap GetImage (Color strokeColor, Color fillColor, System.Drawing.SizeF size, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return SignaturePadCanvas.GetImage (strokeColor, fillColor, size, shouldCrop, keepAspectRatio);
		}

		/// <summary>
		/// Create an image of the currently drawn signature at the specified scale with the specified stroke and background colors.
		/// </summary>
		public Bitmap GetImage (Color strokeColor, Color fillColor, float scale, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return SignaturePadCanvas.GetImage (strokeColor, fillColor, scale, shouldCrop, keepAspectRatio);
		}

		/// <summary>
		/// Create an image of the currently drawn signature using the specified settings.
		/// </summary>
		public Bitmap GetImage (ImageConstructionSettings settings)
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
		public Task<Stream> GetImageStreamAsync (SignatureImageFormat format, System.Drawing.SizeF size, bool shouldCrop = true, bool keepAspectRatio = true)
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
		public Task<Stream> GetImageStreamAsync (SignatureImageFormat format, Color strokeColor, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return SignaturePadCanvas.GetImageStreamAsync (format, strokeColor, shouldCrop, keepAspectRatio);
		}

		/// <summary>
		/// Create an encoded image of the currently drawn signature at the specified size with the specified stroke color.
		/// </summary>
		public Task<Stream> GetImageStreamAsync (SignatureImageFormat format, Color strokeColor, System.Drawing.SizeF size, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return SignaturePadCanvas.GetImageStreamAsync (format, strokeColor, size, shouldCrop, keepAspectRatio);
		}

		/// <summary>
		/// Create an encoded image of the currently drawn signature at the specified scale with the specified stroke color.
		/// </summary>
		public Task<Stream> GetImageStreamAsync (SignatureImageFormat format, Color strokeColor, float scale, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return SignaturePadCanvas.GetImageStreamAsync (format, strokeColor, scale, shouldCrop, keepAspectRatio);
		}

		/// <summary>
		/// Create an encoded image of the currently drawn signature with the specified stroke and background colors.
		/// </summary>
		public Task<Stream> GetImageStreamAsync (SignatureImageFormat format, Color strokeColor, Color fillColor, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return SignaturePadCanvas.GetImageStreamAsync (format, strokeColor, fillColor, shouldCrop, keepAspectRatio);
		}

		/// <summary>
		/// Create an encoded image of the currently drawn signature at the specified size with the specified stroke and background colors.
		/// </summary>
		public Task<Stream> GetImageStreamAsync (SignatureImageFormat format, Color strokeColor, Color fillColor, System.Drawing.SizeF size, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return SignaturePadCanvas.GetImageStreamAsync (format, strokeColor, fillColor, size, shouldCrop, keepAspectRatio);
		}

		/// <summary>
		/// Create an encoded image of the currently drawn signature at the specified scale with the specified stroke and background colors.
		/// </summary>
		public Task<Stream> GetImageStreamAsync (SignatureImageFormat format, Color strokeColor, Color fillColor, float scale, bool shouldCrop = true, bool keepAspectRatio = true)
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

		private void UpdateUi ()
		{
			ClearLabel.Visibility = IsBlank ? ViewStates.Invisible : ViewStates.Visible;
		}
	}
}
