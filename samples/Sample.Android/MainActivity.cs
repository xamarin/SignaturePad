using System.IO;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;

using Xamarin.Controls;

namespace Sample.Android
{
	[Activity (MainLauncher = true)]
	public class MainActivity : AppCompatActivity
	{
		private System.Drawing.PointF[] points;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.main);

			var signatureView = FindViewById<SignaturePadView> (Resource.Id.signatureView);

			var btnSave = FindViewById<Button> (Resource.Id.btnSave);
			var btnLoad = FindViewById<Button> (Resource.Id.btnLoad);
			var btnSaveImage = FindViewById<Button> (Resource.Id.btnSaveImage);

			btnSave.Click += delegate
			{
				points = signatureView.Points;

				Toast.MakeText (this, "Vector signature saved to memory.", ToastLength.Short).Show ();
			};

			btnLoad.Click += delegate
			{
				if (points != null)
					signatureView.LoadPoints (points);
			};

			btnSaveImage.Click += async delegate
			{
				var path = Environment.GetExternalStoragePublicDirectory (Environment.DirectoryPictures).AbsolutePath;
				var file = System.IO.Path.Combine (path, "signature.png");

				using (var bitmap = await signatureView.GetImageStreamAsync (SignatureImageFormat.Png, Color.Black, Color.White, 1f))
				using (var dest = File.OpenWrite (file))
				{
					await bitmap.CopyToAsync (dest);
				}

				Toast.MakeText (this, "Raster signature saved to the photo gallery.", ToastLength.Short).Show ();
			};
		}
	}
}
