using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Graphics;

namespace SignaturePad
{
	internal class SignatureCanvasView : View
	{
		private Path currentPath;
		private Paint currentPaint;

		public SignatureCanvasView (Context context) :
			base (context)
		{
			Initialize ();
		}

		public SignatureCanvasView (Context context, IAttributeSet attrs) :
			base (context, attrs)
		{
			Initialize ();
		}

		public SignatureCanvasView (Context context, IAttributeSet attrs, int defStyle) :
			base (context, attrs, defStyle)
		{
			Initialize ();
		}

		void Initialize ()
		{
		}

		public Path Path {
			set { this.currentPath = value; }
		}

		public Paint Paint {
			set { this.currentPaint = value; }
		}

		protected override void OnDraw (Canvas canvas)
		{
			if (this.currentPath == null || this.currentPath.IsEmpty)
				return;

			canvas.DrawPath (this.currentPath, this.currentPaint);
		}
	}
}

