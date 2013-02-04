
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;
using Android.Graphics;

namespace SignaturePad {

	public class ClearingImageView : ImageView {

		private Bitmap imageBitmap = null; 

		public ClearingImageView (Context context)
			: base (context)
		{
		}

		public ClearingImageView (Context context, IAttributeSet attrs)
			: base (context, attrs)
		{
		}

		public ClearingImageView (Context context, IAttributeSet attrs, int defStyle)
			: base (context, attrs, defStyle)
		{
		}

		public override void SetImageBitmap(Bitmap bm)
		{
			base.SetImageBitmap (bm);
			if (imageBitmap != null)
			{
				imageBitmap.Recycle ();
			}
			imageBitmap = bm;
		}
	}
}

