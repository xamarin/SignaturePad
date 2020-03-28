using System;
using System.ComponentModel;

namespace Xamarin.Controls
{
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Media;
	using Gtk;
	using Brush = System.Drawing.Brush;
	using Color = Gdk.Color;
	using Image = System.Windows.Controls.Image;
	using Stretch = Pango.Stretch;

	[TemplatePart (Name = PartBackgroundImageView, Type = typeof (Image))]
	[TemplatePart (Name = PartSignaturePadCanvas, Type = typeof (SignaturePadCanvasView))]
	[TemplatePart (Name = PartCaption, Type = typeof (TextView))]
	[TemplatePart (Name = PartSignatureLine, Type = typeof (Border))]
	[TemplatePart (Name = PartSignaturePrompt, Type = typeof (TextView))]
	[TemplatePart (Name = PartClearLabel, Type = typeof (TextView))]
	public partial class SignaturePad : Widget
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
			//DefaultStyleKeyProperty.OverrideMetadata (
			//	typeof (SignaturePad),
			//	new FrameworkPropertyMetadata (typeof (SignaturePad)));


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
				new PropertyMetadata (new SolidColorBrush (Colors.Black)));

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
				new PropertyMetadata (new SolidColorBrush (Colors.Black)));

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
				new PropertyMetadata (new SolidColorBrush (Colors.Black)));

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
				new PropertyMetadata (new SolidColorBrush (Colors.Black)));

			BackgroundImageProperty = DependencyProperty.Register (
				nameof (BackgroundImage),
				typeof (ImageSource),
				typeof (SignaturePad),
				new PropertyMetadata (null));

			BackgroundImageStretchProperty = DependencyProperty.Register (
				nameof (BackgroundImageStretch),
				typeof (Stretch),
				typeof (SignaturePad), new PropertyMetadata (Stretch.Normal));

			BackgroundImageOpacityProperty = DependencyProperty.Register (
				nameof (BackgroundImageOpacity),
				typeof (double),
				typeof (SignaturePad),
				new PropertyMetadata (1.0));
		}

		public SignaturePad ()
		{
			//DefaultStyleKey = typeof (SignaturePad);

			//RegisterPropertyChangedCallback (PaddingProperty, OnPaddingChanged);
			
			//Padding = new Thickness (DefaultWideSpacing, DefaultWideSpacing, DefaultWideSpacing, DefaultNarrowSpacing);
			//OnApplyTemplate();
		}

		/// <inheritdoc />
		//public override void OnApplyTemplate()
		//{
		//	base.OnApplyTemplate();
		
		//	SignaturePadCanvas.StrokeCompleted += delegate
		//	{
		//		OnSignatureStrokeCompleted ();
		//	};
		//	SignaturePadCanvas.Cleared += delegate
		//	{
		//		OnSignatureCleared ();
		//	};
		//	ClearLabel.MouseDown += delegate
		//	{
		//		OnClearTapped ();
		//	};

		//	OnPaddingChanged (this, PaddingProperty);
		//	UpdateUi ();
		//}

		/// <summary>
		/// The real signature canvas view
		/// </summary>
		public SignaturePadCanvasView SignaturePadCanvas => new SignaturePadCanvasView();

		/// <summary>
		/// The prompt displayed at the beginning of the signature line.
		/// </summary>
		public TextView SignaturePrompt => new TextView();

		/// <summary>
		/// The caption displayed under the signature line.
		/// </summary>
		public TextView Caption => new TextView ();

		/// <summary>
		/// An image view that may be used as a watermark or as a texture for the signature pad.
		/// </summary>
		public Image BackgroundImageView =>new Image();

		/// <summary>
		/// Gets the label that clears the pad when clicked.
		/// </summary>
		public TextView ClearLabel => new TextView ();

		/// <summary>
		/// Gets the horizontal line that goes in the lower part of the pad.
		/// </summary>
		public Border SignatureLine => new Border();

		public Color StrokeColor
		{
			get;
			set;
		}

		public double StrokeWidth
		{
			get;
			set;
		}

		public Brush SignatureLineBrush
		{
			get;
			set;
		}

		public Thickness SignatureLineThickness
		{
			get;
			set;
		}

		public double SignatureLineSpacing
		{
			get;
			set;
		}

		public string CaptionText
		{
			get;
			set;
		}

		public double CaptionFontSize
		{
			get;
			set;
		}

		public Brush CaptionForeground
		{
			get;
			set;
		}

		public string SignaturePromptText
		{
			get;
			set;
		}

		public double SignaturePromptFontSize
		{
			get;
			set;
		}

		public Brush SignaturePromptForeground
		{
			get;
			set;
		}

		public string ClearLabelText
		{
			get;
			set;
		}

		public double ClearLabelFontSize
		{
			get;
			set;
		}

		public Brush ClearLabelForeground
		{
			get;
			set;
		}

		public ImageSource BackgroundImage
		{
			get;
			set;
		}

		public Stretch BackgroundImageStretch
		{
			get;
			set;
		}

		public double BackgroundImageOpacity
		{
			get;
			set;
		}

		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ("Use Background instead.")]
		public Color BackgroundColor
		{
			get;
			set;
		}

		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ("Use SignatureLineBrush instead.")]
		public Color SignatureLineColor
		{
			get;
			set;
		}

		private void UpdateUi ()
		{
			//ClearLabel.Visibility = IsBlank ? Visibility.Collapsed : Visibility.Visible;
		}

		//private void OnPaddingChanged (DependencyObject sender, DependencyProperty dp)
		//{
		//	var padding = Padding;
		//	var spacing = SignatureLineSpacing;

		//	if (SignatureLine != null)
		//	{
		//		SignatureLine.Margin = new Thickness (padding.Left, 0, padding.Right, 0);
		//	}
		//	if (Caption != null)
		//	{
		//		Caption.Margin = new Thickness (0, spacing, 0, padding.Bottom);
		//	}
		//	if (ClearLabel != null)
		//	{
		//		ClearLabel.Margin = new Thickness (0, padding.Top, padding.Right, 0);
		//	}
		//	if (SignaturePrompt != null)
		//	{
		//		SignaturePrompt.Margin = new Thickness (padding.Left, 0, 0, spacing);
		//	}
		//}

		private static void OnPaddingChanged (DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			//((SignaturePad)d).OnPaddingChanged (d, e.Property);
		}
	}
}
