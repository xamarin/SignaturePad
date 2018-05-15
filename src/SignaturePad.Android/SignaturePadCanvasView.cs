using System.IO;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Xamarin.Controls
{
	public partial class SignaturePadCanvasView : FrameLayout
	{
		private InkPresenter inkPresenter;

		public SignaturePadCanvasView (Context context)
			: base (context)
		{
			Initialize ();
		}

		public SignaturePadCanvasView (Context context, IAttributeSet attrs)
			: base (context, attrs)
		{
			Initialize ();
		}

		public SignaturePadCanvasView (Context context, IAttributeSet attrs, int defStyle)
			: base (context, attrs, defStyle)
		{
			Initialize ();
		}

		private void Initialize ()
		{
			inkPresenter = new InkPresenter (Context)
			{
				LayoutParameters = new FrameLayout.LayoutParams (FrameLayout.LayoutParams.MatchParent, FrameLayout.LayoutParams.MatchParent)
			};
			inkPresenter.StrokeCompleted += OnStrokeCompleted;
			AddView (inkPresenter);

			StrokeWidth = ImageConstructionSettings.DefaultStrokeWidth;
			StrokeColor = ImageConstructionSettings.DefaultStrokeColor;
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

			OnCleared ();
		}

		private Bitmap GetImageInternal (System.Drawing.SizeF scale, System.Drawing.RectangleF signatureBounds, System.Drawing.SizeF imageSize, float strokeWidth, Color strokeColor, Color backgroundColor)
		{
			// create bitmap and set the desired options
			var image = Bitmap.CreateBitmap ((int)imageSize.Width, (int)imageSize.Height, Bitmap.Config.Argb8888);
			using (var canvas = new Canvas (image))
			{
				// background
				canvas.DrawColor (backgroundColor);

				// cropping / scaling
				canvas.Scale (scale.Width, scale.Height);
				canvas.Translate (-signatureBounds.Left, -signatureBounds.Top);

				// strokes
				using (var paint = new Paint ())
				{
					paint.Color = strokeColor;
					paint.StrokeWidth = strokeWidth * InkPresenter.ScreenDensity;
					paint.StrokeJoin = Paint.Join.Round;
					paint.StrokeCap = Paint.Cap.Round;
					paint.AntiAlias = true;
					paint.SetStyle (Paint.Style.Stroke);

					foreach (var path in inkPresenter.GetStrokes ())
					{
						canvas.DrawPath (path.Path, paint);
					}
				}
			}

			// get the image
			return image;
		}

		private async Task<Stream> GetImageStreamInternal (SignatureImageFormat format, System.Drawing.SizeF scale, System.Drawing.RectangleF signatureBounds, System.Drawing.SizeF imageSize, float strokeWidth, Color strokeColor, Color backgroundColor)
		{
			Bitmap.CompressFormat bcf;
			if (format == SignatureImageFormat.Jpeg)
			{
				bcf = Bitmap.CompressFormat.Jpeg;
			}
			else if (format == SignatureImageFormat.Png)
			{
				bcf = Bitmap.CompressFormat.Png;
			}
			else
			{
				return null;
			}

			var image = GetImageInternal (scale, signatureBounds, imageSize, strokeWidth, strokeColor, backgroundColor);
			if (image != null)
			{
				using (image)
				{
					var stream = new MemoryStream ();
					var result = await image.CompressAsync (bcf, 100, stream);

					image.Recycle ();

					if (result)
					{
						stream.Position = 0;
						return stream;
					}
				}
			}

			return null;
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
