using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Xamarin.Controls
{
	public partial class SignaturePadCanvasView : Grid
	{
		private Color strokeColor;
		private float lineWidth;

		private InkPresenter inkPresenter;

		public SignaturePadCanvasView ()
		{
			Initialize ();
		}

		private void Initialize ()
		{
			inkPresenter = new InkPresenter ();
			inkPresenter.ClipToBounds = true;
			inkPresenter.StrokeCompleted += OnStrokeCompleted;
			Children.Add (inkPresenter);

			// get some defaults
			var settings = new ImageConstructionSettings ();
			settings.ApplyDefaults ();

			StrokeWidth = settings.StrokeWidth.Value;
			StrokeColor = settings.StrokeColor.Value;
		}

		/// <summary>
		/// Gets or sets the color of the strokes for the signature.
		/// </summary>
		/// <value>The color of the stroke.</value>
		public Color StrokeColor
		{
			get { return inkPresenter.StrokeColor; }
			set
			{
				inkPresenter.StrokeColor = value;
				foreach (var stroke in inkPresenter.GetStrokes ())
				{
					stroke.Color = value;
				}
				inkPresenter.Invalidate ();
			}
		}

		/// <summary>
		/// Gets or sets the width in pixels of the strokes for the signature.
		/// </summary>
		/// <value>The width of the line.</value>
		public float StrokeWidth
		{
			get { return inkPresenter.StrokeWidth; }
			set
			{
				inkPresenter.StrokeWidth = value;
				foreach (var stroke in inkPresenter.GetStrokes ())
				{
					stroke.Width = value;
				}
				inkPresenter.Invalidate ();
			}
		}

		public void Clear ()
		{
			inkPresenter.Clear ();
		}

		private async Task<Stream> GetImageStreamInternal (SignatureImageFormat format, Size scale, Rect signatureBounds, Size imageSize, float strokeWidth, Color strokeColor, Color backgroundColor)
		{
			throw new NotImplementedException ();
		}

		private WriteableBitmap GetImageInternal (Size scale, Rect signatureBounds, Size imageSize, float strokeWidth, Color strokeColor, Color backgroundColor)
		{
			throw new NotImplementedException ();
		}
	}
}
