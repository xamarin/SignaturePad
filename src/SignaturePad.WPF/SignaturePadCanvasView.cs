using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

namespace Xamarin.Controls
{
	using System.Drawing;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Ink;
	using System.Windows.Media;
	using System.Windows.Media.Imaging;
	using System.Windows.Threading;

	[TemplatePart (Name = PartInkCanvas, Type = typeof (InkCanvas))]
	public partial class SignaturePadCanvasView : Control
	{
		public static readonly DependencyProperty StrokeColorProperty;
		public static readonly DependencyProperty StrokeWidthProperty;

		private const string PartInkCanvas = "InkCanvas";

		private InkPresenter inkPresenter;

		static SignaturePadCanvasView ()
		{
			DefaultStyleKeyProperty.OverrideMetadata (typeof (SignaturePadCanvasView), new FrameworkPropertyMetadata (typeof (SignaturePadCanvasView)));

			StrokeColorProperty = DependencyProperty.Register (
				nameof (StrokeColor),
				typeof (System.Windows.Media.Color),
				typeof (SignaturePadCanvasView),
				new PropertyMetadata (ImageConstructionSettings.DefaultStrokeColor, OnStrokePropertiesChanged));

			StrokeWidthProperty = DependencyProperty.Register (
				nameof (StrokeWidth),
				typeof (double),
				typeof (SignaturePadCanvasView),
				new PropertyMetadata ((double)ImageConstructionSettings.DefaultStrokeWidth, OnStrokePropertiesChanged));
		}

		public SignaturePadCanvasView ()
		{
			DefaultStyleKey = typeof (SignaturePadCanvasView);
			OnApplyTemplate();
		}

		/// <inheritdoc />
		public override void OnApplyTemplate ()
		{
			base.OnApplyTemplate();
			var canvas = new InkCanvas();
			canvas.Width = 200;
			canvas.Height = 200;
			canvas.Background=new SolidColorBrush(Colors.Red);
			canvas.Strokes = new InkPresenter();
			inkPresenter = canvas.Strokes as InkPresenter;
			inkPresenter.StrokesChanged += (sender, e) => OnStrokeCompleted ();
			OnStrokePropertiesChanged (this, new DependencyPropertyChangedEventArgs (StrokeColorProperty, "", ""));
		}


		private InkCanvas InkCanvas => GetTemplateChild (PartInkCanvas) as InkCanvas;

		public System.Windows.Media.Color StrokeColor
		{
			get { return (System.Windows.Media.Color)GetValue (StrokeColorProperty); }
			set { SetValue (StrokeColorProperty, value); }
		}

		public double StrokeWidth
		{
			get { return (double)GetValue (StrokeWidthProperty); }
			set { SetValue (StrokeWidthProperty, value); }
		}

		public void Clear ()
		{
			if (inkPresenter != null)
			{
				inkPresenter.Clear ();

				OnCleared ();
			}
		}

		private Task<Stream> GetImageStreamInternal (SignatureImageFormat format, System.Drawing.SizeF scale, RectangleF signatureBounds, System.Drawing.SizeF imageSize, float strokeWidth, System.Windows.Media.Color strokeColor, System.Windows.Media.Color backgroundColor)
		{

				return null;
			
		}

		private Bitmap GetImageInternal (System.Drawing.SizeF scale, System.Drawing.RectangleF signatureBounds,
			System.Drawing.SizeF imageSize, float strokeWidth, System.Windows.Media.Color strokeColor, System.Windows.Media.Color backgroundColor)
		{
			return new Bitmap ("");
		}

		private static void OnStrokePropertiesChanged (DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var signaturePad = d as SignaturePadCanvasView;

			var inkPresenter = signaturePad?.inkPresenter;
			if (inkPresenter != null)
			{
				foreach (var stroke in inkPresenter)
				{
					stroke.DrawingAttributes.Color = signaturePad.StrokeColor;
					stroke.DrawingAttributes.Width = signaturePad.StrokeWidth;
				}
			}
		}
	}
}
