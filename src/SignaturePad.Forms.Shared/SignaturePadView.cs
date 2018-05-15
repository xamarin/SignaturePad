using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SignaturePad.Forms
{
	public partial class SignaturePadView : Grid
	{
		public static readonly BindableProperty CaptionTextProperty = BindableProperty.Create (
			nameof (CaptionText),
			typeof (string),
			typeof (SignaturePadView),
			(string)null,
			propertyChanged: (bindable, oldValue, newValue) => ((SignaturePadView)bindable).CaptionLabel.Text = (string)newValue);

		public static readonly BindableProperty CaptionTextColorProperty = BindableProperty.Create (
			nameof (CaptionTextColor),
			typeof (Color),
			typeof (SignaturePadView),
			Color.Default,
			propertyChanged: (bindable, oldValue, newValue) => ((SignaturePadView)bindable).CaptionLabel.TextColor = (Color)newValue);

		public static readonly BindableProperty ClearTextProperty = BindableProperty.Create (
			nameof (ClearText),
			typeof (string),
			typeof (SignaturePadView),
			(string)null,
			propertyChanged: (bindable, oldValue, newValue) => ((SignaturePadView)bindable).ClearLabel.Text = (string)newValue);

		public static readonly BindableProperty ClearTextColorProperty = BindableProperty.Create (
			nameof (ClearTextColor),
			typeof (Color),
			typeof (SignaturePadView),
			Color.Default,
			propertyChanged: (bindable, oldValue, newValue) => ((SignaturePadView)bindable).ClearLabel.TextColor = (Color)newValue);

		public static readonly BindableProperty PromptTextProperty = BindableProperty.Create (
			nameof (PromptText),
			typeof (string),
			typeof (SignaturePadView),
			(string)null,
			propertyChanged: (bindable, oldValue, newValue) => ((SignaturePadView)bindable).SignaturePrompt.Text = (string)newValue);

		public static readonly BindableProperty PromptTextColorProperty = BindableProperty.Create (
			nameof (PromptTextColor),
			typeof (Color),
			typeof (SignaturePadView),
			Color.Default,
			propertyChanged: (bindable, oldValue, newValue) => ((SignaturePadView)bindable).SignaturePrompt.TextColor = (Color)newValue);

		public static readonly BindableProperty SignatureLineColorProperty = BindableProperty.Create (
			nameof (SignatureLineColor),
			typeof (Color),
			typeof (SignaturePadView),
			Color.Default,
			propertyChanged: (bindable, oldValue, newValue) => ((SignaturePadView)bindable).SignatureLine.Color = (Color)newValue);

		public static readonly BindableProperty StrokeColorProperty = BindableProperty.Create (
			nameof (StrokeColor),
			typeof (Color),
			typeof (SignaturePadView),
			Color.Default,
			propertyChanged: (bindable, oldValue, newValue) => ((SignaturePadView)bindable).SignaturePadCanvas.StrokeColor = (Color)newValue);

		public static readonly BindableProperty StrokeWidthProperty = BindableProperty.Create (
			nameof (StrokeWidth),
			typeof (float),
			typeof (SignaturePadView),
			(float)0,
			propertyChanged: (bindable, oldValue, newValue) => ((SignaturePadView)bindable).SignaturePadCanvas.StrokeWidth = (float)newValue);

		public static readonly BindableProperty BackgroundImageProperty = BindableProperty.Create (
			nameof (BackgroundImage),
			typeof (ImageSource),
			typeof (SignaturePadView),
			(ImageSource)null,
			propertyChanged: (bindable, oldValue, newValue) => ((SignaturePadView)bindable).BackgroundImageView.Source = (ImageSource)newValue);

		public static readonly BindableProperty BackgroundImageAspectProperty = BindableProperty.Create (
			nameof (BackgroundImageAspect),
			typeof (Aspect),
			typeof (SignaturePadView),
			Aspect.AspectFit,
			propertyChanged: (bindable, oldValue, newValue) => ((SignaturePadView)bindable).BackgroundImageView.Aspect = (Aspect)newValue);

		public static readonly BindableProperty BackgroundImageOpacityProperty = BindableProperty.Create (
			nameof (BackgroundImageOpacity),
			typeof (double),
			typeof (SignaturePadView),
			(double)0,
			propertyChanged: (bindable, oldValue, newValue) => ((SignaturePadView)bindable).BackgroundImageView.Opacity = (double)newValue);

		private TapGestureRecognizer clearLabelTap;

		public SignaturePadView ()
		{
			Initialize ();
		}

		private void Initialize ()
		{
			const int ThinPad = 3;
			const int ThickPad = 12;
			const int LineHeight = 2;

			RowSpacing = 0;
			ColumnSpacing = 0;

			// create the chrome layout
			var chromeStack = new StackLayout ();
			chromeStack.Spacing = 0;
			chromeStack.Padding = 0;
			chromeStack.Margin = 0;
			Children.Add (chromeStack);

			// add the background view
			{
				BackgroundImageView = new Image ();
				BackgroundImageView.SetValue (View.VerticalOptionsProperty, LayoutOptions.FillAndExpand);
				chromeStack.Children.Add (BackgroundImageView);
			}

			// add the prompt
			{
				SignaturePrompt = new Label
				{
					Text = "X",
					FontSize = 20,
					FontAttributes = FontAttributes.Bold,
					Margin = new Thickness (ThickPad, 0, 0, 0)
				};
				chromeStack.Children.Add (SignaturePrompt);
			}

			// add the signature line
			{
				SignatureLine = new BoxView
				{
					BackgroundColor = Color.Gray,
					HeightRequest = LineHeight,
					Margin = new Thickness (ThickPad, 0, ThickPad, 0)
				};
				chromeStack.Children.Add (SignatureLine);
			}

			// add the caption
			{
				CaptionLabel = new Label
				{
					Text = "Sign here.",
					FontSize = 11,
					TextColor = Color.Gray,
					HorizontalTextAlignment = TextAlignment.Center,
					Margin = new Thickness (ThinPad)
				};
				chromeStack.Children.Add (CaptionLabel);
			}

			// add the main signature view
			{
				SignaturePadCanvas = new SignaturePadCanvasView ();
				SignaturePadCanvas.SetValue (View.HorizontalOptionsProperty, LayoutOptions.Fill);
				SignaturePadCanvas.SetValue (View.VerticalOptionsProperty, LayoutOptions.Fill);
				SignaturePadCanvas.StrokeCompleted += (sender, e) =>
				{
					UpdateUi ();
					StrokeCompleted?.Invoke (this, EventArgs.Empty);
				};
				SignaturePadCanvas.Cleared += (sender, e) => Cleared?.Invoke (this, EventArgs.Empty);
				Children.Add (SignaturePadCanvas);
			}

			// add the clear label
			{
				ClearLabel = new Label
				{
					Text = "Clear",
					FontSize = 11,
					FontAttributes = FontAttributes.Bold,
					IsVisible = false,
					TextColor = Color.Gray,
					Margin = new Thickness (0, ThickPad, ThickPad, 0)
				};
				ClearLabel.SetValue (View.HorizontalOptionsProperty, LayoutOptions.End);
				ClearLabel.SetValue (View.VerticalOptionsProperty, LayoutOptions.Start);
				Children.Add (ClearLabel);

				// attach the "clear" command
				clearLabelTap = new TapGestureRecognizer { Command = new Command (() => Clear ()) };
				ClearLabel.GestureRecognizers.Add (clearLabelTap);
			}

			// clear / initialize the view
			Clear ();
		}

		protected override void OnPropertyChanged ([CallerMemberName] string propertyName = null)
		{
			base.OnPropertyChanged (propertyName);

			if (propertyName == IsEnabledProperty.PropertyName)
			{
				SignaturePadCanvas.IsEnabled = IsEnabled;
				if (IsEnabled)
					ClearLabel.GestureRecognizers.Add (clearLabelTap);
				else
					ClearLabel.GestureRecognizers.Remove (clearLabelTap);
			}
		}

		public IEnumerable<IEnumerable<Point>> Strokes
		{
			get { return SignaturePadCanvas.Strokes; }
			set
			{
				SignaturePadCanvas.Strokes = value;
				UpdateUi ();
			}
		}

		public IEnumerable<Point> Points
		{
			get { return SignaturePadCanvas.Points; }
			set
			{
				SignaturePadCanvas.Points = value;
				UpdateUi ();
			}
		}

		public bool IsBlank => SignaturePadCanvas.IsBlank;

		public SignaturePadCanvasView SignaturePadCanvas { get; private set; }

		public Color StrokeColor
		{
			get { return (Color)GetValue (StrokeColorProperty); }
			set { SetValue (StrokeColorProperty, value); }
		}

		public float StrokeWidth
		{
			get { return (float)GetValue (StrokeWidthProperty); }
			set { SetValue (StrokeWidthProperty, value); }
		}

		/// <summary>
		/// The prompt displayed at the beginning of the signature line.
		/// </summary>
		/// <remarks>
		/// Text value defaults to 'X'.
		/// </remarks>
		/// <value>The signature prompt.</value>
		public Label SignaturePrompt { get; private set; }

		/// <summary>
		/// The caption displayed under the signature line.
		/// </summary>
		/// <remarks>
		/// Text value defaults to 'Sign here.'
		/// </remarks>
		/// <value>The caption.</value>
		public Label CaptionLabel { get; private set; }

		/// <summary>
		/// The color of the caption text.
		/// </summary>
		/// <value>The color of the caption text.</value>
		public Color CaptionTextColor
		{
			get { return (Color)GetValue (CaptionTextColorProperty); }
			set { SetValue (CaptionTextColorProperty, value); }
		}

		/// <summary>
		/// The color of the signature line.
		/// </summary>
		/// <value>The color of the signature line.</value>
		public Color SignatureLineColor
		{
			get { return (Color)GetValue (SignatureLineColorProperty); }
			set { SetValue (SignatureLineColorProperty, value); }
		}

		/// <summary>
		///  An image view that may be used as a watermark or as a texture
		///  for the signature pad.
		/// </summary>
		/// <value>The background image view.</value>
		public Image BackgroundImageView { get; private set; }

		/// <summary>
		///  An image view that may be used as a watermark or as a texture
		///  for the signature pad.
		/// </summary>
		/// <value>The background image.</value>
		public ImageSource BackgroundImage
		{
			get { return (ImageSource)GetValue (BackgroundImageProperty); }
			set { SetValue (BackgroundImageProperty, value); }
		}

		/// <summary>
		///  An image view that may be used as a watermark or as a texture
		///  for the signature pad.
		/// </summary>
		/// <value>The background image.</value>
		public Aspect BackgroundImageAspect
		{
			get { return (Aspect)GetValue (BackgroundImageAspectProperty); }
			set { SetValue (BackgroundImageAspectProperty, value); }
		}

		/// <summary>
		///  The transparency of the watermark.
		/// </summary>
		/// <value>The background image.</value>
		public double BackgroundImageOpacity
		{
			get { return (double)GetValue (BackgroundImageOpacityProperty); }
			set { SetValue (BackgroundImageOpacityProperty, value); }
		}

		/// <summary>
		/// The text for the prompt displayed at the beginning of the signature line.
		/// </summary>
		/// <remarks>
		/// Text value defaults to 'X'.
		/// </remarks>
		/// <value>The signature prompt.</value>
		public string PromptText
		{
			get { return (string)GetValue (PromptTextProperty); }
			set { SetValue (PromptTextProperty, value); }
		}

		/// <summary>
		/// The color of the prompt text.
		/// </summary>
		/// <value>The color of the prompt text.</value>
		public Color PromptTextColor
		{
			get { return (Color)GetValue (PromptTextColorProperty); }
			set { SetValue (PromptTextColorProperty, value); }
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
			get { return (string)GetValue (CaptionTextProperty); }
			set { SetValue (CaptionTextProperty, value); }
		}

		/// <summary>
		/// Gets the text for the label that clears the pad when clicked.
		/// </summary>
		/// <value>The clear label.</value>
		public string ClearText
		{
			get { return (string)GetValue (ClearTextProperty); }
			set { SetValue (ClearTextProperty, value); }
		}

		/// <summary>
		/// The color of the clear text.
		/// </summary>
		/// <value>The color of the clear text.</value>
		public Color ClearTextColor
		{
			get { return (Color)GetValue (ClearTextColorProperty); }
			set { SetValue (ClearTextColorProperty, value); }
		}

		/// <summary>
		/// Gets the label that clears the pad when clicked.
		/// </summary>
		/// <value>The clear label.</value>
		public Label ClearLabel { get; private set; }

		/// <summary>
		/// Gets the horizontal line that goes in the lower part of the pad.
		/// </summary>
		/// <value>The signature line.</value>
		public BoxView SignatureLine { get; private set; }

		public event EventHandler StrokeCompleted;

		public event EventHandler Cleared;

		public void Clear ()
		{
			SignaturePadCanvas.Clear ();

			UpdateUi ();
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
		public Task<Stream> GetImageStreamAsync (SignatureImageFormat format, Size size, bool shouldCrop = true, bool keepAspectRatio = true)
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
		public Task<Stream> GetImageStreamAsync (SignatureImageFormat format, Color strokeColor, Size size, bool shouldCrop = true, bool keepAspectRatio = true)
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
		public Task<Stream> GetImageStreamAsync (SignatureImageFormat format, Color strokeColor, Color fillColor, Size size, bool shouldCrop = true, bool keepAspectRatio = true)
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
			ClearLabel.IsVisible = !IsBlank;
		}
	}
}
