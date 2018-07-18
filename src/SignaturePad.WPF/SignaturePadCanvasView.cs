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

	public partial class SignaturePadCanvasView : InkCanvas
	{
		public static readonly DependencyProperty StrokeColorProperty;
		public static readonly DependencyProperty StrokeWidthProperty;

		InkCanvas inkPresenter => this;
		static SignaturePadCanvasView ()
		{
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
			inkPresenter.Strokes.StrokesChanged += (sender, e) => OnStrokeCompleted ();
			OnStrokePropertiesChanged (this, new DependencyPropertyChangedEventArgs (StrokeColorProperty, "", ""));
		}

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
			if (Strokes != null)
			{
				inkPresenter.Strokes.Clear();
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

			var inkPresenter = signaturePad?.Strokes;
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
