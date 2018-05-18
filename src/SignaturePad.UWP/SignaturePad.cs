using System;
using System.ComponentModel;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Xamarin.Controls
{
	[TemplatePart (Name = PartBackgroundImageView, Type = typeof (Image))]
	[TemplatePart (Name = PartSignaturePadCanvas, Type = typeof (SignaturePadCanvasView))]
	[TemplatePart (Name = PartCaption, Type = typeof (TextBlock))]
	[TemplatePart (Name = PartSignatureLine, Type = typeof (Border))]
	[TemplatePart (Name = PartSignaturePrompt, Type = typeof (TextBlock))]
	[TemplatePart (Name = PartClearLabel, Type = typeof (TextBlock))]
	public partial class SignaturePad : Control
	{
		private const string PartBackgroundImageView = "BackgroundImageView";
		private const string PartSignaturePadCanvas = "SignaturePadCanvas";
		private const string PartCaption = "Caption";
		private const string PartSignatureLine = "SignatureLine";
		private const string PartSignaturePrompt = "SignaturePrompt";
		private const string PartClearLabel = "ClearLabel";

		public static readonly DependencyProperty StrokeColorProperty;
		public static readonly DependencyProperty StrokeWidthProperty;
		public static readonly DependencyProperty SignatureLineBrushProperty;
		public static readonly DependencyProperty SignatureLineThicknessProperty;
		public static readonly DependencyProperty SignatureLineSpacingProperty;
		public static readonly DependencyProperty CaptionTextProperty;
		public static readonly DependencyProperty CaptionFontSizeProperty;
		public static readonly DependencyProperty CaptionForegroundProperty;
		public static readonly DependencyProperty SignaturePromptTextProperty;
		public static readonly DependencyProperty SignaturePromptFontSizeProperty;
		public static readonly DependencyProperty SignaturePromptForegroundProperty;
		public static readonly DependencyProperty ClearLabelTextProperty;
		public static readonly DependencyProperty ClearLabelFontSizeProperty;
		public static readonly DependencyProperty ClearLabelForegroundProperty;
		public static readonly DependencyProperty BackgroundImageProperty;
		public static readonly DependencyProperty BackgroundImageStretchProperty;
		public static readonly DependencyProperty BackgroundImageOpacityProperty;

		static SignaturePad ()
		{
			StrokeColorProperty = DependencyProperty.Register (
				nameof (StrokeColor),
				typeof (Color),
				typeof (SignaturePad),
				new PropertyMetadata (ImageConstructionSettings.DefaultStrokeColor));

			StrokeWidthProperty = DependencyProperty.Register (
				nameof (StrokeWidth),
				typeof (double),
				typeof (SignaturePad),
				new PropertyMetadata ((double)ImageConstructionSettings.DefaultStrokeWidth));

			SignatureLineBrushProperty = DependencyProperty.Register (
				nameof (SignatureLineBrush),
				typeof (Brush),
				typeof (SignaturePad),
				new PropertyMetadata (new SolidColorBrush (SignaturePadDarkColor)));

			SignatureLineThicknessProperty = DependencyProperty.Register (
				nameof (SignatureLineThickness),
				typeof (double),
				typeof (SignaturePad),
				new PropertyMetadata ((double)DefaultLineThickness));

			SignatureLineSpacingProperty = DependencyProperty.Register (
				nameof (SignatureLineSpacing),
				typeof (double),
				typeof (SignaturePad),
				new PropertyMetadata ((double)DefaultNarrowSpacing, OnPaddingChanged));

			CaptionTextProperty = DependencyProperty.Register (
				nameof (CaptionText),
				typeof (string),
				typeof (SignaturePad),
				new PropertyMetadata (DefaultCaptionText));

			CaptionFontSizeProperty = DependencyProperty.Register (
				nameof (CaptionFontSize),
				typeof (double),
				typeof (SignaturePad),
				new PropertyMetadata ((double)DefaultFontSize));

			CaptionForegroundProperty = DependencyProperty.Register (
				nameof (CaptionForeground),
				typeof (Brush),
				typeof (SignaturePad),
				new PropertyMetadata (new SolidColorBrush (SignaturePadDarkColor)));

			SignaturePromptTextProperty = DependencyProperty.Register (
				nameof (SignaturePromptText),
				typeof (string),
				typeof (SignaturePad),
				new PropertyMetadata (DefaultPromptText));

			SignaturePromptFontSizeProperty = DependencyProperty.Register (
				nameof (SignaturePromptFontSize),
				typeof (double),
				typeof (SignaturePad),
				new PropertyMetadata ((double)DefaultFontSize));

			SignaturePromptForegroundProperty = DependencyProperty.Register (
				nameof (SignaturePromptForeground),
				typeof (Brush),
				typeof (SignaturePad),
				new PropertyMetadata (new SolidColorBrush (SignaturePadDarkColor)));

			ClearLabelTextProperty = DependencyProperty.Register (
				nameof (ClearLabelText),
				typeof (string),
				typeof (SignaturePad),
				new PropertyMetadata (DefaultClearLabelText));

			ClearLabelFontSizeProperty = DependencyProperty.Register (
				nameof (ClearLabelFontSize),
				typeof (double),
				typeof (SignaturePad),
				new PropertyMetadata ((double)DefaultFontSize));

			ClearLabelForegroundProperty = DependencyProperty.Register (
				nameof (ClearLabelForeground),
				typeof (Brush),
				typeof (SignaturePad),
				new PropertyMetadata (new SolidColorBrush (SignaturePadDarkColor)));

			BackgroundImageProperty = DependencyProperty.Register (
				nameof (BackgroundImage),
				typeof (ImageSource),
				typeof (SignaturePad),
				new PropertyMetadata (null));

			BackgroundImageStretchProperty = DependencyProperty.Register (
				nameof (BackgroundImageStretch),
				typeof (Stretch),
				typeof (SignaturePad), new PropertyMetadata (Stretch.None));

			BackgroundImageOpacityProperty = DependencyProperty.Register (
				nameof (BackgroundImageOpacity),
				typeof (double),
				typeof (SignaturePad),
				new PropertyMetadata (1.0));
		}

		public SignaturePad ()
		{
			DefaultStyleKey = typeof (SignaturePad);

			RegisterPropertyChangedCallback (PaddingProperty, OnPaddingChanged);

			Padding = new Thickness (DefaultWideSpacing, DefaultWideSpacing, DefaultWideSpacing, DefaultNarrowSpacing);
		}

		protected override void OnApplyTemplate ()
		{
			SignaturePadCanvas.StrokeCompleted += delegate
			{
				OnSignatureStrokeCompleted ();
			};
			SignaturePadCanvas.Cleared += delegate
			{
				OnSignatureCleared ();
			};
			ClearLabel.Tapped += delegate
			{
				OnClearTapped ();
			};

			OnPaddingChanged (this, PaddingProperty);
			UpdateUi ();
		}

		/// <summary>
		/// The real signature canvas view
		/// </summary>
		public SignaturePadCanvasView SignaturePadCanvas => GetTemplateChild (PartSignaturePadCanvas) as SignaturePadCanvasView;

		/// <summary>
		/// The prompt displayed at the beginning of the signature line.
		/// </summary>
		public TextBlock SignaturePrompt => GetTemplateChild (PartSignaturePrompt) as TextBlock;

		/// <summary>
		/// The caption displayed under the signature line.
		/// </summary>
		public TextBlock Caption => GetTemplateChild (PartCaption) as TextBlock;

		/// <summary>
		/// An image view that may be used as a watermark or as a texture for the signature pad.
		/// </summary>
		public Image BackgroundImageView => GetTemplateChild (PartBackgroundImageView) as Image;

		/// <summary>
		/// Gets the label that clears the pad when clicked.
		/// </summary>
		public TextBlock ClearLabel => GetTemplateChild (PartClearLabel) as TextBlock;

		/// <summary>
		/// Gets the horizontal line that goes in the lower part of the pad.
		/// </summary>
		public Border SignatureLine => GetTemplateChild (PartSignatureLine) as Border;

		public Color StrokeColor
		{
			get { return (Color)GetValue (StrokeColorProperty); }
			set { SetValue (StrokeColorProperty, value); }
		}

		public double StrokeWidth
		{
			get { return (double)GetValue (StrokeWidthProperty); }
			set { SetValue (StrokeWidthProperty, value); }
		}

		public Brush SignatureLineBrush
		{
			get { return (Brush)GetValue (SignatureLineBrushProperty); }
			set { SetValue (SignatureLineBrushProperty, value); }
		}

		public Thickness SignatureLineThickness
		{
			get { return (Thickness)GetValue (SignatureLineThicknessProperty); }
			set { SetValue (SignatureLineThicknessProperty, value); }
		}

		public double SignatureLineSpacing
		{
			get { return (double)GetValue (SignatureLineSpacingProperty); }
			set { SetValue (SignatureLineSpacingProperty, value); }
		}

		public string CaptionText
		{
			get { return (string)GetValue (CaptionTextProperty); }
			set { SetValue (CaptionTextProperty, value); }
		}

		public double CaptionFontSize
		{
			get { return (double)GetValue (CaptionFontSizeProperty); }
			set { SetValue (CaptionFontSizeProperty, value); }
		}

		public Brush CaptionForeground
		{
			get { return (Brush)GetValue (CaptionForegroundProperty); }
			set { SetValue (CaptionForegroundProperty, value); }
		}

		public string SignaturePromptText
		{
			get { return (string)GetValue (SignaturePromptTextProperty); }
			set { SetValue (SignaturePromptTextProperty, value); }
		}

		public double SignaturePromptFontSize
		{
			get { return (double)GetValue (SignaturePromptFontSizeProperty); }
			set { SetValue (SignaturePromptFontSizeProperty, value); }
		}

		public Brush SignaturePromptForeground
		{
			get { return (Brush)GetValue (SignaturePromptForegroundProperty); }
			set { SetValue (SignaturePromptForegroundProperty, value); }
		}

		public string ClearLabelText
		{
			get { return (string)GetValue (ClearLabelTextProperty); }
			set { SetValue (ClearLabelTextProperty, value); }
		}

		public double ClearLabelFontSize
		{
			get { return (double)GetValue (ClearLabelFontSizeProperty); }
			set { SetValue (ClearLabelFontSizeProperty, value); }
		}

		public Brush ClearLabelForeground
		{
			get { return (Brush)GetValue (ClearLabelForegroundProperty); }
			set { SetValue (ClearLabelForegroundProperty, value); }
		}

		public ImageSource BackgroundImage
		{
			get { return (ImageSource)GetValue (BackgroundImageProperty); }
			set { SetValue (BackgroundImageProperty, value); }
		}

		public Stretch BackgroundImageStretch
		{
			get { return (Stretch)GetValue (BackgroundImageStretchProperty); }
			set { SetValue (BackgroundImageStretchProperty, value); }
		}

		public double BackgroundImageOpacity
		{
			get { return (double)GetValue (BackgroundImageOpacityProperty); }
			set { SetValue (BackgroundImageOpacityProperty, value); }
		}

		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ("Use Background instead.")]
		public Color BackgroundColor
		{
			get
			{
				var scb = Background as SolidColorBrush;
				return scb == null ? Colors.Transparent : scb.Color;
			}
			set { Background = new SolidColorBrush (value); }
		}

		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ("Use SignatureLineBrush instead.")]
		public Color SignatureLineColor
		{
			get
			{
				var scb = SignatureLineBrush as SolidColorBrush;
				return scb == null ? Colors.Transparent : scb.Color;
			}
			set { SignatureLineBrush = new SolidColorBrush (value); }
		}

		private void UpdateUi ()
		{
			ClearLabel.Visibility = IsBlank ? Visibility.Collapsed : Visibility.Visible;
		}

		private void OnPaddingChanged (DependencyObject sender, DependencyProperty dp)
		{
			var padding = Padding;
			var spacing = SignatureLineSpacing;

			if (SignatureLine != null)
			{
				SignatureLine.Margin = new Thickness (padding.Left, 0, padding.Right, 0);
			}
			if (Caption != null)
			{
				Caption.Margin = new Thickness (0, spacing, 0, padding.Bottom);
			}
			if (ClearLabel != null)
			{
				ClearLabel.Margin = new Thickness (0, padding.Top, padding.Right, 0);
			}
			if (SignaturePrompt != null)
			{
				SignaturePrompt.Margin = new Thickness (padding.Left, 0, 0, spacing);
			}
		}

		private static void OnPaddingChanged (DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((SignaturePad)d).OnPaddingChanged (d, e.Property);
		}
	}
}
