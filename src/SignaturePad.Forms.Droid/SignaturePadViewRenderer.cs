using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer (typeof (SignaturePad.Forms.SignaturePadView), typeof (SignaturePad.Forms.SignaturePadViewRenderer))]

namespace SignaturePad.Forms
{
	// TODO: Remove this whole logic once Xamarin.Forms v2.4 is released.
	//       This is a fix for issue #94
	//       https://github.com/xamarin/SignaturePad/issues/94

	[RenderWith (typeof (SignaturePadViewRenderer))]
	partial class SignaturePadView
	{
	}

	internal class SignaturePadViewRenderer : VisualElementRenderer<Grid>
	{
		public override bool OnInterceptTouchEvent (Android.Views.MotionEvent ev)
		{
			if (!Enabled || Element?.IsEnabled == false)
				return true;

			return base.OnInterceptTouchEvent (ev);
		}
	}
}
