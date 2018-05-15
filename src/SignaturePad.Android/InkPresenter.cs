using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;

namespace Xamarin.Controls
{
	partial class InkPresenter : View
	{
		static InkPresenter ()
		{
			// we may be in a designer
			if (Application.Context == null)
			{
				ScreenDensity = 1f;
				return;
			}

			var wndMngr = Application.Context.GetSystemService (Context.WindowService).JavaCast<IWindowManager> ();
			var dm = new DisplayMetrics ();
			wndMngr.DefaultDisplay.GetMetrics (dm);
			ScreenDensity = (float)dm.Density;
		}

		public InkPresenter (Context context)
			: base (context)
		{
			Initialize ();
		}

		private void Initialize ()
		{
		}

		public override bool OnTouchEvent (MotionEvent e)
		{
			switch (e.Action)
			{
				case MotionEventActions.Down:
					TouchesBegan (e);
					return true;
				case MotionEventActions.Move:
					TouchesMoved (e);
					return true;
				case MotionEventActions.Up:
					TouchesEnded (e);
					return true;
			}
			return false;
		}

		private void TouchesBegan (MotionEvent e)
		{
			// don't allow the event to propagate because we're handling it here
			Parent?.RequestDisallowInterceptTouchEvent (true);

			// create a new path and set the options
			currentPath = new InkStroke (new Path (), new List<System.Drawing.PointF> (), StrokeColor, StrokeWidth);

			// obtain the location of the touch
			float touchX = e.GetX ();
			float touchY = e.GetY ();

			// move to the touched point
			currentPath.Path.MoveTo (touchX, touchY);
			currentPath.GetPoints ().Add (new System.Drawing.PointF (touchX, touchY));

			// update the dirty rectangle
			ResetBounds (touchX, touchY);
			Invalidate (DirtyRect);
		}

		private void TouchesMoved (MotionEvent e, bool update = true)
		{
			// something may have happened (clear) so start the stroke again
			if (currentPath == null)
			{
				TouchesBegan (e);
			}

			var hasMoved = false;

			for (var i = 0; i < e.HistorySize; i++)
			{
				float historicalX = e.GetHistoricalX (i);
				float historicalY = e.GetHistoricalY (i);

				if (HasMovedFarEnough (currentPath, historicalX, historicalY))
				{
					// update the dirty rectangle
					UpdateBounds (historicalX, historicalY);
					hasMoved = true;

					// add it to the current path
					currentPath.Path.LineTo (historicalX, historicalY);
					currentPath.GetPoints ().Add (new System.Drawing.PointF (historicalX, historicalY));
				}
			}

			float touchX = e.GetX ();
			float touchY = e.GetY ();

			if (HasMovedFarEnough (currentPath, touchX, touchY))
			{
				// add it to the current path
				currentPath.Path.LineTo (touchX, touchY);
				currentPath.GetPoints ().Add (new System.Drawing.PointF (touchX, touchY));

				// update the dirty rectangle
				UpdateBounds (touchX, touchY);
				hasMoved = true;
			}

			if (update && hasMoved)
			{
				Invalidate (DirtyRect);
			}
		}

		private void TouchesEnded (MotionEvent e)
		{
			// something may have happened (clear) during the stroke
			if (currentPath != null)
			{
				TouchesMoved (e, false);

				// add the current path and points to their respective lists.
				var smoothed = PathSmoothing.SmoothedPathWithGranularity (currentPath, 2);
				paths.Add (smoothed);
			}

			// reset the drawing
			currentPath = null;

			// update the dirty rectangle
			Invalidate (DirtyRect);

			// we are done with drawing
			OnStrokeCompleted ();

			// allow the event to propagate
			Parent?.RequestDisallowInterceptTouchEvent (false);
		}

		private void Invalidate (RectangleF dirtyRect)
		{
			using (var rect = new Rect (
				(int)(dirtyRect.Left - 0.5f),
				(int)(dirtyRect.Top - 0.5f),
				(int)(dirtyRect.Right + 0.5f),
				(int)(dirtyRect.Bottom + 0.5f)))
			{
				Invalidate (rect);
			}
		}

		protected override void OnDraw (Canvas canvas)
		{
			base.OnDraw (canvas);

			// destroy an old bitmap
			if (bitmapBuffer != null && ShouldRedrawBufferImage)
			{
				var temp = bitmapBuffer;
				bitmapBuffer = null;

				temp.Recycle ();
				temp.Dispose ();
				temp = null;
			}

			// re-create
			if (bitmapBuffer == null)
			{
				bitmapBuffer = CreateBufferImage ();
			}

			// if there are no lines, the the bitmap will be null
			if (bitmapBuffer != null)
			{
				canvas.DrawBitmap (bitmapBuffer, 0, 0, null);
			}

			// draw the current path over the old paths
			if (currentPath != null)
			{
				using (var paint = new Paint ())
				{
					paint.StrokeJoin = Paint.Join.Round;
					paint.StrokeCap = Paint.Cap.Round;
					paint.AntiAlias = true;
					paint.SetStyle (Paint.Style.Stroke);

					paint.Color = currentPath.Color;
					paint.StrokeWidth = currentPath.Width * ScreenDensity;

					canvas.DrawPath (currentPath.Path, paint);
				}
			}
		}

		private Bitmap CreateBufferImage ()
		{
			if (paths == null || paths.Count == 0)
			{
				return null;
			}

			var size = new SizeF (Width, Height);
			var image = Bitmap.CreateBitmap ((int)size.Width, (int)size.Height, Bitmap.Config.Argb8888);

			using (var canvas = new Canvas (image))
			using (var paint = new Paint ())
			{
				paint.StrokeJoin = Paint.Join.Round;
				paint.StrokeCap = Paint.Cap.Round;
				paint.AntiAlias = true;
				paint.SetStyle (Paint.Style.Stroke);

				foreach (var path in paths)
				{
					paint.Color = path.Color;
					paint.StrokeWidth = path.Width * ScreenDensity;

					canvas.DrawPath (path.Path, paint);

					path.IsDirty = false;
				}
			}

			return image;
		}
	}
}
