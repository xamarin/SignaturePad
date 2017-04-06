using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using AlertDialog = Android.Support.V7.App.AlertDialog;

using Xamarin.Controls;

namespace Sample.Android
{
	[Activity (Label = "@string/app_name", MainLauncher = true, Theme = "@style/Theme.AppCompat.Light.DarkActionBar")]
	public class MainActivity : AppCompatActivity
	{
		private const bool EnableCustomization = true;

		private System.Drawing.PointF[] points;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			var signature = FindViewById<SignaturePadView> (Resource.Id.signatureView);

			if (EnableCustomization)
			{
				var root = FindViewById<View> (Resource.Id.rootView);

				signature.Caption.Text = "Authorization Signature";
				signature.Caption.SetTypeface (Typeface.Serif, TypefaceStyle.BoldItalic);
				signature.Caption.SetTextSize (global::Android.Util.ComplexUnitType.Sp, 16f);
				signature.SignaturePrompt.Text = ">>";
				signature.SignaturePrompt.SetTypeface (Typeface.SansSerif, TypefaceStyle.Normal);
				signature.SignaturePrompt.SetTextSize (global::Android.Util.ComplexUnitType.Sp, 32f);
				signature.BackgroundColor = Color.Rgb (255, 255, 200); // a light yellow.
				signature.StrokeColor = Color.Black;

				signature.BackgroundImageView.SetImageResource (Resource.Drawable.logo_galaxy_black_64);
				signature.BackgroundImageView.SetAlpha (16);
				signature.BackgroundImageView.SetAdjustViewBounds (true);

				var layout = new RelativeLayout.LayoutParams (ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
				layout.AddRule (LayoutRules.CenterInParent);
				layout.SetMargins (20, 20, 20, 20);
				signature.BackgroundImageView.LayoutParameters = layout;

				// You can change paddings for positioning...
				var caption = signature.Caption;
				caption.SetPadding (caption.PaddingLeft, 1, caption.PaddingRight, 25);
			}

			// get our button from the layout resource,
			// and attach an event to it
			var btnSave = FindViewById<Button> (Resource.Id.btnSave);
			btnSave.Click += delegate
			{
				if (signature.IsBlank)
				{
					// display the base line for the user to sign on.
					AlertDialog.Builder alert = new AlertDialog.Builder (this);
					alert.SetMessage ("No signature to save.");
					alert.SetNeutralButton ("Okay", delegate { });
					alert.Create ().Show ();
				}
				points = signature.Points;
			};
			btnSave.Dispose ();

			var btnLoad = FindViewById<Button> (Resource.Id.btnLoad);
			btnLoad.Click += delegate
			{
				if (points != null)
					signature.LoadPoints (points);
			};
			btnLoad.Dispose ();
		}
	}
}
