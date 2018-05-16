using System.IO;
using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace Samples.Droid
{
	[Activity (MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : FormsAppCompatActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			// set the layout resources first
			ToolbarResource = Resource.Layout.toolbar;
			TabLayoutResource = Resource.Layout.tabs;

			// then call base.OnCreate and the Xamarin.Forms methods
			base.OnCreate (bundle);
			Forms.Init (this, bundle);
			LoadApplication (new App (OnSaveSignature));
		}

		private async Task<bool> OnSaveSignature (Stream bitmap, string filename)
		{
			var path = Environment.GetExternalStoragePublicDirectory (Environment.DirectoryPictures).AbsolutePath;
			var file = Path.Combine (path, "signature.png");

			using (var dest = File.OpenWrite (file))
			{
				await bitmap.CopyToAsync (dest);
			}

			return true;
		}
	}
}
