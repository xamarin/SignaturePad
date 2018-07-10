using System.Collections.Generic;
using CoreGraphics;
using AppKit;

namespace Xamarin.Controls
{

	partial class InkPresenter : NSView
	{
		static InkPresenter ()
		{
			ScreenDensity = (float)NSScreen.MainScreen.BackingScaleFactor;
		}

		public InkPresenter ()
			: base ()
		{
			Initialize ();
		}

		public InkPresenter (CGRect frame)
			: base (frame)
		{
			Initialize ();
		}

		private void Initialize ()
		{
			//Opaque = false;
		}

		// If you put SignaturePad inside a ScrollView, this line of code prevent that the gesture inside 
		// an InkPresenter are dispatched to the ScrollView below
		//public override bool GestureRecognizerShouldBegin (NSGestureRecognizer gestureRecognizer) => false;

		public override void MouseDown (NSEvent evt)
		{
			// create a new path and set the options
			currentPath = new InkStroke (new CGPath(), new List<CGPoint> (), StrokeColor, StrokeWidth);

			// obtain the location of the touch
			var touchLocation = evt.LocationInWindow;

			// move the path to that position
			currentPath.Path.MoveTo (touchLocation.X, touchLocation.Y);
			currentPath.GetPoints ().Add (touchLocation);

			// update the dirty rectangle
			ResetBounds (touchLocation);
		}

		public override void MouseDragged (NSEvent evt)
		{
			// something may have happened (clear) so start the stroke again
			if (currentPath == null)
			{
				TouchesBeganWithEvent(evt);
			}

			// obtain the location of the touch

			var touchLocation = evt.LocationInWindow;
			if (HasMovedFarEnough (currentPath, touchLocation.X, touchLocation.Y))
			{
				// add it to the current path
				currentPath.Path.LineTo (touchLocation.X, touchLocation.Y);
				currentPath.GetPoints ().Add (touchLocation);

				// update the dirty rectangle
				UpdateBounds (touchLocation);
				SetNeedsDisplayInRect (DirtyRect);
			}
		}

		public override void MouseUp (NSEvent evt)
		{
			// obtain the location of the touch
			var touchLocation = evt.LocationInWindow;

			// something may have happened (clear) during the stroke
			if (currentPath != null)
			{
				if (HasMovedFarEnough (currentPath, touchLocation.X, touchLocation.Y))
				{
					// add it to the current path
					currentPath.Path.LineTo (touchLocation.X, touchLocation.Y);
					currentPath.GetPoints ().Add (touchLocation);
				}

				// obtain the smoothed path, and add it to the old paths
				var smoothed = PathSmoothing.SmoothedPathWithGranularity (currentPath, 4);
				paths.Add (smoothed);
			}

			// clear the current path
			//currentPath = null;

			// update the dirty rectangle
			UpdateBounds (touchLocation);
			NeedsDisplay = true;

			// we are done with drawing
			OnStrokeCompleted ();
		}

		

		public override void DrawRect (CGRect rect)
		{
			base.DrawRect (rect);

			// destroy an old bitmap
			if (bitmapBuffer != null && ShouldRedrawBufferImage)
			{
				var temp = bitmapBuffer;
				bitmapBuffer = null;

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
				bitmapBuffer.Draw (CGPoint.Empty, rect, NSCompositingOperation.Screen, 0);
			}

			// draw the current path over the old paths
			if (currentPath != null)
			{
				var context = NSGraphicsContext.CurrentContext.CGContext;
				context.SetLineCap (CGLineCap.Round);
				context.SetLineJoin (CGLineJoin.Round);
				context.SetStrokeColor (currentPath.Color.CGColor);
				context.SetLineWidth (currentPath.Width);

				context.AddPath (currentPath.Path);
				context.StrokePath ();
			}
		}

		private NSImage CreateBufferImage ()
		{
			if (paths == null || paths.Count == 0)
			{
				return null;
			}

			var size = Bounds.Size;
			
			return null;
		}

		public override void Layout ()
		{
			base.Layout ();

			NeedsDisplay = true;
		}
	}
}
