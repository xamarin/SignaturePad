using System.IO;
using System.Threading.Tasks;

namespace Xamarin.Controls
{
	using System;
	using System.Drawing;
	using System.Windows;
	using Cairo;
	using Gtk;

	public partial class SignaturePadCanvasView : DrawingArea
	{
		delegate void DrawShape (Cairo.Context ctx, PointD start, PointD end);

		public static readonly DependencyProperty StrokeColorProperty;
		public static readonly DependencyProperty StrokeWidthProperty;
		ImageSurface surface;
		DrawShape Painter;
		PointD Start, End;

		bool isDrawing;
		bool isDrawingPoint;

		static SignaturePadCanvasView ()
		{
			StrokeColorProperty = DependencyProperty.Register (
				nameof (StrokeColor),
				typeof (Gdk.Color),
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
			//DefaultStyleKey = typeof (SignaturePadCanvasView);
			surface = new ImageSurface (Format.Argb32, 500, 500);

			OnStrokePropertiesChanged (dependencyObject, new DependencyPropertyChangedEventArgs (StrokeColorProperty, "", ""));
			inkPresenter.AddEvents (
				(int)Gdk.EventMask.PointerMotionMask
				| (int)Gdk.EventMask.ButtonPressMask
				| (int)Gdk.EventMask.ButtonReleaseMask);

			inkPresenter.ExposeEvent += OnDrawingAreaExposed;
			inkPresenter.ButtonPressEvent += OnMousePress;
			inkPresenter.ButtonReleaseEvent += OnMouseRelease;
			inkPresenter.MotionNotifyEvent += OnMouseMotion;
		}

		public Gdk.Color StrokeColor
		{
			get { return (Gdk.Color)dependencyObject.GetValue (StrokeColorProperty); }
			set { dependencyObject.SetValue (StrokeColorProperty, value); }
		}

		public double StrokeWidth
		{
			get { return (double)dependencyObject.GetValue (StrokeWidthProperty); }
			set { dependencyObject.SetValue (StrokeWidthProperty, value); }
		}

		public DrawingArea inkPresenter { get ; set ; }
		public DependencyObject dependencyObject { get ; set ; }

		public void Clear ()
		{
			if (Strokes != null)
			{
				//inkPresenter.Strokes.Clear();
				OnCleared ();
			}
		}

		private Task<Stream> GetImageStreamInternal (SignatureImageFormat format, System.Drawing.SizeF scale, RectangleF signatureBounds, System.Drawing.SizeF imageSize, float strokeWidth, Gdk.Color strokeColor, Gdk.Color backgroundColor)
		{

			return Task.FromResult<Stream>(null);

		}

		private Bitmap GetImageInternal (System.Drawing.SizeF scale, System.Drawing.RectangleF signatureBounds,
			System.Drawing.SizeF imageSize, float strokeWidth, Gdk.Color strokeColor, Gdk.Color backgroundColor)
		{
			return new Bitmap ("");
		}

		private static void OnStrokePropertiesChanged (DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			//var signaturePad = d as SignaturePadCanvasView;

			//var inkPresenter = signaturePad?.Strokes;
			//if (inkPresenter != null)
			//{
			//	foreach (var stroke in inkPresenter)
			//	{
			//		stroke.DrawingAttributes.Color = signaturePad.StrokeColor;
			//		stroke.DrawingAttributes.Width = signaturePad.StrokeWidth;
			//	}
			//}
		}

		void OnDrawingAreaExposed (object source, ExposeEventArgs args)
		{
			Cairo.Context ctx;

			using (ctx = Gdk.CairoHelper.Create (inkPresenter.GdkWindow))
			{
				ctx.SetSource(new SurfacePattern (surface));
				ctx.Paint ();
			}

			if (isDrawing)
			{
				using (ctx = Gdk.CairoHelper.Create (inkPresenter.GdkWindow))
				{
					Painter (ctx, Start, End);
				}
			}
		}

		void OnMousePress (object source, ButtonPressEventArgs args)
		{
			Start.X = args.Event.X;
			Start.Y = args.Event.Y;

			End.X = args.Event.X;
			End.Y = args.Event.Y;

			isDrawing = true;
			inkPresenter.QueueDraw ();
		}

		void OnMouseRelease (object source, ButtonReleaseEventArgs args)
		{
			End.X = args.Event.X;
			End.Y = args.Event.Y;

			isDrawing = false;

			using (Context ctx = new Context (surface))
			{
				Painter (ctx, Start, End);
			}

			inkPresenter.QueueDraw ();
			OnStrokeCompleted ();
		}

		void OnMouseMotion (object source, MotionNotifyEventArgs args)
		{
			if (isDrawing)
			{
				End.X = args.Event.X;
				End.Y = args.Event.Y;

				if (isDrawingPoint)
				{
					using (Context ctx = new Context (surface))
					{
						Painter (ctx, Start, End);
					}
				}

				inkPresenter.QueueDraw ();
			}
		}

		void LineClicked (object sender, EventArgs args)
		{
			isDrawingPoint = false;
			Painter = new DrawShape (DrawLine);
		}

		void PenClicked (object sender, EventArgs args)
		{
			isDrawingPoint = true;
			Painter = new DrawShape (DrawPoint);
		}

		void DrawLine (Cairo.Context ctx, PointD start, PointD end)
		{
			ctx.MoveTo (start);
			ctx.LineTo (end);
			ctx.Stroke ();
		}

		void DrawPoint (Cairo.Context ctx, PointD start, PointD end)
		{
			ctx.Rectangle (end, 1, 1);
			ctx.Stroke ();
		}
	}
}
