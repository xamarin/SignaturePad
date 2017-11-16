using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Xamarin.Controls
{
	public class SignaturePad : ContentControl
	{
		public SignaturePad ()
		{
			Initialize ();
		}

		private void Initialize ()
		{
			const int ThinPad = 3;
			const int ThickPad = 20;
			const int LineHeight = 2;

			var grid = new Grid ();

			grid.RowDefinitions.Add (new RowDefinition { Height = new GridLength (1, GridUnitType.Star) });
			grid.RowDefinitions.Add (new RowDefinition { Height = GridLength.Auto });

			// add the background view
			{
				BackgroundImageView = new Image ();
				BackgroundImageView.SetValue (Grid.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
				BackgroundImageView.SetValue (Grid.VerticalAlignmentProperty, VerticalAlignment.Stretch);
				BackgroundImageView.SetValue (Grid.RowProperty, 0);
				grid.Children.Add (BackgroundImageView);
			}

			// add the main signature view
			{
				SignaturePadCanvas = new SignaturePadCanvasView ();
				SignaturePadCanvas.SetValue (Grid.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
				SignaturePadCanvas.SetValue (Grid.VerticalAlignmentProperty, VerticalAlignment.Stretch);
				SignaturePadCanvas.SetValue (Grid.RowProperty, 0);
				SignaturePadCanvas.StrokeCompleted += (sender, e) =>
				{
					UpdateUi ();
					StrokeCompleted?.Invoke (this, EventArgs.Empty);
				};
				SignaturePadCanvas.Cleared += (sender, e) => Cleared?.Invoke (this, EventArgs.Empty);
				grid.Children.Add (SignaturePadCanvas);
			}

			// add the caption
			{
				Caption = new TextBlock ()
				{
					Text = "Sign here.",
					FontSize = 11,
					Foreground = new SolidColorBrush (Colors.Gray),
					TextAlignment = TextAlignment.Center,
					Margin = new Thickness (ThinPad)
				};
				Caption.SetValue (Grid.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
				Caption.SetValue (Grid.VerticalAlignmentProperty, VerticalAlignment.Bottom);
				Caption.SetValue (Grid.RowProperty, 1);
				grid.Children.Add (Caption);
			}

			// add the signature line
			{
				SignatureLine = new Border ()
				{
					Background = new SolidColorBrush (Colors.Gray),
					Height = LineHeight,
					Margin = new Thickness (ThickPad, 0, ThickPad, 0)
				};
				SignatureLine.SetValue (Grid.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
				SignatureLine.SetValue (Grid.VerticalAlignmentProperty, VerticalAlignment.Bottom);
				grid.Children.Add (SignatureLine);
			}

			// add the prompt
			{
				SignaturePrompt = new TextBlock ()
				{
					Text = "X",
					FontSize = 20,
					FontWeight = FontWeights.Bold,
					Margin = new Thickness (ThickPad, 0, 0, ThinPad)
				};
				SignaturePrompt.SetValue (Grid.HorizontalAlignmentProperty, HorizontalAlignment.Left);
				SignaturePrompt.SetValue (Grid.VerticalAlignmentProperty, VerticalAlignment.Bottom);
				grid.Children.Add (SignaturePrompt);
			}

			// add the clear label
			{
				ClearLabel = new TextBlock ()
				{
					Text = "Clear",
					FontSize = 11,
					FontWeight = FontWeights.Bold,
					Visibility = Visibility.Collapsed,
					Foreground = new SolidColorBrush (Colors.Gray),
					Margin = new Thickness (0, ThickPad, ThickPad, 0)
				};
				ClearLabel.SetValue (Grid.HorizontalAlignmentProperty, HorizontalAlignment.Right);
				ClearLabel.SetValue (Grid.VerticalAlignmentProperty, VerticalAlignment.Top);
				grid.Children.Add (ClearLabel);

				// attach the "clear" command
				ClearLabel.Tapped += (sender, e) => Clear ();
			}

			HorizontalContentAlignment = HorizontalAlignment.Stretch;
			VerticalContentAlignment = VerticalAlignment.Stretch;
			Content = grid;

			// clear / initialize the view
			Clear ();
		}

		public Point[][] Strokes => SignaturePadCanvas.Strokes;

		public Point[] Points => SignaturePadCanvas.Points;

		public bool IsBlank => SignaturePadCanvas.IsBlank;

		public SignaturePadCanvasView SignaturePadCanvas { get; private set; }

		public Color StrokeColor
		{
			get { return SignaturePadCanvas.StrokeColor; }
			set { SignaturePadCanvas.StrokeColor = value; }
		}

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
		public TextBlock SignaturePrompt { get; private set; }

		/// <summary>
		/// The caption displayed under the signature line.
		/// </summary>
		/// <remarks>
		/// Text value defaults to 'Sign here.'
		/// </remarks>
		/// <value>The caption.</value>
		public TextBlock Caption { get; private set; }

		/// <summary>
		/// The brush of the signature line.
		/// </summary>
		/// <value>The brush of the signature line.</value>
		public Brush SignatureLineBrush
		{
			get { return SignatureLine.Background; }
			set { SignatureLine.Background = value; }
		}

		/// <summary>
		/// The color of the signature line.
		/// </summary>
		/// <value>The color of the signature line.</value>
		public Color SignatureLineColor
		{
			get
			{
				var scb = SignatureLine.Background as SolidColorBrush;
				return scb == null ? Colors.Transparent : scb.Color;
			}
			set { SignatureLine.Background = new SolidColorBrush (value); }
		}

		/// <summary>
		///  An image view that may be used as a watermark or as a texture
		///  for the signature pad.
		/// </summary>
		/// <value>The background image view.</value>
		public Image BackgroundImageView { get; private set; }

		/// <summary>
		/// The color of the background.
		/// </summary>
		/// <value>The color of the background.</value>
		public Color BackgroundColor
		{
			get
			{
				var scb = Background as SolidColorBrush;
				return scb == null ? Colors.Transparent : scb.Color;
			}
			set { Background = new SolidColorBrush (value); }
		}

		/// <summary>
		///  An image view that may be used as a watermark or as a texture
		///  for the signature pad.
		/// </summary>
		/// <value>The background image.</value>
		public ImageSource BackgroundImage
		{
			get { return BackgroundImageView.Source; }
			set { BackgroundImageView.Source = value; }
		}

		/// <summary>
		///  An image view that may be used as a watermark or as a texture
		///  for the signature pad.
		/// </summary>
		/// <value>The background image.</value>
		public Stretch BackgroundImageStretch
		{
			get { return BackgroundImageView.Stretch; }
			set { BackgroundImageView.Stretch = value; }
		}

		/// <summary>
		///  The transparency of the watermark.
		/// </summary>
		/// <value>The background image.</value>
		public double BackgroundImageOpacity
		{
			get { return BackgroundImageView.Opacity; }
			set { BackgroundImageView.Opacity = value; }
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
		/// Gets the text for the label that clears the pad when clicked.
		/// </summary>
		/// <value>The clear label.</value>
		public string ClearLabelText
		{
			get { return ClearLabel.Text; }
			set { ClearLabel.Text = value; }
		}

		/// <summary>
		/// Gets the label that clears the pad when clicked.
		/// </summary>
		/// <value>The clear label.</value>
		public TextBlock ClearLabel { get; private set; }

		/// <summary>
		/// Gets the horizontal line that goes in the lower part of the pad.
		/// </summary>
		/// <value>The signature line.</value>
		public Border SignatureLine { get; private set; }

		public event EventHandler StrokeCompleted;

		public event EventHandler Cleared;

		public void Clear ()
		{
			SignaturePadCanvas.Clear ();

			UpdateUi ();
		}
		
		public void LoadPoints (Point[] points)
		{
			SignaturePadCanvas.LoadPoints (points);

			UpdateUi ();
		}

		public void LoadStrokes (Point[][] strokes)
		{
			SignaturePadCanvas.LoadStrokes (strokes);

			UpdateUi ();
		}

		/// <summary>
		/// Create an image of the currently drawn signature.
		/// </summary>
		public WriteableBitmap GetImage (bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return SignaturePadCanvas.GetImage (shouldCrop, keepAspectRatio);
		}

		/// <summary>
		/// Create an image of the currently drawn signature at the specified size.
		/// </summary>
		public WriteableBitmap GetImage (Size size, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return SignaturePadCanvas.GetImage (size, shouldCrop, keepAspectRatio);
		}

		/// <summary>
		/// Create an image of the currently drawn signature at the specified scale.
		/// </summary>
		public WriteableBitmap GetImage (float scale, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return SignaturePadCanvas.GetImage (scale, shouldCrop, keepAspectRatio);
		}

		/// <summary>
		/// Create an image of the currently drawn signature with the specified stroke color.
		/// </summary>
		public WriteableBitmap GetImage (Color strokeColor, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return SignaturePadCanvas.GetImage (strokeColor, shouldCrop, keepAspectRatio);
		}

		/// <summary>
		/// Create an image of the currently drawn signature at the specified size with the specified stroke color.
		/// </summary>
		public WriteableBitmap GetImage (Color strokeColor, Size size, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return SignaturePadCanvas.GetImage (strokeColor, size, shouldCrop, keepAspectRatio);
		}

		/// <summary>
		/// Create an image of the currently drawn signature at the specified scale with the specified stroke color.
		/// </summary>
		public WriteableBitmap GetImage (Color strokeColor, float scale, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return SignaturePadCanvas.GetImage (strokeColor, scale, shouldCrop, keepAspectRatio);
		}

		/// <summary>
		/// Create an image of the currently drawn signature with the specified stroke and background colors.
		/// </summary>
		public WriteableBitmap GetImage (Color strokeColor, Color fillColor, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return SignaturePadCanvas.GetImage (strokeColor, fillColor, shouldCrop, keepAspectRatio);
		}

		/// <summary>
		/// Create an image of the currently drawn signature at the specified size with the specified stroke and background colors.
		/// </summary>
		public WriteableBitmap GetImage (Color strokeColor, Color fillColor, Size size, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return SignaturePadCanvas.GetImage (strokeColor, fillColor, size, shouldCrop, keepAspectRatio);
		}

		/// <summary>
		/// Create an image of the currently drawn signature at the specified scale with the specified stroke and background colors.
		/// </summary>
		public WriteableBitmap GetImage (Color strokeColor, Color fillColor, float scale, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return SignaturePadCanvas.GetImage (strokeColor, fillColor, scale, shouldCrop, keepAspectRatio);
		}

		/// <summary>
		/// Create an image of the currently drawn signature using the specified settings.
		/// </summary>
		public WriteableBitmap GetImage (ImageConstructionSettings settings)
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
			ClearLabel.Visibility = IsBlank ? Visibility.Collapsed : Visibility.Visible;
		}

	}
}
